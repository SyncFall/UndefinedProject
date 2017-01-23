using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI
{ 
    public class TextContainer
    {
        public string Text;
        public GlColor3 GlColor;
        public Glyph[] Glyphs;
        public GlyphContainer GlyphContainer;
        public float HorizontalAdvance;
        public float VerticalAdvance;

        public TextContainer(string Text, Color Color, GlyphContainer GlyphContainer)
        {
            this.Text = Text;
            this.GlColor = Color.GetGlColor3();
            this.Glyphs = new Glyph[Text.Length];
            this.VerticalAdvance = GlyphContainer.Font.Metric.GlyphVerticalAdvance;
            for(int i=0; i<Text.Length; i++)
            {
                char charCode = Text[i];
                if (charCode == ' ')
                {
                    this.HorizontalAdvance += GlyphContainer.Font.Metric.WhiteSpaceHorizontalAdvance;
                }
                else if (charCode == '\t')
                {
                    this.HorizontalAdvance += GlyphContainer.Font.Metric.TabSpaceHorizontalAdvance;
                }
                else if (charCode == '\n')
                {
                    throw new Exception("new-line text-glyph not allowed");
                }
                else
                {
                    Glyph glyph = GlyphContainer.GetGlyph(charCode);
                    this.Glyphs[i] = glyph;
                    this.HorizontalAdvance += glyph.HoriziontalAdvance;
                }
            }
        }

        public void Draw(float x, float y)
        {
            GL.Color3(GlColor.Rgb);
            for (int i = 0; i < Text.Length; i++)
            {
                char charCode = Text[i];
                if (charCode == ' ')
                {
                    x += GlyphContainer.Font.Metric.WhiteSpaceHorizontalAdvance;
                }
                else if (charCode == '\t')
                {
                    y += GlyphContainer.Font.Metric.TabSpaceHorizontalAdvance;
                }
                else if (charCode == '\n')
                {
                    throw new Exception("new-line text-glyph not allowed");
                }
                else
                {
                    Glyph glyph = Glyphs[i];
                    float glyphX = (x + glyph.HoriziontalBearingX);
                    float glyphY = (y + glyph.VerticalAdvance - glyph.HoriziontalBearingY);
                    glyph.Draw(glyphX, glyphY);
                    x += glyph.HoriziontalAdvance;
                }
            }
        }
    }
}
