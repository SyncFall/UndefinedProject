using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Runtime.Types
{
    public class ExtendSymbol
    {
        public ObjectSymbol Parent;
        public string TypeName;
        public GenericType GenericType;
        public ObjectSymbol ObjectType;

        public ExtendSymbol(string TypeName, GenericType GenericType)
        {
            this.TypeName = TypeName;
            this.GenericType = GenericType;
            if(this.GenericType != null)
            {
                this.GenericType.CreateSignatur();
            }
        }

        public override string ToString()
        {
            return TypeName;
        }
    }
}
