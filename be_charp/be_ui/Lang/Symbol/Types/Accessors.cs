using Bee.Library;

namespace Bee.Language
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
        public static readonly AccessorSymbol[] Array = new AccessorSymbol[]
        {
            new AccessorSymbol("public", AccessorType.Public),
            new AccessorSymbol("private", AccessorType.Private),
            new AccessorSymbol("protected", AccessorType.Protected),
            new AccessorSymbol("internal",  AccessorType.Internal),
        };
        public static readonly MapCollection<string, AccessorSymbol> StringMap = new MapCollection<string, AccessorSymbol>();

        static Accessors()
        {
            for(int i=0; i<Array.Length; i++)
            {
                AccessorSymbol symbol = Array[i];
                StringMap.Put(symbol.String, symbol);
            }
        }
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
