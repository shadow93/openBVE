using System;
using TrainsimApi.Vectors;

namespace TrainsimApi.Geometry {
	public class Material : IEquatable<Material> {
		
		
		// --- members ---
		
		public Vector3f EmissiveColor;
		
		public Vector3f ReflectiveColor;
		
		public float SpecularExponent;

		public float Alpha;
		
		public Texture Texture;
		
		
		// --- constructors ---
		
		public Material() {
			this.EmissiveColor = Vector3f.Black;
			this.ReflectiveColor = Vector3f.White;
			this.SpecularExponent = 1.0f;
			this.Alpha = 1.0f;
			this.Texture = null;
		}
		
		
		// --- operators ---
		
		public static bool operator ==(Material a, Material b) {
			if (object.ReferenceEquals(a, b))    return true;
			if (object.ReferenceEquals(a, null)) return false;
			if (object.ReferenceEquals(b, null)) return false;
			if (a.EmissiveColor    != b.EmissiveColor)    return false;
			if (a.ReflectiveColor  != b.ReflectiveColor)  return false;
			if (a.SpecularExponent != b.SpecularExponent) return false;
			if (a.Alpha            != b.Alpha)            return false;
			if (!object.ReferenceEquals(a.Texture, b.Texture)) {
				if (object.ReferenceEquals(a.Texture, null) || object.ReferenceEquals(b.Texture, null)) return false;
				if (!a.Texture.Equals(b.Texture)) return false;
			}
			return true;
		}
		
		public static bool operator !=(Material a, Material b) {
			return !(a == b);
		}
		
		
		// --- overrides and interface implementations ---
		
		public bool Equals(Material other) {
			return this == other;
		}
		
		public override bool Equals(object obj) {
			Material other = obj as Material;
			return this == other;
		}
		
		public override int GetHashCode() {
			int hashCode = 0;
			unchecked {
				hashCode += 1000000007 * EmissiveColor.GetHashCode();
				hashCode += 1000000009 * ReflectiveColor.GetHashCode();
				hashCode += 1000000021 * SpecularExponent.GetHashCode();
				hashCode += 1000000033 * Alpha.GetHashCode();
				if (Texture != null) {
					hashCode += 1000000087 * Texture.GetHashCode();
				}
			}
			return hashCode;
		}

		
	}
}