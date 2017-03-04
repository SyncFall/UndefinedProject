using Feltic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feltic.Language
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
                RegistryEntry entry = EntryList.GetExist(source);
                if(entry == null)
                {
                    entry = new RegistryEntry(source);
                    EntryList.Add(entry);
                }
                entry.ParseSource();
            }
            Validate();
        }

        public void UpdateSource(SourceText SourceText)
        {
            RegistryEntry entry = EntryList.GetExist(SourceText);
            entry.ParseSource();
            Validate();
        }

        public void Validate()
        {
            Validator validator = new Validator(this);
            for (int i=0; i<EntryList.Size; i++)
            {
                RegistryEntry entry = EntryList[i];
                validator.ValidateSource(entry.SourceSymbol);
            }
        }

        public void WriteToTarget(string Filepath)
        {
            new TargetWriter(Filepath).Convert(EntryList[0].SourceSymbol);
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
                        if(objectSymbol.Signature.Identifier.String == ObjectName)
                        {
                            return objectSymbol;
                        }
                    }
                }
            }
            return null;
        }
    }

    public class EntryList : ListCollection<RegistryEntry>
    {
        public RegistryEntry GetExist(SourceText SourceText)
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

    public class RegistryEntry
    {
        public SourceSymbol SourceSymbol;
        public SourceText SourceText;
        public TokenContainer TokenContainer;
        public SignatureContainer SignatureContainer;
        public SymbolContainer SymbolContainer;
        
        public RegistryEntry(SourceText SourceText)
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
