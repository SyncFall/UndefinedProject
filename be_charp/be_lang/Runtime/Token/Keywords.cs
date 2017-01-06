using Be.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.Runtime
{
    public enum Keyword
    {
        // source-file
        Using,
        Namespace,
        // objects
        Object,
        Interface,
        Enumeration,
        Attribute,
        Exception,
        // oop-extend
        Extend,
        Implement,
        // accessors
        Public,
        Private,
        Protected,
        Internal,
        // implementation
        Abstract,
        Virtual,
        Override,
        // access
        Static,
        Native,
        Readonly,
        // properties
        Get,
        Set,
    }

    public enum KeywordGroup
    {
        SourceFile,
        ObjectType,
        ObjectExtend,
        Accessor,
        Implement,
        Access,
        PropertyCode,
    }

    public static class Keywords
    {
        public static readonly KeywordSymbol[] Array =
        {
            new KeywordSymbol(Keyword.Using, "using", KeywordGroup.SourceFile),
            new KeywordSymbol(Keyword.Namespace, "namespace", KeywordGroup.SourceFile),
            new KeywordSymbol(Keyword.Object, "object", KeywordGroup.ObjectType),
            new KeywordSymbol(Keyword.Interface, "interface", KeywordGroup.ObjectType),
            new KeywordSymbol(Keyword.Enumeration, "enum", KeywordGroup.ObjectType),
            new KeywordSymbol(Keyword.Attribute, "attribute", KeywordGroup.ObjectType),
            new KeywordSymbol(Keyword.Exception, "exception", KeywordGroup.ObjectType),
            new KeywordSymbol(Keyword.Extend, "extend", KeywordGroup.ObjectExtend),
            new KeywordSymbol(Keyword.Extend, "implement", KeywordGroup.ObjectExtend),
            new KeywordSymbol(Keyword.Public, "public", KeywordGroup.Accessor),
            new KeywordSymbol(Keyword.Private, "private", KeywordGroup.Accessor),
            new KeywordSymbol(Keyword.Protected, "protected", KeywordGroup.Accessor),
            new KeywordSymbol(Keyword.Internal, "internal", KeywordGroup.Accessor),
            new KeywordSymbol(Keyword.Abstract, "abstract", KeywordGroup.Implement),
            new KeywordSymbol(Keyword.Virtual, "virtual", KeywordGroup.Implement),
            new KeywordSymbol(Keyword.Override, "override", KeywordGroup.Implement),
            new KeywordSymbol(Keyword.Static, "static", KeywordGroup.Access),
            new KeywordSymbol(Keyword.Native, "native", KeywordGroup.Access),
            new KeywordSymbol(Keyword.Readonly, "readonly", KeywordGroup.Access),
            new KeywordSymbol(Keyword.Get, "get", KeywordGroup.PropertyCode),
            new KeywordSymbol(Keyword.Set, "set", KeywordGroup.PropertyCode),
        };

        public static readonly MapCollection<Keyword, KeywordSymbol> EnumMap = new MapCollection<Keyword, KeywordSymbol>();
        public static readonly MapCollection<string, KeywordSymbol> StringMap = new MapCollection<string, KeywordSymbol>();

        static Keywords()
        {
            for(int i=0; i<Array.Length; i++)
            {
                KeywordSymbol keyword = Array[i];
                EnumMap.Add(keyword.Keyword, keyword);
                StringMap.Add(keyword.KeywordString, keyword);
            }
        }
    }

    public class KeywordSymbol
    {
        public readonly Keyword Keyword;
        public readonly KeywordGroup KeywordGroup;
        public readonly string KeywordString;

        public KeywordSymbol(Keyword Keyword, string KeywordString, KeywordGroup KeywordGroup)
        {
            this.Keyword = Keyword;
            this.KeywordGroup = KeywordGroup;
            this.KeywordString = KeywordString;
        }
    }
}
