using Bee.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Integrator
{
    public enum CodeHistoryType
    {

    }

    public class CodeHistory
    {
        public CodeText CodeText;
        public ListCollection<CodeHistoryEntry> History = new ListCollection<CodeHistoryEntry>();
        public int Position;

        public CodeHistory(CodeText CodeText)
        {
            this.CodeText = CodeText;
            this.Clear();
        }

        public void Clear()
        {
            History.Clear();
            Position = -1;
        }

        public void AddHistory(CodeHistoryEntry Entry)
        {
            if(Position != -1)
            {
                while(Position+1 < History.Size)
                {
                    History.RemoveAt(Position+1);
                }
            }
            History.Add(Entry);
            Position = (History.Size);
        }

        public CodeHistoryEntry UndoHistory()
        {
            if(History.Size==0 || Position < 1)
            {
                return null;
            }
            return History.Get(--Position);
        }

        public CodeHistoryEntry RedoHistory()
        {
            if (History.Size==0 || Position+1 >= History.Size)
            {
                return null;
            }
            return History.Get(++Position);
        }
    }

    public class CodeHistoryEntry
    {
        public string CodeText;
        public CodeCursor CodeCursor;
        public CodeSelection CodeSelection;

        public CodeHistoryEntry(string CodeText, CodeCursor CodeCursor, CodeSelection CodeSelection)
        {
            this.CodeText = CodeText;
            this.CodeCursor = CodeCursor.Clone();
            this.CodeSelection = CodeSelection.Clone();
        }
    }
}
