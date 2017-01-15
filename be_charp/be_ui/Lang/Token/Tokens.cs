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
        Comment,
        // regions
        RegionBegin,
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
        BlockBegin,
        BlockEnd,
        ClosingBegin,
        ClosingEnd,
        BracketBegin,
        BracketEnd,
        ShiftBegin,
        ShiftEnd,
        // keyword
        Keyword,
        // literal
        Literal,
        // namepath
        Identifier,
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
        Identifier,
        Unknown,
    }

    public static class Tokens
    {
        public static readonly TokenSymbol[] AllTokensArray = null;

        public static readonly TokenSymbol[] StructureTokensArray = new TokenSymbol[]
        {
            new TokenSymbol(Token.WhiteSpace, TokenGroup.Space, " "),
            new TokenSymbol(Token.TabSpace, TokenGroup.Space, "\t"),
            new TokenSymbol(Token.LineSpace, TokenGroup.Space, "\n"),
            new TokenSymbol(Token.Complete, TokenGroup.Statement, ";"),
            new TokenSymbol(Token.Assigment, TokenGroup.Statement, "="),
            new TokenSymbol(Token.Comma, TokenGroup.Statement, ","),
            new TokenSymbol(Token.Point, TokenGroup.Statement, "."),
            new TokenSymbol(Token.Seperator, TokenGroup.Statement, ";"),
            new TokenSymbol(Token.BlockBegin, TokenGroup.Block, "{"),
            new TokenSymbol(Token.BlockEnd, TokenGroup.Block, "}"),
            new TokenSymbol(Token.ClosingBegin, TokenGroup.Block, "("),
            new TokenSymbol(Token.ClosingEnd, TokenGroup.Block, ")"),
            new TokenSymbol(Token.BracketBegin, TokenGroup.Block, "["),
            new TokenSymbol(Token.BracketEnd, TokenGroup.Block, "]"),
            new TokenSymbol(Token.ShiftBegin, TokenGroup.Block, "<"),
            new TokenSymbol(Token.ShiftEnd, TokenGroup.Block, ">"),
            /*
            new TokenSymbol(Token.RegionBegin, TokenGroup.Region, "#region"),
            new TokenSymbol(Token.RegionEnd, TokenGroup.Region, "#regionend"),
            new TokenSymbol(Token.ProcessorIf, TokenGroup.Processor, "#if"),
            new TokenSymbol(Token.ProcessorElseIf, TokenGroup.Processor, "#elseif"),
            new TokenSymbol(Token.ProcessorElse, TokenGroup.Processor, "#else"),
            new TokenSymbol(Token.ProcessorEndIf, TokenGroup.Processor, "#endif"),
            */
        };
        public static readonly MapCollection<string, TokenSymbol> StructureTokenStringMap = new MapCollection<string, TokenSymbol>();
        
        public static readonly KeywordToken[] KeywordTokenArray = null;
        public static readonly MapCollection<string, KeywordToken> KeywordTokenStringMap = new MapCollection<string, KeywordToken>();

        public static readonly NativeToken[] NativeTokenArray = null;
        public static readonly MapCollection<string, NativeToken> NativeTokenStringMap = new MapCollection<string, NativeToken>();

        static Tokens()
        {
            int idx = 0;
            int totalLen = (StructureTokensArray.Length + Keywords.Array.Length + Natives.Array.Length);
            AllTokensArray = new TokenSymbol[totalLen];
            for(int i=0; i < StructureTokensArray.Length; i++)
            {
                TokenSymbol structureToken = StructureTokensArray[i];
                AllTokensArray[idx++] = structureToken;
                StructureTokenStringMap.Add(structureToken.String, structureToken);
            }
            KeywordTokenArray = new KeywordToken[Keywords.Array.Length];
            for(int i=0;  i < Keywords.Array.Length; i++)
            {
                KeywordToken keywordToken = new KeywordToken(Keywords.Array[i]);
                KeywordTokenArray[i] = keywordToken;
                AllTokensArray[idx++] = keywordToken;
                KeywordTokenStringMap.Add(keywordToken.String, keywordToken);
            }
            NativeTokenArray = new NativeToken[Natives.Array.Length];
            for(int i=0; i < Natives.Array.Length; i++)
            {
                NativeToken nativeToken = new NativeToken(Natives.Array[i]);
                NativeTokenArray[i] = nativeToken;
                AllTokensArray[idx++] = nativeToken;
                NativeTokenStringMap.Add(nativeToken.String, nativeToken);
            }
        }
    }

    public class TokenSymbol
    {
        public readonly Token Type;
        public readonly TokenGroup Group;
        public readonly string String;

        public TokenSymbol(Token Type, TokenGroup Group, string String)
        {
            this.Type = Type;
            this.Group = Group;
            this.String = String;
        }

        public virtual bool IsEqual(TokenSymbol compare)
        {
            return (String == compare.String);
        }
    }

    public class KeywordToken : TokenSymbol
    {
        public KeywordSymbol KeywordSymbol;

        public KeywordToken(KeywordSymbol KeywordType) : base(Token.Keyword, TokenGroup.Keyword, KeywordType.String)
        {
            this.KeywordSymbol = KeywordType;
        }

        public override bool IsEqual(TokenSymbol compare)
        {
            return (base.IsEqual(compare) && KeywordSymbol.IsEqual((compare as KeywordToken).KeywordSymbol));
        }
    }

    public class NativeToken : TokenSymbol
    {
        public NativeSymbol NativeSymbol;

        public NativeToken(NativeSymbol NativeType) : base(Token.Keyword, TokenGroup.Native, NativeType.String)
        {
            this.NativeSymbol = NativeType;
        }

        public override bool IsEqual(TokenSymbol compare)
        {
            return (base.IsEqual(compare) && NativeSymbol.IsEqual((compare as NativeToken).NativeSymbol));
        }
    }

    public class LiteralToken : TokenSymbol
    {
        public LiteralSymbol LiteralSymbol;

        public LiteralToken(LiteralSymbol LiteralSymbol, string TokenString) : base(Token.Literal, TokenGroup.Literal, TokenString)
        {
            this.LiteralSymbol = LiteralSymbol;
        }

        public override bool IsEqual(TokenSymbol compare)
        {
            return (base.IsEqual(compare) && LiteralSymbol.IsEqual((compare as LiteralToken).LiteralSymbol));
        }
    }

    public class IdentifierToken : TokenSymbol
    {
        public IdentifierToken(string TokenString) : base(Token.Identifier, TokenGroup.Identifier, TokenString)
        { }
    }

    public class CommentToken : TokenSymbol
    {
        public string CommentData;

        public CommentToken(string TokenString, string CommentData) : base(Token.Comment, TokenGroup.Comment, TokenString)
        {
            this.CommentData = CommentData;
        }
    }

    public class UnknownToken : TokenSymbol
    {
        public UnknownToken(string TokenString) : base(Token.Unknown, TokenGroup.Unknown, TokenString)
        { }
    }
}
