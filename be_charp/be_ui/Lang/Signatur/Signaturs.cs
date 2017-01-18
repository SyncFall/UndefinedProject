using Be.Runtime.Types;
using Bee.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.Runtime
{
    public enum SignaturType
    {
        Using,
        Namespace,
        IdentifierPath,
        Identifier,
        Object,
        Accessor,
        Method,
        Property,
        Member,
        Attribute,
        Generic,
    }

    public abstract class SignaturSymbol
    {
        public SignaturType Type;

        public SignaturSymbol(SignaturType Type)
        {
            this.Type = Type;
        }
    }

    public class UsingSignatur : SignaturSymbol
    {
        public KeywordToken KeywordToken;
        public TokenList SeperatorTokens;
        public IdentifierPathSignatur IdentifierPath;
        public TokenList CompleteTokens;

        public UsingSignatur() : base(SignaturType.Using)
        { }
    }

    public class NamespaceSignatur : SignaturSymbol
    {
        public KeywordToken KeywordToken;
        public TokenList SeperatorTokens;
        public IdentifierPathSignatur IdentifierPath;
        public TokenList BlockBeginTokens;
        public ObjectSignaturList ObjectList;
        public TokenList BlockEndTokens;

        public NamespaceSignatur() : base(SignaturType.Namespace)
        { }
    }

    public class IdentifierPathSignatur : SignaturSymbol
    {
        public TokenList Tokens;

        public IdentifierPathSignatur(TokenList Tokens) : base(SignaturType.Using)
        {
            this.Tokens = Tokens;
        }
    }

    public class IdentifierSignatur : SignaturSymbol
    {
        public IdentifierToken IdentifierToken;

        public IdentifierSignatur() : base(SignaturType.Identifier)
        { }
    }

    public class AccessorSignatur : SignaturSymbol
    {
        public AccessorToken AccessorToken;
        public TokenList SeperatorTokens;

        public AccessorSignatur() : base(SignaturType.Accessor)
        { }
    }

    public class ObjectSignaturList : ListCollection<ObjectSignatur>
    { }

    public class ObjectSignatur : SignaturSymbol
    {
        public AccessorSignatur Accessor;
        public KeywordToken KeywordToken;
        public TokenList SeperatorTokens;

        public ObjectSignatur() : base(SignaturType.Object)
        { }
    }
}
