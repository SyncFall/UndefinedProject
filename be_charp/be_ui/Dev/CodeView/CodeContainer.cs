using Be.Runtime;
using Be.Runtime.Types;
using Be.UI;
using Be.UI.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.Integrator
{
    public class CodeContainer
    {
        public CodeColor CodeColor;
        public CodeText CodeText;
        public GlyphMetrics GlyphMetrics;
        public GlyphContainer GlyphContainer;
        public TokenContainer TokenContainer;
        public ListCollection<CodeToken> CodeTokens = new ListCollection<CodeToken>();

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
            StreamWriter streamWriter = new StreamWriter(CodeText.SourceFile.Filepath);
            TokenNode node = TokenContainer.FirstTokenNode();
            while(node != null)
            {
                streamWriter.Write(node.Token.String);
                node = node.Next;
            }
            streamWriter.Flush();
            streamWriter.Close();
        }

        public void SetTokenContainer(TokenContainer TokenContainer)
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
                if (token.Type == Token.WhiteSpace)
                {
                    CurrentX += GlyphMetrics.SpaceWidth;
                }
                else if (token.Type == Token.TabSpace)
                {
                    CurrentX += GlyphMetrics.TabWidth;
                }
                else if (token.Type == Token.LineSpace)
                {
                    LineNumber++;
                    CurrentX = GlyphMetrics.LeftSpace;
                    CurrentY = GlyphMetrics.TopSpace + ((GlyphMetrics.VerticalAdvance + GlyphMetrics.LineSpace) * LineNumber);
                }
                else if (token.Group == TokenGroup.Comment)
                {
                    DrawToken(token, CodeColorType.Comment);
                }
                else if (token.Group == TokenGroup.Region || token.Group == TokenGroup.Processor)
                {
                    DrawToken(token, CodeColorType.Region);
                }
                else if (token.Type == Token.Keyword)
                {
                    DrawToken(token, CodeColorType.Keyword);
                }
                else if (token.Type == Token.Literal)
                {
                    LiteralSymbol literal = (token as LiteralToken).LiteralSymbol;
                    if (literal.Type == LiteralType.String || literal.Type == LiteralType.Char)
                    {
                        DrawToken(token, CodeColorType.String);
                    }
                    else if (literal.Type == LiteralType.Number)
                    {
                        DrawToken(token, CodeColorType.Normal);
                    }
                    else
                    {
                        DrawToken(token, CodeColorType.Keyword);
                    }
                }
                else if (token.Type == Token.Unknown)
                {
                    DrawToken(token, CodeColorType.Normal);
                }
                else
                {
                    DrawToken(token, CodeColorType.Normal);
                }
                node = node.Next;
            }
        }

        private float StartX;
        private float StartY;
        private Glyph Glyph;
        private float GlyphX;
        private float GlyphY;
        public void DrawToken(TokenSymbol token, CodeColorType Color)
        {
            StartX = CurrentX;
            StartY = CurrentY;

            CodeColor.Set(Color);

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
                    Glyph = GlyphContainer.GetGlyph(charCode);
                    GlyphX = (CurrentX + Glyph.HoriziontalBearingX);
                    GlyphY = (CurrentY + Glyph.VerticalAdvance - Glyph.HoriziontalBearingY);
                    Glyph.Draw(GlyphX, GlyphY);
                    CurrentX += Glyph.HoriziontalAdvance;
                }
            }
        }
    }
}
