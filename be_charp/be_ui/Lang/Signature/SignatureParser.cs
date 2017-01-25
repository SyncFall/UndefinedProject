using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Language
{
    public partial class SignatureParser
    {
        public SignatureSymbol TrySignature()
        {
            if (IsEnd())
            {
                return null;
            }
            TrySpace();
            SignatureSymbol signature = null;
            if ((signature = TryUse()) != null ||
               (signature = TryScope()) != null ||
               (signature = TryObject()) != null ||
               (signature = TryObjectElement()) != null ||
               (signature = TryStatement(null)) != null ||
               (signature = TryTypeDeclaration()) != null ||
               (signature = TryParameterDeclaration()) != null ||
               (signature = TryIdentifierPath()) != null ||
               (signature = TryIdentifier()) != null ||
               (signature = TryUnknown()) != null
            )
            {
                Console.WriteLine(signature);
            }
            return signature;
        }
    }
}
