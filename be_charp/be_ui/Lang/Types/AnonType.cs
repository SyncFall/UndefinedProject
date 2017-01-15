using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Runtime.Types
{
    public class AnonObjectType : ObjectSymbol
    {
        public ObjectSymbol ParentObjectType;
        public CodeType ParentCodeType;

        public AnonObjectType() : base(null, null, null, null, null, null, false)
        {
            IsAnonymous = true;
        }
    }
}
