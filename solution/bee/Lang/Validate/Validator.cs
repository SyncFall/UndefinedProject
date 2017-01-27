using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Language
{
    public partial class Validator
    {
        public Registry Registry;

        public Validator(Registry Registry)
        {
            this.Registry = Registry;
        }

        public void ValidateSource(SourceSymbol SourceSymbol)
        {
            for (int i = 0; i < SourceSymbol.ScopeList.Size(); i++)
            {
                ScopeSymbol scopeSymbol = SourceSymbol.ScopeList.Get(i);
                for (int j = 0; j < scopeSymbol.ObjectList.Size(); j++)
                {
                    ObjectSymbol objectSymbol = scopeSymbol.ObjectList.Get(j);
                    ValidateObject(objectSymbol);
                }
            }
        }
    }
}
