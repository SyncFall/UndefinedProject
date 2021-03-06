﻿using feltic.Language;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public class TokenParser
    {
        public TokenTextReader TextParser;

        public TokenParser(SourceText source)
        {
            this.TextParser = new TokenTextReader(source);
        }

        public bool IsEnd
        {
            get
            {
                return TextParser.Position >= TextParser.Length;
            }
        }

        public Symbol TryToken()
        {
            if(IsEnd)
                return null;
            Symbol token =null;
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
        
        public Symbol TryKeywordOrIdentifierToken()
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
            int alphaLowerStart = -1;
            int alphaLowerLen = 0;
            if (idx > TextParser.Position)
            {
                alphaLowerStart = TextParser.Position;
                alphaLowerLen = idx - TextParser.Position;
                TextParser.Finish(idx);
            }
            // check for alpha numeric chars
            bool hasAlpha = (alphaLowerStart != -1);
            for(; idx < TextParser.Length; idx++)
            {
                _char = TextParser.Text[idx];
                if (!((_char >= 'a' && _char <= 'z' && (hasAlpha|=true)) || (_char >= 'A' && _char <= 'Z' && (hasAlpha|=true)) || (hasAlpha && _char >= '0' && _char <= '9')))
                {
                    break;
                }
            }
            int alphaNumericStart = -1;
            int alphaNumericLen = 0;
            if (idx > TextParser.Position)
            {
                alphaNumericStart = TextParser.Position;
                alphaNumericLen = idx - TextParser.Position;
                TextParser.Finish(idx);
            }
            // check for keyword
            if(alphaLowerLen > 0 && alphaNumericLen == 0)
            {
                Symbol tokenSymbol = SymbolLookup.Keywords.FindSymbol(ref TextParser.Text, alphaLowerStart, alphaLowerStart + alphaLowerLen);
                if(tokenSymbol != null)
                {
                    return tokenSymbol;
                }
            }
            // check for identifier
            if(alphaLowerLen > 0 || alphaNumericLen > 0)
            {
                char[] charArray = new char[alphaLowerLen + alphaNumericLen];
                int i,x=0;
                for(i=0; i<alphaLowerLen; i++){ 
                    charArray[x++] = TextParser.Text[alphaLowerStart+i];
                }
                for(i=0; i<alphaNumericLen; i++){
                    charArray[x++] = TextParser.Text[alphaNumericStart+i];
                }
                return new Symbol(new string(charArray), (int)TokenType.Identifier);
            }
            return null;
        }

        public Symbol TryCharOrStringToken()
        {
            int start = TextParser.Start;
            // check for string
            if (TextParser.EqualChar('"'))
            {
                TextParser.ToCharWithoutEscapeOrFileEnd('"');
                int end = TextParser.Start;
                string stringData = new string(TextParser.Text, start, end - start);
                return new Symbol(stringData, (int)TokenType.Literal, (int)LiteralType.String);
            }
            // check for char
            if (TextParser.EqualChar('\''))
            {
                TextParser.ToCharWithoutEscapeOrFileEnd('\'');
                int end = TextParser.Start;
                string charData = new string(TextParser.Text, start, end - start);
                return new Symbol(charData, (int)TokenType.Literal, (int)LiteralType.Char);
            }
            return null;
        }

        public Symbol TryNumberToken()
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
                    else if(TextParser.EqualString("em") || TextParser.EqualString("px") || TextParser.EqualString("pc"))
                    {
                        idx = TextParser.Position;
                    }
                }
                string numberData = new string(TextParser.Text, start, idx - start);
                return new Symbol(numberData, (int)TokenType.Literal, (int)LiteralType.Number);
            }
            // none here
            TextParser.Finish(start);
            return null;
        }

        public Symbol TryCommentToken()
        {
            int start = TextParser.Start;
            // check for line comment
            if (TextParser.EqualString("//"))
            {
                TextParser.ToLineOrFileEnd();
                int end = TextParser.Start;
                string commentData = new string(TextParser.Text, start, end - start);
                return new Symbol(commentData, (int)TokenType.Comment, 1);
            }
            // check for block comment
            else if (TextParser.EqualString("/*"))
            {
                TextParser.ToStringOrFileEnd("*/");
                int end = TextParser.Start;
                string commentData = new string(TextParser.Text, start, end - start);
                return new Symbol(commentData, (int)TokenType.Comment, 2);
            }
            return null;
        }


        public Symbol TryStructureToken()
        {
            if(TextParser.Text[TextParser.Start] > (char)127)
            {
                return null;
            }
            Symbol symbol = SymbolLookup.Structures.FindSymbol(ref TextParser.Text, TextParser.Start, TextParser.Start+1);
            if(symbol != null)
            {
                TextParser.Finish(TextParser.Start+1);
                return symbol;
            }
            return null;
        }

        public Symbol TryOperationToken()
        {
            for(int i=0; i<SymbolLookup.Operations.Length; i++)
            {
                if(TextParser.EqualString(SymbolLookup.Operations[i].String))
                {
                    return SymbolLookup.Operations[i];
                }
            }
            return null;
        }

        public Symbol TryUnknownToken()
        {
            string unknown = TextParser.Text[TextParser.Start]+"";
            TextParser.Finish(TextParser.Start+1);
            return new Symbol(unknown, (int)TokenType.Unknown);
        }
    }
}
