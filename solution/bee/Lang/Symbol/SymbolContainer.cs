using Feltic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feltic.Language
{
    public class SymbolContainer
    {
        public SourceSymbol SourceSymbol; 

        public SymbolContainer()
        { }

        public void SetContainer(SignatureContainer SignatureContainer)
        { 
            SymbolParser symbolParser = new SymbolParser();
            SourceSymbol = symbolParser.TrySymbol(SignatureContainer);
        }
    }
}