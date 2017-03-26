using feltic.Language;
using feltic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public class SignatureContainer
    {
        public ListCollection<SignatureSymbol> Signatures = new ListCollection<SignatureSymbol>();

        public void SetTokenContainer(TokenContainer TokenContainer)
		{
            Signatures.Clear();
            SignatureParser parser = new SignatureParser(TokenContainer);
            SignatureSymbol signature;
            while(true)
            {
                signature = parser.TrySignature();
                if(signature == null) break;
                Signatures.Add(signature);
            }
        }
    }
}
