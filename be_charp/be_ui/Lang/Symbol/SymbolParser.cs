using Be.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Be.Runtime
{
    public class SymbolParser
    {
        public TokenContainer TokenContainer;
        public TokenSymbol token;

        public SymbolParser(TokenContainer TokenContainer)
        {
            this.TokenContainer = TokenContainer;
            if (TokenContainer.AllTokenNodes.Size() == 0)
            {
                return;
            }
            //token = TokenContainer.AllTokenNodes.First();
        }

        /*
        public bool Next()
        {
            token = token.NextToken;
            return (token != null);
        }

        public bool IsEnd()
        {
            return (token == null);
        }

        public bool SpaceNext()
        {
            bool hasSpace = false;
            while (token != null && (token.Group == TokenGroup.Space || token.Group == TokenGroup.Comment))
            {
                hasSpace = true;
                token = token.NextToken;
            }
            if (token == null)
            {
                return false;
            }
            return hasSpace;
        }

        public bool SpaceNotNewLineNext()
        {
            bool hasSpace = false;
            while (token != null && token.Type != Token.LineSpace && (token.Group == TokenGroup.Space || token.Group == TokenGroup.Comment))
            {
                hasSpace = true;
                token = token.NextToken;
            }
            if (token == null)
            {
                return false;
            }
            return hasSpace;
        }

        public bool TrySpaceToken()
        {
            // space-token
            if (token.Group == TokenGroup.Space || token.Group == TokenGroup.Comment)
            {
                Next();
                return true;
            }
            return false;
        }

        public bool TryRegionToken()
        {
            if(token.Type == Token.RegionBegin)
            {
                RegionToken regionToken = token as RegionToken;
                Next();
                if (!SpaceNotNewLineNext())
                {
                    regionToken.Status = new TokenStatusSymbol(TokenStatus.Error, "invalid 'region' declaration");
                }
                else if(token.Type != Token.Identifier)
                {
                    regionToken.Status = new TokenStatusSymbol(TokenStatus.Error, "invalid 'region' declaration");
                }
                Next();
                return true;
            }
            else if(token.Type == Token.RegionEnd)
            {
                Next();
                return true;
            }
            else
            {
                return false;
            }
        }

        public UsingSymbol TryUsingSymbol()
        {
            // using-keyword
            if (token.Type == Token.Keyword && (token as KeywordToken).KeywordSymbol.Type == KeywordType.Using)
            {
                KeywordToken usingToken = token as KeywordToken;
                // space
                Next();
                if (!SpaceNotNewLineNext())
                {
                    usingToken.Status = new TokenStatusSymbol(TokenStatus.Error, "missing 'using namepath declaration'");
                    return null;
                }
                // namepath
                while (true)
                {
                    SpaceNotNewLineNext();
                    if (IsEnd())
                    {
                        usingToken.Status = new TokenStatusSymbol(TokenStatus.Error, "incomplete 'using namepath declaration'");
                        break;
                    }
                    else if (token.Type == Token.Identifier)
                    {
                        TokenSymbol nameToken = token;
                        Next();
                    }
                    else
                    {
                        token.Status = new TokenStatusSymbol(TokenStatus.Error, "invalid 'token'");
                        usingToken.Status = new TokenStatusSymbol(TokenStatus.Error, "invalid 'using namepath declaration'");
                        break;
                    }
                    SpaceNotNewLineNext();
                    if (IsEnd())
                    {
                        usingToken.Status = new TokenStatusSymbol(TokenStatus.Error, "incomplete 'using namepath declaration'");
                        break;
                    }
                    else if (token.Type == Token.Point)
                    {
                        Next();
                        continue;
                    }
                    else if (token.Type == Token.Complete)
                    {
                        Next();
                        break;
                    }
                    else
                    {
                        token.Status = new TokenStatusSymbol(TokenStatus.Error, "invalid 'token'");
                        usingToken.Status = new TokenStatusSymbol(TokenStatus.Error, "invalid 'using namepath declaration'");
                        break;
                    }
                }
            }
            return null;
        }

        public NamespaceSymbol TryNamespaceSymbol()
        {
            if(token.Type == Token.Keyword && (token as KeywordToken).KeywordSymbol.Type == KeywordType.Namespace)
            {
                KeywordToken namespaceToken = token as KeywordToken;
                // space
                Next();
                if (!SpaceNotNewLineNext())
                {
                    namespaceToken.Status = new TokenStatusSymbol(TokenStatus.Error, "missing 'namespace declaration'");
                    return null;
                }
                // namepath
                if(token.Type == Token.Identifier)
                {
                    Next();
                }
                else
                {
                    token.Status = new TokenStatusSymbol(TokenStatus.Error, "invalid 'token");
                    namespaceToken.Status = new TokenStatusSymbol(TokenStatus.Error, "invalid 'namespace declaration'");
                    return null;
                }
                // block start
                SpaceNext();
                if(token.Type != Token.BlockBegin)
                {
                    token.Status = new TokenStatusSymbol(TokenStatus.Error, "- invalid 'token'\n- missing 'namespace block-start'");
                    namespaceToken.Status = new TokenStatusSymbol(TokenStatus.Error, "invalid 'namespace declaration'");
                    return null;
                }
                Next();
                // objects
                while(true)
                {
                    SpaceNext();
                    if (IsEnd())
                    {
                        return null;
                    }
                    ObjectSymbol objectSymbol = TryObjectSymbol();
                    if(objectSymbol == null)
                    {
                        break;
                    }
                }
                
                // block end
                SpaceNext();
                if(token.Type != Token.BlockEnd)
                {
                    token.Status = new TokenStatusSymbol(TokenStatus.Error, "- invalid 'token'\n- missing 'namespace' block end'");
                    namespaceToken.Status = new TokenStatusSymbol(TokenStatus.Error, "invalid 'namespace declaration'");
                    return null;
                }
                
            }
            return null;
        }

        public ObjectSymbol TryObjectSymbol()
        {
            if(token.Group == TokenGroup.Keyword && (token as KeywordToken).KeywordSymbol.Group == KeywordGroup.Accessor)
            {
                SpaceNext();
            }
            if(token.Group == TokenGroup.Keyword && (token as KeywordToken).KeywordSymbol.Group == KeywordGroup.ObjectType)
            {
                SpaceNext();
                KeywordToken objectToken = token as KeywordToken;
                Next();
                if (!SpaceNext())
                {
                    objectToken.Status = new TokenStatusSymbol(TokenStatus.Error, "- invalid 'object-declaration'\n- missing 'object-name'");
                }
                else if(token.Type != Token.Identifier)
                {
                    objectToken.Status = new TokenStatusSymbol(TokenStatus.Error, "- invalid 'object-declaration'\n- invalid 'object-name'");
                }
                Next();
                SpaceNext();
                if(token.Type == Token.Keyword && (token as KeywordToken).KeywordSymbol.Type == KeywordType.Extend)
                {
                    Next();
                    SpaceNext();
                }
                
                if(token.Type != Token.BlockBegin)
                {
                    objectToken.Status = new TokenStatusSymbol(TokenStatus.Error, "- invalid 'object-declaration'\n- invalid 'object-block-start'");
                }
                if(!TryMemberSymbol() || !TryMethodSymbol() || !TryPropertySymbol())
                {
                    //objectToken.Status = new TokenStatusSymbol(TokenStatus.Error, "- invalid 'object-content-block'");
                }
                
                SpaceNext();
                if (token.Type != Token.BlockEnd)
                {
                    objectToken.Status = new TokenStatusSymbol(TokenStatus.Error, "- invalid 'object-declaration'\n- invalid 'object-block-end'");
                }
                
            }

            return null;
        }

        public bool TryMemberSymbol()
        {
            return false;
        }

        public bool TryMethodSymbol()
        {
            return false;
        }

        public bool TryPropertySymbol()
        {
            return false;
        }
        */
    }
}
