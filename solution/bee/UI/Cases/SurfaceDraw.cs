using Bee.UI.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Bee.Library;

namespace Bee.UI
{
    public class SurfaceDraw
    {
        public FpsCounter FpsCounter;
        public Text Text;
        public Surface Surface;

        public SurfaceDraw()
        {
            FpsCounter = new FpsCounter();
            Text = new Text("fuckyou\nTOO!", new TextFormat());
            Text.Input = new TextInputListener();

        }

        public void Draw()
        {
            //Text.Draw();
            
            FpsCounter.Draw();

        }
    }

    public class TextInputListener : InputListener
    {
        public override void Input(InputEvent Event)
        {
            if(Event.IsButton && Event.Button.IsDown)
            {
                (Sender as Text).String += "A";
            }
        }
    }
}
