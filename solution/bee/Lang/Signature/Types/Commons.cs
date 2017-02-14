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
                elementSignature.Identifier = PrevToken as IdentifierToken;
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
            IdentifierSignature signatur = new IdentifierSignature();
            signatur.IdentifiereToken = PrevToken as IdentifierToken;
            return signatur;
        }

        public TypeDeclarationSignature TryTypeDeclaration()
        {
            TypeDeclarationSignature signatur = new TypeDeclarationSignature();
            if (TryToken(TokenType.Native) != null)
            {
                signatur.TypeNative = PrevToken as NativeToken;
            }
            else if (TryToken(TokenType.Identifier) != null)
            {
                signatur.TypeIdentifier = PrevToken as IdentifierToken;
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
                ParameterDeclartionElementSignature parameterElement = new ParameterDeclartionElementSignature(typeDeclaration);
                signature.ParameterList.Add(parameterElement);
                parameterElement.ParameterSeperator = TrySeperator(StructureType.Seperator);
                if (parameterElement.ParameterSeperator == null)
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
        public NativeToken TypeNative;
        public IdentifierToken TypeIdentifier;
        public IdentifierSignature NameIdentifier;
     
        public TypeDeclarationSignature() : base(SignatureType.TypeDeclartion)
        { }

        public override string ToString()
        {
            string str = "";
            if (TypeNative != null)
            {
                str += "native:" + TypeNative.String;
            }
            else if (TypeIdentifier != null)
            {
                str += "object:" + TypeIdentifier.String;
            }
            if (NameIdentifier != null)
            {
                str += ", name:" + NameIdentifier.IdentifiereToken.String;
            }
            return str;
        }
    }

    public class ParameterDeclarationSignature : SignatureSymbol
    {
        public BlockSignature BlockBegin;
        public ParameterDeclarationElementList ParameterList = new ParameterDeclarationElementList();
        public BlockSignature BlockEnd;

        public ParameterDeclarationSignature() : base(SignatureType.ParameterDeclaration)
        { }

        public override string ToString()
        {
            return ParameterList.ToString();
        }
    }

    public class ParameterDeclarationElementList : ListCollection<ParameterDeclartionElementSignature>
    { }

    public class ParameterDeclartionElementSignature : SignatureSymbol
    {
        public TypeDeclarationSignature TypeDeclaration;
        public SeperatorSignature ParameterSeperator;

        public ParameterDeclartionElementSignature(TypeDeclarationSignature TypeDeclaration) : base(SignatureType.ParameterElementDeclaration)
        {
            this.TypeDeclaration = TypeDeclaration;
        }

        public override string ToString()
        {
            return "parameter(" + TypeDeclaration + ")";
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
        public IdentifierToken Identifier;
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
                    str += PointSeperator.SeperatorToken.String;
                }
                return str;
            }
            return "";
        }
    }

    public class IdentifierSignature : SignatureSymbol
    {
        public IdentifierToken IdentifiereToken;

        public IdentifierSignature() : base(SignatureType.Identifier)
        { }

        public override string ToString()
        {
            return "name(" + IdentifiereToken != null ? IdentifiereToken.String : "" + ")";
        }
    }
}
