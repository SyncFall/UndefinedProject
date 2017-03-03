using Bee.Language;
using Bee.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Language
{
    public class TokenNodeList : ListCollection<TokenNode>
    {
        public TokenNodeList(int Size) : base(Size)
        { }
    }

	public class TokenPointer
	{
		public TokenNode Root;
		public TokenNode Current;
        public ListCollection<TokenNode> BeginStepNode = new ListCollection<TokenNode>();

        public TokenPointer(TokenNode Root)
		{
			this.Current = this.Root = Root;
		}

        public TokenNode Next()
        {
            return (Current != null ? Current = Current.Next : null);
        }

        public void StepBegin()
        {
            BeginStepNode.Add(Current);
        }

        public void StepReset()
        {
            Current = BeginStepNode.RemoveAt(BeginStepNode.Size-1);
        }
	}

    public class TokenNode
    {
        public TokenNode Prev;
        public TokenNode Next;
        public TokenSymbol Token;
        
        public TokenNode(TokenSymbol Token)
        {
            this.Token = Token;  
        }
    }

    public class TokenContainer
    {
        public SourceText SourceText;
        public TokenNodeList AllTokenNodes = new TokenNodeList(2048);
        public TokenNodeList LineTokenNodes = new TokenNodeList(128);

        public TokenContainer()
        { }

        private void AddToken(TokenSymbol token)
        {
            if(token.Type == TokenType.Comment)
            {
                AddCommentToken(token);
            }
            else
            {
                AddTokenIntern(token);
            }
        }

        private void AddCommentToken(TokenSymbol token)
        {
            if (token.Type == TokenType.Comment)
            {
                string[] commentLines = token.String.Split('\n');
                for (int i = 0; i < commentLines.Length; i++)
                {
                    AddTokenIntern(new TokenSymbol(TokenType.Comment, commentLines[i], null));
                    if (i < commentLines.Length - 1)
                    {
                        AddTokenIntern(new TokenSymbol(TokenType.Structure, "\n", new StructureSymbol(StructureType.LineSpace, StructureGroup.Space, "\n")));
                    }
                }
            }
        }

        private void AddTokenIntern(TokenSymbol token)
        {
            TokenNode newNode = new TokenNode(token);
            if (AllTokenNodes.Size > 0)
            {
                TokenNode lastNode = AllTokenNodes.Get(AllTokenNodes.Size - 1);
                lastNode.Next = newNode;
                newNode.Prev = lastNode;
            }
            AllTokenNodes.Add(newNode);
            if (token.IsLineSpace())
            {
                LineTokenNodes.Add(newNode);
            }
        }

        public void SetSource(SourceText Source)
        {
            this.SourceText = Source;
            AllTokenNodes.Clear();
            LineTokenNodes.Clear();
            TokenParser TokenParser = new TokenParser(Source.Text);
            while(!TokenParser.IsEnd())
            {
                TokenSymbol token = TokenParser.TryToken();
                AddToken(token);
            }
        }
        
        public int TextCount(int lineNumber)
        {
            int textCount = 0;
            TokenNode node = FirstLineTokenNode(lineNumber);
            while (node != null && !node.Token.IsLineSpace())
            {
                textCount += node.Token.String.Length;
                node = node.Next;
            }
            return textCount;
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
                return AllTokenNodes.First;
            }
        }

        public TokenNode FirstLineTokenNode(int lineNumber)
        {
            if(AllTokenNodes.Size==0)
            {
                return null;
            }
            else if(lineNumber==0)
            {
                return AllTokenNodes.First;
            }
            else
            {
                return LineTokenNodes.Get(lineNumber-1).Next;
            }
        }
    }
}
