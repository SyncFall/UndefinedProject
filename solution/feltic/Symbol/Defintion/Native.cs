using feltic.Library;

namespace feltic.Language
{
    public enum NativeType
    {
        None=0,
        Void,
        Bool,
        Byte,
        Char,
        String,
        Int,
        Long,
        Float,
        Big,
        Double,
        High,
        Object,
        Enum,
        Type,
        Var,
        Func,
        State,
    }

    public static class Natives
    {
        public static readonly NativeSymbol[] Array =
        {
            new NativeSymbol(NativeType.Void, "void"),
            new NativeSymbol(NativeType.Bool, "bool"),
            new NativeSymbol(NativeType.Byte, "byte"),
            new NativeSymbol(NativeType.Int, "int"),
            new NativeSymbol(NativeType.Long, "long"),
            new NativeSymbol(NativeType.Big, "big"),
            new NativeSymbol(NativeType.Float, "float"),
            new NativeSymbol(NativeType.Double, "double"),
            new NativeSymbol(NativeType.High, "high"),
            new NativeSymbol(NativeType.Char, "char"),
            new NativeSymbol(NativeType.String, "string"),
            new NativeSymbol(NativeType.Object, "object"),
            new NativeSymbol(NativeType.Func, "func"),
            new NativeSymbol(NativeType.State, "state"),
            new NativeSymbol(NativeType.Type, "type"),
            new NativeSymbol(NativeType.Var, "var"),
            new NativeSymbol(NativeType.Enum, "enum"),
        };
    }

    public class NativeSymbol : Symbol
    {
        public NativeSymbol(NativeType Type, string String) : base(String, (int)TokenType.Native, (int)Type)
        { }
    }
}
