using System;
using System.Drawing;

namespace TrainsimApi.Geometry {
	public abstract class Texture : IEquatable<Texture> {
		
		
		// --- functions ---
		
		public abstract bool Equals(Texture other);
		
		public override abstract bool Equals(object obj);
		
		public override abstract int GetHashCode();
		
		
	}
}