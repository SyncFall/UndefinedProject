using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public enum LiteralType
    {
        None=0,
        // constants
        Null,
        True,
        False,
        This,
        // variables
        Char,
        String,
        Number,
    }

    public static class LiteralConstants
    {
        public static LiteralSymbol[] Array = 
        {
            new LiteralSymbol(LiteralType.Null, "null"),
            new LiteralSymbol(LiteralType.True, "true"),
            new LiteralSymbol(LiteralType.False, "false"),
            new LiteralSymbol(LiteralType.This, "this"),
        };
    }

    public class LiteralSymbol : Symbol
    {
        public LiteralSymbol(LiteralType Type, string String) : base(String, (int)TokenType.Literal, (int)Type)
        { }

        public bool IsType(LiteralType Type)
        {
            return (this.Type == (int)Type);
        }
    }
}
