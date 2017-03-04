using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feltic.Language
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
               (signature = TryObject()) != null ||
               (signature = TryMember()) != null ||
               (signature = TryMethod()) != null ||
               (signature = TryProperty()) != null ||
               (signature = TryCode()) != null ||
               (signature = TryStatement()) != null ||
               (signature = TryExpression()) != null ||
               (signature = TryTypeDeclaration()) != null ||
               (signature = TryParameterDeclaration(StructureType.ClosingBegin, StructureType.ClosingEnd)) != null ||
               (signature = TryParameterDeclaration(StructureType.BracketBegin, StructureType.BracketEnd)) != null ||
               (signature = TryIdentifierPath()) != null ||
               (signature = TryUnknown()) != null
            ){
                ;
            }
            //Console.WriteLine(signature);
            return signature;
        }
    }
}
