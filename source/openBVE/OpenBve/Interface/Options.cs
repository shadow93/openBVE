using System;
using System.Globalization;

namespace OpenBve
{
	internal class Options {
		
		internal enum MotionBlurMode {
			None = 0,
			Low = 1,
			Medium = 2,
			High = 3
		}
		internal enum GameMode {
			Arcade = 0,
			Normal = 1,
			Expert = 2
		}
		internal enum InterpolationMode {
			NearestNeighbor,
			Bilinear,
			NearestNeighborMipmapped,
			BilinearMipmapped,
			TrilinearMipmapped,
			AnisotropicFiltering
		}
		internal struct EncodingValue {
			internal int Codepage;
			internal string Value;
		}

		internal string LanguageCode;
		internal bool FullscreenMode;
		internal bool VerticalSynchronization;
		internal int WindowWidth;
		internal int WindowHeight;
		internal int FullscreenWidth;
		internal int FullscreenHeight;
		internal string UserInterfaceFolder;
		internal InterpolationMode Interpolation;
		internal Renderer.TransparencyMode TransparencyMode;
		internal int AnisotropicFilteringLevel;
		internal int AnisotropicFilteringMaximum;
		internal int AntiAliasingLevel;
		internal int ViewingDistance;
		internal MotionBlurMode MotionBlur;
		internal int ObjectOptimizationBasicThreshold;
		internal int ObjectOptimizationFullThreshold;
		internal bool Toppling;
		internal bool Collisions;
		internal bool Derailments;
		internal bool BlackBox;
		internal bool UseJoysticks;
		internal double JoystickAxisThreshold;
		internal double KeyRepeatDelay;
		internal double KeyRepeatInterval;
		internal Sounds.SoundModels SoundModel;
		internal Sounds.SoundRange SoundRange;
		internal int SoundNumber;
		internal bool ShowWarningMessages;
		internal bool ShowErrorMessages;
		internal string RouteFolder;
		internal string TrainFolder;
		internal string[] RecentlyUsedRoutes;
		internal string[] RecentlyUsedTrains;
		internal int RecentlyUsedLimit;
		internal EncodingValue[] RouteEncodings;
		internal EncodingValue[] TrainEncodings;
		internal GameMode CurrentGameMode;
		internal int MainMenuWidth;
		internal int MainMenuHeight;
		internal bool DisableDisplayLists;
		internal bool LoadInAdvance;
		internal bool NoTextureResize;
		internal string ProxyUrl;
		internal string ProxyUserName;
		internal string ProxyPassword;
		internal Options() {
			this.LanguageCode = "en-US";
			this.FullscreenMode = false;
			this.VerticalSynchronization = true;
			this.WindowWidth = 960;
			this.WindowHeight = 600;
			this.FullscreenWidth = 1024;
			this.FullscreenHeight = 768;
			this.UserInterfaceFolder = "Default";
			this.Interpolation = InterpolationMode.BilinearMipmapped;
			this.TransparencyMode = Renderer.TransparencyMode.Quality;
			this.AnisotropicFilteringLevel = 0;
			this.AnisotropicFilteringMaximum = 0;
			this.AntiAliasingLevel = 0;
			this.ViewingDistance = 600;
			this.MotionBlur = MotionBlurMode.None;
			this.Toppling = true;
			this.Collisions = true;
			this.Derailments = true;
			this.CurrentGameMode = GameMode.Normal;
			this.BlackBox = false;
			this.UseJoysticks = true;
			this.JoystickAxisThreshold = 0.0;
			this.KeyRepeatDelay = 0.5;
			this.KeyRepeatInterval = 0.1;
			this.SoundModel = Sounds.SoundModels.Inverse;
			this.SoundRange = Sounds.SoundRange.Low;
			this.SoundNumber = 16;
			this.ShowWarningMessages = true;
			this.ShowErrorMessages = true;
			this.ObjectOptimizationBasicThreshold = 10000;
			this.ObjectOptimizationFullThreshold = 1000;
			this.RouteFolder = "";
			this.TrainFolder = "";
			this.RecentlyUsedRoutes = new string[] { };
			this.RecentlyUsedTrains = new string[] { };
			this.RecentlyUsedLimit = 10;
			this.RouteEncodings = new EncodingValue[] { };
			this.TrainEncodings = new EncodingValue[] { };
			this.MainMenuWidth = 0;
			this.MainMenuHeight = 0;
			this.DisableDisplayLists = false;
			this.LoadInAdvance = false;
			this.NoTextureResize = false;
			this.ProxyUrl = string.Empty;
			this.ProxyUserName = string.Empty;
			this.ProxyPassword = string.Empty;
		}
	internal static Options Current;
	internal static void LoadOptions() {
		Current = new Options();
		CultureInfo Culture = CultureInfo.InvariantCulture;
		string File = OpenBveApi.Path.CombineFile(Program.FileSystem.SettingsFolder, "options.cfg");
		if (System.IO.File.Exists(File)) {
			// load options
			string[] Lines = System.IO.File.ReadAllLines(File, new System.Text.UTF8Encoding());
			string Section = "";
			for (int i = 0; i < Lines.Length; i++) {
				Lines[i] = Lines[i].Trim();
				if (Lines[i].Length != 0 && !Lines[i].StartsWith(";", StringComparison.OrdinalIgnoreCase)) {
					if (Lines[i].StartsWith("[", StringComparison.Ordinal) & Lines[i].EndsWith("]", StringComparison.Ordinal)) {
						Section = Lines[i].Substring(1, Lines[i].Length - 2).Trim().ToLowerInvariant();
					} else {
						int j = Lines[i].IndexOf("=", StringComparison.OrdinalIgnoreCase);
						string Key, Value;
						if (j >= 0) {
							Key = Lines[i].Substring(0, j).TrimEnd().ToLowerInvariant();
							Value = Lines[i].Substring(j + 1).TrimStart();
						} else {
							Key = "";
							Value = Lines[i];
						}
						switch (Section) {
							case "language":
								switch (Key) {
									case "code":
										Current.LanguageCode = Value.Length != 0 ? Value : "en-US";
										break;
								} break;
							case "interface":
								switch (Key) {
									case "folder":
										Current.UserInterfaceFolder = Value.Length != 0 ? Value : "Default";
										break;
								} break;
							case "display":
								switch (Key) {
									case "mode":
										Current.FullscreenMode = string.Compare(Value, "fullscreen", StringComparison.OrdinalIgnoreCase) == 0;
										break;
									case "vsync":
										Current.VerticalSynchronization = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
										break;
									case "windowwidth":
										{
											int a;
											if (!int.TryParse(Value, NumberStyles.Integer, Culture, out a)) {
												a = 960;
											}
											Current.WindowWidth = a;
										} break;
									case "windowheight":
										{
											int a;
											if (!int.TryParse(Value, NumberStyles.Integer, Culture, out a)) {
												a = 600;
											}
											Current.WindowHeight = a;
										} break;
									case "fullscreenwidth":
										{
											int a;
											if (!int.TryParse(Value, NumberStyles.Integer, Culture, out a)) {
												a = 1024;
											}
											Current.FullscreenWidth = a;
										} break;
									case "fullscreenheight":
										{
											int a;
											if (!int.TryParse(Value, NumberStyles.Integer, Culture, out a)) {
												a = 768;
											}
											Current.FullscreenHeight = a;
										} break;
									case "mainmenuwidth":
										{
											int a;
											int.TryParse(Value, NumberStyles.Integer, Culture, out a);
											Current.MainMenuWidth = a;
										} break;
									case "mainmenuheight":
										{
											int a;
											int.TryParse(Value, NumberStyles.Integer, Culture, out a);
											Current.MainMenuHeight = a;
										} break;
									case "disabledisplaylists":
										Current.DisableDisplayLists = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
										break;
									case "loadinadvance":
										Current.LoadInAdvance = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
										break;
									case "notextureresize":
										Current.NoTextureResize = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
										break;
								} break;
							case "quality":
								switch (Key) {
									case "interpolation":
										switch (Value.ToLowerInvariant()) {
											case "nearestneighbor": Current.Interpolation = InterpolationMode.NearestNeighbor; break;
											case "bilinear": Current.Interpolation = InterpolationMode.Bilinear; break;
											case "nearestneighbormipmapped": Current.Interpolation = InterpolationMode.NearestNeighborMipmapped; ; break;
											case "bilinearmipmapped": Current.Interpolation = InterpolationMode.BilinearMipmapped; break;
											case "trilinearmipmapped": Current.Interpolation = InterpolationMode.TrilinearMipmapped; break;
											case "anisotropicfiltering": Current.Interpolation = InterpolationMode.AnisotropicFiltering; break;
											default: Current.Interpolation = InterpolationMode.BilinearMipmapped; break;
										} break;
									case "anisotropicfilteringlevel":
										{
											int a;
											int.TryParse(Value, NumberStyles.Integer, Culture, out a);
											Current.AnisotropicFilteringLevel = a;
										} break;
									case "anisotropicfilteringmaximum":
										{
											int a;
											int.TryParse(Value, NumberStyles.Integer, Culture, out a);
											Current.AnisotropicFilteringMaximum = a;
										} break;
									case "antialiasinglevel":
										{
											int a;
											int.TryParse(Value, NumberStyles.Integer, Culture, out a);
											Current.AntiAliasingLevel = a;
										} break;
									case "transparencymode":
										switch (Value.ToLowerInvariant()) {
											case "sharp": Current.TransparencyMode = Renderer.TransparencyMode.Performance; break;
											case "smooth": Current.TransparencyMode = Renderer.TransparencyMode.Quality; break;
											default: {
													int a;
													if (int.TryParse(Value, NumberStyles.Integer, Culture, out a)) {
														Current.TransparencyMode = (Renderer.TransparencyMode)a;
													} else {
														Current.TransparencyMode = Renderer.TransparencyMode.Quality;
													}
													break;
												}
										} break;
									case "viewingdistance":
										{
											int a;
											int.TryParse(Value, NumberStyles.Integer, Culture, out a);
											Current.ViewingDistance = a;
										} break;
									case "motionblur":
										switch (Value.ToLowerInvariant()) {
											case "low": Current.MotionBlur = MotionBlurMode.Low; break;
											case "medium": Current.MotionBlur = MotionBlurMode.Medium; break;
											case "high": Current.MotionBlur = MotionBlurMode.High; break;
											default: Current.MotionBlur = MotionBlurMode.None; break;
										} break;
								} break;
							case "objectoptimization":
								switch (Key) {
									case "basicthreshold":
										{
											int a;
											int.TryParse(Value, NumberStyles.Integer, Culture, out a);
											Current.ObjectOptimizationBasicThreshold = a;
										} break;
									case "fullthreshold":
										{
											int a;
											int.TryParse(Value, NumberStyles.Integer, Culture, out a);
											Current.ObjectOptimizationFullThreshold = a;
										} break;
								} break;
							case "simulation":
								switch (Key) {
									case "toppling":
										Current.Toppling = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
										break;
									case "collisions":
										Current.Collisions = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
										break;
									case "derailments":
										Current.Derailments = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
										break;
									case "blackbox":
										Current.BlackBox = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
										break;
									case "mode":
										switch (Value.ToLowerInvariant()) {
												case "arcade": Current.CurrentGameMode = GameMode.Arcade; break;
												case "normal": Current.CurrentGameMode = GameMode.Normal; break;
												case "expert": Current.CurrentGameMode = GameMode.Expert; break;
												default: Current.CurrentGameMode = GameMode.Normal; break;
										} break;
								} break;
							case "controls":
								switch (Key) {
									case "usejoysticks":
										Current.UseJoysticks = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
										break;
									case "joystickaxisthreshold":
										{
											double a;
											double.TryParse(Value, NumberStyles.Float, Culture, out a);
											Current.JoystickAxisThreshold = a;
										} break;
									case "keyrepeatdelay":
										{
											int a;
											int.TryParse(Value, NumberStyles.Integer, Culture, out a);
											if (a <= 0) a = 500;
											Current.KeyRepeatDelay = 0.001 * (double)a;
										} break;
									case "keyrepeatinterval":
										{
											int a;
											int.TryParse(Value, NumberStyles.Integer, Culture, out a);
											if (a <= 0) a = 100;
											Current.KeyRepeatInterval = 0.001 * (double)a;
										} break;
								} break;
							case "sound":
								switch (Key) {
									case "model":
										switch (Value.ToLowerInvariant()) {
											case "linear": Current.SoundModel = Sounds.SoundModels.Linear; break;
											default: Current.SoundModel = Sounds.SoundModels.Inverse; break;
										}
										break;
									case "range":
										switch (Value.ToLowerInvariant()) {
												case "low": Current.SoundRange = Sounds.SoundRange.Low; break;
												case "medium": Current.SoundRange = Sounds.SoundRange.Medium; break;
												case "high": Current.SoundRange = Sounds.SoundRange.High; break;
												default: Current.SoundRange = Sounds.SoundRange.Low; break;
										}
										break;
									case "number":
										{
											int a;
											int.TryParse(Value, NumberStyles.Integer, Culture, out a);
											Current.SoundNumber = a < 16 ? 16 : a;
										} break;
								} break;
							case "verbosity":
								switch (Key) {
									case "showwarningmessages":
										Current.ShowWarningMessages = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
										break;
									case "showerrormessages":
										Current.ShowErrorMessages = string.Compare(Value, "false", StringComparison.OrdinalIgnoreCase) != 0;
										break;
								} break;
							case "folders":
								switch (Key) {
									case "route":
										Current.RouteFolder = Value;
										break;
									case "train":
										Current.TrainFolder = Value;
										break;
								} break;
							case "proxy":
								switch (Key) {
									case "url":
										Current.ProxyUrl = Value;
										break;
									case "username":
										Current.ProxyUserName = Value;
										break;
									case "password":
										Current.ProxyPassword = Value;
										break;
								} break;
							case "recentlyusedroutes":
								{
									int n = Current.RecentlyUsedRoutes.Length;
									Array.Resize<string>(ref Current.RecentlyUsedRoutes, n + 1);
									Current.RecentlyUsedRoutes[n] = Value;
								} break;
							case "recentlyusedtrains":
								{
									int n = Current.RecentlyUsedTrains.Length;
									Array.Resize<string>(ref Current.RecentlyUsedTrains, n + 1);
									Current.RecentlyUsedTrains[n] = Value;
								} break;
							case "routeencodings":
								{
									int a = System.Text.Encoding.UTF8.CodePage;
									int.TryParse(Key, NumberStyles.Integer, Culture, out a);
									int n = Current.RouteEncodings.Length;
									Array.Resize<EncodingValue>(ref Current.RouteEncodings, n + 1);
									Current.RouteEncodings[n].Codepage = a;
									Current.RouteEncodings[n].Value = Value;
								} break;
							case "trainencodings":
								{
									int a = System.Text.Encoding.UTF8.CodePage;
									int.TryParse(Key, NumberStyles.Integer, Culture, out a);
									int n = Current.TrainEncodings.Length;
									Array.Resize<EncodingValue>(ref Current.TrainEncodings, n + 1);
									Current.TrainEncodings[n].Codepage = a;
									Current.TrainEncodings[n].Value = Value;
								} break;
						}
					}
				}
			}
		} else {
			// file not found
			string Code = CultureInfo.CurrentUICulture.Name;
			if (string.IsNullOrEmpty(Code))
				Code = "en-US";
			File = OpenBveApi.Path.CombineFile(Program.FileSystem.GetDataFolder("Languages"), Code + ".cfg");
			if (System.IO.File.Exists(File)) {
				Current.LanguageCode = Code;
			} else {
				try {
					int i = Code.IndexOf("-", StringComparison.Ordinal);
					if (i > 0) {
						Code = Code.Substring(0, i);
						File = OpenBveApi.Path.CombineFile(Program.FileSystem.GetDataFolder("Languages"), Code + ".cfg");
						if (System.IO.File.Exists(File)) {
							Current.LanguageCode = Code;
						}
					}
				} catch {
					Current.LanguageCode = "en-US";
				}
			}
		}
	}
	internal static void SaveOptions() {
		CultureInfo Culture = CultureInfo.InvariantCulture;
		System.Text.StringBuilder Builder = new System.Text.StringBuilder();
		Builder.AppendLine("; Options");
		Builder.AppendLine("; =======");
		Builder.AppendLine("; This file was automatically generated. Please modify only if you know what you're doing.");
		Builder.AppendLine();
		Builder.AppendLine("[language]");
		Builder.AppendLine("code = " + Current.LanguageCode);
		Builder.AppendLine();
		Builder.AppendLine("[interface]");
		Builder.AppendLine("folder = " + Current.UserInterfaceFolder);
		Builder.AppendLine();
		Builder.AppendLine("[display]");
		Builder.AppendLine("mode = " + (Current.FullscreenMode ? "fullscreen" : "window"));
		Builder.AppendLine("vsync = " + (Current.VerticalSynchronization ? "true" : "false"));
		Builder.AppendLine("windowWidth = " + Current.WindowWidth.ToString(Culture));
		Builder.AppendLine("windowHeight = " + Current.WindowHeight.ToString(Culture));
		Builder.AppendLine("fullscreenWidth = " + Current.FullscreenWidth.ToString(Culture));
		Builder.AppendLine("fullscreenHeight = " + Current.FullscreenHeight.ToString(Culture));
		Builder.AppendLine("mainmenuWidth = " + Current.MainMenuWidth.ToString(Culture));
		Builder.AppendLine("mainmenuHeight = " + Current.MainMenuHeight.ToString(Culture));
		Builder.AppendLine("disableDisplayLists = " + (Current.DisableDisplayLists ? "true" : "false"));
		Builder.AppendLine("loadInAdvance = " + (Current.LoadInAdvance ? "true" : "false"));
		Builder.AppendLine("noTextureResize = " + (Current.NoTextureResize ? "true" : "false"));
		Builder.AppendLine();
		Builder.AppendLine("[quality]");
		{
			string t; switch (Current.Interpolation) {
				case InterpolationMode.NearestNeighbor: t = "nearestNeighbor"; break;
				case InterpolationMode.Bilinear: t = "bilinear"; break;
				case InterpolationMode.NearestNeighborMipmapped: t = "nearestNeighborMipmapped"; break;
				case InterpolationMode.BilinearMipmapped: t = "bilinearMipmapped"; break;
				case InterpolationMode.TrilinearMipmapped: t = "trilinearMipmapped"; break;
				case InterpolationMode.AnisotropicFiltering: t = "anisotropicFiltering"; break;
				default: t = "bilinearMipmapped"; break;
			}
			Builder.AppendLine("interpolation = " + t);
		}
		Builder.AppendLine("anisotropicFilteringLevel = " + Current.AnisotropicFilteringLevel.ToString(Culture));
		Builder.AppendLine("anisotropicFilteringMaximum = " + Current.AnisotropicFilteringMaximum.ToString(Culture));
		Builder.AppendLine("antiAliasingLevel = " + Current.AntiAliasingLevel.ToString(Culture));
		Builder.AppendLine("transparencyMode = " + ((int)Current.TransparencyMode).ToString(Culture));
		Builder.AppendLine("viewingDistance = " + Current.ViewingDistance.ToString(Culture));
		{
			string t; switch (Current.MotionBlur) {
				case MotionBlurMode.Low: t = "low"; break;
				case MotionBlurMode.Medium: t = "medium"; break;
				case MotionBlurMode.High: t = "high"; break;
				default: t = "none"; break;
			}
			Builder.AppendLine("motionBlur = " + t);
		}
		Builder.AppendLine();
		Builder.AppendLine("[objectOptimization]");
		Builder.AppendLine("basicThreshold = " + Current.ObjectOptimizationBasicThreshold.ToString(Culture));
		Builder.AppendLine("fullThreshold = " + Current.ObjectOptimizationFullThreshold.ToString(Culture));
		Builder.AppendLine();
		Builder.AppendLine("[simulation]");
		Builder.AppendLine("toppling = " + (Current.Toppling ? "true" : "false"));
		Builder.AppendLine("collisions = " + (Current.Collisions ? "true" : "false"));
		Builder.AppendLine("derailments = " + (Current.Derailments ? "true" : "false"));
		Builder.AppendLine("blackbox = " + (Current.BlackBox ? "true" : "false"));
		Builder.Append("mode = ");
		switch (Current.CurrentGameMode) {
			case GameMode.Arcade: Builder.AppendLine("arcade"); break;
			case GameMode.Normal: Builder.AppendLine("normal"); break;
			case GameMode.Expert: Builder.AppendLine("expert"); break;
			default: Builder.AppendLine("normal"); break;
		}
		Builder.AppendLine();
		Builder.AppendLine("[verbosity]");
		Builder.AppendLine("showWarningMessages = " + (Current.ShowWarningMessages ? "true" : "false"));
		Builder.AppendLine("showErrorMessages = " + (Current.ShowErrorMessages ? "true" : "false"));
		Builder.AppendLine();
		Builder.AppendLine("[controls]");
		Builder.AppendLine("useJoysticks = " + (Current.UseJoysticks ? "true" : "false"));
		Builder.AppendLine("joystickAxisthreshold = " + Current.JoystickAxisThreshold.ToString(Culture));
		Builder.AppendLine("keyRepeatDelay = " + (1000.0 * Current.KeyRepeatDelay).ToString("0", Culture));
		Builder.AppendLine("keyRepeatInterval = " + (1000.0 * Current.KeyRepeatInterval).ToString("0", Culture));
		Builder.AppendLine();
		Builder.AppendLine("[sound]");
		Builder.Append("model = ");
		switch (Current.SoundModel) {
			case Sounds.SoundModels.Linear: Builder.AppendLine("linear"); break;
			default: Builder.AppendLine("inverse"); break;
		}
		Builder.Append("range = ");
		switch (Current.SoundRange) {
				case Sounds.SoundRange.Low: Builder.AppendLine("low"); break;
				case Sounds.SoundRange.Medium: Builder.AppendLine("medium"); break;
				case Sounds.SoundRange.High: Builder.AppendLine("high"); break;
			default: Builder.AppendLine("low"); break;
		}
		Builder.AppendLine("number = " + Current.SoundNumber.ToString(Culture));
		Builder.AppendLine();
		Builder.AppendLine("[proxy]");
		Builder.AppendLine("url = " + Current.ProxyUrl);
		Builder.AppendLine("username = " + Current.ProxyUserName);
		Builder.AppendLine("password = " + Current.ProxyPassword);
		Builder.AppendLine();
		Builder.AppendLine("[folders]");
		Builder.AppendLine("route = " + Current.RouteFolder);
		Builder.AppendLine("train = " + Current.TrainFolder);
		Builder.AppendLine();
		Builder.AppendLine("[recentlyUsedRoutes]");
		for (int i = 0; i < Current.RecentlyUsedRoutes.Length; i++) {
			Builder.AppendLine(Current.RecentlyUsedRoutes[i]);
		}
		Builder.AppendLine();
		Builder.AppendLine("[recentlyUsedTrains]");
		for (int i = 0; i < Current.RecentlyUsedTrains.Length; i++) {
			Builder.AppendLine(Current.RecentlyUsedTrains[i]);
		}
		Builder.AppendLine();
		Builder.AppendLine("[routeEncodings]");
		for (int i = 0; i < Current.RouteEncodings.Length; i++) {
			Builder.AppendLine(Current.RouteEncodings[i].Codepage.ToString(Culture) + " = " + Current.RouteEncodings[i].Value);
		}
		Builder.AppendLine();
		Builder.AppendLine("[trainEncodings]");
		for (int i = 0; i < Current.TrainEncodings.Length; i++) {
			Builder.AppendLine(Current.TrainEncodings[i].Codepage.ToString(Culture) + " = " + Current.TrainEncodings[i].Value);
		}
		string File = OpenBveApi.Path.CombineFile(Program.FileSystem.SettingsFolder, "options.cfg");
		System.IO.File.WriteAllText(File, Builder.ToString(), new System.Text.UTF8Encoding(true));
		}
	}
}