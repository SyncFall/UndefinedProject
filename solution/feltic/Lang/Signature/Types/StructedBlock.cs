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
            if(!BeginStep()) return null;
            TokenSymbol openBlockBegin;
            if((openBlockBegin = TryToken(OperationType.Less)) == null)
            {
                ResetStep();
                return null;
            }
            TokenSymbol openBlockIdentifier;
            if((openBlockIdentifier = TryToken(TokenType.Visual)) == null && (openBlockIdentifier = TryIdentifier()) == null)
            {
                ResetStep();
                return null;
            }
            StructedAttributeList attributes = TryStructureAttributes();
            TokenSymbol openBlockClosing = TryToken(OperationType.Divide);
            TokenSymbol openBlockEnd;
            if ((openBlockEnd = TryToken(OperationType.Greater)) == null)
            {
                ResetStep();
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
                CommitStep();
                return signature;
            }
            SignatureSymbol element;
            while (true)
            {
                if((element = TryBaseSignature()) == null)
                {
                    break;
                }
                if(signature.ElementList == null)
                {
                    signature.ElementList = new SignatureList();
                }
                signature.ElementList.Add(element);
            }
            if ((signature.CloseBlockBegin = TryToken(OperationType.Less)) == null)
            {
                ResetStep();
                return null;
            }
            if ((signature.CloseBlockClosing = TryToken(OperationType.Divide)) == null)
            {
                ResetStep();
                return null;
            }
            if ((signature.CloseBlockIdentifier = TryToken(TokenType.Visual)) == null && (signature.CloseBlockIdentifier = TryIdentifier()) == null)
            {
                ResetStep();
                return null;
            }
            if ((signature.CloseBlockEnd = TryToken(OperationType.Greater)) == null)
            {
                ResetStep();
                return null;
            }
            CommitStep();
            return signature;
        }

        public StructedAttributeList TryStructureAttributes()
        {
            StructedAttributeList attributeList = null;
            while(true)
            {
                TrySpace();
                TokenSymbol identifier = TryIdentifier();
                if(identifier == null)
                {
                    break;
                }
                StructedAttributeSignature attribute = new StructedAttributeSignature();
                attribute.Identifier = identifier;
                if((attribute.Assigment = TryNonSpace(StructureType.Assigment)) == null ||
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
        public TokenSymbol OpenBlockBegin;
        public TokenSymbol OpenBlockIdentifiere;
        public TokenSymbol OpenBlockClosing;
        public TokenSymbol OpenBlockEnd;
        public StructedAttributeList Attributes;
        public SignatureList ElementList;
        public TokenSymbol CloseBlockBegin;
        public TokenSymbol CloseBlockClosing;
        public TokenSymbol CloseBlockIdentifier;
        public TokenSymbol CloseBlockEnd;

        public StructedBlockSignature() : base(SignatureType.StructedBlock)
        { }
    }

    public class StructedAttributeList : ListCollection<StructedAttributeSignature>
    { }

    public class StructedAttributeSignature : SignatureSymbol
    {
        public TokenSymbol Identifier;
        public TokenSymbol Assigment;
        public OperandSignature AssigmentOperand;
        public TokenSymbol Complete;

        public StructedAttributeSignature() : base(SignatureType.StructedAttribute)
        { }
    }
}
