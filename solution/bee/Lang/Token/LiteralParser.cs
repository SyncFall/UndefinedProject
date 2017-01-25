using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Language
{
    public class LiteralParser
    {
        public TokenTextReader TextParser;

        public LiteralParser(TokenTextReader TextParser)
        {
            this.TextParser = TextParser;
        }

        public LiteralSymbol TryLiteral()
        {
            LiteralSymbol literal = null;
            if((literal = TryString()) != null ||
               (literal = TryChar()) != null ||
               (literal = TryNumber()) != null ||
               (literal = TryBool()) != null ||
               (literal = TryNull()) != null
            ){
                return literal;
            }
            return null;
        }

        public NullLiteral TryNull()
        {
            if(TextParser.EqualString(LiteralConst.Null))
            {
                return new NullLiteral();
            }
            return null;
        }

        public BoolLiteral TryBool()
        {
            if (TextParser.EqualString(LiteralConst.True))
            {
                return new BoolLiteral(true);
            }
            else if(TextParser.EqualString(LiteralConst.False))
            {
                return new BoolLiteral(false);
            }
            return null;
        }

        public CharLiteral TryChar()
        {
            if (TextParser.EqualChar(LiteralConst.CharEscape[0]))
            {
                int startPosition = TextParser.Start;
                TextParser.ToCharWithoutEscapeOrFileEnd(LiteralConst.CharEscape[0]);
                int endPosition = TextParser.Start;
                string dataValue = (endPosition > startPosition ? TextParser.Text.Substring(startPosition, endPosition - startPosition - 1) : "");
                return new CharLiteral(dataValue);
            }
            return null;
        }

        public StringLiteral TryString()
        {
            if(TextParser.EqualChar(LiteralConst.StringEscape[0]))
            {
                int startPosition = TextParser.Start;
                TextParser.ToCharWithoutEscapeOrFileEnd(LiteralConst.StringEscape[0]);
                int endPosition = TextParser.Start;
                string dataValue = (endPosition > startPosition ? TextParser.Text.Substring(startPosition, endPosition - startPosition-1) : "");
                return new StringLiteral(dataValue);
            }
            return null;
        }

        public NumberLiteral TryNumber()
        {
            int startPosition = TextParser.Start;
            int idx = startPosition;
            char chr = TextParser.Text[idx];
            if(chr == '.' && idx + 1 < TextParser.Length)
            {
                chr = TextParser.Text[idx + 1];
                idx++;
            }
            if (Char.IsDigit(chr))
            {
                for (; idx < TextParser.Length; idx++)
                {
                    chr = TextParser.Text[idx];
                    if (Char.IsDigit(chr))
                    {
                        ;
                    }
                    else if(chr == '.')
                    {
                        ;
                    }
                    else if(chr == 'f' || chr == 'd' || chr == 'i' || chr == 'l')
                    {
                        ;
                    }
                    else
                    {
                        break;
                    }
                }
                TextParser.Finish(idx);
                return new NumberLiteral(TextParser.Text.Substring(startPosition, idx - startPosition));
            }
            // none
            else
            {
                return null;
            }
        }
    }
}
