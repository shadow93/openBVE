using System;
using System.Drawing;
using System.Windows.Forms;
using System.Text;
using SDL2;
namespace OpenBve {
	internal partial class formMain : Form {
		internal formMain() {
			InitializeComponent();
		}

		// show main dialog
		internal struct MainDialogResult {
			internal bool Start;
			internal string RouteFile;
			internal System.Text.Encoding RouteEncoding;
			internal string TrainFolder;
			internal System.Text.Encoding TrainEncoding;
		}
		internal static MainDialogResult ShowMainDialog(MainDialogResult initial) {
			formMain Dialog = new formMain();
			Dialog.Result = initial;
			Dialog.ShowDialog();
			MainDialogResult result = Dialog.Result;
			Dialog.Dispose();
			return result;
		}

		// members
		private MainDialogResult Result;
		private int[] EncodingCodepages = new int[0];
		private Image JoystickImage = null;
		private string[] LanguageFiles = new string[0];
		private string CurrentLanguageCode = "en-US";

		
		
		// ====
		// form
		// ====

		// load
		private void formMain_Load(object sender, EventArgs e) {
			this.MinimumSize = this.Size;
			if (Options.Current.MainMenuWidth == -1 && Options.Current.MainMenuHeight == -1) {
				this.WindowState = FormWindowState.Maximized;
			} else if (Options.Current.MainMenuWidth > 0 && Options.Current.MainMenuHeight > 0) {
				this.Size = new Size(Options.Current.MainMenuWidth, Options.Current.MainMenuHeight);
				this.CenterToScreen();
			}
			#pragma warning disable 0162 // Unreachable code
			if (Program.IsDevelopmentVersion) {
				labelVersion.Text = "v" + Application.ProductVersion + Program.VersionSuffix + " (development)";
				labelVersion.BackColor = Color.Firebrick;
				panelInfo.BackColor = Color.Firebrick;
				linkHomepage.BackColor = Color.Firebrick;
			} else {
				labelVersion.Text = "v" + Application.ProductVersion + Program.VersionSuffix;
			}
			#pragma warning restore 0162 // Unreachable code
			System.Globalization.CultureInfo Culture = System.Globalization.CultureInfo.InvariantCulture;
			// form icon
			try {
				string File = OpenBveApi.Path.CombineFile(Program.FileSystem.GetDataFolder(), "icon.ico");
				this.Icon = new Icon(File);
			} catch { }
			// use button-style radio buttons on non-Mono
			if (!Program.CurrentlyRunningOnMono) {
				radiobuttonStart.Appearance = Appearance.Button;
				radiobuttonStart.AutoSize = false;
				radiobuttonStart.Size = new Size(buttonClose.Width, buttonClose.Height);
				radiobuttonStart.TextAlign = ContentAlignment.MiddleCenter;
				radiobuttonReview.Appearance = Appearance.Button;
				radiobuttonReview.AutoSize = false;
				radiobuttonReview.Size = new Size(buttonClose.Width, buttonClose.Height);
				radiobuttonReview.TextAlign = ContentAlignment.MiddleCenter;
				radiobuttonControls.Appearance = Appearance.Button;
				radiobuttonControls.AutoSize = false;
				radiobuttonControls.Size = new Size(buttonClose.Width, buttonClose.Height);
				radiobuttonControls.TextAlign = ContentAlignment.MiddleCenter;
				radiobuttonOptions.Appearance = Appearance.Button;
				radiobuttonOptions.AutoSize = false;
				radiobuttonOptions.Size = new Size(buttonClose.Width, buttonClose.Height);
				radiobuttonOptions.TextAlign = ContentAlignment.MiddleCenter;
				radiobuttonGetAddOns.Appearance = Appearance.Button;
				radiobuttonGetAddOns.AutoSize = false;
				radiobuttonGetAddOns.Size = new Size(buttonClose.Width, buttonClose.Height);
				radiobuttonGetAddOns.TextAlign = ContentAlignment.MiddleCenter;
			}
			// options
			BlackBox.LoadLogs();
			ListLanguages();
			{
				int Tab = 0;
				string[] Args = System.Environment.GetCommandLineArgs();
				for (int i = 1; i < Args.Length; i++) {
					switch (Args[i].ToLowerInvariant()) {
							case "/newgame": Tab = 0; break;
							case "/review": Tab = 1; break;
							case "/controls": Tab = 2; break;
							case "/options": Tab = 3; break;
					}
				}
				switch (Tab) {
						case 1: radiobuttonReview.Checked = true; break;
						case 2: radiobuttonControls.Checked = true; break;
						case 3: radiobuttonOptions.Checked = true; break;
						default: radiobuttonStart.Checked = true; break;
				}
			}
			// icons and images
			string MenuFolder = Program.FileSystem.GetDataFolder("Menu");
			Image ParentIcon = LoadImage(MenuFolder, "icon_parent.png");
			Image FolderIcon = LoadImage(MenuFolder, "icon_folder.png");
			Image RouteIcon = LoadImage(MenuFolder, "icon_route.png");
			Image TrainIcon = LoadImage(MenuFolder, "icon_train.png");
			Image LibraryIcon = LoadImage(MenuFolder, "icon_library.png");
			Image KeyboardIcon = LoadImage(MenuFolder, "icon_keyboard.png");
			Image MouseIcon = LoadImage(MenuFolder, "icon_mouse.png");
			Image JoystickIcon = LoadImage(MenuFolder, "icon_joystick.png");
			Image GamepadIcon = LoadImage(MenuFolder, "icon_gamepad.png");
			JoystickImage = LoadImage(MenuFolder, "joystick.png");
			Image Logo = LoadImage(MenuFolder, "logo.png");
			if (Logo != null) pictureboxLogo.Image = Logo;
			string flagsFolder = Program.FileSystem.GetDataFolder("Flags");
			string[] flags = System.IO.Directory.GetFiles(flagsFolder);
			// route selection
			listviewRouteFiles.SmallImageList = new ImageList();
			listviewRouteFiles.SmallImageList.TransparentColor = Color.White;
			if (ParentIcon != null) listviewRouteFiles.SmallImageList.Images.Add("parent", ParentIcon);
			if (FolderIcon != null) listviewRouteFiles.SmallImageList.Images.Add("folder", FolderIcon);
			if (RouteIcon != null) listviewRouteFiles.SmallImageList.Images.Add("route", RouteIcon);
			treeviewRouteAddOns.ImageList = new ImageList();
			if (FolderIcon != null) treeviewRouteAddOns.ImageList.Images.Add("folder", FolderIcon);
			if (RouteIcon != null) treeviewRouteAddOns.ImageList.Images.Add("route", RouteIcon);
			foreach (string flag in flags) {
				try {
					treeviewRouteAddOns.ImageList.Images.Add(System.IO.Path.GetFileNameWithoutExtension(flag), Image.FromFile(flag));
				} catch { }
			}
			listviewRouteFiles.Columns.Clear();
			listviewRouteFiles.Columns.Add("");
			listviewRouteRecently.Items.Clear();
			listviewRouteRecently.Columns.Add("");
			listviewRouteRecently.SmallImageList = new ImageList();
			listviewRouteRecently.SmallImageList.TransparentColor = Color.White;
			if (RouteIcon != null) listviewRouteRecently.SmallImageList.Images.Add("route", RouteIcon);
			for (int i = 0; i < Options.Current.RecentlyUsedRoutes.Length; i++) {
				ListViewItem Item = listviewRouteRecently.Items.Add(System.IO.Path.GetFileName(Options.Current.RecentlyUsedRoutes[i]));
				Item.ImageKey = "route";
				Item.Tag = Options.Current.RecentlyUsedRoutes[i];
			}
			listviewRouteRecently.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			// train selection
			listviewTrainFolders.SmallImageList = new ImageList();
			listviewTrainFolders.SmallImageList.TransparentColor = Color.White;
			if (ParentIcon != null) listviewTrainFolders.SmallImageList.Images.Add("parent", ParentIcon);
			if (FolderIcon != null) listviewTrainFolders.SmallImageList.Images.Add("folder", FolderIcon);
			if (TrainIcon != null) listviewTrainFolders.SmallImageList.Images.Add("train", TrainIcon);
			treeviewTrainAddOns.ImageList = new ImageList();
			if (FolderIcon != null) treeviewTrainAddOns.ImageList.Images.Add("folder", FolderIcon);
			if (RouteIcon != null) treeviewTrainAddOns.ImageList.Images.Add("train", TrainIcon);
			foreach (string flag in flags) {
				try {
					treeviewTrainAddOns.ImageList.Images.Add(System.IO.Path.GetFileNameWithoutExtension(flag), Image.FromFile(flag));
				} catch { }
			}
			listviewTrainFolders.Columns.Clear();
			listviewTrainFolders.Columns.Add("");
			listviewTrainRecently.Columns.Clear();
			listviewTrainRecently.Columns.Add("");
			listviewTrainRecently.SmallImageList = new ImageList();
			listviewTrainRecently.SmallImageList.TransparentColor = Color.White;
			if (TrainIcon != null) listviewTrainRecently.SmallImageList.Images.Add("train", TrainIcon);
			for (int i = 0; i < Options.Current.RecentlyUsedTrains.Length; i++) {
				ListViewItem Item = listviewTrainRecently.Items.Add(System.IO.Path.GetFileName(Options.Current.RecentlyUsedTrains[i]));
				Item.ImageKey = "train";
				Item.Tag = Options.Current.RecentlyUsedTrains[i];
			}
			listviewTrainRecently.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			// text boxes
			if (Options.Current.RouteFolder.Length != 0 && System.IO.Directory.Exists(Options.Current.RouteFolder)) {
				textboxRouteFolder.Text = Options.Current.RouteFolder;
			} else {
				textboxRouteFolder.Text = Program.FileSystem.InitialRouteFolder;
			}
			if (Options.Current.TrainFolder.Length != 0 && System.IO.Directory.Exists(Options.Current.TrainFolder)) {
				textboxTrainFolder.Text = Options.Current.TrainFolder;
			} else {
				textboxTrainFolder.Text = Program.FileSystem.InitialTrainFolder;
			}
			// encodings
			{
				System.Text.EncodingInfo[] Info = System.Text.Encoding.GetEncodings();
				EncodingCodepages = new int[Info.Length + 1];
				string[] EncodingDescriptions = new string[Info.Length + 1];
				EncodingCodepages[0] = System.Text.Encoding.UTF8.CodePage;
				EncodingDescriptions[0] = "(UTF-8)";
				for (int i = 0; i < Info.Length; i++) {
					EncodingCodepages[i + 1] = Info[i].CodePage;
					try { // MoMA says that DisplayName is flagged with [MonoTodo]
						EncodingDescriptions[i + 1] = Info[i].DisplayName + " - " + Info[i].CodePage.ToString(Culture);
					} catch {
						EncodingDescriptions[i + 1] = Info[i].Name;
					}
				}
				Array.Sort<string, int>(EncodingDescriptions, EncodingCodepages, 1, Info.Length);
				comboboxRouteEncoding.Items.Clear();
				comboboxTrainEncoding.Items.Clear();
				for (int i = 0; i < Info.Length + 1; i++) {
					comboboxRouteEncoding.Items.Add(EncodingDescriptions[i]);
					comboboxTrainEncoding.Items.Add(EncodingDescriptions[i]);
				}
			}
			// modes
			comboboxMode.Items.Clear();
			comboboxMode.Items.AddRange(new string[] { "", "", "" });
			comboboxMode.SelectedIndex = Options.Current.CurrentGameMode == Options.GameMode.Arcade ? 0 : Options.Current.CurrentGameMode == Options.GameMode.Expert ? 2 : 1;
			// review last game
			{
				if (Game.LogRouteName.Length == 0 | Game.LogTrainName.Length == 0) {
					radiobuttonReview.Enabled = false;
				} else {
					double ratio = Game.CurrentScore.Maximum == 0 ? 0.0 : (double)Game.CurrentScore.Value / (double)Game.CurrentScore.Maximum;
					if (ratio < 0.0) ratio = 0.0;
					if (ratio > 1.0) ratio = 1.0;
					int index = (int)Math.Floor(ratio * (double)BlackBox.RatingsCount);
					if (index >= BlackBox.RatingsCount) index = BlackBox.RatingsCount - 1;
					labelReviewRouteValue.Text = Game.LogRouteName;
					labelReviewTrainValue.Text = Game.LogTrainName;
					labelReviewDateValue.Text = Game.LogDateTime.ToString("yyyy-MM-dd", Culture);
					labelReviewTimeValue.Text = Game.LogDateTime.ToString("HH:mm:ss", Culture);
					switch (Options.Current.CurrentGameMode) {
							case Options.GameMode.Arcade: labelRatingModeValue.Text = Strings.GetInterfaceString("mode_arcade"); break;
							case Options.GameMode.Normal: labelRatingModeValue.Text = Strings.GetInterfaceString("mode_normal"); break;
							case Options.GameMode.Expert: labelRatingModeValue.Text = Strings.GetInterfaceString("mode_expert"); break;
							default: labelRatingModeValue.Text = Strings.GetInterfaceString("mode_unkown"); break;
					}
					if (Game.CurrentScore.Maximum == 0) {
						labelRatingColor.BackColor = Color.Gray;
						labelRatingDescription.Text = Strings.GetInterfaceString("rating_unkown");
					} else {
						Color[] Colors = new Color[] { Color.PaleVioletRed, Color.IndianRed, Color.Peru, Color.Goldenrod, Color.DarkKhaki, Color.YellowGreen, Color.MediumSeaGreen, Color.MediumAquamarine, Color.SkyBlue, Color.CornflowerBlue };
						if (index >= 0 & index < Colors.Length) {
							labelRatingColor.BackColor = Colors[index];
						} else {
							labelRatingColor.BackColor = Color.Gray;
						}
						labelRatingDescription.Text = Strings.GetInterfaceString("rating_" + index.ToString(Culture));
					}
					labelRatingAchievedValue.Text = Game.CurrentScore.Value.ToString(Culture);
					labelRatingMaximumValue.Text = Game.CurrentScore.Maximum.ToString(Culture);
					labelRatingRatioValue.Text = (100.0 * ratio).ToString("0.00", Culture) + "%";
				}
			}
			comboboxBlackBoxFormat.Items.Clear();
			comboboxBlackBoxFormat.Items.AddRange(new string[] { "", "" });
			comboboxBlackBoxFormat.SelectedIndex = 1;
			if (Game.BlackBoxEntryCount == 0) {
				labelBlackBox.Enabled = false;
				labelBlackBoxFormat.Enabled = false;
				comboboxBlackBoxFormat.Enabled = false;
				buttonBlackBoxExport.Enabled = false;
			}
			// controls
			listviewControls.SmallImageList = new ImageList();
			listviewControls.SmallImageList.TransparentColor = Color.White;
			if (KeyboardIcon != null) listviewControls.SmallImageList.Images.Add("keyboard", KeyboardIcon);
			if (MouseIcon != null) listviewControls.SmallImageList.Images.Add("mouse", MouseIcon);
			if (JoystickIcon != null) listviewControls.SmallImageList.Images.Add("joystick", JoystickIcon);
			if (GamepadIcon != null) listviewControls.SmallImageList.Images.Add("gamepad", GamepadIcon);
			// options
			if (Options.Current.FullscreenMode) {
				radiobuttonFullscreen.Checked = true;
			} else {
				radiobuttonWindow.Checked = true;
			}
			comboboxVSync.Items.Clear();
			comboboxVSync.Items.Add("");
			comboboxVSync.Items.Add("");
			comboboxVSync.SelectedIndex = Options.Current.VerticalSynchronization ? 1 : 0;
			updownWindowWidth.Value = (decimal)Options.Current.WindowWidth;
			updownWindowHeight.Value = (decimal)Options.Current.WindowHeight;
			updownFullscreenWidth.Value = (decimal)Options.Current.FullscreenWidth;
			updownFullscreenHeight.Value = (decimal)Options.Current.FullscreenHeight;
			comboboxInterpolation.Items.Clear();
			comboboxInterpolation.Items.AddRange(new string[] { "", "", "", "", "", "" });
			if ((int)Options.Current.Interpolation >= 0 & (int)Options.Current.Interpolation < comboboxInterpolation.Items.Count) {
				comboboxInterpolation.SelectedIndex = (int)Options.Current.Interpolation;
			} else {
				comboboxInterpolation.SelectedIndex = 3;
			}
			if (Options.Current.AnisotropicFilteringMaximum <= 0) {
				labelAnisotropic.Enabled = false;
				updownAnisotropic.Enabled = false;
				updownAnisotropic.Minimum = (decimal)0;
				updownAnisotropic.Maximum = (decimal)0;
			} else {
				updownAnisotropic.Minimum = (decimal)1;
				updownAnisotropic.Maximum = (decimal)Options.Current.AnisotropicFilteringMaximum;
				if ((decimal)Options.Current.AnisotropicFilteringLevel >= updownAnisotropic.Minimum & (decimal)Options.Current.AnisotropicFilteringLevel <= updownAnisotropic.Maximum) {
					updownAnisotropic.Value = (decimal)Options.Current.AnisotropicFilteringLevel;
				} else {
					updownAnisotropic.Value = updownAnisotropic.Minimum;
				}
			}
			updownAntiAliasing.Value = (decimal)Options.Current.AntiAliasingLevel;
			updownDistance.Value = (decimal)Options.Current.ViewingDistance;
			comboboxMotionBlur.Items.Clear();
			comboboxMotionBlur.Items.AddRange(new string[] { "", "", "", "" });
			comboboxMotionBlur.SelectedIndex = (int)Options.Current.MotionBlur;
			trackbarTransparency.Value = (int)Options.Current.TransparencyMode;
			checkboxToppling.Checked = Options.Current.Toppling;
			checkboxCollisions.Checked = Options.Current.Collisions;
			checkboxDerailments.Checked = Options.Current.Derailments;
			checkboxBlackBox.Checked = Options.Current.BlackBox;
			checkboxJoysticksUsed.Checked = Options.Current.UseJoysticks;
			{
				double a = (double)(trackbarJoystickAxisThreshold.Maximum - trackbarJoystickAxisThreshold.Minimum) * Options.Current.JoystickAxisThreshold + (double)trackbarJoystickAxisThreshold.Minimum;
				int b = (int)Math.Round(a);
				if (b < trackbarJoystickAxisThreshold.Minimum) b = trackbarJoystickAxisThreshold.Minimum;
				if (b > trackbarJoystickAxisThreshold.Maximum) b = trackbarJoystickAxisThreshold.Maximum;
				trackbarJoystickAxisThreshold.Value = b;
			}
			updownSoundNumber.Value = (decimal)Options.Current.SoundNumber;
			checkboxWarningMessages.Checked = Options.Current.ShowWarningMessages;
			checkboxErrorMessages.Checked = Options.Current.ShowErrorMessages;
			// language
			{
				string Folder = Program.FileSystem.GetDataFolder("Languages");
				int j;
				for (j = 0; j < LanguageFiles.Length; j++) {
					string File = OpenBveApi.Path.CombineFile(Folder, Options.Current.LanguageCode + ".cfg");
					if (string.Compare(File, LanguageFiles[j], StringComparison.OrdinalIgnoreCase) == 0) {
						comboboxLanguages.SelectedIndex = j;
						break;
					}
				}
				if (j == LanguageFiles.Length) {
					#if !DEBUG
					try {
						#endif
						string File = OpenBveApi.Path.CombineFile(Folder, "en-US.cfg");
						Strings.LoadLanguage(File);
						ApplyLanguage();
						#if !DEBUG
					} catch (Exception ex) {
						MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
					}
					#endif
				}
			}
			// lists
			ShowScoreLog(checkboxScorePenalties.Checked);
			// get add-ons
			checkboxFilterRoutes.Image = RouteIcon;
			checkboxFilterTrains.Image = TrainIcon;
			checkboxFilterLibraries.Image = LibraryIcon;
			checkboxFilterSharedLibraries.Image = LibraryIcon;
			treeviewPackages.ImageList = new ImageList();
			treeviewPackages.ImageList.Images.Add("route_notinstalled", LoadImage(MenuFolder, "icon_route_notinstalled.png"));
			treeviewPackages.ImageList.Images.Add("route_outdatedversion", LoadImage(MenuFolder, "icon_route_outdatedversion.png"));
			treeviewPackages.ImageList.Images.Add("route_latestversion", LoadImage(MenuFolder, "icon_route_latestversion.png"));
			treeviewPackages.ImageList.Images.Add("route_protected", LoadImage(MenuFolder, "icon_route_protected.png"));
			treeviewPackages.ImageList.Images.Add("train_notinstalled", LoadImage(MenuFolder, "icon_train_notinstalled.png"));
			treeviewPackages.ImageList.Images.Add("train_outdatedversion", LoadImage(MenuFolder, "icon_train_outdatedversion.png"));
			treeviewPackages.ImageList.Images.Add("train_latestversion", LoadImage(MenuFolder, "icon_train_latestversion.png"));
			treeviewPackages.ImageList.Images.Add("train_protected", LoadImage(MenuFolder, "icon_train_protected.png"));
			treeviewPackages.ImageList.Images.Add("library_notinstalled", LoadImage(MenuFolder, "icon_library_notinstalled.png"));
			treeviewPackages.ImageList.Images.Add("library_outdatedversion", LoadImage(MenuFolder, "icon_library_outdatedversion.png"));
			treeviewPackages.ImageList.Images.Add("library_latestversion", LoadImage(MenuFolder, "icon_library_latestversion.png"));
			treeviewPackages.ImageList.Images.Add("library_protected", LoadImage(MenuFolder, "icon_library_protected.png"));
			treeviewPackages.ImageList.Images.Add("folder", LoadImage(MenuFolder, "icon_folder.png"));
			foreach (string flag in flags) {
				try {
					treeviewPackages.ImageList.Images.Add(System.IO.Path.GetFileNameWithoutExtension(flag), Image.FromFile(flag));
				} catch { }
			}
			// result
			Result.Start = false;
//			Result.RouteFile = null;
//			Result.RouteEncoding = System.Text.Encoding.UTF8;
//			Result.TrainFolder = null;
//			Result.TrainEncoding = System.Text.Encoding.UTF8;
		}

		// apply language
		private void ApplyLanguage() {
			// panel
			radiobuttonStart.Text = Strings.GetInterfaceString("panel_start");
			radiobuttonReview.Text = Strings.GetInterfaceString("panel_review");
			radiobuttonGetAddOns.Text = Strings.GetInterfaceString("panel_getaddons");
			radiobuttonControls.Text = Strings.GetInterfaceString("panel_controls");
			radiobuttonOptions.Text = Strings.GetInterfaceString("panel_options");
			linkHomepage.Text = Strings.GetInterfaceString("panel_homepage");
			//linkUpdates.Text = Strings.GetInterfaceString("panel_updates");
			buttonClose.Text = Strings.GetInterfaceString("panel_close");
			// options
			labelOptionsTitle.Text = Strings.GetInterfaceString("options_title");
			groupboxDisplayMode.Text = Strings.GetInterfaceString("options_display_mode");
			radiobuttonWindow.Text = Strings.GetInterfaceString("options_display_mode_window");
			radiobuttonFullscreen.Text = Strings.GetInterfaceString("options_display_mode_fullscreen");
			labelVSync.Text = Strings.GetInterfaceString("options_display_vsync");
			comboboxVSync.Items[0] = Strings.GetInterfaceString("options_display_vsync_off");
			comboboxVSync.Items[1] = Strings.GetInterfaceString("options_display_vsync_on");
			groupboxWindow.Text = Strings.GetInterfaceString("options_display_window");
			labelWindowWidth.Text = Strings.GetInterfaceString("options_display_window_width");
			labelWindowHeight.Text = Strings.GetInterfaceString("options_display_window_height");
			groupboxFullscreen.Text = Strings.GetInterfaceString("options_display_fullscreen");
			labelFullscreenWidth.Text = Strings.GetInterfaceString("options_display_fullscreen_width");
			labelFullscreenHeight.Text = Strings.GetInterfaceString("options_display_fullscreen_height");
			groupboxInterpolation.Text = Strings.GetInterfaceString("options_quality_interpolation");
			labelInterpolation.Text = Strings.GetInterfaceString("options_quality_interpolation_mode");
			comboboxInterpolation.Items[0] = Strings.GetInterfaceString("options_quality_interpolation_mode_nearest");
			comboboxInterpolation.Items[1] = Strings.GetInterfaceString("options_quality_interpolation_mode_bilinear");
			comboboxInterpolation.Items[2] = Strings.GetInterfaceString("options_quality_interpolation_mode_nearestmipmap");
			comboboxInterpolation.Items[3] = Strings.GetInterfaceString("options_quality_interpolation_mode_bilinearmipmap");
			comboboxInterpolation.Items[4] = Strings.GetInterfaceString("options_quality_interpolation_mode_trilinearmipmap");
			comboboxInterpolation.Items[5] = Strings.GetInterfaceString("options_quality_interpolation_mode_anisotropic");
			labelAnisotropic.Text = Strings.GetInterfaceString("options_quality_interpolation_anisotropic_level");
			labelAntiAliasing.Text = Strings.GetInterfaceString("options_quality_interpolation_antialiasing_level");
			labelTransparency.Text = Strings.GetInterfaceString("options_quality_interpolation_transparency");
			labelTransparencyPerformance.Text = Strings.GetInterfaceString("options_quality_interpolation_transparency_sharp");
			labelTransparencyQuality.Text = Strings.GetInterfaceString("options_quality_interpolation_transparency_smooth");
			groupboxDistance.Text = Strings.GetInterfaceString("options_quality_distance");
			labelDistance.Text = Strings.GetInterfaceString("options_quality_distance_viewingdistance");
			labelDistanceUnit.Text = Strings.GetInterfaceString("options_quality_distance_viewingdistance_meters");
			labelMotionBlur.Text = "options_quality_distance_motionblur";
			comboboxMotionBlur.Items[0] = Strings.GetInterfaceString("options_quality_distance_motionblur_none");
			comboboxMotionBlur.Items[1] = Strings.GetInterfaceString("options_quality_distance_motionblur_low");
			comboboxMotionBlur.Items[2] = Strings.GetInterfaceString("options_quality_distance_motionblur_medium");
			comboboxMotionBlur.Items[3] = Strings.GetInterfaceString("options_quality_distance_motionblur_high");
			labelMotionBlur.Text = Strings.GetInterfaceString("options_quality_distance_motionblur");
			groupboxSimulation.Text = Strings.GetInterfaceString("options_misc_simulation");
			checkboxToppling.Text = Strings.GetInterfaceString("options_misc_simulation_toppling");
			checkboxCollisions.Text = Strings.GetInterfaceString("options_misc_simulation_collisions");
			checkboxDerailments.Text = Strings.GetInterfaceString("options_misc_simulation_derailments");
			checkboxBlackBox.Text = Strings.GetInterfaceString("options_misc_simulation_blackbox");
			groupboxControls.Text = Strings.GetInterfaceString("options_misc_controls");
			checkboxJoysticksUsed.Text = Strings.GetInterfaceString("options_misc_controls_joysticks");
			labelJoystickAxisThreshold.Text = Strings.GetInterfaceString("options_misc_controls_threshold");
			groupboxSound.Text = Strings.GetInterfaceString("options_misc_sound");
			labelSoundNumber.Text = Strings.GetInterfaceString("options_misc_sound_number");
			groupboxVerbosity.Text = Strings.GetInterfaceString("options_verbosity");
			checkboxWarningMessages.Text = Strings.GetInterfaceString("options_verbosity_warningmessages");
			checkboxErrorMessages.Text = Strings.GetInterfaceString("options_verbosity_errormessages");
			// start
			labelStartTitle.Text = Strings.GetInterfaceString("start_title");
			labelRoute.Text = " " + Strings.GetInterfaceString("start_route");
			groupboxRouteSelection.Text = Strings.GetInterfaceString("start_route_selection");
			tabpageRouteManaged.Text = Strings.GetInterfaceString("start_route_addons");
			tabpageRouteBrowse.Text = Strings.GetInterfaceString("start_route_browse");
			tabpageRouteRecently.Text = Strings.GetInterfaceString("start_route_recently");
			groupboxRouteDetails.Text = Strings.GetInterfaceString("start_route_details");
			tabpageRouteDescription.Text = Strings.GetInterfaceString("start_route_description");
			tabpageRouteMap.Text = Strings.GetInterfaceString("start_route_map");
			tabpageRouteGradient.Text = Strings.GetInterfaceString("start_route_gradient");
			tabpageRouteSettings.Text = Strings.GetInterfaceString("start_route_settings");
			labelRouteEncoding.Text = Strings.GetInterfaceString("start_route_settings_encoding");
			comboboxRouteEncoding.Items[0] = Strings.GetInterfaceString("(UTF-8)");
			labelRouteEncodingPreview.Text = Strings.GetInterfaceString("start_route_settings_encoding_preview");
			labelTrain.Text = " " + Strings.GetInterfaceString("start_train");
			groupboxTrainSelection.Text = Strings.GetInterfaceString("start_train_selection");
			tabpageTrainManaged.Text = Strings.GetInterfaceString("start_train_addons");
			tabpageTrainBrowse.Text = Strings.GetInterfaceString("start_train_browse");
			tabpageTrainRecently.Text = Strings.GetInterfaceString("start_train_recently");
			tabpageTrainDefault.Text = Strings.GetInterfaceString("start_train_default");
			checkboxTrainDefault.Text = Strings.GetInterfaceString("start_train_usedefault");
			groupboxTrainDetails.Text = Strings.GetInterfaceString("start_train_details");
			tabpageTrainDescription.Text = Strings.GetInterfaceString("start_train_description");
			tabpageTrainSettings.Text = Strings.GetInterfaceString("start_train_settings");
			labelTrainEncoding.Text = Strings.GetInterfaceString("start_train_settings_encoding");
			comboboxTrainEncoding.Items[0] = Strings.GetInterfaceString("(UTF-8)");
			labelTrainEncodingPreview.Text = Strings.GetInterfaceString("start_train_settings_encoding_preview");
			labelStart.Text = " " + Strings.GetInterfaceString("start_start");
			labelMode.Text = Strings.GetInterfaceString("start_start_mode");
			buttonStart.Text = Strings.GetInterfaceString("start_start_start");
			comboboxMode.Items[0] = Strings.GetInterfaceString("mode_arcade");
			comboboxMode.Items[1] = Strings.GetInterfaceString("mode_normal");
			comboboxMode.Items[2] = Strings.GetInterfaceString("mode_expert");
			// getaddons
			labelGetAddOnsTitle.Text = Strings.GetInterfaceString("getaddons_title");
			labelFilter.Text = Strings.GetInterfaceString("getaddons_filter");
			checkboxFilterRoutes.Text = Strings.GetInterfaceString("getaddons_filter_routes");
			checkboxFilterTrains.Text = Strings.GetInterfaceString("getaddons_filter_trains");
			checkboxFilterLibraries.Text = Strings.GetInterfaceString("getaddons_filter_libraries");
			checkboxFilterSharedLibraries.Text = Strings.GetInterfaceString("getaddons_filter_sharedlibraries");
			checkboxFilterNoWIPs.Text = Strings.GetInterfaceString("getaddons_filter_nowips");
			checkboxFilterUpdates.Text = Strings.GetInterfaceString("getaddons_filter_onlyupdates");
			groupboxPackage.Text = Strings.GetInterfaceString("getaddons_package");
			buttonPackageInstall.Text = Strings.GetInterfaceString("getaddons_package_install");
			buttonPackageRemove.Text = Strings.GetInterfaceString("getaddons_package_remove");
			buttonScreenshotPrevious.Text = Strings.GetInterfaceString("getaddons_screenshot_previous");
			buttonScreenshotNext.Text = Strings.GetInterfaceString("getaddons_screenshot_next");
			// review
			labelReviewTitle.Text = Strings.GetInterfaceString("review_title");
			labelConditions.Text = " " + Strings.GetInterfaceString("review_conditions");
			groupboxReviewRoute.Text = Strings.GetInterfaceString("review_conditions_route");
			labelReviewRouteCaption.Text = Strings.GetInterfaceString("review_conditions_route_file");
			groupboxReviewTrain.Text = Strings.GetInterfaceString("review_conditions_train");
			labelReviewTrainCaption.Text = Strings.GetInterfaceString("review_conditions_train_folder");
			groupboxReviewDateTime.Text = Strings.GetInterfaceString("review_conditions_datetime");
			labelReviewDateCaption.Text = Strings.GetInterfaceString("review_conditions_datetime_date");
			labelReviewTimeCaption.Text = Strings.GetInterfaceString("review_conditions_datetime_time");
			labelScore.Text = " " + Strings.GetInterfaceString("review_score");
			groupboxRating.Text = Strings.GetInterfaceString("review_score_rating");
			labelRatingModeCaption.Text = Strings.GetInterfaceString("review_score_rating_mode");
			switch (Options.Current.CurrentGameMode) {
					case Options.GameMode.Arcade: labelRatingModeValue.Text = Strings.GetInterfaceString("mode_arcade"); break;
					case Options.GameMode.Normal: labelRatingModeValue.Text = Strings.GetInterfaceString("mode_normal"); break;
					case Options.GameMode.Expert: labelRatingModeValue.Text = Strings.GetInterfaceString("mode_expert"); break;
					default: labelRatingModeValue.Text = Strings.GetInterfaceString("mode_unkown"); break;
			}
			{
					double ratio = Game.CurrentScore.Maximum == 0 ? 0.0 : (double)Game.CurrentScore.Value / (double)Game.CurrentScore.Maximum;
					if (ratio < 0.0) ratio = 0.0;
					if (ratio > 1.0) ratio = 1.0;
					int index = (int)Math.Floor(ratio * (double)BlackBox.RatingsCount);
					if (index >= BlackBox.RatingsCount) index = BlackBox.RatingsCount - 1;
					if (Game.CurrentScore.Maximum == 0) {
						labelRatingDescription.Text = Strings.GetInterfaceString("rating_unkown");
					} else {
						labelRatingDescription.Text = Strings.GetInterfaceString("rating_" + index.ToString(System.Globalization.CultureInfo.InvariantCulture));
					}
			}
			labelRatingAchievedCaption.Text = Strings.GetInterfaceString("review_score_rating_achieved");
			labelRatingMaximumCaption.Text = Strings.GetInterfaceString("review_score_rating_maximum");
			labelRatingRatioCaption.Text = Strings.GetInterfaceString("review_score_rating_ratio");
			groupboxScore.Text = Strings.GetInterfaceString("review_score_log");
			listviewScore.Columns[0].Text = Strings.GetInterfaceString("review_score_log_list_time");
			listviewScore.Columns[1].Text = Strings.GetInterfaceString("review_score_log_list_position");
			listviewScore.Columns[2].Text = Strings.GetInterfaceString("review_score_log_list_value");
			listviewScore.Columns[3].Text = Strings.GetInterfaceString("review_score_log_list_cumulative");
			listviewScore.Columns[4].Text = Strings.GetInterfaceString("review_score_log_list_reason");
			ShowScoreLog(checkboxScorePenalties.Checked);
			checkboxScorePenalties.Text = Strings.GetInterfaceString("review_score_log_penalties");
			buttonScoreExport.Text = Strings.GetInterfaceString("review_score_log_export");
			labelBlackBox.Text = " " + Strings.GetInterfaceString("review_blackbox");
			labelBlackBoxFormat.Text = Strings.GetInterfaceString("review_blackbox_format");
			comboboxBlackBoxFormat.Items[0] = Strings.GetInterfaceString("review_blackbox_format_csv");
			comboboxBlackBoxFormat.Items[1] = Strings.GetInterfaceString("review_blackbox_format_text");
			buttonBlackBoxExport.Text = Strings.GetInterfaceString("review_blackbox_export");
			// controls
			for (int i = 0; i < listviewControls.SelectedItems.Count; i++) {
				listviewControls.SelectedItems[i].Selected = false;
			}
			labelControlsTitle.Text = Strings.GetInterfaceString("controls_title");
			listviewControls.Columns[0].Text = Strings.GetInterfaceString("controls_list_command");
			listviewControls.Columns[1].Text = Strings.GetInterfaceString("controls_list_type");
			listviewControls.Columns[2].Text = Strings.GetInterfaceString("controls_list_description");
			listviewControls.Columns[3].Text = Strings.GetInterfaceString("controls_list_assignment");
			buttonControlAdd.Text = Strings.GetInterfaceString("controls_add");
			buttonControlRemove.Text = Strings.GetInterfaceString("controls_remove");
			buttonControlsImport.Text = Strings.GetInterfaceString("controls_import");
			buttonControlsExport.Text = Strings.GetInterfaceString("controls_export");
			buttonControlUp.Text = Strings.GetInterfaceString("controls_up");
			buttonControlDown.Text = Strings.GetInterfaceString("controls_down");
			groupboxControl.Text = Strings.GetInterfaceString("controls_selection");
			labelCommand.Text = Strings.GetInterfaceString("controls_selection_command");
			radiobuttonKeyboard.Text = Strings.GetInterfaceString("controls_selection_keyboard");
			labelKeyboardKey.Text = Strings.GetInterfaceString("controls_selection_keyboard_key");
			labelKeyboardModifier.Text = Strings.GetInterfaceString("controls_selection_keyboard_modifiers");
			checkboxKeyboardShift.Text = Strings.GetInterfaceString("controls_selection_keyboard_modifiers_shift");
			checkboxKeyboardCtrl.Text = Strings.GetInterfaceString("controls_selection_keyboard_modifiers_ctrl");
			checkboxKeyboardAlt.Text = Strings.GetInterfaceString("controls_selection_keyboard_modifiers_alt");
			radiobuttonJoystick.Text = Strings.GetInterfaceString("controls_selection_joystick");
			labelJoystickAssignmentCaption.Text = Strings.GetInterfaceString("controls_selection_joystick_assignment");
			textboxJoystickGrab.Text = Strings.GetInterfaceString("controls_selection_joystick_assignment_grab");
			groupboxJoysticks.Text = Strings.GetInterfaceString("controls_attached");
			{
				listviewControls.Items.Clear();
				comboboxCommand.Items.Clear();
				for (int i = 0; i < OpenBve.Controls.CommandInfos.Length; i++) {
					comboboxCommand.Items.Add(OpenBve.Controls.CommandInfos[i].Name + " - " + OpenBve.Controls.CommandInfos[i].Description);
				}
				comboboxKeyboardKey.Items.Clear();
				for (int i = 0; i < OpenBve.Controls.Keys.Length; i++) {
					string sdlname = SDL.SDL_GetKeyName(SDL.SDL_GetKeyFromScancode(OpenBve.Controls.Keys[i].Scancode));
					string bvename = OpenBve.Controls.Keys[i].Description;
					string description = bvename;
					if (Conversions.TrimInside(sdlname.ToLower()) != Conversions.TrimInside(bvename.ToLower()))
						description += " (" + sdlname + ")";
					comboboxKeyboardKey.Items.Add(description);
				}
				ListViewItem[] Items = new ListViewItem[OpenBve.Controls.CurrentControls.Length];
				for (int i = 0; i < OpenBve.Controls.CurrentControls.Length; i++) {
					Items[i] = new ListViewItem(new string[] { "", "", "", "" });
					UpdateControlListElement(Items[i], i, false);
				}
				listviewControls.Items.AddRange(Items);
				listviewControls.AutoResizeColumns(ColumnHeaderAutoResizeStyle.ColumnContent);
			}
		}

		// form closing
		private void formMain_FormClosing(object sender, FormClosingEventArgs e) {
			if (IsBusy()) {
				MessageBox.Show("The form cannot be closed because add-ons are currently being maintained.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Exclamation);
				e.Cancel = true;
				return;
			}
			Options.Current.LanguageCode = CurrentLanguageCode;
			Options.Current.FullscreenMode = radiobuttonFullscreen.Checked;
			Options.Current.VerticalSynchronization = comboboxVSync.SelectedIndex == 1;
			Options.Current.WindowWidth = (int)Math.Round(updownWindowWidth.Value);
			Options.Current.WindowHeight = (int)Math.Round(updownWindowHeight.Value);
			Options.Current.FullscreenWidth = (int)Math.Round(updownFullscreenWidth.Value);
			Options.Current.FullscreenHeight = (int)Math.Round(updownFullscreenHeight.Value);
			Options.Current.Interpolation = (Options.InterpolationMode)comboboxInterpolation.SelectedIndex;
			Options.Current.AnisotropicFilteringLevel = (int)Math.Round(updownAnisotropic.Value);
			Options.Current.AntiAliasingLevel = (int)Math.Round(updownAntiAliasing.Value);
			Options.Current.TransparencyMode = (Renderer.TransparencyMode)trackbarTransparency.Value;
			Options.Current.ViewingDistance = (int)Math.Round(updownDistance.Value);
			Options.Current.MotionBlur = (Options.MotionBlurMode)comboboxMotionBlur.SelectedIndex;
			Options.Current.Toppling = checkboxToppling.Checked;
			Options.Current.Collisions = checkboxCollisions.Checked;
			Options.Current.Derailments = checkboxDerailments.Checked;
			Options.Current.CurrentGameMode = (Options.GameMode)comboboxMode.SelectedIndex;
			Options.Current.BlackBox = checkboxBlackBox.Checked;
			Options.Current.UseJoysticks = checkboxJoysticksUsed.Checked;
			Options.Current.JoystickAxisThreshold = ((double)trackbarJoystickAxisThreshold.Value - (double)trackbarJoystickAxisThreshold.Minimum) / (double)(trackbarJoystickAxisThreshold.Maximum - trackbarJoystickAxisThreshold.Minimum);
			Options.Current.SoundNumber = (int)Math.Round(updownSoundNumber.Value);
			Options.Current.ShowWarningMessages = checkboxWarningMessages.Checked;
			Options.Current.ShowErrorMessages = checkboxErrorMessages.Checked;
			Options.Current.RouteFolder = textboxRouteFolder.Text;
			Options.Current.TrainFolder = textboxTrainFolder.Text;
			Options.Current.MainMenuWidth = this.WindowState == FormWindowState.Maximized ? -1 : this.Size.Width;
			Options.Current.MainMenuHeight = this.WindowState == FormWindowState.Maximized ? -1 : this.Size.Height;
			if (Result.Start) {
				// recently used routes
				if (Options.Current.RecentlyUsedLimit > 0) {
					int i; for (i = 0; i < Options.Current.RecentlyUsedRoutes.Length; i++) {
						if (string.Compare(Result.RouteFile, Options.Current.RecentlyUsedRoutes[i], StringComparison.OrdinalIgnoreCase) == 0) {
							break;
						}
					} if (i == Options.Current.RecentlyUsedRoutes.Length) {
						if (Options.Current.RecentlyUsedRoutes.Length < Options.Current.RecentlyUsedLimit) {
							Array.Resize<string>(ref Options.Current.RecentlyUsedRoutes, i + 1);
						} else {
							i--;
						}
					}
					for (int j = i; j > 0; j--) {
						Options.Current.RecentlyUsedRoutes[j] = Options.Current.RecentlyUsedRoutes[j - 1];
					}
					Options.Current.RecentlyUsedRoutes[0] = Result.RouteFile;
				}
				// recently used trains
				if (Options.Current.RecentlyUsedLimit > 0) {
					int i; for (i = 0; i < Options.Current.RecentlyUsedTrains.Length; i++) {
						if (string.Compare(Result.TrainFolder, Options.Current.RecentlyUsedTrains[i], StringComparison.OrdinalIgnoreCase) == 0) {
							break;
						}
					} if (i == Options.Current.RecentlyUsedTrains.Length) {
						if (Options.Current.RecentlyUsedTrains.Length < Options.Current.RecentlyUsedLimit) {
							Array.Resize<string>(ref Options.Current.RecentlyUsedTrains, i + 1);
						} else {
							i--;
						}
					}
					for (int j = i; j > 0; j--) {
						Options.Current.RecentlyUsedTrains[j] = Options.Current.RecentlyUsedTrains[j - 1];
					}
					Options.Current.RecentlyUsedTrains[0] = Result.TrainFolder;
				}
			}
			// remove non-existing recently used routes
			{
				int n = 0;
				string[] a = new string[Options.Current.RecentlyUsedRoutes.Length];
				for (int i = 0; i < Options.Current.RecentlyUsedRoutes.Length; i++) {
					if (System.IO.File.Exists(Options.Current.RecentlyUsedRoutes[i])) {
						a[n] = Options.Current.RecentlyUsedRoutes[i];
						n++;
					}
				}
				Array.Resize<string>(ref a, n);
				Options.Current.RecentlyUsedRoutes = a;
			}
			// remove non-existing recently used trains
			{
				int n = 0;
				string[] a = new string[Options.Current.RecentlyUsedTrains.Length];
				for (int i = 0; i < Options.Current.RecentlyUsedTrains.Length; i++) {
					if (System.IO.Directory.Exists(Options.Current.RecentlyUsedTrains[i])) {
						a[n] = Options.Current.RecentlyUsedTrains[i];
						n++;
					}
				}
				Array.Resize<string>(ref a, n);
				Options.Current.RecentlyUsedTrains = a;
			}
			// remove non-existing route encoding mappings
			{
				int n = 0;
				Options.EncodingValue[] a = new Options.EncodingValue[Options.Current.RouteEncodings.Length];
				for (int i = 0; i < Options.Current.RouteEncodings.Length; i++) {
					if (System.IO.File.Exists(Options.Current.RouteEncodings[i].Value)) {
						a[n] = Options.Current.RouteEncodings[i];
						n++;
					}
				}
				Array.Resize<Options.EncodingValue>(ref a, n);
				Options.Current.RouteEncodings = a;
			}
			// remove non-existing train encoding mappings
			{
				int n = 0;
				Options.EncodingValue[] a = new Options.EncodingValue[Options.Current.TrainEncodings.Length];
				for (int i = 0; i < Options.Current.TrainEncodings.Length; i++) {
					if (System.IO.Directory.Exists(Options.Current.TrainEncodings[i].Value)) {
						a[n] = Options.Current.TrainEncodings[i];
						n++;
					}
				}
				Array.Resize<Options.EncodingValue>(ref a, n);
				Options.Current.TrainEncodings = a;
			}
			// clear cache
			string directory = System.IO.Path.Combine(Program.FileSystem.SettingsFolder, "Cache");
			ClearCache(directory, NumberOfDaysScreenshotsAreCached);
			// finish
			#if !DEBUG
			try {
				#endif
				Options.SaveOptions();
				#if !DEBUG
			} catch (Exception ex) {
				MessageBox.Show(ex.Message, "Save options", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			#endif
			#if !DEBUG
			try {
				#endif
				OpenBve.Controls.SaveControls(null);
				#if !DEBUG
			} catch (Exception ex) {
				MessageBox.Show(ex.Message, "Save controls", MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
			#endif
		}

		// resize
		private void formMain_Resize(object sender, EventArgs e) {
			try {
				int wt = panelStart.Width;
				int ox = labelStart.Left;
				int wa = (wt - 3 * ox) / 2;
				int wb = (wt - 3 * ox) / 2;
				groupboxRouteSelection.Width = wa;
				groupboxRouteDetails.Left = 2 * ox + wa;
				groupboxRouteDetails.Width = wb;
				groupboxTrainSelection.Width = wa;
				groupboxTrainDetails.Left = 2 * ox + wa;
				groupboxTrainDetails.Width = wb;
				int oy = (labelRoute.Top - labelStartTitleBackground.Height) / 2;
				int ht = (labelStart.Top - labelRoute.Top - 4 * oy) / 2 - labelRoute.Height - oy;
				groupboxRouteSelection.Height = ht;
				groupboxRouteDetails.Height = ht;
				labelTrain.Top = groupboxRouteSelection.Top + groupboxRouteSelection.Height + 2 * oy;
				groupboxTrainSelection.Top = labelTrain.Top + labelTrain.Height + oy;
				groupboxTrainDetails.Top = labelTrain.Top + labelTrain.Height + oy;
				groupboxTrainSelection.Height = ht;
				groupboxTrainDetails.Height = ht;
				tabcontrolRouteSelection.Width = groupboxRouteSelection.Width - 2 * tabcontrolRouteSelection.Left;
				tabcontrolRouteSelection.Height = groupboxRouteSelection.Height - 3 * tabcontrolRouteSelection.Top / 2;
				tabcontrolRouteDetails.Width = groupboxRouteDetails.Width - 2 * tabcontrolRouteDetails.Left;
				tabcontrolRouteDetails.Height = groupboxRouteDetails.Height - 3 * tabcontrolRouteDetails.Top / 2;
				tabcontrolTrainSelection.Width = groupboxTrainSelection.Width - 2 * tabcontrolTrainSelection.Left;
				tabcontrolTrainSelection.Height = groupboxTrainSelection.Height - 3 * tabcontrolTrainSelection.Top / 2;
				tabcontrolTrainDetails.Width = groupboxTrainDetails.Width - 2 * tabcontrolTrainDetails.Left;
				tabcontrolTrainDetails.Height = groupboxTrainDetails.Height - 3 * tabcontrolTrainDetails.Top / 2;
			} catch { }
			try {
				int width = Math.Min((panelOptions.Width - 24) / 2, 420);
				panelOptionsLeft.Width = width;
				panelOptionsRight.Left = panelOptionsLeft.Left + width + 8;
				panelOptionsRight.Width = width;
			} catch { }
			try {
				int width = Math.Min((panelReview.Width - 32) / 3, 360);
				groupboxReviewRoute.Width = width;
				groupboxReviewTrain.Left = groupboxReviewRoute.Left + width + 8;
				groupboxReviewTrain.Width = width;
				groupboxReviewDateTime.Left = groupboxReviewTrain.Left + width + 8;
				groupboxReviewDateTime.Width = width;
			} catch { }
		}

		// shown
		private void formMain_Shown(object sender, EventArgs e) {
			if (radiobuttonStart.Checked) {
				listviewRouteFiles.Focus();
			} else if (radiobuttonReview.Checked) {
				listviewScore.Focus();
			} else if (radiobuttonControls.Checked) {
				listviewControls.Focus();
			} else if (radiobuttonOptions.Checked) {
				comboboxLanguages.Focus();
			}
			formMain_Resize(null, null);
			if (this.WindowState != FormWindowState.Maximized) {
				Size sss = this.ClientRectangle.Size;
				System.Windows.Forms.Screen s = System.Windows.Forms.Screen.FromControl(this);
				if ((double)this.Width >= 0.95 * (double)s.WorkingArea.Width | (double)this.Height >= 0.95 * (double)s.WorkingArea.Height) {
					this.WindowState = FormWindowState.Maximized;
				}
			}
			// add-ons
			TextboxTrainFilterTextChanged(null, null);
			if (treeviewTrainAddOns.Nodes.Count == 0) {
				tabcontrolTrainSelection.TabPages.RemoveAt(0);
			}
			TextboxRouteFilterTextChanged(null, null);
			if (treeviewRouteAddOns.Nodes.Count == 0) {
				tabcontrolRouteSelection.TabPages.RemoveAt(0);
			}
			radiobuttonStart.Focus();
			// command line arguments
			if (Result.TrainFolder != null) {
				if (checkboxTrainDefault.Checked) checkboxTrainDefault.Checked = false;
				ShowTrain(false);
			}
			if (Result.RouteFile != null) {
				ShowRoute(false);
			}
		}

		// list languages
		private void ListLanguages() {
			string Folder = Program.FileSystem.GetDataFolder("Languages");
			if (System.IO.Directory.Exists(Folder)) {
				string[] Files = System.IO.Directory.GetFiles(Folder);
				string[] LanguageNames = new string[Files.Length];
				LanguageFiles = new string[Files.Length];
				int n = 0;
				for (int i = 0; i < Files.Length; i++) {
					string Title = System.IO.Path.GetFileName(Files[i]);
					if (Title.EndsWith(".cfg", StringComparison.OrdinalIgnoreCase)) {
						string Code = Title.Substring(0, Title.Length - 4);
						string[] Lines = System.IO.File.ReadAllLines(Files[i], System.Text.Encoding.UTF8);
						string Section = "";
						string Name = Code;
						for (int j = 0; j < Lines.Length; j++) {
							Lines[j] = Lines[j].Trim();
							if (Lines[j].StartsWith("[", StringComparison.Ordinal) & Lines[j].EndsWith("]", StringComparison.Ordinal)) {
								Section = Lines[j].Substring(1, Lines[j].Length - 2).Trim().ToLowerInvariant();
							} else if (!Lines[j].StartsWith(";", StringComparison.OrdinalIgnoreCase)) {
								int k = Lines[j].IndexOf('=');
								if (k >= 0) {
									string Key = Lines[j].Substring(0, k).TrimEnd().ToLowerInvariant();
									string Value = Lines[j].Substring(k + 1).TrimStart();
									if (Section == "language" & Key == "name") {
										Name = Value;
										break;
									}
								}
							}
						}
						LanguageFiles[n] = Files[i];
						LanguageNames[n] = Name;
						n++;
					}
				}
				Array.Resize<string>(ref LanguageFiles, n);
				Array.Resize<string>(ref LanguageNames, n);
				Array.Sort<string, string>(LanguageNames, LanguageFiles);
				comboboxLanguages.Items.Clear();
				for (int i = 0; i < n; i++) {
					comboboxLanguages.Items.Add(LanguageNames[i]);
				}
			} else {
				LanguageFiles = new string[] { };
				comboboxLanguages.Items.Clear();
			}
		}

		
		
		// ========
		// top page
		// ========

		// page selection
		private void radiobuttonStart_CheckedChanged(object sender, EventArgs e) {
			panelStart.Visible = true;
			panelReview.Visible = false;
			panelControls.Visible = false;
			panelOptions.Visible = false;
			panelGetAddOns.Visible = false;
			panelPanels.BackColor = labelStartTitle.BackColor;
			pictureboxJoysticks.Visible = false;
			radiobuttonStart.BackColor = SystemColors.ButtonHighlight;
			radiobuttonReview.BackColor = SystemColors.ButtonFace;
			radiobuttonControls.BackColor = SystemColors.ButtonFace;
			radiobuttonOptions.BackColor = SystemColors.ButtonFace;
			radiobuttonGetAddOns.BackColor = SystemColors.ButtonFace;
			UpdateRadioButtonBackColor();
		}
		private void radiobuttonReview_CheckedChanged(object sender, EventArgs e) {
			panelReview.Visible = true;
			panelStart.Visible = false;
			panelControls.Visible = false;
			panelOptions.Visible = false;
			panelGetAddOns.Visible = false;
			panelPanels.BackColor = labelReviewTitle.BackColor;
			pictureboxJoysticks.Visible = false;
			radiobuttonStart.BackColor = SystemColors.ButtonFace;
			radiobuttonReview.BackColor = SystemColors.ButtonHighlight;
			radiobuttonControls.BackColor = SystemColors.ButtonFace;
			radiobuttonOptions.BackColor = SystemColors.ButtonFace;
			radiobuttonGetAddOns.BackColor = SystemColors.ButtonFace;
			UpdateRadioButtonBackColor();
		}
		private void radiobuttonControls_CheckedChanged(object sender, EventArgs e) {
			panelControls.Visible = true;
			panelStart.Visible = false;
			panelReview.Visible = false;
			panelOptions.Visible = false;
			panelGetAddOns.Visible = false;
			panelPanels.BackColor = labelControlsTitle.BackColor;
			pictureboxJoysticks.Visible = true;
			radiobuttonStart.BackColor = SystemColors.ButtonFace;
			radiobuttonReview.BackColor = SystemColors.ButtonFace;
			radiobuttonControls.BackColor = SystemColors.ButtonHighlight;
			radiobuttonOptions.BackColor = SystemColors.ButtonFace;
			radiobuttonGetAddOns.BackColor = SystemColors.ButtonFace;
			UpdateRadioButtonBackColor();
		}
		private void radiobuttonOptions_CheckedChanged(object sender, EventArgs e) {
			panelOptions.Visible = true;
			panelStart.Visible = false;
			panelReview.Visible = false;
			panelControls.Visible = false;
			panelGetAddOns.Visible = false;
			panelPanels.BackColor = labelOptionsTitle.BackColor;
			pictureboxJoysticks.Visible = false;
			radiobuttonStart.BackColor = SystemColors.ButtonFace;
			radiobuttonReview.BackColor = SystemColors.ButtonFace;
			radiobuttonControls.BackColor = SystemColors.ButtonFace;
			radiobuttonOptions.BackColor = SystemColors.ButtonHighlight;
			radiobuttonGetAddOns.BackColor = SystemColors.ButtonFace;
			UpdateRadioButtonBackColor();
		}
		private void RadiobuttonGetAddOnsCheckedChanged(object sender, EventArgs e) {
			panelGetAddOns.Visible = true;
			panelStart.Visible = false;
			panelReview.Visible = false;
			panelControls.Visible = false;
			panelOptions.Visible = false;
			panelPanels.BackColor = labelGetAddOnsTitle.BackColor;
			pictureboxJoysticks.Visible = false;
			radiobuttonStart.BackColor = SystemColors.ButtonFace;
			radiobuttonReview.BackColor = SystemColors.ButtonFace;
			radiobuttonControls.BackColor = SystemColors.ButtonFace;
			radiobuttonOptions.BackColor = SystemColors.ButtonFace;
			radiobuttonGetAddOns.BackColor = SystemColors.ButtonHighlight;
			UpdateRadioButtonBackColor();
			if (radiobuttonGetAddOns.Checked) {
				EnterGetAddOns();
			}
		}
		private void UpdateRadioButtonBackColor() {
			// work-around for button-style radio buttons on Mono
			if (Program.CurrentlyRunningOnMono) {
				radiobuttonStart.BackColor = panelPanels.BackColor;
				radiobuttonReview.BackColor = panelPanels.BackColor;
				radiobuttonControls.BackColor = panelPanels.BackColor;
				radiobuttonOptions.BackColor = panelPanels.BackColor;
				radiobuttonGetAddOns.BackColor = panelPanels.BackColor;
			}
		}

		// homepage
		private void linkHomepage_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e) {
			const string Url = "http://odakyufan.zxq.net/openbve/index.html";
			try {
				System.Diagnostics.Process.Start(Url);
			} catch (Exception ex) {
				MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}


		// close
		private void buttonClose_Click(object sender, EventArgs e) {
			this.Close();
		}


		// ======
		// events
		// ======

		// tick
		private void timerEvents_Tick(object sender, EventArgs e) {
			if (textboxJoystickGrab.Focused & this.Tag == null & listviewControls.SelectedIndices.Count == 1) {
				int j = listviewControls.SelectedIndices[0];
				SDL.SDL_JoystickUpdate();
				int k = 0;
				foreach (var joystick in Joysticks.AttachedJoysticks) {
					int axes = SDL.SDL_JoystickNumAxes(joystick.Handle);
					for (int i = 0; i < axes; i++) {
						double a = SDL.SDL_JoystickGetAxis(joystick.Handle, i);
						if (a < -0.75) {
							OpenBve.Controls.CurrentControls[j].Device = k;
							OpenBve.Controls.CurrentControls[j].Component = OpenBve.Controls.JoystickComponent.Axis;
							OpenBve.Controls.CurrentControls[j].Element = i;
							OpenBve.Controls.CurrentControls[j].Direction = -1;
							radiobuttonJoystick.Focus();
							UpdateJoystickDetails();
							UpdateControlListElement(listviewControls.Items[j], j, true);
							return;
						} if (a > 0.75) {
							OpenBve.Controls.CurrentControls[j].Device = k;
							OpenBve.Controls.CurrentControls[j].Component = OpenBve.Controls.JoystickComponent.Axis;
							OpenBve.Controls.CurrentControls[j].Element = i;
							OpenBve.Controls.CurrentControls[j].Direction = 1;
							radiobuttonJoystick.Focus();
							UpdateJoystickDetails();
							UpdateControlListElement(listviewControls.Items[j], j, true);
							return;
						}
					}
					int buttons = SDL.SDL_JoystickNumButtons(joystick.Handle);
					for (int i = 0; i < buttons; i++) {
						bool press = SDL.SDL_JoystickGetButton(joystick.Handle, i) == 1;
						if (press) {
							OpenBve.Controls.CurrentControls[j].Device = k;
							OpenBve.Controls.CurrentControls[j].Component = OpenBve.Controls.JoystickComponent.Button;
							OpenBve.Controls.CurrentControls[j].Element = i;
							OpenBve.Controls.CurrentControls[j].Direction = 1;
							radiobuttonJoystick.Focus();
							UpdateJoystickDetails();
							UpdateControlListElement(listviewControls.Items[j], j, true);
							return;
						}
					}
					int hats = SDL.SDL_JoystickNumHats(joystick.Handle);
					for (int i = 0; i < hats; i++) {
						int hat = SDL.SDL_JoystickGetHat(joystick.Handle,i);
						if (hat != SDL.SDL_HAT_CENTERED) {
							OpenBve.Controls.CurrentControls[j].Device = k;
							OpenBve.Controls.CurrentControls[j].Component = OpenBve.Controls.JoystickComponent.Hat;
							OpenBve.Controls.CurrentControls[j].Element = i;
							OpenBve.Controls.CurrentControls[j].Direction = hat;
							radiobuttonJoystick.Focus();
							UpdateJoystickDetails();
							UpdateControlListElement(listviewControls.Items[j], j, true);
							return;
						}
					}
				}
			}
			SDL.SDL_Event Event;
			while (SDL.SDL_PollEvent(out Event) != 0) { }
			pictureboxJoysticks.Invalidate();
		}

		
		
		// =========
		// functions
		// =========
		
		// load image
		private Image LoadImage(string Folder, string Title) {
			string File = OpenBveApi.Path.CombineFile(Folder, Title);
			if (System.IO.File.Exists(File)) {
				try {
					return Image.FromFile(File);
				} catch { }
			}
			return null;
		}

		// try load image
		private bool TryLoadImage(PictureBox Box, string Title) {
			string Folder = Program.FileSystem.GetDataFolder("Menu");
			string File = OpenBveApi.Path.CombineFile(Folder, Title);
			if (System.IO.File.Exists(File)) {
				try {
					Box.Image = Image.FromFile(File);
					return true;
				} catch {
					Box.Image = Box.ErrorImage;
					return false;
				}
			} else {
				Box.Image = Box.ErrorImage;
				return false;
			}
		}
		
	}
}