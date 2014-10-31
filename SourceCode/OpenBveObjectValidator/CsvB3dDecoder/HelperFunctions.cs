using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

using TrainsimApi.Codecs;
using TrainsimApi.Vectors;

namespace CsvB3dDecoder {
	public partial class Decoder : MeshDecoder {
		
		
		// --- validation functions ---
		
		private static void CheckMeshBuilderPresence(string command, LineInformation lineInfo, ref bool meshBuilderPresent) {
			if (!meshBuilderPresent) {
				string meshBuilder = lineInfo.FileInfo.IsB3d ? "[MeshBuilder]" : "CreateMeshBuilder";
				string text = meshBuilder + " is required before " + command + " can be used on line " + lineInfo.LineNumber.ToString() + " in file \"" + lineInfo.FileInfo.File + "\".";
				lineInfo.FileInfo.Logger.Add(text);
				meshBuilderPresent = true;
			}
		}
		
		private static bool CheckCommand(string actual, string b3dCommand, string csvCommand, LineInformation lineInfo) {
			if (lineInfo.FileInfo.IsB3d) {
				if (!string.Equals(actual, b3dCommand, StringComparison.OrdinalIgnoreCase)) {
					string text = actual + " should be " + b3dCommand + " in B3D files on line " + lineInfo.LineNumber.ToString() + " in file \"" + lineInfo.FileInfo.File + "\".";
					lineInfo.FileInfo.Logger.Add(text);
					return false;
				}
			} else {
				if (!string.Equals(actual, csvCommand, StringComparison.OrdinalIgnoreCase)) {
					string text = actual + " should be " + csvCommand + " in CSV files on line " + lineInfo.LineNumber.ToString() + " in file \"" + lineInfo.FileInfo.File + "\".";
					lineInfo.FileInfo.Logger.Add(text);
					return false;
				}
			}
			return true;
		}
		
		private static bool CheckArgumentCount(string command, int actual, int lower, int upper, LineInformation lineInfo) {
			if (actual < lower | actual > upper) {
				if (lower == upper) {
					string text = command + " expects " + lower.ToString() + (lower == 1 ? " argument" : " arguments") + " but " + actual.ToString() + (actual == 1 ? " argument was" : " arguments were") + " found on line " + (lineInfo.LineNumber).ToString() + " in file \"" + lineInfo.FileInfo.File + "\".";
					lineInfo.FileInfo.Logger.Add(text);
				} else if (lower == 0) {
					string text = command + " expects at most " + upper.ToString() + (upper == 1 ? " argument" : " arguments") + " but " + actual.ToString() + (actual == 1 ? " argument was" : " arguments were") + " found on line " + (lineInfo.LineNumber).ToString() + " in \"" + lineInfo.FileInfo.File + "\".";
					lineInfo.FileInfo.Logger.Add(text);
				} else if (upper == int.MaxValue) {
					string text = command + " expects at least " + lower.ToString() + (lower == 1 ? " argument" : " arguments") + " but " + actual.ToString() + (actual == 1 ? " argument was" : " arguments were") + " found on line " + (lineInfo.LineNumber).ToString() + " in \"" + lineInfo.FileInfo.File + "\".";
					lineInfo.FileInfo.Logger.Add(text);
				} else {
					string text = command + " expects between " + lower.ToString() + " and " + upper.ToString() + " arguments but " + actual.ToString() + (actual == 1 ? " argument was" : " arguments were") + " found on line " + (lineInfo.LineNumber).ToString() + " in \"" + lineInfo.FileInfo.File + "\".";
					lineInfo.FileInfo.Logger.Add(text);
				}
				return false;
			} else {
				return true;
			}
		}
		
		
		// --- visual-basic-compatible number parsing (int32) ---
		
		private static int GetInt32FromArgument(int argumentIndex, string argumentName, int defaultValue, LineInformation lineInfo) {
			int value;
			if (TryGetInt32FromArgument(argumentIndex, argumentName, int.MinValue, int.MaxValue, defaultValue, true, lineInfo, out value)) {
				return value;
			} else {
				return defaultValue;
			}
		}
		
		private static bool TryGetInt32FromArgument(int argumentIndex, string argumentName, int minValue, int maxValue, int defaultValue, bool allowMissingArgument, LineInformation lineInfo, out int value) {
			if (argumentIndex >= 0 && argumentIndex < lineInfo.ArgumentCount && lineInfo.Arguments[argumentIndex].Length != 0) {
				int success = TryParseInt32Vb(lineInfo.Arguments[argumentIndex], out value);
				if (success == 0) {
					string text = "\"" + lineInfo.Arguments[argumentIndex] + "\" as argument " + (argumentIndex + 1).ToString() + " (" + argumentName + ") to command " + lineInfo.Command + " is not a valid integer on line " + lineInfo.LineNumber.ToString() + " in file \"" + lineInfo.FileInfo.File + "\".";
					lineInfo.FileInfo.Logger.Add(text);
					value = defaultValue;
					return false;
				}
				if (lineInfo.FileInfo.StrictParsing & success == -1) {
					string text = "\"" + lineInfo.Arguments[argumentIndex] + "\" as argument " + (argumentIndex + 1).ToString() + " (" + argumentName + ") to command " + lineInfo.Command + " is a malformed integer and will be treated as " + value.ToString() + " on line " + lineInfo.LineNumber.ToString() + " in file \"" + lineInfo.FileInfo.File + "\".";
					lineInfo.FileInfo.Logger.Add(text);
				}
				if (value < minValue | value > maxValue) {
					if (minValue == int.MinValue) {
						value = maxValue;
						string text = "\"" + lineInfo.Arguments[argumentIndex] + "\" as argument " + (argumentIndex + 1).ToString() + " (" + argumentName + ") to command " + lineInfo.Command + " must be at most " + maxValue.ToString() + " on line " + lineInfo.LineNumber.ToString() + " in file \"" + lineInfo.FileInfo.File + "\".";
						lineInfo.FileInfo.Logger.Add(text);
					} else if (maxValue == int.MaxValue) {
						value = minValue;
						string text = "\"" + lineInfo.Arguments[argumentIndex] + "\" as argument " + (argumentIndex + 1).ToString() + " (" + argumentName + ") to command " + lineInfo.Command + " must be at least " + minValue.ToString() + " on line " + lineInfo.LineNumber.ToString() + " in file \"" + lineInfo.FileInfo.File + "\".";
						lineInfo.FileInfo.Logger.Add(text);
					} else {
						value = value < minValue ? minValue : maxValue;
						string text = "\"" + lineInfo.Arguments[argumentIndex] + "\" as argument " + (argumentIndex + 1).ToString() + " (" + argumentName + ") to command " + lineInfo.Command + " must be between " + minValue.ToString() + " and " + maxValue.ToString() + " on line " + lineInfo.LineNumber.ToString() + " in file \"" + lineInfo.FileInfo.File + "\".";
						lineInfo.FileInfo.Logger.Add(text);
					}
					return false;
				}
				return true;
			} else if (allowMissingArgument) {
				value = defaultValue;
				return true;
			} else {
				string text = "Argument " + (argumentIndex + 1).ToString() + " (" + argumentName + ") to command " + lineInfo.Command + " is missing on line " + lineInfo.LineNumber.ToString() + " in file \"" + lineInfo.FileInfo.File + "\".";
				lineInfo.FileInfo.Logger.Add(text);
				value = defaultValue;
				return false;
			}
		}
		
		private static int TryParseInt32Vb(string text, out int value) {
			if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value)) {
				return 1;
			} else {
				text = TrimInside(text);
				while (text.Length != 0) {
					if (int.TryParse(text, NumberStyles.Integer, CultureInfo.InvariantCulture, out value)) {
						return -1;
					}
					text = text.Substring(0, text.Length - 1);
				}
				value = 0;
				return 0;
			}
		}

		
		// --- visual-basic-compatible number parsing (double) ---
		
		private static double GetDoubleFromArgument(int argumentIndex, string argumentName, double defaultValue, LineInformation lineInfo) {
			double value;
			if (TryGetDoubleFromArgument(argumentIndex, argumentName, double.MinValue, double.MaxValue, defaultValue, true, lineInfo, out value)) {
				return value;
			} else {
				return defaultValue;
			}
		}
		
		private static bool TryGetDoubleFromArgument(int argumentIndex, string argumentName, double minValue, double maxValue, double defaultValue, bool allowMissingArgument, LineInformation lineInfo, out double value) {
			if (argumentIndex >= 0 && argumentIndex < lineInfo.ArgumentCount && lineInfo.Arguments[argumentIndex].Length != 0) {
				int success = TryParseDoubleVb(lineInfo.Arguments[argumentIndex], out value);
				if (success == 0) {
					string text = "\"" + lineInfo.Arguments[argumentIndex] + "\" as argument " + (argumentIndex + 1).ToString() + " (" + argumentName + ") to command " + lineInfo.Command + " is not a valid floating-point number on line " + lineInfo.LineNumber.ToString() + " in file \"" + lineInfo.FileInfo.File + "\".";
					lineInfo.FileInfo.Logger.Add(text);
					value = defaultValue;
					return false;
				}
				if (lineInfo.FileInfo.StrictParsing & success == -1) {
					string text = "\"" + lineInfo.Arguments[argumentIndex] + "\" as argument " + (argumentIndex + 1).ToString() + " (" + argumentName + ") to command " + lineInfo.Command + " is a malformed floating-point number and will be treated as " + value.ToString() + " on line " + lineInfo.LineNumber.ToString() + " in file \"" + lineInfo.FileInfo.File + "\".";
					lineInfo.FileInfo.Logger.Add(text);
				}
				if (value < minValue | value > maxValue) {
					if (minValue == double.MinValue) {
						value = maxValue;
						string text = "\"" + lineInfo.Arguments[argumentIndex] + "\" as argument " + (argumentIndex + 1).ToString() + " (" + argumentName + ") to command " + lineInfo.Command + " must be at most " + maxValue.ToString() + " on line " + lineInfo.LineNumber.ToString() + " in file \"" + lineInfo.FileInfo.File + "\".";
						lineInfo.FileInfo.Logger.Add(text);
					} else if (maxValue == double.MaxValue) {
						value = minValue;
						string text = "\"" + lineInfo.Arguments[argumentIndex] + "\" as argument " + (argumentIndex + 1).ToString() + " (" + argumentName + ") to command " + lineInfo.Command + " must be at least " + minValue.ToString() + " on line " + lineInfo.LineNumber.ToString() + " in file \"" + lineInfo.FileInfo.File + "\".";
						lineInfo.FileInfo.Logger.Add(text);
					} else {
						value = value < minValue ? minValue : maxValue;
						string text = "\"" + lineInfo.Arguments[argumentIndex] + "\" as argument " + (argumentIndex + 1).ToString() + " (" + argumentName + ") to command " + lineInfo.Command + " must be between " + minValue.ToString() + " and " + maxValue.ToString() + " on line " + lineInfo.LineNumber.ToString() + " in file \"" + lineInfo.FileInfo.File + "\".";
						lineInfo.FileInfo.Logger.Add(text);
					}
					return false;
				}
				return true;
			} else if (allowMissingArgument) {
				value = defaultValue;
				return true;
			} else {
				string text = "Argument " + (argumentIndex + 1).ToString() + " (" + argumentName + ") to command " + lineInfo.Command + " is missing on line " + lineInfo.LineNumber.ToString() + " in file \"" + lineInfo.FileInfo.File + "\".";
				lineInfo.FileInfo.Logger.Add(text);
				value = defaultValue;
				return false;
			}
		}
		
		private static int TryParseDoubleVb(string text, out double value) {
			if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value)) {
				return 1;
			} else {
				text = TrimInside(text);
				while (text.Length != 0) {
					if (double.TryParse(text, NumberStyles.Float, CultureInfo.InvariantCulture, out value)) {
						return -1;
					}
					text = text.Substring(0, text.Length - 1);
				}
				value = 0.0f;
				return 0;
			}
		}
		
		
		// --- visual-basic-compatible number parsing (helper functions) ---
		
		private static string TrimInside(string text) {
			StringBuilder builder = new StringBuilder(text.Length);
			for (int i = 0; i < text.Length; i++) {
				char value = text[i];
				if (!char.IsWhiteSpace(value)) {
					builder.Append(value);
				}
			}
			return builder.ToString();
		}

		
	}
}