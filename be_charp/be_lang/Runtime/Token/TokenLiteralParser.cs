using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.Runtime
{
    public class TokenLiteralParser
    {
        public TokenTextParser Parser;

        public TokenLiteralParser(TokenTextParser Parser)
        {
            this.Parser = Parser;
        }

        public ObjectConstantLiteral TryObjectConstant()
        {
            int startPosition = Parser.Position;
            string name = Parser.GetNamePath();
            if(name == null)
            {
                return null;
            }
            if(name == Literals.This)
            {
                return new ObjectConstantLiteral(LiteralType.This, Literals.This);
            }
            else if(name == Literals.Base)
            {
                return new ObjectConstantLiteral(LiteralType.Base, Literals.Base);
            }
            else if(name == Literals.Null)
            {
                return new ObjectConstantLiteral(LiteralType.Null, Literals.Null);
            }
            else if(name == Literals.Value)
            {
                return new ObjectConstantLiteral(LiteralType.Value, Literals.Value);
            }
            else if(name == Literals.New)
            {
                return new ObjectConstantLiteral(LiteralType.New, Literals.New);
            }
            else
            {
                Parser.Begin(startPosition);
                return null;
            }
        }

        public BoolLiteral TryBool()
        {
            int startPosition = Parser.Position;
            string boolValue = Parser.GetNamePath();
            if (boolValue == null)
            {
                return null;
            }
            else if (boolValue == Literals.True)
            {
                return new BoolLiteral(Literals.True);
            }
            else if (boolValue == Literals.False)
            {
                return new BoolLiteral(Literals.False);
            }
            else
            {
                Parser.Begin(startPosition);
                return null;
            }
        }

        public StringLiteral TryString()
        {
            if(Parser.EqualChar(Literals.String))
            {
                int startPosition = Parser.Position;
                Parser.ToCharWithoutEscapeOrFileEnd(Literals.String);
                int endPosition = Parser.Position;
                string dataValue = Parser.Text.Substring(startPosition, endPosition - startPosition - 1);
                return new StringLiteral(dataValue);
            }
            return null;
        }

        public CharLiteral TryChar()
        {
            if (Parser.EqualChar(Literals.Char))
            {
                int startPosition = Parser.Position;
                Parser.ToCharWithoutEscapeOrFileEnd(Literals.Char);
                int endPosition = Parser.Position;
                string dataValue = Parser.Text.Substring(startPosition, endPosition - startPosition - 1);
                return new CharLiteral(dataValue);
            }
            return null;
        }

        public NumberLiteral TryNumber()
        {
            // number-literal
            int startPosition = Parser.Position;
            int idx = startPosition;
            char chr = Parser.Text[idx];
            if(chr == '.' && idx + 1 < Parser.Length)
            {
                chr = Parser.Text[idx + 1];
                idx++;
            }
            if (Char.IsDigit(chr))
            {
                for (; idx < Parser.Length; idx++)
                {
                    chr = Parser.Text[idx];
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
                    else if(chr == 'u' || chr == 's')
                    {
                        ;
                    }
                    else
                    {
                        break;
                    }
                }
                Parser.Begin(idx);
                return new NumberLiteral(Parser.Text.Substring(startPosition, idx - startPosition));
            }
            // none
            else
            {
                return null;
            }
        }
    }
}
