using Be.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Be.UI;

namespace Be.Integrator
{
    public class CodeToken
    { 
        public TokenSymbol TokenSymbol;
        public TextContainer TextContainer;
      
        public CodeToken(TokenSymbol TokenSymbol, TextContainer TextContainer)
        {
            this.TokenSymbol = TokenSymbol;
            this.TextContainer = TextContainer;
        }
    }
}
