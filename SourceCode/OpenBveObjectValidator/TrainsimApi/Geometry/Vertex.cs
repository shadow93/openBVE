using System;
using TrainsimApi.Vectors;

namespace TrainsimApi.Geometry {
	public class Vertex : IComparable<Vertex>, IEquatable<Vertex> {
		
		
		// --- members ---
		
		public Vector3f Position;
		
		public Vector3f Normal;
		
		public Vector2f Texture;
		
		public Vector3f Color;
		
		
		// --- constructors ---
		
		public Vertex(Vector3f position) {
			this.Position = position;
			this.Normal = Vector3f.Zero;
			this.Texture = Vector2f.Zero;
			this.Color = Vector3f.White;
		}
		
		public Vertex(Vector3f position, Vector3f normal) {
			this.Position = position;
			this.Normal = normal;
			this.Texture = Vector2f.Zero;
			this.Color = Vector3f.White;
		}
		
		public Vertex(Vector3f position, Vector3f normal, Vector2f texture) {
			this.Position = position;
			this.Normal = normal;
			this.Texture = texture;
			this.Color = Vector3f.White;
		}
		
		public Vertex(Vector3f position, Vector3f normal, Vector2f texture, Vector3f color) {
			this.Position = position;
			this.Normal = normal;
			this.Texture = texture;
			this.Color = color;
		}
		
		
		// --- operators ---
		
		public static bool operator ==(Vertex a, Vertex b) {
			if (object.ReferenceEquals(a, b)) return true;
			if (object.ReferenceEquals(a, null)) return false;
			if (object.ReferenceEquals(b, null)) return false;
			if (a.Position != b.Position) return false;
			if (a.Normal   != b.Normal)   return false;
			if (a.Texture  != b.Texture)  return false;
			if (a.Color    != b.Color)    return false;
			return true;
		}

		public static bool operator !=(Vertex a, Vertex b) {
			return !(a == b);
		}
		
		
		// --- overrides and interface implementations ---
		
		public int CompareTo(Vertex other) {
			if (object.ReferenceEquals(this, other)) return 0;
			if (object.ReferenceEquals(other, null)) return 1;
			int value;
			value = this.Position.CompareTo(other.Position);
			if (value != 0) return value;
			value = this.Normal.CompareTo(other.Normal);
			if (value != 0) return value;
			value = this.Texture.CompareTo(other.Texture);
			if (value != 0) return value;
			value = this.Color.CompareTo(other.Color);
			if (value != 0) return value;
			return 0;
		}

		public bool Equals(Vertex other) {
			return this == other;
		}
		
		public override bool Equals(object obj) {
			Vertex other = obj as Vertex;
			return this == other;
		}
		
		public override int GetHashCode() {
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * Position.GetHashCode();
				hashCode += 1000000009 * Normal.GetHashCode();
				hashCode += 1000000021 * Texture.GetHashCode();
				hashCode += 1000000033 * Color.GetHashCode();
			}
			return hashCode;
		}

		
	}
}