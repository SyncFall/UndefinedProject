using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using feltic.UI.Types;
using feltic.Language;
using OpenTK.Graphics.OpenGL;
using feltic.UI;
using feltic.Language;

namespace feltic.Integrator
{
    public class CodeText
    {
        public static readonly int DefaultFontSize = 10;
        public static readonly int DefaultTopSpace = 5;
        public static readonly int DefaultLeftSpace = 5;

        public Font SourceFont;
        public SourceText SourceText;
        public Registry Registry;
        public GlyphMetrics GlyphMetrics;
        public GlyphContainer GlyphContainer;
        public CodeColor CodeColor;
        public CodeContainer CodeContainer;
        public CodeCursor CodeCursor;
        public CodeInput CodeInput;
        public CodeSelection CodeSelection;
        public CodeHistory CodeHistory;
        public TokenContainer TokenContainer;
        public VisualElement VisualRoot;

        public CodeText()
        {
            this.SourceFont = new Font("DroidSansMono.ttf", DefaultFontSize);
            this.SourceText = SourceText.FromFile("language.src");
            SourceList list = new SourceList();
            list.Add(this.SourceText);
            this.Registry = new Registry();
            this.Registry.AddSourceList(list);
            this.TokenContainer = Registry.EntryList.GetExist(SourceText).TokenContainer;
            this.GlyphMetrics = new GlyphMetrics(SourceFont, DefaultTopSpace, DefaultLeftSpace);
            this.GlyphContainer = new GlyphContainer(SourceFont);
            this.CodeColor = new CodeColor();
            this.CodeContainer = new CodeContainer(this);
            this.CodeContainer.SetContainer(Registry.EntryList.GetExist(SourceText).TokenContainer);
            this.CodeCursor = new CodeCursor(this);
            this.CodeInput = new CodeInput(this);
            this.CodeSelection = new CodeSelection(this);
            this.CodeHistory = new CodeHistory(this);
        }

        public void SetSourceText(SourceText Source)
        {
            this.SourceText = Source;
            this.CodeContainer.SetContainer(Registry.EntryList.GetExist(SourceText).TokenContainer);
            this.Registry.UpdateSource(SourceText);
        }
       
        public void Draw()
        {
            CodeSelection.Draw();
            //CodeContainer.Draw();
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
            this.TabWidth = (SpaceWidth * 2);
            this.VerticalAdvance = (int)SpaceGlyph.VerticalAdvance;
            this.LineSpace = (int)(DelimeterGlyph.VerticalAdvance * 0.25);// (int)((DelimeterGlyph.Height - DelimeterGlyph.VerticalAdvance));
        }
    }
}
