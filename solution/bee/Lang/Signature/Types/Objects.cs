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
            if (TryToken(KeywordType.Use) == null)
            {
                return null;
            }
            UseSignature signatur = new UseSignature();
            signatur.Keyword = PrevToken;
            if (!TrySpace() ||
                (signatur.IdentifierPath = TryIdentifierPath()) == null ||
                (signatur.Complete = TryNonSpace(StructureType.Complete)) == null
            ){
                ;
            }
            return signatur;
        }

        public ScopeSignature TryScope()
        {
            if (TryToken(KeywordType.Scope) == null)
            {
                return null;
            }
            ScopeSignature signatur = new ScopeSignature();
            signatur.Keyword = PrevToken;
            if (!TrySpace() ||
                (signatur.IdentifierPath = TryIdentifierPath()) == null ||
                (signatur.Complete = TryNonSpace(StructureType.Complete)) == null
            )
            {
                ;
            }
            return signatur;
        }

        public ObjectSignature TryObject()
        {
            TrySpace();
            if (TryToken(NativeType.Object) == null)
            {
                return null;
            }
            ObjectSignature signature = new ObjectSignature();
            signature.Keyword = PrevToken;
            if (!TrySpace() ||
                (signature.Identifier = TryIdentifier()) == null ||
                (signature.BlockBegin = TryNonSpace(StructureType.BlockBegin)) == null
            ){
                return signature;
            }
            SignatureSymbol element;
            while(true)
            {
                if((element = TryBaseSignature()) == null)
                {
                    break;
                }
                signature.ElementList.Add(element);
            }
            if ((signature.BlockEnd = TryNonSpace(StructureType.BlockEnd)) == null)
            {
                ;
            }
            return signature;
        }

        public MemberSignature TryMember()
        {
            TrySpace();
            if(!BeginStep()) return null;
            TypeDeclarationSignature typeDeclaration = TryTypeDeclaration();
            if (typeDeclaration == null)
            {
                ResetStep();
                return null;
            }
            TokenSymbol complete = TryNonSpace(StructureType.Complete);
            if(complete != null)
            {
                MemberSignature member = new MemberSignature();
                member.TypeDeclaration = typeDeclaration;
                member.Complete = complete;
                CommitStep();
                return member;
            }
            ResetStep();
            return null;
        }

        public MethodSignature TryMethod()
        {
            TrySpace();
            if(!BeginStep()) return null;
            TypeDeclarationSignature typeDeclaration = TryTypeDeclaration(false);
            if(typeDeclaration == null)
            {
                ResetStep();
                return null;
            }
            if(!BeginStep())
            {
                ResetStep();
                return null;
            }
            TokenSymbol enclosing = TryNonSpace(StructureType.ClosingBegin);
            ResetStep();
            if(enclosing != null)
            {
                MethodSignature method = new MethodSignature();
                method.TypeDeclaration = typeDeclaration;
                if ((method.ParameterDeclaration = TryParameterDeclaration(StructureType.ClosingBegin, StructureType.ClosingEnd)) == null ||
                    (method.Code = TryCode()) == null
                ){
                    ;
                }
                CommitStep();
                return method;
            }
            ResetStep();
            return null;
        }

        public PropertySignature TryProperty()
        {
            TrySpace();
            if(!BeginStep()) return null;
            TokenSymbol propertyType = null;
            if((propertyType = TryToken(KeywordType.Get)) == null && (propertyType = TryToken(KeywordType.Set)) == null)
            {
                ResetStep();
                return null;
            }
            TypeDeclarationSignature typeDeclaration = TryTypeDeclaration(false);
            if(typeDeclaration == null)
            {
                ResetStep();
                return null;
            }
            ParameterDeclarationSignature parameterDeclaration = TryParameterDeclaration(StructureType.BracketBegin, StructureType.BracketEnd);
            if(!BeginStep())
            {
                ResetStep();
                return null;
            }
            TokenSymbol enclosing = TryNonSpace(StructureType.BlockBegin);
            ResetStep();
            if(enclosing != null)
            {
                PropertySignature property = new PropertySignature(propertyType);
                property.TypeDeclaration = typeDeclaration;
                property.ParameterDeclaration = parameterDeclaration;
                if ((property.Code = TryCode()) == null)
                {
                    ;
                }
                CommitStep();
                return property;
            }
            ResetStep();
            return null;
        }
    }

    public class UseSignature : SignatureSymbol
    {
        public TokenSymbol Keyword;
        public IdentifierPathSignature IdentifierPath;
        public TokenSymbol Complete;

        public UseSignature() : base(SignatureType.Use)
        { }

        public override string ToString()
        {
            return "use(" + IdentifierPath + ")\n";
        }
    }

    public class ScopeSignature : SignatureSymbol
    {
        public TokenSymbol Keyword;
        public IdentifierPathSignature IdentifierPath;
        public TokenSymbol Complete;

        public ScopeSignature() : base(SignatureType.Scope)
        { }

        public bool IsDefaultScope()
        {
            return (IdentifierPath == null);
        }

        public override string ToString()
        {
            return "scope(" + IdentifierPath + ")\n";
        }
    }

    public class ObjectSignatureList : ListCollection<ObjectSignature>
    {
        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < Size; i++)
            {
                str += Get(i);
            }
            return str;
        }
    }

    public class ObjectSignature : SignatureSymbol
    {
        public TokenSymbol Keyword;
        public TokenSymbol Identifier;
        public TokenSymbol BlockBegin;
        public SignatureList ElementList = new SignatureList();
        public TokenSymbol BlockEnd;

        public ObjectSignature() : base(SignatureType.Object)
        { }

        public override string ToString()
        {
            string str = "object(" + Identifier + ")\n";
            str += ElementList;
            return str;
        }
    }

    public class MemberSignature : SignatureSymbol
    {
        public TypeDeclarationSignature TypeDeclaration;
        public TokenSymbol Complete;

        public MemberSignature() : base(SignatureType.Member)
        { }

        public override string ToString()
        {
            string str = "member(";
            str += TypeDeclaration;
            return str + ")\n";
        }
    }

    public class MethodSignature : SignatureSymbol
    {
        public TypeDeclarationSignature TypeDeclaration;
        public ParameterDeclarationSignature ParameterDeclaration;
        public CodeSignature Code;

        public MethodSignature() : base(SignatureType.Method)
        { }

        public override string ToString()
        {
            string str = "method(";
            str += TypeDeclaration;
            str += ", "+ParameterDeclaration;
            str += ")\n";
            str += Code;
            return str;
        }
    }

    public class PropertySignature : SignatureSymbol
    {
        public TypeDeclarationSignature TypeDeclaration;
        public ParameterDeclarationSignature ParameterDeclaration;
        public TokenSymbol CodeType;
        public CodeSignature Code;

        public PropertySignature(TokenSymbol CodeType) : base(SignatureType.Property)
        {
            this.CodeType = CodeType;
        }

        public override string ToString()
        {
            string str = "property(";
            str += "type:"+CodeType.String+", ";
            str += TypeDeclaration;
            str += ", parameters(" + ParameterDeclaration + ")";
            str += ")\n";
            str += Code;
            return str;
        }
    }
}
