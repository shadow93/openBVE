using System;

namespace TrainsimApi.Vectors {
	public struct Orientation3d : IComparable<Orientation3d>, IEquatable<Orientation3d> {
		
		
		// --- members ---
		
		public Vector3d X;
		
		public Vector3d Y;
		
		public Vector3d Z;
		
		
		// --- constructors ---
		
		public Orientation3d(Vector3d x, Vector3d y, Vector3d z) {
			this.X = x;
			this.Y = y;
			this.Z = z;
		}
		
		
		// --- read-only fields ---
		
		public static readonly Orientation3d Default = new Orientation3d(Vector3d.Right, Vector3d.Up,   Vector3d.Forward);
		
		public static readonly Orientation3d Zero    = new Orientation3d(Vector3d.Zero,  Vector3d.Zero, Vector3d.Zero);
		
		
		// --- operators ---
		
		public static Orientation3d operator +(Orientation3d a, Orientation3d b) {
			return new Orientation3d(a.X + b.X, a.Y + b.Y, a.Z + b.Z);
		}
		
		public static Orientation3d operator +(Orientation3d a, Vector3d b) {
			return new Orientation3d(a.X + b, a.Y + b, a.Z + b);
		}

		public static Orientation3d operator +(Orientation3d a, double b) {
			return new Orientation3d(a.X + b, a.Y + b, a.Z + b);
		}
		
		public static Orientation3d operator +(Vector3d a, Orientation3d b) {
			return new Orientation3d(a + b.X, a + b.Y, a + b.Z);
		}

		public static Orientation3d operator +(double a, Orientation3d b) {
			return new Orientation3d(a + b.X, a + b.Y, a + b.Z);
		}
		
		public static Orientation3d operator -(Orientation3d a, Orientation3d b) {
			return new Orientation3d(a.X - b.X, a.Y - b.Y, a.Z - b.Z);
		}
		
		public static Orientation3d operator -(Orientation3d a, Vector3d b) {
			return new Orientation3d(a.X - b, a.Y - b, a.Z - b);
		}

		public static Orientation3d operator -(Orientation3d a, double b) {
			return new Orientation3d(a.X - b, a.Y - b, a.Z - b);
		}
		
		public static Orientation3d operator -(Vector3d a, Orientation3d b) {
			return new Orientation3d(a - b.X, a - b.Y, a - b.Z);
		}

		public static Orientation3d operator -(double a, Orientation3d b) {
			return new Orientation3d(a - b.X, a - b.Y, a - b.Z);
		}
		
		public static Orientation3d operator -(Orientation3d a) {
			return new Orientation3d(-a.X, -a.Y, -a.Z);
		}
		
		public static Orientation3d operator *(Orientation3d a, Orientation3d b) {
			return new Orientation3d(a.X * b.X, a.Y * b.Y, a.Z * b.Z);
		}
		
		public static Orientation3d operator *(Orientation3d a, Vector3d b) {
			return new Orientation3d(a.X * b, a.Y * b, a.Z * b);
		}

		public static Orientation3d operator *(Orientation3d a, double b) {
			return new Orientation3d(a.X * b, a.Y * b, a.Z * b);
		}
		
		public static Orientation3d operator *(Vector3d a, Orientation3d b) {
			return new Orientation3d(a * b.X, a * b.Y, a * b.Z);
		}

		public static Orientation3d operator *(double a, Orientation3d b) {
			return new Orientation3d(a * b.X, a * b.Y, a * b.Z);
		}
		
		public static Orientation3d operator /(Orientation3d a, Orientation3d b) {
			return new Orientation3d(a.X / b.X, a.Y / b.Y, a.Z / b.Z);
		}
		
		public static Orientation3d operator /(Orientation3d a, Vector3d b) {
			return new Orientation3d(a.X / b, a.Y / b, a.Z / b);
		}

		public static Orientation3d operator /(Orientation3d a, double b) {
			return new Orientation3d(a.X / b, a.Y / b, a.Z / b);
		}
		
		public static Orientation3d operator /(Vector3d a, Orientation3d b) {
			return new Orientation3d(a / b.X, a / b.Y, a / b.Z);
		}

		public static Orientation3d operator /(double a, Orientation3d b) {
			return new Orientation3d(a / b.X, a / b.Y, a / b.Z);
		}
		
		public static bool operator ==(Orientation3d a, Orientation3d b) {
			if (a.X != b.X) return false;
			if (a.Y != b.Y) return false;
			if (a.Z != b.Z) return false;
			return true;
		}
		
		public static bool operator !=(Orientation3d a, Orientation3d b) {
			if (a.X != b.X) return true;
			if (a.Y != b.Y) return true;
			if (a.Z != b.Z) return true;
			return false;
		}
		
		public static implicit operator Orientation3d(Orientation3f orientation) {
			return new Orientation3d(orientation.X, orientation.Y, orientation.Z);
		}
		
		public static explicit operator Orientation3f(Orientation3d orientation) {
			return new Orientation3f((Vector3f)orientation.X, (Vector3f)orientation.Y, (Vector3f)orientation.Z);
		}
		
		
		// --- instance functions ---
		
		public Orientation3d Orthonormalize() {
			return Orientation3d.Orthonormalize(this);
		}
		
		
		// --- static functions ---
		
		public static Orientation3d Normalize(Orientation3d orientation) {
			return new Orientation3d(
				Vector3d.Normalize(orientation.X),
				Vector3d.Normalize(orientation.Y),
				Vector3d.Normalize(orientation.Z)
			);
		}
		
		public static Orientation3d Orthonormalize(Orientation3d orientation) {
			Vector3d x = Vector3d.Normalize(orientation.X);
			Vector3d y, z;
			if (x.IsZero()) {
				y = Vector3d.Normalize(orientation.Y);
				if (y.IsZero()) {
					z = Vector3d.Normalize(orientation.Z);
					if (z.IsZero()) {
						x = Vector3d.Right;
						y = Vector3d.Up;
						z = Vector3d.Forward;
					} else {
						x = Vector3d.Normalize(Vector3d.Cross(Vector3d.Up, z));
						if (x.IsZero()) {
							x = Vector3d.Right;
						}
						y = Vector3d.Cross(z, x);
					}
				} else {
					x = Vector3d.Normalize(Vector3d.Cross(y, orientation.Z));
					if (x.IsZero()) {
						x = Vector3d.Normalize(Vector3d.Cross(y, Vector3d.Forward));
						if (x.IsZero()) {
							x = Vector3d.Right;
						}
						z = Vector3d.Cross(x, y);
					} else {
						z = Vector3d.Cross(x, y);
					}
				}
			} else {
				y = Vector3d.Normalize(Vector3d.Cross(orientation.Z, x));
				if (y.IsZero()) {
					z = Vector3d.Normalize(Vector3d.Cross(x, orientation.Y));
					if (z.IsZero()) {
						y = Vector3d.Normalize(Vector3d.Cross(Vector3d.Forward, x));
						if (y.IsZero()) {
							y = Vector3d.Up;
						}
						z = Vector3d.Cross(x, y);
					} else {
						y = Vector3d.Cross(z, x);
					}
				} else {
					z = Vector3d.Cross(x, y);
				}
			}
			return new Orientation3d(x, y, z);
		}
		
		public static Orientation3d Transpose(Orientation3d orientation) {
			Vector3d x = new Vector3d(orientation.X.X, orientation.Y.X, orientation.Z.X);
			Vector3d y = new Vector3d(orientation.X.Y, orientation.Y.Y, orientation.Z.Y);
			Vector3d z = new Vector3d(orientation.X.Z, orientation.Y.Z, orientation.Z.Z);
			return new Orientation3d(x, y, z);
		}
		
		public static Orientation3d RotateAbsoluteXY(Orientation3d orientation, Vector2d angle) {
			Vector3d x = Vector3d.RotateXY(orientation.X, angle);
			Vector3d y = Vector3d.RotateXY(orientation.Y, angle);
			Vector3d z = Vector3d.RotateXY(orientation.Z, angle);
			return new Orientation3d(x, y, z);
		}

		public static Orientation3d RotateAbsoluteXZ(Orientation3d orientation, Vector2d angle) {
			Vector3d x = Vector3d.RotateXZ(orientation.X, angle);
			Vector3d y = Vector3d.RotateXZ(orientation.Y, angle);
			Vector3d z = Vector3d.RotateXZ(orientation.Z, angle);
			return new Orientation3d(x, y, z);
		}
		
		public static Orientation3d RotateAbsoluteYZ(Orientation3d orientation, Vector2d angle) {
			Vector3d x = Vector3d.RotateYZ(orientation.X, angle);
			Vector3d y = Vector3d.RotateYZ(orientation.Y, angle);
			Vector3d z = Vector3d.RotateYZ(orientation.Z, angle);
			return new Orientation3d(x, y, z);
		}
		
		public static Orientation3d RotateRelativeXY(Orientation3d orientation, Vector2d angle) {
			Vector3d z = orientation.Z;
			Vector3d x = Vector3d.Rotate(orientation.X, orientation.Z, angle);
			Vector3d y = Vector3d.Cross(z, x);
			return new Orientation3d(x, y, z);
		}
		
		public static Orientation3d RotateRelativeXZ(Orientation3d orientation, Vector2d angle) {
			Vector3d x = Vector3d.Rotate(orientation.X, orientation.Y, angle);
			Vector3d y = orientation.Y;
			Vector3d z = Vector3d.Cross(x, y);
			return new Orientation3d(x, y, z);
		}
		
		public static Orientation3d RotateRelativeYZ(Orientation3d orientation, Vector2d angle) {
			Vector3d x = orientation.X;
			Vector3d y = Vector3d.Rotate(orientation.Y, orientation.X, angle);
			Vector3d z = Vector3d.Cross(x, y);
			return new Orientation3d(x, y, z);
		}
		
		public static Orientation3d Rotate(Orientation3d orientation, Vector3d direction, Vector2d angle) {
			Vector3d x = Vector3d.Rotate(orientation.X, direction, angle);
			Vector3d y = Vector3d.Rotate(orientation.Y, direction, angle);
			Vector3d z = Vector3d.Cross(x, y);
			return new Orientation3d(x, y, z);
		}
		
		public static Orientation3d Rotate(Orientation3d orientation, Orientation3d relative) {
			Vector3d x = relative.X.X * orientation.X + relative.Y.X * orientation.Y + relative.Z.X * orientation.Z;
			Vector3d y = relative.X.Y * orientation.X + relative.Y.Y * orientation.Y + relative.Z.Y * orientation.Z;
			Vector3d z = Vector3d.Cross(x, y);
			return new Orientation3d(x, y, z);
		}
		
		public static Orientation3d Nlerp(Orientation3d p0, Orientation3d p1, double t) {
			return Orientation3d.Orthonormalize((1.0 - t) * p0 + t * p1);
		}

		
		// --- overrides and interface implementations ---
		
		public int CompareTo(Orientation3d other) {
			int value;
			value = this.X.CompareTo(other.X);
			if (value != 0) return value;
			value = this.Y.CompareTo(other.Y);
			if (value != 0) return value;
			value = this.Z.CompareTo(other.Z);
			if (value != 0) return value;
			return 0;
		}
		
		public bool Equals(Orientation3d other) {
			if (this.X != other.X) return false;
			if (this.Y != other.Y) return false;
			if (this.Z != other.Z) return false;
			return true;
		}
		
		public override bool Equals(object obj) {
			if (!(obj is Orientation3d)) return false;
			Orientation3d other = (Orientation3d)obj;
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