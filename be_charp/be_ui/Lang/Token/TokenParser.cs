﻿using Be.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.Runtime
{
    public class TokenParser
    {
        public TokenReader TextParser;
        public LiteralParser LiteralParser;

        public TokenParser(string text)
        {
            this.TextParser = new TokenReader(text);
            this.LiteralParser = new LiteralParser(TextParser);
        }

        public bool IsEnd()
        {
            return (TextParser.Position >= TextParser.Length);
        }

        public TokenSymbol TryToken()
        {
            TokenSymbol token = null;
            if((token = TryKeywordToken()) != null ||
               (token = TryStructureToken()) != null ||
               (token = TryLiteralToken()) != null ||
               (token = TryIdentifierToken()) != null ||
               (token = TryCommentToken()) != null ||
               (token = TryUnknownToken()) != null
            ){
                return token;
            }
            else
            {
                throw new Exception("invald state");
            }
        }

        public TokenSymbol TryKeywordToken()
        {
            int startPosition = TextParser.Position;
            int endPositon = startPosition;
            for(int i=startPosition; i < TextParser.Length; i++)
            {
                char chr = TextParser.Text[i];
                if(!Char.IsLetter(chr) || !Char.IsLower(chr))
                {
                    endPositon = i;
                    break;
                }
            }
            if(startPosition == endPositon)
            {
                return null;
            }
            string str = TextParser.Text.Substring(startPosition, endPositon - startPosition);
            if(Keywords.StringMap.KeyExist(str))
            {
                TextParser.Finish(endPositon);
                return Tokens.KeywordTokenStringMap.GetValue(str);
            }
            else if(Natives.StringMap.KeyExist(str))
            {
                TextParser.Finish(endPositon);
                return Tokens.NativeTokenStringMap.GetValue(str);
            }
            return null;
        }

        public TokenSymbol TryStructureToken()
        {
            for(int i=0; i<Tokens.StructureTokensArray.Length; i++)
            {
                TokenSymbol structureToken = Tokens.StructureTokensArray[i];
                if(TextParser.EqualString(structureToken.String))
                {
                    return structureToken;
                }
            }
            return null;
        }

        public TokenSymbol TryLiteralToken()
        {
            LiteralSymbol literal = LiteralParser.TryLiteral();
            if(literal != null)
            {
                return new LiteralToken(literal, literal.String);
            }
            return null;
        }

        public IdentifierToken TryIdentifierToken()
        {
            string namePath = TextParser.GetNamePath();
            if (namePath != null)
            {
                return new IdentifierToken(namePath);
            }
            return null;
        }

        public CommentToken TryCommentToken()
        {
            if (TextParser.EqualString("//"))
            {
                int commentStartPosition = TextParser.Position;
                TextParser.ToLineEndOrFileEnd();
                int commentEndPosition = TextParser.Position;
                string commentData = TextParser.Text.Substring(commentStartPosition, commentEndPosition - commentStartPosition);
                return new CommentToken("//"+commentData, commentData);
            }
            else if (TextParser.EqualString("/*"))
            {
                int commentStartPosition = TextParser.Position;
                TextParser.ToStringOrFileEnd("*/");
                int commendEndPosition = TextParser.Position;
                string commentData = TextParser.Text.Substring(commentStartPosition, ((commendEndPosition - commentStartPosition) - 2));
                return new CommentToken("/*"+commentData+"*/", commentData);
            }
            return null;
        }

        public UnknownToken TryUnknownToken()
        {
            UnknownToken token = new UnknownToken(TextParser.Text[TextParser.Position] + "");
            TextParser.Finish(TextParser.Position + 1);
            return token;
        }
    }
}
