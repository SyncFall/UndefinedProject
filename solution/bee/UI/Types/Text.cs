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
        public GlyphContainer GlyphContainer;
        public TextFormat Format;

        public Text(string String, TextFormat Format)
        {
            this.String = String;
            if(Format == null)
                this.Format = new TextFormat();
            else
                this.Format = Format;
            this.GlyphContainer = new GlyphContainer(this.Format.Font);
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
       
        public void Draw(float X=0, float Y=0)
        {
            float currentX = X;
            float currentY = Y;
            GL.Color3(Format.Color.GetGlColor().Rgb);
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
                    float glyphX = (currentX + glyph.HoriziontalBearingX);
                    float glyphY = (currentY + glyph.VerticalAdvance - glyph.HoriziontalBearingY);
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
            this.Font = new Font("DroidSansMono.ttf");
            this.Color = new Color(0, 100, 150);
        }
    }
}
