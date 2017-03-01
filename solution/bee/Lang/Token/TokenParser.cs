using Bee.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Language
{
    public class TokenParser
    {
        public TokenTextReader TextParser;

        public TokenParser(string text)
        {
            this.TextParser = new TokenTextReader(text);
        }

        public bool IsEnd()
        {
            return (TextParser.Position >= TextParser.Length);
        }

        public TokenSymbol TryToken()
        {
            if(IsEnd())
            {
                return null;
            }
            TokenSymbol token=null;
            if((token = TryKeywordOrIdentifierToken()) != null ||
               (token = TryCharOrStringToken()) != null ||
               (token = TryNumberToken()) != null ||
               (token = TryCommentToken()) != null ||
               (token = TryStructureToken()) != null ||
               (token = TryOperationToken()) != null ||
               (token = TryUnknownToken()) != null
            ){
                ;
            }
            return token;
        }

        public TokenSymbol TryKeywordOrIdentifierToken()
        {
            int idx;
            char _char;
            // check for lower alpha chars
            for (idx = TextParser.Position; idx < TextParser.Length; idx++)
            {
                _char = TextParser.Text[idx];
                if(_char < 'a' || _char > 'z')
                {
                    break;
                }
            }
            string alphaLower = null;
            if (idx > TextParser.Position)
            { 
                alphaLower = new string(TextParser.Text, TextParser.Position, idx - TextParser.Position);
                TextParser.Finish(idx);
            }
            // check for alpha numeric chars
            bool hasAlpha = (alphaLower != null);
            for(; idx < TextParser.Length; idx++)
            {
                _char = TextParser.Text[idx];
                if (!((_char >= 'a' && _char <= 'z' && (hasAlpha=true)) || (_char >= 'A' && _char <= 'Z' && (hasAlpha=true)) || (hasAlpha && _char >= '0' && _char <= '9')))
                {
                    break;
                }
            }
            string alphaNumeric = null;
            if (idx > TextParser.Position)
            {
                alphaNumeric = new string(TextParser.Text, TextParser.Position, idx - TextParser.Position);
                TextParser.Finish(idx);
            }
            // check for keyword
            if(alphaLower != null && alphaNumeric == null)
            {
                if (Tokens.KeywordMap.KeyExist(alphaLower))
                {
                    return Tokens.KeywordMap[alphaLower];
                }
            }
            // check for identifier
            if(alphaLower != null || alphaNumeric != null)
            {
                TokenSymbol identifierSymbol = new TokenSymbol(TokenType.Identifier, (alphaLower != null ? alphaLower : "") + (alphaNumeric != null ? alphaNumeric : ""), null);
                return identifierSymbol;
            }
            // none here
            return null;
        }

        public TokenSymbol TryCharOrStringToken()
        {
            int start = TextParser.Start;
            // check for string
            if (TextParser.EqualChar('"'))
            {
                TextParser.ToCharWithoutEscapeOrFileEnd('"');
                int end = TextParser.Start;
                string stringData = new string(TextParser.Text, start, end - start);
                return new TokenSymbol(TokenType.Literal, stringData, new LiteralSymbol(LiteralType.String, stringData));
            }
            // check for char
            if (TextParser.EqualChar('\''))
            {
                TextParser.ToCharWithoutEscapeOrFileEnd('\'');
                int end = TextParser.Start;
                string charData = new string(TextParser.Text, start, end - start);
                return new TokenSymbol(TokenType.Literal, charData, new LiteralSymbol(LiteralType.Char, charData));
            }
            // none here
            return null;
        }

        public TokenSymbol TryNumberToken()
        {
            int start = TextParser.Start;
            int idx;
            char _char;
            bool hasNumber = false;
            // check for begining numbers
            for (idx = TextParser.Start; idx < TextParser.Length; idx++)
            {
                _char = TextParser.Text[idx];
                if (_char < '0' || _char > '9')
                {
                    break;
                }
                TextParser.Finish(idx+1);
                hasNumber = true;
            }
            // check for point
            if(TextParser.EqualChar('.'))
            {
                // check for floating numbers
                for (idx = TextParser.Start; idx < TextParser.Length; idx++)
                {
                    _char = TextParser.Text[idx];
                    if (_char < '0' || _char > '9')
                    {
                        break;
                    }
                    TextParser.Finish(idx+1);
                    hasNumber = true;
                }
            }
            // check for number
            if (hasNumber)
            {
                // check for formating
                if (idx < TextParser.Length)
                {
                    _char = TextParser.Text[idx];
                    if (_char == 'f' || _char == 'd' || _char == 'i' || _char == 'l')
                    {
                        TextParser.Finish(++idx);
                    }
                }
                string numberData = new string(TextParser.Text, start, idx - start);
                return new TokenSymbol(TokenType.Literal, numberData, new LiteralSymbol(LiteralType.Number, numberData));
            }
            // none here
            TextParser.Finish(start);
            return null;
        }

        public TokenSymbol TryCommentToken()
        {
            int start = TextParser.Start;
            // check for line comment
            if (TextParser.EqualString("//"))
            {
                TextParser.ToLineOrFileEnd();
                int end = TextParser.Start;
                string commentData = new string(TextParser.Text, start, end - start);
                return new TokenSymbol(TokenType.Comment, commentData, null);
            }
            // check for block comment
            else if (TextParser.EqualString("/*"))
            {
                TextParser.ToStringOrFileEnd("*/");
                int end = TextParser.Start;
                string commentData = new string(TextParser.Text, start, end - start);
                return new TokenSymbol(TokenType.Comment, commentData, null);
            }
            // none here
            return null;
        }


        public TokenSymbol TryStructureToken()
        {
            for (int i = 0; i < Tokens.StructureArray.Length; i++)
            {
                if (TextParser.EqualChar(Tokens.StructureArray[i].String[0]))
                {
                    return Tokens.StructureArray[i];
                }
            }
            return null;
        }

        public TokenSymbol TryOperationToken()
        {
            for (int i = 0; i < Tokens.OperationArray.Length; i++)
            {
                if (TextParser.EqualString(Tokens.OperationArray[i].String))
                {
                    return Tokens.OperationArray[i];
                }
            }
            return null;
        }

        public TokenSymbol TryUnknownToken()
        {
            string unknown = TextParser.Text[TextParser.Start]+"";
            TextParser.Finish(TextParser.Start + 1);
            return new TokenSymbol(TokenType.Unknown, unknown, null);
        }
    }
}
