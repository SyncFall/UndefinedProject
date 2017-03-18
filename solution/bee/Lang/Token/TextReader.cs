using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public class TokenTextReader
    {
        public char[] Text;
        public int Position, Length; 
        public int Start;

        public TokenTextReader(SourceText Source)
        {
            this.Text = Source.CharArray;
            this.Length = Text.Length;
        }

        public void Finish(int StartPosition)
        {
            Start = Position = StartPosition;
        }
        
        public bool EqualChar(char chr)
        {
            if(Text[Start] == chr)
            {
                Start = Position = (Start+1 < Length ? Start+1 : Length);
                return true;
            }
            return false;
        }

        public bool EqualString(string str)
        {
            if(Start + str.Length > Length-1)
            {
                return false;
            }
            Position = Start;
            for (int i = 0; i < str.Length; i++, Position++)
            {
                if (Text[Position] != str[i])
                {
                    return false;
                }
            }
            Start = Position;
            return true;
        }

        public bool ToLineOrFileEnd()
        {
            Position = Start;
            while(Position < Length)
            {
                if(Text[Position] == '\n')
                {
                    Start = Position;
                    return true;
                }
                Position++;
            }
            Start = Position;
            return false;
        }

        public bool ToStringOrFileEnd(string str)
        {
            Position = Start;
            while (Position < Length)
            {
                bool found = true;
                for(int i=0; i<str.Length; i++)
                {
                    if(Text[Position] != str[i])
                    {
                        found = false;
                        break;
                    }
                    Position++;
                }
                if(found)
                {
                    Start = (Position+1 < Length ? Position+1 : Length);
                    return true;
                }
                Position++;
            }
            Start = Position;
            return false;
        }

        public bool ToCharWithoutEscapeOrFileEnd(char chr)
        {
            Position = Start;
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
                        Start = (Position+1 < Length ? Position+1 : Length);
                        return true;
                    }
                }
                else
                {
                    Position++;
                }
            }
            Start = Position;
            return false;
        }
    }
}
