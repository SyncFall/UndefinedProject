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
               (token = TryLiteralToken()) != null ||
               (token = TryIdentifierToken()) != null ||
               (token = TryStructureToken()) != null ||
               (token = TryCommentToken()) != null ||
               (token = TryOperationToken()) != null ||
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
            TokenSymbol tokenSymbol;
            if(Keywords.StringMap.KeyExist(str))
            {   
                tokenSymbol = Tokens.KeywordTokenStringMap.GetValue(str);
            }
            else if(Natives.StringMap.KeyExist(str))
            {
                tokenSymbol = Tokens.NativeTokenStringMap.GetValue(str);
            }
            else if(Accessors.StringMap.KeyExist(str))
            {
                tokenSymbol = Tokens.AccessorTokenStringMap.GetValue(str);
            }
            else
            {
                return null;
            }
            TextParser.Finish(endPositon);
            return tokenSymbol;
        }

        public TokenSymbol TryStructureToken()
        {
            for(int i=0; i<Tokens.StructureTokensArray.Length; i++)
            {
                TokenSymbol structureToken = Tokens.StructureTokensArray[i];
                if(TextParser.EqualChar(structureToken.String[0]))
                {
                    return structureToken;
                }
            }
            return null;
        }

        public OperationToken TryOperationToken()
        {
            for (int i = 0; i < Tokens.OperationTokenArray.Length; i++)
            {
                OperationToken operationToken = Tokens.OperationTokenArray[i];
                if (TextParser.EqualString(operationToken.String))
                {
                    return operationToken;
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
            string identifier = TextParser.TryIdentifier();
            if (identifier != null)
            {
                return new IdentifierToken(identifier);
            }
            return null;
        }

        public CommentToken TryCommentToken()
        {
            if (TextParser.EqualString("//"))
            {
                int commentStartPosition = TextParser.Position;
                TextParser.ToLineOrFileEnd();
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
