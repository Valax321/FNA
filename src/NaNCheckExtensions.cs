using System;
using System.Diagnostics;
using System.Numerics;

namespace Microsoft.Xna.Framework
{
	internal static class NaNCheckExtensions
	{
		[Conditional("DEBUG")]
		public static void CheckForNaNs(this Matrix4x4 m)
		{
			if (	float.IsNaN(m.M11) ||
				    float.IsNaN(m.M12) ||
				    float.IsNaN(m.M13) ||
				    float.IsNaN(m.M14) ||
				    float.IsNaN(m.M21) ||
				    float.IsNaN(m.M22) ||
				    float.IsNaN(m.M23) ||
				    float.IsNaN(m.M24) ||
				    float.IsNaN(m.M31) ||
				    float.IsNaN(m.M32) ||
				    float.IsNaN(m.M33) ||
				    float.IsNaN(m.M34) ||
				    float.IsNaN(m.M41) ||
				    float.IsNaN(m.M42) ||
				    float.IsNaN(m.M43) ||
				    float.IsNaN(m.M44)	)
			{
				throw new InvalidOperationException("Matrix contains NaNs!");
			}
		}

		[Conditional("DEBUG")]
		public static void CheckForNaNs(this Quaternion q)
		{
			if (	float.IsNaN(q.X) ||
				    float.IsNaN(q.Y) ||
				    float.IsNaN(q.Z) ||
				    float.IsNaN(q.W)	)
			{
				throw new InvalidOperationException("Quaternion contains NaNs!");
			}
		}

		[Conditional("DEBUG")]
		public static void CheckForNaNs(this Vector3 v)
		{
			if (	float.IsNaN(v.X) ||
				    float.IsNaN(v.Y) ||
				    float.IsNaN(v.Z)	)
			{
				throw new InvalidOperationException("Vector3 contains NaNs!");
			}
		}

		[Conditional("DEBUG")]
		public static void CheckForNaNs(this Vector2 v)
		{
			if (	float.IsNaN(v.X) ||
				    float.IsNaN(v.Y) )
			{
				throw new InvalidOperationException("Vector3 contains NaNs!");
			}
		}

		[Conditional("DEBUG")]
		public static void CheckForNaNs(this Vector4 v)
		{
			if (	float.IsNaN(v.X) ||
				    float.IsNaN(v.Y) ||
				    float.IsNaN(v.Z) ||
				    float.IsNaN(v.W)	)
			{
				throw new InvalidOperationException("Vector4 contains NaNs!");
			}
		}
	}
}
