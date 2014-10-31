using System;
using System.Runtime.InteropServices;

namespace TrainsimApi.Vectors {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Vector3f : IComparable<Vector3f>, IEquatable<Vector3f> {
		
		
		// --- members ---
		
		public float X;
		
		public float Y;
		
		public float Z;
		
		
		// --- constructors ---
		
		public Vector3f(float x, float y, float z) {
			this.X = x;
			this.Y = y;
			this.Z = z;
		}

		
		// --- readonly fields (vectors) ---
		
		public static readonly Vector3f Zero     = new Vector3f( 0.0f,  0.0f,  0.0f);
		
		public static readonly Vector3f Left     = new Vector3f(-1.0f,  0.0f,  0.0f);
		
		public static readonly Vector3f Right    = new Vector3f( 1.0f,  0.0f,  0.0f);
		
		public static readonly Vector3f Down     = new Vector3f( 0.0f, -1.0f,  0.0f);
		
		public static readonly Vector3f Up       = new Vector3f( 0.0f,  1.0f,  0.0f);
		
		public static readonly Vector3f Backward = new Vector3f( 0.0f,  0.0f, -1.0f);
		
		public static readonly Vector3f Forward  = new Vector3f( 0.0f,  0.0f,  1.0f);
		
		public static readonly Vector3f One      = new Vector3f( 1.0f,  1.0f,  1.0f);
		
		
		// --- readonly fields (colors) ---
		
		public static readonly Vector3f Black    = new Vector3f( 0.0f,  0.0f,  0.0f);
		
		public static readonly Vector3f Red      = new Vector3f( 1.0f,  0.0f,  0.0f);
		
		public static readonly Vector3f Green    = new Vector3f( 0.0f,  1.0f,  0.0f);
		
		public static readonly Vector3f Blue     = new Vector3f( 0.0f,  0.0f,  1.0f);
		
		public static readonly Vector3f Cyan     = new Vector3f( 0.0f,  1.0f,  1.0f);
		
		public static readonly Vector3f Magenta  = new Vector3f( 1.0f,  0.0f,  1.0f);
		
		public static readonly Vector3f Yellow   = new Vector3f( 1.0f,  1.0f,  0.0f);
		
		public static readonly Vector3f White    = new Vector3f( 1.0f,  1.0f,  1.0f);
		
		
		// --- operators ---
		
		public static Vector3f operator +(Vector3f a, Vector3f b) {
			return new Vector3f(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}
		
		public static Vector3f operator +(Vector3f a, float b) {
			return new Vector3f(a.X + b, a.Y + b, a.Z + b);
		}
		
		public static Vector3f operator +(float a, Vector3f b) {
			return new Vector3f(a + b.X, a + b.Y, a + b.Z);
		}
		
		public static Vector3f operator -(Vector3f a, Vector3f b) {
			return new Vector3f(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}
		
		public static Vector3f operator -(Vector3f a, float b) {
			return new Vector3f(a.X - b, a.Y - b, a.Z - b);
		}
		
		public static Vector3f operator -(float a, Vector3f b) {
			return new Vector3f(a - b.X, a - b.Y, a - b.Z);
		}
		
		public static Vector3f operator -(Vector3f a) {
			return new Vector3f(-a.X, -a.Y, -a.Z);
		}
		
		public static Vector3f operator *(Vector3f a, Vector3f b) {
			return new Vector3f(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
		}
		
		public static Vector3f operator *(Vector3f a, float b) {
			return new Vector3f(a.X * b, a.Y * b, a.Z * b);
		}
		
		public static Vector3f operator *(float a, Vector3f b) {
			return new Vector3f(a * b.X, a * b.Y, a * b.Z);
		}
		
		public static Vector3f operator /(Vector3f a, Vector3f b) {
			return new Vector3f(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
		}
		
		public static Vector3f operator /(Vector3f a, float b) {
			return new Vector3f(a.X / b, a.Y / b, a.Z / b);
		}
		
		public static Vector3f operator /(float a, Vector3f b) {
			return new Vector3f(a / b.X, a / b.Y, a / b.Z);
		}
		
		public static bool operator ==(Vector3f a, Vector3f b) {
			if (a.X != b.X) return false;
			if (a.Y != b.Y) return false;
			if (a.Z != b.Z) return false;
			return true;
		}
		
		public static bool operator !=(Vector3f a, Vector3f b) {
			if (a.X != b.X) return true;
			if (a.Y != b.Y) return true;
			if (a.Z != b.Z) return true;
			return false;
		}
		
		
		// --- static functions (mathematical) ---
		
		public static float Abs(Vector3f vector) {
			return (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z);
		}
		
		public static Vector3f Sign(Vector3f vector) {
			float t = vector.X * vector.X + vector.Y * vector.Y + vector.Z * vector.Z;
			if (t != 0.0f) {
				t = 1.0f / (float)Math.Sqrt(t);
				return new Vector3f(vector.X * t, vector.Y * t, vector.Z * t);
			} else {
				return Vector3f.Zero;
			}
		}
		
		public static float Dot(Vector3f a, Vector3f b) {
			return a.X * b.X + a.Y * b.Y + a.Z * b.Z;
		}
		
		public static Vector3f Cross(Vector3f a, Vector3f b) {
			return new Vector3f(
				a.Y * b.Z - a.Z * b.Y,
				a.Z * b.X - a.X * b.Z,
				a.X * b.Y - a.Y * b.X
			);
		}
		
		
		// --- static functions (geometrical) ---
		
		public static Vector3f Normalize(Vector3f vector) {
			return Vector3f.Sign(vector);
		}
		
		public static Vector3f RotateXY(Vector3f vector, Vector2f angle) {
			float x = angle.X * vector.X - angle.Y * vector.Y;
			float y = angle.Y * vector.X + angle.X * vector.Y;
			float z = vector.Z;
			return new Vector3f(x, y, z);
		}

		public static Vector3f RotateXZ(Vector3f vector, Vector2f angle) {
			float x = angle.X * vector.X + angle.Y * vector.Z;
			float y = vector.Y;
			float z = angle.X * vector.Z - angle.Y * vector.X;
			return new Vector3f(x, y, z);
		}

		public static Vector3f RotateYZ(Vector3f vector, Vector2f angle) {
			float x = vector.X;
			float y = angle.X * vector.Y - angle.Y * vector.Z;
			float z = angle.Y * vector.Y + angle.X * vector.Z;
			return new Vector3f(x, y, z);
		}
		
		public static Vector3f Rotate(Vector3f vector, Vector3f direction, Vector2f angle) {
			float versin = 1.0f - angle.X;
			float x =
				(angle.X + versin * direction.X * direction.X)               * vector.X +
				(versin * direction.X * direction.Y - angle.Y * direction.Z) * vector.Y +
				(versin * direction.X * direction.Z + angle.Y * direction.Y) * vector.Z;
			float y =
				(versin * direction.X * direction.Y + angle.Y * direction.Z) * vector.X +
				(angle.X + versin * direction.Y * direction.Y)               * vector.Y +
				(versin * direction.Y * direction.Z - angle.Y * direction.X) * vector.Z;
			float z =
				(versin * direction.X * direction.Z - angle.Y * direction.Y) * vector.X +
				(versin * direction.Y * direction.Z + angle.Y * direction.X) * vector.Y +
				(angle.X + versin * direction.Z * direction.Z)               * vector.Z;
			return new Vector3f(x, y, z);
		}
		
		public static Vector3f Rotate(Vector3f vector, Orientation3f orientation) {
			return vector.X * orientation.X + vector.Y * orientation.Y + vector.Z * orientation.Z;
		}
		
		
		// --- static functions (interpolation) ---

		public static Vector3f Lerp(Vector3f a, Vector3f b, float t) {
			return (1.0f - t) * a + t * b;
		}
		
		public static Vector3f Hermite(Vector3f p0, Vector3f m0, Vector3f p1, Vector3f m1, float t) {
			float t2 = t * t;
			float t3 = t2 * t;
			float tx = 3.0f * t2 - 2.0f * t3;
			return (1.0f - tx) * p0 + (t3 - 2.0f * t2 + t) * m0 + tx * p1 + (t3 - t2) * m1;
		}
		
		
		// --- instance functions ---
		
		public bool IsZero() {
			return this.X == 0.0f & this.Y == 0.0f & this.Z == 0.0;
		}
		
		public Vector3f Normalize() {
			return Vector3f.Normalize(this);
		}
		
		
		// --- overrides and interface implementations ---
		
		public int CompareTo(Vector3f other) {
			if (this.X < other.X) return -1;
			if (this.X > other.X) return  1;
			if (this.Y < other.Y) return -1;
			if (this.Y > other.Y) return  1;
			if (this.Z < other.Z) return -1;
			if (this.Z > other.Z) return  1;
			return 0;
		}
		
		public bool Equals(Vector3f other) {
			if (this.X != other.X) return false;
			if (this.Y != other.Y) return false;
			if (this.Z != other.Z) return false;
			return true;
		}
		
		public override bool Equals(object obj) {
			if (!(obj is Vector3f)) return false;
			Vector3f other = (Vector3f)obj;
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