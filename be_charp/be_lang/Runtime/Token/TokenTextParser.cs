﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.Runtime
{
    public class TokenTextParser
    {
        public string Text;
        public int Position, Length;
        public int Start;

        public TokenTextParser(string Text)
        {
            this.Text = (Text == null ? "" : Text);
            this.Length = Text.Length;
        }

        public void Begin(int StartPosition)
        {
            Start = Position = StartPosition;
        }

        public void Finish(bool forward)
        {
            if (forward)
            {
                Start = Position;
            }
            else
            {
                Position = Start;
            }
        }

        public bool EqualChar(char chr)
        {
            if(Text[Position] == chr)
            {
                Position++;
                Finish(true);
                return true;
            }
            return false;
        }

        public bool EqualString(string str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                if (Text[Position] == str[i])
                {
                    Position++;
                }
                else
                {
                    Finish(false);
                    return false;
                }
            }
            Finish(true);
            return true;
        }

        public void ToLineEndOrFileEnd()
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
            Finish(true);
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
            Finish(true);
        }

        public void ToCharWithoutEscapeOrFileEnd(char chr)
        {
            while (Position < Length)
            {
                if (Text[Position] == chr)
                {
                    if ((Position < Length && Text[Position - 1] == '\\'))
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
            Finish(true);
        }

        public string GetNamePath()
        {
            if (!Char.IsLetter(Text[Position]))
            {
                Finish(false);
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
            Finish(true);
            return result;
        }
    }
}