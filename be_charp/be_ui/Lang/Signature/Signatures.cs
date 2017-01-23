using Be.Runtime.Types;
using Bee.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.Runtime
{
    public enum SignatureType
    {
        Unknown,
        Seperator,
        Block,
        Keyword,
        Use,
        Scope,
        IdentifierPath,
        IdentifierPathElement,
        Identifier,
        Object,
        Member,
        Method,
        TypeDeclartion,
        ParameterDeclaration,
        ParameterElementDeclaration,
        Code,
        Statement,
        Expression,   
        ExpressionOperation,
        Operation,
        Operand,
        OperandOperation,
        LiteralAccess,
        VariableAccess,
        FunctionAccess,
        ArrayAccess,
    }

    public abstract class SignatureSymbol
    { 
        public SignatureType Type;

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
            return base.ToString() + " (group:"+UnknownToken.Group+", type:"+UnknownToken.Type+", string:'"+ UnknownToken.String+"')";
        }
    }

    public class KeywordSignature : SignatureSymbol
    {
        public KeywordToken KeywordToken;

        public KeywordSignature(KeywordToken keywordToken) : base(SignatureType.Keyword)
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

    public class UseSignature : SignatureSymbol
    {
        public KeywordSignature Keyword;
        public IdentifierPathSignature IdentifierPath;
        public SeperatorSignature Complete;

        public UseSignature() : base(SignatureType.Use)
        { }

        public override string ToString()
        {
            return base.ToString() + " (path:" + IdentifierPath + ")";
        }
    }

    public class ScopeSignature : SignatureSymbol
    {
        public KeywordSignature Keyword;
        public IdentifierPathSignature IdentifierPath;
        public BlockSignature BlockBegin;
        public ObjectSignatureList ObjectList;
        public BlockSignature BlockEnd;

        public ScopeSignature() : base(SignatureType.Scope)
        { }

        public override string ToString()
        {
            string str = base.ToString() + " (path:" + IdentifierPath  + ")\n";
            str += ObjectList.ToString();
            return str;
        }
    }

    public class IdentifierPathSignature : SignatureSymbol
    {
        public IdentifierPathElemementList PathElements = new IdentifierPathElemementList();

        public IdentifierPathSignature() : base(SignatureType.IdentifierPath)
        { }

        public override string ToString()
        {
            string str = "";
            for(int i=0; i<PathElements.Size(); i++)
            {
                str += PathElements.Get(i);
            }
            return str;
        }
    }

    public class IdentifierPathElemementList : ListCollection<IdentifierPathElementSignatur>
    { }

    public class IdentifierPathElementSignatur : SignatureSymbol
    {
        public IdentifierToken Identifier;
        public SeperatorSignature PointSeperator;

        public IdentifierPathElementSignatur() : base(SignatureType.IdentifierPathElement)
        { }

        public override string ToString()
        {
            string str = Identifier.String;
            if(PointSeperator != null)
            {
                str += PointSeperator.SeperatorToken.String;
            }
            return str;
        }
    }

    public class IdentifierSignature : SignatureSymbol
    {
        public IdentifierToken IdentifiereToken;
        
        public IdentifierSignature() : base(SignatureType.Identifier)
        { }

        public override string ToString()
        {
            return base.ToString()+"|"+IdentifiereToken.String;
        }
    }

    public class ObjectSignatureList : ListCollection<ObjectSignature>
    {
        public override string ToString()
        {
            string str = "";
            for(int i=0; i<Size(); i++)
            {
                str += Get(i);
            }
            return str;
        }
    }

    public class ObjectSignature : SignatureSymbol
    {
        public KeywordSignature Keyword;
        public IdentifierSignature Identifier;
        public BlockSignature BlockBegin;
        public MemberSignatureList Members = new MemberSignatureList();
        public MethodSignatureList Methods = new MethodSignatureList();
        public BlockSignature BlockEnd;

        public ObjectSignature() : base(SignatureType.Object)
        { }

        public override string ToString()
        {
            string str = "object (name:" + Identifier.IdentifiereToken.String + ")\n";
            str += Members;
            return str;
        }
    }

    public class MemberSignatureList : ListCollection<MemberSignature>
    {
        public override string ToString()
        {
            string str = "";
            for(int i=0; i<Size(); i++)
            {
                str += Get(i);
                if(i < Size() - 1)
                {
                    str += "\n";
                }
            }
            return str;
        }
    }

    public class MemberSignature : SignatureSymbol
    {
        public TypeDeclarationSignature TypeDeclaration;
        public SeperatorSignature Assigment;
        public ExpressionSignature AssigmentExpression;
        public SeperatorSignature Complete;

        public MemberSignature() : base(SignatureType.Member)
        { }

        public override string ToString()
        {
            string str = "member (";
            str += TypeDeclaration;
            return str  + ")";
        }
    }

    public class MethodSignatureList : ListCollection<MethodSignature>
    { }

    public class MethodSignature : SignatureSymbol
    {
        public TypeDeclarationSignature TypeDeclaration;
        public ParameterDeclarationSignature ParameterDeclaration;
        public BlockSignature BlockBegin;
        public CodeSignature Code;
        public BlockSignature BlockEnd;

        public MethodSignature() : base(SignatureType.Method)
        { }
    }

    public class TypeDeclarationSignature : SignatureSymbol
    {
        public NativeToken TypeNative;
        public IdentifierToken TypeIdentifier;
        public IdentifierSignature NameIdentifier;

        public TypeDeclarationSignature() : base(SignatureType.TypeDeclartion)
        { }

        public override string ToString()
        {
            string str = "";
            if(TypeNative != null)
            {
                str += "native:"+TypeNative.String;
            }
            else if(TypeIdentifier != null)
            {
                str += "object:"+TypeIdentifier.String;
            }
            str += ", name:" + NameIdentifier.IdentifiereToken.String;
            return str;
        }
    }

    public class ParameterDeclarationSignature : SignatureSymbol
    {
        public BlockSignature BlockBegin;
        public ParameterDeclarationElementList ParameterList = new ParameterDeclarationElementList();
        public BlockSignature BlockEnd;

        public ParameterDeclarationSignature() : base(SignatureType.ParameterDeclaration)
        { }
    }

    public class ParameterDeclarationElementList : ListCollection<ParameterDeclartionElementSignature>
    { } 

    public class ParameterDeclartionElementSignature : SignatureSymbol
    {
        public TypeDeclarationSignature TypeDeclaration;
        public SeperatorSignature ParameterSeperator;

        public ParameterDeclartionElementSignature() : base(SignatureType.ParameterElementDeclaration)
        { }
    }

    public class CodeSignature : SignatureSymbol
    {
        public CodeSignature() : base(SignatureType.Code)
        { }
    }

    public class StatementSignature : SignatureSymbol
    {
        public StatementSignature() : base(SignatureType.Statement)
        { }
    }

    public class ExpressionSignature : SignatureSymbol
    {
        public BlockSignature BlockBegin;
        public ExpressionSignature ChildExpression;
        public BlockSignature BlockEnd;
        public OperandSignatur Operand;
        public ExpressionOperationList ExpressionOperationList;

        public ExpressionSignature() : base(SignatureType.Expression)
        { }
    }

    public class OperationSignature : SignatureSymbol
    {
        public OperationToken OperationToken;

        public OperationSignature(OperationToken OperationToken) : base(SignatureType.Operation)
        {
            this.OperationToken = OperationToken;
        }
    }

    public class ExpressionOperationList : ListCollection<OperationPair>
    { }

    public class OperationPair
    {
        public OperationSignature Operation;
        public ExpressionSignature ExpressionPair;

        public OperationPair(OperationSignature Operation, ExpressionSignature ExpressionPair)
        {
            this.Operation = Operation;
            this.ExpressionPair = ExpressionPair;
        }
    }

    public class OperandSignatur : SignatureSymbol
    {
        public OperandAccessSignatureList AccessSignatureList = new OperandAccessSignatureList();

        public OperandSignatur() : base(SignatureType.Operand)
        { }
    }

    public class OperandAccessSignatureList : ListCollection<OperandAccessSignature>
    { }

    public abstract class OperandAccessSignature : SignatureSymbol
    {
        public OperandAccessSignature(SignatureType accessType) : base(accessType)
        { }
    }

    public class LiteralAccessSignature : OperandAccessSignature
    {
        public LiteralToken LiteralToken;

        public LiteralAccessSignature(LiteralToken LiteralToken) : base(SignatureType.LiteralAccess)
        {
            this.LiteralToken = LiteralToken;
        }
    }

    public class VariableAccessSignature : OperandAccessSignature
    {
        public IdentifierPathSignature IdentifierPath;

        public VariableAccessSignature(IdentifierPathSignature IdentifierPath) : base(SignatureType.VariableAccess)
        {
            this.IdentifierPath = IdentifierPath;
        }
    }

    public class FunctionAccessSignature : OperandAccessSignature
    {
        public FunctionAccessSignature() : base(SignatureType.FunctionAccess)
        { }
    }

    public class ArrayAccessSignature : OperandAccessSignature
    {
        public ArrayAccessSignature() : base(SignatureType.ArrayAccess)
        { }
    }
}
