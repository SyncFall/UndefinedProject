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
        Extends,
        // methods
        Abstract,
        Override,
        // access
        Static,
        Const,
        // other
        This,
        Base,
        New,
    }

    public static class Keywords
    {
        public static readonly KeywordSymbol[] Array =
        {
            new KeywordSymbol(KeywordType.Use, "use"),
            new KeywordSymbol(KeywordType.Scope, "scope"),
            new KeywordSymbol(KeywordType.Extends, "partial"),
            new KeywordSymbol(KeywordType.Extends, "extends"),
            new KeywordSymbol(KeywordType.Abstract, "abstract"),
            new KeywordSymbol(KeywordType.Override, "override"),
            new KeywordSymbol(KeywordType.Static, "static"),
            new KeywordSymbol(KeywordType.Const, "const"),
            new KeywordSymbol(KeywordType.This, "this"),
            new KeywordSymbol(KeywordType.Base, "base"),
            new KeywordSymbol(KeywordType.New, "new"),
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
