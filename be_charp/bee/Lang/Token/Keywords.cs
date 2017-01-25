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
        // source-file
        Use,
        Scope,
        // objects
        Extends,
        // implementation
        Abstract,
        Virtual,
        Override,
        // access-method
        Static,
        Readonly,
        // object
        This,
        Base,
        // variable
        New,
    }


    public static class Keywords
    {
        public static readonly KeywordSymbol[] Array =
        {
            new KeywordSymbol(KeywordType.Use, "use"),
            new KeywordSymbol(KeywordType.Scope, "scope"),
            new KeywordSymbol(KeywordType.Extends, "extends"),
            new KeywordSymbol(KeywordType.Abstract, "abstract"),
            new KeywordSymbol(KeywordType.Virtual, "virtual"),
            new KeywordSymbol(KeywordType.Override, "override"),
            new KeywordSymbol(KeywordType.Static, "static"),
            new KeywordSymbol(KeywordType.Readonly, "readonly"),
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
