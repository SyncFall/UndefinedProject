using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    // n log n - keyword-lookup trie
    public class SymbolTable
    {
        private Node Root = new Node(null);

        public SymbolTable()
        { }

        public void AddSymbol(Symbol Symbol)
        {
            string String = Symbol.String;
            if (String == null || String.Length == 0)
            {
                throw new Exception("invalid string");
            }
            InsertNode(String, Symbol, Root);
        }

        public Symbol FindSymbol(ref char[] Chars, int start, int end)
        {
            int index=start;
            Node node=Root;
            Node child;
            while (true)
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
                // not-exact-node
                else
                {
                    return null;
                }
            }
        }

        private void InsertNode(string String, Symbol Symbol, Node Node)
        {
            int Char = String[0];
            if(Char < 0 || Char > 127)
            {
                throw new Exception("invalid char");
            }
            Node child = Node.Childrens[Char];
            if(child == null)
            {
                child = new Node((String.Length == 1 ? Symbol : null));
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

        public class Node
        {
            public Symbol Symbol;
            public Node[] Childrens;

            public Node(Symbol Symbol)
            {
                this.Symbol = Symbol;
                this.Childrens = new Node[128];
            }
        }
    }
}
