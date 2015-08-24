using System;
using System.Windows.Forms;
using SDL2;
using OpenTK.Graphics.OpenGL;
using OpenTK.Graphics;
using OpenTK;

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

		internal static IntPtr Window{ get; private set;}
		internal static IntPtr GLContext { get; private set;}
		private static IntPtr iconSurface = IntPtr.Zero;
		private static System.Drawing.Imaging.BitmapData iconData = null;
		private static System.Drawing.Bitmap iconBmp = null;
		private static GraphicsContext tkContext;
		internal static System.Drawing.Size Size {
			get {
				return new System.Drawing.Size(Width, Height);
			}
			set {
				Width = value.Width;
				Height = value.Height;
			}
		}
		// --- functions ---
		
		/// <summary>Initializes the screen. A call to SDL_Init must have been made before calling this function. A call to Deinitialize must be made when terminating the program.</summary>
		/// <returns>Whether initializing the screen was successful.</returns>
		internal static bool Initialize() {
			if (SDL.SDL_InitSubSystem(SDL.SDL_INIT_VIDEO) != 0) {
				return false;
			}
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_DOUBLEBUFFER, 1);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_RED_SIZE, 8);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_GREEN_SIZE, 8);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_BLUE_SIZE, 8);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_ALPHA_SIZE, 0);
			SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_DEPTH_SIZE, 24);
			SDL.SDL_GL_SetSwapInterval(Options.Current.VerticalSynchronization ? 1 : 0);
			if (Options.Current.AntiAliasingLevel != 0) {
				SDL.SDL_GL_SetAttribute(SDL.SDL_GLattr.SDL_GL_MULTISAMPLESAMPLES, Options.Current.AntiAliasingLevel);
			}
			// --- video mode ---
			Width = Options.Current.FullscreenMode ? Options.Current.FullscreenWidth : Options.Current.WindowWidth;
			Height = Options.Current.FullscreenMode ? Options.Current.FullscreenHeight : Options.Current.WindowHeight;
			Fullscreen = Options.Current.FullscreenMode;
			SDL.SDL_WindowFlags flags = SDL.SDL_WindowFlags.SDL_WINDOW_OPENGL/*| SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE*/
			                             | SDL.SDL_WindowFlags.SDL_WINDOW_HIDDEN | SDL.SDL_WindowFlags.SDL_WINDOW_RESIZABLE;
			if (Fullscreen) {
				flags |= SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN;
			}
			SDL.SDL_SetHint(SDL.SDL_HINT_RENDER_VSYNC, "true");
			SDL.SDL_SetHint(SDL.SDL_HINT_VIDEO_ALLOW_SCREENSAVER, "false");
			Window = SDL.SDL_CreateWindow(Application.ProductName, SDL.SDL_WINDOWPOS_CENTERED, SDL.SDL_WINDOWPOS_CENTERED, Width, Height, flags);
			if (Window == IntPtr.Zero) {
				// --- not successful ---
				SDL.SDL_QuitSubSystem(SDL.SDL_INIT_VIDEO);
				return false;
			}
			GLContext = SDL.SDL_GL_CreateContext(Window);
			// --- set up OpenTK context
			tkContext = new GraphicsContext(new ContextHandle(GLContext),
				                        SDL.SDL_GL_GetProcAddress, 
				() => new ContextHandle(SDL.SDL_GL_GetCurrentContext()));
			// --- set up icon ---
			string bitmapFile = OpenBveApi.Path.CombineFile(Program.FileSystem.DataFolder, "icon.png");
			if (System.IO.File.Exists(bitmapFile)) {
				iconBmp = new System.Drawing.Bitmap(bitmapFile); // load file
				iconData = iconBmp.LockBits(new System.Drawing.Rectangle(0, 0, iconBmp.Width, iconBmp.Height),
					System.Drawing.Imaging.ImageLockMode.ReadOnly,
					System.Drawing.Imaging.PixelFormat.Format32bppArgb); // lock data
				iconSurface = SDL.SDL_CreateRGBSurfaceFrom(iconData.Scan0, iconBmp.Width, iconBmp.Height, 32, iconData.Stride,
					0x00FF0000, 0x0000FF00, 0x000000FF, 0xFF000000); // upload to sdl
				SDL.SDL_SetWindowIcon(Window, iconSurface); // use icon
				// free in Deinitialize()
			}
			// --- set up anisotropic filtering ---
			Options.Current.AnisotropicFilteringMaximum = 0;
			string[] extensions = GL.GetString(StringName.Extensions).Split(new []{ ' ' });
			for (int i = 0; i < extensions.Length; i++) {
				if (extensions[i] == "GL_EXT_texture_filter_anisotropic") {
					float n;
					GL.GetFloat((GetPName)ExtTextureFilterAnisotropic.MaxTextureMaxAnisotropyExt, out n);
					int m = (int)Math.Round(n);
					Options.Current.AnisotropicFilteringMaximum = Math.Max(0, m);
					break;
				}
			}
			if (Options.Current.AnisotropicFilteringLevel <= 0) {
				Options.Current.AnisotropicFilteringLevel = Options.Current.AnisotropicFilteringMaximum;
			} else if (Options.Current.AnisotropicFilteringLevel > Options.Current.AnisotropicFilteringMaximum) {
				Options.Current.AnisotropicFilteringLevel = Options.Current.AnisotropicFilteringMaximum;
			}
			// --- done ---
			Initialized = true;
			return true;
		}
		
		/// <summary>Deinitializes the screen.</summary>
		internal static void Deinitialize() {
			if (Initialized) {
				if (iconSurface != IntPtr.Zero)
					SDL.SDL_FreeSurface(iconSurface); // free surface
				if (iconBmp != null && iconData != null) {
					iconBmp.UnlockBits(iconData); // free pixels
					iconBmp.Dispose();
				}
				tkContext.Dispose();
				SDL.SDL_GL_DeleteContext(GLContext);
				SDL.SDL_DestroyWindow(Window);
				SDL.SDL_QuitSubSystem(SDL.SDL_INIT_VIDEO);
				Initialized = false;
			}
		}
		
		/// <summary>Changes to or from fullscreen mode.</summary>
		internal static void ToggleFullscreen() {
			Fullscreen = !Fullscreen;
			// begin HACK //
			Renderer.ClearDisplayLists();
			if (World.MouseGrabEnabled) {
				SDL.SDL_SetRelativeMouseMode(SDL.SDL_bool.SDL_FALSE);
			}
			GL.Disable(EnableCap.Fog);
			Renderer.FogEnabled = false;
			GL.Disable(EnableCap.Lighting);
			Renderer.LightingEnabled = false;
			Textures.UnloadAllTextures();
			if (Fullscreen) {
				SDL.SDL_SetWindowSize(Window,Options.Current.FullscreenWidth,Options.Current.FullscreenHeight);
				SDL.SDL_SetWindowFullscreen(Window,(uint)SDL.SDL_WindowFlags.SDL_WINDOW_FULLSCREEN);
				Width = Options.Current.FullscreenWidth;
				Height = Options.Current.FullscreenHeight;
			} else {
				SDL.SDL_SetWindowSize(Window,Options.Current.WindowWidth,Options.Current.WindowHeight);
				SDL.SDL_SetWindowFullscreen(Window,0);
				Width = Options.Current.WindowWidth;
				Height = Options.Current.WindowHeight;
			}
			Renderer.InitializeLighting();
			MainLoop.UpdateViewport(MainLoop.ViewPortChangeMode.NoChange);
			MainLoop.InitializeMotionBlur();
			Timetable.CreateTimetable();
			Timetable.UpdateCustomTimetable(null, null);
			if (World.MouseGrabEnabled) {
				SDL.SDL_SetRelativeMouseMode(SDL.SDL_bool.SDL_TRUE);
			}
			World.MouseGrabTarget = new OpenBveApi.Math.Vector2D(0.0, 0.0);
			World.MouseGrabIgnoreOnce = true;
			World.InitializeCameraRestriction();
			if (Renderer.OptionBackfaceCulling) {
				GL.Enable(EnableCap.CullFace);
			} else {
				GL.Disable(EnableCap.CullFace);
			}
			Renderer.ReAddObjects();
			// end HACK //
		}
		internal static void SwapBuffers(){
			SDL.SDL_GL_SwapWindow(Window);
		}
		private static Object makeCurrentLock = null;
		internal static void MakeCurrent(){
			if (makeCurrentLock == null)
				makeCurrentLock = new object();
			lock (makeCurrentLock) {
				SDL.SDL_GL_MakeCurrent(Window,GLContext);
			}
		}
		internal static void Show(){
			SDL.SDL_ShowWindow(Window);
		}
		internal static void Hide(){
			SDL.SDL_HideWindow(Window);
		}
	}
}