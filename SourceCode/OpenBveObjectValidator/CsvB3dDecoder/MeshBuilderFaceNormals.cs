using System;

namespace CsvB3dDecoder {
	internal enum MeshBuilderFaceNormals {
		
		
		/// <summary>The normals assigned to the vertices shall be used.</summary>
		Default,
		
		/// <summary>The inverse of the normals assigned to the vertices shall be used.</summary>
		Inverse,
		
		/// <summary>The normals shall be generated from the face vertices.</summary>
		Generate
			
			
	}
}
