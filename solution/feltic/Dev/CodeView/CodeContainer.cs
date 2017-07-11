using feltic.Language;
using feltic.Visual;
using feltic.Visual.Types;
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
                if (VisualCode == null || VisualCode.Render == null || !VisualCode.Render.True)
                    return null;
                return VisualCode.Render.Position;
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
                    VisualCode.add(new VisualText(" "));
                }
                else if (token.IsStructure(StructureType.TabSpace))
                {
                    VisualCode.add(new VisualText("\t"));
                }
                else if (token.IsStructure(StructureType.LineSpace))
                {
                    VisualCode.add(new VisualElement(VisualType.Break));
                }
                else if(token.IsType(TokenType.Object) || token.IsType(TokenType.Native) || token.IsType(TokenType.Statement))
                {
                    VisualCode.add(new VisualText(token.String, CodeColor.Keyword));
                }
                else if (token.IsType(TokenType.Literal))
                {
                    if (token.IsLiteral(LiteralType.String) || token.IsLiteral(LiteralType.Char))
                    {
                        VisualCode.add(new VisualText(token.String, CodeColor.String));
                    }
                    else if (token.IsLiteral(LiteralType.Number))
                    {
                        VisualCode.add(new VisualText(token.String, CodeColor.Normal));
                    }
                    else
                    {
                        VisualCode.add(new VisualText(token.String, CodeColor.Keyword));
                    }
                }
                else if (token.IsType(TokenType.Comment))
                {
                    VisualCode.add(new VisualText(token.String, CodeColor.Comment));
                }
                else if (token.IsType(TokenType.Unknown))
                {
                    VisualCode.add(new VisualText(token.String, CodeColor.Error));
                }
                else
                {
                    VisualCode.add(new VisualText(token.String, CodeColor.Normal));
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
