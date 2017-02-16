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
        }

        private float CurrentX;
        private float CurrentY;
        private int LineNumber;
        public void Draw()
        {
            CurrentX = GlyphMetrics.LeftSpace;
            CurrentY = GlyphMetrics.TopSpace;

            LineNumber = 0;

            TokenNode node = TokenContainer.FirstLineTokenNode(LineNumber);
            while(node != null)
            {
                TokenSymbol token = node.Token;
                if (token.IsStructure(StructureType.WhiteSpace))
                {
                    CurrentX += GlyphMetrics.SpaceWidth;
                }
                else if (token.IsStructure(StructureType.TabSpace))
                {
                    CurrentX += GlyphMetrics.TabWidth;
                }
                else if (token.IsStructure(StructureType.LineSpace))
                {
                    LineNumber++;
                    CurrentX = GlyphMetrics.LeftSpace;
                    CurrentY = GlyphMetrics.TopSpace + ((GlyphMetrics.VerticalAdvance + GlyphMetrics.LineSpace) * LineNumber);
                }
                else if (token.Type == TokenType.Comment)
                {
                    DrawToken(token, CodeColor.Comment);
                }
                else if (token.Type == TokenType.Keyword || token.Type == TokenType.Native || token.Type == TokenType.Statement)
                {
                    DrawToken(token, CodeColor.Keyword);
                }
                else if (token.Type == TokenType.Literal)
                {
                    LiteralSymbol literal = (token as LiteralToken).Symbol;
                    if (literal.Type == LiteralType.String || literal.Type == LiteralType.Char)
                    {
                        DrawToken(token, CodeColor.String);
                    }
                    else if (literal.Type == LiteralType.Number)
                    {
                        DrawToken(token, CodeColor.Normal);
                    }
                    else
                    {
                        DrawToken(token, CodeColor.Keyword);
                    }
                }
                else if (token.Type == TokenType.Unknown)
                {
                    DrawToken(token, CodeColor.Error);
                }
                else
                {
                    DrawToken(token, CodeColor.Normal);
                }
                node = node.Next;
            }
        }

        public void DrawToken(TokenSymbol token, float[] color)
        {
            float StartX = CurrentX;
            float StartY = CurrentY;

            GL.Color3(color);

            for (int i = 0; i < token.String.Length; i++)
            {
                char charCode = token.String[i];
                if(charCode == '\n')
                {
                    LineNumber++;
                    CurrentX = GlyphMetrics.LeftSpace;
                    CurrentY = GlyphMetrics.TopSpace + ((GlyphMetrics.VerticalAdvance + GlyphMetrics.LineSpace) * LineNumber);
                }
                else if (charCode == ' ')
                { 
                    CurrentX += GlyphMetrics.SpaceWidth;
                }
                else if (charCode == '\t')
                {
                    CurrentX += GlyphMetrics.TabWidth;
                }
                else
                {
                    Glyph Glyph = GlyphContainer.GetGlyph(charCode);
                    float GlyphX = (CurrentX + Glyph.HoriziontalBearingX);
                    float GlyphY = (CurrentY + Glyph.VerticalAdvance - Glyph.HoriziontalBearingY);
                    Glyph.Draw(GlyphX, GlyphY);
                    CurrentX += Glyph.HoriziontalAdvance;
                }
            }
        }
    }
}
