using System;
using System.Reflection;

namespace Microsoft.Xna.Framework
{
	public static class ApplicationInfo
	{
		public static string Product { get; } = string.Empty;
		public static string Company { get; } = string.Empty;
		public static string VersionString { get; } = string.Empty;
		public static Version Version { get; } = new();
		public static string Copyright { get; } = string.Empty;
		public static string AppIdentifier { get; } = string.Empty;

		static ApplicationInfo()
		{
			var assembly = Assembly.GetEntryAssembly();
			if (assembly != null)
			{
				var product = assembly.GetCustomAttribute<AssemblyProductAttribute>();
				var company = assembly.GetCustomAttribute<AssemblyCompanyAttribute>();
				var version = assembly.GetCustomAttribute<AssemblyVersionAttribute>();
				var copyright = assembly.GetCustomAttribute<AssemblyCopyrightAttribute>();
				var id = assembly.GetCustomAttribute<AssemblyIdentifierAttribute>();

				var versionNum = assembly.GetName().Version;
				if (versionNum != null)
					Version = versionNum;

				if (product != null)
					Product = product.Product;

				if (company != null)
					Company = company.Company;

				if (version != null)
					VersionString = version.Version;

				if (copyright != null)
					Copyright = copyright.Copyright;

				if (id != null)
					AppIdentifier = id.Identifier;
			}
		}
	}
}
