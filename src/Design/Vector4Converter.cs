#region License
/* FNA - XNA4 Reimplementation for Desktop Platforms
 * Copyright 2009-2024 Ethan Lee and the MonoGame Team
 *
 * Released under the Microsoft Public License.
 * See LICENSE for details.
 */
#endregion

#region Using Statements
using System;
using System.Collections;
using System.ComponentModel;
using System.Globalization;
using System.Numerics;

#endregion

namespace Microsoft.Xna.Framework.Design
{
	public class Vector4Converter : MathTypeConverter
	{
		#region Public Constructor

		public Vector4Converter() : base()
		{
			// FIXME: Initialize propertyDescriptions... how? -flibit
		}

		#endregion

		#region Public Methods

		public override object ConvertFrom(
			ITypeDescriptorContext context,
			CultureInfo culture,
			object value
		) {
			string s = value as string;
			if (s != null)
			{
				string[] v = s.Split(
					culture.TextInfo.ListSeparator.ToCharArray()
				);
				return new Vector4(
					float.Parse(v[0], culture),
					float.Parse(v[1], culture),
					float.Parse(v[2], culture),
					float.Parse(v[3], culture)
				);
			}
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(
			ITypeDescriptorContext context,
			CultureInfo culture,
			object value,
			Type destinationType
		) {
			if (destinationType == typeof(string))
			{
				Vector4 vec = (Vector4) value;
				return string.Join(
					culture.TextInfo.ListSeparator + " ",
					new string[]
					{
						vec.X.ToString(culture),
						vec.Y.ToString(culture),
						vec.Z.ToString(culture),
						vec.W.ToString(culture)
					}
				);
			}
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object CreateInstance(
			ITypeDescriptorContext context,
			IDictionary propertyValues
		) {
			return (object) new Vector4(
				(float) propertyValues["X"],
				(float) propertyValues["Y"],
				(float) propertyValues["Z"],
				(float) propertyValues["W"]
			);
		}

		#endregion
	}
}
