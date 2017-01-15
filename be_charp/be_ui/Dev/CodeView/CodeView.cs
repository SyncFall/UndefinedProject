using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Be.Integrator;
using Be.Runtime.Types;
using Be.UI.Types;
using Be.UI;

namespace Be.Integrator
{
    public class CodeView
    {
        public IntegratorView IntegratorView;
        public SourceFile SourceFile;
        public CodeText CodeText;
        
        public CodeView(IntegratorView IntegratorView)
        {
            this.IntegratorView = IntegratorView;   
        }

        public void LoadSource(SourceFile SourceFile)
        {
            this.SourceFile = SourceFile;
            this.CodeText = new CodeText(this.SourceFile);
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
