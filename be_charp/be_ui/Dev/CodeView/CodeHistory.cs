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
        public ListCollection<CodeHistoryStep> History = new ListCollection<CodeHistoryStep>();
        public int HistoryPointer;

        public CodeHistory(CodeText CodeText)
        {
            this.CodeText = CodeText;
            this.Clear();
        }

        public void Clear()
        {
            History.Clear();
            HistoryPointer = -1;
        }

        public void AddStep(CodeHistoryStep StepEntry)
        {
            History.Add(StepEntry);
            HistoryPointer++;
        }

        public bool UndoStep()
        {
            if(HistoryPointer < 0)
            {
                return false;
            }
            History.Get(HistoryPointer).DoUndo();
            HistoryPointer--;
            return true;
        }

        public bool RedoStep()
        {
            if(HistoryPointer >= History.Size()-1)
            {
                return false;
            }
            HistoryPointer++;
            History.Get(HistoryPointer).DoRedo();
            return true;
        }
    }

    public abstract class CodeHistoryStep
    {
        public CodeHistoryType Type;

        public CodeHistoryStep(CodeHistoryType Type)
        {
            this.Type = Type;
        }

        public abstract void DoUndo();
        public abstract void DoRedo();
    }
}
