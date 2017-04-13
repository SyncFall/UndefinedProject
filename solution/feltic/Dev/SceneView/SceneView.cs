using feltic.Language;
using feltic.UI;
using Scope;
using feltic.UI.Types;
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
        GlyphContainer GlyphContainer = new GlyphContainer(new Font("DroidSansMono.ttf"));
        public static VisualElement Root = new VisualElement(VisualType.Block);
        public static VisualElement Code = null;
        public CodeText CodeText;

        public SceneView()
        {
            Editor editor = new Editor();
            AddCode(editor);
            Root.add(editor);
        }

        public void AddCode(Editor editor)
        {
            editor.Workspace.Nodes[0].add(new VisualTextElement(File.ReadAllText("./Compose/four.src"), CodeColor.String));
        }
        
        public void Draw()
        {
            if(Root != null)
            {
                //CodeText.CodeSelection.Draw();
                Position offset = new Position(20, 20);
                Root.CalculateSizeAndPosition(offset);
                Root.Draw(offset.x, offset.y);
                //CodeText.CodeCursor.Draw();
            }
        }
    }
}
