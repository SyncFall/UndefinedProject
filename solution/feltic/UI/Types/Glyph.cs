using feltic.UI.Types;
using feltic.Library;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.UI
{
    public class GlyphContainer
    {
        public Font Font;
        public MapCollection<char, Glyph> Map = new MapCollection<char, Glyph>();

        public GlyphContainer(Font font)
        {
            this.Font = font;
        }

        public Glyph GetGlyph(char chr)
        {
            if (Map.KeyExist(chr))
            {
                return Map.GetValue(chr);
            }
            else
            {
                Glyph glyph = Font.GetGlyph(chr);
                Map.Put(chr, glyph);
                return glyph;
            }
        }

        public void Draw(string Text, float X, float Y)
        {
            float currentX = X;
            float currentY = Y;
            for (int i = 0; i < Text.Length; i++)
            {
                Glyph glyph = this.GetGlyph(Text[i]);
                if (glyph.CharCode == '\n')
                {
                    currentX = 0;
                    currentY += (glyph.VerticalAdvance * 1.2f);
                }
                else
                {
                    float glyphX = (currentX + glyph.HoriziontalBearingX);
                    float glyphY = (currentY + glyph.VerticalAdvance - glyph.HoriziontalBearingY);
                    glyph.Draw(glyphX, glyphY);
                    currentX += glyph.HoriziontalAdvance;
                }
            }
        }
    }

    public class Glyph
    {
        public Font Font;
        public char CharCode;

        public float Width;
        public float Height;
        public float HoriziontalAdvance;
        public float HoriziontalBearingX;
        public float HoriziontalBearingY;
        public float VerticalAdvance;
        public float VerticalBearingY;
        public float VerticalBearingX;

        public byte[] BitmapRgba;
        public byte[] BitmapAlpha;
        public int BitmapBufferId;

        public ListCollection<Curve> Outlines = new ListCollection<Curve>();

        public Glyph(Font Font, char CharCode)
        {
            this.Font = Font;
            this.CharCode = CharCode;
        }

        public void Draw(float X, float Y)
        {
            // bitmap
            if (BitmapRgba != null || BitmapAlpha != null)
            {
                // create gpu-texture-buffer
                if (BitmapBufferId == 0)
                {
                    if (BitmapRgba != null)
                    {
                        BitmapBufferId = GL.GenTexture();
                        GL.BindTexture(TextureTarget.Texture2D, BitmapBufferId);
                        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, (int)Width, (int)Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, Utils.ByteArrayToIntPtr(BitmapRgba));
                    }
                    else if (BitmapAlpha != null)
                    {
                        BitmapBufferId = GL.GenTexture();
                        GL.BindTexture(TextureTarget.Texture2D, BitmapBufferId);
                        GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)TextureWrapMode.ClampToEdge);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)TextureWrapMode.ClampToEdge);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)TextureMinFilter.Linear);
                        GL.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)TextureMagFilter.Linear);
                        GL.TexImage2D(TextureTarget.Texture2D, 0, PixelInternalFormat.Rgba, (int)Width, (int)Height, 0, PixelFormat.LuminanceAlpha, PixelType.UnsignedByte, Utils.ByteArrayToIntPtr(BitmapAlpha));
                    }
                }
                GL.BindTexture(TextureTarget.Texture2D, BitmapBufferId);
                GL.Begin(PrimitiveType.Quads);
                GL.TexCoord2(0, 0); GL.Vertex2(X, Y);
                GL.TexCoord2(1, 0); GL.Vertex2(X + Width, Y);
                GL.TexCoord2(1, 1); GL.Vertex2(X + Width, Y + Height);
                GL.TexCoord2(0, 1); GL.Vertex2(X, Y + Height);
                GL.End();
                GL.BindTexture(TextureTarget.Texture2D, 0);
            }
            // outline
            else if (Outlines.Size > 0)
            {
                for (int i = 0; i < Outlines.Size; i++)
                {
                    Outlines[i].Draw();
                }
            }
            /*
            GL.Begin(PrimitiveType.LineStrip);
            GL.Vertex2(0, 0);
            GL.Vertex2(0 + HoriziontalAdvance, 0);
            GL.Vertex2(0 + HoriziontalAdvance, 0 + Height);
            GL.Vertex2(0, 0 + Height);
            GL.Vertex2(0, 0);
            GL.End();
            */
        }
    }
}
