﻿using Feltic.Language;
using Feltic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feltic.Language
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
        Region,
        Processor,
    }
    
    public static class Tokens
    {
        //public static MapCollection<string, TokenSymbol> KeywordMap = new MapCollection<string, TokenSymbol>();
        public static TokenTrie KeywordMap = new TokenTrie();
        public static TokenTrie StructureMap = new TokenTrie();
        public static TokenSymbol[] StructureArray;
        public static TokenSymbol[] OperationArray;

        static Tokens()
        {
            for (int i=0; i < Keywords.Array.Length; i++)
            {
                KeywordMap.AddSymbol(new TokenSymbol(TokenType.Keyword, Keywords.Array[i].String, Keywords.Array[i]));
            }
            for (int i = 0; i < LiteralKeywords.Array.Length; i++)
            {
                KeywordMap.AddSymbol(new TokenSymbol(TokenType.Literal, LiteralKeywords.Array[i].String, LiteralKeywords.Array[i]));
            }
            for (int i=0; i < Natives.Array.Length; i++)
            {
                KeywordMap.AddSymbol(new TokenSymbol(TokenType.Native, Natives.Array[i].String, Natives.Array[i]));
            }
            for(int i=0; i < Accessors.Array.Length; i++)
            {
                KeywordMap.AddSymbol(new TokenSymbol(TokenType.Accessor, Accessors.Array[i].String, Accessors.Array[i]));
            }
            for(int i=0; i < StatementKeywords.Array.Length; i++)
            {
                KeywordMap.AddSymbol(new TokenSymbol(TokenType.Statement, StatementKeywords.Array[i].String, StatementKeywords.Array[i]));
            }

            StructureArray = new TokenSymbol[Structures.Array.Length];
            for(int i=0; i<Structures.Array.Length; i++)
            {
                StructureArray[i] = new TokenSymbol(TokenType.Structure, Structures.Array[i].String, Structures.Array[i]);
                StructureMap.AddSymbol(StructureArray[i]);
            }

            OperationArray = new TokenSymbol[Operations.Array.Length];
            for(int i=0; i<Operations.Array.Length; i++)
            {
                OperationArray[i] = new TokenSymbol(TokenType.Operation, Operations.Array[i].String, Operations.Array[i]);
            }
        }
    }

    public class TokenList : ListCollection<TokenSymbol>
    { }

    public class TokenSymbol
    {
        public readonly TokenType Type;
        public readonly string String;
        public readonly object Symbol;

        public TokenSymbol(TokenType Type, string String, object Symbol)
        {
            this.Type = Type;
            this.String = String;
            this.Symbol = Symbol;
        }

        public bool IsLineSpace()
        {
            return (Type == TokenType.Structure && (this.Symbol as StructureSymbol).Type == StructureType.LineSpace);
        }

        public bool IsSpace()
        {
            return (Type == TokenType.Structure && (this.Symbol as StructureSymbol).Group == StructureGroup.Space) || Type == TokenType.Comment;
        }

        public bool IsStructure(StructureType StructureType)
        {
            return (Type == TokenType.Structure && (this.Symbol as StructureSymbol).Type == StructureType);
        }

        public bool IsKeyword(KeywordType KeywordType)
        {
            return (Type == TokenType.Keyword && (this.Symbol as KeywordSymbol).Type == KeywordType);
        }

        public bool IsLiteral(LiteralType LiteralType)
        {
            return (Type == TokenType.Literal && (this.Symbol as LiteralSymbol).Type == LiteralType);
        }

        public bool IsNative(NativeType NativeType)
        {
            return (Type == TokenType.Native && (this.Symbol as NativeSymbol).Type == NativeType);
        }

        public bool IsStatement(StatementKeywordType StatementType)
        {
            return (Type == TokenType.Statement && (this.Symbol as StatementKeywordSymbol).Type == StatementType);
        }

        public bool isOperation(OperationType OperationType)
        {
            return (Type == TokenType.Operation && (this.Symbol as OperationSymbol).Type == OperationType);
        }
    }
}
