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
	public class BoundingSphereConverter : MathTypeConverter
	{
		#region Public Constructor

		public BoundingSphereConverter() : base()
		{
			// FIXME: Initialize propertyDescriptions... how? -flibit
			supportStringConvert = false;
		}

		#endregion

		#region Public Methods

		public override object ConvertFrom(
			ITypeDescriptorContext context,
			CultureInfo culture,
			object value
		) {
			// FIXME: This method exists in the spec, but... why?! -flibit
			return base.ConvertFrom(context, culture, value);
		}

		public override object ConvertTo(
			ITypeDescriptorContext context,
			CultureInfo culture,
			object value,
			Type destinationType
		) {
			// FIXME: This method exists in the spec, but... why?! -flibit
			return base.ConvertTo(context, culture, value, destinationType);
		}

		public override object CreateInstance(
			ITypeDescriptorContext context,
			IDictionary propertyValues
		) {
			return (object) new BoundingSphere(
				(Vector3) propertyValues["Center"],
				(float) propertyValues["Radius"]
			);
		}

		#endregion
	}
}
