using feltic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Language
{
    public class SourceSymbolList : ListCollection<SourceSymbol>
    {
        public SourceSymbol GetEqual(SourceText Source)
        {
            for (int i = 0; i < Size; i++)
            {
                if (Get(i).IsEqualFile(Source))
                {
                    return Get(i);
                }
            }
            return null;
        }
    }

    public class SourceSymbol
    {
        public SourceText Source;
        public UseSymbolList UseList = new UseSymbolList();
        public ScopeSymbolList ScopeList = new ScopeSymbolList();

        public SourceSymbol(SourceText SourceText)
        {
            this.Source = SourceText;
        }

        public bool IsEqualFile(SourceText Compare)
        {
            return Source.IsEqualFile(Compare);
        }

        public void Clear()
        {
            UseList.Clear();
            ScopeList.Clear();
        }
    }

    public class UseSymbolList : ListCollection<UseSymbol>
    {
        public bool ContainsEqual(UseSignature signature)
        {
            for (int i = 0; i < Size; i++)
            {
                if (Get(i).IsEqual(signature))
                {
                    return true;
                }
            }
            return false;
        }
    }

    public class UseSymbol
    {
        public SourceSymbol Source;
        public UseSignature Signature;

        public UseSymbol(SourceSymbol SourceSymbol, UseSignature UseSignature)
        {
            this.Source = SourceSymbol;
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
            for (int i = 0; i < Size; i++)
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
        public SourceSymbol Source;
        public ScopeSignature Signature;
        public ObjectSymbolList ObjectList = new ObjectSymbolList();
        public SignatureSymbol VisualElement;

        public ScopeSymbol(SourceSymbol SourceSymbol, ScopeSignature Signature)
        {
            this.Source = SourceSymbol;
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
            for (int i = 0; i < Size; i++)
            {
                if (Get(i).IsEqualIdentifier(Signature))
                {
                    return Get(i);
                }
            }
            return null;
        }
    }

    public class ObjectSymbol
    {
        public ScopeSymbol Scope;
        public ObjectSignature Signature;
        public MemberSymbolList MemberList = new MemberSymbolList();
        public MethodSymbolList MethodList = new MethodSymbolList();

        public ObjectSymbol(ScopeSymbol ScopeSymbol, ObjectSignature Signature)
        {
            this.Scope = ScopeSymbol;
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
            for (int i = 0; i < Size; i++)
            {
                if (this[i].IsEqualIdentifier(Signature))
                {
                    return Get(i);
                }
            }
            return null;
        }
    }

    public class MemberSymbol
    {
        public ObjectSymbol Object;
        public MemberSignature Signature;

        public MemberSymbol(ObjectSymbol ObjectSymbol, MemberSignature Signature)
        {
            this.Object = ObjectSymbol;
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
            for (int i = 0; i < Size; i++)
            {
                if (Get(i).IsEqualIdentifier(Signature))
                {
                    return Get(i);
                }
            }
            return null;
        }
    }

    public class MethodSymbol
    {
        public ObjectSymbol Object;
        public MethodSignature Signature;
        public CodeSymbol Code;

        public MethodSymbol(ObjectSymbol ObjectSymbol, MethodSignature Signature)
        {
            this.Object = ObjectSymbol;
            this.Signature = Signature;
        }

        public bool IsEqualIdentifier(MethodSignature Compare)
        {
            return false;
        }
    }

    public class CodeSymbol
    {
        public MethodSymbol Method;
        public CodeSignature Signature;

        public CodeSymbol(MethodSymbol MethodSymbol, CodeSignature Signature)
        {
            this.Method = MethodSymbol;
            this.Signature = Signature;
        }
    }

    public class VisualElementSymbol
    {
        public StructedBlockSignature Signature;

        public VisualElementSymbol(StructedBlockSignature Signature)
        {
            this.Signature = Signature;
        }
    }
}
