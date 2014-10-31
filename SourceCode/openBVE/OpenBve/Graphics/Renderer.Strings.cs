using System;
using System.Drawing;
using OpenBveApi.Colors;
using Tao.OpenGl;

namespace OpenBve {
	internal static partial class Renderer {
		
		// --- structures ---
		
		/// <summary>Represents the alignment of a text compared to a reference coordinate.</summary>
		private enum TextAlignment {
			/// <summary>The reference coordinate represents the top-left corner.</summary>
			TopLeft = 1,
			/// <summary>The reference coordinate represents the top-middle corner.</summary>
			TopMiddle = 2,
			/// <summary>The reference coordinate represents the top-right corner.</summary>
			TopRight = 4,
			/// <summary>The reference coordinate represents the center-left corner.</summary>
			CenterLeft = 8,
			/// <summary>The reference coordinate represents the center-middle corner.</summary>
			CenterMiddle = 16,
			/// <summary>The reference coordinate represents the center-right corner.</summary>
			CenterRight = 32,
			/// <summary>The reference coordinate represents the bottom-left corner.</summary>
			BottomLeft = 64,
			/// <summary>The reference coordinate represents the bottom-middle corner.</summary>
			BottomMiddle = 128,
			/// <summary>The reference coordinate represents the bottom-right corner.</summary>
			BottomRight = 256,
			/// <summary>Represents the left for bitmasking.</summary>
			Left = TopLeft | CenterLeft | BottomLeft,
			/// <summary>Represents the (horizontal) middle for bitmasking.</summary>
			Middle = TopMiddle | CenterMiddle | BottomMiddle,
			/// <summary>Represents the right for bitmasking.</summary>
			Right = TopRight | CenterRight | BottomRight,
			/// <summary>Represents the top for bitmasking.</summary>
			Top = TopLeft | TopMiddle | TopRight,
			/// <summary>Represents the (vertical) center for bitmasking.</summary>
			Center = CenterLeft | CenterMiddle | CenterRight,
			/// <summary>Represents the bottom for bitmasking.</summary>
			Bottom = BottomLeft | BottomMiddle | BottomRight
		}
		
		
		// --- functions ---
		
		/// <summary>Measures the size of a string as it would be rendered using the specified font.</summary>
		/// <param name="font">The font to use.</param>
		/// <param name="text">The string to render.</param>
		/// <returns>The size of the string.</returns>
		private static Size MeasureString(Fonts.OpenGlFont font, string text) {
			int width = 0;
			int height = 0;
			if (text != null) {
				for (int i = 0; i < text.Length; i++) {
					Textures.Texture texture;
					Fonts.OpenGlFontChar data;
					i += font.GetCharacterData(text, i, out texture, out data) - 1;
					width += data.TypographicSize.Width;
					if (data.TypographicSize.Height > height) {
						height = data.TypographicSize.Height;
					}
				}
			}
			return new Size(width, height);
		}

		/// <summary>Renders a string to the screen.</summary>
		/// <param name="font">The font to use.</param>
		/// <param name="text">The string to render.</param>
		/// <param name="location">The location.</param>
		/// <param name="orientation">The orientation.</param>
		/// <param name="color">The color.</param>
		/// <remarks>This function sets the OpenGL blend function to glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA).</remarks>
		private static void DrawString(Fonts.OpenGlFont font, string text, Point location, TextAlignment alignment, Color128 color) {
			if (text == null) {
				return;
			}
			/*
			 * Prepare the top-left coordinates for rendering, incorporating the
			 * orientation of the string in relation to the specified location.
			 * */
			int left;
			if ((alignment & TextAlignment.Left) == 0) {
				int width = 0;
				for (int i = 0; i < text.Length; i++) {
					Textures.Texture texture;
					Fonts.OpenGlFontChar data;
					i += font.GetCharacterData(text, i, out texture, out data) - 1;
					width += data.TypographicSize.Width;
				}
				if ((alignment & TextAlignment.Right) != 0) {
					left = location.X - width;
				} else {
					left = location.X - width / 2;
				}
			} else {
				left = location.X;
			}
			int top;
			if ((alignment & TextAlignment.Top) == 0) {
				int height = 0;
				for (int i = 0; i < text.Length; i++) {
					Textures.Texture texture;
					Fonts.OpenGlFontChar data;
					i += font.GetCharacterData(text, i, out texture, out data) - 1;
					if (data.TypographicSize.Height > height) {
						height = data.TypographicSize.Height;
					}
				}
				if ((alignment & TextAlignment.Bottom) != 0) {
					top = location.Y - height;
				} else {
					top = location.Y - height / 2;
				}
			} else {
				top = location.Y;
			}
			/*
			 * Render the string.
			 * */
			Gl.glEnable(Gl.GL_TEXTURE_2D);
			for (int i = 0; i < text.Length; i++) {
				Textures.Texture texture;
				Fonts.OpenGlFontChar data;
				i += font.GetCharacterData(text, i, out texture, out data) - 1;
				if (Textures.LoadTexture(texture, Textures.OpenGlTextureWrapMode.ClampClamp)) {
					Gl.glBindTexture(Gl.GL_TEXTURE_2D, texture.OpenGlTextures[(int)Textures.OpenGlTextureWrapMode.ClampClamp].Name);
					int x = left - (data.PhysicalSize.Width - data.TypographicSize.Width) / 2;
					int y = top - (data.PhysicalSize.Height - data.TypographicSize.Height) / 2;
					/*
					 * In the first pass, mask off the background with pure black.
					 * */
					Gl.glBlendFunc(Gl.GL_ZERO, Gl.GL_ONE_MINUS_SRC_COLOR);
					Gl.glBegin(Gl.GL_POLYGON);
					Gl.glColor4f(color.A, color.A, color.A, 1.0f);
					Gl.glTexCoord2f(data.TextureCoordinates.Left, data.TextureCoordinates.Top);
					Gl.glVertex2f(x, y);
					Gl.glColor4f(color.A, color.A, color.A, 1.0f);
					Gl.glTexCoord2f(data.TextureCoordinates.Right, data.TextureCoordinates.Top);
					Gl.glVertex2f(x + data.PhysicalSize.Width, y);
					Gl.glColor4f(color.A, color.A, color.A, 1.0f);
					Gl.glTexCoord2f(data.TextureCoordinates.Right, data.TextureCoordinates.Bottom);
					Gl.glVertex2f(x + data.PhysicalSize.Width, y + data.PhysicalSize.Height);
					Gl.glColor4f(color.A, color.A, color.A, 1.0f);
					Gl.glTexCoord2f(data.TextureCoordinates.Left, data.TextureCoordinates.Bottom);
					Gl.glVertex2f(x, y + data.PhysicalSize.Height);
					Gl.glEnd();
					/*
					 * In the second pass, add the character onto the background.
					 * */
					Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE);
					Gl.glBegin(Gl.GL_POLYGON);
					Gl.glColor4f(color.R, color.G, color.B, color.A);
					Gl.glTexCoord2f(data.TextureCoordinates.Left, data.TextureCoordinates.Top);
					Gl.glVertex2f(x, y);
					Gl.glColor4f(color.R, color.G, color.B, color.A);
					Gl.glTexCoord2f(data.TextureCoordinates.Right, data.TextureCoordinates.Top);
					Gl.glVertex2f(x + data.PhysicalSize.Width, y);
					Gl.glColor4f(color.R, color.G, color.B, color.A);
					Gl.glTexCoord2f(data.TextureCoordinates.Right, data.TextureCoordinates.Bottom);
					Gl.glVertex2f(x + data.PhysicalSize.Width, y + data.PhysicalSize.Height);
					Gl.glColor4f(color.R, color.G, color.B, color.A);
					Gl.glTexCoord2f(data.TextureCoordinates.Left, data.TextureCoordinates.Bottom);
					Gl.glVertex2f(x, y + data.PhysicalSize.Height);
					Gl.glEnd();
				}
				left += data.TypographicSize.Width;
			}
			Gl.glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA); // HACK //
		}
		
		/// <summary>Renders a string to the screen.</summary>
		/// <param name="font">The font to use.</param>
		/// <param name="text">The string to render.</param>
		/// <param name="location">The location.</param>
		/// <param name="orientation">The orientation.</param>
		/// <param name="color">The color.</param>
		/// <param name="shadow">Whether to draw a shadow.</param>
		/// <remarks>This function sets the OpenGL blend function to glBlendFunc(Gl.GL_SRC_ALPHA, Gl.GL_ONE_MINUS_SRC_ALPHA).</remarks>
		private static void DrawString(Fonts.OpenGlFont font, string text, Point location, TextAlignment alignment, Color128 color, bool shadow) {
			if (shadow) {
				DrawString(font, text, new Point(location.X - 1, location.Y + 1), alignment, new Color128(0.0f, 0.0f, 0.0f, 0.5f * color.A));
				DrawString(font, text, location, alignment, color);
			} else {
				DrawString(font, text, location, alignment, color);
			}
		}
		
	}
}