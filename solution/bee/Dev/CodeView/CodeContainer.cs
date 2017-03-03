using Bee.Language;
using Bee.UI;
using Bee.UI.Types;
using Bee.Library;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Integrator
{
    public class CodeContainer
    {
        public Size ViewSize = new Size(600, 400);
        public Size CodeSize = new Size(0, 0);
        public Size ScrollVertical = new Size(0, 0);
        public Size ScrollHorizontal = new Size(0, 0);
        public CodeColor CodeColor;
        public CodeText CodeText;
        public GlyphMetrics GlyphMetrics;
        public GlyphContainer GlyphContainer;
        public TokenContainer TokenContainer;

        public CodeContainer(CodeText CodeText)
        {
            this.CodeText = CodeText;
            this.CodeColor = CodeText.CodeColor;
            this.GlyphMetrics = CodeText.GlyphMetrics;
            this.GlyphContainer = CodeText.GlyphContainer;
            this.TokenContainer = CodeText.TokenContainer;
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
            this.ScrollVertical = new Size(0, 0);
            this.ScrollHorizontal = new Size(0, 0);
        }

        public void Draw()
        {
            int lineNumber = (int)Math.Floor(ViewSize.Height - GlyphMetrics.TopSpace) / (GlyphMetrics.VerticalAdvance + GlyphMetrics.LineSpace);
            lineNumber = 0;
            Point position = new Point(GlyphMetrics.LeftSpace, GlyphMetrics.TopSpace + ((GlyphMetrics.VerticalAdvance + GlyphMetrics.LineSpace) * lineNumber));

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
                    position.x = GlyphMetrics.LeftSpace;
                    position.y += (GlyphMetrics.VerticalAdvance + GlyphMetrics.LineSpace);
                    if(position.y >= ViewSize.Height)
                    {
                        break;
                    }
                }
                else if (token.Type == TokenType.Comment)
                {
                    DrawToken(token, position, CodeColor.Comment);
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

            float w = 10;
            float h = ViewSize.Height;
            float s = ViewSize.Height / CodeSize.Height;
            float x = ViewSize.Width - w;
            float y = GlyphMetrics.TopSpace;
            GL.Color3(75 / 255f, 75 / 255f, 75 / 255f);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(x, y);
            GL.Vertex2(x + w, y);
            GL.Vertex2(x + w, y + h);
            GL.Vertex2(x, y + h);
            GL.End();
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
                else
                {
                    Glyph glyph = GlyphContainer.GetGlyph(charCode);
                    float glyphX = (position.x + glyph.HoriziontalBearingX);
                    float glyphY = (position.y + glyph.VerticalAdvance - glyph.HoriziontalBearingY);
                    if(position.x + glyph.HoriziontalAdvance >= ViewSize.Width)
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

            return new Size(totalWidth, totalHeight);
        }
    }
}
