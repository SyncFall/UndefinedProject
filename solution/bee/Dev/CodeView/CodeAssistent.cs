using feltic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Integrator
{
    public class CodeAssistent
    {
        public CodeText CodeText;
        public AssistentEntryContainer EntryContainer = new AssistentEntryContainer();

        public CodeAssistent(CodeText CodeText)
        {
            this.CodeText = CodeText;        
        }
    }

    public class AssistentEntryContainer
    {
        public ListCollection<AssisentEntry> List = new ListCollection<AssisentEntry>();
    }

    public class AssisentEntry
    {
        
    }
}
