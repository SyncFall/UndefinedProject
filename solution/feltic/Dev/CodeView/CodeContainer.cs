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
            TokenPointer pointer = TokenContainer.FirstToken;
            Symbol token = pointer.Current;
            while(token != null)
            {
                streamWriter.Write(token.String);
                token = pointer.Next;
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
            TokenPointer pointer = TokenContainer.FirstToken;
            Symbol token = pointer.Current;
            while (token != null)
            {
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
                    new VisualElement(VisualType.Break, VisualCode);
                }
                else if(token.IsType(TokenType.Object) || token.IsType(TokenType.Native) || token.IsType(TokenType.Statement))
                {
                    new VisualTextElement(token.String, VisualCode, CodeColor.Keyword);
                }
                else if (token.IsType(TokenType.Literal))
                {
                    if (token.IsLiteral(LiteralType.String) || token.IsLiteral(LiteralType.Char))
                    {
                        new VisualTextElement(token.String, VisualCode, CodeColor.String);
                    }
                    else if (token.IsLiteral(LiteralType.Number))
                    {
                        new VisualTextElement(token.String, VisualCode, CodeColor.Normal);
                    }
                    else
                    {
                        new VisualTextElement(token.String, VisualCode, CodeColor.Keyword);
                    }
                }
                else if (token.IsType(TokenType.Comment))
                {
                    new VisualTextElement(token.String, VisualCode, CodeColor.Comment);
                }
                else if (token.IsType(TokenType.Unknown))
                {
                    new VisualTextElement(token.String, VisualCode, CodeColor.Error);
                }
                else
                {
                    new VisualTextElement(token.String, VisualCode, CodeColor.Normal);
                }
                token = pointer.Next;
            }
            Position pos = VisualCode.Position;
            VisualCode.Size = null;
            VisualCode.Position = null;
            VisualElementMetrics.GetSize(VisualCode);
            VisualCode.Position = pos;
        }
    }
}
