using feltic.Language;
using feltic.UI;
using feltic.UI.Types;
using feltic.Library;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Integrator
{
    public class CodeContainer
    {
        public Size ViewSize = new Size(800, 600);
        public Size CodeSize = new Size(0, 0);
        public Size ScrollPosition = new Size(0, 0);
        public Size ScrollSize = new Size(0, 0);
        public CodeColor CodeColor;
        public CodeText CodeText;
        public GlyphMetrics GlyphMetrics;
        public GlyphContainer GlyphContainer;
        public TokenContainer TokenContainer;
        public ScrollListener InputListener;
        public float TotalLineNumbers;
        public float VisibleLineNumbers;
        public int StartLineNumber;
        public int EndLineNumber;

        public CodeContainer(CodeText CodeText)
        {
            this.CodeText = CodeText;
            this.CodeColor = CodeText.CodeColor;
            this.GlyphMetrics = CodeText.GlyphMetrics;
            this.GlyphContainer = CodeText.GlyphContainer;
            this.TokenContainer = CodeText.TokenContainer;
            this.InputListener = new ScrollListener(this);
        }

        public void Save()
        {
            StreamWriter streamWriter = new StreamWriter(CodeText.SourceText.Filepath);
            TokenNode node = TokenContainer.FirstTokenNode;
            while(node != null)
            {
                streamWriter.Write(node.Token.String);
                node = node.Next;
            }
            streamWriter.Flush();
            streamWriter.Close();
        }

        public void SetContainer(TokenContainer TokenContainer)
        {
            this.TokenContainer = TokenContainer;
            this.CodeSize = GetCodeSize();
        }

        public void Draw()
        {
            this.TotalLineNumbers = (CodeSize.Height - GlyphMetrics.TopSpace) / (float)(GlyphMetrics.VerticalAdvance + GlyphMetrics.LineSpace);
            this.VisibleLineNumbers = (ViewSize.Height - GlyphMetrics.TopSpace) / (float)(GlyphMetrics.VerticalAdvance + GlyphMetrics.LineSpace);

            float notVisibleFactor = (TotalLineNumbers / VisibleLineNumbers);
            float relativeLineNumber = (ScrollPosition.Height / (GlyphMetrics.VerticalAdvance + GlyphMetrics.LineSpace));

            this.StartLineNumber = (int)Math.Round(relativeLineNumber * notVisibleFactor);
            this.EndLineNumber = (int)Math.Round(StartLineNumber + VisibleLineNumbers);

            int lineNumber = StartLineNumber;
            Point position = new Point(GlyphMetrics.LeftSpace, GlyphMetrics.TopSpace);

            TokenNode node = TokenContainer.FirstLineTokenNode(lineNumber);
            while(node != null)
            {
                TokenSymbol token = node.Token;
                if (token.IsStructure(StructureType.WhiteSpace))
                {
                    position.x += GlyphMetrics.SpaceWidth;
                }
                else if (token.IsStructure(StructureType.TabSpace))
                {
                    position.x += GlyphMetrics.TabWidth;
                }
                else if (token.IsStructure(StructureType.LineSpace))
                {
                    lineNumber++;
                    if(lineNumber >= EndLineNumber)
                    {
                        break;
                    }
                    position.x = GlyphMetrics.LeftSpace;
                    position.y += (GlyphMetrics.VerticalAdvance + GlyphMetrics.LineSpace);
                    if(position.y + GlyphMetrics.VerticalAdvance > ViewSize.Height)
                    {
                        break;
                    }
                }
                else if (token.Type == TokenType.Keyword || token.Type == TokenType.Native || token.Type == TokenType.Statement)
                {
                    DrawToken(token, position, CodeColor.Keyword);
                }
                else if (token.Type == TokenType.Literal)
                {
                    LiteralSymbol literal = token.Symbol as LiteralSymbol;
                    if (literal.Type == LiteralType.String || literal.Type == LiteralType.Char)
                    {
                        DrawToken(token, position, CodeColor.String);
                    }
                    else if (literal.Type == LiteralType.Number)
                    {
                        DrawToken(token, position, CodeColor.Normal);
                    }
                    else
                    {
                        DrawToken(token, position, CodeColor.Keyword);
                    }
                }
                else if (token.Type == TokenType.Comment)
                {
                    DrawToken(token, position, CodeColor.Comment);
                }
                else if (token.Type == TokenType.Unknown)
                {
                    DrawToken(token, position, CodeColor.Error);
                }
                else
                {
                    DrawToken(token, position, CodeColor.Normal);
                }
                node = node.Next;
            }

            DrawScroller();
        }

        public void DrawScroller()
        {
            ScrollSize.Width = 10;
            ScrollSize.Height = (ViewSize.Height) * (ViewSize.Height / CodeSize.Height);

            float w = ScrollSize.Width;
            float h = ScrollSize.Height;
            float x = GlyphMetrics.LeftSpace + ViewSize.Width - w;
            float y = ScrollPosition.Height;

            GL.Color3(75 / 255f, 75 / 255f, 75 / 255f);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(x, y);
            GL.Vertex2(x + w, y);
            GL.Vertex2(x + w, y + h);
            GL.Vertex2(x, y + h);
            GL.End();
        }

        public class ScrollListener : InputListener
        {
            public CodeContainer CodeContainer;
            public bool ScrollY;
            public float ScrollYOffset;

            public ScrollListener(CodeContainer CodeContainer)
            {
                this.CodeContainer = CodeContainer;
            }

            public override void Input(InputEvent Event)
            {
                if(Event.IsButton && Event.Button.IsClick && Event.Button.Type == Button.Left)
                {
                    if(GeometryUtils.IntersetMarginBound((int)CodeContainer.ViewSize.Width, (int)CodeContainer.ScrollSize.Width, (int)CodeContainer.ScrollPosition.Height, (int)CodeContainer.ScrollSize.Height, 10, Mouse.Cursor.x, Mouse.Cursor.y))                    
                    {
                        ScrollY = true;
                        ScrollYOffset = (Mouse.Cursor.y - CodeContainer.ScrollPosition.Height);
                    }
                }
                if(Event.IsButton && Event.Button.IsUp)
                {
                    ScrollY = false;
                }
                if(Event.IsCursor && ScrollY)
                {
                    float offset = (Mouse.Cursor.y - ScrollYOffset);
                    if (offset < 0)
                        offset = 0;
                    if (offset > CodeContainer.ViewSize.Height - CodeContainer.ScrollSize.Height)
                        offset = (CodeContainer.ViewSize.Height - CodeContainer.ScrollSize.Height);
                    CodeContainer.ScrollPosition.Height = offset;
                }
            }
        }

        public void DrawToken(TokenSymbol token, Point position, float[] color)
        {
            GL.Color3(color);
            for (int i = 0; i < token.String.Length; i++)
            {
                char charCode = token.String[i];
                if (charCode == ' ')
                {
                    position.x += GlyphMetrics.SpaceWidth;
                }
                else if (charCode == '\t')
                {
                    position.x += GlyphMetrics.TabWidth;
                }
                else if (charCode == '\n')
                {
                    throw new Exception("invalid state");
                }
                else
                {
                    Glyph glyph = GlyphContainer.GetGlyph(charCode);
                    float glyphX = (position.x + glyph.HoriziontalBearingX);
                    float glyphY = (position.y + glyph.VerticalAdvance - glyph.HoriziontalBearingY);
                    if(position.x + glyph.HoriziontalAdvance > ViewSize.Width)
                    {
                        break;
                    }
                    glyph.Draw(glyphX, glyphY);
                    position.x += glyph.HoriziontalAdvance;
                }
            }
        }

        public Size GetCodeSize()
        {
            float currentWidth = GlyphMetrics.LeftSpace;
            float totalWidth = currentWidth;
            float totalHeight = GlyphMetrics.TopSpace;

            TokenNode node = TokenContainer.FirstTokenNode;
            while (node != null)
            {
                TokenSymbol token = node.Token;
                if (token.IsStructure(StructureType.WhiteSpace))
                {
                    currentWidth += GlyphMetrics.SpaceWidth;
                }
                else if (token.IsStructure(StructureType.TabSpace))
                {
                    currentWidth += GlyphMetrics.TabWidth;
                }
                else if (token.IsStructure(StructureType.LineSpace))
                {
                    if (currentWidth > totalWidth)
                    {
                        totalWidth = currentWidth;
                    }
                    currentWidth = GlyphMetrics.LeftSpace;
                    totalHeight += (GlyphMetrics.VerticalAdvance + GlyphMetrics.LineSpace);
                }
                else
                {
                    for (int i = 0; i < token.String.Length; i++)
                    {
                        char charCode = token.String[i];
                        if (charCode == ' ')
                        {
                            currentWidth += GlyphMetrics.SpaceWidth;
                        }
                        else if (charCode == '\t')
                        {
                            currentWidth += GlyphMetrics.TabWidth;
                        }
                        else if (charCode == '\n')
                        {
                            throw new Exception("invalid state");
                        }
                        else
                        {
                            Glyph Glyph = GlyphContainer.GetGlyph(charCode);
                            currentWidth += Glyph.HoriziontalAdvance;
                        }
                    }
                }
                node = node.Next;
            }

            if (currentWidth > totalWidth)
            {
                totalWidth = currentWidth;
            }
            totalHeight += (GlyphMetrics.VerticalAdvance + GlyphMetrics.LineSpace);

            return new Size(totalWidth, totalHeight);
        }
    }
}
