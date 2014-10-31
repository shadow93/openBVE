using System;

namespace TrainsimApi.Geometry {
	public abstract class Object3D {
		
		
		// --- functions ---
		
		public abstract bool Equals(Object3D other);
		
		public override abstract bool Equals(object obj);
		
		public override abstract int GetHashCode();
		
		
	}
}