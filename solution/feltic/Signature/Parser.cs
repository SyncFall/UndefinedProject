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
            if (IsEnd)
                return null;
            SignatureSymbol signature=null;
            if((signature = TryUse()) != null ||
               (signature = TryScope()) != null ||
               (signature = TryObject()) != null ||
               (signature = TryVariable()) != null ||
               (signature = TryFunction()) != null ||
               (signature = TryProperty()) != null ||
               (signature = TryStatement()) != null ||
               (signature = TryUnknown()) != null
            ){
                ;
            }
            Console.WriteLine(signature);
            return signature;
        }
    }
}
