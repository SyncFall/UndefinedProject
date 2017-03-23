using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public enum LiteralType
    {
        Null,
        True,
        False,
        This,
        Char,
        String,
        Number,
    }

    public static class LiteralKeywords
    {
        public static LiteralSymbol[] Array = 
        {
            new LiteralSymbol(LiteralType.Null, "null"),
            new LiteralSymbol(LiteralType.True, "true"),
            new LiteralSymbol(LiteralType.False, "false"),
            new LiteralSymbol(LiteralType.This, "this"),
        };
    }

    public class LiteralSymbol
    {
        public readonly LiteralType Type;
        public readonly string String;

        public LiteralSymbol(LiteralType type, string String)
        {
            this.Type = type;
            this.String = String;
        }
    }
}
