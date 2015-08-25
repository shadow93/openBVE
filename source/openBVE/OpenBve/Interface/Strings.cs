using System;

namespace OpenBve
{
    internal static class Strings
	{
		internal struct InterfaceQuickReference {
			internal string HandleForward;
			internal string HandleNeutral;
			internal string HandleBackward;
			internal string HandlePower;
			internal string HandlePowerNull;
			internal string HandleBrake;
			internal string HandleBrakeNull;
			internal string HandleRelease;
			internal string HandleLap;
			internal string HandleService;
			internal string HandleEmergency;
			internal string HandleHoldBrake;
			internal string DoorsLeft;
			internal string DoorsRight;
			internal string Score;
		}
		internal static InterfaceQuickReference QuickReferences;
		private struct InterfaceString {
			internal string Name;
			internal string Text;
		}
		private static InterfaceString[] InterfaceStrings = new InterfaceString[16];
		private static int InterfaceStringCount = 0;
		private static int CurrentInterfaceStringIndex = 0;
		private static void AddInterfaceString(string Name, string Text) {
			if (InterfaceStringCount >= InterfaceStrings.Length) {
				Array.Resize<InterfaceString>(ref InterfaceStrings, InterfaceStrings.Length << 1);
			}
			InterfaceStrings[InterfaceStringCount].Name = Name;
			InterfaceStrings[InterfaceStringCount].Text = Text;
			InterfaceStringCount++;
		}
		internal static string GetInterfaceString(string Name) {
			int n = Name.Length;
			for (int k = 0; k < InterfaceStringCount; k++) {
				int i;
				if ((k & 1) == 0) {
					i = (CurrentInterfaceStringIndex + (k >> 1) + InterfaceStringCount) % InterfaceStringCount;
				} else {
					i = (CurrentInterfaceStringIndex - (k + 1 >> 1) + InterfaceStringCount) % InterfaceStringCount;
				}
				if (InterfaceStrings[i].Name.Length == n) {
					if (InterfaceStrings[i].Name == Name) {
						CurrentInterfaceStringIndex = (i + 1) % InterfaceStringCount;
						return InterfaceStrings[i].Text;
					}
				}
			}
			return Name;
		}

		// load language
		internal static void LoadLanguage(string File) {
			string[] Lines = System.IO.File.ReadAllLines(File, new System.Text.UTF8Encoding());
			string Section = "";
			InterfaceStrings = new InterfaceString[16];
			InterfaceStringCount = 0;
			QuickReferences.HandleForward = "F";
			QuickReferences.HandleNeutral = "N";
			QuickReferences.HandleBackward = "B";
			QuickReferences.HandlePower = "P";
			QuickReferences.HandlePowerNull = "N";
			QuickReferences.HandleBrake = "B";
			QuickReferences.HandleBrakeNull = "N";
			QuickReferences.HandleRelease = "RL";
			QuickReferences.HandleLap = "LP";
			QuickReferences.HandleService = "SV";
			QuickReferences.HandleEmergency = "EM";
			QuickReferences.HandleHoldBrake = "HB";
			QuickReferences.DoorsLeft = "L";
			QuickReferences.DoorsRight = "R";
			QuickReferences.Score = "Score: ";
			for (int i = 0; i < Lines.Length; i++) {
				Lines[i] = Lines[i].Trim();
				if (!Lines[i].StartsWith(";", StringComparison.Ordinal)) {
					if (Lines[i].StartsWith("[", StringComparison.Ordinal) & Lines[i].EndsWith("]", StringComparison.Ordinal)) {
						Section = Lines[i].Substring(1, Lines[i].Length - 2).Trim().ToLowerInvariant();
					} else {
						int j = Lines[i].IndexOf('=');
						if (j >= 0) {
							string a = Lines[i].Substring(0, j).TrimEnd().ToLowerInvariant();
							string b = Strings.Unescape(Lines[i].Substring(j + 1).TrimStart());
							switch (Section) {
								case "handles":
									switch (a) {
										case "forward": QuickReferences.HandleForward = b; break;
										case "neutral": QuickReferences.HandleNeutral = b; break;
										case "backward": QuickReferences.HandleBackward = b; break;
										case "power": QuickReferences.HandlePower = b; break;
										case "powernull": QuickReferences.HandlePowerNull = b; break;
										case "brake": QuickReferences.HandleBrake = b; break;
										case "brakenull": QuickReferences.HandleBrakeNull = b; break;
										case "release": QuickReferences.HandleRelease = b; break;
										case "lap": QuickReferences.HandleLap = b; break;
										case "service": QuickReferences.HandleService = b; break;
										case "emergency": QuickReferences.HandleEmergency = b; break;
										case "holdbrake": QuickReferences.HandleHoldBrake = b; break;
									} break;
								case "doors":
									switch (a) {
										case "left": QuickReferences.DoorsLeft = b; break;
										case "right": QuickReferences.DoorsRight = b; break;
									} break;
								case "misc":
									switch (a) {
										case "score": QuickReferences.Score = b; break;
									} break;
								case "commands":
									{
										for (int k = 0; k < Controls.CommandInfos.Length; k++) {
											if (string.Compare(Controls.CommandInfos[k].Name, a, StringComparison.OrdinalIgnoreCase) == 0) {
												Controls.CommandInfos[k].Description = b;
												break;
											}
										}
									} break;
								case "keys":
									{
										for (int k = 0; k < Controls.Keys.Length; k++) {
											if (Controls.Keys[k].Scancode != SDL2.SDL.SDL_Scancode.SDL_SCANCODE_UNKNOWN &&
												string.Compare(Controls.Keys[k].Name, a, StringComparison.OrdinalIgnoreCase) == 0) {
												Controls.Keys[k].Description = b;
												break;
											}
										}
									} break;
								default:
									AddInterfaceString(Section + "_" + a, b);
									break;
							}
						}
					}
				}
			}
		}


		// is japanese
		internal static bool IsJapanese(string Name) {
			for (int i = 0; i < Name.Length; i++) {
				int a = char.ConvertToUtf32(Name, i);
				if (a < 0x10000) {
					bool q = false;
					while (true) {
						if (a >= 0x2E80 & a <= 0x2EFF) break;
						if (a >= 0x3000 & a <= 0x30FF) break;
						if (a >= 0x31C0 & a <= 0x4DBF) break;
						if (a >= 0x4E00 & a <= 0x9FFF) break;
						if (a >= 0xF900 & a <= 0xFAFF) break;
						if (a >= 0xFE30 & a <= 0xFE4F) break;
						if (a >= 0xFF00 & a <= 0xFFEF) break;
						q = true; break;
					} if (q) return false;
				} else {
					return false;
				}
			} return true;
		}

		internal enum Encoding {
			Unknown = 0,
			Utf8 = 1,
			Utf16Le = 2,
			Utf16Be = 3,
			Utf32Le = 4,
			Utf32Be = 5,
		}
		internal static Encoding GetEncodingFromFile(string File) {
			try {
				byte[] Data = System.IO.File.ReadAllBytes(File);
				if (Data.Length >= 3) {
					if (Data[0] == 0xEF & Data[1] == 0xBB & Data[2] == 0xBF) return Encoding.Utf8;
				}
				if (Data.Length >= 2) {
					if (Data[0] == 0xFE & Data[1] == 0xFF) return Encoding.Utf16Be;
					if (Data[0] == 0xFF & Data[1] == 0xFE) return Encoding.Utf16Le;
				}
				if (Data.Length >= 4) {
					if (Data[0] == 0x00 & Data[1] == 0x00 & Data[2] == 0xFE & Data[3] == 0xFF) return Encoding.Utf32Be;
					if (Data[0] == 0xFF & Data[1] == 0xFE & Data[2] == 0x00 & Data[3] == 0x00) return Encoding.Utf32Le;
				}
				return Encoding.Unknown;
			} catch {
				return Encoding.Unknown;
			}
		}
		internal static Encoding GetEncodingFromFile(string Folder, string File) {
			return GetEncodingFromFile(OpenBveApi.Path.CombineFile(Folder, File));
		}


		// unescape
		internal static string Unescape(string Text) {
			System.Text.StringBuilder Builder = new System.Text.StringBuilder(Text.Length);
			int Start = 0;
			for (int i = 0; i < Text.Length; i++) {
				if (Text[i] == '\\') {
					Builder.Append(Text, Start, i - Start);
					if (i + 1 < Text.Length) {
						switch (Text[i + 1]) {
							case 'a': Builder.Append('\a'); break;
							case 'b': Builder.Append('\b'); break;
							case 't': Builder.Append('\t'); break;
							case 'n': Builder.Append('\n'); break;
							case 'v': Builder.Append('\v'); break;
							case 'f': Builder.Append('\f'); break;
							case 'r': Builder.Append('\r'); break;
							case 'e': Builder.Append('\x1B'); break;
							case 'c':
								if (i + 2 < Text.Length) {
									int CodePoint = char.ConvertToUtf32(Text, i + 2);
									if (CodePoint >= 0x40 & CodePoint <= 0x5F) {
										Builder.Append(char.ConvertFromUtf32(CodePoint - 64));
									} else if (CodePoint == 0x3F) {
										Builder.Append('\x7F');
									} else {
										//Debug.AddMessage(MessageType.Error, false, "Unrecognized control character found in " + Text.Substring(i, 3));
										return Text;
									} i++;
								} else {
									//Debug.AddMessage(MessageType.Error, false, "Insufficient characters available in " + Text + " to decode control character escape sequence");
									return Text;
								} break;
							case '"':
								Builder.Append('"');
								break;
							case '\\':
								Builder.Append('\\');
								break;
							case 'x':
								if (i + 3 < Text.Length) {
									Builder.Append(char.ConvertFromUtf32(Convert.ToInt32(Text.Substring(i + 2, 2), 16)));
									i += 2;
								} else {
									//Debug.AddMessage(MessageType.Error, false, "Insufficient characters available in " + Text + " to decode hexadecimal escape sequence.");
									return Text;
								} break;
							case 'u':
								if (i + 5 < Text.Length) {
									Builder.Append(char.ConvertFromUtf32(Convert.ToInt32(Text.Substring(i + 2, 4), 16)));
									i += 4;
								} else {
									//Debug.AddMessage(MessageType.Error, false, "Insufficient characters available in " + Text + " to decode hexadecimal escape sequence.");
									return Text;
								} break;
							default:
								//Debug.AddMessage(MessageType.Error, false, "Unrecognized escape sequence found in " + Text + ".");
								return Text;
						}
						i++;
						Start = i + 1;
					} else {
						//Debug.AddMessage(MessageType.Error, false, "Insufficient characters available in " + Text + " to decode escape sequence.");
						return Text;
					}
				}
			}
			Builder.Append(Text, Start, Text.Length - Start);
			return Builder.ToString();
		}

		// ================================

		// convert newlines to crlf
		internal static string ConvertNewlinesToCrLf(string Text) {
			System.Text.StringBuilder Builder = new System.Text.StringBuilder();
			for (int i = 0; i < Text.Length; i++) {
				int a = char.ConvertToUtf32(Text, i);
				if (a == 0xD & i < Text.Length - 1) {
					int b = char.ConvertToUtf32(Text, i + 1);
					if (b == 0xA) {
						Builder.Append("\r\n");
						i++;
					} else {
						Builder.Append("\r\n");
					}
				} else if (a == 0xA | a == 0xC | a == 0xD | a == 0x85 | a == 0x2028 | a == 0x2029) {
					Builder.Append("\r\n");
				} else if (a < 0x10000) {
					Builder.Append(Text[i]);
				} else {
					Builder.Append(Text.Substring(i, 2));
					i++;
				}
			} return Builder.ToString();
		}
    }
}

