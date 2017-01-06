using Be.UI.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.Integrator
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
            Clear();
            Begin(CodeText.CodeCursor.LinePosition, CodeText.CodeCursor.CursorPosition);
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
            return orderedSelection;
        }

        public void Draw()
        {
            if (!HasSelection())
            {
                return;
            }

            /*
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Color3(38 / 255f, 79 / 255f, 120 / 255f);

            CodeSelection OrderedSelection = GetOrderedSelection();
            CodeGlyphMetrics GlyphMetrics = CodeText.GlyphMetrics;
            GlyphCollection GlyphCollection = CodeText.GlyphCollection;
            CodeContainer CodeContainer = CodeText.CodeContainer;

            for(int i=OrderedSelection.StartLinePosition; i <= OrderedSelection.EndLinePosition; i++)
            {
                float yOffset = GlyphMetrics.TopSpace + ((GlyphMetrics.VerticalAdvance + GlyphMetrics.LineSpace) * i);
                float xOffset = GlyphMetrics.LeftSpace;
                float xStart=0, xEnd=0;
                CodeLine codeLine = CodeContainer.Get(i);
                // empty
                if (codeLine.Text.Length == 0)
                {
                    xStart = xOffset;
                    xEnd = xOffset + ((int)Math.Ceiling(GlyphMetrics.SpaceWidth/(float)2));
                }
                for (int j=0; j<codeLine.Text.Length; j++)
                {
                    // start
                    if(i == OrderedSelection.StartLinePosition && (j == OrderedSelection.StartCursorPosition || j+1 == OrderedSelection.StartCursorPosition))
                    {
                        xStart = xOffset;
                    }
                    else if(i > OrderedSelection.StartLinePosition && j == 0)
                    {
                        xStart = xOffset;
                    }

                    // glyph
                    char textChar = codeLine.Text[j];
                    BeeGlyph glyph = GlyphCollection.GetGlyph(textChar);
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
                        xOffset += glyph.HoriziontalAdvance;
                    }

                    // end
                    if (i < OrderedSelection.EndLinePosition && j == codeLine.Text.Length - 1)
                    {
                        xEnd = xOffset;
                    }
                    else if (i == OrderedSelection.EndLinePosition && j == OrderedSelection.EndCursorPosition - 1)
                    {
                        xEnd = xOffset;
                    }
                }

                yOffset += GlyphMetrics.DelimeterGlyph.VerticalAdvance - GlyphMetrics.DelimeterGlyph.HoriziontalBearingY;
                float yHeight = GlyphMetrics.DelimeterGlyph.Height;

                GL.Begin(PrimitiveType.Quads);
                GL.Vertex2(xStart, yOffset);
                GL.Vertex2(xEnd, yOffset);
                GL.Vertex2(xEnd, yOffset + yHeight);
                GL.Vertex2(xStart, yOffset + yHeight);
                GL.End();
            }
            */
        }
    }
}
