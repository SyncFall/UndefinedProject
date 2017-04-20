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
            editor.content.content.Offset = new Position(0, 0);
            editor.content.content.Clip = new Size(0, 0);
            editor.content.content.Padding = new Spacing(20, 20, 20, 10);
            editor.content.content.Nodes[0].Margin = new Spacing(20, 10);
            editor.content.content.Nodes[1].Margin = new Spacing(40, 20, 40, 20);
            editor.content.content.Nodes[2].Margin = new Spacing(10, 10, 10, 10);
            editor.content.content.Nodes[2].Nodes[0].Padding = new Spacing(20, 20, 20, 20);
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
