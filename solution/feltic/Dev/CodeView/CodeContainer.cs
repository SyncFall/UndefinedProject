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
            if(VisualCode.Nodes != null) VisualCode.Nodes.Clear();
            TokenPointer pointer = TokenContainer.FirstToken;
            Symbol token = pointer.Current;
            while (token != null)
            {
                if (token.IsStructure(StructureType.WhiteSpace))
                {
                    VisualCode.add(new VisualTextElement(" "));
                }
                else if (token.IsStructure(StructureType.TabSpace))
                {
                    VisualCode.add(new VisualTextElement("\t"));
                }
                else if (token.IsStructure(StructureType.LineSpace))
                {
                    VisualCode.add(new VisualElement(VisualType.Break));
                }
                else if(token.IsType(TokenType.Object) || token.IsType(TokenType.Native) || token.IsType(TokenType.Statement))
                {
                    VisualCode.add(new VisualTextElement(token.String, CodeColor.Keyword));
                }
                else if (token.IsType(TokenType.Literal))
                {
                    if (token.IsLiteral(LiteralType.String) || token.IsLiteral(LiteralType.Char))
                    {
                        VisualCode.add(new VisualTextElement(token.String, CodeColor.String));
                    }
                    else if (token.IsLiteral(LiteralType.Number))
                    {
                        VisualCode.add(new VisualTextElement(token.String, CodeColor.Normal));
                    }
                    else
                    {
                        VisualCode.add(new VisualTextElement(token.String, CodeColor.Keyword));
                    }
                }
                else if (token.IsType(TokenType.Comment))
                {
                    VisualCode.add(new VisualTextElement(token.String, CodeColor.Comment));
                }
                else if (token.IsType(TokenType.Unknown))
                {
                    VisualCode.add(new VisualTextElement(token.String, CodeColor.Error));
                }
                else
                {
                    VisualCode.add(new VisualTextElement(token.String, CodeColor.Normal));
                }
                token = pointer.Next;
            }
            /*
            Position pos = VisualCode.Position;
            VisualCode.Size = null;
            VisualCode.Position = null;
            VisualElementMetrics.GetSize(VisualCode);
            VisualCode.Position = pos;
            */
        }
    }
}
