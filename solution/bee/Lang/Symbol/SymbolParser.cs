using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Language
{
    public class SymbolParser
    {
        public SymbolParser()
        { }

        public SourceSymbol TrySymbol(SignatureContainer SignatureContainer)
        {
            SourceSymbol sourceSymbol = new SourceSymbol(SignatureContainer.SourceText);
            for (int i = 0; i < SignatureContainer.SignatureNodes.Size(); i++)
            {
                SignatureSymbol signature = SignatureContainer.SignatureNodes.Get(i).Signature;
                if (signature.Type == SignatureType.Use)
                {
                    TryUse(sourceSymbol, signature as UseSignature);
                }
                else if (signature.Type == SignatureType.Scope)
                {
                    TryScope(sourceSymbol, signature as ScopeSignature);
                }
                else
                {
                    ;
                }
            }
            return sourceSymbol;
        }

        public void TryUse(SourceSymbol Source, UseSignature Signature)
        {
            if (Signature.IdentifierPath == null)
            {
                ;
            }
            else if (Source.UseList.ContainsEqual(Signature))
            {
                ;
            }
            else
            {
                Source.UseList.Add(new UseSymbol(Source, Signature));
            }
        }

        public void TryScope(SourceSymbol Source, ScopeSignature Signature)
        {
            if (Signature.IdentifierPath == null)
            {
                ;
            }
            else
            {
                ScopeSymbol scopeSymbol = Source.ScopeList.GetEqual(Signature);
                if (scopeSymbol == null)
                {
                    scopeSymbol = new ScopeSymbol(Source, Signature);
                    Source.ScopeList.Add(scopeSymbol);
                }
                for (int i = 0; i < Signature.ObjectList.Size(); i++)
                {
                    ObjectSignature objectSignatur = Signature.ObjectList.Get(i);
                    TryObject(scopeSymbol, objectSignatur);
                }
            }
        }

        public void TryObject(ScopeSymbol Scope, ObjectSignature Signature)
        {
            if (Signature.Identifier == null)
            {
                ;
            }
            else
            {
                ObjectSymbol objectSymbol = Scope.ObjectList.GetEqualByIdentifier(Signature);
                if (objectSymbol == null)
                {
                    objectSymbol = new ObjectSymbol(Scope, Signature);
                    Scope.ObjectList.Add(objectSymbol);
                }
                else
                {
                    ;
                }
                for (int i = 0; i < Signature.Members.Size(); i++)
                {
                    MemberSignature memberSignature = Signature.Members.Get(i);
                    TryMember(objectSymbol, memberSignature);
                }
                for (int i = 0; i < Signature.Methods.Size(); i++)
                {
                    MethodSignature methodSignature = Signature.Methods.Get(i);
                    TryMethod(objectSymbol, methodSignature);
                }
            }
        }

        public void TryMember(ObjectSymbol Object, MemberSignature Signature)
        {
            if (Signature.TypeDeclaration == null)
            {
                ;
            }
            else
            {
                MemberSymbol memberSymbol = Object.MemberList.GetEqualByIdentifier(Signature);
                if (memberSymbol == null)
                {
                    memberSymbol = new MemberSymbol(Object, Signature);
                    Object.MemberList.Add(memberSymbol);
                }
                else
                {
                    ;
                }
            }
        }

        public void TryMethod(ObjectSymbol Object, MethodSignature Signature)
        {
            if (Signature.TypeDeclaration == null)
            {
                ;
            }
            else
            {
                MethodSymbol methodSymbol = Object.MethodList.GetEqualByIdentifier(Signature);
                if (methodSymbol == null)
                {
                    methodSymbol = new MethodSymbol(Object, Signature);
                    Object.MethodList.Add(methodSymbol);
                }
                else
                {
                    ;
                }
                TryCode(methodSymbol, Signature.Code);
            }
        }

        public void TryCode(MethodSymbol Method, CodeSignature Signature)
        {
            if (Signature == null)
            {
                ;
            }
            else
            {
                Method.Code = new CodeSymbol(Method, Signature);
            }
        }
    }
}
