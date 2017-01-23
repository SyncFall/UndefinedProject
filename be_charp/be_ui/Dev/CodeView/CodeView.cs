using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bee.Integrator;
using Bee.UI.Types;
using Bee.UI;
using Bee.Language;

namespace Bee.Integrator
{
    public class CodeView
    {
        public IntegratorView IntegratorView;
        public SourceText SourceText;
        public CodeText CodeText;
        
        public CodeView(IntegratorView IntegratorView)
        {
            this.IntegratorView = IntegratorView;   
        }

        public void LoadSource(SourceText SourceText)
        {
            this.SourceText = SourceText;
            this.CodeText = new CodeText(SourceText);
        }

        public void Draw()
        {
            if (CodeText != null)
            {
                CodeText.Draw();
            }
        }
    }
}
