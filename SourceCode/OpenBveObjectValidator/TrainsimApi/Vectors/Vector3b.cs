using System;
using System.Runtime.InteropServices;

namespace TrainsimApi.Vectors {
	[StructLayout(LayoutKind.Sequential, Pack = 1)]
	public struct Vector3b : IComparable<Vector3b>, IEquatable<Vector3b> {
		
		
		// --- members ---
		
		public byte X;
		
		public byte Y;
		
		public byte Z;
		
		
		// --- constructors ---
		
		public Vector3b(byte x, byte y, byte z) {
			this.X = x;
			this.Y = y;
			this.Z = z;
		}
		
		
		// --- readonly fields (colors) ---
		
		public static readonly Vector3b Black    = new Vector3b(  0,   0,   0);
		
		public static readonly Vector3b Red      = new Vector3b(255,   0,   0);
		
		public static readonly Vector3b Green    = new Vector3b(  0, 255,   0);
		
		public static readonly Vector3b Blue     = new Vector3b(  0,   0, 255);
		
		public static readonly Vector3b Cyan     = new Vector3b(  0, 255, 255);
		
		public static readonly Vector3b Magenta  = new Vector3b(255,   0, 255);
		
		public static readonly Vector3b Yellow   = new Vector3b(255, 255,   0);
		
		public static readonly Vector3b White    = new Vector3b(255, 255, 255);
		
		
		// --- operators ---
		
		public static bool operator ==(Vector3b a, Vector3b b) {
			if (a.X != b.X) return false;
			if (a.Y != b.Y) return false;
			if (a.Z != b.Z) return false;
			return true;
		}
		
		public static bool operator !=(Vector3b a, Vector3b b) {
			if (a.X != b.X) return true;
			if (a.Y != b.Y) return true;
			if (a.Z != b.Z) return true;
			return false;
		}
		
		
		// --- overrides and interface implementations ---
		
		public int CompareTo(Vector3b other) {
			if (this.X < other.X) return -1;
			if (this.X > other.X) return  1;
			if (this.Y < other.Y) return -1;
			if (this.Y > other.Y) return  1;
			if (this.Z < other.Z) return -1;
			if (this.Z > other.Z) return  1;
			return 0;
		}

		public bool Equals(Vector3b other) {
			if (this.X != other.X) return false;
			if (this.Y != other.Y) return false;
			if (this.Z != other.Z) return false;
			return true;
		}
		
		public override bool Equals(object obj) {
			if (!(obj is Vector3b)) return false;
			Vector3b other = (Vector3b)obj;
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