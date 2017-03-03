﻿using Bee.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Language
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
            signatur.Keyword = new KeywordSignature(PrevToken);
            if (!TrySpace() ||
                (signatur.IdentifierPath = TryIdentifierPath()) == null ||
                (signatur.Complete = TrySeperator(StructureType.Complete)) == null
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
            signatur.Keyword = new KeywordSignature(PrevToken);
            if (!TrySpace() ||
                (signatur.IdentifierPath = TryIdentifierPath()) == null ||
                (signatur.Complete = TrySeperator(StructureType.Complete)) == null
            )
            {
                ;
            }
            return signatur;
        }

        public ObjectSignatureList TryObjectList()
        {
            ObjectSignatureList list = new ObjectSignatureList();
            ObjectSignature signatur;
            while ((signatur = TryObject()) != null)
            {
                list.Add(signatur);
            }
            return list;
        }

        public ObjectSignature TryObject()
        {
            TrySpace();
            if (TryToken(NativeType.Object) == null)
            {
                return null;
            }
            ObjectSignature signatur = new ObjectSignature();
            signatur.Keyword = new KeywordSignature(PrevToken);
            if (!TrySpace() ||
                (signatur.Identifier = TryIdentifier()) == null ||
                (signatur.BlockBegin = TryBlock(StructureType.BlockBegin)) == null
            ){
                return signatur;
            }
            SignatureSymbol objectElement;
            while(true)
            {
                MemberSignature member = TryMember();
                if(member != null)
                {
                    signatur.Members.Add(member);
                    continue;
                }
                MethodSignature method = TryMethod();
                if(method != null)
                {
                    signatur.Methods.Add(method);
                    continue;
                }
                PropertySignature property = TryProperty();
                if(property != null)
                {
                    signatur.Properties.Add(property);
                }
                //
                break;
            }
            if ((signatur.BlockEnd = TryBlock(StructureType.BlockEnd)) == null)
            {
                ;
            }
            return signatur;
        }

        public MemberSignature TryMember()
        {
            TrySpace();
            TypeDeclarationSignature typeDeclaration = TryTypeDeclaration();
            if (typeDeclaration == null)
            {
                return null;
            }
            SeperatorSignature complete = TrySeperator(StructureType.Complete);
            if (complete != null)
            {
                MemberSignature member = new MemberSignature();
                member.TypeDeclaration = typeDeclaration;
                member.Complete = complete;
                return member;
            }
            return null;
        }

        public MethodSignature TryMethod()
        {
            TrySpace();
            TypeDeclarationSignature typeDeclaration = TryTypeDeclaration(false);
            if (typeDeclaration == null)
            {
                return null;
            }
            BeginStep();
            SeperatorSignature enclosing = TrySeperator(StructureType.ClosingBegin);
            ResetStep();
            if (enclosing != null)
            {
                MethodSignature method = new MethodSignature();
                method.TypeDeclaration = typeDeclaration;
                if ((method.ParameterDeclaration = TryParameterDeclaration()) == null ||
                    (method.Code = TryCode()) == null
                ){
                    ;
                }
                return method;
            }
            return null;
        }


        public PropertySignature TryProperty()
        {
            TrySpace();
            TokenSymbol propertyType = null;
            if((propertyType = TryToken(KeywordType.Get)) == null && (propertyType = TryToken(KeywordType.Set)) == null)
            {
                return null;
            }
            TypeDeclarationSignature typeDeclaration = TryTypeDeclaration(false);
            if (typeDeclaration == null)
            {
                 return null;
            }
            BeginStep();
            SeperatorSignature enclosing = TrySeperator(StructureType.BlockBegin);
            ResetStep();
            if (enclosing != null)
            {
                PropertySignature property = new PropertySignature(propertyType);
                property.TypeDeclaration = typeDeclaration;
                if ((property.Code = TryCode()) == null)
                {
                    ;
                }
                return property;
            }
            return null;
        }
    }

    public class UseSignature : SignatureSymbol
    {
        public KeywordSignature Keyword;
        public IdentifierPathSignature IdentifierPath;
        public SeperatorSignature Complete;

        public UseSignature() : base(SignatureType.Use)
        { }

        public override string ToString()
        {
            return "use(" + IdentifierPath + ")";
        }
    }

    public class ScopeSignature : SignatureSymbol
    {
        public KeywordSignature Keyword;
        public IdentifierPathSignature IdentifierPath;
        public SeperatorSignature Complete;

        public ScopeSignature() : base(SignatureType.Scope)
        { }

        public override string ToString()
        {
            return "scope(" + IdentifierPath + ")";
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
        public KeywordSignature Keyword;
        public IdentifierSignature Identifier;
        public BlockSignature BlockBegin;
        public MemberSignatureList Members = new MemberSignatureList();
        public MethodSignatureList Methods = new MethodSignatureList();
        public PropertySignatureList Properties = new PropertySignatureList();
        public BlockSignature BlockEnd;

        public ObjectSignature() : base(SignatureType.Object)
        { }

        public override string ToString()
        {
            string str = "object(" + Identifier + ")\n";
            str += Members;
            str += Methods;
            str += Properties;
            return str;
        }
    }

    public class MemberSignatureList : ListCollection<MemberSignature>
    {
        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < Size; i++)
            {
                str += Get(i) + "\n";
            }
            return str;
        }
    }

    public class MemberSignature : SignatureSymbol
    {
        public TypeDeclarationSignature TypeDeclaration;
        public SeperatorSignature Complete;

        public MemberSignature() : base(SignatureType.Member)
        { }

        public override string ToString()
        {
            string str = "member(";
            str += TypeDeclaration;
            return str + ")";
        }
    }

    public class MethodSignatureList : ListCollection<MethodSignature>
    {
        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < Size; i++)
            {
                str += Get(i) + "\n";
            }
            return str;
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
            str += ", parameters(" + ParameterDeclaration + ")";
            str += ")\n";
            str += Code;
            return str;
        }
    }

    public class PropertySignatureList : ListCollection<PropertySignature>
    {
        public override string ToString()
        {
            string str = "";
            for (int i = 0; i < Size; i++)
            {
                str += Get(i) + "\n";
            }
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
