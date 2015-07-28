using System;
using System.Windows.Forms;
using Tao.OpenGl;
using Tao.Sdl;

namespace OpenBve {
	internal static class Screen {
		
		// --- members ---
		
		/// <summary>Whether the screen is initialized.</summary>
		private static bool Initialized = false;
		
		/// <summary>The fixed width of the screen.</summary>
		internal static int Width = 0;
		
		/// <summary>The fixed height of the screen.</summary>
		internal static int Height = 0;
		
		/// <summary>Whether the screen is set to fullscreen mode.</summary>
		internal static bool Fullscreen = false;
		
		
		// --- functions ---
		
		/// <summary>Initializes the screen. A call to SDL_Init must have been made before calling this function. A call to Deinitialize must be made when terminating the program.</summary>
		/// <returns>Whether initializing the screen was successful.</returns>
		internal static bool Initialize() {
			if (Sdl.SDL_Init(Sdl.SDL_INIT_VIDEO) != 0) {
				return false;
			} else {
				Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_DOUBLEBUFFER, 1);
				Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_RED_SIZE, 8);
				Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_GREEN_SIZE, 8);
				Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_BLUE_SIZE, 8);
				Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_ALPHA_SIZE, 0);
				Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_DEPTH_SIZE, 24);
				Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_SWAP_CONTROL, Interface.CurrentOptions.VerticalSynchronization ? 1 : 0);
				if (Interface.CurrentOptions.AntiAliasingLevel != 0) {
					Sdl.SDL_GL_SetAttribute(Sdl.SDL_GL_MULTISAMPLESAMPLES, Interface.CurrentOptions.AntiAliasingLevel);
				}
				Sdl.SDL_ShowCursor(Sdl.SDL_DISABLE);
				// --- window caption and icon ---
				Sdl.SDL_WM_SetCaption(Application.ProductName, null);
				{
					string bitmapFile = OpenBveApi.Path.CombineFile(Program.FileSystem.DataFolder, "icon.bmp");
					IntPtr bitmap = Sdl.SDL_LoadBMP(bitmapFile);
					if (bitmap != null) {
						string maskFile = OpenBveApi.Path.CombineFile(Program.FileSystem.DataFolder, "mask.bin");
						byte[] mask = System.IO.File.ReadAllBytes(maskFile);
						Sdl.SDL_WM_SetIcon(bitmap, mask);
					}
				}
				// --- video mode ---
				Width = Interface.CurrentOptions.FullscreenMode ? Interface.CurrentOptions.FullscreenWidth : Interface.CurrentOptions.WindowWidth;
				Height = Interface.CurrentOptions.FullscreenMode ? Interface.CurrentOptions.FullscreenHeight : Interface.CurrentOptions.WindowHeight;
				Fullscreen = Interface.CurrentOptions.FullscreenMode;
				int bits = Interface.CurrentOptions.FullscreenMode ? Interface.CurrentOptions.FullscreenBits : 32;
				int flags = Sdl.SDL_OPENGL | Sdl.SDL_HWSURFACE | Sdl.SDL_ANYFORMAT | Sdl.SDL_DOUBLEBUF;
				if (Fullscreen) {
					flags |= Sdl.SDL_FULLSCREEN;
				}
				IntPtr video = Sdl.SDL_SetVideoMode(Width, Height, bits, flags);
				if (video == IntPtr.Zero) {
					// --- not successful ---
					Sdl.SDL_QuitSubSystem(Sdl.SDL_INIT_VIDEO);
					return false;
				} else {
					// --- set up anisotropic filtering ---
					Interface.CurrentOptions.AnisotropicFilteringMaximum = 0;
					string[] extensions = Gl.glGetString(Gl.GL_EXTENSIONS).Split(new char[] { ' ' });
					for (int i = 0; i < extensions.Length; i++) {
						if (extensions[i] == "GL_EXT_texture_filter_anisotropic") {
							float n;
							Gl.glGetFloatv(Gl.GL_MAX_TEXTURE_MAX_ANISOTROPY_EXT, out n);
							int m = (int)Math.Round(n);
							Interface.CurrentOptions.AnisotropicFilteringMaximum = Math.Max(0, m);
							break;
						}
					}
					if (Interface.CurrentOptions.AnisotropicFilteringLevel <= 0) {
						Interface.CurrentOptions.AnisotropicFilteringLevel = Interface.CurrentOptions.AnisotropicFilteringMaximum;
					} else if (Interface.CurrentOptions.AnisotropicFilteringLevel > Interface.CurrentOptions.AnisotropicFilteringMaximum) {
						Interface.CurrentOptions.AnisotropicFilteringLevel = Interface.CurrentOptions.AnisotropicFilteringMaximum;
					}
					// --- done ---
					Initialized = true;
					return true;
				}
			}
		}
		
		/// <summary>Deinitializes the screen.</summary>
		internal static void Deinitialize() {
			if (Initialized) {
				Sdl.SDL_QuitSubSystem(Sdl.SDL_INIT_VIDEO);
				Initialized = false;
			}
		}
		
		/// <summary>Changes to or from fullscreen mode.</summary>
		internal static void ToggleFullscreen() {
			Fullscreen = !Fullscreen;
			// begin HACK //
			Renderer.ClearDisplayLists();
			if (World.MouseGrabEnabled) {
				Sdl.SDL_WM_GrabInput(Sdl.SDL_GRAB_OFF);
			}
			Gl.glDisable(Gl.GL_FOG);
			Renderer.FogEnabled = false;
			Gl.glDisable(Gl.GL_LIGHTING);
			Renderer.LightingEnabled = false;
			Textures.UnloadAllTextures();
			if (Fullscreen) {
				Sdl.SDL_SetVideoMode(Interface.CurrentOptions.FullscreenWidth, Interface.CurrentOptions.FullscreenHeight, Interface.CurrentOptions.FullscreenBits, Sdl.SDL_OPENGL | Sdl.SDL_DOUBLEBUF | Sdl.SDL_FULLSCREEN);
				Width = Interface.CurrentOptions.FullscreenWidth;
				Height = Interface.CurrentOptions.FullscreenHeight;
			} else {
				Sdl.SDL_SetVideoMode(Interface.CurrentOptions.WindowWidth, Interface.CurrentOptions.WindowHeight, 32, Sdl.SDL_OPENGL | Sdl.SDL_DOUBLEBUF);
				Width = Interface.CurrentOptions.WindowWidth;
				Height = Interface.CurrentOptions.WindowHeight;
			}
			Renderer.InitializeLighting();
			MainLoop.UpdateViewport(MainLoop.ViewPortChangeMode.NoChange);
			MainLoop.InitializeMotionBlur();
			Timetable.CreateTimetable();
			Timetable.UpdateCustomTimetable(null, null);
			if (World.MouseGrabEnabled) {
				Sdl.SDL_WM_GrabInput(Sdl.SDL_GRAB_ON);
			}
			World.MouseGrabTarget = new World.Vector2D(0.0, 0.0);
			World.MouseGrabIgnoreOnce = true;
			World.InitializeCameraRestriction();
			if (Renderer.OptionBackfaceCulling) {
				Gl.glEnable(Gl.GL_CULL_FACE);
			} else {
				Gl.glDisable(Gl.GL_CULL_FACE);
			}
			Renderer.ReAddObjects();
			// end HACK //
		}
		
	}
}