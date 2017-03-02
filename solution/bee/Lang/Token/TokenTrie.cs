using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Language
{
    public class TokenTrie
    {
        private Node Root; 

        public TokenTrie()
        {
            Root = new Node(0, null);
        }

        public void Add(TokenSymbol Symbol)
        {
            string String = Symbol.String;
            if (String == null || String.Length == 0)
            {
                throw new Exception("invalid string");
            }
            InsertNode(String, Symbol, Root);
        }

        public TokenSymbol Find(char[] Chars, int start, int end)
        {
            /*
            if (Chars == null || Chars.Length == 0)
            {
                throw new Exception("invalid string");
            }
            */
            return FindSymbolNode(Chars, start, end, start, Root);
        }

        private void InsertNode(string String, TokenSymbol Symbol, Node Node)
        {
            int Char = String[0];
            if(Char < 0 || Char > 127)
            {
                throw new Exception("invalid char");
            }
            Node child = Node.Childrens[Char];
            if(child == null)
            {
                child = new Node(Char, (String.Length == 1 ? Symbol : null));
                Node.Childrens[Char] = child;
                if(String.Length > 1)
                {
                    InsertNode(String.Substring(1), Symbol, child);
                }
            }
            else
            {
                if(String.Length == 1)
                {
                    child.Symbol = Symbol;
                }
                else
                {
                    InsertNode(String.Substring(1), Symbol, child);
                }
            }
        }

        private TokenSymbol FindSymbolNode(char[] Chars, int start, int end, int index, Node node)
        {
            /*
            if (Char > 127)
            {
                throw new Exception("invalid char");
            }
            */
            Node child;
            while(true)
            {
                child = node.Childrens[Chars[index]];
                // no-node
                if (child == null)
                {
                    return null;
                }
                // go-depth
                else if (index+1 < end)
                {
                    index++;
                    node = child;
                    continue;
                }
                // exact-node
                else if (child.Symbol != null)
                {
                    return child.Symbol;
                }
                // no-exact-node
                else
                {
                    return null;
                }
            }
        }

        class Node
        {
            public int Char;
            public TokenSymbol Symbol;
            public Node[] Childrens;

            public Node(int Char, TokenSymbol Symbol)
            {
                this.Char = Char;
                this.Symbol = Symbol;
                this.Childrens = new Node[128];
            }
        }
    }
}
