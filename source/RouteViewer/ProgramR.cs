// ╔═════════════════════════════════════════════════════════════╗
// ║ Program.cs for the Route Viewer                             ║
// ╠═════════════════════════════════════════════════════════════╣
// ║ This file cannot be used in the openBVE main program.       ║
// ║ The file from the openBVE main program cannot be used here. ║
// ╚═════════════════════════════════════════════════════════════╝

using System;
using System.Globalization;
using System.Windows.Forms;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using SDL2;
using System.Drawing;

namespace OpenBve {
	internal static class Program {

		// system
		internal enum Platform { Windows, Linux, Mac }
		internal static Platform CurrentPlatform = Platform.Windows;
		internal static bool CurrentlyRunOnMono = false;
		internal static FileSystem FileSystem = null;
		internal enum ProgramType { OpenBve, ObjectViewer, RouteViewer, Other }
		internal const ProgramType CurrentProgramType = ProgramType.RouteViewer;

		// members
		private static bool Quit = false;
		private static int LastTicks = int.MaxValue;
		internal static bool CpuReducedMode = false;
		internal static bool CpuAutomaticMode = true;
		private static int ReducedModeEnteringTime = 0;
		private static string CurrentRoute = null;
		internal static bool CurrentlyLoading = false;
		internal static int CurrentStation = -1;
		internal static bool JumpToPositionEnabled = false;
		internal static string JumpToPositionValue = "";
		private static IntPtr iconSurface = IntPtr.Zero;
		private static Bitmap iconBmp = null;
		private static System.Drawing.Imaging.BitmapData iconData = null;
		internal static IntPtr GLContext = IntPtr.Zero;
		internal static IntPtr Window = IntPtr.Zero;
		private static System.Diagnostics.Stopwatch timer;
		
		// keys
		private static bool ShiftPressed = false;
		private static bool ControlPressed = false;
		private static bool AltPressed = false;

		// main
		[STAThread]
		internal static void Main(string[] args) {
			Interface.CurrentOptions.UseSound = true;
			Interface.CurrentOptions.ObjectOptimizationBasicThreshold = 1000;
			Interface.CurrentOptions.ObjectOptimizationFullThreshold = 250;
			// platform and mono
			int p = (int)Environment.OSVersion.Platform;
			if (p == 4 | p == 128) {
				// general Unix
				CurrentPlatform = Platform.Linux;
			} else if (p == 6) {
				// Mac
				CurrentPlatform = Platform.Mac;
			} else {
				// non-Unix
				CurrentPlatform = Platform.Windows;
			}
			CurrentlyRunOnMono = Type.GetType("Mono.Runtime") != null;
			// file system
			FileSystem = FileSystem.FromCommandLineArgs(args);
			FileSystem.CreateFileSystem();
			SetPackageLookupDirectories();
			// command line arguments
			bool[] SkipArgs = new bool[args.Length];
			if (args.Length != 0) {
				string File = System.IO.Path.Combine(Application.StartupPath, "ObjectViewer.exe");
				if (System.IO.File.Exists(File)) {
					int Skips = 0;
					System.Text.StringBuilder NewArgs = new System.Text.StringBuilder();
					for (int i = 0; i < args.Length; i++) {
						if (System.IO.File.Exists(args[i])) {
							if (System.IO.Path.GetExtension(args[i]).Equals(".csv", StringComparison.OrdinalIgnoreCase)) {
								string Text = System.IO.File.ReadAllText(args[i], System.Text.Encoding.UTF8);
								if (Text.Length == 0 || Text.IndexOf("CreateMeshBuilder", StringComparison.OrdinalIgnoreCase) >= 0) {
									if (NewArgs.Length != 0)
										NewArgs.Append(" ");
									NewArgs.Append("\"" + args[i] + "\"");
									SkipArgs[i] = true;
									Skips++;
								}
							}
						} else {
							SkipArgs[i] = true;
							Skips++;
						}
					}
					if (NewArgs.Length != 0) {
						System.Diagnostics.Process.Start(File, NewArgs.ToString());
					}
					if (Skips == args.Length)
						return;
				}
			}
			// application
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) != 0) {
				MessageBox.Show("SDL failed to initialize the video subsystem.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			// initialize sdl window
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_DOUBLEBUFFER, 1);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_DEPTH_SIZE, 16);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_RED_SIZE, 8);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_GREEN_SIZE, 8);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_BLUE_SIZE, 8);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_ALPHA_SIZE, 8);
			SDL.SDL_ShowCursor(1);

			// initialize camera
			ResetCamera();
			World.BackgroundImageDistance = 600.0;
			World.ForwardViewingDistance = 600.0;
			World.BackwardViewingDistance = 0.0;
			World.ExtraViewingDistance = 50.0;
			// create window
			Renderer.ScreenWidth = 960;
			Renderer.ScreenHeight = 600;
			//int Bits = 32;
			//IntPtr video = Sdl.SDL_SetVideoMode(Renderer.ScreenWidth, Renderer.ScreenHeight, Bits, Sdl.SDL_OPENGL | Sdl.SDL_DOUBLEBUF);
			Window = SDL.SDL_CreateWindow(Application.ProductName,
				SDL.SDL_WINDOWPOS_UNDEFINED, SDL.SDL_WINDOWPOS_UNDEFINED,
				Renderer.ScreenWidth, Renderer.ScreenHeight,
				SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
			if (Window == IntPtr.Zero) {
				MessageBox.Show("SDL failed to create the window.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			// icon
			string iconFile = OpenBveApi.Path.CombineFile(Program.FileSystem.GetDataFolder(), "icon.png");
			if (System.IO.File.Exists(iconFile)) {
				iconBmp = new Bitmap(iconFile); // load file
				iconData = iconBmp.LockBits(new Rectangle(0, 0, iconBmp.Width, iconBmp.Height),
					System.Drawing.Imaging.ImageLockMode.ReadOnly,
					System.Drawing.Imaging.PixelFormat.Format32bppArgb); // lock data
				iconSurface = SDL.SDL_CreateRGBSurfaceFrom(iconData.Scan0, iconBmp.Width, iconBmp.Height, 32, iconData.Stride,
					0x00FF0000, 0x0000FF00, 0x000000FF, 0xFF000000); // upload to sdl
				SDL.SDL_SetWindowIcon(Window, iconSurface); // use icon
			}
			GLContext = SDL.SDL_GL_CreateContext(Window);
			GraphicsContext tkContext = new GraphicsContext(new ContextHandle(GLContext),
				                             SDL.SDL_GL_GetProcAddress,
				                             SDLGetCurrentContext);
			// anisotropic filtering
			string[] extensions = GL.GetString(StringName.Extensions).Split(new []{ ' ' }, StringSplitOptions.RemoveEmptyEntries);
			Interface.CurrentOptions.AnisotropicFilteringMaximum = 0;
			for (int i = 0; i < extensions.Length; i++) {
				if (extensions[i] == "GL_EXT_texture_filter_anisotropic") {
					float n;
					GL.GetFloat((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out n);
					Interface.CurrentOptions.AnisotropicFilteringMaximum = (int)Math.Round((double)n);
					break;
				}
			}
			if (Interface.CurrentOptions.AnisotropicFilteringMaximum <= 0) {
				Interface.CurrentOptions.AnisotropicFilteringMaximum = 0;
				Interface.CurrentOptions.AnisotropicFilteringLevel = 0;
				Interface.CurrentOptions.Interpolation = TextureManager.InterpolationMode.AnisotropicFiltering;
			} else {
				Interface.CurrentOptions.AnisotropicFilteringLevel = Interface.CurrentOptions.AnisotropicFilteringMaximum;
				Interface.CurrentOptions.Interpolation = TextureManager.InterpolationMode.TrilinearMipmapped;
			}
			Interface.CurrentOptions.TransparencyMode = Renderer.TransparencyMode.Sharp;
			// module initialization
			Renderer.Initialize();
			Renderer.InitializeLighting();
			SoundManager.Initialize();
			GL.ClearColor(0.75f, 0.75f, 0.75f, 1.0f);
			SwapBuffers();
			Fonts.Initialize();
			UpdateViewport();
			// loop
			bool processCommandLineArgs = true;
			timer = new System.Diagnostics.Stopwatch();
			timer.Start();
			while (!Quit) {
				ProcessEvents();
				int a = (int)timer.ElapsedMilliseconds;
				double TimeElapsed = 0.001 * (double)(a - LastTicks);
				if (CpuReducedMode) {
					System.Threading.Thread.Sleep(250);
				} else {
					System.Threading.Thread.Sleep(1);
					if (ReducedModeEnteringTime == 0) {
						ReducedModeEnteringTime = a + 2500;
					}
					if (World.CameraAlignmentDirection.Position.X != 0.0 | World.CameraAlignmentDirection.Position.Y != 0.0 | World.CameraAlignmentDirection.Position.Z != 0.0 | World.CameraAlignmentDirection.Pitch != 0.0 | World.CameraAlignmentDirection.Yaw != 0.0 | World.CameraAlignmentDirection.Roll != 0.0 | World.CameraAlignmentDirection.TrackPosition != 0.0 | World.CameraAlignmentDirection.Zoom != 0.0) {
						ReducedModeEnteringTime = a + 2500;
					} else if (a > ReducedModeEnteringTime & CpuAutomaticMode) {
						ReducedModeEnteringTime = 0;
						CpuReducedMode = true;
					}
				}
				DateTime d = DateTime.Now;
				Game.SecondsSinceMidnight = (double)(3600 * d.Hour + 60 * d.Minute + d.Second) + 0.001 * (double)d.Millisecond;
				ObjectManager.UpdateAnimatedWorldObjects(TimeElapsed, false);
				World.UpdateAbsoluteCamera(TimeElapsed);
				ObjectManager.UpdateVisibility(World.CameraTrackFollower.TrackPosition + World.CameraCurrentAlignment.Position.Z);
				TextureManager.Update(TimeElapsed);
				SoundManager.Update(TimeElapsed);
				Renderer.RenderScene(TimeElapsed);
				SwapBuffers();
				LastTicks = a;
				// command line arguments
				if (processCommandLineArgs) {
					processCommandLineArgs = false;
					for (int i = 0; i < args.Length; i++) {
						if (!SkipArgs[i] && System.IO.File.Exists(args[i])) {
							CurrentlyLoading = true;
							Renderer.RenderScene(0.0);
							SwapBuffers();
							CurrentRoute = args[i];
							LoadRoute();
							CurrentlyLoading = false;
							UpdateCaption();
							break;
						}
					}
				}
			}
			// quit
			TextureManager.UnuseAllTextures();
			SoundManager.Deinitialize();
			if (iconSurface != IntPtr.Zero)
				SDL.SDL_FreeSurface(iconSurface); // free surface
			if (iconBmp != null && iconData != null) {
				iconBmp.UnlockBits(iconData); // free pixels
				iconBmp.Dispose();
			}
			SDL.SDL_GL_DeleteContext(GLContext);
			SDL.SDL_DestroyWindow(Window);
			SDL.SDL_Quit();
		}

		private static ContextHandle SDLGetCurrentContext ()
		{
			return new ContextHandle(SDL.SDL_GL_GetCurrentContext());
		}

		// reset camera
		private static void ResetCamera() {
			World.AbsoluteCameraPosition = new World.Vector3D(0.0, 2.5, -5.0);
			World.AbsoluteCameraDirection = new World.Vector3D(-World.AbsoluteCameraPosition.X, -World.AbsoluteCameraPosition.Y, -World.AbsoluteCameraPosition.Z);
			World.AbsoluteCameraSide = new World.Vector3D(-World.AbsoluteCameraPosition.Z, 0.0, World.AbsoluteCameraPosition.X);
			World.Normalize(ref World.AbsoluteCameraDirection.X, ref World.AbsoluteCameraDirection.Y, ref World.AbsoluteCameraDirection.Z);
			World.Normalize(ref World.AbsoluteCameraSide.X, ref World.AbsoluteCameraSide.Y, ref World.AbsoluteCameraSide.Z);
			World.AbsoluteCameraUp = World.Cross(World.AbsoluteCameraDirection, World.AbsoluteCameraSide);
			World.VerticalViewingAngle = 45.0 * 0.0174532925199433;
			World.HorizontalViewingAngle = 2.0 * Math.Atan(Math.Tan(0.5 * World.VerticalViewingAngle) * World.AspectRatio);
			World.OriginalVerticalViewingAngle = World.VerticalViewingAngle;
		}

		// update viewport
		internal static void UpdateViewport() {
			GL.Viewport(0, 0, Renderer.ScreenWidth, Renderer.ScreenHeight);
			World.AspectRatio = (double)Renderer.ScreenWidth / (double)Renderer.ScreenHeight;
			World.HorizontalViewingAngle = 2.0 * Math.Atan(Math.Tan(0.5 * World.VerticalViewingAngle) * World.AspectRatio);
			GL.MatrixMode(MatrixMode.Projection);
			//const double invdeg = 57.295779513082320877;
			var mat = Matrix4d.CreatePerspectiveFieldOfView(World.VerticalViewingAngle,World.AspectRatio,0.2,1000.0);
			GL.LoadMatrix(ref mat);
			GL.Scale(-1,1,1);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();
		}

		// load route
		private static bool LoadRoute() {
			CurrentStation = -1;
			Game.Reset();
			Renderer.Initialize();
			Fonts.Initialize();
			UpdateViewport();
			bool result;
			try {
				Loading.Load(CurrentRoute, System.Text.Encoding.UTF8);
				result = true;
			} catch (Exception ex) {
				MessageBox.Show(ex.Message, Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				Game.Reset();
				CurrentRoute = null;
				result = false;
			}
			Renderer.InitializeLighting();
			ObjectManager.InitializeVisibility();
			return result;
		}

		// jump to station
		private static void JumpToStation(int Direction) {
			if (Direction < 0) {
				for (int i = Game.Stations.Length - 1; i >= 0; i--) {
					if (Game.Stations[i].Stops.Length != 0) {
						double p = Game.Stations[i].Stops[Game.Stations[i].Stops.Length - 1].TrackPosition;
						if (p < World.CameraTrackFollower.TrackPosition - 0.1) {
							TrackManager.UpdateTrackFollower(ref World.CameraTrackFollower, p, true, false);
							World.CameraCurrentAlignment.TrackPosition = p;
							CurrentStation = i;
							break;
						}
					}
				}
			} else if (Direction > 0) {
				for (int i = 0; i < Game.Stations.Length; i++) {
					if (Game.Stations[i].Stops.Length != 0) {
						double p = Game.Stations[i].Stops[Game.Stations[i].Stops.Length - 1].TrackPosition;
						if (p > World.CameraTrackFollower.TrackPosition + 0.1) {
							TrackManager.UpdateTrackFollower(ref World.CameraTrackFollower, p, true, false);
							World.CameraCurrentAlignment.TrackPosition = p;
							CurrentStation = i;
							break;
						}
					}
				}
			}
		}
		private static bool Rotate = false;
		// process events
		private static void ProcessEvents() {
			SDL.SDL_Event Event;
			double speedModified = (ShiftPressed ? 2.0 : 1.0) * (ControlPressed ? 4.0 : 1.0) * (AltPressed ? 8.0 : 1.0);
			while (SDL.SDL_PollEvent(out Event) != 0) {
				switch (Event.type) {
						// quit
					case SDL.SDL_EventType.SDL_QUIT:
						Quit = true;
						return;
						// resize
					case SDL.SDL_EventType.SDL_WINDOWEVENT:
						if (Event.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED) {
							Renderer.ScreenWidth = Event.window.data1;
							Renderer.ScreenHeight = Event.window.data2;
							UpdateViewport();
						}
						break;
						// mouse
					case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN: // FIXME implement check whether any track is loaded!
						if (CurrentRoute != null) {
							switch (Event.button.button) {
								case (byte)SDL.SDL_BUTTON_LEFT:
									World.CameraAlignmentDirection.TrackPosition = World.CameraExteriorTopSpeed * speedModified;
									CpuReducedMode = false;
									break;
								case (byte)SDL.SDL_BUTTON_RIGHT:
									World.CameraAlignmentDirection.TrackPosition = -World.CameraExteriorTopSpeed * speedModified;
									CpuReducedMode = false;
									break;
								case (byte)SDL.SDL_BUTTON_MIDDLE:
									Rotate = true;
									SDL.SDL_SetRelativeMouseMode(SDL.SDL_bool.SDL_TRUE);
									CpuReducedMode = false;
									break;
							}
						}
						break;
					case SDL.SDL_EventType.SDL_MOUSEBUTTONUP: // FIXME implement check whether any track is loaded!
						if (CurrentRoute != null) {
							if (Event.button.button == SDL.SDL_BUTTON_LEFT || Event.button.button == SDL.SDL_BUTTON_RIGHT) {
								World.CameraAlignmentDirection.TrackPosition = 0.0;
							} else if (Event.button.button == SDL.SDL_BUTTON_MIDDLE) {
								SDL.SDL_SetRelativeMouseMode(SDL.SDL_bool.SDL_FALSE);
								World.CameraAlignmentDirection.Pitch = 0.0;
								World.CameraAlignmentDirection.Yaw = 0.0;
								Rotate = false;
							}
						}
						break;
					case SDL.SDL_EventType.SDL_MOUSEMOTION: // TODO - rotate
						if (Rotate && CurrentStation != -1) {/*
							World.CameraAlignmentDirection.Pitch = speedModified * -World.CameraExteriorTopAngularSpeed * Event.motion.yrel/3;
							World.CameraAlignmentDirection.Yaw   = speedModified *  World.CameraExteriorTopAngularSpeed * Event.motion.xrel/3;*/
						}
						break;
						// key down
					case SDL.SDL_EventType.SDL_KEYDOWN:
						switch (Event.key.keysym.sym) {
							case SDL.SDL_Keycode.SDLK_LSHIFT:
							case SDL.SDL_Keycode.SDLK_RSHIFT:
								ShiftPressed = true;
								break;
							case SDL.SDL_Keycode.SDLK_LCTRL:
							case SDL.SDL_Keycode.SDLK_RCTRL:
								ControlPressed = true;
								break;
							case SDL.SDL_Keycode.SDLK_LALT:
							case SDL.SDL_Keycode.SDLK_RALT:
								AltPressed = true;
								break;
							case SDL.SDL_Keycode.SDLK_F5:
								if (CurrentRoute != null) {
									CurrentlyLoading = true;
									Renderer.RenderScene(0.0);
									SwapBuffers();
									World.CameraAlignment a = World.CameraCurrentAlignment;
									if (LoadRoute()) {
										World.CameraCurrentAlignment = a;
										TrackManager.UpdateTrackFollower(ref World.CameraTrackFollower, -1.0, true, false);
										TrackManager.UpdateTrackFollower(ref World.CameraTrackFollower, a.TrackPosition, true, false);
										World.CameraAlignmentDirection = new World.CameraAlignment();
										World.CameraAlignmentSpeed = new World.CameraAlignment();
										ObjectManager.UpdateVisibility(a.TrackPosition, true);
										ObjectManager.UpdateAnimatedWorldObjects(0.0, true);
									}
									CurrentlyLoading = false;
								}
								break;
							case SDL.SDL_Keycode.SDLK_F7:
								{
									OpenFileDialog Dialog = new OpenFileDialog();
									Dialog.CheckFileExists = true;
									Dialog.Filter = "CSV/RW files|*.csv;*.rw|All files|*";
									if (Dialog.ShowDialog() == DialogResult.OK) {
										CurrentlyLoading = true;
										Renderer.RenderScene(0.0);
										SwapBuffers();
										CurrentRoute = Dialog.FileName;
										LoadRoute();
										ObjectManager.UpdateAnimatedWorldObjects(0.0, true);
										CurrentlyLoading = false;
										UpdateCaption();
									}
								}
								break;
							case SDL.SDL_Keycode.SDLK_F9:
								if (Interface.MessageCount != 0) {
									formMessages.ShowMessages();
								}
								break;
							case SDL.SDL_Keycode.SDLK_a:
							case SDL.SDL_Keycode.SDLK_KP_4:
								World.CameraAlignmentDirection.Position.X = -World.CameraExteriorTopSpeed * speedModified;
								CpuReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_d:
							case SDL.SDL_Keycode.SDLK_KP_6:
								World.CameraAlignmentDirection.Position.X = World.CameraExteriorTopSpeed * speedModified;
								CpuReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_KP_2:
								World.CameraAlignmentDirection.Position.Y = -World.CameraExteriorTopSpeed * speedModified;
								CpuReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_KP_8:
								World.CameraAlignmentDirection.Position.Y = World.CameraExteriorTopSpeed * speedModified;
								CpuReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_w:
							case SDL.SDL_Keycode.SDLK_KP_9:
								if (CurrentRoute != null) {
									World.CameraAlignmentDirection.TrackPosition = World.CameraExteriorTopSpeed * speedModified;
									CpuReducedMode = false;
								}
								break;
							case SDL.SDL_Keycode.SDLK_s:
							case SDL.SDL_Keycode.SDLK_KP_3:
								if (CurrentRoute != null) {
									World.CameraAlignmentDirection.TrackPosition = -World.CameraExteriorTopSpeed * speedModified;
									CpuReducedMode = false;
								}
								break;
							case SDL.SDL_Keycode.SDLK_LEFT:
								World.CameraAlignmentDirection.Yaw = -World.CameraExteriorTopAngularSpeed * speedModified;
								CpuReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_RIGHT:
								World.CameraAlignmentDirection.Yaw = World.CameraExteriorTopAngularSpeed * speedModified;
								CpuReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_UP:
								World.CameraAlignmentDirection.Pitch = World.CameraExteriorTopAngularSpeed * speedModified;
								CpuReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_DOWN:
								World.CameraAlignmentDirection.Pitch = -World.CameraExteriorTopAngularSpeed * speedModified;
								CpuReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_KP_DIVIDE:
								World.CameraAlignmentDirection.Roll = -World.CameraExteriorTopAngularSpeed * speedModified;
								CpuReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_KP_MULTIPLY:
								World.CameraAlignmentDirection.Roll = World.CameraExteriorTopAngularSpeed * speedModified;
								CpuReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_KP_0:
								World.CameraAlignmentDirection.Zoom = World.CameraZoomTopSpeed * speedModified;
								CpuReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_KP_PERIOD:
								World.CameraAlignmentDirection.Zoom = -World.CameraZoomTopSpeed * speedModified;
								CpuReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_KP_1:
								Game.ApplyPointOfInterest(-1, true);
								CpuReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_KP_7:
								Game.ApplyPointOfInterest(1, true);
								CpuReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_PAGEUP:
								JumpToStation(1);
								CpuReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_PAGEDOWN:
								JumpToStation(-1);
								CpuReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_KP_5:
								World.CameraCurrentAlignment.Yaw = 0.0;
								World.CameraCurrentAlignment.Pitch = 0.0;
								World.CameraCurrentAlignment.Roll = 0.0;
								World.CameraCurrentAlignment.Position = new World.Vector3D(0.0, 2.5, 0.0);
								World.CameraCurrentAlignment.Zoom = 0.0;
								World.CameraAlignmentDirection = new World.CameraAlignment();
								World.CameraAlignmentSpeed = new World.CameraAlignment();
								World.VerticalViewingAngle = World.OriginalVerticalViewingAngle;
								UpdateViewport();
								World.UpdateAbsoluteCamera(0.0);
								World.UpdateViewingDistances();
								CpuReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_f:
								Renderer.OptionWireframe = !Renderer.OptionWireframe;
								CpuReducedMode = false;
								if (Renderer.OptionWireframe) {
									GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
								} else {
									GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
								} break;
							case SDL.SDL_Keycode.SDLK_n:
								Renderer.OptionNormals = !Renderer.OptionNormals;
								CpuReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_e:
								Renderer.OptionEvents = !Renderer.OptionEvents;
								CpuReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_c:
								CpuAutomaticMode = !CpuAutomaticMode;
								CpuReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_i:
								Renderer.OptionInterface = !Renderer.OptionInterface;
								CpuReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_m:
								SoundManager.Mute = !SoundManager.Mute;
								break;
							case SDL.SDL_Keycode.SDLK_PLUS:
							case SDL.SDL_Keycode.SDLK_KP_PLUS:
								if (!JumpToPositionEnabled) {
									JumpToPositionEnabled = true;
									JumpToPositionValue = "+";
									CpuReducedMode = false;
								}
								break;
							case SDL.SDL_Keycode.SDLK_MINUS:
							case SDL.SDL_Keycode.SDLK_KP_MINUS:
								if (!JumpToPositionEnabled) {
									JumpToPositionEnabled = true;
									JumpToPositionValue = "-";
									CpuReducedMode = false;
								}
								break;
							case SDL.SDL_Keycode.SDLK_0:
							case SDL.SDL_Keycode.SDLK_1:
							case SDL.SDL_Keycode.SDLK_2:
							case SDL.SDL_Keycode.SDLK_3:
							case SDL.SDL_Keycode.SDLK_4:
							case SDL.SDL_Keycode.SDLK_5:
							case SDL.SDL_Keycode.SDLK_6:
							case SDL.SDL_Keycode.SDLK_7:
							case SDL.SDL_Keycode.SDLK_8:
							case SDL.SDL_Keycode.SDLK_9:
								if (!JumpToPositionEnabled) {
									JumpToPositionEnabled = true;
									JumpToPositionValue = string.Empty;
								}
								JumpToPositionValue += char.ConvertFromUtf32(48 + Event.key.keysym.sym - SDL.SDL_Keycode.SDLK_0);
								CpuReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_PERIOD:
								if (!JumpToPositionEnabled) {
									JumpToPositionEnabled = true;
									JumpToPositionValue = "0.";
								} else if (JumpToPositionValue.IndexOf('.') == -1) {
									JumpToPositionValue += ".";
								}
								CpuReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_BACKSPACE:
								if (JumpToPositionEnabled && JumpToPositionValue.Length != 0) {
									JumpToPositionValue = JumpToPositionValue.Substring(0, JumpToPositionValue.Length - 1);
									CpuReducedMode = false;
								}
								break;
							case SDL.SDL_Keycode.SDLK_RETURN:
								if (JumpToPositionEnabled) {
									if (JumpToPositionValue.Length != 0) {
										int direction;
										if (JumpToPositionValue[0] == '-') {
											JumpToPositionValue = JumpToPositionValue.Substring(1);
											direction = -1;
										} else if (JumpToPositionValue[0] == '+') {
											JumpToPositionValue = JumpToPositionValue.Substring(1);
											direction = 1;
										} else {
											direction = 0;
										}
										double value;
										if (double.TryParse(JumpToPositionValue, NumberStyles.Float, CultureInfo.InvariantCulture, out value)) {
											if (direction != 0) {
												value = World.CameraTrackFollower.TrackPosition + (double)direction * value;
											}
											TrackManager.UpdateTrackFollower(ref World.CameraTrackFollower, value, true, false);
											World.CameraCurrentAlignment.TrackPosition = value;
											World.UpdateAbsoluteCamera(0.0);
											World.UpdateViewingDistances();
										}
									}
									JumpToPositionEnabled = false;
									CpuReducedMode = false;
								}
								break;
							case SDL.SDL_Keycode.SDLK_ESCAPE:
								JumpToPositionEnabled = false;
								CpuReducedMode = false;
								break;
						} break;
						// key up
					case SDL.SDL_EventType.SDL_KEYUP:
						switch (Event.key.keysym.sym) {
							case SDL.SDL_Keycode.SDLK_LSHIFT:
							case SDL.SDL_Keycode.SDLK_RSHIFT:
								ShiftPressed = false;
								break;
							case SDL.SDL_Keycode.SDLK_LCTRL:
							case SDL.SDL_Keycode.SDLK_RCTRL:
								ControlPressed = false;
								break;
							case SDL.SDL_Keycode.SDLK_LALT:
							case SDL.SDL_Keycode.SDLK_RALT:
								AltPressed = false;
								break;
							case SDL.SDL_Keycode.SDLK_a:
							case SDL.SDL_Keycode.SDLK_KP_4:
							case SDL.SDL_Keycode.SDLK_d:
							case SDL.SDL_Keycode.SDLK_KP_6:
								World.CameraAlignmentDirection.Position.X = 0.0;
								break;
							case SDL.SDL_Keycode.SDLK_KP_2:
							case SDL.SDL_Keycode.SDLK_KP_8:
								World.CameraAlignmentDirection.Position.Y = 0.0;
								break;
							case SDL.SDL_Keycode.SDLK_w:
							case SDL.SDL_Keycode.SDLK_KP_9:
							case SDL.SDL_Keycode.SDLK_s:
							case SDL.SDL_Keycode.SDLK_KP_3:
								World.CameraAlignmentDirection.TrackPosition = 0.0;
								break;
							case SDL.SDL_Keycode.SDLK_LEFT:
							case SDL.SDL_Keycode.SDLK_RIGHT:
								World.CameraAlignmentDirection.Yaw = 0.0;
								break;
							case SDL.SDL_Keycode.SDLK_UP:
							case SDL.SDL_Keycode.SDLK_DOWN:
								World.CameraAlignmentDirection.Pitch = 0.0;
								break;
							case SDL.SDL_Keycode.SDLK_KP_DIVIDE:
							case SDL.SDL_Keycode.SDLK_KP_MULTIPLY:
								World.CameraAlignmentDirection.Roll = 0.0;
								break;
							case SDL.SDL_Keycode.SDLK_KP_0:
							case SDL.SDL_Keycode.SDLK_KP_PERIOD:
								World.CameraAlignmentDirection.Zoom = 0.0;
								break;
						} break;
				}
			}
		}

		// update caption
		private static void UpdateCaption() {
			string text = CurrentRoute != null ? System.IO.Path.GetFileName(CurrentRoute) + " - " + Application.ProductName :
				Application.ProductName;
			SDL.SDL_SetWindowTitle(Window,text);
		}
		
		/// <summary>The object that serves as an authentication for the SetPackageLookupDirectories call.</summary>
		private static object SetPackageLookupDirectoriesAuthentication = null;

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
		internal static void SwapBuffers(){
			SDL.SDL_GL_SwapWindow(Window);
		}
	}
}