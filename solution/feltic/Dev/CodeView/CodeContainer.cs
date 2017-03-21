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
        public CodeColor CodeColor;
        public CodeText CodeText;
        public TokenContainer TokenContainer;
        public VisualElement VisualCode;

        public CodeContainer(CodeText CodeText)
        {
            this.CodeText = CodeText;
            this.CodeColor = CodeText.CodeColor;
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
            Build();
        }

        public Position Start
        {
            get
            {
                if (VisualCode == null || VisualCode.Position == null)
                    return null;
                return VisualCode.Position;
            }
        }

        public void Build()
        {
            this.VisualCode = CodeText.VisualCode;
            if(VisualCode == null) return;
            if(VisualCode.Childrens != null) VisualCode.Childrens.Clear();
            TokenNode node = TokenContainer.FirstTokenNode;
            while (node != null)
            {
                TokenSymbol token = node.Token;
                if (token.IsStructure(StructureType.WhiteSpace))
                {
                    new VisualTextElement(" ", VisualCode);
                }
                else if (token.IsStructure(StructureType.TabSpace))
                {
                    new VisualTextElement("\t", VisualCode);
                }
                else if (token.IsStructure(StructureType.LineSpace))
                {
                    new VisualElement(VisualElementType.Break, VisualCode);
                }
                else if (token.Type == TokenType.Keyword || token.Type == TokenType.Native || token.Type == TokenType.Statement)
                {
                    new VisualTextElement(token.String, VisualCode, CodeColor.Keyword);
                }
                else if (token.Type == TokenType.Literal)
                {
                    LiteralSymbol literal = token.Symbol as LiteralSymbol;
                    if (literal.Type == LiteralType.String || literal.Type == LiteralType.Char)
                    {
                        new VisualTextElement(token.String, VisualCode, CodeColor.String);
                    }
                    else if (literal.Type == LiteralType.Number)
                    {
                        new VisualTextElement(token.String, VisualCode, CodeColor.Normal);
                    }
                    else
                    {
                        new VisualTextElement(token.String, VisualCode, CodeColor.Keyword);
                    }
                }
                else if (token.Type == TokenType.Comment)
                {
                    new VisualTextElement(token.String, VisualCode, CodeColor.Comment);
                }
                else if (token.Type == TokenType.Unknown)
                {
                    new VisualTextElement(token.String, VisualCode, CodeColor.Error);
                }
                else
                {
                    new VisualTextElement(token.String, VisualCode, CodeColor.Normal);
                }
                node = node.Next;
            }
            Position pos = VisualCode.Position;
            VisualCode.Size = null;
            VisualCode.Position = null;
            VisualElementMetrics.GetSize(VisualCode);
            VisualCode.Position = pos;
        }
    }
}
