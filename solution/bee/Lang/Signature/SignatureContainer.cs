using Bee.Language;
using Bee.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Language
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
        public SourceText SourceText;
        public ListCollection<SignatureNode> SignatureNodes = new ListCollection<SignatureNode>(64);


        public SignatureContainer()
        { }

        public void AddSignature(SignatureSymbol signature)
        {
            SignatureNode newNode = new SignatureNode(signature);
            if(SignatureNodes.Size > 0)
            {
                SignatureNode lastNode = SignatureNodes.Get(SignatureNodes.Size-1);
                lastNode.Next = newNode;
                newNode.Prev = lastNode;
            }
            SignatureNodes.Add(newNode);
        }

        public void SetContainer(TokenContainer TokenContainer)
		{
            this.SourceText = TokenContainer.SourceText;
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
