using System;
using System.Runtime.InteropServices;

namespace TrainsimApi.Vectors {

	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Vector2d : IComparable<Vector2d>, IEquatable<Vector2d> {
		
		
		// --- members ---
		
		public double X;
		
		public double Y;
		
		
		// --- constructors ---
		
		public Vector2d(double angle) {
			this.X = Math.Cos(angle);
			this.Y = Math.Sin(angle);
		}
		
		public Vector2d(double x, double y) {
			this.X = x;
			this.Y = y;
		}
		
		
		// --- readonly fields ---
		
		public static readonly Vector2d Zero  = new Vector2d( 0.0,  0.0);
		
		public static readonly Vector2d Left  = new Vector2d(-1.0,  0.0);
		
		public static readonly Vector2d Right = new Vector2d( 1.0,  0.0);
		
		public static readonly Vector2d Down  = new Vector2d( 0.0, -1.0);
		
		public static readonly Vector2d Up    = new Vector2d( 0.0,  1.0);
		
		public static readonly Vector2d One   = new Vector2d( 1.0,  1.0);

		
		// --- operators ---
		
		public static Vector2d operator +(Vector2d a, Vector2d b) {
			return new Vector2d(a.X + b.X, a.Y + b.Y);
		}
		
		public static Vector2d operator +(Vector2d a, double b) {
			return new Vector2d(a.X + b, a.Y + b);
		}

		public static Vector2d operator +(double a, Vector2d b) {
			return new Vector2d(a + b.X, a + b.Y);
		}

		public static Vector2d operator -(Vector2d a, Vector2d b) {
			return new Vector2d(a.X - b.X, a.Y - b.Y);
		}
		
		public static Vector2d operator -(Vector2d a, double b) {
			return new Vector2d(a.X - b, a.Y - b);
		}

		public static Vector2d operator -(double a, Vector2d b) {
			return new Vector2d(a - b.X, a - b.Y);
		}

		public static Vector2d operator -(Vector2d a) {
			return new Vector2d(-a.X, -a.Y);
		}
		
		public static Vector2d operator *(Vector2d a, Vector2d b) {
			return new Vector2d(a.X * b.X, a.Y * b.Y);
		}
		
		public static Vector2d operator *(Vector2d a, double b) {
			return new Vector2d(a.X * b, a.Y * b);
		}

		public static Vector2d operator *(double a, Vector2d b) {
			return new Vector2d(a * b.X, a * b.Y);
		}

		public static Vector2d operator /(Vector2d a, Vector2d b) {
			return new Vector2d(a.X / b.X, a.Y / b.Y);
		}
		
		public static Vector2d operator /(Vector2d a, double b) {
			return new Vector2d(a.X / b, a.Y / b);
		}

		public static Vector2d operator /(double a, Vector2d b) {
			return new Vector2d(a / b.X, a / b.Y);
		}

		public static bool operator ==(Vector2d a, Vector2d b) {
			if (a.X != b.X) return false;
			if (a.Y != b.Y) return false;
			return true;
		}
		
		public static bool operator !=(Vector2d a, Vector2d b) {
			if (a.X != b.X) return true;
			if (a.Y != b.Y) return true;
			return false;
		}
		
		public static implicit operator Vector2d(Vector2f vector) {
			return new Vector2d(vector.X, vector.Y);
		}
		
		public static explicit operator Vector2f(Vector2d vector) {
			return new Vector2f((float)vector.X, (float)vector.Y);
		}
		
		
		// --- static functions (mathematical) ---
		
		public static double Abs(Vector2d vector) {
			return Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y);
		}
		
		public static Vector2d Sign(Vector2d vector) {
			double t = vector.X * vector.X + vector.Y * vector.Y;
			if (t != 0.0) {
				t = 1.0 / Math.Sqrt(t);
				return new Vector2d(vector.X * t, vector.Y * t);
			} else {
				return Vector2d.Zero;
			}
		}
		
		public static double Dot(Vector2d a, Vector2d b) {
			return a.X * b.X + a.Y * b.Y;
		}
		
		public static Vector2d Cross(Vector2d a) {
			return new Vector2d(-a.Y, a.X);
		}
		
		
		// --- static functions (geometrical) ---
		
		public static Vector2d Normalize(Vector2d vector) {
			return Vector2d.Sign(vector);
		}
		
		public static Vector2d Rotate(Vector2d vector, Vector2d angle) {
			double x = angle.X * vector.X - angle.Y * vector.Y;
			double y = angle.Y * vector.X + angle.X * vector.Y;
			return new Vector2d(x, y);
		}
		
		public static Vector2d Rotate(Vector2d vector, Orientation2d orientation) {
			return vector.X * orientation.X + vector.Y * orientation.Y;
		}
		
		
		// --- static functions (interpolation) ---
		
		public static Vector2d Lerp(Vector2d a, Vector2d b, double t) {
			return (1.0 - t) * a + t * b;
		}
		
		public static Vector2d Hermite(Vector2d p0, Vector2d m0, Vector2d p1, Vector2d m1, double t) {
			double t2 = t * t;
			double t3 = t2 * t;
			double tx = 3.0 * t2 - 2.0 * t3;
			return (1.0 - tx) * p0 + (t3 - 2.0 * t2 + t) * m0 + tx * p1 + (t3 - t2) * m1;
		}
		
		
		// --- instance functions ---
		
		public bool IsZero() {
			return this.X == 0.0 & this.Y == 0.0;
		}
		
		
		// --- overrides and interface implementations ---
		
		public int CompareTo(Vector2d other) {
			if (this.X < other.X) return -1;
			if (this.X > other.X) return  1;
			if (this.Y < other.Y) return -1;
			if (this.Y > other.Y) return  1;
			return 0;
		}
		
		public bool Equals(Vector2d other) {
			if (this.X != other.X) return false;
			if (this.Y != other.Y) return false;
			return true;
		}
		
		public override bool Equals(object obj) {
			if (!(obj is Vector2d)) return false;
			Vector2d other = (Vector2d)obj;
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
