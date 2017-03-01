
using System;

namespace Bee.Language
{
    public partial class SignatureParser
    {
        public TokenPointer TokenPointer;

        public SignatureParser(TokenContainer TokenContainer)
        {
            this.TokenPointer = new TokenPointer(TokenContainer.FirstTokenNode);
        }

        public bool IsEnd()
        {
            return (TokenPointer.Current == null);
        }

        public TokenSymbol Token
        {
            get{
                return (TokenPointer.Current != null ? TokenPointer.Current.Token : null);
            }
        }

        public TokenSymbol PrevToken
        {
            get{
                return (TokenPointer.Current != null && TokenPointer.Current.Prev != null ? TokenPointer.Current.Prev.Token : null);
            }
        }

        public TokenSymbol Next()
        {
            return (TokenPointer.Current != null && TokenPointer.Next() != null ? TokenPointer.Current.Token : null);
        }

        public void BeginStep()
        {
            TokenPointer.StepBegin();
        }

        public void ResetStep()
        {
            TokenPointer.StepReset();
        }

        public TokenSymbol TryToken(TokenType tokenType)
        {
            if (Token == null || Token.Type != tokenType)
            {
                return null;
            }
            Next();
            return PrevToken;
        }

        public TokenSymbol TryToken(KeywordType keywordType)
        {
            if (Token == null || !Token.IsKeyword(keywordType))
            {
                return null;
            }
            Next();
            return PrevToken;
        }

        public TokenSymbol TryToken(StructureType structureType)
        {
            if (Token == null || !Token.IsStructure(structureType))
            {
                return null;
            }
            Next();
            return PrevToken;
        }

        public TokenSymbol TryToken(NativeType nativeType)
        {
            if (Token == null || !Token.IsNative(nativeType))
            {
                return null;
            }
            Next();
            return PrevToken;
        }

        public TokenSymbol TryToken(StatementKeywordType statementType)
        {
            if (Token == null || !Token.IsStatement(statementType))
            {
                return null;
            }
            Next();
            return PrevToken;
        }

        public bool TrySpace()
        {
            bool hasSpace = false;
            while(Token != null && Token.IsSpace())
            {
                hasSpace = true;
                Next();
            }
            return hasSpace;
        }

        public SeperatorSignature TrySeperator(StructureType seperatorType)
        {
            TrySpace();
            if(Token == null || !Token.IsStructure(seperatorType))
            { 
                return null;
            }
            SeperatorSignature signatur = new SeperatorSignature(Token);
            Next();
            TrySpace();
            return signatur;
        }

        public BlockSignature TryBlock(StructureType blockType)
        {
            TrySpace();
            if (Token == null || !Token.IsStructure(blockType))
            {
                return null;
            }
            BlockSignature signatur = new BlockSignature(Token);
            Next();
            TrySpace();
            return signatur;
        }

        public UnknownSignatur TryUnknown()
        {
            if (Token == null)
            {
                return null;
            }
            TokenSymbol unknownToken = Token;
            Next();
            return new UnknownSignatur(unknownToken);
        }
    }

    public abstract class SignatureSymbol
    {
        public readonly SignatureType Type;

        public SignatureSymbol(SignatureType Type)
        {
            this.Type = Type;
        }

        public override string ToString()
        {
            return Type.ToString();
        }
    }

    public class UnknownSignatur : SignatureSymbol
    {
        public TokenSymbol UnknownToken;

        public UnknownSignatur(TokenSymbol UnknownToken) : base(SignatureType.Unknown)
        {
            this.UnknownToken = UnknownToken;
        }

        public override string ToString()
        {
            return base.ToString() + " (type:" + UnknownToken.Type + ", string:'" + UnknownToken.String + "')";
        }
    }

    public class KeywordSignature : SignatureSymbol
    {
        public TokenSymbol KeywordToken;

        public KeywordSignature(TokenSymbol keywordToken) : base(SignatureType.Keyword)
        {
            this.KeywordToken = keywordToken;
        }
    }

    public class SeperatorSignature : SignatureSymbol
    {
        public TokenSymbol SeperatorToken;

        public SeperatorSignature(TokenSymbol SeperatorToken) : base(SignatureType.Seperator)
        {
            this.SeperatorToken = SeperatorToken;
        }
    }

    public class BlockSignature : SignatureSymbol
    {
        public TokenSymbol BlockToken;

        public BlockSignature(TokenSymbol BlockToken) : base(SignatureType.Block)
        {
            this.BlockToken = BlockToken;
        }
    }
}
