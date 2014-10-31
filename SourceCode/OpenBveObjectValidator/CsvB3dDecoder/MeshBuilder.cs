using System;
using System.Collections.Generic;
using TrainsimApi.Vectors;

namespace CsvB3dDecoder {
	internal class MeshBuilder {
		
		
		// --- members --
		
		internal List<MeshBuilderVertex> Vertices;
		
		internal List<MeshBuilderFace> Faces;
		
		internal string DaytimeTexture;
		
		internal bool TransparentColorUsed;
		
		internal Vector3b TransparentColor;
		
		
		// --- constructors ---
		
		internal MeshBuilder() {
			this.Vertices = new List<MeshBuilderVertex>();
			this.Faces = new List<MeshBuilderFace>();
			this.DaytimeTexture = null;
			this.TransparentColor = Vector3b.Black;
			this.TransparentColorUsed = false;
		}
		
		
		// --- transformation ---
		
		internal void Translate(Vector3d offset) {
			for (int i = 0; i < this.Vertices.Count; i++) {
				this.Vertices[i].Position += offset;
			}
		}
		
		internal void Scale(Vector3d factor) {
			Vector3d inverseFactor;
			if (factor.X == 0.0 | factor.Y == 0.0 | factor.Z == 0.0) {
				inverseFactor = new Vector3d(
					factor.X == 0.0 ? 1.0 : 0.0,
					factor.Y == 0.0 ? 1.0 : 0.0,
					factor.Z == 0.0 ? 1.0 : 0.0
				);
			} else {
				inverseFactor = 1.0 / factor;
			}
			for (int i = 0; i < this.Vertices.Count; i++) {
				this.Vertices[i].Position *= factor;
				this.Vertices[i].Normal = Vector3d.Normalize(this.Vertices[i].Normal * inverseFactor);
			}
			if (factor.X * factor.Y * factor.Z < 0.0) {
				for (int i = 0; i < this.Faces.Count; i++) {
					Array.Reverse(this.Faces[i].Vertices);
				}
			}
		}
		
		internal void Rotate(Vector3d direction, Vector2d angle) {
			for (int i = 0; i < this.Vertices.Count; i++) {
				this.Vertices[i].Position = Vector3d.Rotate(this.Vertices[i].Position, direction, angle);
				this.Vertices[i].Normal   = Vector3d.Rotate(this.Vertices[i].Normal,   direction, angle);
			}
		}
		
		internal void Shear(Vector3d direction, Vector3d shift, double ratio) {
			for (int i = 0; i < this.Vertices.Count; i++) {
				double positionFactor = ratio * Vector3d.Dot(this.Vertices[i].Position, direction);
				double normalFactor   = ratio * Vector3d.Dot(this.Vertices[i].Normal,   shift);
				this.Vertices[i].Position += shift * positionFactor;
				this.Vertices[i].Normal = Vector3d.Normalize(this.Vertices[i].Normal - direction * normalFactor);
				
			}
		}
		
		
		// --- geometry primitives ---
		
		internal void AddCube(double x, double y, double z, int lineNumber) {
			
			// --- vertices --
			int vertexOffset = this.Vertices.Count;
			this.Vertices.Add(new MeshBuilderVertex(new Vector3d( x,  y, -z)));
			this.Vertices.Add(new MeshBuilderVertex(new Vector3d( x, -y, -z)));
			this.Vertices.Add(new MeshBuilderVertex(new Vector3d(-x, -y, -z)));
			this.Vertices.Add(new MeshBuilderVertex(new Vector3d(-x,  y, -z)));
			this.Vertices.Add(new MeshBuilderVertex(new Vector3d( x,  y,  z)));
			this.Vertices.Add(new MeshBuilderVertex(new Vector3d( x, -y,  z)));
			this.Vertices.Add(new MeshBuilderVertex(new Vector3d(-x, -y,  z)));
			this.Vertices.Add(new MeshBuilderVertex(new Vector3d(-x,  y,  z)));
			
			// --- faces ---
			this.Faces.Add(new MeshBuilderFace(vertexOffset, new int[] { 0, 1, 2, 3 }, lineNumber));
			this.Faces.Add(new MeshBuilderFace(vertexOffset, new int[] { 0, 4, 5, 1 }, lineNumber));
			this.Faces.Add(new MeshBuilderFace(vertexOffset, new int[] { 0, 3, 7, 4 }, lineNumber));
			this.Faces.Add(new MeshBuilderFace(vertexOffset, new int[] { 6, 5, 4, 7 }, lineNumber));
			this.Faces.Add(new MeshBuilderFace(vertexOffset, new int[] { 6, 7, 3, 2 }, lineNumber));
			this.Faces.Add(new MeshBuilderFace(vertexOffset, new int[] { 6, 2, 1, 5 }, lineNumber));
			
		}
		
		internal void AddCylinder(int count, double upper, double lower, double height, int lineNumber) {
			
			// --- preparations ---
			bool upperCap = upper > 0.0;
			bool lowerCap = lower > 0.0;
			upper = Math.Abs(upper);
			lower = Math.Abs(lower);
			
			// --- vertices ---
			int vertexOffset = this.Vertices.Count;
			Vector3d planarDirection = Vector3d.Right;
			Vector3d slopedDirectionUpper;
			if (height != 0.0) {
				// TODO: Math.Sign(height)?
				double slopeAngle = Math.Atan((lower - upper) / height);
				slopedDirectionUpper = Math.Sign(height) * (new Vector3d(Math.Cos(slopeAngle), Math.Sin(slopeAngle), 0.0));
			} else {
				// TODO: Math.Sign(height)?
				slopedDirectionUpper = new Vector3d(0.0, Math.Sign(lower - upper), 0.0);
			}
			Vector3d slopedDirectionLower = slopedDirectionUpper;
			double rotateAngle = 2.0 * Math.PI / (double)count;
			Vector2d rotateVector = new Vector2d(0.5 * rotateAngle);
			if (upper == 0.0) {
				slopedDirectionUpper = Vector3d.RotateXZ(slopedDirectionUpper, rotateVector);
			}
			if (lower == 0.0) {
				slopedDirectionLower = Vector3d.RotateXZ(slopedDirectionLower, rotateVector);
			}
			rotateVector = Vector2d.Rotate(rotateVector, rotateVector);
			for (int i = 0; i < count; i++) {
				Vector3d positionUpper = new Vector3d(planarDirection.X * upper,  0.5 * height, planarDirection.Z * upper);
				Vector3d positionLower = new Vector3d(planarDirection.X * lower, -0.5 * height, planarDirection.Z * lower);
				this.Vertices.Add(new MeshBuilderVertex(positionUpper, slopedDirectionUpper, Vector2d.Zero));
				this.Vertices.Add(new MeshBuilderVertex(positionLower, slopedDirectionLower, Vector2d.Zero));
				planarDirection      = Vector3d.RotateXZ(planarDirection,      rotateVector);
				slopedDirectionUpper = Vector3d.RotateXZ(slopedDirectionUpper, rotateVector);
				slopedDirectionLower = Vector3d.RotateXZ(slopedDirectionLower, rotateVector);
			}
			
			// --- cylinder wall ---
			if (height != 0.0 & (lower != 0.0 | upper != 0.0)) {
				if (upper == 0.0) {
					for (int i = 0; i < count; i++) {
						int v0 = (2 * i + 2) % (2 * count);
						int v1 = (2 * i + 3) % (2 * count);
						int v2 = 2 * i + 1;
						this.Faces.Add(new MeshBuilderFace(vertexOffset, new int[] { v0, v1, v2 }, lineNumber));
					}
				} else if (lower == 0.0) {
					for (int i = 0; i < count; i++) {
						int v0 = (2 * i + 2) % (2 * count);
						int v2 = 2 * i + 1;
						int v3 = 2 * i;
						this.Faces.Add(new MeshBuilderFace(vertexOffset, new int[] { v0, v2, v3 }, lineNumber));
					}
				} else {
					for (int i = 0; i < count; i++) {
						int v0 = (2 * i + 2) % (2 * count);
						int v1 = (2 * i + 3) % (2 * count);
						int v2 = 2 * i + 1;
						int v3 = 2 * i;
						this.Faces.Add(new MeshBuilderFace(vertexOffset, new int[] { v0, v1, v2, v3 }, lineNumber));
					}
				}
			}
			
			// --- upper cap ---
			if (upperCap) {
				int[] vertices = new int[count];
				for (int i = 0; i < count; i++) {
					vertices[i] = 2 * i + 1;
				}
				MeshBuilderFace face = new MeshBuilderFace(vertexOffset, vertices, lineNumber);
				face.Normals = MeshBuilderFaceNormals.Generate;
				this.Faces.Add(face);
			}
			
			// --- lower cap ---
			if (lowerCap) {
				int[] vertices = new int[count];
				for (int i = 0; i < count; i++) {
					vertices[i] = 2 * (count - i - 1);
				}
				MeshBuilderFace face = new MeshBuilderFace(vertexOffset, vertices, lineNumber);
				face.Normals = MeshBuilderFaceNormals.Generate;
				this.Faces.Add(face);
			}
			
		}
		
		
	}
}