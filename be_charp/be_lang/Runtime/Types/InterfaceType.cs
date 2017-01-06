using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Runtime.Types
{
    public class InterfaceType : ObjectSymbol
    {
        public InterfaceType(string Name, AttributeType Attribute, GenericType Generics, ExtendSymbol Extend, bool IsVirtual) : base(Name, Attribute, null, Generics, Extend, null, IsVirtual)
        {
            this.IsInterface = true;
        }
    }
}
