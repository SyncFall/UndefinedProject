using feltic.Language;
using feltic.UI;
using feltic.UI.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Text;

namespace feltic.Integrator
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

        private CodeCursor()
        { }

        public CodeCursor(CodeText CodeText)
        {
            this.CodeText = CodeText;
            this.TokenContainer = CodeText.TokenContainer;
            this.CodeContainer = CodeText.CodeContainer;
            this.CursorBlink = new BlinkCursor();
        }

        public CodeCursor Clone()
        {
            CodeCursor clone = new CodeCursor();
            clone.LineNumber = LineNumber;
            clone.CursorPosition = CursorPosition;
            clone.CursorPreferedPosition = CursorPreferedPosition;
            return clone;
        }

        public void Bind(CodeCursor Bind)
        {
            LineNumber = Bind.LineNumber;
            CursorPosition = Bind.CursorPosition;
            CursorPreferedPosition = Bind.CursorPreferedPosition;
        }

        public void SetPosition(int lineNumber, int cursorPosition)
        {
            this.LineNumber = lineNumber;
            this.CursorPosition = cursorPosition;
            EnsureCursor();
            this.CursorPreferedPosition = this.CursorPosition;
        }

        public void Paste()
        {
            string insertText = Clipboard.GetText();
            if (insertText != null && insertText.Length > 0)
            {
                TextInsert(insertText);
            }
        }

        public void Copy()
        {
            CodeSelection selection = CodeText.CodeSelection.GetOrdered();
            if (selection.HasSelection())
            {
                string sourceText = CodeText.SourceText.StringContent;
                StringBuilder strBuilder = new StringBuilder();
                int line = 0, cursor = 0;
                bool copy = false;
                for (int i = 0; i < sourceText.Length; i++, cursor++)
                {
                    if (line == selection.BeginPart.LinePosition && cursor == selection.BeginPart.CursorPosition)
                    {
                        copy = true;
                    }
                    else if(line == selection.EndPart.LinePosition && cursor == selection.EndPart.CursorPosition)
                    {
                        copy = false;
                    }
                    if(copy)
                    {
                        strBuilder.Append(sourceText[i]);
                    }
                    if(sourceText[i] == '\n')
                    {
                        line++;
                        cursor = -1;
                    }
                }
                Clipboard.SetText(strBuilder.ToString().Replace("\n", "\r\n"));
            }
        }

        public void Cut()
        {
            CodeSelection selection = CodeText.CodeSelection.GetOrdered();
            if (selection.HasSelection())
            {
                string sourceText = CodeText.SourceText.StringContent;
                StringBuilder strBuilder = new StringBuilder(sourceText.Length);
                StringBuilder strBuilderCut = new StringBuilder();
                int line = 0, cursor = 0;
                bool cut = false;
                for (int i = 0; i < sourceText.Length; i++, cursor++)
                {
                    if (line == selection.BeginPart.LinePosition && cursor == selection.BeginPart.CursorPosition)
                    {
                        cut = true;
                    }
                    else if (line == selection.EndPart.LinePosition && cursor == selection.EndPart.CursorPosition)
                    {
                        cut = false;
                    }
                    if (cut)
                    {
                        strBuilderCut.Append(sourceText[i]);
                    }
                    else
                    {
                        strBuilder.Append(sourceText[i]);
                    }
                    if (sourceText[i] == '\n')
                    {
                        line++;
                        cursor = -1;
                    }
                }
                Clipboard.SetText(strBuilderCut.ToString().Replace("\n", "\r\n"));
                CodeText.CodeSelection.Clear();
                CodeText.SetSourceText(CodeText.SourceText.SetText(strBuilder.ToString()));
                SetPosition(selection.BeginPart.LinePosition, selection.BeginPart.CursorPosition);
            }
        }

        public void SetCursor(int CursorX, int CursorY)
        {
            VisualElement VisualCode = CodeText.VisualCode;
            if (VisualCode == null) return;

            FontMetric fontMetric = Text.GlyphContainer.Font.Metric;

            Position start = CodeText.CodeContainer.Start;
            if (start == null) return;

            float cursorOffset = (CursorY - start.y);
            float scrollOffset = (VisualCode.Parent as VisualScrollElement).ScrollYPosition;
            float scrollHeight = (VisualCode.Parent as VisualScrollElement).Size.Height;
            float codeHeight = (VisualCode.Size.Height);
            float factorHeight = (codeHeight / scrollHeight);
            float offsetHeight = (scrollOffset * factorHeight);
           
            int lineNumber = (int)((offsetHeight + cursorOffset) / (fontMetric.VerticalAdvance + fontMetric.LineSpace));

            int viewLineNumber = (int)(cursorOffset / (fontMetric.VerticalAdvance + fontMetric.LineSpace));

            if (lineNumber < 0)
            {
                lineNumber = 0;
            }
            else if(lineNumber > CodeText.TokenContainer.LineCount()-1)
            {
                lineNumber = CodeText.TokenContainer.LineCount()-1;
            }

            string lineText = CodeText.TokenContainer.LineText(lineNumber);
            float currentX = start.x;
            bool cursorSet = false;
            for (int i = 0; i < lineText.Length; i++)
            {
                char charCode = lineText[i];
                if (charCode == ' ')
                {
                    currentX += fontMetric.SpaceWidth;
                }
                else if (charCode == '\t')
                {
                    currentX += fontMetric.TabWidth;
                }
                else
                {
                    Glyph glyph = Text.GlyphContainer.GetGlyph(charCode);
                    currentX += glyph.HoriziontalAdvance;
                }
                if (currentX > CursorX)
                {
                    CodeText.CodeCursor.SetPosition(lineNumber, i);
                    cursorSet = true;
                    break;
                }
            }
            if (!cursorSet)
            {
                CodeText.CodeCursor.SetPosition(lineNumber, CodeText.TokenContainer.TextCount(lineNumber));
                cursorSet = true;
            }
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
            insertText = insertText.Replace("\r", "");
            CodeSelection selection = CodeText.CodeSelection.GetOrdered();
            string sourceText = CodeText.SourceText.StringContent;
            StringBuilder strBuilder = new StringBuilder(sourceText.Length);
            bool isTab = (insertText == "\t");
            if (selection.HasSelection())
            {
                int line=0, cursor=0;
                bool append = true;
                for (int i=0; i<sourceText.Length+1; i++, cursor++)
                {
                    if(isTab && line >= selection.BeginPart.LinePosition && line <= selection.EndPart.LinePosition && cursor == 0)
                    {
                        strBuilder.Append("\t");
                        append = true;
                    }
                    else if(!isTab && selection.BeginPart.LinePosition == line && selection.BeginPart.CursorPosition == cursor)
                    {
                        strBuilder.Append(insertText);
                        append = false;
                    }
                    else if (!isTab && selection.EndPart.LinePosition == line && selection.EndPart.CursorPosition == cursor)
                    {
                        append = true;
                    }
     
                    if(i < sourceText.Length)
                    {
                        if (append)
                        {
                            strBuilder.Append(sourceText[i]);
                        }
                        if (sourceText[i] == '\n')
                        {
                            line++;
                            cursor = -1;
                        }
                    }
                }
            }
            else
            {
                int line=0, cursor=0;
                for (int i = 0; i < sourceText.Length+1; i++, cursor++)
                {
                    if(LineNumber == line && CursorPosition == cursor)
                    {
                        strBuilder.Append(insertText);
                    }
                    if(i < sourceText.Length)
                    {
                        if (sourceText[i] == '\n')
                        {
                            line++;
                            cursor = -1;
                        }
                        strBuilder.Append(sourceText[i]);
                    }
                }
            }
            string[] insertLines = insertText.Split('\n');
            int newLine = (LineNumber + insertLines.Length-1);
            int newCursor = ((LineNumber == newLine ? CursorPosition : 0) + insertLines[insertLines.Length-1].Length);
            CodeText.SetSourceText(CodeText.SourceText.SetText(strBuilder.ToString()));
            CodeText.CodeSelection.Clear();
            SetPosition(newLine, newCursor);
        }

        public void KeyBackspace()
        {
            if(LineNumber == 0 && CursorPosition == 0)
            {
                return;
            }
            CodeSelection selection = CodeText.CodeSelection.GetOrdered();
            string sourceText = CodeText.SourceText.StringContent;
            StringBuilder strBuilder = new StringBuilder(sourceText.Length);
            if(selection.HasSelection())
            {
                int line = 0, cursor = 0;
                bool append = true;
                for (int i = 0; i < sourceText.Length+1; i++, cursor++)
                {
                    if (selection.BeginPart.LinePosition == line && selection.BeginPart.CursorPosition == cursor)
                    {
                        append = false;
                    }
                    else if (selection.EndPart.LinePosition == line && selection.EndPart.CursorPosition == cursor)
                    {
                        append = true;
                    }
                    if(i < sourceText.Length)
                    {
                        if (append)
                        {
                            strBuilder.Append(sourceText[i]);
                        }
                        if (sourceText[i] == '\n')
                        {
                            line++;
                            cursor = -1;
                        }
                    }
                }
                SetPosition(selection.BeginPart.LinePosition, selection.BeginPart.CursorPosition);
            }
            else
            {
                int line=0, cursor=0;
                for (int i = 0; i < sourceText.Length+1; i++, cursor++)
                {
                    if (line == LineNumber && CursorPosition == cursor)
                    {
                        strBuilder.Remove(i - 1, 1);
                    }
                    if(i < sourceText.Length)
                    {
                        if (sourceText[i] == '\n')
                        {
                            line++;
                            cursor = -1;
                        }
                        strBuilder.Append(sourceText[i]);
                    }
                }
                CursorLeft();
            }
            CodeText.SetSourceText(CodeText.SourceText.SetText(strBuilder.ToString()));
            CodeText.CodeSelection.Clear();
        }

        public void KeyDelete()
        {
            CodeSelection selection = CodeText.CodeSelection.GetOrdered();
            string sourceText = CodeText.SourceText.StringContent;
            StringBuilder strBuilder = new StringBuilder(sourceText.Length);
            if (selection.HasSelection())
            {
                int line = 0, cursor = 0;
                bool append = true;
                for (int i = 0; i < sourceText.Length; i++, cursor++)
                {
                    if (selection.BeginPart.LinePosition == line && selection.BeginPart.CursorPosition == cursor)
                    {
                        append = false;
                    }
                    else if (selection.EndPart.LinePosition == line && selection.EndPart.CursorPosition == cursor)
                    {
                        append = true;
                    }
                    if (append)
                    {
                        strBuilder.Append(sourceText[i]);
                    }
                    if (sourceText[i] == '\n')
                    {
                        line++;
                        cursor = -1;
                    }
                }
                CodeText.SetSourceText(CodeText.SourceText.SetText(strBuilder.ToString()));
                CodeText.CodeSelection.Clear();
                SetPosition(selection.BeginPart.LinePosition, selection.BeginPart.CursorPosition);
            }
            else
            {
                int line = 0, cursor = 0;
                for (int i = 0; i < sourceText.Length; i++, cursor++)
                {
                    strBuilder.Append(sourceText[i]);
                    if (line == LineNumber && CursorPosition == cursor && i + 1 < sourceText.Length)
                    {
                        strBuilder.Remove(i, 1);
                    }
                    if (sourceText[i] == '\n')
                    {
                        line++;
                        cursor = -1;
                    }
                }
                CodeText.SetSourceText(CodeText.SourceText.SetText(strBuilder.ToString()));
                CodeText.CodeSelection.Clear();
            }
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
            VisualElement VisualCode = CodeText.VisualCode;
            if (VisualCode == null) return;

            if (!CodeText.CodeInput.Active) return;

            FontMetric fontMetric = Text.GlyphContainer.Font.Metric;

            Position start = CodeText.CodeContainer.Start;
            if (start == null) return;


            float yOffset = start.y + ((fontMetric.VerticalAdvance + fontMetric.LineSpace) * LineNumber);
            float xOffset = start.x;

 
            float scrollOffset = (VisualCode.Parent as VisualScrollElement).ScrollYPosition;
            float scrollHeight = (VisualCode.Parent as VisualScrollElement).Size.Height;
            float codeHeight = (VisualCode.Size.Height);
            float factorHeight = (codeHeight / scrollHeight);
            float offsetHeight = (scrollOffset * factorHeight);


            yOffset = start.y + (((fontMetric.VerticalAdvance + fontMetric.LineSpace) * (LineNumber)) - (offsetHeight));

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
                        xOffset += fontMetric.SpaceWidth;
                    }
                    else if (charCode == '\t')
                    {
                        xOffset += fontMetric.TabWidth;
                    }
                    else
                    {
                        Glyph glyph = Text.GlyphContainer.GetGlyph(charCode);
                        xOffset += glyph.HoriziontalAdvance;
                    }
                }
                node = node.Next;
            }

            float glyphX = xOffset - (float)Math.Ceiling(fontMetric.Delimeter.Width / 2f);
            float glyphY = yOffset + fontMetric.Delimeter.VerticalAdvance - fontMetric.Delimeter.HoriziontalBearingY;
            if (CursorBlink.IsBlink())
            {
                GL.Color3(220 / 255f, 220 / 255f, 220 / 255f);
                fontMetric.Delimeter.Draw(glyphX, glyphY-3);
            }
        }
    }

}
