using Be.Runtime;
using Be.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.Runtime
{
    public class SignaturContainer
    {
        public TokenContainer TokenContainer;
        public SignaturParser SignaturParser;

        public SignaturContainer()
        { }

        public void SetTokenContainer(TokenContainer TokenContainer)
		{
            this.TokenContainer = TokenContainer;
            SignaturParser = new SignaturParser(TokenContainer);
            while (!SignaturParser.IsEnd())
            {
                if(SignaturParser.TrySpace() != null ||
                   SignaturParser.TryUsing() != null ||
                   SignaturParser.TryNamespace() != null
                ){
                    continue;
                }
                else
                {
                    SignaturParser.Next();
                }
            }
        }
    }
}
