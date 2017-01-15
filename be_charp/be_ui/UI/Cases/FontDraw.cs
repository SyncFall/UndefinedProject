using Be.UI.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.UI.Cases
{
    public class FontDraw
    {
        public WindowType WindowType;
        public Font Font;
        public Glyph Glyph;
        public GlyphContainer GlyphCollection;
        public string DisplayText = "Fuck Silicon NS and Aln!";

        public FontDraw(WindowType WindowType)
        {
            this.WindowType = WindowType;
            this.Font = new Font(@"D:\dev\UndefinedProject\be-output\verdana.ttf");
            this.Glyph = Font.GetGlyph('g');
            this.GlyphCollection = new GlyphContainer(this.Font);
        }

        public void Draw()
        {
            //Glyph.Draw();
            GlyphCollection.Draw(DisplayText, 20, 20);
        }
    }
}
