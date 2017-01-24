using Bee.Language;
using Bee.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Language
{
    public enum TokenType
    {
        Keyword,
        Structure,
        Literal,
        Identifier,
        Operation,
        Accessor,
        Native,
        Statement,
        Comment,
        Unknown,
    }

    /*
        // regions
        RegionBegin,
        RegionEnd,
        // pre-processor
        ProcessorIf,
        ProcessorElse,
        ProcessorElseIf,
        ProcessorEndIf,
    */
    
    public static class Tokens
    {
        public static readonly StructureToken[] StructureTokenArray = new StructureToken[Structures.Array.Length];
        public static readonly OperationToken[] OperationTokenArray = new OperationToken[Operations.Array.Length];
        public static readonly MapCollection<string, KeywordToken> KeywordTokenStringMap = new MapCollection<string, KeywordToken>();
        public static readonly MapCollection<string, NativeToken> NativeTokenStringMap = new MapCollection<string, NativeToken>();
        public static readonly MapCollection<string, AccessorToken> AccessorTokenStringMap = new MapCollection<string, AccessorToken>();
        public static readonly MapCollection<string, StatementToken> StatementTokenStringMap = new MapCollection<string, StatementToken>();

        static Tokens()
        {
            for(int i=0; i < Structures.Array.Length; i++)
            {
                StructureToken structureToken = new StructureToken(Structures.Array[i]);
                StructureTokenArray[i] = structureToken;
            };
            for(int i=0; i < Operations.Array.Length; i++)
            {
                OperationToken operationToken = new OperationToken(Operations.Array[i]);
                OperationTokenArray[i] = operationToken;
            }
            for(int i=0; i < Keywords.Array.Length; i++)
            {
                KeywordToken keywordToken = new KeywordToken(Keywords.Array[i]);
                KeywordTokenStringMap.Put(keywordToken.String, keywordToken);
            }
            for(int i=0; i < Natives.Array.Length; i++)
            {
                NativeToken nativeToken = new NativeToken(Natives.Array[i]);
                NativeTokenStringMap.Put(nativeToken.String, nativeToken);
            }
            for(int i=0; i < Accessors.Array.Length; i++)
            {
                AccessorToken accessorToken = new AccessorToken(Accessors.Array[i]);
                AccessorTokenStringMap.Put(accessorToken.String, accessorToken);
            }
            for(int i=0; i < StatementKeywords.Array.Length; i++)
            {
                StatementToken statementToken = new StatementToken(StatementKeywords.Array[i]);
                StatementTokenStringMap.Put(statementToken.String, statementToken);
            }
        }
    }

    public class TokenList : ListCollection<TokenSymbol>
    { }

    public abstract class TokenSymbol
    {
        public readonly TokenType Type;
        public readonly string String;

        public TokenSymbol(TokenType Type, string String)
        {
            this.Type = Type;
            this.String = String;
        }

        public bool IsEqual(TokenSymbol compare)
        {
            return (String == compare.String);
        }

        public bool IsLineSpace()
        {
            return (Type == TokenType.Structure && (this as StructureToken).Symbol.Type == StructureType.LineSpace);
        }

        public bool IsSpace()
        {
            return (Type == TokenType.Structure && (this as StructureToken).Symbol.Group == StructureGroup.Space) || Type == TokenType.Comment;
        }

        public bool IsStructure(StructureType StructureType)
        {
            return (Type == TokenType.Structure && (this as StructureToken).Symbol.Type == StructureType);
        }

        public bool IsKeyword(KeywordType KeywordType)
        {
            return (Type == TokenType.Keyword && (this as KeywordToken).Symbol.Type == KeywordType);
        }

        public bool IsNative(NativeType NativeType)
        {
            return (Type == TokenType.Native && (this as NativeToken).Symbol.Type == NativeType);
        }
    }

    public class StructureToken : TokenSymbol
    {
        public StructureSymbol Symbol;

        public StructureToken(StructureSymbol StructureSymbol) : base(TokenType.Structure, StructureSymbol.String)
        {
            this.Symbol = StructureSymbol;
        }
    }

    public class KeywordToken : TokenSymbol
    {
        public KeywordSymbol Symbol;

        public KeywordToken(KeywordSymbol KeywordSymbol) : base(TokenType.Keyword, KeywordSymbol.String)
        {
            this.Symbol = KeywordSymbol;
        }
    }

    public class NativeToken : TokenSymbol
    {
        public NativeSymbol Symbol;

        public NativeToken(NativeSymbol NativeSymbol) : base(TokenType.Native, NativeSymbol.String)
        {
            this.Symbol = NativeSymbol;
        }
    }

    public class LiteralToken : TokenSymbol
    {
        public LiteralSymbol LiteralSymbol;

        public LiteralToken(LiteralSymbol LiteralSymbol, string TokenString) : base(TokenType.Literal, TokenString)
        {
            this.LiteralSymbol = LiteralSymbol;
        }
    }

    public class IdentifierToken : TokenSymbol
    {
        public IdentifierToken(string IdentifierString) : base(TokenType.Identifier, IdentifierString)
        { }
    }

    public class CommentToken : TokenSymbol
    {
        public string CommentData;

        public CommentToken(string TokenString, string CommentData) : base(TokenType.Comment, TokenString)
        {
            this.CommentData = CommentData;
        }
    }

    public class AccessorToken : TokenSymbol
    {
        public AccessorSymbol AccessorSymbol;

        public AccessorToken(AccessorSymbol AccessorSymbol) : base(TokenType.Accessor, AccessorSymbol.String)
        {
            this.AccessorSymbol = AccessorSymbol;
        }
    }

    public class OperationToken : TokenSymbol
    {
        public OperationSymbol OperationSymbol;

        public OperationToken(OperationSymbol OperationSymbol) : base(TokenType.Operation, OperationSymbol.String)
        {
            this.OperationSymbol = OperationSymbol;
        }
    }

    public class StatementToken : TokenSymbol
    {
        public StatementKeywordSymbol StatementKeywordSymbol;

        public StatementToken(StatementKeywordSymbol Symbol) : base(TokenType.Statement, Symbol.String)
        { }
    }

    public class UnknownToken : TokenSymbol
    {
        public UnknownToken(string TokenString) : base(TokenType.Unknown, TokenString)
        { }
    }
}
