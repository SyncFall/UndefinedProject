using Be.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.Runtime
{
    public enum KeywordType
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
        // object
        This,
        Base,
        // variable
        New,
        // properties
        Get,
        Set,
        Value,
    }

    public enum KeywordGroup
    {
        SourceFile,
        ObjectType,
        ObjectExtend,
        Accessor,
        Implement,
        Access,
        ObjectCode,
        VariableCode,
        PropertyCode,
    }

    public static class Keywords
    {
        public static readonly KeywordSymbol[] Array =
        {
            new KeywordSymbol(KeywordType.Using, "using", KeywordGroup.SourceFile),
            new KeywordSymbol(KeywordType.Namespace, "namespace", KeywordGroup.SourceFile),
            new KeywordSymbol(KeywordType.Object, "object", KeywordGroup.ObjectType),
            new KeywordSymbol(KeywordType.Interface, "interface", KeywordGroup.ObjectType),
            new KeywordSymbol(KeywordType.Enumeration, "enum", KeywordGroup.ObjectType),
            new KeywordSymbol(KeywordType.Attribute, "attribute", KeywordGroup.ObjectType),
            new KeywordSymbol(KeywordType.Exception, "exception", KeywordGroup.ObjectType),
            new KeywordSymbol(KeywordType.Extend, "extend", KeywordGroup.ObjectExtend),
            new KeywordSymbol(KeywordType.Extend, "implement", KeywordGroup.ObjectExtend),
            new KeywordSymbol(KeywordType.Public, "public", KeywordGroup.Accessor),
            new KeywordSymbol(KeywordType.Private, "private", KeywordGroup.Accessor),
            new KeywordSymbol(KeywordType.Protected, "protected", KeywordGroup.Accessor),
            new KeywordSymbol(KeywordType.Internal, "internal", KeywordGroup.Accessor),
            new KeywordSymbol(KeywordType.Abstract, "abstract", KeywordGroup.Implement),
            new KeywordSymbol(KeywordType.Virtual, "virtual", KeywordGroup.Implement),
            new KeywordSymbol(KeywordType.Override, "override", KeywordGroup.Implement),
            new KeywordSymbol(KeywordType.Static, "static", KeywordGroup.Access),
            new KeywordSymbol(KeywordType.Native, "native", KeywordGroup.Access),
            new KeywordSymbol(KeywordType.Readonly, "readonly", KeywordGroup.Access),
            new KeywordSymbol(KeywordType.This, "this", KeywordGroup.ObjectCode),
            new KeywordSymbol(KeywordType.Base, "base", KeywordGroup.ObjectCode),
            new KeywordSymbol(KeywordType.New, "new", KeywordGroup.VariableCode),
            new KeywordSymbol(KeywordType.Get, "get", KeywordGroup.PropertyCode),
            new KeywordSymbol(KeywordType.Set, "set", KeywordGroup.PropertyCode),
        };

        public static readonly MapCollection<KeywordType, KeywordSymbol> EnumMap = new MapCollection<KeywordType, KeywordSymbol>();
        public static readonly MapCollection<string, KeywordSymbol> StringMap = new MapCollection<string, KeywordSymbol>();

        static Keywords()
        {
            for(int i=0; i<Array.Length; i++)
            {
                KeywordSymbol keyword = Array[i];
                EnumMap.Add(keyword.Type, keyword);
                StringMap.Add(keyword.String, keyword);
            }
        }
    }
    
    public class KeywordSymbol
    {
        public readonly KeywordType Type;
        public readonly KeywordGroup Group;
        public readonly string String;

        public KeywordSymbol(KeywordType Keyword, string KeywordString, KeywordGroup KeywordGroup)
        {
            this.Type = Keyword;
            this.Group = KeywordGroup;
            this.String = KeywordString;
        }

        public bool IsEqual(KeywordSymbol compare)
        {
            return (String == compare.String);
        }
    }
}
