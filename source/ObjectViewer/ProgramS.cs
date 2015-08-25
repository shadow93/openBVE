// ╔═════════════════════════════════════════════════════════════╗
// ║ Program.cs for the Structure Viewer                         ║
// ╠═════════════════════════════════════════════════════════════╣
// ║ This file cannot be used in the openBVE main program.       ║
// ║ The file from the openBVE main program cannot be used here. ║
// ╚═════════════════════════════════════════════════════════════╝

using System;
using System.Windows.Forms;
using SDL2;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Stopwatch = System.Diagnostics.Stopwatch;
using System.Drawing;


namespace OpenBve {
	internal static class Program {

		// system
		internal enum Platform { Windows, Linux, Mac }
		internal static Platform CurrentPlatform = Platform.Windows;
		internal static bool CurrentlyRunOnMono = false;
		internal static FileSystem FileSystem = null;
		internal enum ProgramType { OpenBve, ObjectViewer, RouteViewer, Other }
		internal const ProgramType CurrentProgramType = ProgramType.ObjectViewer;
		internal static IntPtr SDLWindow{ get; private set; }
		internal static IntPtr GLContext{ get; private set; }

		// members
		private static bool Quit = false;
		private static string[] Files = new string[] { };
		private static long LastTicks = 0;
		private static bool ReducedMode = true;
		private static int ReducedModeEnteringTime = 0;
		private static int RotateX = 0;
		private static int RotateY = 0;
		private static double RotateXSpeed = 0.0;
		private static double RotateYSpeed = 0.0;
		private static int MoveX = 0;
		private static int MoveY = 0;
		private static int MoveZ = 0;
		private static double MoveXSpeed = 0.0;
		private static double MoveYSpeed = 0.0;
		private static double MoveZSpeed = 0.0;
		internal static int LightingTarget = 1;
		internal static double LightingRelative = 1.0;
		private static bool ShiftPressed = false;
		private static IntPtr iconSurface = IntPtr.Zero;
		private static System.Drawing.Imaging.BitmapData iconData = null;
		private static Bitmap iconBmp = null;

		// mouse
		internal static short MouseCenterX = 0;
		internal static short MouseCenterY = 0;
		internal static World.Vector3D MouseCameraPosition = new World.Vector3D(0.0, 0.0, 0.0);
		internal static World.Vector3D MouseCameraDirection = new World.Vector3D(0.0, 0.0, 1.0);
		internal static World.Vector3D MouseCameraUp = new World.Vector3D(0.0, 1.0, 0.0);
		internal static World.Vector3D MouseCameraSide = new World.Vector3D(1.0, 0.0, 0.0);
		internal static byte MouseButton = 0;

		// main
		[STAThread]
		internal static void Main(string[] args) {
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
				string File = System.IO.Path.Combine(Application.StartupPath, "RouteViewer.exe");
				if (System.IO.File.Exists(File)) {
					int Skips = 0;
					System.Text.StringBuilder NewArgs = new System.Text.StringBuilder();
					for (int i = 0; i < args.Length; i++) {
						if (System.IO.File.Exists(args[i])) {
							if (System.IO.Path.GetExtension(args[i]).Equals(".csv", StringComparison.OrdinalIgnoreCase)) {
								string Text = System.IO.File.ReadAllText(args[i], System.Text.Encoding.UTF8);
								if (Text.Length != -1 && Text.IndexOf("CreateMeshBuilder", StringComparison.OrdinalIgnoreCase) == -1) {
									if (NewArgs.Length != 0) NewArgs.Append(" ");
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
					if (Skips == args.Length) return;
				}
			}
			// application
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			if (SDL.SDL_Init(SDL.SDL_INIT_VIDEO) != 0) {
				MessageBox.Show("SDL failed to initialize the video subsystem.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
				return;
			}
			Interface.CurrentOptions.ObjectOptimizationBasicThreshold = 1000;
			Interface.CurrentOptions.ObjectOptimizationFullThreshold = 250;
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
			// create window
			Renderer.ScreenWidth = 960;
			Renderer.ScreenHeight = 600;
			int Bits = 32;
			//Sdl.SDL_SetVideoMode(Renderer.ScreenWidth, Renderer.ScreenHeight, Bits, Sdl.SDL_OPENGL | Sdl.SDL_DOUBLEBUF);
			SDLWindow = SDL.SDL_CreateWindow(Application.ProductName,
				SDL.SDL_WINDOWPOS_UNDEFINED,SDL.SDL_WINDOWPOS_UNDEFINED,
				Renderer.ScreenWidth,Renderer.ScreenHeight,
				SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE);
			if (SDLWindow != IntPtr.Zero) {
				// create window
				string File = OpenBveApi.Path.CombineFile(Program.FileSystem.GetDataFolder(), "icon.png");
				if (System.IO.File.Exists(File)) {
					// set up icon
					iconBmp = new Bitmap(File); // load file
					iconData = iconBmp.LockBits(new Rectangle(0,0,iconBmp.Width,iconBmp.Height),
						System.Drawing.Imaging.ImageLockMode.ReadOnly,
						System.Drawing.Imaging.PixelFormat.Format32bppArgb); // lock data
					iconSurface = SDL.SDL_CreateRGBSurfaceFrom(iconData.Scan0,iconBmp.Width,iconBmp.Height,32,iconData.Stride,
						0x00FF0000,0x0000FF00,0x000000FF,0xFF000000); // upload to sdl
					SDL.SDL_SetWindowIcon(SDLWindow,iconSurface); // use icon
					// free is on the end of the program
					/*
					IntPtr bitmap = SDL.SDL_LoadBMP(File);
					if (bitmap != IntPtr.Zero) {
						SDL.SDL_Surface Surface = (SDL.SDL_Surface)System.Runtime.InteropServices.Marshal.PtrToStructure(bitmap, typeof(SDL.SDL_Surface));
						uint ColorKey = SDL.SDL_MapRGB(Surface.format, 0, 0, 255);
						SDL.SDL_SetColorKey(bitmap, 1, ColorKey);
						SDL.SDL_SetWindowIcon(SDLWindow,bitmap);
					}
					*/
				}
				GLContext = SDL.SDL_GL_CreateContext(SDLWindow);
				GraphicsContext tkContext = new GraphicsContext(new ContextHandle(GLContext),
					                            SDL.SDL_GL_GetProcAddress,
					                            () => new ContextHandle(SDL.SDL_GL_GetCurrentContext()));
				// anisotropic filtering
				string[] extensions = GL.GetString(StringName.Extensions).Split(new []{' '},StringSplitOptions.RemoveEmptyEntries);
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
				// module initialization
				Renderer.Initialize();
				Renderer.InitializeLighting();
				SDL.SDL_GL_SwapWindow(SDLWindow);
				Fonts.Initialize();
				UpdateViewport();
				// command line arguments
				for (int i = 0; i < args.Length; i++) {
					if (!SkipArgs[i] && System.IO.File.Exists(args[i])) {
						try {
							ObjectManager.UnifiedObject o = ObjectManager.LoadObject(args[i], System.Text.Encoding.UTF8, ObjectManager.ObjectLoadMode.Normal, false, false, false);
							ObjectManager.CreateObject(o, new World.Vector3D(0.0, 0.0, 0.0), new World.Transformation(0.0, 0.0, 0.0), new World.Transformation(0.0, 0.0, 0.0), true, 0.0, 0.0, 25.0, 0.0);
						} catch (Exception ex) {
							Interface.AddMessage(Interface.MessageType.Critical, false, "Unhandled error (" + ex.Message + ") encountered while processing the file " + args[i] + ".");
						}
						Array.Resize<string>(ref Files, Files.Length + 1);
						Files[Files.Length - 1] = args[i];
					}
				}
				ObjectManager.InitializeVisibility();
				ObjectManager.FinishCreatingObjects();
				ObjectManager.UpdateVisibility(0.0, true);
				ObjectManager.UpdateAnimatedWorldObjects(0.01, true);
				UpdateCaption();
				Stopwatch timer = new Stopwatch();
				timer.Start();
				LastTicks = timer.ElapsedMilliseconds;
				// loop
				while (!Quit) {
					long ticks = timer.ElapsedMilliseconds;
					double timeElapsed = 0.001 * (double)(ticks - LastTicks);
					if (timeElapsed < 0.0001) {
						timeElapsed = 0.0001;
					}
					LastTicks = ticks;
					DateTime time = DateTime.Now;
					Game.SecondsSinceMidnight = (double)(3600 * time.Hour + 60 * time.Minute + time.Second) + 0.001 * (double)time.Millisecond;
					ObjectManager.UpdateAnimatedWorldObjects(timeElapsed, false);
					ProcessEvents();
					if (ReducedMode) {
						System.Threading.Thread.Sleep(125);
					} else {
						System.Threading.Thread.Sleep(1);
					}
					bool updatelight = false;
					bool keep = false;
					// rotate x
					if (RotateX == 0) {
						double d = (1.0 + Math.Abs(RotateXSpeed)) * timeElapsed;
						if (RotateXSpeed >= -d & RotateXSpeed <= d) {
							RotateXSpeed = 0.0;
						} else {
							RotateXSpeed -= (double)Math.Sign(RotateXSpeed) * d;
						}
					} else {
						double d = (1.0 + 1.0 - 1.0 / (1.0 + RotateXSpeed * RotateXSpeed)) * timeElapsed;
						double m = 1.0;
						RotateXSpeed += (double)RotateX * d;
						if (RotateXSpeed < -m) {
							RotateXSpeed = -m;
						} else if (RotateXSpeed > m) {
							RotateXSpeed = m;
						}
					}
					if (RotateXSpeed != 0.0) {
						double cosa = Math.Cos(RotateXSpeed * timeElapsed);
						double sina = Math.Sin(RotateXSpeed * timeElapsed);
						World.Rotate(ref World.AbsoluteCameraDirection.X, ref World.AbsoluteCameraDirection.Y, ref World.AbsoluteCameraDirection.Z, 0.0, 1.0, 0.0, cosa, sina);
						World.Rotate(ref World.AbsoluteCameraUp.X, ref World.AbsoluteCameraUp.Y, ref World.AbsoluteCameraUp.Z, 0.0, 1.0, 0.0, cosa, sina);
						World.Rotate(ref World.AbsoluteCameraSide.X, ref World.AbsoluteCameraSide.Y, ref World.AbsoluteCameraSide.Z, 0.0, 1.0, 0.0, cosa, sina);
						keep = true;
					}
					// rotate y
					if (RotateY == 0) {
						double d = (1.0 + Math.Abs(RotateYSpeed)) * timeElapsed;
						if (RotateYSpeed >= -d & RotateYSpeed <= d) {
							RotateYSpeed = 0.0;
						} else {
							RotateYSpeed -= (double)Math.Sign(RotateYSpeed) * d;
						}
					} else {
						double d = (1.0 + 1.0 - 1.0 / (1.0 + RotateYSpeed * RotateYSpeed)) * timeElapsed;
						double m = 1.0;
						RotateYSpeed += (double)RotateY * d;
						if (RotateYSpeed < -m) {
							RotateYSpeed = -m;
						} else if (RotateYSpeed > m) {
							RotateYSpeed = m;
						}
					}
					if (RotateYSpeed != 0.0) {
						double cosa = Math.Cos(RotateYSpeed * timeElapsed);
						double sina = Math.Sin(RotateYSpeed * timeElapsed);
						World.Rotate(ref World.AbsoluteCameraDirection.X, ref World.AbsoluteCameraDirection.Y, ref World.AbsoluteCameraDirection.Z, World.AbsoluteCameraSide.X, World.AbsoluteCameraSide.Y, World.AbsoluteCameraSide.Z, cosa, sina);
						World.Rotate(ref World.AbsoluteCameraUp.X, ref World.AbsoluteCameraUp.Y, ref World.AbsoluteCameraUp.Z, World.AbsoluteCameraSide.X, World.AbsoluteCameraSide.Y, World.AbsoluteCameraSide.Z, cosa, sina);
						keep = true;
					}
					// move x
					if (MoveX == 0) {
						double d = (2.5 + Math.Abs(MoveXSpeed)) * timeElapsed;
						if (MoveXSpeed >= -d & MoveXSpeed <= d) {
							MoveXSpeed = 0.0;
						} else {
							MoveXSpeed -= (double)Math.Sign(MoveXSpeed) * d;
						}
					} else {
						double d = (5.0 + 10.0 - 10.0 / (1.0 + MoveXSpeed * MoveXSpeed)) * timeElapsed;
						double m = 25.0;
						MoveXSpeed += (double)MoveX * d;
						if (MoveXSpeed < -m) {
							MoveXSpeed = -m;
						} else if (MoveXSpeed > m) {
							MoveXSpeed = m;
						}
					}
					if (MoveXSpeed != 0.0) {
						World.AbsoluteCameraPosition.X += MoveXSpeed * timeElapsed * World.AbsoluteCameraSide.X;
						World.AbsoluteCameraPosition.Y += MoveXSpeed * timeElapsed * World.AbsoluteCameraSide.Y;
						World.AbsoluteCameraPosition.Z += MoveXSpeed * timeElapsed * World.AbsoluteCameraSide.Z;
						keep = true;
					}
					// move y
					if (MoveY == 0) {
						double d = (2.5 + Math.Abs(MoveYSpeed)) * timeElapsed;
						if (MoveYSpeed >= -d & MoveYSpeed <= d) {
							MoveYSpeed = 0.0;
						} else {
							MoveYSpeed -= (double)Math.Sign(MoveYSpeed) * d;
						}
					} else {
						double d = (5.0 + 10.0 - 10.0 / (1.0 + MoveYSpeed * MoveYSpeed)) * timeElapsed;
						double m = 25.0;
						MoveYSpeed += (double)MoveY * d;
						if (MoveYSpeed < -m) {
							MoveYSpeed = -m;
						} else if (MoveYSpeed > m) {
							MoveYSpeed = m;
						}
					}
					if (MoveYSpeed != 0.0) {
						World.AbsoluteCameraPosition.X += MoveYSpeed * timeElapsed * World.AbsoluteCameraUp.X;
						World.AbsoluteCameraPosition.Y += MoveYSpeed * timeElapsed * World.AbsoluteCameraUp.Y;
						World.AbsoluteCameraPosition.Z += MoveYSpeed * timeElapsed * World.AbsoluteCameraUp.Z;
						keep = true;
					}
					// move z
					if (MoveZ == 0) {
						double d = (2.5 + Math.Abs(MoveZSpeed)) * timeElapsed;
						if (MoveZSpeed >= -d & MoveZSpeed <= d) {
							MoveZSpeed = 0.0;
						} else {
							MoveZSpeed -= (double)Math.Sign(MoveZSpeed) * d;
						}
					} else {
						double d = (5.0 + 10.0 - 10.0 / (1.0 + MoveZSpeed * MoveZSpeed)) * timeElapsed;
						double m = 25.0;
						MoveZSpeed += (double)MoveZ * d;
						if (MoveZSpeed < -m) {
							MoveZSpeed = -m;
						} else if (MoveZSpeed > m) {
							MoveZSpeed = m;
						}
					}
					if (MoveZSpeed != 0.0) {
						World.AbsoluteCameraPosition.X += MoveZSpeed * timeElapsed * World.AbsoluteCameraDirection.X;
						World.AbsoluteCameraPosition.Y += MoveZSpeed * timeElapsed * World.AbsoluteCameraDirection.Y;
						World.AbsoluteCameraPosition.Z += MoveZSpeed * timeElapsed * World.AbsoluteCameraDirection.Z;
						keep = true;
					}
					// lighting
					if (LightingRelative == -1) {
						LightingRelative = (double)LightingTarget;
						updatelight = true;
					}
					if (LightingTarget == 0) {
						if (LightingRelative != 0.0) {
							LightingRelative -= 0.5 * timeElapsed;
							if (LightingRelative < 0.0) LightingRelative = 0.0;
							updatelight = true;
							keep = true;
						}
					} else {
						if (LightingRelative != 1.0) {
							LightingRelative += 0.5 * timeElapsed;
							if (LightingRelative > 1.0) LightingRelative = 1.0;
							updatelight = true;
							keep = true;
						}
					}
					// continue
					if (ReducedMode) {
						ReducedModeEnteringTime = (int)(ticks + 3000);
					} else {
						if (keep) {
							ReducedModeEnteringTime = (int)(ticks + 3000);
						} else if (ticks > ReducedModeEnteringTime) {
							ReducedMode = true;
							World.AbsoluteCameraSide.Y = 0.0;
							World.Normalize(ref World.AbsoluteCameraSide.X, ref World.AbsoluteCameraSide.Y, ref World.AbsoluteCameraSide.Z);
							World.Normalize(ref World.AbsoluteCameraDirection.X, ref World.AbsoluteCameraDirection.Y, ref World.AbsoluteCameraDirection.Z);
							World.AbsoluteCameraUp = World.Cross(World.AbsoluteCameraDirection, World.AbsoluteCameraSide);
						}
					}
					if (updatelight) {
						Renderer.OptionAmbientColor.R = (byte)Math.Round(32.0 + 128.0 * LightingRelative * (2.0 - LightingRelative));
						Renderer.OptionAmbientColor.G = (byte)Math.Round(32.0 + 128.0 * 0.5 * (LightingRelative + LightingRelative * (2.0 - LightingRelative)));
						Renderer.OptionAmbientColor.B = (byte)Math.Round(32.0 + 128.0 * LightingRelative);
						Renderer.OptionDiffuseColor.R = (byte)Math.Round(32.0 + 128.0 * LightingRelative);
						Renderer.OptionDiffuseColor.G = (byte)Math.Round(32.0 + 128.0 * LightingRelative);
						Renderer.OptionDiffuseColor.B = (byte)Math.Round(32.0 + 128.0 * Math.Sqrt(LightingRelative));
						Renderer.InitializeLighting();
					}
					Renderer.RenderScene();
					SDL.SDL_GL_SwapWindow(SDLWindow);
				}
				// quit
				TextureManager.UnuseAllTextures();
				if (iconSurface != IntPtr.Zero)
					SDL.SDL_FreeSurface(iconSurface); // free surface
				if (iconBmp != null && iconData != null) {
					iconBmp.UnlockBits(iconData); // free pixels
					iconBmp.Dispose();
				}
				SDL.SDL_GL_DeleteContext(GLContext);
				SDL.SDL_DestroyWindow(SDLWindow);
				SDL.SDL_Quit();
			} else {
				MessageBox.Show("SDL failed to create the window.", Application.ProductName, MessageBoxButtons.OK, MessageBoxIcon.Hand);
			}
		}

		// reset camera
		private static void ResetCamera() {
			World.AbsoluteCameraPosition = new World.Vector3D(-5.0, 2.5, -25.0);
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
			Matrix4d persp = Matrix4d.CreatePerspectiveFieldOfView(World.VerticalViewingAngle,World.AspectRatio, 0.2, 1000.0);
			GL.LoadMatrix(ref persp);
			GL.Scale(-1,1,1);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.LoadIdentity();
		}

		// process events
		private static void ProcessEvents() {
			SDL.SDL_Event Event;
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
					case SDL.SDL_EventType.SDL_MOUSEBUTTONDOWN:
						MouseCenterX = (short)Event.button.x;
						MouseCenterY = (short)Event.button.y;
						MouseCameraPosition = World.AbsoluteCameraPosition;
						MouseCameraDirection = World.AbsoluteCameraDirection;
						MouseCameraUp = World.AbsoluteCameraUp;
						MouseCameraSide = World.AbsoluteCameraSide;
						MouseButton = Event.button.button;
						break;
					case SDL.SDL_EventType.SDL_MOUSEBUTTONUP:
						MouseButton = 0;
						break;
					case SDL.SDL_EventType.SDL_MOUSEMOTION:
						if (MouseButton == SDL.SDL_BUTTON_LEFT) {
							World.AbsoluteCameraDirection = MouseCameraDirection;
							World.AbsoluteCameraUp = MouseCameraUp;
							World.AbsoluteCameraSide = MouseCameraSide;
							{
								double dx = 0.0025 * (double)(MouseCenterX - Event.motion.x);
								double cosa = Math.Cos(dx);
								double sina = Math.Sin(dx);
								World.Rotate(ref World.AbsoluteCameraDirection.X, ref World.AbsoluteCameraDirection.Y, ref World.AbsoluteCameraDirection.Z, 0.0, 1.0, 0.0, cosa, sina);
								World.Rotate(ref World.AbsoluteCameraUp.X, ref World.AbsoluteCameraUp.Y, ref World.AbsoluteCameraUp.Z, 0.0, 1.0, 0.0, cosa, sina);
								World.Rotate(ref World.AbsoluteCameraSide.X, ref World.AbsoluteCameraSide.Y, ref World.AbsoluteCameraSide.Z, 0.0, 1.0, 0.0, cosa, sina);
							}
							{
								double dy = 0.0025 * (double)(MouseCenterY - Event.motion.y);
								double cosa = Math.Cos(dy);
								double sina = Math.Sin(dy);
								World.Rotate(ref World.AbsoluteCameraDirection.X, ref World.AbsoluteCameraDirection.Y, ref World.AbsoluteCameraDirection.Z, World.AbsoluteCameraSide.X, World.AbsoluteCameraSide.Y, World.AbsoluteCameraSide.Z, cosa, sina);
								World.Rotate(ref World.AbsoluteCameraUp.X, ref World.AbsoluteCameraUp.Y, ref World.AbsoluteCameraUp.Z, World.AbsoluteCameraSide.X, World.AbsoluteCameraSide.Y, World.AbsoluteCameraSide.Z, cosa, sina);
							}
							ReducedMode = false;
						} else if (MouseButton == SDL.SDL_BUTTON_RIGHT) {
							World.AbsoluteCameraPosition = MouseCameraPosition;
							double dx = -0.025 * (double)(Event.motion.x - MouseCenterX);
							World.AbsoluteCameraPosition.X += dx * World.AbsoluteCameraSide.X;
							World.AbsoluteCameraPosition.Y += dx * World.AbsoluteCameraSide.Y;
							World.AbsoluteCameraPosition.Z += dx * World.AbsoluteCameraSide.Z;
							double dy = 0.025 * (double)(Event.motion.y - MouseCenterY);
							World.AbsoluteCameraPosition.X += dy * World.AbsoluteCameraUp.X;
							World.AbsoluteCameraPosition.Y += dy * World.AbsoluteCameraUp.Y;
							World.AbsoluteCameraPosition.Z += dy * World.AbsoluteCameraUp.Z;
							ReducedMode = false;
						} else if (MouseButton == SDL.SDL_BUTTON_MIDDLE) {
							World.AbsoluteCameraPosition = MouseCameraPosition;
							double dx = -0.025 * (double)(Event.motion.x - MouseCenterX);
							World.AbsoluteCameraPosition.X += dx * World.AbsoluteCameraSide.X;
							World.AbsoluteCameraPosition.Y += dx * World.AbsoluteCameraSide.Y;
							World.AbsoluteCameraPosition.Z += dx * World.AbsoluteCameraSide.Z;
							double dz = -0.025 * (double)(Event.motion.y - MouseCenterY);
							World.AbsoluteCameraPosition.X += dz * World.AbsoluteCameraDirection.X;
							World.AbsoluteCameraPosition.Y += dz * World.AbsoluteCameraDirection.Y;
							World.AbsoluteCameraPosition.Z += dz * World.AbsoluteCameraDirection.Z;
							ReducedMode = false;
						}
						break;
						// key down
					case SDL.SDL_EventType.SDL_KEYDOWN:
						switch (Event.key.keysym.sym) {
							case SDL.SDL_Keycode.SDLK_LSHIFT:
							case SDL.SDL_Keycode.SDLK_RSHIFT:
								ShiftPressed = true;
								break;
							case SDL.SDL_Keycode.SDLK_F5:
								// reset
								ReducedMode = false;
								LightingRelative = -1.0;
								Game.Reset();
								TextureManager.UnuseAllTextures();
								Fonts.Initialize();
								Interface.ClearMessages();
								for (int i = 0; i < Files.Length; i++) {
									#if !DEBUG
									try {
										#endif
										ObjectManager.UnifiedObject o = ObjectManager.LoadObject(Files[i], System.Text.Encoding.UTF8, ObjectManager.ObjectLoadMode.Normal, false, false, false);
										ObjectManager.CreateObject(o, new World.Vector3D(0.0, 0.0, 0.0), new World.Transformation(0.0, 0.0, 0.0), new World.Transformation(0.0, 0.0, 0.0), true, 0.0, 0.0, 25.0, 0.0);
										#if !DEBUG
									} catch (Exception ex) {
										Interface.AddMessage(Interface.MessageType.Critical, false, "Unhandled error (" + ex.Message + ") encountered while processing the file " + Files[i] + ".");
									}
									#endif
								}
								ObjectManager.InitializeVisibility();
								ObjectManager.UpdateVisibility(0.0, true);
								ObjectManager.UpdateAnimatedWorldObjects(0.01, true);
								break;
							case SDL.SDL_Keycode.SDLK_F7:
								{
									OpenFileDialog Dialog = new OpenFileDialog();
									Dialog.CheckFileExists = true;
									Dialog.Multiselect = true;
									Dialog.Filter = "CSV/B3D/X/ANIMATED files|*.csv;*.b3d;*.x;*.animated|All files|*";
									if (Dialog.ShowDialog() == DialogResult.OK) {
										string[] f = Dialog.FileNames;
										int n = Files.Length;
										Array.Resize<string>(ref Files, n + f.Length);
										for (int i = 0; i < f.Length; i++) {
											Files[n + i] = f[i];
										}
										// reset
										ReducedMode = false;
										LightingRelative = -1.0;
										Game.Reset();
										TextureManager.UnuseAllTextures();
										Fonts.Initialize();
										Interface.ClearMessages();
										for (int i = 0; i < Files.Length; i++) {
											#if !DEBUG
											try {
												#endif
												ObjectManager.UnifiedObject o = ObjectManager.LoadObject(Files[i], System.Text.Encoding.UTF8, ObjectManager.ObjectLoadMode.Normal, false, false, false);
												ObjectManager.CreateObject(o, new World.Vector3D(0.0, 0.0, 0.0), new World.Transformation(0.0, 0.0, 0.0), new World.Transformation(0.0, 0.0, 0.0), true, 0.0, 0.0, 25.0, 0.0);
												#if !DEBUG
											} catch (Exception ex) {
												Interface.AddMessage(Interface.MessageType.Critical, false, "Unhandled error (" + ex.Message + ") encountered while processing the file " + Files[i] + ".");
											}
											#endif
										}
										ObjectManager.InitializeVisibility();
										ObjectManager.FinishCreatingObjects();
										ObjectManager.UpdateVisibility(0.0, true);
										ObjectManager.UpdateAnimatedWorldObjects(0.01, true);
									}
								} break;
							case SDL.SDL_Keycode.SDLK_F9:
								if (Interface.MessageCount != 0) {
									formMessages.ShowMessages();
								}
								break;
							case SDL.SDL_Keycode.SDLK_DELETE:
								ReducedMode = false;
								LightingRelative = -1.0;
								Game.Reset();
								TextureManager.UnuseAllTextures();
								Fonts.Initialize();
								Interface.ClearMessages();
								Files = new string[] { };
								UpdateCaption();
								break;
							case SDL.SDL_Keycode.SDLK_LEFT:
								RotateX = -1;
								ReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_RIGHT:
								RotateX = 1;
								ReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_UP:
								RotateY = -1;
								ReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_DOWN:
								RotateY = 1;
								ReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_a:
							case SDL.SDL_Keycode.SDLK_KP_4:
								MoveX = -1;
								ReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_d:
							case SDL.SDL_Keycode.SDLK_KP_6:
								MoveX = 1;
								ReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_KP_8:
								MoveY = 1;
								ReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_KP_2:
								MoveY = -1;
								ReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_w:
							case SDL.SDL_Keycode.SDLK_KP_9:
								MoveZ = 1;
								ReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_s:
							case SDL.SDL_Keycode.SDLK_KP_3:
								MoveZ = -1;
								ReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_KP_5:
								ResetCamera();
								break;
							case SDL.SDL_Keycode.SDLK_f:
							case SDL.SDL_Keycode.SDLK_F1:
								Renderer.OptionWireframe = !Renderer.OptionWireframe;
								if (Renderer.OptionWireframe) {
									GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
								} else {
									GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
								} break;
							case SDL.SDL_Keycode.SDLK_n:
							case SDL.SDL_Keycode.SDLK_F2:
								Renderer.OptionNormals = !Renderer.OptionNormals;
								break;
							case SDL.SDL_Keycode.SDLK_l:
							case SDL.SDL_Keycode.SDLK_F3:
								LightingTarget = 1 - LightingTarget;
								ReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_i:
							case SDL.SDL_Keycode.SDLK_F4:
								Renderer.OptionInterface = !Renderer.OptionInterface;
								ReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_g:
							case SDL.SDL_Keycode.SDLK_c:
								Renderer.OptionCoordinateSystem = !Renderer.OptionCoordinateSystem;
								ReducedMode = false;
								break;
							case SDL.SDL_Keycode.SDLK_b:
								if (ShiftPressed) {
									ColorDialog dialog = new ColorDialog();
									dialog.FullOpen = true;
									if (dialog.ShowDialog() == DialogResult.OK) {
										Renderer.BackgroundColor = -1;
										Renderer.ApplyBackgroundColor(dialog.Color.R, dialog.Color.G, dialog.Color.B);
									}
								} else {
									Renderer.BackgroundColor++;
									if (Renderer.BackgroundColor >= Renderer.MaxBackgroundColor) {
										Renderer.BackgroundColor = 0;
									}
									Renderer.ApplyBackgroundColor();
								}
								ReducedMode = false;
								break;
						} break;
						// key up
					case SDL.SDL_EventType.SDL_KEYUP:
						switch (Event.key.keysym.sym) {
							case SDL.SDL_Keycode.SDLK_LSHIFT:
							case SDL.SDL_Keycode.SDLK_RSHIFT:
								ShiftPressed = false;
								break;
							case SDL.SDL_Keycode.SDLK_LEFT:
							case SDL.SDL_Keycode.SDLK_RIGHT:
								RotateX = 0;
								break;
							case SDL.SDL_Keycode.SDLK_UP:
							case SDL.SDL_Keycode.SDLK_DOWN:
								RotateY = 0;
								break;
							case SDL.SDL_Keycode.SDLK_a:
							case SDL.SDL_Keycode.SDLK_d:
							case SDL.SDL_Keycode.SDLK_KP_4:
							case SDL.SDL_Keycode.SDLK_KP_6:
								MoveX = 0;
								break;
							case SDL.SDL_Keycode.SDLK_KP_8:
							case SDL.SDL_Keycode.SDLK_KP_2:
								MoveY = 0;
								break;
							case SDL.SDL_Keycode.SDLK_w:
							case SDL.SDL_Keycode.SDLK_s:
							case SDL.SDL_Keycode.SDLK_KP_9:
							case SDL.SDL_Keycode.SDLK_KP_3:
								MoveZ = 0;
								break;
						} break;
				}
			}
		}

		// update caption
		private static void UpdateCaption() {
			if (Files.Length != 0) {
				string Title = "";
				for (int i = 0; i < Files.Length; i++) {
					if (i != 0) Title += ", ";
					Title += System.IO.Path.GetFileName(Files[i]);
				}
				SDL.SDL_SetWindowTitle(SDLWindow,Title + " - " + Application.ProductName);
			} else {
				SDL.SDL_SetWindowTitle(SDLWindow,Application.ProductName);
			}
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

	}
}