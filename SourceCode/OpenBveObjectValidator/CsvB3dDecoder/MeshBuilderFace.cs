using System;
using TrainsimApi.Vectors;

namespace CsvB3dDecoder {
	internal class MeshBuilderFace {
		
		
		// --- members ---
		
		internal int[] Vertices;

		internal MeshBuilderFaceNormals Normals;
		
		internal float Alpha;
		
		internal Vector3f EmissiveColor;
		
		internal Vector3f ReflectiveColor;
		
		internal int LineNumber;
		
		
		// --- constructors ---
		
		internal MeshBuilderFace(int offset, int[] vertices, int lineNumber) {
			this.Vertices = new int[vertices.Length];
			this.Normals = MeshBuilderFaceNormals.Default;
			for (int i = 0; i < vertices.Length; i++) {
				this.Vertices[i] = vertices[i] + offset;
			}
			this.Alpha = 1.0f;
			this.EmissiveColor = Vector3f.Zero;
			this.ReflectiveColor = Vector3f.One;
			this.LineNumber = lineNumber;
		}
		
		
	}
}
