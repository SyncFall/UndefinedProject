using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Feltic.UI;
using Feltic.Language;

namespace Feltic.Integrator
{
    public class IntegratorView
    {
        public CodeView CodeView;
        public VisualView VisualView;
        public FpsCounter FpsCounter;

        public IntegratorView()
        {
            this.CodeView = new CodeView(this);
            //this.VisualView = new VisualView();
            this.FpsCounter = new FpsCounter();
        }

        public void Draw()
        {
            CodeView.Draw();
            //VisualView.Draw();
            FpsCounter.Draw();
        }
    }
}
