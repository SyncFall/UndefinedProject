using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Be.UI.Types;
using Be.Runtime;
using Be.Runtime.Types;
using OpenTK.Graphics.OpenGL;
using Be.UI;

namespace Be.Integrator
{
    public class CodeText
    {
        public static readonly int DefaultFontSize = 10;
        public static readonly int DefaultTopSpace = 5;
        public static readonly int DefaultLeftSpace = 5;

        public Font SourceFont;
        public SourceFile SourceFile;
        public TokenContainer TokenContainer;
        public SymbolContainer SymbolContainer;
        public GlyphMetrics GlyphMetrics;
        public GlyphContainer GlyphContainer;
        public CodeColor CodeColor;
        public CodeContainer CodeContainer;
        public CodeCursor CodeCursor;
        public CodeInput CodeInput;
        public CodeSelection CodeSelection;
        public CodeHistory CodeHistory;
        
        public CodeText(SourceFile SourceFile)
        {
            this.SourceFont = new Font(@"D:\dev\UndefinedProject\be-output\source-code-pro-regular.ttf", DefaultFontSize);
            this.SourceFile = SourceFile;
            this.TokenContainer = new TokenContainer();
            this.SymbolContainer = new SymbolContainer();
            this.GlyphMetrics = new GlyphMetrics(SourceFont, DefaultTopSpace, DefaultLeftSpace);
            this.GlyphContainer = new GlyphContainer(SourceFont);
            this.CodeColor = new CodeColor();
            this.CodeContainer = new CodeContainer(this);
            this.CodeCursor = new CodeCursor(this);
            this.CodeInput = new CodeInput(this);
            this.CodeSelection = new CodeSelection(this);
            this.CodeHistory = new CodeHistory(this);
            this.SetSourceFile(SourceFile);
        }

        public void SetSourceFile(SourceFile SourceFile)
        {
            this.SourceFile = SourceFile;
            this.TokenContainer.SetSourceFile(SourceFile);
            this.CodeContainer.SetTokenContainer(TokenContainer);
            this.SymbolContainer.Operate(TokenContainer);
        }
       
        public void Draw()
        {
            CodeSelection.Draw();
            CodeContainer.Draw();
            CodeCursor.Draw();
        }
    }

    public class GlyphMetrics
    {
        public Font Font;
        public Glyph SpaceGlyph;
        public Glyph DelimeterGlyph;
        public int LeftSpace;
        public int TopSpace;
        public int SpaceWidth;
        public int TabWidth;
        public int VerticalAdvance;
        public int LineSpace;
        
        public GlyphMetrics(Font font, int topSpace, int leftSpace)
        {
            this.Font = font;
            this.SpaceGlyph = Font.GetGlyph(' ');
            this.DelimeterGlyph = Font.GetGlyph('|');
            this.TopSpace = topSpace;
            this.LeftSpace = leftSpace;
            this.SpaceWidth = (int)SpaceGlyph.HoriziontalAdvance;
            this.TabWidth = (SpaceWidth * 4);
            this.VerticalAdvance = (int)SpaceGlyph.VerticalAdvance;
            this.LineSpace = (int)(DelimeterGlyph.Height - DelimeterGlyph.VerticalAdvance);
        }
    }
}
