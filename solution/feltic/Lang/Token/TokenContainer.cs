using feltic.Language;
using feltic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
	public class TokenPointer
	{
		public TokenNode Root;
		public TokenNode Current;
        public ListCollection<TokenNode> StepNodes = new ListCollection<TokenNode>(8);

        public TokenPointer(TokenNode Root)
		{
			this.Current = this.Root = Root;
		}

        public TokenNode Next()
        {
            return (Current != null ? Current = Current.Next : null);
        }

        public bool StepBegin()
        {
            if(Current != null)
                StepNodes.Add(Current);
            return (Current != null);
        }

        public void StepReset()
        {
            Current = StepNodes.RemoveAt(StepNodes.Size-1);
        }

        public void StepCommit()
        {
            StepNodes.RemoveAt(StepNodes.Size-1);
        }
	}

    public class TokenNode
    {
        public TokenNode Prev;
        public TokenNode Next;
        public TokenSymbol Token;
    }

    public class TokenNodeList : ListCollection<TokenNode>
    {
        public TokenNodeList(int Size) : base(Size)
        { }
    }

    public class TokenContainer
    {
        public SourceText SourceText;
        public TokenNode Begin;
        public TokenNode Current;
        public TokenNodeList LineTokenNodes = new TokenNodeList(128);

        public TokenContainer()
        { }

        private void AddToken(TokenSymbol token)
        {
            // split string-content types by new line-tokens (hack)
            if (token.Type == TokenType.Comment || token.IsLiteral(LiteralType.Char) || token.IsLiteral(LiteralType.String))
            {
                string[] commentLines = token.String.Split('\n');
                for (int i = 0; i < commentLines.Length; i++)
                {
                    AddTokenIntern(new TokenSymbol(token.Type, commentLines[i], token.Symbol));
                    if (i < commentLines.Length - 1)
                    {
                        AddTokenIntern(new TokenSymbol(TokenType.Structure, "\n", new StructureSymbol(StructureType.LineSpace, StructureGroup.Space, "\n")));
                    }
                }
            }
            else
            {
                AddTokenIntern(token);
            }
        }

        private void AddTokenIntern(TokenSymbol token)
        {
            TokenNode newNode = new TokenNode();
            newNode.Token = token;
            if (Begin != null)
            {
                newNode.Prev = Current;
                Current.Next = newNode;
                Current = newNode;
            }
            else
            {
                Begin = Current = newNode;
            }
            if(token.IsLineSpace())
            {
                LineTokenNodes.Add(newNode);
            }
        }

        public void SetSource(SourceText Source)
        {
            this.SourceText = Source;
            this.Begin = null;
            this.Current = null;
            LineTokenNodes.Clear();
            TokenParser TokenParser = new TokenParser(Source);
            while(!TokenParser.IsEnd())
            {
                TokenSymbol token = TokenParser.TryToken();
                AddToken(token);
            }
        }
        
        public int TextCount(int lineNumber)
        {
            return LineText(lineNumber).Length;
        }

        public string LineText(int lineNumber)
        {
            string lineText = "";
            TokenNode node = FirstLineTokenNode(lineNumber);
            while (node != null && !node.Token.IsLineSpace())
            {
                lineText += node.Token.String;
                node = node.Next;
            }
            return lineText;
        }

        public int LineCount()
        {
            return (LineTokenNodes.Size + 1);
        }

        public TokenNode FirstTokenNode
        {
            get
            {
                return Begin;
            }
        }

        public TokenNode FirstLineTokenNode(int lineNumber)
        {
            if(Begin==null)
            {
                return null;
            }
            if(lineNumber<=0)
            {
                return Begin;
            }
            if(lineNumber >= LineTokenNodes.Size-1)
            {
                lineNumber = LineTokenNodes.Size-1;
            }
            return LineTokenNodes.Get(lineNumber-1).Next;
        }
    }
}
