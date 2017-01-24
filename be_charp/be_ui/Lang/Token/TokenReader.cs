using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Language
{
    public class TokenReader
    {
        public readonly string Text;
        public int Position, Length;
        public int Start;

        public TokenReader(string Text)
        {
            this.Text = (Text == null ? "" : Text);
            this.Length = Text.Length;
        }

        public void Finish(int StartPosition)
        {
            Start = Position = StartPosition;
        }

        public bool EqualChar(char chr)
        {
            if(Text[Position] == chr)
            {
                Start = ++Position;
                return true;
            }
            return false;
        }

        public bool EqualString(string str)
        {
            if(Position + str.Length >= Length)
            {
                return false;
            }
            for (int i = 0; i < str.Length; i++, Position++)
            {
                if (Text[Position] != str[i])
                {
                    Position = Start;
                    return false;
                }
            }
            Start = Position;
            return true;
        }

        public void ToLineOrFileEnd()
        {
            while(Position < Length)
            {
                if(Text[Position] != '\n')
                {
                    Position++;
                }
                else
                {
                    break;
                }
            }
            Start = Position;
        }

        public void ToStringOrFileEnd(string str)
        {
            while (Position < Length)
            {
                bool stringFound = true;
                for(int i=0; i<str.Length; i++)
                {
                    if(Text[Position] == str[i])
                    {
                        Position++;
                    }
                    else
                    {
                        Position++;
                        stringFound = false;
                        break;
                    }
                }
                if(stringFound)
                {
                    break;
                }
            }
            Start = Position;
        }

        public void ToCharWithoutEscapeOrFileEnd(char chr)
        {
            while (Position < Length)
            {
                if (Text[Position] == chr)
                {
                    if (Position > 0 && Text[Position-1] == '\\')
                    {
                        Position++;
                    }
                    else
                    {
                        Position++;
                        break;
                    }
                }
                else
                {
                    Position++;
                }
            }
            Start = Position;
        }

        public string TryIdentifier()
        {
            if (!Char.IsLetter(Text[Position]))
            {
                return null;
            }
            Position++;
            while (Position < Length)
            {
                if (Char.IsLetter(Text[Position]) || Char.IsDigit(Text[Position]) || Text[Position] == '_')
                {
                    Position++;
                }
                else
                {
                    break;
                }
            }
            string result = Text.Substring(Start, Position - Start);
            Start = Position;
            return result;
        }
    }
}
