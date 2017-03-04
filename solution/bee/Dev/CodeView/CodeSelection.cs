using Feltic.UI.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Feltic.Language;
using Feltic.UI;

namespace Feltic.Integrator
{ 
    public class CodeSelectionPart
    {
        public int LinePosition;
        public int CursorPosition;

        public CodeSelectionPart(int LinePosition, int CursorPosition)
        {
            this.LinePosition = LinePosition;
            this.CursorPosition = CursorPosition;
        }

        public CodeSelectionPart Clone()
        {
            return new CodeSelectionPart(LinePosition, CursorPosition);
        }
    }

    public class CodeSelection
    {
        public CodeText CodeText;
        public CodeSelectionPart BeginPart;
        public CodeSelectionPart EndPart;

        private CodeSelection()
        { }

        public CodeSelection(CodeText CodeText)
        {
            this.CodeText = CodeText;
            Begin(CodeText.CodeCursor.LineNumber, CodeText.CodeCursor.CursorPosition);
        }

        public CodeSelection Clone()
        {
            CodeSelection clone = new CodeSelection();
            clone.BeginPart = (BeginPart != null ? BeginPart.Clone() : null);
            clone.EndPart = (EndPart != null ? EndPart.Clone() : null);
            return clone;
        }

        public void Bind(CodeSelection Bind)
        {
            BeginPart = Bind.BeginPart;
            EndPart = Bind.EndPart;
        }

        public void Begin(int LinePosition, int CursorPosition)
        {
            BeginPart = new CodeSelectionPart(LinePosition, CursorPosition);
            EndPart = null;
        }

        public void End(int LinePosition, int CursorPosition)
        {
            EndPart = new CodeSelectionPart(LinePosition, CursorPosition);
        }

        public bool HasSelection()
        {
            return (BeginPart != null && EndPart != null);
        }

        public void Clear()
        {
            BeginPart = null;
            EndPart = null;
        }

        public CodeSelection GetOrdered()
        {
            CodeSelection ordered = new CodeSelection(CodeText);
            if(BeginPart == null || EndPart == null)
            {
                ordered.BeginPart = BeginPart;
                ordered.EndPart = EndPart;
                return ordered;
            }
            if(EndPart.LinePosition < BeginPart.LinePosition)
            {
                ordered.BeginPart = EndPart;
                ordered.EndPart = BeginPart;
            }
            else if(BeginPart.LinePosition == EndPart.LinePosition && EndPart.CursorPosition < BeginPart.CursorPosition)
            {
                ordered.BeginPart = BeginPart;
                ordered.BeginPart.CursorPosition = EndPart.CursorPosition;
                ordered.EndPart = EndPart;
                ordered.EndPart.CursorPosition = BeginPart.CursorPosition;
            }
            else
            {
                ordered.BeginPart = BeginPart;
                ordered.EndPart = EndPart;
            }
            return ordered;
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

            CodeSelection CodeSelection = GetOrdered();
            GlyphMetrics GlyphMetrics = CodeText.GlyphMetrics;
            GlyphContainer GlyphContainer = CodeText.GlyphContainer;
            TokenContainer TokenContainer = CodeText.TokenContainer;

            for(int line=CodeSelection.BeginPart.LinePosition; line <= CodeSelection.EndPart.LinePosition; line++)
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
                    if(line == CodeSelection.BeginPart.LinePosition && cursor == CodeSelection.BeginPart.CursorPosition)
                    {
                        xBegin = xOffset;
                    }
                    else if(line > CodeSelection.BeginPart.CursorPosition && cursor == 0)
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
                    if (line < CodeSelection.EndPart.LinePosition && cursor == lineText.Length-1)
                    {
                        xEnd = xOffset;
                    }
                    else if (line == CodeSelection.EndPart.LinePosition && cursor == CodeSelection.EndPart.CursorPosition-1)
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
