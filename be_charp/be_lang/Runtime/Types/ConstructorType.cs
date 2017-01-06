using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Runtime.Types
{
    public class ConstructorType : MethodType
    {
        public ConstructorType(string Name, ObjectSymbol objectType, AttributeType Attributes, AccessorType Accessor, GenericType Generics, string ReturnType, ParameterCollection ParameterCollection) 
            : base(Name, objectType, Attributes, Accessor, Generics, ReturnType, ParameterCollection)
        {
            this.IsConstructor = true;
        }
    }
}
