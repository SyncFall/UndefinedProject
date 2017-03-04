using Feltic.Library;

namespace Feltic.Language
{
    public enum AccessorType
    {
        Public,
        Private,
        Protected,
        Internal,
    }

    public static class Accessors
    {
        public static readonly AccessorSymbol[] Array =
        {
            new AccessorSymbol("public", AccessorType.Public),
            new AccessorSymbol("private", AccessorType.Private),
            new AccessorSymbol("protected", AccessorType.Protected),
            new AccessorSymbol("internal",  AccessorType.Internal),
        };
    }

    public class AccessorSymbol
    {
        public readonly AccessorType Type;
        public readonly string String;

        public AccessorSymbol(string SymbolString, AccessorType Type)
        {
            this.Type = Type;
            this.String = SymbolString;
        }
    }
}
