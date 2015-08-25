using System;
using System.Windows.Forms;
using System.Globalization;
using SDL2;
namespace OpenBve {
	/// <summary>Provides methods for starting the program, including the Main procedure.</summary>
	internal static partial class Program {

		// --- members ---
		/// <summary>The invariant culture.</summary>
		internal static readonly CultureInfo InvariantCulture = CultureInfo.InvariantCulture;
		/// <summary>Whether the program is currently running on Mono. This is of interest for the Windows Forms main menu which behaves differently on Mono than on Microsoft .NET.</summary>
		internal static bool CurrentlyRunningOnMono = false;

		/// <summary>Whether the program is currently running on Microsoft Windows or compatible. This is of interest for whether running Win32 plugins is possible.</summary>
		internal static bool CurrentlyRunningOnWindows = false;

		/// <summary>The host API used by this program.</summary>
		internal static Host CurrentHost = null;

		/// <summary>Information about the file system organization.</summary>
		internal static FileSystem FileSystem = null;
		
		/// <summary>The object that serves as an authentication for the SetPackageLookupDirectories call.</summary>
		private static object SetPackageLookupDirectoriesAuthentication = null;

		/// <summary>If the program is to be restarted, this contains the command-line arguments that should be passed to the process, or a null reference otherwise.</summary>
		internal static string RestartArguments = null;

		/// <summary>The random number generator used by this program.</summary>
		internal static Random RandomNumberGenerator = new Random();


		// --- functions ---
		
		/// <summary>Is executed when the program starts.</summary>
		/// <param name="args">The command-line arguments.</param>
		[STAThread]
		private static void Main(string[] args) {
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			// --- determine the running environment ---
			CurrentlyRunningOnMono = Type.GetType("Mono.Runtime") != null;
			CurrentlyRunningOnWindows = Environment.OSVersion.Platform == PlatformID.Win32S || Environment.OSVersion.Platform == PlatformID.Win32Windows || Environment.OSVersion.Platform == PlatformID.Win32NT;
			CurrentHost = new Host();
			try {
				FileSystem = FileSystem.FromCommandLineArgs(args);
				FileSystem.CreateFileSystem();
			} catch (Exception ex) {
				MessageBox.Show("The file system configuration could not be accessed or is invalid due to the following reason:\n\n" + ex.Message, "openBVE", MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			// --- set up packages ---
			SetPackageLookupDirectories();
			// --- load options and controls ---
			Options.LoadOptions();
			Controls.LoadControls(null, out Controls.CurrentControls);
			{
				string folder = Program.FileSystem.GetDataFolder("Controls");
				string file = OpenBveApi.Path.CombineFile(folder, "Default keyboard assignment.controls");
				Controls.Control[] controls;
				Controls.LoadControls(file, out controls);
				Controls.AddControls(ref Controls.CurrentControls, controls);
			}
			// --- load language ---
			{
				string folder = Program.FileSystem.GetDataFolder("Languages");
				string file = OpenBveApi.Path.CombineFile(folder, Options.Current.LanguageCode + ".cfg");
				if (!System.IO.File.Exists(file)) {
					file = OpenBveApi.Path.CombineFile(folder, "en-US.cfg");
				}
				Strings.LoadLanguage(file);
			}
			// --- check the command-line arguments for route and train ---
			formMain.MainDialogResult result = new formMain.MainDialogResult();
			for (int i = 0; i < args.Length; i++) {
				if (args[i].StartsWith("/route=", StringComparison.OrdinalIgnoreCase)) {
					result.RouteFile = args[i].Substring(7);
					result.RouteEncoding = System.Text.Encoding.UTF8;
					for (int j = 0; j < Options.Current.RouteEncodings.Length; j++) {
						if (string.Compare(Options.Current.RouteEncodings[j].Value, result.RouteFile, StringComparison.InvariantCultureIgnoreCase) == 0) {
							result.RouteEncoding = System.Text.Encoding.GetEncoding(Options.Current.RouteEncodings[j].Codepage);
							break;
						}
					}
				} else if (args[i].StartsWith("/train=", StringComparison.OrdinalIgnoreCase)) {
					result.TrainFolder = args[i].Substring(7);
					result.TrainEncoding = System.Text.Encoding.UTF8;
					for (int j = 0; j < Options.Current.TrainEncodings.Length; j++) {
						if (string.Compare(Options.Current.TrainEncodings[j].Value, result.TrainFolder, StringComparison.InvariantCultureIgnoreCase) == 0) {
							result.TrainEncoding = System.Text.Encoding.GetEncoding(Options.Current.TrainEncodings[j].Codepage);
							break;
						}
					}
				}
			}
			// --- check whether route and train exist ---
			if (result.RouteFile != null) {
				if (!System.IO.File.Exists(result.RouteFile)) {
					result.RouteFile = null;
				}
			}
			if (result.TrainFolder != null) {
				if (!System.IO.Directory.Exists(result.TrainFolder)) {
					result.TrainFolder = null;
				}
			}
			// --- if a route was provided but no train, try to use the route default ---
			if (result.RouteFile != null && result.TrainFolder == null) {
				bool isRW = string.Equals(System.IO.Path.GetExtension(result.RouteFile), ".rw", StringComparison.OrdinalIgnoreCase);
				CsvRwRouteParser.ParseRoute(result.RouteFile, isRW, result.RouteEncoding, null, null, null, true);
				if (Game.TrainName != null && Game.TrainName.Length != 0) {
					string folder = System.IO.Path.GetDirectoryName(result.RouteFile);
					while (true) {
						string trainFolder = OpenBveApi.Path.CombineDirectory(folder, "Train");
						if (System.IO.Directory.Exists(trainFolder)) {
							folder = OpenBveApi.Path.CombineDirectory(trainFolder, Game.TrainName);
							if (System.IO.Directory.Exists(folder)) {
								string file = OpenBveApi.Path.CombineFile(folder, "train.dat");
								if (System.IO.File.Exists(file)) {
									result.TrainFolder = folder;
									result.TrainEncoding = System.Text.Encoding.UTF8;
									for (int j = 0; j < Options.Current.TrainEncodings.Length; j++) {
										if (string.Compare(Options.Current.TrainEncodings[j].Value, result.TrainFolder, StringComparison.InvariantCultureIgnoreCase) == 0) {
											result.TrainEncoding = System.Text.Encoding.GetEncoding(Options.Current.TrainEncodings[j].Codepage);
											break;
										}
									}
								}
							} break;
						} else {
							System.IO.DirectoryInfo info = System.IO.Directory.GetParent(folder);
							if (info != null) {
								folder = info.FullName;
							} else {
								break;
							}
						}
					}
				}
				Game.Reset(false);
			}
			SDL.SDL_Init(0);
			// --- show the main menu if necessary ---
			if (result.RouteFile == null || result.TrainFolder == null) {
				// begin HACK //
				if (SDL.SDL_InitSubSystem(SDL.SDL_INIT_VIDEO) != 0)
					MessageBox.Show("Cannot initialize SDL!","openBVE");
				if (!Joysticks.Initialize()) {
					MessageBox.Show("SDL failed to initialize the joystick subsystem.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
					return;
				}
				// end HACK //
				result = formMain.ShowMainDialog(result);
			} else {
				result.Start = true;
			}
			// --- start the actual program ---
			if (result.Start) {
				if (Initialize()) {
					#if !DEBUG
					try {
						#endif
						MainLoop.StartLoopEx(result);
						#if !DEBUG
					} catch (Exception ex) {
						bool found = false;
						for (int i = 0; i < TrainManager.Trains.Length; i++) {
							if (TrainManager.Trains[i] != null && TrainManager.Trains[i].Plugin != null) {
								if (TrainManager.Trains[i].Plugin.LastException != null) {
									MessageBox.Show("The train plugin " + TrainManager.Trains[i].Plugin.PluginTitle + " caused a runtime exception: " + TrainManager.Trains[i].Plugin.LastException.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
									found = true;
									break;
								}
							}
						}
						if (!found) {
							MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
						}
					}
					#endif
				}
				Deinitialize();
			}
			// --- restart the program if necessary ---
			if (RestartArguments != null) {
				string arguments;
				if (FileSystem.RestartArguments.Length != 0 && RestartArguments.Length != 0) {
					arguments = FileSystem.RestartArguments + " " + RestartArguments;
				} else {
					arguments = FileSystem.RestartArguments + RestartArguments;
				}
				try {
					System.Diagnostics.Process.Start(FileSystem.RestartProcess, arguments);
				} catch (Exception ex) {
					MessageBox.Show(ex.Message + "\n\nProcess = " + FileSystem.RestartProcess + "\nArguments = " + arguments, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Error);
				}
			}
		}

		/// <summary>Initializes the program. A matching call to deinitialize must be made when the program is terminated.</summary>
		/// <returns>Whether the initialization was successful.</returns>
		private static bool Initialize() {
			if (!Plugins.LoadPlugins()) {
				return false;
			}
			if (!Screen.Initialize()) {
				MessageBox.Show("SDL failed to initialize the video subsystem.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			if (!Joysticks.Initialize()) {
				MessageBox.Show("SDL failed to initialize the joystick subsystem.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return false;
			}
			Sounds.Initialize();
			// begin HACK //
			const double degrees = 0.0174532925199433;
			World.VerticalViewingAngle = 45.0 * degrees;
			World.HorizontalViewingAngle = 2.0 * Math.Atan(Math.Tan(0.5 * World.VerticalViewingAngle) * World.AspectRatio);
			World.OriginalVerticalViewingAngle = World.VerticalViewingAngle;
			World.ExtraViewingDistance = 50.0;
			World.ForwardViewingDistance = (double)Options.Current.ViewingDistance;
			World.BackwardViewingDistance = 0.0;
			World.BackgroundImageDistance = (double)Options.Current.ViewingDistance;
			// end HACK //
			ClearLogFile();
			return true;
		}

		/// <summary>Deinitializes the program.</summary>
		private static void Deinitialize() {
			Plugins.UnloadPlugins();
			Sounds.Deinitialize();
			Joysticks.Deinitialize();
			Screen.Deinitialize();
			SDL.SDL_Quit();
		}

		/// <summary>Provides the API with lookup directories for all installed packages.</summary>
		internal static void SetPackageLookupDirectories() {
			int size = 16;
			string[] names = new string[size];
			string[] directories = new string[size];
			int count = 0;
			foreach (string lookupDirectory in FileSystem.ManagedContentFolders) {
				string[] packageDirectories = System.IO.Directory.GetDirectories(lookupDirectory);
				foreach (string packageDirectory in packageDirectories) {
					string package = System.IO.Path.GetFileName(packageDirectory);
					if (count == size) {
						size <<= 1;
						Array.Resize<string>(ref names, size);
						Array.Resize<string>(ref directories, size);
					}
					names[count] = package;
					directories[count] = packageDirectory;
					count++;
				}
			}
			Array.Resize<string>(ref names, count);
			Array.Resize<string>(ref directories, count);
			SetPackageLookupDirectoriesAuthentication = OpenBveApi.Path.SetPackageLookupDirectories(names, directories, SetPackageLookupDirectoriesAuthentication);
		}

		/// <summary>Clears the log file.</summary>
		internal static void ClearLogFile() {
			try {
				string file = System.IO.Path.Combine(Program.FileSystem.SettingsFolder, "log.txt");
				System.IO.File.WriteAllText(file, string.Empty, new System.Text.UTF8Encoding(true));
			} catch { }
		}

		/// <summary>Appends the specified text to the log file.</summary>
		/// <param name="text">The text.</param>
		internal static void AppendToLogFile(string text) {
			try {
				string file = System.IO.Path.Combine(Program.FileSystem.SettingsFolder, "log.txt");
				System.IO.File.AppendAllText(file, text + "\n", new System.Text.UTF8Encoding(false));
			} catch { }
		}
	}
}
