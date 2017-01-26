using Bee.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Language
{
    public class SymbolContainer
    {
        public SymbolContainer()
        { }

        public void SetContainer(SignatureContainer SignatureContainer)
        {
            SourceSymbol sourceSymbol = new SourceSymbol();
            for(int i=0; i<SignatureContainer.SignatureNodes.Size(); i++)
            {
                SignatureSymbol signature = SignatureContainer.SignatureNodes.Get(i).Signature;
                if(signature.Type == SignatureType.Use)
                {
                    TryUse(sourceSymbol, signature as UseSignature);
                }
                else if(signature.Type == SignatureType.Scope)
                {
                    TryScope(sourceSymbol, signature as ScopeSignature);
                }
                else
                {
                    ;
                }
            }
        }

        public void TryUse(SourceSymbol SourceSymbol, UseSignature UseSignature)
        {
            if(UseSignature.IdentifierPath == null)
            {
                ;
            }
            else if(SourceSymbol.UseList.ContainsEqual(UseSignature))
            {
                ;
            }
            else
            {
                SourceSymbol.UseList.Add(new UseSymbol(UseSignature));
            }
        }

        public void TryScope(SourceSymbol SourceSymbol, ScopeSignature ScopeSignature)
        {
            if(ScopeSignature.IdentifierPath == null)
            {
                ;
            }
            else
            {
                ScopeSymbol scopeSymbol = SourceSymbol.ScopeList.GetEqual(ScopeSignature);
                if(scopeSymbol == null)
                {
                    scopeSymbol = new ScopeSymbol(ScopeSignature);
                    SourceSymbol.ScopeList.Add(scopeSymbol);
                }
                for(int i=0; i<ScopeSignature.ObjectList.Size(); i++)
                {
                    ObjectSignature objectSignatur = ScopeSignature.ObjectList.Get(i);
                    TryObject(scopeSymbol, objectSignatur);
                }
            }
        }

        public void TryObject(ScopeSymbol ScopeSymbol, ObjectSignature ObjectSignature)
        {
            if(ObjectSignature.Identifier == null)
            {
                ;
            }
            else
            {
                ObjectSymbol objectSymbol = ScopeSymbol.ObjectList.GetEqualByIdentifier(ObjectSignature);
                if (objectSymbol == null)
                {
                    objectSymbol = new ObjectSymbol(ObjectSignature);
                    ScopeSymbol.ObjectList.Add(objectSymbol);
                }
                else
                {
                    ;
                }
                for(int i=0; i<ObjectSignature.Members.Size(); i++)
                {
                    MemberSignature memberSignature = ObjectSignature.Members.Get(i);
                    TryMember(objectSymbol, memberSignature);
                }
                for(int i=0; i<ObjectSignature.Methods.Size(); i++)
                {
                    MethodSignature methodSignature = ObjectSignature.Methods.Get(i);
                    TryMethod(objectSymbol, methodSignature);
                }
            }
        }

        public void TryMember(ObjectSymbol ObjectSymbol, MemberSignature MemberSignature)
        {
            if(MemberSignature.TypeDeclaration == null)
            {
                ;
            }
            else
            {
                MemberSymbol memberSymbol = ObjectSymbol.MemberList.GetEqualByIdentifier(MemberSignature);
                if(memberSymbol == null)
                {
                    memberSymbol = new MemberSymbol(MemberSignature);
                    ObjectSymbol.MemberList.Add(memberSymbol);
                }
                else
                {
                    ;
                }
            }
        }

        public void TryMethod(ObjectSymbol ObjectSymbol, MethodSignature MethodSignature)
        {
            if(MethodSignature.TypeDeclaration == null)
            {
                ;
            }
            else
            {
                MethodSymbol methodSymbol = ObjectSymbol.MethodList.GetEqualByIdentifier(MethodSignature);
                if (methodSymbol == null)
                {
                    methodSymbol = new MethodSymbol(MethodSignature);
                    ObjectSymbol.MethodList.Add(methodSymbol);
                }
                else
                {
                    ;
                }
                TryCode(methodSymbol, MethodSignature.Code);
            }
        }

        public void TryCode(MethodSymbol MethodSymbol, CodeSignature CodeSignature)
        {
            if(CodeSignature == null)
            {
                ;
            }
            else
            {

            }
        }
    }

    public class SourceSymbolList : ListCollection<SourceSymbol>
    { }

    public class SourceSymbol
    {
        public UseSymbolList UseList = new UseSymbolList();
        public ScopeSymbolList ScopeList = new ScopeSymbolList();
    }

    public class UseSymbolList : ListCollection<UseSymbol>
    {
        public bool ContainsEqual(UseSignature signature)
        {
            for(int i=0; i<Size(); i++)
            {
                if(Get(i).IsEqual(signature))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class UseSymbol
    {
        public UseSignature Signature;

        public UseSymbol(UseSignature UseSignature)
        {
            this.Signature = UseSignature;
        }

        public bool IsEqual(UseSignature compare)
        {
            return false;
        }
    }

    public class ScopeSymbolList : ListCollection<ScopeSymbol>
    {
        public ScopeSymbol GetEqual(ScopeSignature signature)
        {
            for (int i = 0; i < Size(); i++)
            {
                if (Get(i).IsEqual(signature))
                {
                    return Get(i);
                }
            }
            return null;
        }
    }

    public class ScopeSymbol
    {
        public ScopeSignature Signature;
        public ObjectSymbolList ObjectList = new ObjectSymbolList();

        public ScopeSymbol(ScopeSignature Signature)
        {
            this.Signature = Signature;
        }

        public bool IsEqual(ScopeSignature compare)
        {
            return false;
        }
    }

    public class ObjectSymbolList : ListCollection<ObjectSymbol>
    {
        public ObjectSymbol GetEqualByIdentifier(ObjectSignature Signature)
        {
            for(int i=0; i<Size(); i++)
            {
                if(Get(i).IsEqualIdentifier(Signature))
                {
                    return Get(i);
                }
            }
            return null;
        }
    }

    public class ObjectSymbol
    {
        public ObjectSignature Signature;
        public MemberSymbolList MemberList = new MemberSymbolList();
        public MethodSymbolList MethodList = new MethodSymbolList();

        public ObjectSymbol(ObjectSignature Signature)
        {
            this.Signature = Signature;
        }

        public bool IsEqualIdentifier(ObjectSignature Compare)
        {
            return false;
        }
    }

    public class MemberSymbolList : ListCollection<MemberSymbol>
    {
        public MemberSymbol GetEqualByIdentifier(MemberSignature Signature)
        {
            for(int i=0; i<Size(); i++)
            {
                if(Get(i).IsEqualIdentifier(Signature))
                {
                    return Get(i);
                }
            }
            return null;
        }
    }

    public class MemberSymbol
    {
        public MemberSignature Signature;

        public MemberSymbol(MemberSignature Signature)
        {
            this.Signature = Signature;
        }

        public bool IsEqualIdentifier(MemberSignature Compare)
        {
            return false;
        }
    }

    public class MethodSymbolList : ListCollection<MethodSymbol>
    {
        public MethodSymbol GetEqualByIdentifier(MethodSignature Signature)
        {
            for(int i=0; i<Size(); i++)
            {
                if(Get(i).IsEqualIdentifier(Signature))
                {
                    return Get(i);
                }
            }
            return null;
        }
    }

    public class MethodSymbol
    {
        public MethodSignature Signature;

        public MethodSymbol(MethodSignature Signature)
        {
            this.Signature = Signature;
        }

        public bool IsEqualIdentifier(MethodSignature Compare)
        {
            return false;
        }
    }
}
