using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Runtime
{
    public enum LiteralType
    {
        Null,
        Bool,
        Char,
        Number,
        String,
    }

    public static class LiteralConst
    {
        public static readonly string Null = "null";
        public static readonly string True = "true";
        public static readonly string False = "false";
        public static readonly string CharEscape = "'";
        public static readonly string StringEscape = "\"";
    }

    public abstract class LiteralSymbol
    {
        public LiteralType Type;
        public string String;

        public LiteralSymbol(LiteralType type, string LiteralString)
        {
            this.Type = type;
            this.String = LiteralString;
        }

        public bool IsEqual(LiteralSymbol compare)
        {
            return (String == compare.String);
        }
    }

    public class NullLiteral : LiteralSymbol
    {
        public NullLiteral() : base(LiteralType.Null, LiteralConst.Null)
        { } 
    }

    public class BoolLiteral : LiteralSymbol
    {
        public bool BoolData;

        public BoolLiteral(bool BoolData) : base(LiteralType.Bool, (BoolData ? LiteralConst.True : LiteralConst.False))
        {
            this.BoolData = BoolData;
        }
    }

    public class CharLiteral : LiteralSymbol
    {
        public string CharData;

        public CharLiteral(string CharData) : base(LiteralType.Char, (LiteralConst.CharEscape + CharData + LiteralConst.CharEscape))
        {
            this.CharData = CharData;
        }
    }

    public class NumberLiteral : LiteralSymbol
    {
        public string NumberData;

        public NumberLiteral(string NumberData) : base(LiteralType.Number, NumberData)
        {
            this.NumberData = NumberData;
        }
    }

    public class StringLiteral : LiteralSymbol
    {
        public string StringData;

        public StringLiteral(string StringData) : base(LiteralType.String, (LiteralConst.StringEscape + StringData + LiteralConst.StringEscape))
        {
            this.StringData = StringData;
        }
    }
}
