using Bee.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bee.UI;

namespace Bee.Integrator
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
