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
        public ListCollection<TokenLine> TokesPerLine = new ListCollection<TokenLine>();

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

            if (TokesPerLine.Size() == 0)
            {
                TokesPerLine.Add(new TokenLine());
            }
            if (token.Type == Token.LineSpace)
            {
                TokesPerLine.Add(new TokenLine());
                return;
            }
            TokenLine tokenLine = TokesPerLine.Last();
            tokenLine.Add(token);
        }

        public void Operate(SourceFile SourceFile)
        {
            this.SourceFile = SourceFile;
            AllTokens.Clear();
            TokesPerLine.Clear();
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
