using System;
using System.Collections.Generic;
using System.Text;

using TrainsimApi.Codecs;
using TrainsimApi.Geometry;
using TrainsimApi.Vectors;

namespace OpenBveObjectValidator {
	internal static class Validator {
		
		
		internal static void CheckMeshFaces(Mesh mesh, ErrorLogger logger) {
			for (int f = 0; f < mesh.Faces.Length; f++) {
				if (mesh.Faces[f].Vertices.Length <= 2) {
					logger.Add("Face " + (f + 1).ToString() + " has less than 3 vertices.");
				} else {
					int ai = mesh.Faces[f].Vertices[0];
					int bi = mesh.Faces[f].Vertices[1];
					int ci = mesh.Faces[f].Vertices[2];
					Vector3f a = mesh.Vertices[ai].Position;
					Vector3f b = mesh.Vertices[bi].Position;
					Vector3f c = mesh.Vertices[ci].Position;
					Vector3f ab = b - a;
					Vector3f ac = c - a;
					Vector3f dx = Vector3f.Normalize(ab);
					Vector3f dy = Vector3f.Normalize(Vector3f.Cross(Vector3f.Cross(ab, ac), ab));
					Vector2f[] projection = new Vector2f[mesh.Faces[f].Vertices.Length];
					for (int i = 0; i < mesh.Faces[f].Vertices.Length; i++) {
						int v = mesh.Faces[f].Vertices[i];
						float x = Vector3f.Dot(mesh.Vertices[v].Position - a, dx);
						float y = Vector3f.Dot(mesh.Vertices[v].Position - a, dy);
						projection[i] = new Vector2f(x, y);
					}
					int winding = Math.Sign(GetWinding(projection, 0));
					for (int i = 1; i < mesh.Faces[f].Vertices.Length; i++) {
						int value = Math.Sign(GetWinding(projection, i));
						if (value != winding) {
							logger.Add("Face " + (f + 1).ToString() + " has an incorrect winding. This usually indicates coinciding vertices, a concave or complex polygon, or a non-planar face. Line number: " + mesh.Faces[f].LineNumber.ToString());
							break;
						}
					}
				}
			}
		}
		
		private static float GetWinding(Vector2f[] points, int index) {
			return GetWinding(points[(index - 1 + points.Length) % points.Length], points[index], points[(index + 1) % points.Length]);
		}
		
		private static float GetWinding(Vector2f a, Vector2f b, Vector2f c) {
			float ax = b.X - a.X;
			float ay = b.Y - a.Y;
			float bx = c.X - b.X;
			float by = c.Y - b.Y;
			return ax * by - ay * bx;
		}
		
		
	}
}
