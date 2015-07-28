using System;

namespace TrainsimApi.Vectors {
	public struct Orientation2d : IComparable<Orientation2d>, IEquatable<Orientation2d> {
		
		
		// --- members ---
		
		public Vector2d X;
		
		public Vector2d Y;
		
		
		// --- constructors ---
		
		public Orientation2d(Vector2d x, Vector2d y) {
			this.X = x;
			this.Y = y;
		}
		
		
		// --- read-only fields ---
		
		public static readonly Orientation2d Default = new Orientation2d(Vector2d.Right, Vector2d.Up);
		
		
		// --- operators ---
		
		public static Orientation2d operator +(Orientation2d a, Orientation2d b) {
			return new Orientation2d(a.X + b.X, a.Y + b.Y);
		}
		
		public static Orientation2d operator +(Orientation2d a, Vector2d b) {
			return new Orientation2d(a.X + b, a.Y + b);
		}

		public static Orientation2d operator +(Orientation2d a, double b) {
			return new Orientation2d(a.X + b, a.Y + b);
		}
		
		public static Orientation2d operator +(Vector2d a, Orientation2d b) {
			return new Orientation2d(a + b.X, a + b.Y);
		}
		
		public static Orientation2d operator +(double a, Orientation2d b) {
			return new Orientation2d(a + b.X, a + b.Y);
		}

		public static Orientation2d operator -(Orientation2d a, Orientation2d b) {
			return new Orientation2d(a.X - b.X, a.Y - b.Y);
		}
		
		public static Orientation2d operator -(Orientation2d a, Vector2d b) {
			return new Orientation2d(a.X - b, a.Y - b);
		}

		public static Orientation2d operator -(Orientation2d a, double b) {
			return new Orientation2d(a.X - b, a.Y - b);
		}

		public static Orientation2d operator -(double a, Orientation2d b) {
			return new Orientation2d(a - b.X, a - b.Y);
		}
		
		public static Orientation2d operator -(Vector2d a, Orientation2d b) {
			return new Orientation2d(a - b.X, a - b.Y);
		}
		
		public static Orientation2d operator -(Orientation2d a) {
			return new Orientation2d(-a.X, -a.Y);
		}
		
		public static Orientation2d operator *(Orientation2d a, Orientation2d b) {
			return new Orientation2d(a.X * b.X, a.Y * b.Y);
		}
		
		public static Orientation2d operator *(Orientation2d a, Vector2d b) {
			return new Orientation2d(a.X * b, a.Y * b);
		}
		
		public static Orientation2d operator *(Orientation2d a, double b) {
			return new Orientation2d(a.X * b, a.Y * b);
		}
		
		public static Orientation2d operator *(Vector2d a, Orientation2d b) {
			return new Orientation2d(a * b.X, a * b.Y);
		}

		public static Orientation2d operator *(double a, Orientation2d b) {
			return new Orientation2d(a * b.X, a * b.Y);
		}
		
		public static Orientation2d operator /(Orientation2d a, Orientation2d b) {
			return new Orientation2d(a.X / b.X, a.Y / b.Y);
		}
		
		public static Orientation2d operator /(Orientation2d a, Vector2d b) {
			return new Orientation2d(a.X / b, a.Y / b);
		}

		public static Orientation2d operator /(Orientation2d a, double b) {
			return new Orientation2d(a.X / b, a.Y / b);
		}
		
		public static Orientation2d operator /(Vector2d a, Orientation2d b) {
			return new Orientation2d(a / b.X, a / b.Y);
		}

		public static Orientation2d operator /(double a, Orientation2d b) {
			return new Orientation2d(a / b.X, a / b.Y);
		}
		
		public static bool operator ==(Orientation2d a, Orientation2d b) {
			if (a.X != b.X) return false;
			if (a.Y != b.Y) return false;
			return true;
		}
		
		public static bool operator !=(Orientation2d a, Orientation2d b) {
			if (a.X != b.X) return true;
			if (a.Y != b.Y) return true;
			return false;
		}
		
		public static implicit operator Orientation2d(Orientation2f orientation) {
			return new Orientation2d(orientation.X, orientation.Y);
		}
		
		public static explicit operator Orientation2f(Orientation2d orientation) {
			return new Orientation2f((Vector2f)orientation.X, (Vector2f)orientation.Y);
		}
		
		
		// --- static functions ---

		public static Orientation2d Normalize(Orientation2d orientation) {
			Vector2d x = Vector2d.Normalize(orientation.X);
			Vector2d y = Vector2d.Normalize(orientation.Y);
			return new Orientation2d(x, y);
		}
		
		public static Orientation2d Orthonormalize(Orientation2d orientation) {
			Vector2d sum = orientation.X + orientation.Y;
			double t = Math.Sqrt(2.0 * (sum.X * sum.X + sum.Y * sum.Y));
			double xx = (sum.Y + sum.X) / t;
			double xy = (sum.Y - sum.X) / t;
			return new Orientation2d(new Vector2d(xx, xy), new Vector2d(-xy, xx));
		}
		
		public static Orientation2d Rotate(Orientation2d orientation, Vector2d angle) {
			Vector2d x = Vector2d.Rotate(orientation.X, angle);
			Vector2d y = Vector2d.Cross(x);
			return new Orientation2d(x, y);
		}
		
		public static Orientation2d Rotate(Orientation2d orientation, Orientation2d relative) {
			Vector2d x = relative.X.X * orientation.X + relative.Y.X * orientation.Y;
			Vector2d y = Vector2d.Cross(x);
			return new Orientation2d(x, y);
		}
		
		public static Orientation2d Nlerp(Orientation2d p0, Orientation2d p1, double t) {
			return Orientation2d.Orthonormalize((1.0 - t) * p0 + t * p1);
		}

		
		// --- overrides and interface implementations ---
		
		public int CompareTo(Orientation2d other) {
			int value;
			value = this.X.CompareTo(other.X);
			if (value != 0) return value;
			value = this.Y.CompareTo(other.Y);
			if (value != 0) return value;
			return 0;
		}
		
		public bool Equals(Orientation2d other) {
			if (this.X != other.X) return false;
			if (this.Y != other.Y) return false;
			return true;
		}
		
		public override bool Equals(object obj) {
			if (!(obj is Orientation2d)) return false;
			Orientation2d other = (Orientation2d)obj;
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