using System;
using System.Runtime.InteropServices;

namespace TrainsimApi.Vectors {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Vector3d : IComparable<Vector3d>, IEquatable<Vector3d> {
		
		
		// --- members ---
		
		public double X;
		
		public double Y;
		
		public double Z;
		
		
		// --- constructors ---
		
		public Vector3d(double x, double y, double z) {
			this.X = x;
			this.Y = y;
			this.Z = z;
		}
		
		
		// --- readonly fields (vectors) ---
		
		public static readonly Vector3d Zero     = new Vector3d( 0.0,  0.0,  0.0);
		
		public static readonly Vector3d Left     = new Vector3d(-1.0,  0.0,  0.0);
		
		public static readonly Vector3d Right    = new Vector3d( 1.0,  0.0,  0.0);
		
		public static readonly Vector3d Down     = new Vector3d( 0.0, -1.0,  0.0);
		
		public static readonly Vector3d Up       = new Vector3d( 0.0,  1.0,  0.0);
		
		public static readonly Vector3d Backward = new Vector3d( 0.0,  0.0, -1.0);
		
		public static readonly Vector3d Forward  = new Vector3d( 0.0,  0.0,  1.0);
		
		public static readonly Vector3d One      = new Vector3d( 1.0,  1.0,  1.0);
		
		
		// --- readonly fields (colors) ---
		
		public static readonly Vector3d Black    = new Vector3d( 0.0,  0.0,  0.0);
		
		public static readonly Vector3d Red      = new Vector3d( 1.0,  0.0,  0.0);
		
		public static readonly Vector3d Green    = new Vector3d( 0.0,  1.0,  0.0);
		
		public static readonly Vector3d Blue     = new Vector3d( 0.0,  0.0,  1.0);
		
		public static readonly Vector3d Cyan     = new Vector3d( 0.0,  1.0,  1.0);
		
		public static readonly Vector3d Magenta  = new Vector3d( 1.0,  0.0,  1.0);
		
		public static readonly Vector3d Yellow   = new Vector3d( 1.0,  1.0,  0.0);
		
		public static readonly Vector3d White    = new Vector3d( 1.0,  1.0,  1.0);
		
		
		// --- operators ---
		
		public static Vector3d operator +(Vector3d a, Vector3d b) {
			return new Vector3d(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}
		
		public static Vector3d operator +(Vector3d a, double b) {
			return new Vector3d(a.X + b, a.Y + b, a.Z + b);
		}
		
		public static Vector3d operator +(double a, Vector3d b) {
			return new Vector3d(a + b.X, a + b.Y, a + b.Z);
		}
		
		public static Vector3d operator -(Vector3d a, Vector3d b) {
			return new Vector3d(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}
		
		public static Vector3d operator -(Vector3d a, double b) {
			return new Vector3d(a.X - b, a.Y - b, a.Z - b);
		}
		
		public static Vector3d operator -(double a, Vector3d b) {
			return new Vector3d(a - b.X, a - b.Y, a - b.Z);
		}
		
		public static Vector3d operator -(Vector3d a) {
			return new Vector3d(-a.X, -a.Y, -a.Z);
		}
		
		public static Vector3d operator *(Vector3d a, Vector3d b) {
			return new Vector3d(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
		}
		
		public static Vector3d operator *(Vector3d a, double b) {
			return new Vector3d(a.X * b, a.Y * b, a.Z * b);
		}
		
		public static Vector3d operator *(double a, Vector3d b) {
			return new Vector3d(a * b.X, a * b.Y, a * b.Z);
		}
		
		public static Vector3d operator /(Vector3d a, Vector3d b) {
			return new Vector3d(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
		}
		
		public static Vector3d operator /(Vector3d a, double b) {
			return new Vector3d(a.X / b, a.Y / b, a.Z / b);
		}
		
		public static Vector3d operator /(double a, Vector3d b) {
			return new Vector3d(a / b.X, a / b.Y, a / b.Z);
		}
		
		public static bool operator ==(Vector3d a, Vector3d b) {
			if (a.X != b.X) return false;
			if (a.Y != b.Y) return false;
			if (a.Z != b.Z) return false;
			return true;
		}
		
		public static bool operator !=(Vector3d a, Vector3d b) {
			if (a.X != b.X) return true;
			if (a.Y != b.Y) return true;
			if (a.Z != b.Z) return true;
			return false;
		}
		
		public static implicit operator Vector3d(Vector3f vector) {
			return new Vector3d(vector.X, vector.Y, vector.Z);
		}
		
		public static explicit operator Vector3f(Vector3d vector) {
			return new Vector3f((float)vector.X, (float)vector.Y, (float)vector.Z);
		}
		
		
		// --- static functions (mathematical) ---
		
		public static double Abs(Vector3d vector) {
			return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
		}
		
		public static Vector3d Sign(Vector3d vector) {
			double t = vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z;
			if (t != 0.0) {
				t = 1.0 / Math.Sqrt(t);
				return new Vector3d(vector.X * t, vector.Y * t, vector.Z * t);
			} else {
				return Vector3d.Zero;
			}
		}
		
		public static double Dot(Vector3d a, Vector3d b) {
			return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
		}
		
		public static Vector3d Cross(Vector3d a, Vector3d b) {
			return new Vector3d(
				a.Y * b.Z - a.Z * b.Y,
				a.Z * b.X - a.X * b.Z,
				a.X * b.Y - a.Y * b.X
			);
		}
		
		
		// --- static functions (geometrical) ---
		
		public static Vector3d Normalize(Vector3d vector) {
			return Vector3d.Sign(vector);
		}
		
		public static Vector3d RotateXY(Vector3d vector, Vector2d angle) {
			double x = angle.X * vector.X - angle.Y * vector.Y;
			double y = angle.Y * vector.X + angle.X * vector.Y;
			double z = vector.Z;
			return new Vector3d(x, y, z);
		}

		public static Vector3d RotateXZ(Vector3d vector, Vector2d angle) {
			double x = angle.X * vector.X + angle.Y * vector.Z;
			double y = vector.Y;
			double z = angle.X * vector.Z - angle.Y * vector.X;
			return new Vector3d(x, y, z);
		}

		public static Vector3d RotateYZ(Vector3d vector, Vector2d angle) {
			double x = vector.X;
			double y = angle.X * vector.Y - angle.Y * vector.Z;
			double z = angle.Y * vector.Y + angle.X * vector.Z;
			return new Vector3d(x, y, z);
		}
		
		public static Vector3d Rotate(Vector3d vector, Vector3d direction, Vector2d angle) {
			double versin = 1.0 - angle.X;
			double x =
				(angle.X + versin * direction.X * direction.X)               * vector.X +
				(versin * direction.X * direction.Y - angle.Y * direction.Z) * vector.Y +
				(versin * direction.X * direction.Z + angle.Y * direction.Y) * vector.Z;
			double y =
				(versin * direction.X * direction.Y + angle.Y * direction.Z) * vector.X +
				(angle.X + versin * direction.Y * direction.Y)               * vector.Y +
				(versin * direction.Y * direction.Z - angle.Y * direction.X) * vector.Z;
			double z =
				(versin * direction.X * direction.Z - angle.Y * direction.Y) * vector.X +
				(versin * direction.Y * direction.Z + angle.Y * direction.X) * vector.Y +
				(angle.X + versin * direction.Z * direction.Z)               * vector.Z;
			return new Vector3d(x, y, z);
		}
		
		public static Vector3d Rotate(Vector3d vector, Orientation3d orientation) {
			return vector.X * orientation.X + vector.Y * orientation.Y + vector.Z * orientation.Z;
		}
		
		
		// --- static functions (interpolation) ---

		public static Vector3d Lerp(Vector3d a, Vector3d b, double t) {
			return (1.0 - t) * a + t * b;
		}
		
		public static Vector3d Hermite(Vector3d p0, Vector3d m0, Vector3d p1, Vector3d m1, double t) {
			double t2 = t * t;
			double t3 = t2 * t;
			double tx = 3.0 * t2 - 2.0 * t3;
			return (1.0 - tx) * p0 + (t3 - 2.0 * t2 + t) * m0 + tx * p1 + (t3 - t2) * m1;
		}
		
		
		// --- instance functions ---
		
		public bool IsZero() {
			return this.X == 0.0 & this.Y == 0.0 & this.Z == 0.0;
		}
		
		public Vector3d Normalize() {
			return Vector3d.Normalize(this);
		}
		
		
		// --- overrides and interface implementations ---
		
		public int CompareTo(Vector3d other) {
			if (this.X < other.X) return -1;
			if (this.X > other.X) return  1;
			if (this.Y < other.Y) return -1;
			if (this.Y > other.Y) return  1;
			if (this.Z < other.Z) return -1;
			if (this.Z > other.Z) return  1;
			return 0;
		}
		
		public bool Equals(Vector3d other) {
			if (this.X != other.X) return false;
			if (this.Y != other.Y) return false;
			if (this.Z != other.Z) return false;
			return true;
		}
		
		public override bool Equals(object obj) {
			if (!(obj is Vector3d)) return false;
			Vector3d other = (Vector3d)obj;
			if (this.X != other.X) return false;
			if (this.Y != other.Y) return false;
			if (this.Z != other.Z) return false;
			return true;
		}
		
		public override int GetHashCode() {
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * X.GetHashCode();
				hashCode += 1000000009 * Y.GetHashCode();
				hashCode += 1000000021 * Z.GetHashCode();
			}
			return hashCode;
		}
		
		public override string ToString() {
			return '{' + this.X.ToString() + ',' + this.Y.ToString() + ',' + this.Z.ToString() + '}';
		}
		
		
	}
}