using System;
using System.Drawing;
using TrainsimApi.Vectors;

namespace TrainsimApi.Geometry {
 	public abstract class TextureManager {
		
		
		// --- functions ---
		
		public abstract Texture Add(string file);
		
		public abstract Texture Add(string file, Vector3b transparentColor);
		
		public abstract Texture Add(Bitmap bitmap);
		
		
	}
}
