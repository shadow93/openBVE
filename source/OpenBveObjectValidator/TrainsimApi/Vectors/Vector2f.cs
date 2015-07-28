using System;
using System.Runtime.InteropServices;

namespace TrainsimApi.Vectors {

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Vector2f : IComparable<Vector2f>, IEquatable<Vector2f> {
		
		
		// --- members ---
		
		public float X;
		
		public float Y;
		
		
		// --- constructors ---
		
		public Vector2f(float angle) {
			this.X = (float)Math.Cos(angle);
			this.Y = (float)Math.Sin(angle);
		}
		
		public Vector2f(float x, float y) {
			this.X = x;
			this.Y = y;
		}
		
		
		// --- readonly fields ---
		
		public static readonly Vector2f Zero  = new Vector2f( 0.0f,  0.0f);
		
		public static readonly Vector2f Left  = new Vector2f(-1.0f,  0.0f);
		
		public static readonly Vector2f Right = new Vector2f( 1.0f,  0.0f);
		
		public static readonly Vector2f Down  = new Vector2f( 0.0f, -1.0f);
		
		public static readonly Vector2f Up    = new Vector2f( 0.0f,  1.0f);
		
		public static readonly Vector2f One   = new Vector2f( 1.0f,  1.0f);

		
		// --- operators ---
		
		public static Vector2f operator +(Vector2f a, Vector2f b) {
			return new Vector2f(a.X + b.X, a.Y + b.Y);
		}
		
		public static Vector2f operator +(Vector2f a, float b) {
			return new Vector2f(a.X + b, a.Y + b);
		}

		public static Vector2f operator +(float a, Vector2f b) {
			return new Vector2f(a + b.X, a + b.Y);
		}

		public static Vector2f operator -(Vector2f a, Vector2f b) {
			return new Vector2f(a.X - b.X, a.Y - b.Y);
		}
		
		public static Vector2f operator -(Vector2f a, float b) {
			return new Vector2f(a.X - b, a.Y - b);
		}

		public static Vector2f operator -(float a, Vector2f b) {
			return new Vector2f(a - b.X, a - b.Y);
		}

		public static Vector2f operator -(Vector2f a) {
			return new Vector2f(-a.X, -a.Y);
		}
		
		public static Vector2f operator *(Vector2f a, Vector2f b) {
			return new Vector2f(a.X * b.X, a.Y * b.Y);
		}
		
		public static Vector2f operator *(Vector2f a, float b) {
			return new Vector2f(a.X * b, a.Y * b);
		}

		public static Vector2f operator *(float a, Vector2f b) {
			return new Vector2f(a * b.X, a * b.Y);
		}

		public static Vector2f operator /(Vector2f a, Vector2f b) {
			return new Vector2f(a.X / b.X, a.Y / b.Y);
		}
		
		public static Vector2f operator /(Vector2f a, float b) {
			return new Vector2f(a.X / b, a.Y / b);
		}

		public static Vector2f operator /(float a, Vector2f b) {
			return new Vector2f(a / b.X, a / b.Y);
		}

		public static bool operator ==(Vector2f a, Vector2f b) {
			if (a.X != b.X) return false;
			if (a.Y != b.Y) return false;
			return true;
		}
		
		public static bool operator !=(Vector2f a, Vector2f b) {
			if (a.X != b.X) return true;
			if (a.Y != b.Y) return true;
			return false;
		}
		
		
		// --- static functions (mathematical) ---
		
		public static float Abs(Vector2f vector) {
			return (float)Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
		}
		
		public static Vector2f Sign(Vector2f vector) {
			float t = vector.X * vector.X + vector.Y * vector.Y;
			if (t != 0.0f) {
				t = 1.0f / (float)Math.Sqrt(t);
				return new Vector2f(vector.X * t, vector.Y * t);
			} else {
				return Vector2f.Zero;
			}
		}
		
		public static float Dot(Vector2f a, Vector2f b) {
			return a.X * b.X + a.Y * b.Y;
		}
		
		public static Vector2f Cross(Vector2f a) {
			return new Vector2f(-a.Y, a.X);
		}
		
		
		// --- static functions (geometrical) ---
		
		public static Vector2f Normalize(Vector2f vector) {
			return Vector2f.Sign(vector);
		}
		
		public static Vector2f Rotate(Vector2f vector, Vector2f angle) {
			float x = angle.X * vector.X - angle.Y * vector.Y;
			float y = angle.Y * vector.X + angle.X * vector.Y;
			return new Vector2f(x, y);
		}
		
		public static Vector2f Rotate(Vector2f vector, Orientation2f orientation) {
			return vector.X * orientation.X + vector.Y * orientation.Y;
		}
		
		
		// --- static functions (interpolation) ---
		
		public static Vector2f Lerp(Vector2f a, Vector2f b, float t) {
			return (1.0f - t) * a + t * b;
		}
		
		public static Vector2f Hermite(Vector2f p0, Vector2f m0, Vector2f p1, Vector2f m1, float t) {
			float t2 = t * t;
			float t3 = t2 * t;
			float tx = 3.0f * t2 - 2.0f * t3;
			return (1.0f - tx) * p0 + (t3 - 2.0f * t2 + t) * m0 + tx * p1 + (t3 - t2) * m1;
		}
		
		
		// --- instance functions ---
		
		public bool IsZero() {
			return this.X == 0.0f & this.Y == 0.0;
		}
		
		
		// --- overrides and interface implementations ---
		
		public int CompareTo(Vector2f other) {
			if (this.X < other.X) return -1;
			if (this.X > other.X) return  1;
			if (this.Y < other.Y) return -1;
			if (this.Y > other.Y) return  1;
			return 0;
		}
		
		public bool Equals(Vector2f other) {
			if (this.X != other.X) return false;
			if (this.Y != other.Y) return false;
			return true;
		}
		
		public override bool Equals(object obj) {
			if (!(obj is Vector2f)) return false;
			Vector2f other = (Vector2f)obj;
			if (this.X != other.X) return false;
			if (this.Y != other.Y) return false;
			return true;
		}
		
		public override int GetHashCode() {
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * X.GetHashCode();
				hashCode += 1000000009 * Y.GetHashCode();
			}
			return hashCode;
		}
		
		public override string ToString() {
			return '{' + this.X.ToString() + ',' + this.Y.ToString() + '}';
		}
		
	}
}
