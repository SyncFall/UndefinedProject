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
            TrySpace();
            if(!Begin()) return null;
            StructedBlockSignature signature = new StructedBlockSignature();
            if((signature.OpenBlockBegin = TryNonSpace(OperationType.Less)) == null ||
               ((signature.OpenBlockIdentifiere = TryNonSpace(TokenType.Visual)) == null && (signature.OpenBlockIdentifiere = TryNonSpace(TokenType.Identifier)) == null)
            ){
                Reset();
                return null;
            }
            StructedAttributeList attributes = TryStructedBlockAttributes();
            signature.OpenBlockClosing = TryNonSpace(OperationType.Divide);
            if((signature.OpenBlockEnd = TryNonSpace(OperationType.Greater)) == null)
            {
                Reset();
                return null;
            }
            signature.Attributes = attributes;
            if(signature.OpenBlockClosing != null)
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
                   (attribute.AssigmentExpression = TryExpression(false)) == null
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
            return "structed_block(count:"+(Elements!=null?Elements.Size:0)+")";
        }
    }

    public class StructedAttributeList : ListCollection<StructedAttributeSignature>
    { }

    public class StructedAttributeSignature : SignatureSymbol
    {
        public Symbol Identifier;
        public Symbol Assigment;
        public ExpressionSignature AssigmentExpression;
        public Symbol Complete;

        public StructedAttributeSignature() : base(SignatureType.StructedAttribute)
        { }
    }
}
