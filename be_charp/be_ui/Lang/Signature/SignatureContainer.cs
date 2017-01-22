using Be.Runtime;
using Be.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.Runtime
{
    public class SignatureContainer
    {
        public SignatureContainer()
        { }

        public void SetTokenContainer(TokenContainer TokenContainer)
		{
            SignatureParser SignatureParser = new SignatureParser(TokenContainer);
            while (!SignatureParser.IsEnd())
            {
                if(SignatureParser.TrySpace() != null ||
                   SignatureParser.TryUse() != null ||
                   SignatureParser.TryScope() != null
                ){
                    continue;
                }
                else
                {
                    SignatureParser.Next();
                }
            }
        }
    }
}
