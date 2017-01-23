using Be.Runtime;
using Be.Runtime.Types;
using Bee.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.Runtime
{
    public class SignatureNode
    {
        public SignatureNode Next;
        public SignatureNode Prev;
        public SignatureSymbol Signature;

        public SignatureNode(SignatureSymbol Symbol)
        {
            this.Signature = Symbol;
        }
    }

    public class SignatureContainer
    {
        public ListCollection<SignatureNode> SignatureNodes = new ListCollection<SignatureNode>(64);

        public SignatureContainer()
        { }

        public void AddSignature(SignatureSymbol signature)
        {
            SignatureNode newNode = new SignatureNode(signature);
            if(SignatureNodes.Size() > 0)
            {
                SignatureNode lastNode = SignatureNodes.Get(SignatureNodes.Size()-1);
                lastNode.Next = newNode;
                newNode.Prev = lastNode;
            }
            SignatureNodes.Add(newNode);
        }

        public void SetTokenContainer(TokenContainer TokenContainer)
		{
            SignatureNodes.Clear();
            SignatureParser SignatureParser = new SignatureParser(TokenContainer);
            while (!SignatureParser.IsEnd())
            {
                SignatureSymbol signature = SignatureParser.TrySignature();
                if(signature != null)
                {
                    AddSignature(signature);
                }
            }
        }
    }
}
