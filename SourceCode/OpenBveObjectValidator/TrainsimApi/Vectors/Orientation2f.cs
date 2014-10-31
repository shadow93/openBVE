using System;

namespace TrainsimApi.Vectors {
	public struct Orientation2f : IComparable<Orientation2f>, IEquatable<Orientation2f> {
		
		
		// --- members ---
		
		public Vector2f X;
		
		public Vector2f Y;
		
		
		// --- constructors ---
		
		public Orientation2f(Vector2f x, Vector2f y) {
			this.X = x;
			this.Y = y;
		}
		
		
		// --- read-only fields ---
		
		public static readonly Orientation2f Default = new Orientation2f(Vector2f.Right, Vector2f.Up);
		
		
		// --- operators ---
		
		public static Orientation2f operator +(Orientation2f a, Orientation2f b) {
			return new Orientation2f(a.X + b.X, a.Y + b.Y);
		}
		
		public static Orientation2f operator +(Orientation2f a, Vector2f b) {
			return new Orientation2f(a.X + b, a.Y + b);
		}

		public static Orientation2f operator +(Orientation2f a, float b) {
			return new Orientation2f(a.X + b, a.Y + b);
		}
		
		public static Orientation2f operator +(Vector2f a, Orientation2f b) {
			return new Orientation2f(a + b.X, a + b.Y);
		}
		
		public static Orientation2f operator +(float a, Orientation2f b) {
			return new Orientation2f(a + b.X, a + b.Y);
		}

		public static Orientation2f operator -(Orientation2f a, Orientation2f b) {
			return new Orientation2f(a.X - b.X, a.Y - b.Y);
		}
		
		public static Orientation2f operator -(Orientation2f a, Vector2f b) {
			return new Orientation2f(a.X - b, a.Y - b);
		}

		public static Orientation2f operator -(Orientation2f a, float b) {
			return new Orientation2f(a.X - b, a.Y - b);
		}

		public static Orientation2f operator -(float a, Orientation2f b) {
			return new Orientation2f(a - b.X, a - b.Y);
		}
		
		public static Orientation2f operator -(Vector2f a, Orientation2f b) {
			return new Orientation2f(a - b.X, a - b.Y);
		}
		
		public static Orientation2f operator -(Orientation2f a) {
			return new Orientation2f(-a.X, -a.Y);
		}
		
		public static Orientation2f operator *(Orientation2f a, Orientation2f b) {
			return new Orientation2f(a.X * b.X, a.Y * b.Y);
		}
		
		public static Orientation2f operator *(Orientation2f a, Vector2f b) {
			return new Orientation2f(a.X * b, a.Y * b);
		}
		
		public static Orientation2f operator *(Orientation2f a, float b) {
			return new Orientation2f(a.X * b, a.Y * b);
		}
		
		public static Orientation2f operator *(Vector2f a, Orientation2f b) {
			return new Orientation2f(a * b.X, a * b.Y);
		}

		public static Orientation2f operator *(float a, Orientation2f b) {
			return new Orientation2f(a * b.X, a * b.Y);
		}
		
		public static Orientation2f operator /(Orientation2f a, Orientation2f b) {
			return new Orientation2f(a.X / b.X, a.Y / b.Y);
		}
		
		public static Orientation2f operator /(Orientation2f a, Vector2f b) {
			return new Orientation2f(a.X / b, a.Y / b);
		}

		public static Orientation2f operator /(Orientation2f a, float b) {
			return new Orientation2f(a.X / b, a.Y / b);
		}
		
		public static Orientation2f operator /(Vector2f a, Orientation2f b) {
			return new Orientation2f(a / b.X, a / b.Y);
		}

		public static Orientation2f operator /(float a, Orientation2f b) {
			return new Orientation2f(a / b.X, a / b.Y);
		}
		
		public static bool operator ==(Orientation2f a, Orientation2f b) {
			if (a.X != b.X) return false;
			if (a.Y != b.Y) return false;
			return true;
		}
		
		public static bool operator !=(Orientation2f a, Orientation2f b) {
			if (a.X != b.X) return true;
			if (a.Y != b.Y) return true;
			return false;
		}
		
		
		// --- static functions ---
		
		public static Orientation2f Normalize(Orientation2f orientation) {
			Vector2f x = Vector2f.Normalize(orientation.X);
			Vector2f y = Vector2f.Normalize(orientation.Y);
			return new Orientation2f(x, y);
		}
		
		public static Orientation2f Orthonormalize(Orientation2f orientation) {
			Vector2f sum = orientation.X + orientation.Y;
			float inverseDenominator = 1.0f / (float)Math.Sqrt(2.0f * (sum.X * sum.X + sum.Y * sum.Y));
			float xx = (sum.Y + sum.X) * inverseDenominator;
			float xy = (sum.Y - sum.X) * inverseDenominator;
			return new Orientation2f(new Vector2f(xx, xy), new Vector2f(-xy, xx));
		}
		
		public static Orientation2f Rotate(Orientation2f orientation, Vector2f angle) {
			Vector2f x = Vector2f.Rotate(orientation.X, angle);
			Vector2f y = Vector2f.Cross(x);
			return new Orientation2f(x, y);
		}
		
		public static Orientation2f Rotate(Orientation2f orientation, Orientation2f relative) {
			Vector2f x = relative.X.X * orientation.X + relative.Y.X * orientation.Y;
			Vector2f y = Vector2f.Cross(x);
			return new Orientation2f(x, y);
		}
		
		public static Orientation2f Nlerp(Orientation2f p0, Orientation2f p1, float t) {
			return Orientation2f.Orthonormalize((1.0f - t) * p0 + t * p1);
		}

		
		// --- overrides and interface implementations ---
		
		public int CompareTo(Orientation2f other) {
			int value;
			value = this.X.CompareTo(other.X);
			if (value != 0) return value;
			value = this.Y.CompareTo(other.Y);
			if (value != 0) return value;
			return 0;
		}
		
		public bool Equals(Orientation2f other) {
			if (this.X != other.X) return false;
			if (this.Y != other.Y) return false;
			return true;
		}
		
		public override bool Equals(object obj) {
			if (!(obj is Orientation2f)) return false;
			Orientation2f other = (Orientation2f)obj;
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