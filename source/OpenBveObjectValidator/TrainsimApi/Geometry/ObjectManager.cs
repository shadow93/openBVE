using System;

namespace TrainsimApi.Geometry {
	public abstract class ObjectManager {
		
		
		// --- functions ---

		public abstract Object3D Add(string file);
		
		public abstract Object3D Add(Mesh mesh);
		
		
	}
}