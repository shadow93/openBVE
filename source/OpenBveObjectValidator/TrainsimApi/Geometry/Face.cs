using System;

namespace TrainsimApi.Geometry {
	public class Face : IEquatable<Face> {
		
		
		// --- members ---
		
		public int[] Vertices;
		
		public int Material;
		
		public int LineNumber;
		
		
		// --- constructors ---

		public Face(int[] vertices, int material, int lineNumber) {
			this.Vertices = vertices;
			this.Material = material;
			this.LineNumber = lineNumber;
		}
		
		
		// --- operators ---
		
		public static bool operator ==(Face a, Face b) {
			if (object.ReferenceEquals(a, b))    return true;
			if (object.ReferenceEquals(a, null)) return false;
			if (object.ReferenceEquals(b, null)) return false;
			if (a.Material != b.Material) return false;
			if (object.ReferenceEquals(a.Vertices, b.Vertices)) return true;
			if (object.ReferenceEquals(a.Vertices, null))       return false;
			if (object.ReferenceEquals(b.Vertices, null))       return false;
			if (a.Vertices.Length != b.Vertices.Length) return false;
			for (int i = 0; i < a.Vertices.Length; i++) {
				if (a.Vertices[i] != b.Vertices[i]) return false;
			}
			return true;
		}
		
		public static bool operator !=(Face a, Face b) {
			return !(a == b);
		}
		
		
		// --- overrides and interface implementations ---
		
		public int CompareTo(Face other) {
			if (object.ReferenceEquals(this, other)) return 0;
			if (object.ReferenceEquals(other, null)) return 1;
			if (this.Material < other.Material) return -1;
			if (this.Material > other.Material) return  1;
			if (object.ReferenceEquals(this.Vertices, other.Vertices)) return 0;
			if (object.ReferenceEquals(this.Vertices, null))           return -1;
			if (object.ReferenceEquals(other.Vertices, null))          return  1;
			if (this.Vertices.Length < other.Vertices.Length) return -1;
			if (this.Vertices.Length > other.Vertices.Length) return  1;
			for (int i = 0; i < this.Vertices.Length; i++) {
				if (this.Vertices[i] < other.Vertices[i]) return -1;
				if (this.Vertices[i] > other.Vertices[i]) return  1;
			}
			return 0;
		}
		
		public bool Equals(Face other) {
			return this == other;
		}
		
		public override bool Equals(object obj) {
			Face other = obj as Face;
			return this == other;
		}
		
		public override int GetHashCode() {
			int hashCode = 0;
			unchecked {
				if (Vertices != null) {
					hashCode += 1000000007 * Vertices.GetHashCode();
				}
				hashCode += 1000000009 * Material.GetHashCode();
			}
			return hashCode;
		}

		
	}
}