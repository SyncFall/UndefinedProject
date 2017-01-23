using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bee.UI;
using Bee.Language;

namespace Bee.Integrator
{
    public class IntegratorView
    {
        public WindowType Window;
        public CodeView CodeView;
        public FpsCounter FpsCounter;

        public IntegratorView(WindowType Window)
        {
            this.Window = Window;
            this.CodeView = new CodeView(this);
            this.FpsCounter = new FpsCounter();

            SourceText source = SourceText.FromFile(@"D:\dev\UndefinedProject\be-output\test.bee-source");

            this.CodeView.LoadSource(source);
        }

        public void Draw()
        {
            CodeView.Draw();
            FpsCounter.Draw();
        }
    }
}
