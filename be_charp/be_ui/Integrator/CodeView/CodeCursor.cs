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
        public TokenContainer TokenOperator;
        public CodeContainer CodeContainer;
        public int LinePosition;
        public int CursorPosition;
        public int CursorPreferedPosition;

        public CodeCursor(CodeText CodeText)
        {
            this.CodeText = CodeText;
            this.TokenOperator = CodeText.TokenContainer;
            this.CodeContainer = CodeText.CodeContainer;
            this.CursorBlink = new BlinkCursor();
        }

        public void SetPosition(int linePosition, int cursorPosition)
        {
            this.LinePosition = linePosition;
            this.CursorPosition = cursorPosition;
            EnsureCursor();
            this.CursorPreferedPosition = this.CursorPosition;
        }

        public TokenLine GetLine(int linePosition)
        {
            return TokenOperator.TokenLines.Get(linePosition);
        }

        public int LineCount()
        {
            return TokenOperator.TokenLines.Size();
        }

        public void TextInsert(string inputText)
        {
            int textIdx = 0;
            for(int i=0; i<LineCount(); i++)
            {
                if(i < LinePosition)
                {
                    textIdx += GetLine(i).TextCount + 1;
                    continue;
                }
                else
                {
                    textIdx += CursorPosition;
                    break;
                }
            }
            string sourceText = CodeText.SourceFile.Source;
            string newSourceText = sourceText.Substring(0, textIdx) + inputText + sourceText.Substring(textIdx);
            CodeText.SourceFile.Source = newSourceText;
            CodeText.SetSourceFile(CodeText.SourceFile);
            CursorPosition += inputText.Length;
            CursorBlink.Reset();
        }

        public void KeyBackspace()
        {
            if(CursorPosition == 0 && LinePosition == 0)
            {
                return;
            }
            int textIdx = 0;
            for (int i = 0; i < LineCount(); i++)
            {
                if (i < LinePosition)
                {
                    textIdx += GetLine(i).TextCount + 1;
                    continue;
                }
                else
                {
                    textIdx += CursorPosition;
                    break;
                }
            }
            if (CursorPosition == 0)
            {
                LinePosition--;
                CursorPosition = GetLine(LinePosition).TextCount;
            }
            else
            {
                CursorPosition--;
            }
            string sourceText = CodeText.SourceFile.Source;
            string newSourceText = sourceText.Substring(0, textIdx - 1)  + sourceText.Substring(textIdx);
            CodeText.SourceFile.Source = newSourceText;
            CodeText.SetSourceFile(CodeText.SourceFile);
            CursorBlink.Reset();
        }

        public void KeyDelete()
        {
            /*
            EnsureCursor();
            CodeLine codeLine = CodeContainer.Get(LinePosition);
            codeLine.Text = codeLine.Text.Substring(0, CursorPosition) + codeLine.Text.Substring(CursorPosition+1);
            SetPosition(LinePosition, CursorPosition);
            */CursorBlink.Reset();
        }

        public void KeyEnter()
        {
            EnsureCursor();
            TextInsert("\n");
            LinePosition++;
            CursorPosition = 0;
            CursorBlink.Reset();
        }
       
        public void CursorLeft()
        {
            if (CursorPosition > 0)
            {
                CursorPosition--;
            }
            else
            {
                if (LinePosition > 0)
                {
                    LinePosition--;
                    TokenLine tokenLine = GetLine(LinePosition);
                    CursorPosition = tokenLine.TextCount;
                }
            }
            CursorPreferedPosition = CursorPosition;
            CursorBlink.Reset();
        }

        public void CursorRight()
        {
            TokenLine tokenLine = GetLine(LinePosition);
            if (CursorPosition < tokenLine.TextCount)
            {
                CursorPosition++;
            }
            else
            {
                if (LinePosition < LineCount() - 1)
                {
                    LinePosition++;
                    CursorPosition = 0;
                }
            }
            CursorPreferedPosition = CursorPosition;
            CursorBlink.Reset();
        }

        public void CursorUp()
        {
            if (LinePosition > 0)
            {
                LinePosition--;
                TokenLine tokenLine = GetLine(LinePosition);
                CursorPosition = CursorPreferedPosition;
                if (CursorPosition > tokenLine.TextCount)
                {
                    CursorPosition = tokenLine.TextCount;
                }
            }
            CursorBlink.Reset();
        }

        public void CursorDown()
        {
            if (LinePosition < LineCount() - 1)
            {
                LinePosition++;
                TokenLine tokenLine = GetLine(LinePosition);
                CursorPosition = CursorPreferedPosition;
                if (CursorPosition > LineCount())
                {
                    CursorPosition = LineCount();
                }
            }
            CursorBlink.Reset();
        }

        private void EnsureCursor()
        {
            if(LinePosition < 0)
            {
                LinePosition = 0;
            }
            else if(LinePosition >= LineCount())
            {
                LinePosition = LineCount() - 1;
            }
            TokenLine tokenLine = GetLine(LinePosition);
            if(CursorPosition < 0)
            {
                CursorPosition = 0;
            }
            else if(CursorPosition > LineCount())
            {
                CursorPosition = LineCount();
            }
        }

        public void Draw()
        {
            GlyphContainer GlyphContainer = CodeText.GlyphContainer;
            GlyphMetrics GlyphMetrics = CodeText.GlyphMetrics;

            float yOffset = GlyphMetrics.TopSpace + ((GlyphMetrics.VerticalAdvance + GlyphMetrics.LineSpace) * LinePosition);
            float xOffset = GlyphMetrics.LeftSpace;
            TokenLine tokenLine = GetLine(LinePosition);
            TokenSymbol token = null;
            int cursorIdx = 0;
            for(int i=0; i<tokenLine.Size(); i++)
            {
                token = tokenLine.Get(i);
                for(int tokenIdx=0; tokenIdx < token.TextCount && cursorIdx < CursorPosition; tokenIdx++, cursorIdx++)
                {
                    Glyph glyph = GlyphContainer.GetGlyph(token.TextString[tokenIdx]);
                    if (glyph.CharCode == ' ')
                    {
                        xOffset += GlyphMetrics.SpaceWidth;
                    }
                    else if (glyph.CharCode == '\t')
                    {
                        xOffset += GlyphMetrics.TabWidth;
                    }
                    else
                    {
                        xOffset += glyph.HoriziontalAdvance;
                    }
                }
                if(cursorIdx == CursorPosition)
                {
                    break;
                }
            }

            float glyphX = xOffset - (float)Math.Ceiling(GlyphMetrics.DelimeterGlyph.Width / (float)2);
            float glyphY = yOffset + GlyphMetrics.DelimeterGlyph.VerticalAdvance - GlyphMetrics.DelimeterGlyph.HoriziontalBearingY;
            if (CursorBlink.IsBlink())
            {
                GL.Color3(220 / 255f, 220 / 255f, 220 / 255f);
                GlyphMetrics.DelimeterGlyph.Draw(glyphX, glyphY);
            }

            if (token != null && token.Status != null && token.Status.Type == TokenStatus.Error)
            {
                CodeMessage.Draw(token.Status, glyphX + 20, glyphY + 20, CodeText);
            }
        }
    }

}
