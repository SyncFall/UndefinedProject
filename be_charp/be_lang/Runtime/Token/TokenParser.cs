using Be.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.Runtime
{
    public class TokenParser
    {
        public TokenTextParser Parser;
        public TokenLiteralParser LiteralParser;

        public TokenParser(string text)
        {
            this.Parser = new TokenTextParser(text);
            this.LiteralParser = new TokenLiteralParser(Parser);
        }

        public bool IsEnd()
        {
            return (Parser.Position == Parser.Length);
        }

        public SpaceToken TrySpaceToken()
        {
            // white-space
            if (Parser.EqualChar(Tokens.WhiteSpace))
            {
                return new SpaceToken(Token.WhiteSpace, Tokens.WhiteSpace);
            }
            // tab-space
            else if (Parser.EqualChar(Tokens.TabSpace))
            {
                return new SpaceToken(Token.TabSpace, Tokens.TabSpace);
            }
            // line-space
            else if (Parser.EqualChar(Tokens.LineSpace))
            {
                return new SpaceToken(Token.LineSpace, Tokens.LineSpace);
            }
            // none
            return null;
        }

        public CommentToken TryCommentToken()
        {
            // line-comment
            if (Parser.EqualString(Tokens.CommentLine))
            {
                int commentStartPosition = Parser.Position;
                Parser.ToLineEndOrFileEnd();
                int commentEndPosition = Parser.Position;
                string commentText = Parser.Text.Substring(commentStartPosition, commentEndPosition - commentStartPosition);
                return new CommentToken(Token.CommentLine, Tokens.CommentLine + commentText);
            }
            // block-comment
            else if (Parser.EqualString(Tokens.CommentBlockStart))
            {
                int commentStartPosition = Parser.Position;
                Parser.ToStringOrFileEnd(Tokens.CommentBlockEnd);
                int commendEndPosition = Parser.Position;
                string commentText = Parser.Text.Substring(commentStartPosition, ((commendEndPosition - commentStartPosition) - 2));
                return new CommentToken(Token.CommentBlock, Tokens.CommentBlockStart + commentText + Tokens.CommentBlockEnd);
            }
            // none
            else
            {
                return null;
            }
        }

        public RegionToken TryRegionToken()
        {
            // region token
            if (Parser.EqualString(Tokens.RegionStart))
            {
                return new RegionToken(Token.RegionStart, Tokens.RegionStart);
            }
            else if (Parser.EqualString(Tokens.RegionEnd))
            {
                return new RegionToken(Token.RegionEnd, Tokens.RegionEnd);
            }
            return null;
        }

        public ProcessorToken TryProcessorToken()
        {
            // processor token
            if (Parser.EqualString(Tokens.ProcessorIf))
            {
                return new ProcessorToken(Token.ProcessorIf, Tokens.ProcessorIf);
            }
            else if (Parser.EqualString(Tokens.ProcessorElse))
            {
                return new ProcessorToken(Token.ProcessorElse, Tokens.ProcessorElse);
            }
            else if (Parser.EqualString(Tokens.ProcessorElseIf))
            {
                return new ProcessorToken(Token.ProcessorElseIf, Tokens.ProcessorElseIf);
            }
            else if (Parser.EqualString(Tokens.ProcessorEndIf))
            {
                return new ProcessorToken(Token.ProcessorEndIf, Tokens.ProcessorEndIf);
            }
            return null;
        }

        public StatementToken TryStatementToken()
        {
            // complete-statement
            if (Parser.EqualChar(Tokens.Complete))
            {
                return new StatementToken(Token.Complete, Tokens.Complete);
            }
            // assigment-statement
            else if (Parser.EqualChar(Tokens.Assigment))
            {
                return new StatementToken(Token.Assigment, Tokens.Assigment);
            }
            // comma-statement
            else if (Parser.EqualChar(Tokens.Comma))
            {
                return new StatementToken(Token.Comma, Tokens.Comma);
            }
            // point-statement
            else if (Parser.EqualChar(Tokens.Point))
            {
                return new StatementToken(Token.Point, Tokens.Point);
            }
            // seperator-statement
            else if (Parser.EqualChar(Tokens.Seperator))
            {
                return new StatementToken(Token.Seperator, Tokens.Seperator);
            }
            // none
            return null;
        }

        public BlockToken TryBlockToken()
        {
            // block token
            if (Parser.EqualChar(Tokens.BlockStart))
            {
                return new BlockToken(Token.BlockStart, Tokens.BlockStart);
            }
            else if (Parser.EqualChar(Tokens.BlockEnd))
            {
                return new BlockToken(Token.BlockEnd, Tokens.BlockEnd);
            }
            else if (Parser.EqualChar(Tokens.ClosingStart))
            {
                return new BlockToken(Token.ClosingStart, Tokens.ClosingStart);
            }
            else if (Parser.EqualChar(Tokens.ClosingEnd))
            {
                return new BlockToken(Token.ClosingEnd, Tokens.ClosingEnd);
            }
            else if (Parser.EqualChar(Tokens.BracketStart))
            {
                return new BlockToken(Token.BracketStart, Tokens.BracketStart);
            }
            else if (Parser.EqualChar(Tokens.BracketEnd))
            {
                return new BlockToken(Token.BracketEnd, Tokens.BracketEnd);
            }
            else if (Parser.EqualChar(Tokens.Backward))
            {
                return new BlockToken(Token.Backward, Tokens.Backward);
            }
            else if (Parser.EqualChar(Tokens.Forward))
            {
                return new BlockToken(Token.Forward, Tokens.Forward);
            }
            // none
            else
            {
                return null;
            }
        }

        public LiteralToken TryLiteralToken()
        {
            // literal token
            LiteralToken token;
            if((token = LiteralParser.TryBool()) != null ||
               (token = LiteralParser.TryString()) != null ||
               (token = LiteralParser.TryChar()) != null ||
               (token = LiteralParser.TryNumber()) != null ||
               (token = LiteralParser.TryObjectConstant()) != null
            ){
                return token;
            }
            return null;
        }

        public KeywordToken TryKeywordToken()
        {
            // keyword token
            int startPosition = Parser.Position;
            string name = Parser.GetNamePath();
            if (name == null)
            {
                return null;
            }
            else if (Keywords.StringMap.KeyExist(name))
            {
                return new KeywordToken(Keywords.StringMap.GetValue(name));
            }
            else
            {
                Parser.Begin(startPosition);
                return null;
            }
        }

        public NativeToken TryNativeToken()
        {
            // native token
            int startPosition = Parser.Position;
            string name = Parser.GetNamePath();
            if (name == null)
            {
                return null;
            }
            else if (Natives.StringMap.KeyExist(name))
            {
                return new NativeToken(Natives.StringMap.GetValue(name));
            }
            else
            {
                Parser.Begin(startPosition);
                return null;
            }
        }

        public NamePathToken TryNamePathToken()
        {
            // namepath token
            string namePath = Parser.GetNamePath();
            if (namePath != null)
            {
                return new NamePathToken(namePath);
            }
            return null;
        }

        public UnknownToken TryUnknownToken()
        {
            // unknown token
            string unknownTextString = Parser.Text.Substring(Parser.Position, 1);
            Parser.Begin(Parser.Position + 1);
            return new UnknownToken(unknownTextString);
        }
    }
}
