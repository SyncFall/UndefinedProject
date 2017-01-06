using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Be.Runtime.Types;
using Be.UI;

namespace Be.Integrator
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

            SourceFile source = new SourceFile();
            source.LoadFile(@"D:\dev\UndefinedProject\be-output\AA.be-src");

            this.CodeView.LoadSource(source);
        }

        public void Draw()
        {
            CodeView.Draw();
            FpsCounter.Draw();
        }
    }
}
