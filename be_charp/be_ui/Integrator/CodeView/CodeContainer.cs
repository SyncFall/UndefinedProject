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
            for(int i=0; i< TokenContainer.AllTokens.Size(); i++)
            {
                streamWriter.Write(TokenContainer.AllTokens.Get(i).TextString);
            }
            streamWriter.Flush();
            streamWriter.Close();
        }

        public float CurrentX;
        public float CurrentY;
        public int LineNumber;
        public void Draw()
        {
            CurrentX = GlyphMetrics.LeftSpace;
            CurrentY = GlyphMetrics.TopSpace;
            LineNumber = 0;

            for (int i = 0; i < this.TokenContainer.AllTokens.Size(); i++)
            {
                TokenSymbol token = this.TokenContainer.AllTokens.Get(i);

                // token string and color
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
                else if(token.Group == TokenGroup.Region || token.Group == TokenGroup.Processor)
                {
                    DrawToken(token, CodeColorType.Region);
                }
                else if (token.Type == Token.Keyword || token.Type == Token.Native)
                {
                    DrawToken(token, CodeColorType.Keyword);
                }
                else if (token.Type == Token.Literal)
                {
                    LiteralToken literalToken = token as LiteralToken;
                    if (literalToken.LiteralType == LiteralType.String || literalToken.LiteralType == LiteralType.Char)
                    {
                        DrawToken(token, CodeColorType.String);
                    }
                    else if (literalToken.LiteralType == LiteralType.Number)
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
            }
        }

        public void DrawToken(TokenSymbol token, CodeColorType Color)
        {
            float startX = CurrentX;
            float startY = CurrentY;

            CodeColor.Set(Color);

            for (int i = 0; i < token.TextString.Length; i++)
            {
                char charCode = token.TextString[i];
                Glyph glyph = GlyphContainer.GetGlyph(charCode);
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
                    float glyphX = (CurrentX + glyph.HoriziontalBearingX);
                    float glyphY = (CurrentY + glyph.VerticalAdvance - glyph.HoriziontalBearingY);
                    glyph.Draw(glyphX, glyphY);
                    CurrentX += glyph.HoriziontalAdvance;
                }
            }
            if(token.Status != null && token.Status.Type == TokenStatus.Error)
            {
                float yHeight = GlyphMetrics.DelimeterGlyph.VerticalAdvance + (GlyphMetrics.DelimeterGlyph.VerticalAdvance - GlyphMetrics.DelimeterGlyph.HoriziontalBearingY);
                CodeColor.Set(CodeColorType.Error);
                GL.LineWidth(2.0f);
                GL.Begin(PrimitiveType.Lines);
                GL.Vertex2(startX, startY + yHeight);
                GL.Vertex2(CurrentX, CurrentY + yHeight);
                GL.End();
            }
        }
    }
}
