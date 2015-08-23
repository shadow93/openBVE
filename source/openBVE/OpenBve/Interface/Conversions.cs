using System;
using System.Globalization;
using OpenBveApi.Colors;

namespace OpenBve
{
	internal static class Conversions
    {

		// ================================

		// try parse vb6
		internal static bool TryParseDoubleVb6(string Expression, out double Value) {
			Expression = TrimInside(Expression);
			CultureInfo Culture = CultureInfo.InvariantCulture;
			for (int n = Expression.Length; n > 0; n--) {
				double a;
				if (double.TryParse(Expression.Substring(0, n), NumberStyles.Float, Culture, out a)) {
					Value = a;
					return true;
				}
			}
			Value = 0.0;
			return false;
		}
		internal static bool TryParseFloatVb6(string Expression, out float Value) {
			Expression = TrimInside(Expression);
			CultureInfo Culture = CultureInfo.InvariantCulture;
			for (int n = Expression.Length; n > 0; n--) {
				float a;
				if (float.TryParse(Expression.Substring(0, n), NumberStyles.Float, Culture, out a)) {
					Value = a;
					return true;
				}
			}
			Value = 0.0f;
			return false;
		}
		internal static bool TryParseIntVb6(string Expression, out int Value) {
			Expression = TrimInside(Expression);
			CultureInfo Culture = CultureInfo.InvariantCulture;
			for (int n = Expression.Length; n > 0; n--) {
				double a;
				if (double.TryParse(Expression.Substring(0, n), NumberStyles.Float, Culture, out a)) {
					if (a >= -2147483648.0 & a <= 2147483647.0) {
						Value = (int)Math.Round(a);
						return true;
					}
					break;
				}
			}
			Value = 0;
			return false;
		}

		// try parse time
		internal static bool TryParseTime(string Expression, out double Value) {
			Expression = TrimInside(Expression);
			if (Expression.Length != 0) {
				CultureInfo Culture = CultureInfo.InvariantCulture;
				int i = Expression.IndexOf('.');
				if (i >= 1) {
					int h; if (int.TryParse(Expression.Substring(0, i), NumberStyles.Integer, Culture, out h)) {
						int n = Expression.Length - i - 1;
						if (n == 1 | n == 2) {
							uint m; if (uint.TryParse(Expression.Substring(i + 1, n), NumberStyles.None, Culture, out m)) {
								Value = 3600.0 * (double)h + 60.0 * (double)m;
								return true;
							}
						} else if (n == 3 | n == 4) {
							uint m; if (uint.TryParse(Expression.Substring(i + 1, 2), NumberStyles.None, Culture, out m)) {
								uint s; if (uint.TryParse(Expression.Substring(i + 3, n - 2), NumberStyles.None, Culture, out s)) {
									Value = 3600.0 * (double)h + 60.0 * (double)m + (double)s;
									return true;
								}
							}
						}
					}
				} else if (i == -1) {
					int h; if (int.TryParse(Expression, NumberStyles.Integer, Culture, out h)) {
						Value = 3600.0 * (double)h;
						return true;
					}
				}
			}
			Value = 0.0;
			return false;
		}

		// try parse hex color
		internal static bool TryParseHexColor(string Expression, out Color24 Color){
			if (Expression.StartsWith("#", StringComparison.Ordinal)) {
				string a = Expression.Substring(1).TrimStart();
				int x;
				if (int.TryParse(a, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out x)) {
					int r = (x >> 16) & 0xFF;
					int g = (x >> 8) & 0xFF;
					int b = x & 0xFF;
					if (r >= 0 & r <= 255 & g >= 0 & g <= 255 & b >= 0 & b <= 255) {
						Color = new Color24((byte)r, (byte)g, (byte)b);
						return true;
					}
					Color = new Color24(0, 0, 255);
					return false;
				}
				Color = new Color24(0, 0, 255);
				return false;
			}
			Color = new Color24(0, 0, 255);
			return false;
		}
		internal static bool TryParseHexColor(string Expression, out Color32 Color) {
			if (Expression.StartsWith("#", StringComparison.Ordinal)) {
				string a = Expression.Substring(1).TrimStart();
				int x; if (int.TryParse(a, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out x)) {
					int r = (x >> 16) & 0xFF;
					int g = (x >> 8) & 0xFF;
					int b = x & 0xFF;
					if (r >= 0 & r <= 255 & g >= 0 & g <= 255 & b >= 0 & b <= 255) {
						Color = new Color32((byte)r, (byte)g, (byte)b, 255);
						return true;
					}
					Color = new Color32(0, 0, 255, 255);
					return false;
				}
				Color = new Color32(0, 0, 255, 255);
				return false;
			}
			Color = new Color32(0, 0, 255, 255);
			return false;
		}

		// try parse with unit factors
		internal static bool TryParseDouble(string Expression, double[] UnitFactors, out double Value) {
			double a;
			if (double.TryParse(Expression, NumberStyles.Any, CultureInfo.InvariantCulture, out a)) {
				Value = a * UnitFactors[UnitFactors.Length - 1];
				return true;
			} else {
				string[] parameters = Expression.Split(':');
				if (parameters.Length <= UnitFactors.Length) {
					Value = 0.0;
					for (int i = 0; i < parameters.Length; i++) {
						if (double.TryParse(parameters[i].Trim(), NumberStyles.Float, CultureInfo.InvariantCulture, out a)) {
							int j = i + UnitFactors.Length - parameters.Length;
							Value += a * UnitFactors[j];
						} else {
							return false;
						}
					}
					return true;
				}
				Value = 0.0;
				return false;
			}
		}
		internal static bool TryParseDoubleVb6(string Expression, double[] UnitFactors, out double Value) {
			double a;
			if (double.TryParse(Expression, NumberStyles.Any, CultureInfo.InvariantCulture, out a)) {
				Value = a * UnitFactors[UnitFactors.Length - 1];
				return true;
			} else {
				string[] parameters = Expression.Split(':');
				Value = 0.0;
				if (parameters.Length <= UnitFactors.Length) {
					for (int i = 0; i < parameters.Length; i++) {
						if (TryParseDoubleVb6(parameters[i].Trim(), out a)) {
							int j = i + UnitFactors.Length - parameters.Length;
							Value += a * UnitFactors[j];
						} else {
							return false;
						}
					}
					return true;
				}
				return false;
			}
		}

		// trim inside
		internal static string TrimInside(string Expression) {
			System.Text.StringBuilder Builder = new System.Text.StringBuilder(Expression.Length);
			for (int i = 0; i < Expression.Length; i++) {
				char c = Expression[i];
				if (!char.IsWhiteSpace(c)) {
					Builder.Append(c);
				}
			} return Builder.ToString();
		}
    }
}

