using System;
using TrainsimApi.Geometry;

namespace TrainsimApi.Codecs {
	public class MeshDecodingOptions {
		

		// --- members ---
		
		public TextureManager Manager;
		
		public ErrorLogger Logger;
		

		// --- constructors ---
		
		public MeshDecodingOptions() {
			this.Manager = null;
			this.Logger = null;
		}
		
		public MeshDecodingOptions(TextureManager manager, ErrorLogger logger) {
			this.Manager = manager;
			this.Logger = logger;
		}
		
		
	}
}
