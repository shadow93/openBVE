using System;

namespace TrainsimApi.Vectors {
	public struct Orientation3f : IComparable<Orientation3f>, IEquatable<Orientation3f> {
		
		
		// --- members ---
		
		public Vector3f X;
		
		public Vector3f Y;
		
		public Vector3f Z;
		
		
		// --- constructors ---
		
		public Orientation3f(Vector3f x, Vector3f y, Vector3f z) {
			this.X = x;
			this.Y = y;
			this.Z = z;
		}
		
		
		// --- read-only fields ---
		
		public static readonly Orientation3f Default = new Orientation3f(Vector3f.Right, Vector3f.Up,   Vector3f.Forward);
		
		public static readonly Orientation3f Zero    = new Orientation3f(Vector3f.Zero,  Vector3f.Zero, Vector3f.Zero);
		
		
		// --- operators ---
		
		public static Orientation3f operator +(Orientation3f a, Orientation3f b) {
			return new Orientation3f(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}
		
		public static Orientation3f operator +(Orientation3f a, Vector3f b) {
			return new Orientation3f(a.X + b, a.Y + b, a.Z + b);
		}

		public static Orientation3f operator +(Orientation3f a, float b) {
			return new Orientation3f(a.X + b, a.Y + b, a.Z + b);
		}
		
		public static Orientation3f operator +(Vector3f a, Orientation3f b) {
			return new Orientation3f(a + b.X, a + b.Y, a + b.Z);
		}

		public static Orientation3f operator +(float a, Orientation3f b) {
			return new Orientation3f(a + b.X, a + b.Y, a + b.Z);
		}
		
		public static Orientation3f operator -(Orientation3f a, Orientation3f b) {
			return new Orientation3f(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}
		
		public static Orientation3f operator -(Orientation3f a, Vector3f b) {
			return new Orientation3f(a.X - b, a.Y - b, a.Z - b);
		}

		public static Orientation3f operator -(Orientation3f a, float b) {
			return new Orientation3f(a.X - b, a.Y - b, a.Z - b);
		}
		
		public static Orientation3f operator -(Vector3f a, Orientation3f b) {
			return new Orientation3f(a - b.X, a - b.Y, a - b.Z);
		}

		public static Orientation3f operator -(float a, Orientation3f b) {
			return new Orientation3f(a - b.X, a - b.Y, a - b.Z);
		}
		
		public static Orientation3f operator -(Orientation3f a) {
			return new Orientation3f(-a.X, -a.Y, -a.Z);
		}
		
		public static Orientation3f operator *(Orientation3f a, Orientation3f b) {
			return new Orientation3f(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
		}
		
		public static Orientation3f operator *(Orientation3f a, Vector3f b) {
			return new Orientation3f(a.X * b, a.Y * b, a.Z * b);
		}

		public static Orientation3f operator *(Orientation3f a, float b) {
			return new Orientation3f(a.X * b, a.Y * b, a.Z * b);
		}
		
		public static Orientation3f operator *(Vector3f a, Orientation3f b) {
			return new Orientation3f(a * b.X, a * b.Y, a * b.Z);
		}

		public static Orientation3f operator *(float a, Orientation3f b) {
			return new Orientation3f(a * b.X, a * b.Y, a * b.Z);
		}
		
		public static Orientation3f operator /(Orientation3f a, Orientation3f b) {
			return new Orientation3f(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
		}
		
		public static Orientation3f operator /(Orientation3f a, Vector3f b) {
			return new Orientation3f(a.X / b, a.Y / b, a.Z / b);
		}

		public static Orientation3f operator /(Orientation3f a, float b) {
			return new Orientation3f(a.X / b, a.Y / b, a.Z / b);
		}
		
		public static Orientation3f operator /(Vector3f a, Orientation3f b) {
			return new Orientation3f(a / b.X, a / b.Y, a / b.Z);
		}

		public static Orientation3f operator /(float a, Orientation3f b) {
			return new Orientation3f(a / b.X, a / b.Y, a / b.Z);
		}
		
		public static bool operator ==(Orientation3f a, Orientation3f b) {
			if (a.X != b.X) return false;
			if (a.Y != b.Y) return false;
			if (a.Z != b.Z) return false;
			return true;
		}
		
		public static bool operator !=(Orientation3f a, Orientation3f b) {
			if (a.X != b.X) return true;
			if (a.Y != b.Y) return true;
			if (a.Z != b.Z) return true;
			return false;
		}
		
		
		// --- instance functions ---
		
		public Orientation3f Orthonormalize() {
			return Orientation3f.Orthonormalize(this);
		}
		
		
		// --- static functions ---
		
		public static Orientation3f Normalize(Orientation3f orientation) {
			return new Orientation3f(
				Vector3f.Normalize(orientation.X),
				Vector3f.Normalize(orientation.Y),
				Vector3f.Normalize(orientation.Z)
			);
		}
		
		public static Orientation3f Orthonormalize(Orientation3f orientation) {
			Vector3f x = Vector3f.Normalize(orientation.X);
			Vector3f y, z;
			if (x.IsZero()) {
				y = Vector3f.Normalize(orientation.Y);
				if (y.IsZero()) {
					z = Vector3f.Normalize(orientation.Z);
					if (z.IsZero()) {
						x = Vector3f.Right;
						y = Vector3f.Up;
						z = Vector3f.Forward;
					} else {
						x = Vector3f.Normalize(Vector3f.Cross(Vector3f.Up, z));
						if (x.IsZero()) {
							x = Vector3f.Right;
						}
						y = Vector3f.Cross(z, x);
					}
				} else {
					x = Vector3f.Normalize(Vector3f.Cross(y, orientation.Z));
					if (x.IsZero()) {
						x = Vector3f.Normalize(Vector3f.Cross(y, Vector3f.Forward));
						if (x.IsZero()) {
							x = Vector3f.Right;
						}
						z = Vector3f.Cross(x, y);
					} else {
						z = Vector3f.Cross(x, y);
					}
				}
			} else {
				y = Vector3f.Normalize(Vector3f.Cross(orientation.Z, x));
				if (y.IsZero()) {
					z = Vector3f.Normalize(Vector3f.Cross(x, orientation.Y));
					if (z.IsZero()) {
						y = Vector3f.Normalize(Vector3f.Cross(Vector3f.Forward, x));
						if (y.IsZero()) {
							y = Vector3f.Up;
						}
						z = Vector3f.Cross(x, y);
					} else {
						y = Vector3f.Cross(z, x);
					}
				} else {
					z = Vector3f.Cross(x, y);
				}
			}
			return new Orientation3f(x, y, z);
		}
		
		public static Orientation3f Transpose(Orientation3f orientation) {
			Vector3f x = new Vector3f(orientation.X.X, orientation.Y.X, orientation.Z.X);
			Vector3f y = new Vector3f(orientation.X.Y, orientation.Y.Y, orientation.Z.Y);
			Vector3f z = new Vector3f(orientation.X.Z, orientation.Y.Z, orientation.Z.Z);
			return new Orientation3f(x, y, z);
		}
		
		public static Orientation3f RotateAbsoluteXY(Orientation3f orientation, Vector2f angle) {
			Vector3f x = Vector3f.RotateXY(orientation.X, angle);
			Vector3f y = Vector3f.RotateXY(orientation.Y, angle);
			Vector3f z = Vector3f.RotateXY(orientation.Z, angle);
			return new Orientation3f(x, y, z);
		}

		public static Orientation3f RotateAbsoluteXZ(Orientation3f orientation, Vector2f angle) {
			Vector3f x = Vector3f.RotateXZ(orientation.X, angle);
			Vector3f y = Vector3f.RotateXZ(orientation.Y, angle);
			Vector3f z = Vector3f.RotateXZ(orientation.Z, angle);
			return new Orientation3f(x, y, z);
		}
		
		public static Orientation3f RotateAbsoluteYZ(Orientation3f orientation, Vector2f angle) {
			Vector3f x = Vector3f.RotateYZ(orientation.X, angle);
			Vector3f y = Vector3f.RotateYZ(orientation.Y, angle);
			Vector3f z = Vector3f.RotateYZ(orientation.Z, angle);
			return new Orientation3f(x, y, z);
		}
		
		public static Orientation3f RotateRelativeXY(Orientation3f orientation, Vector2f angle) {
			Vector3f z = orientation.Z;
			Vector3f x = Vector3f.Rotate(orientation.X, orientation.Z, angle);
			Vector3f y = Vector3f.Cross(z, x);
			return new Orientation3f(x, y, z);
		}
		
		public static Orientation3f RotateRelativeXZ(Orientation3f orientation, Vector2f angle) {
			Vector3f x = Vector3f.Rotate(orientation.X, orientation.Y, angle);
			Vector3f y = orientation.Y;
			Vector3f z = Vector3f.Cross(x, y);
			return new Orientation3f(x, y, z);
		}
		
		public static Orientation3f RotateRelativeYZ(Orientation3f orientation, Vector2f angle) {
			Vector3f x = orientation.X;
			Vector3f y = Vector3f.Rotate(orientation.Y, orientation.X, angle);
			Vector3f z = Vector3f.Cross(x, y);
			return new Orientation3f(x, y, z);
		}
		
		public static Orientation3f Rotate(Orientation3f orientation, Vector3f direction, Vector2f angle) {
			Vector3f x = Vector3f.Rotate(orientation.X, direction, angle);
			Vector3f y = Vector3f.Rotate(orientation.Y, direction, angle);
			Vector3f z = Vector3f.Cross(x, y);
			return new Orientation3f(x, y, z);
		}
		
		public static Orientation3f Rotate(Orientation3f orientation, Orientation3f relative) {
			Vector3f x = relative.X.X * orientation.X + relative.Y.X * orientation.Y + relative.Z.X * orientation.Z;
			Vector3f y = relative.X.Y * orientation.X + relative.Y.Y * orientation.Y + relative.Z.Y * orientation.Z;
			Vector3f z = Vector3f.Cross(x, y);
			return new Orientation3f(x, y, z);
		}
		
		public static Orientation3f Nlerp(Orientation3f p0, Orientation3f p1, float t) {
			return Orientation3f.Orthonormalize((1.0f - t) * p0 + t * p1);
		}

		
		// --- overrides and interface implementations ---
		
		public int CompareTo(Orientation3f other) {
			int value;
			value = this.X.CompareTo(other.X);
			if (value != 0) return value;
			value = this.Y.CompareTo(other.Y);
			if (value != 0) return value;
			value = this.Z.CompareTo(other.Z);
			if (value != 0) return value;
			return 0;
		}
		
		public bool Equals(Orientation3f other) {
			if (this.X != other.X) return false;
			if (this.Y != other.Y) return false;
			if (this.Z != other.Z) return false;
			return true;
		}
		
		public override bool Equals(object obj) {
			if (!(obj is Orientation3f)) return false;
			Orientation3f other = (Orientation3f)obj;
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