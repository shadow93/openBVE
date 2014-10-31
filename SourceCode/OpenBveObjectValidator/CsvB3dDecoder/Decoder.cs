using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

using TrainsimApi;
using TrainsimApi.Codecs;
using TrainsimApi.Geometry;
using TrainsimApi.Vectors;

namespace CsvB3dDecoder {
	public partial class Decoder : MeshDecoder {
		
		
		// --- public functions ---
		
		public override bool CanLoad(string file) {
			if (file.EndsWith(".b3d", StringComparison.OrdinalIgnoreCase)) {
				return true;
			} else if (file.EndsWith(".csv", StringComparison.OrdinalIgnoreCase)) {
				return true;
			} else {
				return false;
			}
		}
		
		public override Mesh Load(string file, MeshDecodingOptions options) {
			const bool strictParsing = true;
			
			// --- preparations ---
			bool isB3d = file.EndsWith(".b3d", StringComparison.OrdinalIgnoreCase);
			char commandArgumentSeparator = isB3d ? ' ' : ',';
			FileInformation fileInfo = new FileInformation(isB3d, file, strictParsing, options.Logger);
			List<MeshBuilder> meshBuilders = new List<MeshBuilder>();
			MeshBuilder currentMeshBuilder = new MeshBuilder();
			bool meshBuilderPresent = false;
			
			// --- line by line ---
			string[] lines = File.ReadAllLines(file, Encoding.UTF8);
			for (int l = 0; l < lines.Length; l++) {
				string line = lines[l];
				
				// --- trim comments and whitespace ---
				int semicolon = line.IndexOf(';');
				if (semicolon >= 0) {
					line = line.Substring(0, semicolon).Trim();
				} else {
					line = line.Trim();
				}
				if (line.Length != 0) {
					
					// --- split into command and argument sequence ---
					string command;
					string argumentSequence;
					int separator = line.IndexOf(commandArgumentSeparator);
					if (separator >= 0) {
						command = line.Substring(0, separator).TrimEnd();
						argumentSequence = line.Substring(separator + 1).TrimStart();
					} else {
						command = line;
						argumentSequence = string.Empty;
					}
					
					// --- handle malformed commands ---
					if (command.Length != 0) {
						if (isB3d) {
							int comma = command.IndexOf(',');
							if (comma >= 0) {
								argumentSequence = command.Substring(comma + 1).TrimStart() + ',' + argumentSequence;
								command = command.Substring(0, comma).TrimEnd();
								if (strictParsing) {
									string text = "Command \"" + command + "\" must be separated from its arguments by a space in B3D files on line " + (l + 1).ToString() + " in file \"" + file + "\".";
									options.Logger.Add(text);
								}
							}
						} else {
							int space = command.IndexOf(' ');
							if (space >= 0) {
								argumentSequence = command.Substring(space + 1).TrimStart() + ',' + argumentSequence;
								command = command.Substring(0, space).TrimEnd();
								if (strictParsing) {
									string text = "Command \"" + command + "\" must be separated from its arguments by a comma in CSV files on line " + (l + 1).ToString() + " in file \"" + file + "\".";
									options.Logger.Add(text);
								}
							}
						}
					}
					if (command.Length != 0) {
						
						// --- split into arguments and trim empty arguments from the end ---
						string[] arguments = argumentSequence.Split(',');
						for (int a = 0; a < arguments.Length; a++) {
							arguments[a] = arguments[a].Trim();
						}
						int argumentCount = 0;
						for (int a = arguments.Length - 1; a >= 0; a--) {
							if (arguments[a].Length != 0) {
								argumentCount = a + 1;
								break;
							}
						}

						// --- process commands ---
						LineInformation lineInfo = new LineInformation(command, arguments, argumentCount, l + 1, fileInfo);
						string commandLower = command.ToLowerInvariant();
						switch (commandLower) {
							case "[meshbuilder]":
							case "createmeshbuilder":
								{
									if (strictParsing) {
										CheckCommand(command, "[MeshBuilder]", "CreateMeshBuilder", lineInfo);
										CheckArgumentCount(command, argumentCount, 0, 0, lineInfo);
									}
									meshBuilders.Add(currentMeshBuilder);
									currentMeshBuilder = new MeshBuilder();
									meshBuilderPresent = true;
								}
								break;
							case "vertex":
							case "addvertex":
								{
									if (strictParsing) {
										CheckMeshBuilderPresence(command, lineInfo, ref meshBuilderPresent);
										CheckCommand(command, "Vertex", "AddVertex", lineInfo);
										CheckArgumentCount(command, argumentCount, 0, 8, lineInfo);
									}
									double positionX = GetDoubleFromArgument(0, "positionX", 0.0, lineInfo);
									double positionY = GetDoubleFromArgument(1, "positionY", 0.0, lineInfo);
									double positionZ = GetDoubleFromArgument(2, "positionZ", 0.0, lineInfo);
									double normalX   = GetDoubleFromArgument(3, "normalX",   0.0, lineInfo);
									double normalY   = GetDoubleFromArgument(4, "normalY",   0.0, lineInfo);
									double normalZ   = GetDoubleFromArgument(5, "normalZ",   0.0, lineInfo);
									double textureX  = GetDoubleFromArgument(6, "textureX",  0.0, lineInfo);
									double textureY  = GetDoubleFromArgument(7, "textureY",  0.0, lineInfo);
									MeshBuilderVertex vertex = new MeshBuilderVertex(new Vector3d(positionX, positionY, positionZ), new Vector3d(normalX, normalY, normalZ), new Vector2d(textureX, textureY));
									currentMeshBuilder.Vertices.Add(vertex);
								}
								break;
							case "face":
							case "face2":
							case "addface":
							case "addface2":
								{
									if (strictParsing) {
										CheckMeshBuilderPresence(command, lineInfo, ref meshBuilderPresent);
									}
									bool face2 = commandLower[commandLower.Length - 1] == '2';
									if (strictParsing) {
										string suffix = face2 ? "2" : string.Empty;
										CheckCommand(command, "Face" + suffix, "AddFace" + suffix, lineInfo);
									}
									bool success = CheckArgumentCount(command, argumentCount, 3, int.MaxValue, lineInfo);
									int[] indices = new int[argumentCount];
									for (int i = 0; i < argumentCount; i++) {
										if (!TryGetInt32FromArgument(i, "vertex" + (i + 1).ToString(), 0, currentMeshBuilder.Vertices.Count - 1, -1, false, lineInfo, out indices[i])) {
											success = false;
										}
									}
									if (success) {
										currentMeshBuilder.Faces.Add(new MeshBuilderFace(0, indices, lineInfo.LineNumber));
										if (face2) {
											Array.Reverse(indices);
											MeshBuilderFace face = new MeshBuilderFace(0, indices, lineInfo.LineNumber);
											face.Normals = MeshBuilderFaceNormals.Inverse;
											currentMeshBuilder.Faces.Add(face);
										}
									}
								}
								break;
							case "cube":
								{
									if (strictParsing) {
										CheckMeshBuilderPresence(command, lineInfo, ref meshBuilderPresent);
										CheckArgumentCount(command, argumentCount, 0, 3, lineInfo);
									}
									double x = GetDoubleFromArgument(0, "x", 0.0, lineInfo);
									double y = GetDoubleFromArgument(1, "y", x,   lineInfo);
									double z = GetDoubleFromArgument(2, "z", x,   lineInfo);
									currentMeshBuilder.AddCube(x, y, z, lineInfo.LineNumber);
								}
								break;
							case "cylinder":
								{
									if (strictParsing) {
										CheckMeshBuilderPresence(command, lineInfo, ref meshBuilderPresent);
										CheckArgumentCount(command, argumentCount, 1, 4, lineInfo);
									}
									int numVertices;
									bool success = TryGetInt32FromArgument(0, "numVertices", 2, int.MaxValue, 0, false, lineInfo, out numVertices);
									double upper  = GetDoubleFromArgument(1, "upper",  0.0, lineInfo);
									double lower  = GetDoubleFromArgument(2, "lower",  0.0, lineInfo);
									double height = GetDoubleFromArgument(3, "height", 0.0, lineInfo);
									if (success) {
										currentMeshBuilder.AddCylinder(numVertices, upper, lower, height, lineInfo.LineNumber);
									}
								}
								break;
							case "[texture]":
							case "generatenormals":
								{
									if (strictParsing) {
										CheckMeshBuilderPresence(command, lineInfo, ref meshBuilderPresent);
										CheckCommand(command, "[Texture]", "GenerateNormals", lineInfo);
										CheckArgumentCount(command, argumentCount, 0, 0, lineInfo);
									}
								}
								break;
							case "translate":
							case "translateall":
								{
									if (strictParsing) {
										CheckMeshBuilderPresence(command, lineInfo, ref meshBuilderPresent);
										CheckArgumentCount(command, argumentCount, 0, 3, lineInfo);
									}
									double x = GetDoubleFromArgument(0, "x", 0.0, lineInfo);
									double y = GetDoubleFromArgument(1, "y", 0.0, lineInfo);
									double z = GetDoubleFromArgument(2, "z", 0.0, lineInfo);
									Vector3d offset = new Vector3d(x, y, z);
									currentMeshBuilder.Translate(offset);
									if (commandLower == "translateall") {
										foreach (MeshBuilder meshBuilder in meshBuilders) {
											meshBuilder.Translate(offset);
										}
									}
								}
								break;
							case "scale":
							case "scaleall":
								{
									if (strictParsing) {
										CheckMeshBuilderPresence(command, lineInfo, ref meshBuilderPresent);
										CheckArgumentCount(command, argumentCount, 0, 3, lineInfo);
									}
									double x = GetDoubleFromArgument(0, "x", 1.0, lineInfo);
									double y = GetDoubleFromArgument(1, "y", 1.0, lineInfo);
									double z = GetDoubleFromArgument(2, "z", 1.0, lineInfo);
									Vector3d factor = new Vector3d(x, y, z);
									currentMeshBuilder.Scale(factor);
									if (commandLower == "scaleall") {
										foreach (MeshBuilder meshBuilder in meshBuilders) {
											meshBuilder.Scale(factor);
										}
									}
								}
								break;
							case "rotate":
							case "rotateall":
								{
									if (strictParsing) {
										CheckMeshBuilderPresence(command, lineInfo, ref meshBuilderPresent);
										CheckArgumentCount(command, argumentCount, 0, 4, lineInfo);
									}
									double x = GetDoubleFromArgument(0, "x",     0.0, lineInfo);
									double y = GetDoubleFromArgument(1, "y",     0.0, lineInfo);
									double z = GetDoubleFromArgument(2, "z",     0.0, lineInfo);
									double a = GetDoubleFromArgument(3, "angle", 0.0, lineInfo) * (Math.PI / 180.0);
									Vector3d direction = Vector3d.Normalize(new Vector3d(x, y, z));
									if (direction.IsZero()) {
										direction = Vector3d.Right;
									}
									Vector2d angle = new Vector2d(Math.Cos(a), Math.Sin(a));
									currentMeshBuilder.Rotate(direction, angle);
									if (commandLower == "rotateall") {
										foreach (MeshBuilder meshBuilder in meshBuilders) {
											meshBuilder.Rotate(direction, angle);
										}
									}
								}
								break;
							case "shear":
							case "shearall":
								{
									if (strictParsing) {
										CheckMeshBuilderPresence(command, lineInfo, ref meshBuilderPresent);
										CheckArgumentCount(command, argumentCount, 0, 7, lineInfo);
									}
									double dx = GetDoubleFromArgument(0, "dx",    0.0, lineInfo);
									double dy = GetDoubleFromArgument(1, "dy",    0.0, lineInfo);
									double dz = GetDoubleFromArgument(2, "dz",    0.0, lineInfo);
									double sx = GetDoubleFromArgument(3, "sx",    0.0, lineInfo);
									double sy = GetDoubleFromArgument(4, "sy",    0.0, lineInfo);
									double sz = GetDoubleFromArgument(5, "sz",    0.0, lineInfo);
									double r  = GetDoubleFromArgument(6, "ratio", 0.0, lineInfo);
									Vector3d direction = Vector3d.Normalize(new Vector3d(dx, dy, dz));
									Vector3d shift     = Vector3d.Normalize(new Vector3d(sx, sy, sz));
									currentMeshBuilder.Shear(direction, shift, r);
									if (commandLower == "shearall") {
										foreach (MeshBuilder meshBuilder in meshBuilders) {
											meshBuilder.Shear(direction, shift, r);
										}
									}
								}
								break;
							case "color":
							case "setcolor":
								{
									if (strictParsing) {
										CheckMeshBuilderPresence(command, lineInfo, ref meshBuilderPresent);
										CheckCommand(command, "Color", "SetColor", lineInfo);
										CheckArgumentCount(command, argumentCount, 0, 4, lineInfo);
									}
									double r, g, b, a;
									TryGetDoubleFromArgument(0, "red",   0.0, 255.0, 255.0, true, lineInfo, out r);
									TryGetDoubleFromArgument(1, "green", 0.0, 255.0, 255.0, true, lineInfo, out g);
									TryGetDoubleFromArgument(2, "blue",  0.0, 255.0, 255.0, true, lineInfo, out b);
									TryGetDoubleFromArgument(3, "alpha", 0.0, 255.0, 255.0, true, lineInfo, out a);
									Vector3f color = new Vector3f((float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f);
									for (int i = 0; i < currentMeshBuilder.Faces.Count; i++) {
										currentMeshBuilder.Faces[i].ReflectiveColor = color;
										currentMeshBuilder.Faces[i].Alpha = (float)a / 255.0f;
									}
								}
								break;
							case "emissivecolor":
							case "setemissivecolor":
								{
									if (strictParsing) {
										CheckMeshBuilderPresence(command, lineInfo, ref meshBuilderPresent);
										CheckCommand(command, "EmissiveColor", "SetEmissiveColor", lineInfo);
										CheckArgumentCount(command, argumentCount, 0, 3, lineInfo);
									}
									double r, g, b;
									TryGetDoubleFromArgument(0, "red",   0.0, 255.0, 0.0, true, lineInfo, out r);
									TryGetDoubleFromArgument(1, "green", 0.0, 255.0, 0.0, true, lineInfo, out g);
									TryGetDoubleFromArgument(2, "blue",  0.0, 255.0, 0.0, true, lineInfo, out b);
									Vector3f color = new Vector3f((float)r / 255.0f, (float)g / 255.0f, (float)b / 255.0f);
									for (int i = 0; i < currentMeshBuilder.Faces.Count; i++) {
										currentMeshBuilder.Faces[i].EmissiveColor = color;
									}
								}
								break;
							case "blendmode":
							case "setblendmode":
								{
									if (strictParsing) {
										CheckMeshBuilderPresence(command, lineInfo, ref meshBuilderPresent);
										CheckCommand(command, "BlendMode", "SetBlendMode", lineInfo);
										CheckArgumentCount(command, argumentCount, 0, 3, lineInfo);
									}
									string blendMode = argumentCount >= 1 && arguments[0].Length != 0 ? arguments[0] : "normal";
									double glowHalfDistance;
									TryGetDoubleFromArgument(1, "glowHalfDistance", 0.0, double.MaxValue, 0.0, true, lineInfo, out glowHalfDistance);
									string glowAttenuationMode = argumentCount >= 3 && arguments[2].Length != 0 ? arguments[2] : "divideexponent2";
									switch (blendMode.ToLowerInvariant()) {
										case "normal":
											break;
										case "additive":
											break;
										default:
											string text = "\"" + blendMode + "\" as argument 1 to command " + lineInfo.Command + " is not a valid blend mode on line " + lineInfo.LineNumber.ToString() + " in file \"" + lineInfo.FileInfo.File + "\".";
											options.Logger.Add(text);
											break;
									}
									switch (glowAttenuationMode.ToLowerInvariant()) {
										case "divideexponent2":
											break;
										case "divideexponent4":
											break;
										default:
											string text = "\"" + blendMode + "\" as argument 3 to command " + lineInfo.Command + " is not a valid glow attenuation mode on line " + lineInfo.LineNumber.ToString() + " in file \"" + lineInfo.FileInfo.File + "\".";
											options.Logger.Add(text);
											break;
									}
									
									// TODO: Not implemented.
									
								}
								break;
							case "load":
							case "loadtexture":
								{
									if (strictParsing) {
										CheckMeshBuilderPresence(command, lineInfo, ref meshBuilderPresent);
										CheckCommand(command, "Load", "LoadTexture", lineInfo);
										CheckArgumentCount(command, argumentCount, 1, 2, lineInfo);
									}
									string daytimeTexture = null;
									string nighttimeTexture = null;
									if (argumentCount >= 1 && arguments[0].Length != 0) {
										daytimeTexture = arguments[0];
									}
									if (argumentCount >= 2 && arguments[1].Length != 0) {
										nighttimeTexture = arguments[1];
									}
									if (daytimeTexture != null) {
										currentMeshBuilder.DaytimeTexture = Platform.CombineFile(Path.GetDirectoryName(file), daytimeTexture);
									}
									
									// TODO: Nighttime textures.
									
								}
								break;
							case "transparent":
							case "setdecaltransparentcolor":
								{
									if (strictParsing) {
										CheckMeshBuilderPresence(command, lineInfo, ref meshBuilderPresent);
										CheckCommand(command, "Transparent", "SetDecalTransparentColor", lineInfo);
										CheckArgumentCount(command, argumentCount, 0, 3, lineInfo);
									}
									int r, g, b;
									TryGetInt32FromArgument(0, "red",   0, 255, 0, true, lineInfo, out r);
									TryGetInt32FromArgument(1, "green", 0, 255, 0, true, lineInfo, out g);
									TryGetInt32FromArgument(2, "blue",  0, 255, 0, true, lineInfo, out b);
									currentMeshBuilder.TransparentColor = new Vector3b((byte)r, (byte)g, (byte)b);
									currentMeshBuilder.TransparentColorUsed = true;
								}
								break;
							case "coordinates":
							case "settexturecoordinates":
								{
									if (strictParsing) {
										CheckMeshBuilderPresence(command, lineInfo, ref meshBuilderPresent);
										CheckCommand(command, "Coordinates", "SetTextureCoordinates", lineInfo);
										CheckArgumentCount(command, argumentCount, 1, 3, lineInfo);
									}
									int v = -1;
									bool success;
									if (currentMeshBuilder.Vertices.Count == 0) {
										string text = lineInfo.Command + " cannot be used at this point because no vertices were defined in the mesh builder on line " + lineInfo.LineNumber.ToString() + " in file \"" + lineInfo.FileInfo.File + "\".";
										options.Logger.Add(text);
										success = false;
									} else {
										success = TryGetInt32FromArgument(0, "vertex", 0, currentMeshBuilder.Vertices.Count - 1, 0, false, lineInfo, out v);
									}
									double x = GetDoubleFromArgument(1, "x", 0.0, lineInfo);
									double y = GetDoubleFromArgument(2, "y", 0.0, lineInfo);
									if (success) {
										currentMeshBuilder.Vertices[v].Texture = new Vector2d(x, y);
									}
								}
								break;
							default:
								{
									string text = "Unsupported command \"" + command + "\" found on line " + (l + 1).ToString() + " in file \"" + file + "\".";
									options.Logger.Add(text);
								}
								break;
						}
					}
				}
			}
			meshBuilders.Add(currentMeshBuilder);
			currentMeshBuilder = null;
			
			// --- assemble mesh ---
			Dictionary<Vertex, int> hashedVertices = new Dictionary<Vertex, int>();
			Dictionary<Material, int> hashedMaterials = new Dictionary<Material, int>();
			List<Vertex> vertices = new List<Vertex>();
			List<Material> materials = new List<Material>();
			List<Face> faces = new List<Face>();
			foreach (MeshBuilder meshBuilder in meshBuilders) {
				foreach (MeshBuilderFace meshBuilderFace in meshBuilder.Faces) {
					
					// --- generate normal ---
					Vector3d normal;
					{
						Vector3d a = meshBuilder.Vertices[meshBuilderFace.Vertices[0]].Position;
						Vector3d b = meshBuilder.Vertices[meshBuilderFace.Vertices[1]].Position;
						Vector3d c = meshBuilder.Vertices[meshBuilderFace.Vertices[2]].Position;
						normal = Vector3d.Normalize(Vector3d.Cross(b - a, c - a));
						if (normal.IsZero()) {
							normal = Vector3d.Up;
						}
					}
					
					// --- vertices ---
					Vertex[] v = new Vertex[meshBuilderFace.Vertices.Length];
					for (int i = 0; i < meshBuilderFace.Vertices.Length; i++) {
						MeshBuilderVertex meshBuilderVertex = meshBuilder.Vertices[meshBuilderFace.Vertices[i]];
						v[i] = new Vertex((Vector3f)meshBuilderVertex.Position, (Vector3f)meshBuilderVertex.Normal, (Vector2f)meshBuilderVertex.Texture);
						if (meshBuilderFace.Normals == MeshBuilderFaceNormals.Generate || meshBuilderVertex.Normal.IsZero()) {
							v[i].Normal = (Vector3f)normal;
						} else if (meshBuilderFace.Normals == MeshBuilderFaceNormals.Inverse) {
							v[i].Normal = (Vector3f)(-meshBuilderVertex.Normal);
						}
					}
					int[] vertexIndices = new int[v.Length];
					for (int i = 0; i < v.Length; i++) {
						if (!hashedVertices.TryGetValue(v[i], out vertexIndices[i])) {
							vertexIndices[i] = vertices.Count;
							vertices.Add(v[i]);
							hashedVertices.Add(v[i], vertexIndices[i]);
						}
					}
					
					// --- material ---
					Material material = new Material();
					material.Alpha = meshBuilderFace.Alpha;
					material.EmissiveColor = meshBuilderFace.EmissiveColor;
					material.ReflectiveColor = meshBuilderFace.ReflectiveColor;
					if (meshBuilder.DaytimeTexture != null) {
						if (meshBuilder.TransparentColorUsed) {
							material.Texture = options.Manager.Add(meshBuilder.DaytimeTexture, meshBuilder.TransparentColor);
						} else {
							material.Texture = options.Manager.Add(meshBuilder.DaytimeTexture);
						}
					}
					int materialIndex;
					if (!hashedMaterials.TryGetValue(material, out materialIndex)) {
						materialIndex = materials.Count;
						materials.Add(material);
						hashedMaterials.Add(material, materialIndex);
					}
					
					// --- face ---
					Face face = new Face(vertexIndices, materialIndex, meshBuilderFace.LineNumber);
					faces.Add(face);
					
				}
			}
			return new Mesh(vertices.ToArray(), materials.ToArray(), faces.ToArray());
		}

		
	}
}