using System;
using System.Drawing;
using OpenBveApi.Colors;
using Tao.OpenGl;

namespace OpenBve {
	internal static partial class Renderer {
		
		/* --------------------------------------------------------------
		 * This file contains the drawing routines for the loading screen
		 * -------------------------------------------------------------- */
		
		internal static void DrawLoadingScreen() {
			
			// begin HACK //
			Gl.glEnable(Gl.GL_BLEND); BlendEnabled = true;
			Gl.glDisable(Gl.GL_LIGHTING); LightingEnabled = false;
			int size = Math.Min(Screen.Width, Screen.Height);
			DrawRectangle(null, new Point(0, 0), new Size(Screen.Width, Screen.Height), Color128.Black);
			if (Textures.LoadTexture(TextureLogo, Textures.OpenGlTextureWrapMode.ClampClamp)) {
				DrawRectangle(TextureLogo, new Point((Screen.Width - size) / 2, (Screen.Height - size) / 2), new Size(size, size), Color128.White);
			}
			DrawRectangle(null, new Point((Screen.Width - size) / 2, Screen.Height - (int)Fonts.NormalFont.FontSize - 10), new Size(Screen.Width, (int)Fonts.NormalFont.FontSize + 10), new Color128(0.0f, 0.0f, 0.0f, 0.5f));
//			double routeProgress = Math.Max(0.0, Math.Min(1.0, Loading.RouteProgress));
//			double trainProgress = Math.Max(0.0, Math.Min(1.0, Loading.TrainProgress));
			string text;
//			if (routeProgress < 1.0) {
//				text = "Loading route... " + (100.0 * routeProgress).ToString("0") + "%";
//			} else if (trainProgress < 1.0) {
//				text = "Loading train... " + (100.0 * trainProgress).ToString("0") + "%";
//			} else {
//				text = "Loading textures and sounds...";
//			}
			text = Interface.GetInterfaceString("message_loading");
			DrawString(Fonts.SmallFont, text, new Point((Screen.Width - size) / 2 + 5, Screen.Height - (int)(Fonts.NormalFont.FontSize / 2) - 5), TextAlignment.CenterLeft, Color128.White);
			// end HACK //

		}
		
	}
}