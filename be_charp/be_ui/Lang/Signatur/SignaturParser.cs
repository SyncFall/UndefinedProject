using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Be.Runtime
{
    public class SignaturParser
    { 
        public TokenPointer TokenPointer;

        public SignaturParser(TokenContainer TokenContainer)
        {
            this.TokenPointer = new TokenPointer(TokenContainer.FirstTokenNode());
        }

        public bool IsEnd()
        {
            return (TokenPointer.Current == null);
        }

        public TokenSymbol Token
        {
            get
            {
                if (TokenPointer.Current != null)
                {
                    return TokenPointer.Current.Token;
                }
                return null;
            }
        }

        public TokenSymbol PrevToken
        {
            get
            {
                if(TokenPointer.Current != null && TokenPointer.Current.Prev != null)
                {
                    return TokenPointer.Current.Prev.Token;
                }
                return null;
            }
        }

        public TokenSymbol Next()
        {
            if (TokenPointer.Current != null && TokenPointer.Next() != null)
            {
                return Token;
            }
            return null;
        }

        public void BeginStep()
        {
            TokenPointer.StepBegin();
        }

        public void ResetStep()
        {
            TokenPointer.StepReset();
        }
	
        public TokenList TrySpace()
        {
            TokenList spaceTokens = new TokenList();
            while(Token != null && (Token.Group == TokenGroup.Space || Token.Group == TokenGroup.Comment))
            {
                spaceTokens.Add(Token);
                TokenPointer.Next();
            }
            return (spaceTokens.Size() > 0 ? spaceTokens : null);
        }

        public TokenList TrySeperator()
        {
            TokenList seperatorTokens = new TokenList();
            while(Token != null && (Token.Group == TokenGroup.Space || Token.Group == TokenGroup.Statement || Token.Group == TokenGroup.Block))
            {
                seperatorTokens.Add(Token);
                TokenPointer.Next();
            }
            return (seperatorTokens.Size() > 0 ? seperatorTokens : null);
        }

        public KeywordToken TryKeywordToken(KeywordType keywordType)
        {
            if(Token != null && Token.Type == TokenType.Keyword && Token.Group == TokenGroup.Keyword && (Token as KeywordToken).KeywordSymbol.Type == keywordType)
            {
                KeywordToken keywordToken = (Token as KeywordToken);
                Next();
                return keywordToken;
            }
            return null;
        }

        public TokenSymbol TryToken(TokenType tokenType)
        {
            if(Token != null && Token.Type == tokenType)
            {
                TokenSymbol tokenSymbol = Token;
                Next();
                return tokenSymbol;
            }
            return null;
        }

        public TokenList TryComplete()
        {
            BeginStep();
            TokenList completeTokens = new TokenList();
            TokenList beginingSpaces = TrySpace();
            if (beginingSpaces != null)
            {
                completeTokens.Add(beginingSpaces);
            }
            TokenSymbol completeToken = TryToken(TokenType.Complete);
            if (completeToken == null)
            {
                ResetStep();
                return null;
            }
            completeTokens.Add(completeToken);
            return completeTokens;
        }

        public IdentifierPathSignatur TryPathBreakOn(TokenType breakTokenType)
        {
            TokenList pathTokens = new TokenList();
            while(Token != null && (Token.Type == TokenType.Identifier || Token.Type == TokenType.Seperator) && (Token.Type != breakTokenType))
            {
                pathTokens.Add(Token);
            }
            return (pathTokens.Size() > 0 ? new IdentifierPathSignatur(pathTokens) : null);
        }

        public TokenList TryBlock(TokenType blockType)
        {
            BeginStep();
            TokenList blockTokens = new TokenList();
            TokenList beginSpaces = TrySpace();
            if (beginSpaces != null)
            {
                blockTokens.Add(beginSpaces);
            }
            TokenSymbol blockToken = TryToken(blockType);
            if(blockToken == null)
            {
                ResetStep();
                return null;
            }
            blockTokens.Add(blockToken);
            TokenList endSpaces = TrySpace();
            if(endSpaces != null)
            {
                blockTokens.Add(endSpaces);
            }
            return blockTokens;
        }

        public UsingSignatur TryUsing()
        {
            if(TryKeywordToken(KeywordType.Using) == null)
            {
                return null;
            }
            UsingSignatur signatur = new UsingSignatur();
            signatur.KeywordToken = PrevToken as KeywordToken;
            signatur.SeperatorTokens = TrySeperator();
            signatur.IdentifierPath = TryPathBreakOn(TokenType.Complete);
            signatur.CompleteTokens = TryComplete();
            return signatur;
        }

        public NamespaceSignatur TryNamespace()
        {
            if(TryKeywordToken(KeywordType.Namespace) == null)
            {
                return null;
            }
            NamespaceSignatur signatur = new NamespaceSignatur();
            signatur.KeywordToken = PrevToken as KeywordToken;
            signatur.SeperatorTokens = TrySeperator();
            signatur.IdentifierPath = TryPathBreakOn(TokenType.BlockBegin);
            signatur.BlockBeginTokens = TryBlock(TokenType.BlockBegin);
            signatur.ObjectList = TryObjectList();
            signatur.BlockEndTokens = TryBlock(TokenType.BlockEnd);
            return signatur;
        }

        public IdentifierSignatur TryIdentifier()
        {
            if(TryToken(TokenType.Identifier) == null)
            {
                return null;
            }
            IdentifierSignatur signatur = new IdentifierSignatur();
            signatur.IdentifierToken = PrevToken as IdentifierToken;
            return signatur;
        }
       

        public ObjectSignaturList TryObjectList()
        {
            ObjectSignaturList list = new ObjectSignaturList();
            ObjectSignatur signatur;
            while((signatur = TryObject()) != null)
            {
                list.Add(signatur);
            }
            return (list.Size() > 0 ? list : null);
        }

        public AccessorSignatur TryAccessor()
        {
            if(TryToken(TokenType.Accessor) == null)
            {
                return null;
            }
            AccessorSignatur signatur = new AccessorSignatur();
            signatur.AccessorToken = PrevToken as AccessorToken;
            signatur.SeperatorTokens = TrySeperator();
            return signatur;
        }

        public ObjectSignatur TryObject()
        {
            BeginStep();
            AccessorSignatur accessor = TryAccessor();
        }
	}
}
