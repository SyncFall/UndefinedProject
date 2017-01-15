using Be.Lib;
using Be.Runtime;
using Be.Runtime.Types;
using Be.UI;
using Be.UI.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Be.Integrator
{
    public class BlinkCursor
    {
        public static readonly int BlinkInterval = 1200;
        public long Last;

        public BlinkCursor()
        {
            Reset();
        }

        public void Reset()
        {
            Last = DateTime.Now.Ticks;
        }

        public bool IsBlink()
        {
            long now = DateTime.Now.Ticks;
            long span = (now - Last) / 10000;
            int rest = (int)(span % BlinkInterval);
            int half = (BlinkInterval / 2);
            if(rest <= half)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public class CodeCursor
    {
        public BlinkCursor CursorBlink;
        public CodeText CodeText;
        public TokenContainer TokenContainer;
        public CodeContainer CodeContainer;
        public int LineNumber;
        public int CursorPosition;
        public int CursorPreferedPosition;

        public CodeCursor(CodeText CodeText)
        {
            this.CodeText = CodeText;
            this.TokenContainer = CodeText.TokenContainer;
            this.CodeContainer = CodeText.CodeContainer;
            this.CursorBlink = new BlinkCursor();
        }

        public void SetPosition(int lineNumber, int cursorPosition)
        {
            this.LineNumber = lineNumber;
            this.CursorPosition = cursorPosition;
            EnsureCursor();
            this.CursorPreferedPosition = this.CursorPosition;
        }

        private void EnsureCursor()
        {
            if (LineNumber < 0)
            {
                LineNumber = 0;
            }
            else if (LineNumber >= TokenContainer.LineCount())
            {
                LineNumber = TokenContainer.LineCount() - 1;
            }
            if (CursorPosition < 0)
            {
                CursorPosition = 0;
            }
            else if (CursorPosition > TokenContainer.TextCount(LineNumber))
            {
                CursorPosition = TokenContainer.TextCount(LineNumber);
            }
        }

        public void TextInsert(string insertText)
        {
            TokenContainer.InsertText(LineNumber, CursorPosition, insertText);
            string[] insertLineSplit = insertText.Split('\n');
            SetPosition(LineNumber + insertLineSplit.Length - 1, CursorPosition + insertLineSplit[insertLineSplit.Length - 1].Length);
        }

        public void KeyBackspace()
        {
            TokenContainer.DeleteText(LineNumber, (CursorPosition-1), 1);
            CursorLeft();
        }

        public void KeyDelete()
        {
            TokenContainer.DeleteText(LineNumber, CursorPosition, 1);
        }

        public void KeyEnter()
        {
            TextInsert("\n");
        }
       
        public void CursorLeft()
        {
            if (CursorPosition > 0)
            {
                CursorPosition--;
            }
            else
            {
                if (LineNumber > 0)
                {
                    LineNumber--;
                    CursorPosition = TokenContainer.TextCount(LineNumber);
                }
            }
            CursorPreferedPosition = CursorPosition;
        }

        public void CursorRight()
        {
            if (CursorPosition < TokenContainer.TextCount(LineNumber))
            {
                CursorPosition++;
            }
            else
            {
                if (LineNumber < TokenContainer.LineCount()-1)
                {
                    LineNumber++;
                    CursorPosition = 0;
                }
            }
            CursorPreferedPosition = CursorPosition;
        }

        public void CursorUp()
        {
            if (LineNumber > 0)
            {
                LineNumber--;
                CursorPosition = CursorPreferedPosition;
                if (CursorPosition > TokenContainer.TextCount(LineNumber))
                {
                    CursorPosition = TokenContainer.TextCount(LineNumber);
                }
            }
        }

        public void CursorDown()
        {
            if (LineNumber < TokenContainer.LineCount()-1)
            {
                LineNumber++;
                CursorPosition = CursorPreferedPosition;
                if (CursorPosition > TokenContainer.TextCount(LineNumber))
                {
                    CursorPosition = TokenContainer.TextCount(LineNumber);
                }
            }
        }

        public void Draw()
        {
            GlyphContainer GlyphContainer = CodeText.GlyphContainer;
            GlyphMetrics GlyphMetrics = CodeText.GlyphMetrics;

            float yOffset = GlyphMetrics.TopSpace + ((GlyphMetrics.VerticalAdvance + GlyphMetrics.LineSpace) * LineNumber);
            float xOffset = GlyphMetrics.LeftSpace;

            TokenNode node = TokenContainer.FirstLineTokenNode(LineNumber);
            int cursorIdx = 0;

            while(node != null)
            {
                TokenSymbol token = node.Token;
                for (int tokenIdx = 0; tokenIdx < token.String.Length && cursorIdx < CursorPosition; tokenIdx++, cursorIdx++)
                {
                    char charCode = token.String[tokenIdx];
                    if (charCode == ' ')
                    {
                        xOffset += GlyphMetrics.SpaceWidth;
                    }
                    else if (charCode == '\t')
                    {
                        xOffset += GlyphMetrics.TabWidth;
                    }
                    else
                    {
                        Glyph glyph = GlyphContainer.GetGlyph(charCode);
                        xOffset += glyph.HoriziontalAdvance;
                    }
                }
                node = node.Next;
            }

            float glyphX = xOffset - (float)Math.Ceiling(GlyphMetrics.DelimeterGlyph.Width / 2f);
            float glyphY = yOffset + GlyphMetrics.DelimeterGlyph.VerticalAdvance - GlyphMetrics.DelimeterGlyph.HoriziontalBearingY;
            if (CursorBlink.IsBlink())
            {
                GL.Color3(220 / 255f, 220 / 255f, 220 / 255f);
                GlyphMetrics.DelimeterGlyph.Draw(glyphX, glyphY);
            }
        }
    }

}
