#region License
/* FNA - XNA4 Reimplementation for Desktop Platforms
 * Copyright 2009-2024 Ethan Lee and the MonoGame Team
 *
 * Released under the Microsoft Public License.
 * See LICENSE for details.
 */
#endregion

using System.Diagnostics.CodeAnalysis;

namespace Microsoft.Xna.Framework.Graphics
{
	/// <summary>
	/// Helper class which ensures we only lookup a vertex
	/// declaration for a particular type once.
	/// </summary>
	/// <typeparam name="T">A vertex structure which implements IVertexType.</typeparam>
	internal static class VertexDeclarationCache<[DynamicallyAccessedMembers(DynamicallyAccessedMemberTypes.PublicParameterlessConstructor)] T>
		where T : struct, IVertexType
	{
		#region Public Static Properties

		public static VertexDeclaration VertexDeclaration
		{
			get
			{
				if (cached == null)
				{
					cached = VertexDeclaration.FromType(typeof(T));
				}

				return cached;
			}
		}

		#endregion

		#region Private Static Variables

		private static VertexDeclaration cached;

		#endregion
	}
}
