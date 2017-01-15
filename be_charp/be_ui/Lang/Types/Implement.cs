using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Be.Runtime.Types
{
    public class ImplementCollection : ListCollection<ImplementSymbol>
    { }

    public class ImplementSymbol
    {
        public ObjectSymbol Parent;
        public string Path;
        public GenericType GenericType;
        public ObjectSymbol ObjectType;

        public ImplementSymbol(string Path, GenericType GenericType)
        {
            this.Path = Path;
            this.GenericType = GenericType;
            if(this.GenericType != null)
            {
                this.GenericType.CreateSignatur();
            }
        }
    }
}
