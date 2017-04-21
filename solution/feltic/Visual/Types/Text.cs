using feltic.Integrator;
using feltic.Library;
using feltic.Visual.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Visual
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
                    totalHeight = GlyphContainer.Font.Metric.VerticalAdvance;
                }
                for (int i = 0; i < String.Length; i++)
                {
                    char _char = String[i];
                    if (_char == ' ')
                    {
                        width += GlyphContainer.Font.Metric.SpaceWidth;
                    }
                    else if (_char == '\t')
                    {
                        width += GlyphContainer.Font.Metric.TabWidth;
                    }
                    else if (_char == '\n')
                    {
                        totalHeight += (GlyphContainer.Font.Metric.VerticalAdvance + GlyphContainer.Font.Metric.LineSpace);
                        width = 0f;
                    }
                    else if (_char == '\r')
                    {;}
                    else
                    {
                        Glyph glyph = GlyphContainer.GetGlyph(String[i]);
                        width += glyph.HoriziontalAdvance;
                    }
                    if (width > maxWidth)
                        maxWidth = width;
                }
                return new Size(maxWidth, totalHeight);
            }
        }

        public void Draw(Position Position, Size Size, Color Color, Position Offset=null, Size Clip=null)
        {
            if (Color == null)
                Color = new Color(220, 220, 200);
            GL.Color3(Color.GetGlColor().Rgb);

            float currentLeft = 0f;
            float currentTop = 0f;
            for (int i = 0; i < String.Length; i++)
            {
                char _char = String[i];
                if(_char == ' ')
                {
                    currentLeft += GlyphContainer.Font.Metric.SpaceWidth;
                }
                else if(_char == '\t')
                {
                    currentLeft += GlyphContainer.Font.Metric.TabWidth;
                }
                else if(_char == '\n')
                {
                    currentTop += (GlyphContainer.Font.Metric.VerticalAdvance + GlyphContainer.Font.Metric.LineSpace);
                    currentLeft = 0f;
                }
                else if (_char == '\r')
                { ; }
                else
                {
                    Glyph glyph = GlyphContainer.GetGlyph(_char);
                    float width = glyph.HoriziontalAdvance;
                    float height = glyph.VerticalAdvance;
                    if (Offset != null && (currentLeft < Offset.X || currentTop < Offset.Y))
                    {
                        currentLeft += width;
                        continue;
                    }
                    float left = (currentLeft - (Offset != null ? Offset.X : 0));
                    float top = (currentTop - (Offset != null ? Offset.Y : 0));
                    if(Clip != null && ((Clip.Width > 0f && left + width > Size.Width) || (Clip.Height > 0f && top + height > Size.Height))){
                        continue;
                    }
                    float glyphX = ((Position.X + left) + glyph.HoriziontalBearingX);
                    float glyphY = ((Position.Y + top) + glyph.VerticalAdvance - glyph.HoriziontalBearingY);
                    glyph.Draw(glyphX, glyphY);
                    currentLeft += width;
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
            this.Color = new Color(220, 220, 220);
        }
    }
}
