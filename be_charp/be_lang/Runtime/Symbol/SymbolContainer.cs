using Be.Runtime;
using Be.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.Runtime
{
    public class SymbolContainer
    {
        public TokenContainer TokenContainer;
        public SymbolParser Parser;

        public SymbolContainer()
        { }

        public void Operate(TokenContainer TokenContainer)
        {
            this.TokenContainer = TokenContainer;

            Parser = new SymbolParser(TokenContainer);
            bool hasInvalidSourceFileToken = false;
            while (true)
            {
                if(Parser.IsEnd())
                {
                    break;
                }
                else if(Parser.TrySpaceToken())
                {
                    continue;
                }
                else if (Parser.TryRegionToken())
                {
                    continue;
                }

                UsingSymbol usingSymbol = Parser.TryUsingSymbol();
                if(usingSymbol != null)
                {
                    continue;
                }
                NamespaceSymbol namespaceSymbol = Parser.TryNamespaceSymbol();
                if(namespaceSymbol != null)
                {
                    continue;
                }

                if (!hasInvalidSourceFileToken)
                {
                    TokenSymbol token = Parser.token;
                    token.Status = new TokenStatusSymbol(TokenStatus.Error, "- invalid 'token'\n- only 'using and namespace' declaration allowed");
                    hasInvalidSourceFileToken = true;
                }

                Parser.Next();
            }
        }
    }
}
