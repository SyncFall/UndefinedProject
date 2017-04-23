using feltic.Language;
using feltic.Visual;
using Scope;
using feltic.Visual.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace feltic.Integrator
{

    public class SceneView
    {
        VisualElement Root = new VisualElement(VisualType.Block);
        CodeText CodeText;

        public SceneView()
        {
            Editor editor = new Editor();
            AddCode(editor);
            Root.add(editor);
        }

        public void AddCode(Editor editor)
        {
           /// editor.content.content.Offset = new Position(25, 50);
           /// editor.content.content.Clip = new Size(50, 100);
        }
        
        public void Draw()
        {
            if(Root != null)
            {
                //CodeText.CodeSelection.Draw();
                Root.Metrics(new Position(20, 20));
                Root.Draw();
                //CodeText.CodeCursor.Draw();
            }
        }
    }
}
