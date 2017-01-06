using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.Runtime
{
    public enum LiteralType
    {
        // object
        This,
        Base,
        Null,
        Value,
        New,
        // natives
        Bool,
        Char,
        Number,
        String,
    }

    public static class Literals
    {
        public static readonly string This = "this";
        public static readonly string Base = "base";
        public static readonly string Null = "null";
        public static readonly string Value = "value";
        public static readonly string New = "new";
        public static readonly string True = "true";
        public static readonly string False = "false";
        public static readonly char Char = '\'';
        public static readonly char String = '"';
        public static readonly char Point = '.';
    }

    public class ObjectConstantLiteral : LiteralToken
    {
        public ObjectConstantLiteral(LiteralType LiteralType, string DataValue) : base(LiteralType, DataValue)
        { }
    }

    public class BoolLiteral : LiteralToken
    {
        public BoolLiteral(string DataValue) : base(LiteralType.Bool, DataValue)
        { }
    }

    public class StringLiteral : LiteralToken
    {
        public StringLiteral(string DataValue) : base(LiteralType.String, Literals.String + DataValue + Literals.String)
        { }
    }

    public class CharLiteral : LiteralToken
    {
        public CharLiteral(string DataValue) : base(LiteralType.Char, Literals.Char + DataValue + Literals.Char)
        { }
    }

    public class NumberLiteral : LiteralToken
    {
        public NumberLiteral(string DataValue) : base(LiteralType.Number, DataValue)
        { }
    }
}
