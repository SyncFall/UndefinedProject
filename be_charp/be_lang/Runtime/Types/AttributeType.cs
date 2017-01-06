using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Runtime.Types
{
    public class AttributeConst
    {
        public static readonly string Target = "target";
    }

    public class AttributeType
    {
        public string TargetName;
        public AttributeItemCollection ElementCollection = new AttributeItemCollection();
    }

    public class AttributeItemCollection : ListCollection<AttributeItem>
    { }

    public class AttributeItem
    {
        public string TypeName;
        public ObjectSymbol ObjectType;
        public AttributeItemDeclaratioType DeclarationType = new AttributeItemDeclaratioType();
    }

    public class AttributeItemDeclaratioType : NativeDeclarationType
    { }
}
