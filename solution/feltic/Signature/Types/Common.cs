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
        public PathSignature TryPath()
        {
            TrySpace();
            if(TryToken(TokenType.Identifier) == null)
            {
                return null;
            }
            PathSignature signature = new PathSignature();
            do
            {
                PathNodeSignature element = new PathNodeSignature();
                signature.Elements.Add(element);
                element.Identifier = PrevToken;
                element.Seperator = TryNonSpace(StructureType.Point);
                if(element.Seperator == null)
                {
                    break;
                }
            }while (TryToken(TokenType.Identifier) != null) ;
            return signature;
        }

        public Symbol TryIdentifier()
        {
            TrySpace();
            if(TryToken(TokenType.Identifier) != null)
            {
                return PrevToken;
            }
            return null;
        }

        public Symbol TryType()
        {
            TrySpace();
            if(TryToken(TokenType.Identifier) != null || TryToken(TokenType.Native) != null)
            {
                return PrevToken;
            }
            return null;
        }

        public TypeDeclarationSignature TryTypeDeclaration(bool WithAssigment=true)
        {
            if(!BeginStep()) return null;
            Symbol typeIdentifier = TryType();
            if (typeIdentifier == null)
            {
                ResetStep();
                return null;
            }
            TypeDeclarationSignature signatur = new TypeDeclarationSignature();
            signatur.TypeIdentifier = typeIdentifier;
            signatur.TypeGeneric = TryGenericDeclaration();
            signatur.TypeArray = TryArrayDeclaration();
            if((signatur.NameIdentifier = TryIdentifier()) == null)
            {
                ResetStep();
                return null;
            }
            if(WithAssigment)
            {
                if((signatur.Assigment = TryNonSpace(OperationType.Assigment)) == null ||
                   (signatur.AssigmentExpression = TryExpression()) == null
                ){
                    ;
                }
            }
            CommitStep();
            return signatur;
        }

        public GenericDeclarationSignature TryGenericDeclaration()
        {
            if(!BeginStep()) return null;
            Symbol blockBegin;
            if((blockBegin = TryNonSpace(OperationType.Less)) == null)
            {
                ResetStep();
                return null;
            }
            GenericDeclarationSignature signature = new GenericDeclarationSignature();
            signature.BlockBegin = blockBegin;
            while(true)
            {
                GenericElementSignature element = new GenericElementSignature();
                if((element.Identifier = TryIdentifier()) != null)
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
            Symbol blockBegin;
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
                Symbol dimensionToken;
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
            Symbol blockBegin;
            if((blockBegin = TryNonSpace(StructureBegin)) == null)
            {
                return null;
            }
            ParameterDeclarationSignature signature = new ParameterDeclarationSignature();
            signature.BlockBegin = blockBegin;
            TypeDeclarationSignature typeDeclaration;
            while((typeDeclaration = TryTypeDeclaration()) != null)
            {
                ParameterSignature parameter = new ParameterSignature(typeDeclaration);
                signature.Elements.Add(parameter);
                parameter.Seperator = TryNonSpace(StructureType.Seperator);
                if (parameter.Seperator == null)
                {
                    break;
                }
            }
            signature.BlockEnd = TryNonSpace(StructureEnd);
            return signature;
        }

        public ParameterDeclarationSignature TryParameterDefintion(StructureType StructureBegin, StructureType StructureEnd)
        {
            Symbol blockBegin;
            if ((blockBegin = TryNonSpace(StructureBegin)) == null)
            {
                return null;
            }
            ParameterDeclarationSignature signature = new ParameterDeclarationSignature();
            signature.BlockBegin = blockBegin;
            ExpressionSignature expression;
            while ((expression = TryExpression()) != null)
            {
                ParameterSignature parameter = new ParameterSignature(expression);
                signature.Elements.Add(parameter);
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
        public Symbol TypeIdentifier;
        public GenericDeclarationSignature TypeGeneric;
        public ArrayDeclarationSignature TypeArray;
        public Symbol NameIdentifier;
        public Symbol Assigment;
        public ExpressionSignature AssigmentExpression;
        public Symbol Seperator;
     
        public TypeDeclarationSignature() : base(SignatureType.TypeDec)
        { }

        public override string ToString()
        {
            string str = "type(";
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
        public Symbol BlockBegin;
        public ListCollection<GenericElementSignature> ElementList = new ListCollection<GenericElementSignature>();
        public Symbol BlockEnd;

        public GenericDeclarationSignature() : base(SignatureType.GenericDec)
        { }

        public override string ToString()
        {
            return "generic("+ElementList.ToString()+")";
        }
    }

    public class GenericElementSignature : SignatureSymbol
    {
        public Symbol Identifier;
        public Symbol Seperator;
        public GenericDeclarationSignature Generic = new GenericDeclarationSignature();

        public GenericElementSignature() : base(SignatureType.GenericElm)
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
        public Symbol BlockBegin;
        public ListCollection<Symbol> DimensionSymbols = new ListCollection<Symbol>();
        public Symbol BlockEnd;

        public ArrayDeclarationSignature() : base(SignatureType.ArrayDec)
        { }

        public override string ToString()
        {
            return "array(dimesion-size("+(DimensionSymbols.Size+1)+")";
        }
    }

    public class ParameterDeclarationSignature : SignatureSymbol
    {
        public Symbol BlockBegin;
        public ListCollection<ParameterSignature> Elements = new ListCollection<ParameterSignature>();
        public Symbol BlockEnd;

        public ParameterDeclarationSignature() : base(SignatureType.ParamDec)
        { }

        public override string ToString()
        {
            return "parameter_list("+Elements+")";
        }
    }

    public class ParameterSignature : SignatureSymbol
    {
        public TypeDeclarationSignature TypeDeclaration;
        public ExpressionSignature Expression;
        public Symbol Seperator;

        public ParameterSignature(TypeDeclarationSignature TypeDeclaration) : base(SignatureType.Param)
        {
            this.TypeDeclaration = TypeDeclaration;
        }

        public ParameterSignature(ExpressionSignature Expression) : base(SignatureType.Param)
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

    public class PathSignature : SignatureSymbol
    {
        public ListCollection<PathNodeSignature> Elements = new ListCollection<PathNodeSignature>();

        public PathSignature() : base(SignatureType.Path)
        { }

        public override string ToString()
        {
            string str = "path(";
            for (int i = 0; i < Elements.Size; i++)
            {
                str += Elements.Get(i);
            }
            return str + ")";
        }
    }

    public class PathNodeSignature : SignatureSymbol
    {
        public Symbol Identifier;
        public Symbol Seperator;

        public PathNodeSignature() : base(SignatureType.PathNode)
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
