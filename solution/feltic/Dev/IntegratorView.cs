using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using feltic.Visual;
using feltic.Language;

namespace feltic.Integrator
{
    public class IntegratorView
    {
        public CodeView CodeView;
        public VisualView VisualView;
        public SceneView SceneView;
        public FpsCounter FpsCounter;

        public IntegratorView()
        {
            //this.CodeView = new CodeView(this);
            //this.VisualView = new VisualView();
            this.SceneView = new SceneView();
            this.FpsCounter = new FpsCounter();
        }

        public void Draw()
        {
            //CodeView.Draw();
            //VisualView.Draw();
            SceneView.Draw();
            FpsCounter.Draw();
        }
    }
}
