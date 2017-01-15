using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.Runtime.Types
{
    public class PropertySymbol : MethodType
    {
        public CodeType GetCode;
        public CodeType SetCode;

        public PropertySymbol(string Name, ObjectSymbol objectType, AttributeType Attributes, AccessorType Accessor, GenericType Generics, string ReturnType) :
            base(Name, objectType, Attributes, Accessor, Generics, ReturnType, null)
        {
            this.IsProperty = true;
        }
    }
}
