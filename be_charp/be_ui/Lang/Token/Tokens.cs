using Be.Runtime.Types;
using Bee.Language;
using Bee.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.Runtime
{
    public enum TokenType
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
        // identfier
        Identifier,
        // accessor
        Accessor,
        // object
        Object,
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
        Accessor,
        Object,
        Unknown,
    }

    public static class Tokens
    {
        public static readonly TokenSymbol[] AllTokensArray = null;

        public static readonly TokenSymbol[] StructureTokensArray = new TokenSymbol[]
        {
            new TokenSymbol(TokenType.WhiteSpace, TokenGroup.Space, " "),
            new TokenSymbol(TokenType.TabSpace, TokenGroup.Space, "\t"),
            new TokenSymbol(TokenType.LineSpace, TokenGroup.Space, "\n"),
            new TokenSymbol(TokenType.Complete, TokenGroup.Statement, ";"),
            new TokenSymbol(TokenType.Assigment, TokenGroup.Statement, "="),
            new TokenSymbol(TokenType.Comma, TokenGroup.Statement, ","),
            new TokenSymbol(TokenType.Point, TokenGroup.Statement, "."),
            new TokenSymbol(TokenType.Seperator, TokenGroup.Statement, ";"),
            new TokenSymbol(TokenType.BlockBegin, TokenGroup.Block, "{"),
            new TokenSymbol(TokenType.BlockEnd, TokenGroup.Block, "}"),
            new TokenSymbol(TokenType.ClosingBegin, TokenGroup.Block, "("),
            new TokenSymbol(TokenType.ClosingEnd, TokenGroup.Block, ")"),
            new TokenSymbol(TokenType.BracketBegin, TokenGroup.Block, "["),
            new TokenSymbol(TokenType.BracketEnd, TokenGroup.Block, "]"),
            new TokenSymbol(TokenType.ShiftBegin, TokenGroup.Block, "<"),
            new TokenSymbol(TokenType.ShiftEnd, TokenGroup.Block, ">"),
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

        public static readonly AccessorToken[] AccessorTokenArray = null;
        public static readonly MapCollection<string, AccessorToken> AccessorTokenStringMap = new MapCollection<string, AccessorToken>();

        static Tokens()
        {
            int idx = 0;
            int totalLen = (StructureTokensArray.Length + Keywords.Array.Length + Natives.Array.Length + Accessors.Array.Length);
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
            AccessorTokenArray = new AccessorToken[Accessors.Array.Length];
            for(int i=0; i < Accessors.Array.Length; i++)
            {
                AccessorToken accessorToken = new AccessorToken(Accessors.Array[i]);
                AccessorTokenArray[i] = accessorToken;
                AllTokensArray[idx++] = accessorToken;
                AccessorTokenStringMap.Add(accessorToken.String, accessorToken);
            }
        }
    }

    public class TokenList : ListCollection<TokenSymbol>
    { }

    public class TokenSymbol
    {
        public readonly TokenType Type;
        public readonly TokenGroup Group;
        public readonly string String;

        public TokenSymbol(TokenType Type, TokenGroup Group, string String)
        {
            this.Type = Type;
            this.Group = Group;
            this.String = String;
        }

        public bool IsEqual(TokenSymbol compare)
        {
            return (String == compare.String);
        }
    }

    public class KeywordToken : TokenSymbol
    {
        public KeywordSymbol KeywordSymbol;

        public KeywordToken(KeywordSymbol KeywordSymbol) : base(TokenType.Keyword, TokenGroup.Keyword, KeywordSymbol.String)
        {
            this.KeywordSymbol = KeywordSymbol;
        }
    }

    public class NativeToken : TokenSymbol
    {
        public NativeSymbol NativeSymbol;

        public NativeToken(NativeSymbol NativeSymbol) : base(TokenType.Keyword, TokenGroup.Native, NativeSymbol.String)
        {
            this.NativeSymbol = NativeSymbol;
        }
    }

    public class LiteralToken : TokenSymbol
    {
        public LiteralSymbol LiteralSymbol;

        public LiteralToken(LiteralSymbol LiteralSymbol, string TokenString) : base(TokenType.Literal, TokenGroup.Literal, TokenString)
        {
            this.LiteralSymbol = LiteralSymbol;
        }
    }

    public class IdentifierToken : TokenSymbol
    {
        public IdentifierToken(string IdentifierString) : base(TokenType.Identifier, TokenGroup.Identifier, IdentifierString)
        { }
    }

    public class CommentToken : TokenSymbol
    {
        public string CommentData;

        public CommentToken(string TokenString, string CommentData) : base(TokenType.Comment, TokenGroup.Comment, TokenString)
        {
            this.CommentData = CommentData;
        }
    }

    public class AccessorToken : TokenSymbol
    {
        public AccessorSymbol AccessorSymbol;

        public AccessorToken(AccessorSymbol AccessorSymbol) : base(TokenType.Accessor, TokenGroup.Accessor, AccessorSymbol.String)
        {
            this.AccessorSymbol = AccessorSymbol;
        }
    }
    
    public class UnknownToken : TokenSymbol
    {
        public UnknownToken(string TokenString) : base(TokenType.Unknown, TokenGroup.Unknown, TokenString)
        { }
    }
}
