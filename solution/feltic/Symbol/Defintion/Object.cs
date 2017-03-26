using feltic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public enum ObjectType
    {
        None=0,
        // source-directive
        Use,
        Scope,
        // object-hierachie
        Extend,
        Part,
        // method-polyphormie
        Abstract,
        Implement,
        Override,
        // variable-access/property
        Static,
        Const,
        New,
        // property
        Get,
        Set,
        // block
        End,
    }

    public static class ObjectTypes
    {
        public static readonly ObjectTypeSymbol[] Array =
        {
            new ObjectTypeSymbol(ObjectType.Use, "use"),
            new ObjectTypeSymbol(ObjectType.Scope, "scope"),
            new ObjectTypeSymbol(ObjectType.Extend, "extend"),
            new ObjectTypeSymbol(ObjectType.Part, "part"),
            new ObjectTypeSymbol(ObjectType.Abstract, "abstract"),
            new ObjectTypeSymbol(ObjectType.Implement, "implement"),
            new ObjectTypeSymbol(ObjectType.Override, "override"),
            new ObjectTypeSymbol(ObjectType.Static, "static"),
            new ObjectTypeSymbol(ObjectType.Const, "const"),
            new ObjectTypeSymbol(ObjectType.New, "new"),
            new ObjectTypeSymbol(ObjectType.Get, "get"),
            new ObjectTypeSymbol(ObjectType.Set, "set"),
            new ObjectTypeSymbol(ObjectType.End, "end"),
        };
    }
    
    public class ObjectTypeSymbol : Symbol
    {
        public ObjectTypeSymbol(ObjectType Type, string String) : base(String, (int)TokenType.Object, (int)Type)
        { }
    }
}
