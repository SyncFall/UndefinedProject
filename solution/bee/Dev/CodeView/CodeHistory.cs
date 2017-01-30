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
        public int HistoryPosition;

        public CodeHistory(CodeText CodeText)
        {
            this.CodeText = CodeText;
            this.Clear();
        }

        public void Clear()
        {
            History.Clear();
            HistoryPosition = -1;
        }

        public void AddStep(CodeHistoryStep StepEntry)
        {
            History.Add(StepEntry);
            HistoryPosition++;
        }

        public bool UndoStep()
        {
            if(HistoryPosition < 0)
            {
                return false;
            }
            History.Get(HistoryPosition).DoUndo();
            HistoryPosition--;
            return true;
        }

        public bool RedoStep()
        {
            if(HistoryPosition >= History.Size()-1)
            {
                return false;
            }
            HistoryPosition++;
            History.Get(HistoryPosition).DoRedo();
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
