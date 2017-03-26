using feltic.Library;

namespace feltic.Language
{
    public enum AccessorType
    {
        None=0,
        Public,
        Private,
        Protected,
        Internal,
    }

    public static class Accessors
    {
        public static readonly AccessorSymbol[] Array =
        {
            new AccessorSymbol(AccessorType.Public, "public"),
            new AccessorSymbol(AccessorType.Private, "private"),
            new AccessorSymbol(AccessorType.Protected, "protected"),
            new AccessorSymbol(AccessorType.Internal, "internal"),
        };
    }

    public class AccessorSymbol : Symbol
    {
        public AccessorSymbol(AccessorType Type, string String) : base(String, (int)TokenType.Accessor, (int)Type)
        { }
    }
}
