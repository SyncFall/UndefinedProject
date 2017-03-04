using Bee.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Language
{
    public partial class SignatureParser
    {
        public IdentifierPathSignature TryIdentifierPath()
        {
            IdentifierPathSignature signature = new IdentifierPathSignature();
            while (TryToken(TokenType.Identifier) != null)
            {
                IdentifierPathElementSignatur element = new IdentifierPathElementSignatur();
                signature.PathElements.Add(element);
                element.Identifier = PrevToken;
                element.Seperator = TrySeperator(StructureType.Point);
                if (element.Seperator == null)
                {
                    break;
                }
            }
            return (signature.PathElements.Size > 0 ? signature : null);
        }

        public TokenSymbol TryIdentifier()
        {
            if (TryToken(TokenType.Identifier) == null)
            {
                return null;
            }
            return PrevToken;
        }

        public TypeDeclarationSignature TryTypeDeclaration(bool WithAssigment=true)
        {
            TrySpace();
            TypeDeclarationSignature signatur = new TypeDeclarationSignature();
            if (TryToken(TokenType.Native) != null)
            {
                signatur.TypeNative = PrevToken;
            }
            else if (TryToken(TokenType.Identifier) != null)
            {
                signatur.TypeIdentifier = PrevToken;
            }
            else
            {
                return null;
            }
            signatur.TypeGeneric = TryGenericDeclaration();
            TrySpace();
            if((signatur.NameIdentifier = TryIdentifier()) == null){
                ;
            }
            if(WithAssigment)
            {
                if((signatur.Assigment = TrySeperator(StructureType.Assigment)) == null ||
                   (signatur.AssigmentExpression = TryExpression()) == null
                ){
                    ;
                }
            }
            return signatur;
        }

        public GenericDeclarationSignature TryGenericDeclaration()
        {
            TrySpace();
            BeginStep();
            TokenSymbol blockBegin;
            if((blockBegin = TryToken(OperationType.Less)) == null)
            {
                ResetStep();
                return null;
            }
            GenericDeclarationSignature signature = new GenericDeclarationSignature();
            signature.BlockBegin = blockBegin;
            while(true)
            {
                GenericElementSignature element = new GenericElementSignature();
                if((element.Identifier = TryIdentifier())!= null)
                {
                    signature.ElementList.Add(element);
                    element.Generic = TryGenericDeclaration();
                    if((element.Seperator = TrySeperator(StructureType.Seperator)) != null)
                    {
                        continue;
                    }
                }
                if((signature.BlockEnd = TryToken(OperationType.Greater)) != null)
                {
                    CommitStep();
                    break;
                }
                //
                ResetStep();
                break;
            }
            return signature;
        }

        public ParameterDeclarationSignature TryParameterDeclaration(StructureType StructureBegin, StructureType StructureEnd)
        {
            TrySpace();
            TokenSymbol blockBegin;
            if ((blockBegin = TryBlock(StructureBegin)) == null)
            {
                return null;
            }
            ParameterDeclarationSignature signature = new ParameterDeclarationSignature();
            signature.BlockBegin = blockBegin;
            TypeDeclarationSignature typeDeclaration;
            while((typeDeclaration = TryTypeDeclaration()) != null)
            {
                ParameterSignature parameter = new ParameterSignature(typeDeclaration);
                signature.ParameterList.Add(parameter);
                parameter.Seperator = TrySeperator(StructureType.Seperator);
                if (parameter.Seperator == null)
                {
                    break;
                }
            }
            signature.BlockEnd = TryBlock(StructureEnd);
            return signature;
        }
    }

    public class TypeDeclarationSignature : SignatureSymbol
    {
        public TokenSymbol TypeNative;
        public TokenSymbol TypeIdentifier;
        public GenericDeclarationSignature TypeGeneric;
        public TokenSymbol NameIdentifier;
        public TokenSymbol Assigment;
        public ExpressionSignature AssigmentExpression;
        public TokenSymbol Seperator;
     
        public TypeDeclarationSignature() : base(SignatureType.TypeDeclartion)
        { }

        public override string ToString()
        {
            string str = "type(";
            if(TypeNative != null)
            {
                str += "native:" + TypeNative.String;
            }
            if(TypeIdentifier != null)
            {
                str += "object:" + TypeIdentifier.String;
            }
            if(TypeGeneric != null)
            {
                str += ", " +TypeGeneric.ToString();
            }
            if(NameIdentifier != null)
            {
                str += ", name:" + NameIdentifier.String;
            }
            if(Assigment != null)
            {
                str += ", assigment(" + AssigmentExpression + ")";
            }
            return str+")";
        }
    }

    public class GenericDeclarationSignature : SignatureSymbol
    {
        public TokenSymbol BlockBegin;
        public GenericElementListSignature ElementList = new GenericElementListSignature();
        public TokenSymbol BlockEnd;

        public GenericDeclarationSignature() : base(SignatureType.GenericDeclaration)
        { }

        public override string ToString()
        {
            string str = "generic(";
            str += ElementList.ToString();
            str += ")";
            return str;
        }
    }

    public class GenericElementListSignature : ListCollection<GenericElementSignature>
    {
        public override string ToString()
        {
            string str = "";
            for(int i=0; i<Size; i++)
            {
                str += Get(i);
                if(i < Size-1)
                {
                    str += ", ";
                }
            }
            return str;
        }
    }

    public class GenericElementSignature : SignatureSymbol
    {
        public TokenSymbol Identifier;
        public TokenSymbol Seperator;
        public GenericDeclarationSignature Generic = new GenericDeclarationSignature();

        public GenericElementSignature() : base(SignatureType.GenericElement)
        { }

        public override string ToString()
        {
            string str = "element(name:" + Identifier.String;
            if(Generic!= null && Generic.ElementList.Size > 0)
            {
                str += ", type("+Generic.ToString()+")";
            }
            str += ")";
            return str;
        }
    }

    public class ParameterDeclarationSignature : SignatureSymbol
    {
        public TokenSymbol BlockBegin;
        public ParameterListSignature ParameterList = new ParameterListSignature();
        public TokenSymbol BlockEnd;

        public ParameterDeclarationSignature() : base(SignatureType.ParameterDeclaration)
        { }

        public override string ToString()
        {
            return ParameterList.ToString();
        }
    }

    public class ParameterListSignature : ListCollection<ParameterSignature>
    {
        public override string ToString()
        {
            string str = "parameter_list(";
            for (int i = 0; i < Size; i++)
            {
                str += Get(i).ToString();
                if(i < Size-1)
                {
                    str += ", ";
                }
            }
            str += ")";
            return str;
        }
    }

    public class ParameterSignature : SignatureSymbol
    {
        public TypeDeclarationSignature TypeDeclaration;
        public ExpressionSignature Expression;
        public TokenSymbol Seperator;

        public ParameterSignature(TypeDeclarationSignature TypeDeclaration) : base(SignatureType.Parameter)
        {
            this.TypeDeclaration = TypeDeclaration;
        }

        public ParameterSignature(ExpressionSignature Expression) : base(SignatureType.Parameter)
        {
            this.Expression = Expression;
        }

        public override string ToString()
        {
            string str = "parameter(";
            if(TypeDeclaration != null)
            {
                str += TypeDeclaration;
            }
            if(Expression != null)
            {
                str += Expression;
            }
            str += ")";
            return str;
        }
    }

    public class IdentifierPathSignature : SignatureSymbol
    {
        public IdentifierPathElemementList PathElements = new IdentifierPathElemementList();

        public IdentifierPathSignature() : base(SignatureType.IdentifierPath)
        { }

        public override string ToString()
        {
            string str = "path(";
            for (int i = 0; i < PathElements.Size; i++)
            {
                str += PathElements.Get(i);
            }
            return str + ")";
        }
    }

    public class IdentifierPathElemementList : ListCollection<IdentifierPathElementSignatur>
    { }

    public class IdentifierPathElementSignatur : SignatureSymbol
    {
        public TokenSymbol Identifier;
        public TokenSymbol Seperator;

        public IdentifierPathElementSignatur() : base(SignatureType.IdentifierPathElement)
        { }

        public override string ToString()
        {
            if (Identifier != null)
            {
                string str = Identifier.String;
                if (Seperator != null)
                {
                    str += Seperator.String;
                }
                return str;
            }
            return "";
        }
    }
}
