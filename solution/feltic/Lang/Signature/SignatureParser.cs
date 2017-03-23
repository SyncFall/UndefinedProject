using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public partial class SignatureParser
    {
        public SignatureSymbol TrySignature()
        {
            if(IsEnd())
            {
                return null;
            }
            TrySpace();
            SignatureSymbol signature=null;
            if((signature = TryUse()) != null ||
               (signature = TryScope()) != null ||
               (signature = TryBaseSignature()) != null ||
               (signature = TryUnknown()) != null
            ){
                ;
            }
            Console.WriteLine(signature);
            return signature;
        }

        public SignatureSymbol TryBaseSignature()
        {
            TrySpace();
            SignatureSymbol signature = null;
            if ((signature = TryObject()) != null ||
                (signature = TryMember()) != null ||
                (signature = TryMethod()) != null ||
                (signature = TryProperty()) != null ||
                (signature = TryStatement()) != null)
            {
                return signature;
            }
            return signature;

        }
    }
}
