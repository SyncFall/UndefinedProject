using Bee.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Language
{
    public class Registry
    {
        public EntryList EntryList = new EntryList();

        public Registry()
        { }

        public void AddSourceList(SourceList SourceList)
        {
            for (int i = 0; i < SourceList.Size; i++)
            {
                SourceText source = SourceList.Get(i);
                Entry entry = EntryList.GetExist(source);
                if(entry == null)
                {
                    entry = new Entry(source);
                    EntryList.Add(entry);
                }
                entry.ParseSource();
            }
            Validate();
        }

        public void UpdateSource(SourceText SourceText)
        {
            Entry entry = EntryList.GetExist(SourceText);
            entry.ParseSource();
            Validate();
        }

        public void Validate()
        {
            Validator validator = new Validator(this);
            for (int i=0; i<EntryList.Size; i++)
            {
                Entry entry = EntryList[i];
                validator.ValidateSource(entry.SourceSymbol);
            }
        }

        public ObjectSymbol GetObjectSymbol(string ObjectName)
        {
            for(int i=0; i<EntryList.Size; i++)
            {
                SourceSymbol sourceSymbol = EntryList.Get(i).SourceSymbol;
                for(int j=0; j<sourceSymbol.ScopeList.Size; j++)
                {
                    ScopeSymbol scopeSymbol = sourceSymbol.ScopeList.Get(j);
                    for(int k=0; k<scopeSymbol.ObjectList.Size; k++)
                    {
                        ObjectSymbol objectSymbol = scopeSymbol.ObjectList.Get(k);
                        if(objectSymbol.Signature.Identifier.IdentifiereToken.String == ObjectName)
                        {
                            return objectSymbol;
                        }
                    }
                }
            }
            return null;
        }
    }

    public class EntryList : ListCollection<Entry>
    {
        public Entry GetExist(SourceText SourceText)
        {
            for(int i=0; i<Size; i++)
            {
                if(Get(i).SourceText.IsEqualFile(SourceText))
                {
                    return Get(i);
                }
            }
            return null;
        }
    }

    public class Entry
    {
        public SourceSymbol SourceSymbol;
        public SourceText SourceText;
        public TokenContainer TokenContainer;
        public SignatureContainer SignatureContainer;
        public SymbolContainer SymbolContainer;
        
        public Entry(SourceText SourceText)
        {
            this.SourceText = SourceText;
            this.TokenContainer = new TokenContainer();
            this.SignatureContainer = new SignatureContainer();
            this.SymbolContainer = new SymbolContainer();
        }

        public void ParseSource()
        {
            this.TokenContainer.SetSource(this.SourceText);
            this.SignatureContainer.SetContainer(this.TokenContainer);
            this.SymbolContainer.SetContainer(this.SignatureContainer);
            this.SourceSymbol = this.SymbolContainer.SourceSymbol;
        }
    }
}
