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
                IdentifierPathElementSignatur elementSignature = new IdentifierPathElementSignatur();
                signature.PathElements.Add(elementSignature);
                elementSignature.Identifier = PrevToken;
                elementSignature.PointSeperator = TrySeperator(StructureType.Point);
                if (elementSignature.PointSeperator == null)
                {
                    break;
                }
            }
            return (signature.PathElements.Size > 0 ? signature : null);
        }

        public IdentifierSignature TryIdentifier()
        {
            if (TryToken(TokenType.Identifier) == null)
            {
                return null;
            }
            return new IdentifierSignature(PrevToken);
        }

        public TypeDeclarationSignature TryTypeDeclaration(bool WithAssigment=true)
        {
            TrySpace();
            TypeDeclarationSignature signatur = new TypeDeclarationSignature();
            if (TryToken(TokenType.Native) != null)
            {
                signatur.TypeNative = new NativeSignature(PrevToken);
            }
            else if (TryToken(TokenType.Identifier) != null)
            {
                signatur.TypeIdentifier = new IdentifierSignature(PrevToken);
            }
            else
            {
                return null;
            }
            if (!TrySpace() ||
               (signatur.NameIdentifier = TryIdentifier()) == null
            ){
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

        public ParameterDeclarationSignature TryParameterDeclaration()
        {
            TrySpace();
            BlockSignature blockBegin;
            if ((blockBegin = TryBlock(StructureType.ClosingBegin)) == null)
            {
                return null;
            }
            ParameterDeclarationSignature signature = new ParameterDeclarationSignature();
            signature.BlockBegin = blockBegin;
            TypeDeclarationSignature typeDeclaration;
            while ((typeDeclaration = TryTypeDeclaration()) != null)
            {
                ParameterSignature parameter = new ParameterSignature(typeDeclaration);
                signature.ParameterList.Add(parameter);
                parameter.Seperator = TrySeperator(StructureType.Seperator);
                if (parameter.Seperator == null)
                {
                    break;
                }
            }
            signature.BlockEnd = TryBlock(StructureType.ClosingEnd);
            return signature;
        }
    }

    public class TypeDeclarationSignature : SignatureSymbol
    {
        public NativeSignature TypeNative;
        public IdentifierSignature TypeIdentifier;
        public IdentifierSignature NameIdentifier;
        public SeperatorSignature Assigment;
        public ExpressionSignature AssigmentExpression;
     
        public TypeDeclarationSignature() : base(SignatureType.TypeDeclartion)
        { }

        public override string ToString()
        {
            string str = "";
            if (TypeNative != null)
            {
                str += "native:" + TypeNative.Native.String;
            }
            else if (TypeIdentifier != null)
            {
                str += "object:" + TypeIdentifier.Identifier.String;
            }
            if (NameIdentifier != null)
            {
                str += ", name:" + NameIdentifier.Identifier.String;
            }
            if(Assigment != null)
            {
                str += ", assigment(" + AssigmentExpression + ")";
            }
            return str;
        }
    }

    public class ParameterDeclarationSignature : SignatureSymbol
    {
        public BlockSignature BlockBegin;
        public ParameterListSignature ParameterList = new ParameterListSignature();
        public BlockSignature BlockEnd;

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
        public SeperatorSignature Seperator;

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
        public SeperatorSignature PointSeperator;

        public IdentifierPathElementSignatur() : base(SignatureType.IdentifierPathElement)
        { }

        public override string ToString()
        {
            if (Identifier != null)
            {
                string str = Identifier.String;
                if (PointSeperator != null)
                {
                    str += PointSeperator.Seperator.String;
                }
                return str;
            }
            return "";
        }
    }

    public class IdentifierSignature : SignatureSymbol
    {
        public TokenSymbol Identifier;

        public IdentifierSignature(TokenSymbol IdentifiereToken) : base(SignatureType.Identifier)
        {
            this.Identifier = IdentifiereToken;
        }

        public override string ToString()
        {
            return "name(" + Identifier != null ? Identifier.String : "" + ")";
        }
    }
}
