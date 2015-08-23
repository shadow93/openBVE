using System;
using System.Globalization;
using OpenBveApi.Colors;

namespace OpenBve
{
	internal static class Hud
    {
		internal struct HudVector {
			internal int X;
			internal int Y;
		}
		internal struct HudVectorF {
			internal float X;
			internal float Y;
		}
		internal struct HudImage {
			internal Textures.Texture BackgroundTexture;
			internal Textures.Texture OverlayTexture;
		}
		[Flags]
		internal enum HudTransition {
			None = 0,
			Move = 1,
			Fade = 2,
			MoveAndFade = 3, // Move | Fade
		}
		internal class HudElement {
			internal string Subject;
			internal HudVectorF Position;
			internal HudVector Alignment;
			internal HudImage TopLeft;
			internal HudImage TopMiddle;
			internal HudImage TopRight;
			internal HudImage CenterLeft;
			internal HudImage CenterMiddle;
			internal HudImage CenterRight;
			internal HudImage BottomLeft;
			internal HudImage BottomMiddle;
			internal HudImage BottomRight;
			internal Color32 BackgroundColor;
			internal Color32 OverlayColor;
			internal Color32 TextColor;
			internal HudVectorF TextPosition;
			internal HudVector TextAlignment;
			internal Fonts.OpenGlFont Font;
			internal bool TextShadow;
			internal string Text;
			internal float Value1;
			internal float Value2;
			internal HudTransition Transition;
			internal HudVectorF TransitionVector;
			internal double TransitionState;
			internal HudElement() {
				this.Subject = null;
				this.Position.X = 0.0f;
				this.Position.Y = 0.0f;
				this.Alignment.X = -1;
				this.Alignment.Y = -1;
				this.BackgroundColor = new Color32(255, 255, 255, 255);
				this.OverlayColor = new Color32(255, 255, 255, 255);
				this.TextColor = new Color32(255, 255, 255, 255);
				this.TextPosition.X = 0.0f;
				this.TextPosition.Y = 0.0f;
				this.TextAlignment.X = -1;
				this.TextAlignment.Y = 0;
				this.Font = Fonts.VerySmallFont;
				this.TextShadow = true;
				this.Text = null;
				this.Value1 = 0.0f;
				this.Value2 = 0.0f;
				this.Transition = HudTransition.None;
				this.TransitionState = 1.0;
			}
		}
		internal static HudElement[] CurrentHudElements = new HudElement[] { };

		// load hud
		internal static void LoadHUD() {
			CultureInfo Culture = CultureInfo.InvariantCulture;
			string Folder = Program.FileSystem.GetDataFolder("In-game", Options.Current.UserInterfaceFolder);
			string File = OpenBveApi.Path.CombineFile(Folder, "interface.cfg");
			CurrentHudElements = new HudElement[16];
			int Length = 0;
			if (System.IO.File.Exists(File)) {
				string[] Lines = System.IO.File.ReadAllLines(File, new System.Text.UTF8Encoding());
				for (int i = 0; i < Lines.Length; i++) {
					int j = Lines[i].IndexOf(';');
					if (j >= 0) {
						Lines[i] = Lines[i].Substring(0, j).Trim();
					} else {
						Lines[i] = Lines[i].Trim();
					}
					if (Lines[i].Length != 0) {
						if (!Lines[i].StartsWith(";", StringComparison.Ordinal)) {
							if (Lines[i].Equals("[element]", StringComparison.OrdinalIgnoreCase)) {
								Length++;
								if (Length > CurrentHudElements.Length) {
									Array.Resize<HudElement>(ref CurrentHudElements, CurrentHudElements.Length << 1);
								}
								CurrentHudElements[Length - 1] = new HudElement();
							} else if (Length == 0) {
								System.Windows.Forms.MessageBox.Show("Line outside of [element] structure encountered at line " + (i + 1).ToString(Culture) + " in " + File);
							} else {
								j = Lines[i].IndexOf("=", StringComparison.Ordinal);
								if (j >= 0) {
									string Command = Lines[i].Substring(0, j).TrimEnd();
									string[] Arguments = Lines[i].Substring(j + 1).TrimStart().Split(new char[] { ',' }, StringSplitOptions.None);
									for (j = 0; j < Arguments.Length; j++) {
										Arguments[j] = Arguments[j].Trim();
									}
									switch (Command.ToLowerInvariant()) {
										case "subject":
											if (Arguments.Length == 1) {
												CurrentHudElements[Length - 1].Subject = Arguments[0];
											} else {
												System.Windows.Forms.MessageBox.Show("Incorrect number of arguments supplied in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
											} break;
										case "position":
											if (Arguments.Length == 2) {
												float x, y;
												if (!float.TryParse(Arguments[0], NumberStyles.Float, Culture, out x)) {
													System.Windows.Forms.MessageBox.Show("X is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else if (!float.TryParse(Arguments[1], NumberStyles.Float, Culture, out y)) {
													System.Windows.Forms.MessageBox.Show("Y is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else {
													CurrentHudElements[Length - 1].Position.X = x;
													CurrentHudElements[Length - 1].Position.Y = y;
												}
											} else {
												System.Windows.Forms.MessageBox.Show("Incorrect number of arguments supplied in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
											} break;
										case "alignment":
											if (Arguments.Length == 2) {
												int x, y;
												if (!int.TryParse(Arguments[0], NumberStyles.Integer, Culture, out x)) {
													System.Windows.Forms.MessageBox.Show("X is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else if (!int.TryParse(Arguments[1], NumberStyles.Integer, Culture, out y)) {
													System.Windows.Forms.MessageBox.Show("Y is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else {
													CurrentHudElements[Length - 1].Alignment.X = Math.Sign(x);
													CurrentHudElements[Length - 1].Alignment.Y = Math.Sign(y);
												}
											} else {
												System.Windows.Forms.MessageBox.Show("Incorrect number of arguments supplied in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
											} break;
										case "topleft":
											if (Arguments.Length == 2) {
												if (Arguments[0].Length != 0 & !Arguments[0].Equals("null", StringComparison.OrdinalIgnoreCase)) {
													Textures.RegisterTexture(OpenBveApi.Path.CombineFile(Folder, Arguments[0]), out CurrentHudElements[Length - 1].TopLeft.BackgroundTexture);
												}
												if (Arguments[1].Length != 0 & !Arguments[1].Equals("null", StringComparison.OrdinalIgnoreCase)) {
													Textures.RegisterTexture(OpenBveApi.Path.CombineFile(Folder, Arguments[1]), out CurrentHudElements[Length - 1].TopLeft.OverlayTexture);
												}
											} else {
												System.Windows.Forms.MessageBox.Show("Incorrect number of arguments supplied in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
											} break;
										case "topmiddle":
											if (Arguments.Length == 2) {
												if (Arguments[0].Length != 0 & !Arguments[0].Equals("null", StringComparison.OrdinalIgnoreCase)) {
													Textures.RegisterTexture(OpenBveApi.Path.CombineFile(Folder, Arguments[0]), out CurrentHudElements[Length - 1].TopMiddle.BackgroundTexture);
												}
												if (Arguments[1].Length != 0 & !Arguments[1].Equals("null", StringComparison.OrdinalIgnoreCase)) {
													Textures.RegisterTexture(OpenBveApi.Path.CombineFile(Folder, Arguments[1]), out CurrentHudElements[Length - 1].TopMiddle.OverlayTexture);
												}
											} else {
												System.Windows.Forms.MessageBox.Show("Incorrect number of arguments supplied in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
											} break;
										case "topright":
											if (Arguments.Length == 2) {
												if (Arguments[0].Length != 0 & !Arguments[0].Equals("null", StringComparison.OrdinalIgnoreCase)) {
													Textures.RegisterTexture(OpenBveApi.Path.CombineFile(Folder, Arguments[0]), out CurrentHudElements[Length - 1].TopRight.BackgroundTexture);
												}
												if (Arguments[1].Length != 0 & !Arguments[1].Equals("null", StringComparison.OrdinalIgnoreCase)) {
													Textures.RegisterTexture(OpenBveApi.Path.CombineFile(Folder, Arguments[1]), out CurrentHudElements[Length - 1].TopRight.OverlayTexture);
												}
											} else {
												System.Windows.Forms.MessageBox.Show("Incorrect number of arguments supplied in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
											} break;
										case "centerleft":
											if (Arguments.Length == 2) {
												if (Arguments[0].Length != 0 & !Arguments[0].Equals("null", StringComparison.OrdinalIgnoreCase)) {
													Textures.RegisterTexture(OpenBveApi.Path.CombineFile(Folder, Arguments[0]), out CurrentHudElements[Length - 1].CenterLeft.BackgroundTexture);
												}
												if (Arguments[1].Length != 0 & !Arguments[1].Equals("null", StringComparison.OrdinalIgnoreCase)) {
													Textures.RegisterTexture(OpenBveApi.Path.CombineFile(Folder, Arguments[1]), out CurrentHudElements[Length - 1].CenterLeft.OverlayTexture);
												}
											} else {
												System.Windows.Forms.MessageBox.Show("Incorrect number of arguments supplied in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
											} break;
										case "centermiddle":
											if (Arguments.Length == 2) {
												if (Arguments[0].Length != 0 & !Arguments[0].Equals("null", StringComparison.OrdinalIgnoreCase)) {
													Textures.RegisterTexture(OpenBveApi.Path.CombineFile(Folder, Arguments[0]), out CurrentHudElements[Length - 1].CenterMiddle.BackgroundTexture);
												}
												if (Arguments[1].Length != 0 & !Arguments[1].Equals("null", StringComparison.OrdinalIgnoreCase)) {
													Textures.RegisterTexture(OpenBveApi.Path.CombineFile(Folder, Arguments[1]), out CurrentHudElements[Length - 1].CenterMiddle.OverlayTexture);
												}
											} else {
												System.Windows.Forms.MessageBox.Show("Incorrect number of arguments supplied in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
											} break;
										case "centerright":
											if (Arguments.Length == 2) {
												if (Arguments[0].Length != 0 & !Arguments[0].Equals("null", StringComparison.OrdinalIgnoreCase)) {
													Textures.RegisterTexture(OpenBveApi.Path.CombineFile(Folder, Arguments[0]), out CurrentHudElements[Length - 1].CenterRight.BackgroundTexture);
												}
												if (Arguments[1].Length != 0 & !Arguments[1].Equals("null", StringComparison.OrdinalIgnoreCase)) {
													Textures.RegisterTexture(OpenBveApi.Path.CombineFile(Folder, Arguments[1]), out CurrentHudElements[Length - 1].CenterRight.OverlayTexture);
												}
											} else {
												System.Windows.Forms.MessageBox.Show("Incorrect number of arguments supplied in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
											} break;
										case "bottomleft":
											if (Arguments.Length == 2) {
												if (Arguments[0].Length != 0 & !Arguments[0].Equals("null", StringComparison.OrdinalIgnoreCase)) {
													Textures.RegisterTexture(OpenBveApi.Path.CombineFile(Folder, Arguments[0]), out CurrentHudElements[Length - 1].BottomLeft.BackgroundTexture);
												}
												if (Arguments[1].Length != 0 & !Arguments[1].Equals("null", StringComparison.OrdinalIgnoreCase)) {
													Textures.RegisterTexture(OpenBveApi.Path.CombineFile(Folder, Arguments[1]), out CurrentHudElements[Length - 1].BottomLeft.OverlayTexture);
												}
											} else {
												System.Windows.Forms.MessageBox.Show("Incorrect number of arguments supplied in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
											} break;
										case "bottommiddle":
											if (Arguments.Length == 2) {
												if (Arguments[0].Length != 0 & !Arguments[0].Equals("null", StringComparison.OrdinalIgnoreCase)) {
													Textures.RegisterTexture(OpenBveApi.Path.CombineFile(Folder, Arguments[0]), out CurrentHudElements[Length - 1].BottomMiddle.BackgroundTexture);
												}
												if (Arguments[1].Length != 0 & !Arguments[1].Equals("null", StringComparison.OrdinalIgnoreCase)) {
													Textures.RegisterTexture(OpenBveApi.Path.CombineFile(Folder, Arguments[1]), out CurrentHudElements[Length - 1].BottomMiddle.OverlayTexture);
												}
											} else {
												System.Windows.Forms.MessageBox.Show("Incorrect number of arguments supplied in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
											} break;
										case "bottomright":
											if (Arguments.Length == 2) {
												if (Arguments[0].Length != 0 & !Arguments[0].Equals("null", StringComparison.OrdinalIgnoreCase)) {
													Textures.RegisterTexture(OpenBveApi.Path.CombineFile(Folder, Arguments[0]), out CurrentHudElements[Length - 1].BottomRight.BackgroundTexture);
												}
												if (Arguments[1].Length != 0 & !Arguments[1].Equals("null", StringComparison.OrdinalIgnoreCase)) {
													Textures.RegisterTexture(OpenBveApi.Path.CombineFile(Folder, Arguments[1]), out CurrentHudElements[Length - 1].BottomRight.OverlayTexture);
												}
											} else {
												System.Windows.Forms.MessageBox.Show("Incorrect number of arguments supplied in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
											} break;
										case "backcolor":
											if (Arguments.Length == 4) {
												int r, g, b, a;
												if (!int.TryParse(Arguments[0], NumberStyles.Integer, Culture, out r)) {
													System.Windows.Forms.MessageBox.Show("R is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else if (!int.TryParse(Arguments[1], NumberStyles.Integer, Culture, out g)) {
													System.Windows.Forms.MessageBox.Show("G is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else if (!int.TryParse(Arguments[2], NumberStyles.Integer, Culture, out b)) {
													System.Windows.Forms.MessageBox.Show("B is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else if (!int.TryParse(Arguments[3], NumberStyles.Integer, Culture, out a)) {
													System.Windows.Forms.MessageBox.Show("A is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else {
													r = r < 0 ? 0 : r > 255 ? 255 : r;
													g = g < 0 ? 0 : g > 255 ? 255 : g;
													b = b < 0 ? 0 : b > 255 ? 255 : b;
													a = a < 0 ? 0 : a > 255 ? 255 : a;
													CurrentHudElements[Length - 1].BackgroundColor = new Color32((byte)r, (byte)g, (byte)b, (byte)a);
												}
												break;
											}
											System.Windows.Forms.MessageBox.Show("Incorrect number of arguments supplied in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
											break;
										case "overlaycolor":
											if (Arguments.Length == 4) {
												int r, g, b, a;
												if (!int.TryParse(Arguments[0], NumberStyles.Integer, Culture, out r)) {
													System.Windows.Forms.MessageBox.Show("R is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else if (!int.TryParse(Arguments[1], NumberStyles.Integer, Culture, out g)) {
													System.Windows.Forms.MessageBox.Show("G is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else if (!int.TryParse(Arguments[2], NumberStyles.Integer, Culture, out b)) {
													System.Windows.Forms.MessageBox.Show("B is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else if (!int.TryParse(Arguments[3], NumberStyles.Integer, Culture, out a)) {
													System.Windows.Forms.MessageBox.Show("A is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else {
													r = r < 0 ? 0 : r > 255 ? 255 : r;
													g = g < 0 ? 0 : g > 255 ? 255 : g;
													b = b < 0 ? 0 : b > 255 ? 255 : b;
													a = a < 0 ? 0 : a > 255 ? 255 : a;
													CurrentHudElements[Length - 1].OverlayColor = new Color32((byte)r, (byte)g, (byte)b, (byte)a);
												}
												break;
											}
											System.Windows.Forms.MessageBox.Show("Incorrect number of arguments supplied in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
											break;
										case "textcolor":
											if (Arguments.Length == 4) {
												int r, g, b, a;
												if (!int.TryParse(Arguments[0], NumberStyles.Integer, Culture, out r)) {
													System.Windows.Forms.MessageBox.Show("R is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else if (!int.TryParse(Arguments[1], NumberStyles.Integer, Culture, out g)) {
													System.Windows.Forms.MessageBox.Show("G is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else if (!int.TryParse(Arguments[2], NumberStyles.Integer, Culture, out b)) {
													System.Windows.Forms.MessageBox.Show("B is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else if (!int.TryParse(Arguments[3], NumberStyles.Integer, Culture, out a)) {
													System.Windows.Forms.MessageBox.Show("A is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else {
													r = r < 0 ? 0 : r > 255 ? 255 : r;
													g = g < 0 ? 0 : g > 255 ? 255 : g;
													b = b < 0 ? 0 : b > 255 ? 255 : b;
													a = a < 0 ? 0 : a > 255 ? 255 : a;
													CurrentHudElements[Length - 1].TextColor = new Color32((byte)r, (byte)g, (byte)b, (byte)a);
												}
												break;
											}
											System.Windows.Forms.MessageBox.Show("Incorrect number of arguments supplied in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
											break;
										case "textposition":
											if (Arguments.Length == 2) {
												float x, y;
												if (!float.TryParse(Arguments[0], NumberStyles.Float, Culture, out x)) {
													System.Windows.Forms.MessageBox.Show("X is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else if (!float.TryParse(Arguments[1], NumberStyles.Float, Culture, out y)) {
													System.Windows.Forms.MessageBox.Show("Y is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else {
													CurrentHudElements[Length - 1].TextPosition.X = x;
													CurrentHudElements[Length - 1].TextPosition.Y = y;
												}
											} else {
												System.Windows.Forms.MessageBox.Show("Incorrect number of arguments supplied in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
											} break;
										case "textalignment":
											if (Arguments.Length == 2) {
												int x, y;
												if (!int.TryParse(Arguments[0], NumberStyles.Integer, Culture, out x)) {
													System.Windows.Forms.MessageBox.Show("X is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else if (!int.TryParse(Arguments[1], NumberStyles.Integer, Culture, out y)) {
													System.Windows.Forms.MessageBox.Show("Y is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else {
													CurrentHudElements[Length - 1].TextAlignment.X = Math.Sign(x);
													CurrentHudElements[Length - 1].TextAlignment.Y = Math.Sign(y);
												}
											} else {
												System.Windows.Forms.MessageBox.Show("Incorrect number of arguments supplied in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
											} break;
										case "textsize":
											if (Arguments.Length == 1) {
												int s;
												if (!int.TryParse(Arguments[0], NumberStyles.Integer, Culture, out s)) {
													System.Windows.Forms.MessageBox.Show("SIZE is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else {
													switch (s) {
														case 0: CurrentHudElements[Length - 1].Font = Fonts.VerySmallFont; break;
														case 1: CurrentHudElements[Length - 1].Font = Fonts.SmallFont; break;
														case 2: CurrentHudElements[Length - 1].Font = Fonts.NormalFont; break;
														case 3: CurrentHudElements[Length - 1].Font = Fonts.LargeFont; break;
														case 4: CurrentHudElements[Length - 1].Font = Fonts.VeryLargeFont; break;
														default: CurrentHudElements[Length - 1].Font = Fonts.NormalFont; break;
													}
												}
											} else {
												System.Windows.Forms.MessageBox.Show("Incorrect number of arguments supplied in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
											} break;
										case "textshadow":
											if (Arguments.Length == 1) {
												int s;
												if (!int.TryParse(Arguments[0], NumberStyles.Integer, Culture, out s)) {
													System.Windows.Forms.MessageBox.Show("SHADOW is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else {
													CurrentHudElements[Length - 1].TextShadow = s != 0;
												}
											} else {
												System.Windows.Forms.MessageBox.Show("Incorrect number of arguments supplied in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
											} break;
										case "text":
											if (Arguments.Length == 1) {
												CurrentHudElements[Length - 1].Text = Arguments[0];
											} else {
												System.Windows.Forms.MessageBox.Show("Incorrect number of arguments supplied in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
											} break;
										case "value":
											if (Arguments.Length == 1) {
												int n;
												if (!int.TryParse(Arguments[0], NumberStyles.Integer, Culture, out n)) {
													System.Windows.Forms.MessageBox.Show("VALUE1 is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else {
													CurrentHudElements[Length - 1].Value1 = n;
												}
											} else if (Arguments.Length == 2) {
												float a, b;
												if (!float.TryParse(Arguments[0], NumberStyles.Float, Culture, out a)) {
													System.Windows.Forms.MessageBox.Show("VALUE1 is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else if (!float.TryParse(Arguments[1], NumberStyles.Float, Culture, out b)) {
													System.Windows.Forms.MessageBox.Show("VALUE2 is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else {
													CurrentHudElements[Length - 1].Value1 = a;
													CurrentHudElements[Length - 1].Value2 = b;
												}
											} else {
												System.Windows.Forms.MessageBox.Show("Incorrect number of arguments supplied in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
											} break;
										case "transition":
											if (Arguments.Length == 1) {
												int n;
												if (!int.TryParse(Arguments[0], NumberStyles.Integer, Culture, out n)) {
													System.Windows.Forms.MessageBox.Show("TRANSITION is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else {
													CurrentHudElements[Length - 1].Transition = (HudTransition)n;
												}
											} else {
												System.Windows.Forms.MessageBox.Show("Incorrect number of arguments supplied in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
											} break;
										case "transitionvector":
											if (Arguments.Length == 2) {
												float x, y;
												if (!float.TryParse(Arguments[0], NumberStyles.Float, Culture, out x)) {
													System.Windows.Forms.MessageBox.Show("X is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else if (!float.TryParse(Arguments[1], NumberStyles.Float, Culture, out y)) {
													System.Windows.Forms.MessageBox.Show("Y is invalid in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
												} else {
													CurrentHudElements[Length - 1].TransitionVector.X = x;
													CurrentHudElements[Length - 1].TransitionVector.Y = y;
												}
											} else {
												System.Windows.Forms.MessageBox.Show("Incorrect number of arguments supplied in " + Command + " at line " + (i + 1).ToString(Culture) + " in " + File);
											} break;
										default:
											System.Windows.Forms.MessageBox.Show("Invalid command encountered at line " + (i + 1).ToString(Culture) + " in " + File);
											break;
									}
								} else {
									System.Windows.Forms.MessageBox.Show("Invalid statement encountered at line " + (i + 1).ToString(Culture) + " in " + File);
								}
							}
						}
					}
				}
			}
			Array.Resize<HudElement>(ref CurrentHudElements, Length);
		}
    }
}

