
using feltic.Library;
using System;

namespace feltic.Language
{
    public partial class SignatureParser
    {
        public TokenPointer Pointer;

        public SignatureParser(TokenContainer TokenContainer)
        {
            this.Pointer = new TokenPointer(TokenContainer);
        }

        public bool IsEnd
        {
            get
            {
                return (Pointer.Current == null);
            }
        }

        public Symbol Token
        {
            get{
                return Pointer.Current;
            }
        }

        public Symbol PrevToken
        {
            get{
                return Pointer.AtPosition(Pointer.Position-1);
            }
        }

        public Symbol NextToken()
        {
            return Pointer.Next;
        }

        public bool BeginStep()
        {
            return Pointer.Begin();
        }

        public void ResetStep()
        {
            Pointer.Reset();
        }

        public void CommitStep()
        {
            Pointer.Commit();
        }

        private Symbol TryToken(int Group, int Type=0)
        {
            if(Token == null || Token.Group != Group || (Type != 0 && Token.Type != Type))
            {
                return null;
            }
            NextToken();
            return PrevToken;
        }

        public Symbol TryToken(TokenType tokenType)
        {
            return TryToken((int)(tokenType));
        }

        public Symbol TryToken(ObjectType keywordType)
        {
            return TryToken((int)TokenType.Object, (int)keywordType);
        }

        public Symbol TryToken(StructureType structureType)
        {
            return TryToken((int)TokenType.Structure, (int)structureType);
        }

        public Symbol TryToken(NativeType nativeType)
        {
            return TryToken((int)TokenType.Native, (int)nativeType);
        }

        public Symbol TryToken(StatementKeywordType statementType)
        {
            return TryToken((int)TokenType.Statement, (int)statementType);
        }

        public Symbol TryToken(OperationType operationType)
        {
            return TryToken((int)TokenType.Statement, (int)operationType);
        }

        public Symbol TryToken(VisualType visualType)
        {
            return TryToken((int)TokenType.Visual, (int)visualType);
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

        private Symbol TryNonSpace(int Group, int Type=0)
        {
            TrySpace();
            if (Token == null || Token.Group != Group || (Type != 0 && Token.Type != Type))
            {
                return null;
            }
            Symbol token = Token;
            NextToken();
            TrySpace();
            return token;
        }

        public Symbol TryNonSpace(TokenType tokenType)
        {
            return TryNonSpace((int)tokenType, 0);
        }

        public Symbol TryNonSpace(StructureType structure)
        {
            return TryNonSpace((int)TokenType.Structure, (int)structure);
        }

        public Symbol TryNonSpace(OperationType operation)
        {
            return TryNonSpace((int)TokenType.Operation, (int)operation);
        }

        public UnknownSignatur TryUnknown()
        {
            if(Token == null) return null;
            NextToken();
            return new UnknownSignatur(PrevToken);
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
            return "signature("+Type.ToString()+")";
        }
    }

    public class SignatureList : ListCollection<SignatureSymbol>
    {
    }

    public class UnknownSignatur : SignatureSymbol
    {
        public Symbol Token;

        public UnknownSignatur(Symbol UnknownToken) : base(SignatureType.Unknown)
        {
            this.Token = UnknownToken;
        }

        public override string ToString()
        {
            return "unknown(group:"+Token.Group+",type:"+Token.Type+",string:'"+Token.String+"')";
        }
    }
}
