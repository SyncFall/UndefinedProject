using Feltic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feltic.Language
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
                element.Seperator = TryNonSpace(StructureType.Point);
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
            signatur.TypeArray = TryArrayDeclaration();
            TrySpace();
            if((signatur.NameIdentifier = TryIdentifier()) == null){
                ;
            }
            if(WithAssigment)
            {
                if((signatur.Assigment = TryNonSpace(StructureType.Assigment)) == null ||
                   (signatur.AssigmentExpression = TryExpression()) == null
                ){
                    ;
                }
            }
            return signatur;
        }

        public GenericDeclarationSignature TryGenericDeclaration()
        {
            if(!BeginStep()) return null;
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
                    if((element.Seperator = TryNonSpace(StructureType.Seperator)) != null)
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

        public ArrayDeclarationSignature TryArrayDeclaration()
        {
            TokenSymbol blockBegin;
            if(!BeginStep()) return null;
            if((blockBegin = TryNonSpace(StructureType.BracketBegin)) == null)
            {
                ResetStep();
                return null; ;
            }
            ArrayDeclarationSignature signature = new ArrayDeclarationSignature();
            signature.BlockBegin = blockBegin;
            while(true)
            {
                if((signature.BlockEnd = TryNonSpace(StructureType.BracketEnd)) != null)
                {
                    CommitStep();
                    break;
                }
                TokenSymbol dimensionToken;
                if ((dimensionToken = TryNonSpace(StructureType.Seperator)) != null)
                {
                    signature.DimensionSymbols.Add(dimensionToken);
                    continue;
                }
                if ((signature.BlockEnd = TryNonSpace(StructureType.BracketEnd)) != null)
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
            TokenSymbol blockBegin;
            if ((blockBegin = TryNonSpace(StructureBegin)) == null)
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
                parameter.Seperator = TryNonSpace(StructureType.Seperator);
                if (parameter.Seperator == null)
                {
                    break;
                }
            }
            signature.BlockEnd = TryNonSpace(StructureEnd);
            return signature;
        }
    }

    public class TypeDeclarationSignature : SignatureSymbol
    {
        public TokenSymbol TypeNative;
        public TokenSymbol TypeIdentifier;
        public GenericDeclarationSignature TypeGeneric;
        public ArrayDeclarationSignature TypeArray;
        public TokenSymbol NameIdentifier;
        public TokenSymbol Assigment;
        public ExpressionSignature AssigmentExpression;
        public TokenSymbol Seperator;
     
        public TypeDeclarationSignature() : base(SignatureType.TypeDeclaration)
        { }

        public override string ToString()
        {
            string str = "type(";
            if(TypeNative != null)
                str += "native:" + TypeNative.String;
            if(TypeIdentifier != null)
                str += "object:" + TypeIdentifier.String;
            if(TypeGeneric != null)
                str += ", " + TypeGeneric.ToString();
            if(TypeArray != null)
                str += ", " + TypeArray.ToString();
            if(NameIdentifier != null)
                str += ", name:" + NameIdentifier.String;
            if(Assigment != null)
                str += ", assigment(" + AssigmentExpression + ")";
            return str+")";
        }
    }

    public class GenericDeclarationSignature : SignatureSymbol
    {
        public TokenSymbol BlockBegin;
        public ListCollection<GenericElementSignature> ElementList = new ListCollection<GenericElementSignature>();
        public TokenSymbol BlockEnd;

        public GenericDeclarationSignature() : base(SignatureType.GenericDeclaration)
        { }

        public override string ToString()
        {
            return "generic("+ElementList.ToString()+")";
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
                str += ", "+Generic.ToString();
            }
            str += ")";
            return str;
        }
    }

    public class ArrayDeclarationSignature : SignatureSymbol
    {
        public TokenSymbol BlockBegin;
        public ListCollection<TokenSymbol> DimensionSymbols = new ListCollection<TokenSymbol>();
        public TokenSymbol BlockEnd;

        public ArrayDeclarationSignature() : base(SignatureType.ArrayDeclaration)
        { }

        public override string ToString()
        {
            return "array(dimesion-size("+(DimensionSymbols.Size+1)+")";
        }
    }

    public class ParameterDeclarationSignature : SignatureSymbol
    {
        public TokenSymbol BlockBegin;
        public ListCollection<ParameterSignature> ParameterList = new ListCollection<ParameterSignature>();
        public TokenSymbol BlockEnd;

        public ParameterDeclarationSignature() : base(SignatureType.ParameterDeclaration)
        { }

        public override string ToString()
        {
            return "parameter_list("+ParameterList+")";
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
        public ListCollection<IdentifierPathElementSignatur> PathElements = new ListCollection<IdentifierPathElementSignatur>();

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
