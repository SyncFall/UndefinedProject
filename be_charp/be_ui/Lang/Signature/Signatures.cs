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
        Seperator,
        Block,
        Keyword,
        Using,
        Namespace,
        IdentifierPath,
        IdentifierPathElement,
        Identifier,
        Object,
        Member,
        Method,
        Property,
        TypeDeclartion,
        ParameterDeclaration,
        ParameterElementDeclaration,
        Code,
        Statement,
        Expression,
        
    }

    public abstract class SignatureSymbol
    {
        public SignatureType Type;
        public TokenList Tokens = new TokenList();

        public SignatureSymbol(SignatureType Type)
        {
            this.Type = Type;
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

        public UseSignature() : base(SignatureType.Using)
        { }
    }

    public class ScopeSignature : SignatureSymbol
    {
        public KeywordSignature Keyword;
        public IdentifierPathSignature IdentifierPath;
        public BlockSignature BlockBegin;
        public ObjectSignatureList ObjectList;
        public BlockSignature BlockEnd;

        public ScopeSignature() : base(SignatureType.Namespace)
        { }
    }

    public class IdentifierPathSignature : SignatureSymbol
    {
        public IdentifierPathSignature() : base(SignatureType.IdentifierPath)
        { }
    }

    public class IdentifierPathElementSignatur : SignatureSymbol
    {
        public IdentifierToken Identifier;
        public SeperatorSignature PointSeperator;

        public IdentifierPathElementSignatur() : base(SignatureType.IdentifierPathElement)
        { }
    }

    public class IdentifierSignature : SignatureSymbol
    {
        public IdentifierToken IdentifiereToken;
        
        public IdentifierSignature() : base(SignatureType.Identifier)
        { }
    }

    public class ObjectSignatureList : ListCollection<ObjectSignature>
    { }

    public class ObjectSignature : SignatureSymbol
    {
        public KeywordSignature Keyword;
        public IdentifierSignature Identifier;
        public BlockSignature BlockBegin;
        public MemberSignaturList Members = new MemberSignaturList();
        public MethodSignaturList Methods = new MethodSignaturList();
        public PropertySignaturList Properties = new PropertySignaturList();
        public BlockSignature BlockEnd;

        public ObjectSignature() : base(SignatureType.Object)
        { }
    }

    public class MemberSignaturList : ListCollection<MemberSignatur>
    { }

    public class MemberSignatur : SignatureSymbol
    {
        public TypeDeclarationSignatur TypeDeclaration;
        public SeperatorSignature Assigment;
        public ExpressionSignatur AssigmentExpression;
        public SeperatorSignature Complete;

        public MemberSignatur() : base(SignatureType.Member)
        { }
    }

    public class MethodSignaturList : ListCollection<MethodSignatur>
    { }

    public class MethodSignatur : SignatureSymbol
    {
        public TypeDeclarationSignatur TypeDeclaration;
        public ParameterDeclarationSignatur ParameterDeclaration;
        public BlockSignature BlockBegin;
        public CodeSignatur Code;
        public BlockSignature BlockEnd;

        public MethodSignatur() : base(SignatureType.Method)
        { }
    }

    public class PropertySignaturList : ListCollection<PropertySignatur>
    { }

    public class PropertySignatur : SignatureSymbol
    {
        public TypeDeclarationSignatur TypeDeclaration;

        public PropertySignatur() : base(SignatureType.Property)
        { }
    }

    public class TypeDeclarationSignatur : SignatureSymbol
    {
        public NativeToken TypeNative;
        public IdentifierToken TypeIdentifier;
        public IdentifierSignature NameIdentifier;

        public TypeDeclarationSignatur() : base(SignatureType.TypeDeclartion)
        { }
    }

    public class ParameterDeclarationSignatur : SignatureSymbol
    {
        public BlockSignature BlockBegin;
        public ParameterDeclarationElementList ParameterDeclarationElementList;
        public BlockSignature BlockEnd;

        public ParameterDeclarationSignatur() : base(SignatureType.ParameterDeclaration)
        { }
    }

    public class ParameterDeclarationElementList : ListCollection<ParameterDeclartionElementSignatur>
    { } 

    public class ParameterDeclartionElementSignatur : SignatureSymbol
    {
        public TypeDeclarationSignatur TypeDeclaration;
        public SeperatorSignature ParameterSeperator;

        public ParameterDeclartionElementSignatur() : base(SignatureType.ParameterElementDeclaration)
        { }
    }

    public class CodeSignatur : SignatureSymbol
    {
        public CodeSignatur() : base(SignatureType.Code)
        { }
    }

    public class StatementSignatur : SignatureSymbol
    {
        public StatementSignatur() : base(SignatureType.Statement)
        { }
    }

    public class ExpressionSignatur : SignatureSymbol
    {
        public ExpressionSignatur() : base(SignatureType.Expression)
        { }
    }
}
