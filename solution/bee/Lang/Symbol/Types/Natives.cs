using Bee.Library;

namespace Bee.Language
{
    public enum NativeType
    {
        Void,
        Bool,
        Byte,
        Char,
        String,
        Int,
        Long,
        Float,
        Double,
        Number,
        Object,
        Enum,
        Type,
        Var,
        Func,
        List,
        Map,
    }

    public class NativeSymbol
    {
        public readonly NativeType Type;
        public readonly string String;

        public NativeSymbol(string SymbolString, NativeType Type)
        {
            this.Type = Type;
            this.String = SymbolString;
        }
    }

    public static class Natives
    {
        public static readonly NativeSymbol[] Array = new NativeSymbol[]
        {
            new NativeSymbol("void", NativeType.Void),
            new NativeSymbol("bool", NativeType.Bool),
            new NativeSymbol("byte", NativeType.Byte),
            new NativeSymbol("int", NativeType.Int),
            new NativeSymbol("long", NativeType.Long),
            new NativeSymbol("float", NativeType.Float),
            new NativeSymbol("double", NativeType.Double),
            new NativeSymbol("number", NativeType.Number),
            new NativeSymbol("char", NativeType.Char),
            new NativeSymbol("string", NativeType.String),
            new NativeSymbol("object", NativeType.Object),
            new NativeSymbol("func", NativeType.Func),
            new NativeSymbol("type", NativeType.Type),
            new NativeSymbol("var", NativeType.Var),
            new NativeSymbol("enum", NativeType.Enum),
            new NativeSymbol("list", NativeType.List),
            new NativeSymbol("map", NativeType.Map),
        };
        public static readonly MapCollection<NativeType, NativeSymbol> EnumMap = new MapCollection<NativeType, NativeSymbol>();

        static Natives()
        {
            for(int i=0; i<Array.Length; i++)
            {
                NativeSymbol symbol = Array[i];
                EnumMap.Put(symbol.Type, symbol);
            }
        }
    }
}
