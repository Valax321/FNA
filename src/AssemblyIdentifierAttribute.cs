using System;

namespace Microsoft.Xna.Framework
{
	[AttributeUsage(AttributeTargets.Assembly)]
	public sealed class AssemblyIdentifierAttribute : Attribute
	{
		public string Identifier { get; }

		public AssemblyIdentifierAttribute(string identifier)
		{
			Identifier = identifier;
		}
	}
}
