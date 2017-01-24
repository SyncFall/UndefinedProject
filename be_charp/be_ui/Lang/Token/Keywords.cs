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
        Object,
        // oop-extend
        Extends,
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
            new KeywordSymbol(KeywordType.Use, "use", KeywordGroup.SourceFile),
            new KeywordSymbol(KeywordType.Scope, "scope", KeywordGroup.SourceFile),
            new KeywordSymbol(KeywordType.Object, "object", KeywordGroup.ObjectType),
            new KeywordSymbol(KeywordType.Extends, "extends", KeywordGroup.ObjectExtend),
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
        };
        public static readonly MapCollection<KeywordType, KeywordSymbol> EnumMap = new MapCollection<KeywordType, KeywordSymbol>();
        public static readonly MapCollection<string, KeywordSymbol> StringMap = new MapCollection<string, KeywordSymbol>();

        static Keywords()
        {
            for(int i=0; i<Array.Length; i++)
            {
                KeywordSymbol keyword = Array[i];
                EnumMap.Put(keyword.Type, keyword);
                StringMap.Put(keyword.String, keyword);
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
