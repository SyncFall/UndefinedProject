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
        private static long UIDCounter = 0;
        public TokenNode Prev;
        public TokenNode Next;
        public TokenSymbol Token;
        public long UID;
        public bool ToInsert;
        public bool ToDelete;

        public TokenNode(TokenSymbol Token)
        {
            this.Token = Token;
            this.UID = (++UIDCounter);       
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

        public void InsertText(int LineNumber, int StartPosition, string InsertText)
        {
            TokenParser tokenParser;
            TokenSymbol token;
            string validateText = "";
            TokenNode node, beginNode=null, backNode=null, newNode=null, nextNode=null;
            // go to start matching node
            node = FirstLineTokenNode(LineNumber);
            int currentPostion=0, textPosition=0;
            while(node != null)
            {
                for(textPosition = 0; (currentPostion < StartPosition) && (textPosition < node.Token.String.Length); textPosition++, currentPostion++)
                { }
                if (currentPostion == StartPosition)
                {
                    if (textPosition == 0)
                    {
                        validateText = InsertText + node.Token.String;     
                    }
                    else
                    {
                        validateText = node.Token.String.Substring(0, textPosition) + InsertText + node.Token.String.Substring(textPosition);
                    }
                    node.ToDelete = true;
                    beginNode = node;
                    break;
                };
                node = node.Next;
            }
            // revalidate backward
            while (node.Prev != null && (node = backNode = node.Prev) != null)
            {
                tokenParser = new TokenParser(node.Token.String + validateText);
                token = tokenParser.TryToken();
                if (token.IsEqual(node.Token))
                {
                    break;
                }
                else
                {
                    validateText = node.Token.String + validateText;
                    node.ToDelete = true;
                }
            }
            // update changed state
            tokenParser = new TokenParser(validateText);
            while (!tokenParser.IsEnd())
            {
                token = tokenParser.TryToken();
                if(tokenParser.IsEnd())
                {
                    if(token.IsEqual(beginNode.Token))
                    {

                        return;
                    }
                    else
                    {
                        validateText = token.String;
                        break;
                    }
                }
                else
                {
                    newNode = new TokenNode(token);
                    newNode.ToInsert = true;
                    if(backNode != null && !backNode.ToDelete)
                    {
                        newNode.Prev = backNode;
                        backNode = newNode;
                    }
                    else if(backNode != null && backNode.ToDelete)
                    {
                        backNode = newNode;
                    }
                    else
                    {
                        newNode.Next = node;
                    }
                }
            }
            // change state next node
            if(beginNode.Next == null)
            {
                nextNode = null;
                tokenParser = new TokenParser(validateText);
            }
            else
            {
                nextNode = beginNode.Next;
                tokenParser = new TokenParser(validateText + nextNode.Token.String);
            }
            // revalidate forward
            while(!tokenParser.IsEnd())
            {
                token = tokenParser.TryToken();
                if(nextNode != null && tokenParser.IsEnd())
                {
                    if(token.IsEqual(nextNode.Token))
                    {
                        nextNode.Prev = node;
                        node.Next = nextNode;
                        return;
                    }
                    else
                    {
                        validateText = token.String;
                        nextNode = nextNode.Next;
                        tokenParser = new TokenParser(validateText);
                        continue;
                    }
                }
                newNode = new TokenNode(token);
                newNode.ToInsert = true;
                newNode.Prev = node;
                node = node.Next = newNode;
            }
        }

        public void DeleteText(int LineNumber, int StartPosition, int TextLength)
        {

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
