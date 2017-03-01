using Bee.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Language
{
    public enum KeywordType
    {
        // sources
        Use,
        Scope,
        // objects
        Partial,
        Part,
        // methods
        Abstract,
        Implement,
        Override,
        // access
        Static,
        Const,
        // other
        This,
        Base,
        Parent,
        New,
        Get,
        Set,
        End,
    }

    public static class Keywords
    {
        public static readonly KeywordSymbol[] Array =
        {
            new KeywordSymbol(KeywordType.Use, "use"),
            new KeywordSymbol(KeywordType.Scope, "scope"),
            new KeywordSymbol(KeywordType.Part, "part"),
            new KeywordSymbol(KeywordType.Abstract, "abstract"),
            new KeywordSymbol(KeywordType.Implement, "implement"),
            new KeywordSymbol(KeywordType.Override, "override"),
            new KeywordSymbol(KeywordType.Static, "static"),
            new KeywordSymbol(KeywordType.Const, "const"),
            new KeywordSymbol(KeywordType.This, "this"),
            new KeywordSymbol(KeywordType.Base, "base"),
            new KeywordSymbol(KeywordType.Parent, "parent"),
            new KeywordSymbol(KeywordType.New, "new"),
            new KeywordSymbol(KeywordType.Get, "get"),
            new KeywordSymbol(KeywordType.Set, "set"),
            new KeywordSymbol(KeywordType.End, "end"),
        };
    }
    
    public class KeywordSymbol
    {
        public readonly KeywordType Type;
        public readonly string String;

        public KeywordSymbol(KeywordType Keyword, string KeywordString)
        {
            this.Type = Keyword;
            this.String = KeywordString;
        }
    }
}
