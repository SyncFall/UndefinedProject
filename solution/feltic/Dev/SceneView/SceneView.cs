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
            editor.content.content.Offset = new Position(0, 125);
            //VisualText txt = new VisualText("asdfasdfasdf", CodeColor.String);
            //txt.Offset = new Position(20);
            //txt.Clip = new Size(50, 20);
            //editor.Workspace.content.content.add(txt);
            //VisualImage img = new VisualImage("cut.png");
            //img.Room = new Room(Way.Try("100px"), null);
            //img.Offset = new Position(25, 25);
            //img.Clip = new Size(75, 75);
            // editor.Workspace.content.content.add(img);
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
