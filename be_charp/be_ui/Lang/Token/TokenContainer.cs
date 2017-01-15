using Be.Runtime.Parse;
using Be.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.Runtime
{
    public class TokenNodeList : ListCollection<TokenNode>
    { }

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
        public SourceFile SourceFile;
        public TokenParser TokenParser;
        public TokenNodeList AllTokenNodes = new TokenNodeList();
        public TokenNodeList LineTokenNodes = new TokenNodeList();

        public TokenContainer()
        { }

        private void AddToken(TokenSymbol token)
        {
            TokenNode newNode = new TokenNode(token);
            if (AllTokenNodes.Size() > 0)
            {
                TokenNode lastNode = AllTokenNodes.Last();
                lastNode.Next = newNode;
                newNode.Prev = lastNode;
            }
            AllTokenNodes.Add(newNode);
            if(token.Type == Token.LineSpace)
            {
                LineTokenNodes.Add(newNode);
            }
        }

        public void SetSourceFile(SourceFile SourceFile)
        {
            this.SourceFile = SourceFile;
            AllTokenNodes.Clear();
            LineTokenNodes.Clear();
            TokenParser = new TokenParser(SourceFile.Source);
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
            while (node != null && node.Token.Type != Token.LineSpace)
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
            while (node != null && node.Token.Type != Token.LineSpace)
            {
                lineText += node.Token.String;
                node = node.Next;
            }
            return lineText;
        }

        public int LineCount()
        {
            return (LineTokenNodes.Size() + 1);
        }

        public TokenNode FirstTokenNode()
        {
            if(AllTokenNodes.Size()==0)
            {
                return null;
            }
            else
            {
                return AllTokenNodes.First();
            }
        }

        public TokenNode FirstLineTokenNode(int lineNumber)
        {
            if(AllTokenNodes.Size()==0)
            {
                return null;
            }
            else if(lineNumber==0)
            {
                return AllTokenNodes.First();
            }
            else
            {
                return LineTokenNodes.Get(lineNumber-1).Next;
            }
        }
    }
}
