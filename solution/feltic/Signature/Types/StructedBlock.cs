using feltic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public partial class SignatureParser
    {
        public StructedBlockSignature TryStructedBlock()
        {
            if(!Begin()) return null;
            Symbol openBlockBegin, openBlockIdentifier;
            if((openBlockBegin = TryNonSpace(OperationType.Less)) == null ||
               (openBlockIdentifier = TryNonSpace(TokenType.Visual)) == null && (openBlockIdentifier = TryNonSpace(TokenType.Identifier)) == null
            ){
                Reset();
                return null;
            }
            StructedAttributeList attributes = TryStructedBlockAttributes();
            Symbol openBlockClosing = TryNonSpace(OperationType.Divide);
            Symbol openBlockEnd;
            if((openBlockEnd = TryNonSpace(OperationType.Greater)) == null)
            {
                Reset();
                return null;
            }
            StructedBlockSignature signature = new StructedBlockSignature();
            signature.OpenBlockBegin = openBlockBegin;
            signature.OpenBlockIdentifiere = openBlockIdentifier;
            signature.Attributes = attributes;
            signature.OpenBlockClosing = openBlockClosing;
            signature.OpenBlockEnd = openBlockEnd;
            if(openBlockClosing != null)
            {
                Commit();
                return signature;
            }
            signature.Elements = TryStatementList();
            if ((signature.CloseBlockBegin = TryNonSpace(OperationType.Less)) == null ||
                (signature.CloseBlockClosing = TryNonSpace(OperationType.Divide)) == null ||
                ((signature.CloseBlockIdentifier = TryNonSpace(TokenType.Visual)) == null && (signature.CloseBlockIdentifier = TryNonSpace(TokenType.Identifier)) == null) ||
                (signature.CloseBlockEnd = TryNonSpace(OperationType.Greater)) == null
            ){
                Reset();
                return signature;
            }
            Commit();
            return signature;
        }

        public StructedAttributeList TryStructedBlockAttributes()
        {
            StructedAttributeList attributeList = null;
            while(true)
            {
                Symbol identifier = TryNonSpace(TokenType.Identifier);
                if (identifier == null) break;
                StructedAttributeSignature attribute = new StructedAttributeSignature();
                attribute.Identifier = identifier;
                if((attribute.Assigment = TryNonSpace(OperationType.Assigment)) == null ||
                   (attribute.AssigmentOperand = TryOperand()) == null
                ){
                    ;
                }
                if (attributeList == null)
                    attributeList = new StructedAttributeList();
                attributeList.Add(attribute);
            }
            return attributeList;
        }
    }

    public class StructedBlockSignature : SignatureSymbol
    {
        public Symbol OpenBlockBegin;
        public Symbol OpenBlockIdentifiere;
        public Symbol OpenBlockClosing;
        public Symbol OpenBlockEnd;
        public StructedAttributeList Attributes;
        public SignatureList Elements;
        public Symbol CloseBlockBegin;
        public Symbol CloseBlockClosing;
        public Symbol CloseBlockIdentifier;
        public Symbol CloseBlockEnd;

        public StructedBlockSignature() : base(SignatureType.StructedBlock)
        { }

        public override string ToString()
        {
            return "structed_block(element_count:"+(Elements!=null?Elements.Size:0)+")";
        }
    }

    public class StructedAttributeList : ListCollection<StructedAttributeSignature>
    { }

    public class StructedAttributeSignature : SignatureSymbol
    {
        public Symbol Identifier;
        public Symbol Assigment;
        public OperandSignature AssigmentOperand;
        public Symbol Complete;

        public StructedAttributeSignature() : base(SignatureType.StructedAttribute)
        { }
    }
}
