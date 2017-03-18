using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public class SymbolParser
    {
        public SymbolParser()
        { }

        public SourceSymbol TrySymbol(SignatureContainer SignatureContainer)
        {
            SourceSymbol sourceSymbol = new SourceSymbol(SignatureContainer.SourceText);
            ScopeSymbol lastScopeSymbol = new ScopeSymbol(sourceSymbol, new ScopeSignature()); // default-scope
            for (int i = 0; i < SignatureContainer.SignatureNodes.Size; i++)
            {
                SignatureSymbol signature = SignatureContainer.SignatureNodes[i].Signature;
                if(signature.Type == SignatureType.Use)
                {
                    TryUse(sourceSymbol, signature as UseSignature);
                }
                else if(signature.Type == SignatureType.Scope)
                {
                    lastScopeSymbol = TryScope(sourceSymbol, signature as ScopeSignature);
                }
                else if(signature.Type == SignatureType.Object)
                {
                    TryObject(lastScopeSymbol, signature as ObjectSignature);
                }
                else if(signature.Type == SignatureType.Statement)
                {
                    lastScopeSymbol.VisualElement = signature;
                    sourceSymbol.ScopeList.Add(lastScopeSymbol);
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

        public ScopeSymbol TryScope(SourceSymbol Source, ScopeSignature Signature)
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
                return scopeSymbol;
            }
            return null;
        }

        public void TryObject(ScopeSymbol Scope, ObjectSignature Signature)
        {
            if(Scope == null)
            {
                throw new Exception("try-object, unknown scope");
            }
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
                for (int i = 0; i < Signature.ElementList.Size; i++)
                {
                    SignatureSymbol element = Signature.ElementList[i];
                    if(element.Type == SignatureType.Member)
                    {
                        TryMember(objectSymbol, element as MemberSignature);
                    }
                    else if(element.Type == SignatureType.Method)
                    {
                        TryMethod(objectSymbol, element as MethodSignature);
                    }
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

        public void TryVisualElement(ScopeSymbol Scope, StructedBlockSignature Signature)
        {
            Scope.VisualElement = Signature;
        }
    }
}
