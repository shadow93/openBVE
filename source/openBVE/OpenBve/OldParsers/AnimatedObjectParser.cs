using System;
using OpenBveApi.Math;
using OpenBveApi;

namespace OpenBve {
	internal static class AnimatedObjectParser {
		private static readonly System.Globalization.CultureInfo Culture = System.Globalization.CultureInfo.InvariantCulture;

		// parse animated object config
		/// <summary>Loads a collection of animated objects from a file.</summary>
		/// <param name="FileName">The text file to load the animated object from. Must be an absolute file name.</param>
		/// <param name="Encoding">The encoding the file is saved in. If the file uses a byte order mark, the encoding indicated by the byte order mark is used and the Encoding parameter is ignored.</param>
		/// <param name="LoadMode">The texture load mode.</param>
		/// <returns>The collection of animated objects.</returns>
		internal static ObjectManager.AnimatedObjectCollection ReadObject(string FileName, System.Text.Encoding Encoding, ObjectManager.ObjectLoadMode LoadMode) {
			ObjectManager.AnimatedObjectCollection Result = new ObjectManager.AnimatedObjectCollection();
			Result.Objects = new ObjectManager.AnimatedObject[4];
			int ObjectCount = 0;
			// load file
			string[] Lines = System.IO.File.ReadAllLines(FileName, Encoding);
			bool rpnUsed = false;
			for (int i = 0; i < Lines.Length; i++) {
				int j = Lines[i].IndexOf(';');
				// cut comments out
				Lines[i] = j >= 0 ? Lines[i].Substring(0, j).Trim() : Lines[i].Trim();
				rpnUsed = Lines[i].IndexOf("functionrpn", StringComparison.OrdinalIgnoreCase) >= 0;
			}
			if (rpnUsed) {
				Debug.AddMessage(Debug.MessageType.Error, false, "An animated object file contains RPN functions. These were never meant to be used directly, only for debugging. They won't be supported indefinately. Please get rid of them in file " + FileName);
			}
			for (int i = 0; i < Lines.Length; i++) {
				if (Lines[i].Length != 0) {
					switch (Lines[i].ToLowerInvariant()) {
						case "[include]":
							{
								i++;
								Vector3D position = new Vector3D(0.0, 0.0, 0.0);
								ObjectManager.UnifiedObject[] obj = new ObjectManager.UnifiedObject[4];
								int objCount = 0;
								while (i < Lines.Length && !(Lines[i].StartsWith("[", StringComparison.Ordinal) && Lines[i].EndsWith("]", StringComparison.Ordinal))) {
									if (Lines[i].Length != 0) {
										int equals = Lines[i].IndexOf("=", StringComparison.Ordinal);
										if (equals > 0) {
											/*
											 * Process key-value pair, the only supported key is position.
											 */
											string before = Lines[i].Substring(0, equals).TrimEnd();
											string after = Lines[i].Substring(equals + 1).TrimStart();
											switch (before.ToLowerInvariant()) {
												case "position":
													ParsePosition(after, ref position, before, i + 1, FileName);
													break;
												default:
													Debug.AddMessage(Debug.MessageType.Error, false, "The attribute " + before + " is not supported at line " + (i + 1).ToString(Culture) + " in file " + FileName);
													break;
											}
										} else {
											/*
											 * Process object with file name relative to the location of this ANIMATED file.
											 */
											string Folder = System.IO.Path.GetDirectoryName(FileName);
											if (Path.ContainsInvalidPathChars(Lines[i])) {
												Debug.AddMessage(Debug.MessageType.Error, false, Lines[i] + " contains illegal characters at line " + (i + 1).ToString(Culture) + " in file " + FileName);
											} else {
												string file = OpenBveApi.Path.CombineFile(Folder, Lines[i]);
												if (System.IO.File.Exists(file)) {
													if (obj.Length == objCount) {
														Array.Resize<ObjectManager.UnifiedObject>(ref obj, obj.Length << 1);
													}
													obj[objCount] = ObjectManager.LoadObject(file, Encoding, LoadMode, false, false, false);
													objCount++;
												} else {
													Debug.AddMessage(Debug.MessageType.Error, true, "File " + file + " not found at line " + (i + 1).ToString(Culture) + " in file " + FileName);
												}
											}
										}
									}
									i++;
								}
								i--;
								for (int j = 0; j < objCount; j++) {
									if (obj[j] != null) {
										if (obj[j] is ObjectManager.StaticObject) {
											ObjectManager.StaticObject s = (ObjectManager.StaticObject)obj[j];
											s.Dynamic = true;
											if (ObjectCount >= Result.Objects.Length) {
												Array.Resize<ObjectManager.AnimatedObject>(ref Result.Objects, Result.Objects.Length << 1);
											}
											ObjectManager.AnimatedObject a = new ObjectManager.AnimatedObject();
											ObjectManager.AnimatedObjectState aos = new ObjectManager.AnimatedObjectState();
											aos.Object = s;
											aos.Position = position;
											a.States = new[] { aos };
											Result.Objects[ObjectCount] = a;
											ObjectCount++;
										} else if (obj[j] is ObjectManager.AnimatedObjectCollection) {
											ObjectManager.AnimatedObjectCollection a = (ObjectManager.AnimatedObjectCollection)obj[j];
											for (int k = 0; k < a.Objects.Length; k++) {
												if (ObjectCount >= Result.Objects.Length) {
													Array.Resize<ObjectManager.AnimatedObject>(ref Result.Objects, Result.Objects.Length << 1);
												}
												for (int h = 0; h < a.Objects[k].States.Length; h++) {
													a.Objects[k].States[h].Position.X += position.X;
													a.Objects[k].States[h].Position.Y += position.Y;
													a.Objects[k].States[h].Position.Z += position.Z;
												}
												Result.Objects[ObjectCount] = a.Objects[k];
												ObjectCount++;
											}
										}
									}
								}
							}
							break;
						case "[object]":
							{
								i++;
								if (Result.Objects.Length == ObjectCount) {
									Array.Resize<ObjectManager.AnimatedObject>(ref Result.Objects, Result.Objects.Length << 1);
								}
								Result.Objects[ObjectCount] = new ObjectManager.AnimatedObject();
								Result.Objects[ObjectCount].States = new ObjectManager.AnimatedObjectState[] { };
								Result.Objects[ObjectCount].CurrentState = -1;
								Result.Objects[ObjectCount].TranslateXDirection = new Vector3D(1.0, 0.0, 0.0);
								Result.Objects[ObjectCount].TranslateYDirection = new Vector3D(0.0, 1.0, 0.0);
								Result.Objects[ObjectCount].TranslateZDirection = new Vector3D(0.0, 0.0, 1.0);
								Result.Objects[ObjectCount].RotateXDirection = new Vector3D(1.0, 0.0, 0.0);
								Result.Objects[ObjectCount].RotateYDirection = new Vector3D(0.0, 1.0, 0.0);
								Result.Objects[ObjectCount].RotateZDirection = new Vector3D(0.0, 0.0, 1.0);
								Result.Objects[ObjectCount].TextureShiftXDirection = new Vector2D(1.0, 0.0);
								Result.Objects[ObjectCount].TextureShiftYDirection = new Vector2D(0.0, 1.0);
								Result.Objects[ObjectCount].RefreshRate = 0.0;
								Result.Objects[ObjectCount].ObjectIndex = -1;
								Vector3D Position = new Vector3D(0.0, 0.0, 0.0);
								bool timetableUsed = false;
								string[] StateFiles = null;
								string StateFunctionRpn = null;
								int StateFunctionLine = -1;
								while (i < Lines.Length && !(Lines[i].StartsWith("[", StringComparison.Ordinal) && Lines[i].EndsWith("]", StringComparison.Ordinal))) {
									if (Lines[i].Length != 0) {
										int equals = Lines[i].IndexOf("=", StringComparison.Ordinal);
										if (equals > 0) {
											string before = Lines[i].Substring(0, equals).TrimEnd();
											string after = Lines[i].Substring(equals + 1).TrimStart();
											switch (before.ToLowerInvariant()) {
												case "position":
													ParsePosition(after, ref Position, before, i + 1, FileName);
													break;
												case "states":
													if (!ParseState(after, ref StateFiles, before, i + 1, FileName))
														return null;
													break;
												case "statefunction":
													try {
														StateFunctionLine = i;
														StateFunctionRpn = FunctionScripts.GetPostfixNotationFromInfixNotation(after);
													} catch (Exception ex) {
														Debug.AddMessage(Debug.MessageType.Error, false, ex.Message + " in " + before + " at line " + (i + 1).ToString(Culture) + " in file " + FileName);
													} break;
												case "statefunctionrpn":
													{
														StateFunctionLine = i;
														StateFunctionRpn = after;
													} break;
												case "translatexdirection":
													ParseTranslateDirection(after, ref Result.Objects[ObjectCount].TranslateXDirection,
														before, i + 1, FileName);
													break;
												case "translateydirection":
													ParseTranslateDirection(after, ref Result.Objects[ObjectCount].TranslateYDirection,
														before, i + 1, FileName);
													break;
												case "translatezdirection":
													ParseTranslateDirection(after, ref Result.Objects[ObjectCount].TranslateZDirection,
														before, i + 1, FileName);
													break;
												case "translatexfunction":
													ParseInfixFunc(after, ref Result.Objects[ObjectCount].TranslateXFunction, before, i + 1, FileName);
													break;
												case "translateyfunction":
													ParseInfixFunc(after, ref Result.Objects[ObjectCount].TranslateYFunction, before, i + 1, FileName);
													break;
												case "translatezfunction":
													ParseInfixFunc(after, ref Result.Objects[ObjectCount].TranslateZFunction, before, i + 1, FileName);
													break;
												case "translatexfunctionrpn":
													ParsePostfixFunc(after, ref Result.Objects[ObjectCount].TranslateXFunction, before, i + 1, FileName);
													break;
												case "translateyfunctionrpn":
													ParsePostfixFunc(after, ref Result.Objects[ObjectCount].TranslateYFunction, before, i + 1, FileName);
													break;
												case "translatezfunctionrpn":
													ParsePostfixFunc(after, ref Result.Objects[ObjectCount].TranslateZFunction, before, i + 1, FileName);
													break;
												case "rotatexdirection":
													ParseRotateDirection(after, ref Result.Objects[ObjectCount].RotateXDirection, before, i + 1, FileName);
													break;
												case "rotateydirection":
													ParseRotateDirection(after, ref Result.Objects[ObjectCount].RotateYDirection, before, i + 1, FileName);
													break;
												case "rotatezdirection":
													ParseRotateDirection(after, ref Result.Objects[ObjectCount].RotateZDirection, before, i + 1, FileName);
													break;
												case "rotatexfunction":
													ParseInfixFunc(after,ref Result.Objects[ObjectCount].RotateXFunction, before,i+1,FileName);
													break;
												case "rotateyfunction":
													ParseInfixFunc(after,ref Result.Objects[ObjectCount].RotateYFunction, before,i+1,FileName);
													break;
												case "rotatezfunction":
													ParseInfixFunc(after,ref Result.Objects[ObjectCount].RotateZFunction, before,i+1,FileName);
													break;
												case "rotatexfunctionrpn":
													ParsePostfixFunc(after,ref Result.Objects[ObjectCount].RotateXFunction, before,i+1,FileName);
													break;
												case "rotateyfunctionrpn":
													ParsePostfixFunc(after,ref Result.Objects[ObjectCount].RotateYFunction, before,i+1,FileName);
													break;
												case "rotatezfunctionrpn":
													ParsePostfixFunc(after,ref Result.Objects[ObjectCount].RotateZFunction, before,i+1,FileName);
													break;
												case "rotatexdamping":
													ParseRotateDamping(after, ref Result.Objects[ObjectCount].RotateXDamping,before, i + 1, FileName);
													break;
												case "rotateydamping":
													ParseRotateDamping(after, ref Result.Objects[ObjectCount].RotateYDamping,before, i + 1, FileName);
													break;
												case "rotatezdamping":
													ParseRotateDamping(after, ref Result.Objects[ObjectCount].RotateZDamping,before, i + 1, FileName);
													break;
												case "textureshiftxdirection":
													ParseTextureShift(after, ref Result.Objects[ObjectCount].TextureShiftXDirection, before, i + 1, FileName);
													break;
												case "textureshiftydirection":
													ParseTextureShift(after, ref Result.Objects[ObjectCount].TextureShiftYDirection, before, i + 1, FileName);
													break;
												case "textureshiftxfunction":
													ParseInfixFunc(after, ref Result.Objects[ObjectCount].TextureShiftXFunction, before, i + 1, FileName);
													break;
												case "textureshiftyfunction":
													ParseInfixFunc(after, ref Result.Objects[ObjectCount].TextureShiftYFunction, before, i + 1, FileName);
													break;
												case "textureshiftxfunctionrpn":
													ParsePostfixFunc(after, ref Result.Objects[ObjectCount].TextureShiftXFunction, before, i + 1, FileName);
													break;
												case "textureshiftyfunctionrpn":
													ParsePostfixFunc(after, ref Result.Objects[ObjectCount].TextureShiftYFunction, before, i + 1, FileName);
													break;
												case "textureoverride":
													switch (after.ToLowerInvariant()) {
														case "none":
															break;
														case "timetable":
															if (!timetableUsed) {
																Timetable.AddObjectForCustomTimetable(Result.Objects[ObjectCount]);
																timetableUsed = true;
															}
															break;
														default:
															Debug.AddMessage(Debug.MessageType.Error, false, "Unrecognized value in " + before + " at line " + (i + 1).ToString(Culture) + " in file " + FileName);
															break;
													}
													break;
												case "refreshrate":
													{
														double r;
														if (!double.TryParse(after, System.Globalization.NumberStyles.Float, Culture, out r)) {
															Debug.AddMessage(Debug.MessageType.Error, false, "Value is invalid in " + before + " at line " + (i + 1).ToString(Culture) + " in file " + FileName);
														} else if (r < 0.0) {
															Debug.AddMessage(Debug.MessageType.Error, false, "Value is expected to be non-negative in " + before + " at line " + (i + 1).ToString(Culture) + " in file " + FileName);
														} else {
															Result.Objects[ObjectCount].RefreshRate = r;
														}
													} break;
												default:
													Debug.AddMessage(Debug.MessageType.Error, false, "The attribute " + before + " is not supported at line " + (i + 1).ToString(Culture) + " in file " + FileName);
													break;
											}
										} else {
											Debug.AddMessage(Debug.MessageType.Error, false, "Invalid statement " + Lines[i] + " encountered at line " + (i + 1).ToString(Culture) + " in file " + FileName);
											return null;
										}
									}
									i++;
								}
								i--;
								if (StateFiles != null) {
									// create the object
									if (timetableUsed) {
										if (StateFunctionRpn != null) {
											StateFunctionRpn = "timetable 0 == " + StateFunctionRpn + " -1 ?";
										} else {
											StateFunctionRpn = "timetable";
										}
									}
									if (StateFunctionRpn != null) {
										try {
											Result.Objects[ObjectCount].StateFunction = FunctionScripts.GetFunctionScriptFromPostfixNotation(StateFunctionRpn);
										} catch (Exception ex) {
											Debug.AddMessage(Debug.MessageType.Error, false, ex.Message + " in StateFunction at line " + (StateFunctionLine + 1).ToString(Culture) + " in file " + FileName);
										}
									}
									Result.Objects[ObjectCount].States = new ObjectManager.AnimatedObjectState[StateFiles.Length];
									bool ForceTextureRepeatX = Result.Objects[ObjectCount].TextureShiftXFunction != null && Result.Objects[ObjectCount].TextureShiftXDirection.X != 0.0 ||
										Result.Objects[ObjectCount].TextureShiftYFunction != null && Result.Objects[ObjectCount].TextureShiftYDirection.Y != 0.0;
									bool ForceTextureRepeatY = Result.Objects[ObjectCount].TextureShiftXFunction != null && Result.Objects[ObjectCount].TextureShiftXDirection.X != 0.0 ||
										Result.Objects[ObjectCount].TextureShiftYFunction != null && Result.Objects[ObjectCount].TextureShiftYDirection.Y != 0.0;
									for (int k = 0; k < StateFiles.Length; k++) {
										Result.Objects[ObjectCount].States[k].Position = new Vector3D(0.0, 0.0, 0.0);
										if (StateFiles[k] != null) {
											Result.Objects[ObjectCount].States[k].Object = ObjectManager.LoadStaticObject(StateFiles[k], Encoding, LoadMode, false, ForceTextureRepeatX, ForceTextureRepeatY);
											if (Result.Objects[ObjectCount].States[k].Object != null) {
												Result.Objects[ObjectCount].States[k].Object.Dynamic = true;
											}
										} else {
											Result.Objects[ObjectCount].States[k].Object = null;
										}
										for (int j = 0; j < Result.Objects[ObjectCount].States.Length; j++) {
											Result.Objects[ObjectCount].States[j].Position = Position;
										}
									}
								} else {
									Result.Objects[ObjectCount].States = new ObjectManager.AnimatedObjectState[] { };
								}
								ObjectCount++;
							}
							break;
						default:
							Debug.AddMessage(Debug.MessageType.Error, false, "Invalid statement " + Lines[i] + " encountered at line " + (i + 1).ToString(Culture) + " in file " + FileName);
							return null;
					}
				}
			}
			Array.Resize<ObjectManager.AnimatedObject>(ref Result.Objects, ObjectCount);
			return Result;
		}

		/// <summary>
		/// Parse the Vector3 position string consisting of comma separated double values. Error messages are written when necessary.
		/// </summary>
		/// <param name="value">x,y,z.</param>
		/// <param name="position">Parsed position.</param>
		/// <param name="field">Actual field name.</param>
		/// <param name="file">File name.</param>
		/// <param name="line">Actual line number string.</param>
		private static void ParsePosition(string value, ref Vector3D position, string field, int line, string file){
			string[] s = value.Split(',');
			if (s.Length == 3) {
				double x, y, z;
				if (!double.TryParse(s[0], System.Globalization.NumberStyles.Float, Culture, out x)) {
					Debug.AddMessage(Debug.MessageType.Error, false, "X is invalid in " + field + " at line " + line + " in file " + file);
				} else if (!double.TryParse(s[1], System.Globalization.NumberStyles.Float, Culture, out y)) {
					Debug.AddMessage(Debug.MessageType.Error, false, "Y is invalid in " + field + " at line " + line + " in file " + file);
				} else if (!double.TryParse(s[2], System.Globalization.NumberStyles.Float, Culture, out z)) {
					Debug.AddMessage(Debug.MessageType.Error, false, "Z is invalid in " + field + " at line " + line + " in file " + file);
				} else {
					position = new Vector3D(x, y, z);
				}
			} else {
				Debug.AddMessage(Debug.MessageType.Error, false, "Exactly 3 arguments are expected in " + field + " at line " + line.ToString(Culture) + " in file " + file);
			}
		}
		/// <summary>
		/// Parse the value to the string array. Error messages are written when necessary.
		/// </summary>
		/// <returns><c>true</c>, if state was successfully parsed, <c>false</c> otherwise.</returns>
		/// <param name="value">Comma-separated string.</param>
		/// <param name="stateFiles">Output string array.</param>
		/// <param name="field">Actual field name.</param>
		/// <param name="filename">File name.</param>
		/// <param name="line">Actual line number string.</param>
		private static bool ParseState(string value, ref string[] stateFiles, string field, int line, string filename){
			string[] s = value.Split(',');
			if (s.Length >= 1) {
				string Folder = System.IO.Path.GetDirectoryName(filename);
				stateFiles = new string[s.Length];
				for (int k = 0; k < s.Length; k++) {
					s[k] = s[k].Trim();
					if (s[k].Length == 0) {
						Debug.AddMessage(Debug.MessageType.Error, false, "File" + k.ToString(Culture) + " is an empty string - did you mean something else? - in " + field + " at line " + line.ToString(Culture) + " in file " + filename);
						stateFiles[k] = null;
					} else if (OpenBveApi.Path.ContainsInvalidPathChars(s[k])) {
						Debug.AddMessage(Debug.MessageType.Error, false, "File" + k.ToString(Culture) + " contains illegal characters in " + field + " at line " + line.ToString(Culture) + " in file " + filename);
						stateFiles[k] = null;
					} else {
						stateFiles[k] = OpenBveApi.Path.CombineFile(Folder, s[k]);
						if (!System.IO.File.Exists(stateFiles[k])) {
							Debug.AddMessage(Debug.MessageType.Error, true, "File " + stateFiles[k] + " not found in " + field + " at line " + line.ToString(Culture) + " in file " + filename);
							stateFiles[k] = null;
						}
					}
				}
			} else {
				Debug.AddMessage(Debug.MessageType.Error, false, "At least one argument is expected in " + field + " at line " + line.ToString(Culture) + " in file " + filename);
				return false;
			}
			return true;
		}
		/// <summary>
		/// Parses the string representing translation direction to a Vector3. Error messages are written when necessary.
		/// </summary>
		/// <param name="value">String representation.</param>
		/// <param name="transDir">Output Vector3.</param>
		/// <param name="field">Actual field name.</param>
		/// <param name="file">File name.</param>
		/// <param name="line">Actual line number string.</param>
		private static void ParseTranslateDirection(string value, ref Vector3D transDir, string field, int line, string file){
			string[] s = value.Split(',');
			if (s.Length == 3) {
				double x, y, z;
				if (!double.TryParse(s[0], System.Globalization.NumberStyles.Float, Culture, out x)) {
					Debug.AddMessage(Debug.MessageType.Error, false, "X is invalid in " + field + " at line " + line.ToString(Culture) + " in file " + file);
				} else if (!double.TryParse(s[1], System.Globalization.NumberStyles.Float, Culture, out y)) {
					Debug.AddMessage(Debug.MessageType.Error, false, "Y is invalid in " + field + " at line " + line.ToString(Culture) + " in file " + file);
				} else if (!double.TryParse(s[2], System.Globalization.NumberStyles.Float, Culture, out z)) {
					Debug.AddMessage(Debug.MessageType.Error, false, "Z is invalid in " + field + " at line " + line.ToString(Culture) + " in file " + file);
				} else {
					transDir = new Vector3D(x, y, z);
				}
			} else {
				Debug.AddMessage(Debug.MessageType.Error, false, "Exactly 3 arguments are expected in " + field + " at line " + line.ToString(Culture) + " in file " + file);
			}
		}

		/// <summary>
		/// Parses the string representation of function in postfix notation to a FunctionScript instance. Error messages are written when necessary.
		/// </summary>
		/// <param name="value">String representation of postfix function.</param>
		/// <param name="func">FunctionScript instance generated from string.</param>
		/// <param name="field">Actual field name.</param>
		/// <param name="filename">File name.</param>
		/// <param name="line">Actual line number string.</param>
		private static void ParsePostfixFunc(string value, ref FunctionScripts.FunctionScript func, string field, int line, string filename){
			try {
				func = FunctionScripts.GetFunctionScriptFromPostfixNotation(value);
			} catch (Exception ex) {
				Debug.AddMessage(Debug.MessageType.Error, false, ex.Message + " in " + field + " at line " + line.ToString(Culture) + " in file " + filename);
			}
		}

		/// <summary>
		/// Parses the string representation of function in infix notation to a FunctionScript instance. Error messages are written when necessary.
		/// </summary>
		/// <param name="value">String representation of infix function.</param>
		/// <param name="func">FunctionScript instance generated from string.</param>
		/// <param name="field">Actual field name.</param>
		/// <param name="filename">File name.</param>
		/// <param name="line">Actual line number string.</param>
		private static void ParseInfixFunc(string value, ref FunctionScripts.FunctionScript func, string field, int line, string filename){
			try {
				func = FunctionScripts.GetFunctionScriptFromInfixNotation(value);
			} catch (Exception ex) {
				Debug.AddMessage(Debug.MessageType.Error, false, ex.Message + " in " + field + " at line " + line.ToString(Culture) + " in file " + filename);
			}
		}

		/// <summary>
		/// Parses the string representing rotation direction to a Vector3. Error messages are written when necessary.
		/// </summary>
		/// <param name="value">String representation.</param>
		/// <param name="rotDir">Output Vector3.</param>
		/// <param name="field">Actual field name.</param>
		/// <param name="filename">File name.</param>
		/// <param name="line">Actual line number string.</param>
		private static void ParseRotateDirection(string value, ref Vector3D rotDir, string field, int line, string filename){
			string[] s = value.Split(',');
			if (s.Length == 3) {
				double x, y, z;
				if (!double.TryParse(s[0], System.Globalization.NumberStyles.Float, Culture, out x)) {
					Debug.AddMessage(Debug.MessageType.Error, false, "X is invalid in " + field + " at line " + line.ToString(Culture) + " in file " + filename);
				} else if (!double.TryParse(s[1], System.Globalization.NumberStyles.Float, Culture, out y)) {
					Debug.AddMessage(Debug.MessageType.Error, false, "Y is invalid in " + field + " at line " + line.ToString(Culture) + " in file " + filename);
				} else if (!double.TryParse(s[2], System.Globalization.NumberStyles.Float, Culture, out z)) {
					Debug.AddMessage(Debug.MessageType.Error, false, "Z is invalid in " + field + " at line " + line.ToString(Culture) + " in file " + filename);
				} else if (x == 0.0 && y == 0.0 && z == 0.0) {
					Debug.AddMessage(Debug.MessageType.Error, false, "The direction indicated by X, Y and Z is expected to be non-zero in " + field + " at line " + line.ToString(Culture) + " in file " + filename);
				} else {
					rotDir = new Vector3D(x, y, z);
				}
			} else {
				Debug.AddMessage(Debug.MessageType.Error, false, "Exactly 3 arguments are expected in " + field + " at line " + line.ToString(Culture) + " in file " + filename);
			}
		}

		/// <summary>
		/// Parses the string representing rotation damping to a ObjectManager.Damping. Error messages are written when necessary.
		/// </summary>
		/// <param name="value">String representation.</param>
		/// <param name="damp">Output damping.</param>
		/// <param name="field">Actual field name.</param>
		/// <param name="filename">File name.</param>
		/// <param name="line">Actual line number string.</param>
		private static void ParseRotateDamping(string value, ref ObjectManager.Damping damp, string field, int line, string filename){
			string[] s = value.Split(',');
			if (s.Length == 2) {
				double nf, dr;
				if (!double.TryParse(s[0], System.Globalization.NumberStyles.Float, Culture, out nf)) {
					Debug.AddMessage(Debug.MessageType.Error, false, "NaturalFrequency is invalid in " + field + " at line " + line.ToString(Culture) + " in file " + filename);
				} else if (!double.TryParse(s[1], System.Globalization.NumberStyles.Float, Culture, out dr)) {
					Debug.AddMessage(Debug.MessageType.Error, false, "DampingRatio is invalid in " + field + " at line " + line.ToString(Culture) + " in file " + filename);
				} else if (nf <= 0.0) {
					Debug.AddMessage(Debug.MessageType.Error, false, "NaturalFrequency is expected to be positive in " + field + " at line " + line.ToString(Culture) + " in file " + filename);
				} else if (dr <= 0.0) {
					Debug.AddMessage(Debug.MessageType.Error, false, "DampingRatio is expected to be positive in " + field + " at line " + line.ToString(Culture) + " in file " + filename);
				} else {
					damp = new ObjectManager.Damping(nf, dr);
				}
			} else {
				Debug.AddMessage(Debug.MessageType.Error, false, "Exactly 2 arguments are expected in " + field + " at line " + line.ToString(Culture) + " in file " + filename);
			}
		}

		/// <summary>
		/// Parses the string representing texture shift to a Vector2. Error messages are written when necessary.
		/// </summary>
		/// <param name="value">String representation.</param>
		/// <param name="shift">Output Vector2.</param>
		/// <param name="field">Actual field name.</param>
		/// <param name="filename">File name.</param>
		/// <param name="line">Actual line number string.</param>
		private static void ParseTextureShift(string value, ref Vector2D shift, string field, int line, string filename){
			string[] s = value.Split(',');
			if (s.Length == 2) {
				double x, y;
				if (!double.TryParse(s[0], System.Globalization.NumberStyles.Float, Culture, out x)) {
					Debug.AddMessage(Debug.MessageType.Error, false, "X is invalid in " + field + " at line " + line.ToString(Culture) + " in file " + filename);
				} else if (!double.TryParse(s[1], System.Globalization.NumberStyles.Float, Culture, out y)) {
					Debug.AddMessage(Debug.MessageType.Error, false, "Y is invalid in " + field + " at line " + line.ToString(Culture) + " in file " + filename);
				} else {
					shift = new Vector2D(x, y);
				}
			} else {
				Debug.AddMessage(Debug.MessageType.Error, false, "Exactly 2 arguments are expected in " + field + " at line " + line.ToString(Culture) + " in file " + filename);
			}
		}
	}
}