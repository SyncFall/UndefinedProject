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
        public UseSignature TryUse()
        {
            TrySpace();
            if(TryToken(ObjectType.Use) == null)
            {
                return null;
            }
            UseSignature signatur = new UseSignature();
            signatur.Keyword = PrevToken;
            if (!TrySpace() ||
                (signatur.IdentifierPath = TryPath()) == null ||
                (signatur.Complete = TryNonSpace(StructureType.Complete)) == null
            ){
                ;
            }
            return signatur;
        }

        public ScopeSignature TryScope()
        {
            TrySpace();
            if(TryToken(ObjectType.Scope) == null)
            {
                return null;
            }
            ScopeSignature signatur = new ScopeSignature();
            signatur.Keyword = PrevToken;
            if (!TrySpace() ||
                (signatur.IdentifierPath = TryPath()) == null ||
                (signatur.Complete = TryNonSpace(StructureType.Complete)) == null
            ){
                ;
            }
            return signatur;
        }

        public ObjectSignature TryObject()
        {
            TrySpace();
            if(TryToken(NativeType.Object) == null)
            {
                return null;
            }
            ObjectSignature signature = new ObjectSignature();
            signature.Keyword = PrevToken;
            if (!TrySpace() ||
                (signature.Identifier = TryIdentifier()) == null ||
                (signature.BlockBegin = TryNonSpace(StructureType.BlockBegin)) == null
            ){
                return null;
            }
            SignatureSymbol element;
            while(true)
            {
                if((element = TryVariable()) != null ||
                   (element = TryFunction()) != null
                ){
                    signature.ElementList.Add(element);
                    continue;
                }
                break;
            }
            if((signature.BlockEnd = TryNonSpace(StructureType.BlockEnd)) == null){
                ;
            }
            return signature;
        }

        public VariableSignature TryVariable()
        {
            TrySpace();
            if(!Begin()) return null;
            TypeDeclarationSignature typeDeclaration = TryTypeDeclaration();
            if(typeDeclaration == null)
            {
                Reset();
                return null;
            }
            Symbol complete = TryNonSpace(StructureType.Complete);
            if(complete != null)
            {
                VariableSignature variable = new VariableSignature();
                variable.TypeDeclaration = typeDeclaration;
                variable.Complete = complete;
                Commit();
                return variable;
            }
            Reset();
            return null;
        }

        public FunctionSignature TryFunction()
        {
            TrySpace();
            if(!Begin()) return null;
            TypeDeclarationSignature typeDeclaration = TryTypeDeclaration(false);
            Symbol typeIdentifier = null;
            if (typeDeclaration == null)
            {
                if((typeIdentifier = TryType()) == null)
                {
                    Reset();
                    return null;
                }
            }
            if(!Begin())
            {
                Reset();
                return null;
            }
            Symbol enclosing = TryNonSpace(StructureType.ClosingBegin);
            Reset();
            if(enclosing != null)
            {
                FunctionSignature function = new FunctionSignature();
                function.TypeDeclaration = typeDeclaration;
                function.TypeIdentifier = typeIdentifier;
                if ((function.Parameters = TryParameterDeclaration(StructureType.ClosingBegin, StructureType.ClosingEnd)) == null ||
                    (function.Code = TryCode()) == null
                ){
                    ;
                }
                Commit();
                return function;
            }
            Reset();
            return null;
        }

        public PropertySignature TryProperty()
        {
            TrySpace();
            if(!Begin()) return null;
            Symbol propertyType = null;
            if((propertyType = TryToken(ObjectType.Get)) == null && (propertyType = TryToken(ObjectType.Set)) == null)
            {
                Reset();
                return null;
            }
            TypeDeclarationSignature typeDeclaration = TryTypeDeclaration(false);
            if(typeDeclaration == null)
            {
                Reset();
                return null;
            }
            ParameterListSignature parameterDeclaration = TryParameterDeclaration(StructureType.BracketBegin, StructureType.BracketEnd);
            if(!Begin())
            {
                Reset();
                return null;
            }
            Symbol enclosing = TryNonSpace(StructureType.BlockBegin);
            Reset();
            if(enclosing != null)
            {
                PropertySignature property = new PropertySignature(propertyType);
                property.TypeDeclaration = typeDeclaration;
                property.ParameterDeclaration = parameterDeclaration;
                if((property.Code = TryCode()) == null){
                    ;
                }
                Commit();
                return property;
            }
            Reset();
            return null;
        }
    }

    public class UseSignature : SignatureSymbol
    {
        public Symbol Keyword;
        public PathSignature IdentifierPath;
        public Symbol Complete;

        public UseSignature() : base(SignatureType.Use)
        { }

        public override string ToString()
        {
            return "use(" + IdentifierPath + ")\n";
        }
    }

    public class ScopeSignature : SignatureSymbol
    {
        public Symbol Keyword;
        public PathSignature IdentifierPath;
        public Symbol Complete;

        public ScopeSignature() : base(SignatureType.Scope)
        { }

        public override string ToString()
        {
            return "scope(" + IdentifierPath + ")\n";
        }
    }

    public class ObjectSignature : SignatureSymbol
    {
        public Symbol Keyword;
        public Symbol Identifier;
        public Symbol BlockBegin;
        public SignatureList ElementList = new SignatureList();
        public Symbol BlockEnd;

        public ObjectSignature() : base(SignatureType.ObjectDec)
        { }

        public override string ToString()
        {
            return "object(" + Identifier.String + ")\n"+ElementList;
        }
    }

    public class VariableSignature : SignatureSymbol
    {
        public TypeDeclarationSignature TypeDeclaration;
        public Symbol Complete;

        public VariableSignature() : base(SignatureType.VariableDec)
        { }

        public override string ToString()
        {
            return "variable("+TypeDeclaration+")\n";
        }
    }

    public class FunctionSignature : SignatureSymbol
    {
        public Symbol TypeIdentifier;
        public TypeDeclarationSignature TypeDeclaration;
        public ParameterListSignature Parameters;
        public CodeSignature Code;

        public FunctionSignature() : base(SignatureType.FunctionDec)
        { }

        public override string ToString()
        {
            string str = "function(";
            if (TypeIdentifier != null) str += "type:" + TypeIdentifier.String;
            if (TypeDeclaration != null) str += "type_decl:"+ TypeDeclaration;
            str += ", "+Parameters;
            str += ")\n";
            str += Code;
            return str;
        }
    }

    public class PropertySignature : SignatureSymbol
    {
        public TypeDeclarationSignature TypeDeclaration;
        public ParameterListSignature ParameterDeclaration;
        public Symbol CodeType;
        public CodeSignature Code;

        public PropertySignature(Symbol CodeType) : base(SignatureType.PropertyDec)
        {
            this.CodeType = CodeType;
        }

        public override string ToString()
        {
            string str = "property(";
            str += "type:"+CodeType.String+", ";
            str += TypeDeclaration;
            str += ", "+ParameterDeclaration;
            str += ")";
            str += Code;
            return str;
        }
    }
}
