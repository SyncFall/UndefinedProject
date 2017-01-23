using Bee.UI.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bee.Runtime;
using Bee.UI;

namespace Bee.Integrator
{ 
    public class CodeSelection
    {
        public CodeText CodeText;
        public int StartLinePosition;
        public int StartCursorPosition;
        public int EndLinePosition;
        public int EndCursorPosition;
        public bool HasSelectionBegin;
        public bool HasSelectionEnd;

        public CodeSelection(CodeText CodeText)
        {
            this.CodeText = CodeText;
            Begin(CodeText.CodeCursor.LineNumber, CodeText.CodeCursor.CursorPosition);
        }

        public void Begin(int LinePosition, int CursorPosition)
        {
            Clear();
            StartLinePosition = LinePosition;
            StartCursorPosition = CursorPosition;
            HasSelectionBegin = true;
        }

        public void End(int LinePosition, int CursorPosition)
        {
            EndLinePosition = LinePosition;
            EndCursorPosition = CursorPosition;
            HasSelectionEnd = true;
        }

        public bool HasSelection()
        {
            return (HasSelectionBegin && HasSelectionEnd);
        }

        public void Clear()
        {
            HasSelectionBegin = false;
            HasSelectionEnd = false;
            StartLinePosition = -1;
            StartCursorPosition = -1;
            EndLinePosition = -1;
            EndCursorPosition = -1;
        }

        public CodeSelection GetOrderedSelection()
        {
            CodeSelection orderedSelection = new CodeSelection(CodeText);
            if(EndLinePosition < StartLinePosition)
            {
                orderedSelection.StartLinePosition = EndLinePosition;
                orderedSelection.StartCursorPosition = EndCursorPosition;
                orderedSelection.EndLinePosition = StartLinePosition;
                orderedSelection.EndCursorPosition = StartCursorPosition;
            }
            else if(EndLinePosition == StartLinePosition && EndCursorPosition < StartCursorPosition)
            {
                orderedSelection.StartLinePosition = StartLinePosition;
                orderedSelection.StartCursorPosition = EndCursorPosition;
                orderedSelection.EndLinePosition = EndLinePosition;
                orderedSelection.EndCursorPosition = StartCursorPosition;
            }
            else
            {
                orderedSelection.StartLinePosition = StartLinePosition;
                orderedSelection.StartCursorPosition = StartCursorPosition;
                orderedSelection.EndLinePosition = EndLinePosition;
                orderedSelection.EndCursorPosition = EndCursorPosition;
            }
            orderedSelection.HasSelectionBegin = HasSelectionBegin;
            orderedSelection.HasSelectionEnd = HasSelectionEnd;
            return orderedSelection;
        }

        public void Draw()
        {
            if (!HasSelection())
            {
                return;
            }

            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Color3(38 / 255f, 79 / 255f, 120 / 255f);

            CodeSelection CodeSelection = GetOrderedSelection();
            GlyphMetrics GlyphMetrics = CodeText.GlyphMetrics;
            GlyphContainer GlyphContainer = CodeText.GlyphContainer;
            TokenContainer TokenContainer = CodeText.TokenContainer;

            for(int line=CodeSelection.StartLinePosition; line <= CodeSelection.EndLinePosition; line++)
            {
                float yOffset = GlyphMetrics.TopSpace + ((GlyphMetrics.VerticalAdvance + GlyphMetrics.LineSpace) * line);
                float xOffset = GlyphMetrics.LeftSpace;
                float xBegin=0, xEnd=0;

                string lineText = TokenContainer.LineText(line);

                // empty
                if (lineText.Length == 0)
                {
                    xBegin = xOffset;
                    xEnd = xOffset + ((int)Math.Ceiling(GlyphMetrics.SpaceWidth/2f));
                }
                for (int cursor=0; cursor<lineText.Length; cursor++)
                {
                    // start
                    if(line == CodeSelection.StartLinePosition && cursor == CodeSelection.StartCursorPosition)
                    {
                        xBegin = xOffset;
                    }
                    else if(line > CodeSelection.StartLinePosition && cursor == 0)
                    {
                        xBegin = xOffset;
                    }

                    // glyph
                    char textChar = lineText[cursor];
                    if (textChar == ' ')
                    {
                        xOffset += GlyphMetrics.SpaceWidth;
                    }
                    else if (textChar == '\t')
                    {
                        xOffset += GlyphMetrics.TabWidth;
                    }
                    else
                    {
                        Glyph glyph = GlyphContainer.GetGlyph(textChar);
                        xOffset += glyph.HoriziontalAdvance;
                    }

                    // end
                    if (line < CodeSelection.EndLinePosition && cursor == lineText.Length-1)
                    {
                        xEnd = xOffset;
                    }
                    else if (line == CodeSelection.EndLinePosition && cursor == CodeSelection.EndCursorPosition-1)
                    {
                        xEnd = xOffset;
                    }
                }

                yOffset += GlyphMetrics.DelimeterGlyph.VerticalAdvance - GlyphMetrics.DelimeterGlyph.HoriziontalBearingY;
                float yHeight = GlyphMetrics.DelimeterGlyph.Height;

                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(xBegin, yOffset);
                GL.Vertex2(xEnd, yOffset);
                GL.Vertex2(xEnd, yOffset + yHeight);
                GL.Vertex2(xBegin, yOffset + yHeight);
                GL.End();
            }
        }
    }
}
