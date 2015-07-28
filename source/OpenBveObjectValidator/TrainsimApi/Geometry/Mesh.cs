using System;

namespace TrainsimApi.Geometry {
	public class Mesh {
		
		
		// --- members --
		
		public Vertex[] Vertices;
		
		public Material[] Materials;

		public Face[] Faces;
		
		
		// --- constructors ---
		
		public Mesh(Vertex[] vertices, Material[] materials, Face[] faces) {
			this.Vertices = vertices;
			this.Materials = materials;
			this.Faces = faces;
		}
		
		
		// --- read-only fields ---
		
		public static readonly Mesh Empty = new Mesh(new Vertex[] { }, new Material[] { }, new Face[] { });
		
		
		// --- functions ---
		
		public bool Validate() {
			for (int v = 0; v < this.Vertices.Length; v++) {
				if (object.ReferenceEquals(this.Vertices[v], null)) return false;
			}
			for (int m = 0; m < this.Materials.Length; m++) {
				if (object.ReferenceEquals(this.Materials[m], null)) return false;
			}
			for (int f = 0; f < this.Faces.Length; f++) {
				if (object.ReferenceEquals(this.Faces[f], null)) return false;
				if (object.ReferenceEquals(this.Faces[f].Vertices, null)) return false;
				if (this.Faces[f].Vertices.Length < 3) return false;
				for (int v = 0; v < this.Faces[f].Vertices.Length; v++) {
					if (this.Faces[f].Vertices[v] < 0) return false;
					if (this.Faces[f].Vertices[v] >= this.Vertices.Length) return false;
				}
				if (this.Faces[f].Material < 0) return false;
				if (this.Faces[f].Material >= this.Materials.Length) return false;
			}
			return true;
		}
		
		
	}
}