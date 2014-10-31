using System;
using System.Drawing;
using OpenBveApi.Colors;
using Tao.OpenGl;

namespace OpenBve {
	internal static partial class Renderer {

		/// <summary>Draws a rectangle.</summary>
		/// <param name="texture">The texture, or a null reference.</param>
		/// <param name="point">The top-left coordinates in pixels.</param>
		/// <param name="size">The size in pixels.</param>
		/// <param name="color">The color, or a null reference.</param>
		internal static void DrawRectangle(Textures.Texture texture, Point point, Size size, Nullable<Color128> color) {
			// TODO: Remove Nullable<T> from color once RenderOverlayTexture and RenderOverlaySolid are fully replaced.
			if (texture == null || !Textures.LoadTexture(texture, Textures.OpenGlTextureWrapMode.ClampClamp)) {
				Gl.glDisable(Gl.GL_TEXTURE_2D);
				if (color.HasValue) {
					Gl.glColor4d(color.Value.R, color.Value.G, color.Value.B, color.Value.A);
				}
				Gl.glBegin(Gl.GL_QUADS);
				Gl.glVertex2d(point.X, point.Y);
				Gl.glVertex2d(point.X + size.Width, point.Y);
				Gl.glVertex2d(point.X + size.Width, point.Y + size.Height);
				Gl.glVertex2d(point.X, point.Y + size.Height);
				Gl.glEnd();
			} else {
				Gl.glEnable(Gl.GL_TEXTURE_2D);
				Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture.OpenGlTextures[(int)Textures.OpenGlTextureWrapMode.ClampClamp].Name);
				if (color.HasValue) {
					Gl.glColor4d(color.Value.R, color.Value.G, color.Value.B, color.Value.A);
				}
				Gl.glBegin(Gl.GL_QUADS);
				Gl.glTexCoord2f(0.0f, 0.0f);
				Gl.glVertex2d(point.X, point.Y);
				Gl.glTexCoord2f(1.0f, 0.0f);
				Gl.glVertex2d(point.X + size.Width, point.Y);
				Gl.glTexCoord2f(1.0f, 1.0f);
				Gl.glVertex2d(point.X + size.Width, point.Y + size.Height);
				Gl.glTexCoord2f(0.0f, 1.0f);
				Gl.glVertex2d(point.X, point.Y + size.Height);
				Gl.glEnd();
			}
		}
		
	}
}