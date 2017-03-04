
using Feltic.Library;
using System;

namespace Feltic.Language
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

        public TokenSymbol NextToken()
        {
            return (TokenPointer.Current != null && TokenPointer.Next() != null ? TokenPointer.Current.Token : null);
        }

        public bool BeginStep()
        {
            return TokenPointer.StepBegin();
        }

        public void ResetStep()
        {
            TokenPointer.StepReset();
        }

        public void CommitStep()
        {
            TokenPointer.StepCommit();
        }

        public TokenSymbol TryToken(TokenType tokenType)
        {
            if (Token == null || Token.Type != tokenType)
            {
                return null;
            }
            NextToken();
            return PrevToken;
        }

        public TokenSymbol TryToken(KeywordType keywordType)
        {
            if (Token == null || !Token.IsKeyword(keywordType))
            {
                return null;
            }
            NextToken();
            return PrevToken;
        }

        public TokenSymbol TryToken(StructureType structureType)
        {
            if (Token == null || !Token.IsStructure(structureType))
            {
                return null;
            }
            NextToken();
            return PrevToken;
        }

        public TokenSymbol TryToken(NativeType nativeType)
        {
            if (Token == null || !Token.IsNative(nativeType))
            {
                return null;
            }
            NextToken();
            return PrevToken;
        }

        public TokenSymbol TryToken(StatementKeywordType statementType)
        {
            if (Token == null || !Token.IsStatement(statementType))
            {
                return null;
            }
            NextToken();
            return PrevToken;
        }

        public TokenSymbol TryToken(OperationType statementType)
        {
            if (Token == null || !Token.isOperation(statementType))
            {
                return null;
            }
            NextToken();
            return PrevToken;
        }

        public bool TrySpace()
        {
            bool hasSpace = false;
            while(Token != null && Token.IsSpace())
            {
                hasSpace = true;
                NextToken();
            }
            return hasSpace;
        }

        public TokenSymbol TryNonSpace(StructureType structure)
        {
            TrySpace();
            if(Token == null || !Token.IsStructure(structure))
            { 
                return null;
            }
            TokenSymbol token = Token;
            NextToken();
            TrySpace();
            return token;
        }

        public UnknownSignatur TryUnknown()
        {
            if (Token == null)
            {
                return null;
            }
            TokenSymbol unknownToken = Token;
            NextToken();
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

    public class SignatureList : ListCollection<SignatureSymbol>
    { }

    public class UnknownSignatur : SignatureSymbol
    {
        public TokenSymbol Token;

        public UnknownSignatur(TokenSymbol UnknownToken) : base(SignatureType.Unknown)
        {
            this.Token = UnknownToken;
        }

        public override string ToString()
        {
            return base.ToString() + " (type:" + Token.Type + ", string:'" + Token.String + "')";
        }
    }
}
