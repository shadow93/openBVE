using System;
using TrainsimApi.Geometry;

namespace TrainsimApi.Codecs {
	public abstract class MeshDecoder {
		
		
		// --- functions ---
		
		public abstract bool CanLoad(string file);
		
		public abstract Mesh Load(string file, MeshDecodingOptions options);
		
		
	}
}