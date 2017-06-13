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
        public ListCollection<Symbol> Container;
        public int Position;
        public ListCollection<int> Stack = new ListCollection<int>();

        public TokenPointer(TokenContainer TokenContainer, int Position=0)
		{
            this.Container = TokenContainer.AllTokens;
            this.Position = Position;
        }

        public Symbol AtPosition(int position)
        {
            if (position < 0 || position > Container.Size - 1)
                return null;
            return Container[position];
        }

        public Symbol Current
        {
            get
            {
                if(Position >= Container.Size)
                    return null;
                return Container[Position];
            }
        }

        public Symbol Next
        {
            get
            {
                if(Position < Container.Size)
                    Position++;
                if(Position >= Container.Size)
                    return null;
                return Container[Position];
            }
        }

        public bool Begin()
        {
            if(Position >= Container.Size)
                return false;
            Stack.Add(Position);
            return true;
        }

        public void Reset()
        {
            Position = Stack.RemoveAt(Stack.Size-1);
        }

        public void Commit()
        {
            Stack.RemoveAt(Stack.Size-1);
        }
	}

    public class TokenContainer
    {
        public ListCollection<Symbol> AllTokens = new ListCollection<Symbol>();
        public ListCollection<int> LineTokenPositions = new ListCollection<int>();

        private void AddToken(Symbol Token)
        {
            if(!Token.IsTextContent())
            {
                AddTokenIntern(Token);
            }
            // split string-content types by new line-tokens (hack)
            else
            {
                string[] commentLines = Token.String.Split('\n');
                for (int i = 0; i < commentLines.Length; i++)
                {
                    AddTokenIntern(new Symbol(commentLines[i], Token.Group, Token.Type));
                    if (i < commentLines.Length - 1)
                    {
                        AddTokenIntern(new Symbol("\n", (int)TokenType.Structure, (int)StructureType.LineSpace, (int)StructureCategory.Space));
                    }
                }
            }
        }

        private void AddTokenIntern(Symbol Token)
        {
            AllTokens.Add(Token);
            if(Token.IsLineSpace())
            {
                LineTokenPositions.Add(AllTokens.Size-1);
            }
        }

        public void SetSourceText(SourceText Source)
        {
            AllTokens.Clear();
            LineTokenPositions.Clear();
            TokenParser parser = new TokenParser(Source);
            Symbol token;
            while(true)
            {
                token = parser.TryToken();
                if(token == null) break;
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
            TokenPointer pointer = FirstLineToken(lineNumber);
            if(pointer == null) return lineText;
            Symbol token = pointer.Current;
            while (token != null && !token.IsLineSpace())
            {
                lineText += token.String;
                token = pointer.Next;
            }
            return lineText;
        }

        public int LineCount
        {
            get
            {
                return (LineTokenPositions.Size+1);
            }
        }

        public TokenPointer FirstToken
        {
            get
            {
                if(AllTokens.Size == 0)
                    return null;
                return new TokenPointer(this);
            }
        }

        public TokenPointer FirstLineToken(int lineNumber)
        {
            if (AllTokens.Size == 0 || lineNumber < 0 || lineNumber >= LineTokenPositions.Size-1)
                return null;
            int position = (LineTokenPositions[lineNumber])+1;
            return new TokenPointer(this, position);
        }
    }
}
