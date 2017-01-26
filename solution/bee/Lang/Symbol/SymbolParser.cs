using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Language
{
    public class SymbolParser
    {
        public SignatureContainer SignatureContainer;

        public SymbolParser(SignatureContainer SignatureContainer)
        {
            this.SignatureContainer = SignatureContainer;
        }

        public bool IsEnd()
        {
            return true;
        }
    }
}
