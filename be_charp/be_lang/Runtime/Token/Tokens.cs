using Be.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.Runtime
{
    public enum Token
    {
        // spaces
        WhiteSpace,
        TabSpace,
        LineSpace,
        // comments
        CommentLine,
        CommentBlock,
        // regions
        RegionStart,
        RegionEnd,
        // pre-processor
        ProcessorIf,
        ProcessorElse,
        ProcessorElseIf,
        ProcessorEndIf,
        // statements
        Complete,
        Assigment,
        Comma,
        Point,
        Seperator,
        // blocks
        BlockStart,
        BlockEnd,
        BracketStart,
        BracketEnd,
        ClosingStart,
        ClosingEnd,
        Backward,
        Forward,
        // keyword
        Keyword,
        // natives
        Native,
        // literal
        Literal,
        // namepath
        NamePath,
        // unknowns
        Unknown,
    }

    public enum TokenGroup
    {
        Space,
        Comment,
        Region,
        Processor,
        Statement,
        Block,
        Keyword,
        Native,
        Literal,
        NamePath,
        Unknown,
    }

    public static class Tokens
    {
        public static readonly char WhiteSpace = ' ';
        public static readonly char TabSpace = '\t';
        public static readonly char LineSpace = '\n';
        public static readonly string CommentLine = "//";
        public static readonly string CommentBlockStart = "/*";
        public static readonly string CommentBlockEnd = "*/";
        public static readonly string RegionStart = "#region";
        public static readonly string RegionEnd = "#endregion";
        public static readonly string ProcessorIf = "#if";
        public static readonly string ProcessorElse = "#else";
        public static readonly string ProcessorElseIf = "#elseif";
        public static readonly string ProcessorEndIf = "#endif";
        public static readonly char Complete = ';';
        public static readonly char Assigment = '=';
        public static readonly char Comma = ',';
        public static readonly char Point = '.';
        public static readonly char Seperator = ';';
        public static readonly char BlockStart = '{';
        public static readonly char BlockEnd = '}';
        public static readonly char BracketStart = '[';
        public static readonly char BracketEnd = ']';
        public static readonly char Backward = '<';
        public static readonly char Forward = '>';
        public static readonly char ClosingStart = '(';
        public static readonly char ClosingEnd = ')';
        public static readonly char String = '"';
        public static readonly char Char = '\'';
    }


    public enum TokenStatus
    {
        Information,
        Warning,
        Error,
    }

    public class TokenStatusSymbol
    {
        public readonly TokenStatus Type;
        public readonly string Message;

        public TokenStatusSymbol(TokenStatus Status, string Message)
        {
            this.Type = Status;
            this.Message = Message;
        }
    }

    public abstract class TokenSymbol
    {
        public readonly Token Type;
        public readonly TokenGroup Group;
        public string TextString;
        public int TextCount;
        public TokenSymbol PrevToken;
        public TokenSymbol NextToken;
        public TokenStatusSymbol Status;

        public TokenSymbol(Token Type, TokenGroup Group, string TextString)
        {
            this.Type = Type;
            this.Group = Group;
            this.TextString = TextString;
            this.TextCount = TextString.Length;
        }
    }

    public class UnknownToken : TokenSymbol
    {
        public UnknownToken(string TextString) : base(Token.Unknown, TokenGroup.Unknown, TextString)
        { }
    }

    public class SpaceToken : TokenSymbol
    {
        public SpaceToken(Token Type, char TextChar) : base(Type, TokenGroup.Space, TextChar+"")
        { }
    }

    public class CommentToken : TokenSymbol
    {
        public CommentToken(Token Type, string TextString) : base(Type, TokenGroup.Comment, TextString)
        { }
    }

    public class RegionToken : TokenSymbol
    {
        public RegionToken(Token Type, string TextString) : base(Type, TokenGroup.Region, TextString)
        { }
    }

    public class ProcessorToken : TokenSymbol
    {
        public ProcessorToken(Token Type, string TextString) : base(Type, TokenGroup.Processor, TextString)
        { }
    }

    public class StatementToken : TokenSymbol
    {
        public StatementToken(Token Type, char TextChar) : base(Type, TokenGroup.Statement, TextChar+"")
        { }
    }

    public class BlockToken : TokenSymbol
    {
        public BlockToken(Token TokenType, char TextChar) : base(TokenType, TokenGroup.Block, TextChar+"")
        { }
    }

    public class KeywordToken : TokenSymbol
    {
        public KeywordSymbol KeywordSymbol;

        public KeywordToken(KeywordSymbol KeywordType) : base(Token.Keyword, TokenGroup.Keyword, KeywordType.KeywordString)
        {
            this.KeywordSymbol = KeywordType;
        }
    }

    public class NativeToken : TokenSymbol
    {
        public NativeSymbol NativeSymbol;

        public NativeToken(NativeSymbol NativeType) : base(Token.Native, TokenGroup.Native, NativeType.Name)
        {
            this.NativeSymbol = NativeType;
        }
    }

    public class LiteralToken : TokenSymbol
    {
        public LiteralType LiteralType;

        public LiteralToken(LiteralType LiteralType, string TextString) : base(Token.Literal, TokenGroup.Literal, TextString)
        {
            this.LiteralType = LiteralType;
        }
    }

    public class NamePathToken : TokenSymbol
    {
        public NamePathToken(string TextString) : base(Token.NamePath, TokenGroup.NamePath, TextString)
        { }
    }
}
