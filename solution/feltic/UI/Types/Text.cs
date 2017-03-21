using feltic.Integrator;
using feltic.Library;
using feltic.UI.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.UI
{
    public class Text
    {
        public string String;
        public static GlyphContainer GlyphContainer;
        public static TextFormat Format;

        public Text(string String, TextFormat f)
        {
            this.String = String;
            if(Format == null)
                Format = new TextFormat();
            if(GlyphContainer == null)
                GlyphContainer = new GlyphContainer(Format.Font);
        }

        public Size Size
        {
            get
            {
                float width = 0f;
                float maxWidth = 0f;
                float totalHeight = 0f;
                if(String.Length > 0)
                {
                    totalHeight = GlyphContainer.Font.Metric.GlyphVerticalAdvance;
                }
                for (int i = 0; i < String.Length; i++)
                {
                    char textChar = String[i];
                    if (textChar == ' ')
                    {
                        width += GlyphContainer.Font.Metric.WhiteSpaceHorizontalAdvance;
                    }
                    else if (textChar == '\t')
                    {
                        width += GlyphContainer.Font.Metric.TabSpaceHorizontalAdvance;
                    }
                    else if (textChar == '\n')
                    {
                        totalHeight += (GlyphContainer.Font.Metric.GlyphVerticalAdvance + GlyphContainer.Font.Metric.LineSpace);
                        width = 0f;
                    }
                    else
                    {
                        Glyph glyph = GlyphContainer.GetGlyph(String[i]);
                        width += glyph.HoriziontalAdvance;
                    }
                    if (width > maxWidth)
                    {
                        maxWidth = width;
                    }
                }
                return new Size(maxWidth, totalHeight);
            }
        }

        //Color same = new Color(220, 220, 200);
        public void Draw(Color Color, float X=0, float Y=0, float OffsetX=0, float OffsetY=0, float Width=0, float Height=0)
        {
            float currentX = X;
            float currentY = Y;
            if(Color == null)   Color = new Color(220, 220, 200);
            GL.Color3(Color.GetGlColor().Rgb);
            //GL.Color3(same.GetGlColor().Rgb);
            for (int i = 0; i < String.Length; i++)
            {
                char textChar = String[i];
                if(textChar == ' ')
                {
                    currentX += GlyphContainer.Font.Metric.WhiteSpaceHorizontalAdvance;
                }
                else if(textChar == '\t')
                {
                    currentX += GlyphContainer.Font.Metric.TabSpaceHorizontalAdvance;
                }
                else if(textChar == '\n')
                {
                    currentY += (GlyphContainer.Font.Metric.GlyphVerticalAdvance + GlyphContainer.Font.Metric.LineSpace);
                    currentX = X;
                }
                else
                {
                    Glyph glyph = GlyphContainer.GetGlyph(textChar);
                    if(currentX < X + OffsetX)
                    {
                        currentX += glyph.HoriziontalAdvance;
                        continue;
                    }
                    if(currentY < Y + OffsetY) continue; 
                    if(Width > 0 && currentX - X - OffsetX + glyph.HoriziontalAdvance > Width) continue;
                    if(Height > 0 && currentY - Y - OffsetY + glyph.VerticalAdvance > Height) break;
                    float glyphX = (currentX - OffsetX + glyph.HoriziontalBearingX);
                    float glyphY = (currentY - OffsetY + glyph.VerticalAdvance - glyph.HoriziontalBearingY);
                    glyph.Draw(glyphX, glyphY);
                    currentX += glyph.HoriziontalAdvance;
                }
            }
        }
    }

    public class TextFormat
    {
        public Font Font;
        public float Size;
        public Color Color;

        public TextFormat()
        {
            this.Font = new Font("D:\\dev\\UndefinedProject\\output\\DroidSansMono.ttf");
            //this.Color = new Color(0, 100, 150);
            this.Color = new Color(220, 220, 220);
        }
    }
}
