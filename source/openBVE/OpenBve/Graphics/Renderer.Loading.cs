using System;
using System.Threading;
using System.Drawing;
using OpenBveApi.Colors;
using OpenTK.Graphics.OpenGL;
using SDL2;
namespace OpenBve {
	internal static partial class Renderer {
		
		/* --------------------------------------------------------------
		 * This file contains the drawing routines for the loading screen
		 * -------------------------------------------------------------- */
		internal static bool DrawLoad = true;
		internal static readonly object LoadingLock = new object();
		internal static bool LoadingRemakeCurrent = false;
		internal static void DrawLoadingScreenLoop(){
			Screen.MakeCurrent();
			GL.Disable(EnableCap.Fog);
			GL.MatrixMode(MatrixMode.Projection);
			GL.PushMatrix();
			GL.LoadIdentity();
			GL.Ortho(0.0, (double)Screen.Width, (double)Screen.Height, 0.0, -1.0, 1.0);
			GL.MatrixMode(MatrixMode.Modelview);
			GL.PushMatrix();
			GL.LoadIdentity();
			GL.ClearColor(Color.Black);

			SDL.SDL_Event ev;
			while (DrawLoad) {
				Timers.GetElapsedTime();
				lock (LoadingLock) {
					if (LoadingRemakeCurrent) {
						Screen.MakeCurrent();
						LoadingRemakeCurrent = false;
					}
					GL.Viewport(0, 0, Screen.Width, Screen.Height);
					DrawLoadingScreen();
					Screen.SwapBuffers();
				}
				while (SDL.SDL_PollEvent(out ev) != 0) {
					switch (ev.type) {
						case SDL.SDL_EventType.SDL_QUIT:
							DrawLoad = false;
							Loading.Cancel = true;
							MainLoop.Quit = true;
							break;
						case SDL.SDL_EventType.SDL_WINDOWEVENT:
							if (ev.window.windowEvent == SDL.SDL_WindowEventID.SDL_WINDOWEVENT_RESIZED) {
								Screen.Width = ev.window.data1;
								Screen.Height = ev.window.data2;
								GL.MatrixMode(MatrixMode.Projection);
								GL.LoadIdentity();
								GL.Ortho(0.0, (double)ev.window.data1, (double)ev.window.data2, 0.0, -1.0, 1.0);
								GL.MatrixMode(MatrixMode.Modelview);
								GL.LoadIdentity();
							}
							break;
					}
				}
				double time = Timers.GetElapsedTime();
				double wait = 1000.0 / 60.0 - time*1000 - 50;
				if (wait > 0)
					Thread.Sleep((int)(wait));
			}
			GL.PopMatrix();
			GL.MatrixMode(MatrixMode.Projection);
			GL.PopMatrix();
			GL.MatrixMode(MatrixMode.Modelview);
		}
		internal static void DrawLoadingScreen() {
			// begin HACK //
			if (!BlendEnabled) {
				GL.Enable(EnableCap.Blend);
				GL.BlendFunc(BlendingFactorSrc.SrcAlpha, BlendingFactorDest.OneMinusSrcAlpha);
				BlendEnabled = true;
			}
			if (LightingEnabled) {
				GL.Disable(EnableCap.Lighting);
				LightingEnabled = false;
			}
			int size = Math.Min(Screen.Width, Screen.Height);
			//DrawRectangle(null, new Point(0, 0), Screen.Size, Color128.Black);
			GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
			GL.PushMatrix();
				DrawRectangle(TextureLogo, new Point((Screen.Width - size) / 2, (Screen.Height - size) / 2), new Size(size, size), Color128.White);
			DrawRectangle(null, 
				new Point((Screen.Width - size) / 2, Screen.Height - (int)Fonts.NormalFont.FontSize - 10),
				new Size(Screen.Width, (int)Fonts.NormalFont.FontSize + 10), 
				new Color128(0.0f, 0.0f, 0.0f, 0.5f));
			double routeProgress = Math.Max(0.0, Math.Min(1.0, Loading.RouteProgress));
			double trainProgress = Math.Max(0.0, Math.Min(1.0, Loading.TrainProgress));
			string text;
			if (routeProgress < 1.0) {
				//text = "Loading route... " + (100.0 * routeProgress).ToString("0") + "%";
				string percent = (100.0 * routeProgress).ToString("0.0");
				text = String.Format("{0} {1}%",Strings.GetInterfaceString("loading_loading_route"),percent);
			} else if (trainProgress < 1.0) {
				//text = "Loading train... " + (100.0 * trainProgress).ToString("0") + "%";
				string percent = (100.0 * trainProgress).ToString("0.0");
				text = String.Format("{0} {1}%",Strings.GetInterfaceString("loading_loading_train"),percent);
			} else {
				//text = "Loading textures and sounds...";
				text = Strings.GetInterfaceString("message_loading");
			}
			DrawString(Fonts.SmallFont, text, new Point((Screen.Width - size) / 2 + 5, Screen.Height - (int)(Fonts.NormalFont.FontSize / 2) - 5), TextAlignment.CenterLeft, Color128.White);
			GL.PopMatrix();
			// end HACK //
		}
		
	}
}