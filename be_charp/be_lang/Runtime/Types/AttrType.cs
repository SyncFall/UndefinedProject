using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Runtime.Types
{
    public class AttrType : ObjectSymbol
    {
        public AttrType(string Name, AttributeType Attribute, AccessorType Accessor, GenericType Generics, ExtendSymbol Extend, ImplementCollection Implement, bool IsVirtual) : base(Name, Attribute, Accessor, Generics, Extend, Implement, IsVirtual)
        {
            this.IsAttribute = true;
        }
    }
}
