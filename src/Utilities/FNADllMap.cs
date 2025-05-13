#region License
/* FNA - XNA4 Reimplementation for Desktop Platforms
 * Copyright 2009-2024 Ethan Lee and the MonoGame Team
 *
 * Released under the Microsoft Public License.
 * See LICENSE for details.
 */
#endregion

#if NET7_0_OR_GREATER

#region Using Statements
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;
using System.Xml;
#endregion

namespace Microsoft.Xna.Framework
{
	internal static class FNADllMap
	{
		#region Private Static Variables

		private static Dictionary<string, string> mapDictionary
			= new Dictionary<string, string>();

		private static List<string> searchPaths
			= new List<string>();

		#endregion

		#region Private Static Methods

		private static string GetPlatformName()
		{
			if (OperatingSystem.IsWindows())
			{
				return "windows";
			}
			else if (OperatingSystem.IsMacOS())
			{
				return  "osx";
			}
			else if (OperatingSystem.IsLinux())
			{
				return "linux";
			}
			else if (OperatingSystem.IsFreeBSD())
			{
				return "freebsd";
			}
			else
			{
				// What is this platform??
				return "unknown";
			}
		}

		#endregion

		#region DllImportResolver Callback Methods

		private static IntPtr MapAndLoad(
			string libraryName,
			Assembly assembly,
			DllImportSearchPath? dllImportSearchPath
		) {
			string mappedName;
			if (!mapDictionary.TryGetValue(libraryName, out mappedName))
			{
				mappedName = libraryName;
			}

			foreach (string path in searchPaths)
			{
				string fullPath = Path.Combine(path, mappedName);
				if (NativeLibrary.TryLoad(fullPath, assembly, dllImportSearchPath, out IntPtr handle))
					return handle;
			}

			return NativeLibrary.Load(mappedName, assembly, dllImportSearchPath);
		}

		private static IntPtr LoadStaticLibrary(
			string libraryName,
			Assembly assembly,
			DllImportSearchPath? dllImportSearchPath
		) {
			return NativeLibrary.GetMainProgramHandle();
		}

		#endregion

		#region Module Initializer

#pragma warning disable CA2255
		[ModuleInitializer]
#pragma warning restore CA2255
		public static void Init()
		{
			/*
			 * NOTE: this will return false if PublishAOT is defined, even if we're still running a JIT build!
			 * Kinda weird!
			 */
			if (!RuntimeFeature.IsDynamicCodeCompiled)
			{
				/* NativeAOT platforms don't perform dynamic loading,
				 * so setting a DllImportResolver is unnecessary.
				 *
				 * However, iOS and tvOS with Mono AOT statically link
				 * their dependencies, so we need special handling for them.
				 */
				if (OperatingSystem.IsIOS() || OperatingSystem.IsTvOS())
				{
					NativeLibrary.SetDllImportResolver(Assembly.GetExecutingAssembly(), LoadStaticLibrary);
				}

				return;
			}

			// Get the platform and architecture
			string os = GetPlatformName();
			string cpu = RuntimeInformation.ProcessArchitecture.ToString().ToLowerInvariant();
			string wordsize = (IntPtr.Size * 8).ToString();

			// Get the executing assembly
			Assembly assembly = Assembly.GetExecutingAssembly();

			// Locate the config file
			string xmlPath = Path.Combine(
				AppContext.BaseDirectory,
				assembly.GetName().Name + ".dll.config"
			);
			if (!File.Exists(xmlPath))
			{
				// Let's hope for the best...
				return;
			}

			// Load the XML
			XmlDocument xmlDoc = new XmlDocument();
			xmlDoc.Load(xmlPath);

			// The NativeLibrary API cannot remap function names. :(
			if (xmlDoc.GetElementsByTagName("dllentry").Count > 0)
			{
				string msg = "Function remapping is not supported by .NET Core. Ignoring dllentry elements...";
				Console.WriteLine(msg);

				// Log it in the debugger for non-console apps.
				if (Debugger.IsAttached)
				{
					Debug.WriteLine(msg);
				}
			}

			// Parse the XML into a mapping dictionary
			foreach (XmlNode node in xmlDoc.GetElementsByTagName("dllmap"))
			{
				XmlAttribute attribute;

				// Check the OS
				attribute = node.Attributes["os"];
				if (attribute != null)
				{
					bool containsOS = attribute.Value.Contains(os);
					bool invert = attribute.Value.StartsWith("!");
					if ((!containsOS && !invert) || (containsOS && invert))
					{
						continue;
					}
				}

				// Check the CPU
				attribute = node.Attributes["cpu"];
				if (attribute != null)
				{
					bool containsCPU = attribute.Value.Contains(cpu);
					bool invert = attribute.Value.StartsWith("!");
					if ((!containsCPU && !invert) || (containsCPU && invert))
					{
						continue;
					}
				}

				// Check the word size
				attribute = node.Attributes["wordsize"];
				if (attribute != null)
				{
					bool containsWordsize = attribute.Value.Contains(wordsize);
					bool invert = attribute.Value.StartsWith("!");
					if ((!containsWordsize && !invert) || (containsWordsize && invert))
					{
						continue;
					}
				}

				// Check for the existence of 'dll' and 'target' attributes
				XmlAttribute dllAttribute = node.Attributes["dll"];
				XmlAttribute targetAttribute = node.Attributes["target"];
				if (dllAttribute == null || targetAttribute == null)
				{
					continue;
				}

				// Get the actual library names
				string oldLib = dllAttribute.Value;
				string newLib = targetAttribute.Value;
				if (string.IsNullOrWhiteSpace(oldLib) || string.IsNullOrWhiteSpace(newLib))
				{
					continue;
				}

				// Don't allow duplicates
				if (mapDictionary.ContainsKey(oldLib))
				{
					continue;
				}

				mapDictionary.Add(oldLib, newLib);
			}

			// This is where dotnet puts native libs from nuget by default
			searchPaths.Add(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "runtimes", GetRidForPlatform(), "native"));

			if (Environment.GetEnvironmentVariable("FNA_USE_DLLMAP_SEARCH_PATHS")?.Equals("1") ?? false)
			{
				foreach (XmlNode node in xmlDoc.GetElementsByTagName("searchpath"))
				{
					XmlAttribute path = node.Attributes["path"];
					if (path != null)
					{
						var pathString = path.Value;
						pathString = pathString.Replace("{rid}", GetRidForPlatform());
						if (!Path.IsPathFullyQualified(pathString))
						{
							pathString = Path.Combine(Directory.GetCurrentDirectory(), pathString);
						}

						searchPaths.Add(pathString);
					}
				}
			}

			// Set the resolver callback
			NativeLibrary.SetDllImportResolver(assembly, MapAndLoad);
		}

		private static string GetRidForPlatform()
		{
			/*
			 * I could have used RuntimeInformation.RuntimeEnvironment to get this
			 * but msdn recommends not parsing the value of that (presumably the format
			 * can change in future). So instead we get this limited list that only supports
			 * the platforms I will ever ship on.
			 */

			var arch = RuntimeInformation.ProcessArchitecture switch
			{
				Architecture.Arm => "arm",
				Architecture.Arm64 => "arm64",
				Architecture.X86 => "x86",
				Architecture.X64 => "x64",
				Architecture.LoongArch64 => "loongarch64",
				_ => string.Empty // We dont really care about the others
			};

			var rid = string.Empty;
			if (OperatingSystem.IsWindows())
				rid = $"win-{arch}";
			else if (OperatingSystem.IsMacOS())
				rid = "osx"; // On mac we use universal libs so we don't need to differentiate arm and intel
			else if (OperatingSystem.IsLinux())
				rid = $"linux-{arch}";
			else if (OperatingSystem.IsAndroid())
				rid = $"android-{arch}";
			// You're on your own for the rest

			return rid;
		}

		#endregion
	}
}

#endif // NET7_0_OR_GREATER
