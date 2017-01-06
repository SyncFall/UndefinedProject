using Be.Runtime.Parse;
using Be.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.Runtime
{
    public class TokenLine : ListCollection<TokenSymbol>
    {
        public string TextString = "";
        public int TextCount = 0;

        public override void Add(TokenSymbol token)
        {
            TextString += token.TextString;
            TextCount += token.TextCount;
            base.Add(token);
        }
    }

    public class TokenContainer
    {
        public SourceFile SourceFile;
        public TokenParser Parser;
        public ListCollection<TokenSymbol> AllTokens = new ListCollection<TokenSymbol>();
        public ListCollection<TokenLine> TokenLines = new ListCollection<TokenLine>();

        public TokenContainer()
        { }

        private void AddToken(TokenSymbol token)
        {
            if (AllTokens.Size() > 0)
            {
                TokenSymbol previousToken = AllTokens.Last();
                previousToken.NextToken = token;
                token.PrevToken = previousToken;
            }
            AllTokens.Add(token);

            if (TokenLines.Size() == 0)
            {
                TokenLines.Add(new TokenLine());
            }
            if (token.Type == Token.LineSpace)
            {
                TokenLines.Add(new TokenLine());
                return;
            }
            TokenLine tokenLine = TokenLines.Get(TokenLines.Size() - 1);
            tokenLine.Add(token);
        }

        public void Operate(SourceFile SourceFile)
        {
            this.SourceFile = SourceFile;
            AllTokens.Clear();
            TokenLines.Clear();
            Parser = new TokenParser(SourceFile.Source);
            while(true)
            {
                if(Parser.IsEnd())
                {
                    break;
                }
                TokenSymbol token;
                if((token = Parser.TrySpaceToken()) != null ||
                   (token = Parser.TryStatementToken()) != null ||
                   (token = Parser.TryBlockToken()) != null ||
                   (token = Parser.TryLiteralToken()) != null ||
                   (token = Parser.TryCommentToken()) != null ||
                   (token = Parser.TryRegionToken()) != null ||
                   (token = Parser.TryProcessorToken()) != null ||
                   (token = Parser.TryKeywordToken()) != null ||
                   (token = Parser.TryNativeToken()) != null ||
                   (token = Parser.TryNamePathToken()) != null ||
                   (token = Parser.TryUnknownToken()) != null
                ){
                    AddToken(token);
                }
            }
        }

    }
}
