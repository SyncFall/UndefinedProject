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
        public CodeView CodeView;
        public FpsCounter FpsCounter;

        public IntegratorView()
        {
            this.CodeView = new CodeView(this);
            this.FpsCounter = new FpsCounter();
        }

        public void Draw()
        {
            CodeView.Draw();
            FpsCounter.Draw();
        }
    }
}
