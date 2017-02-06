using Bee.UI.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI
{
    public class CurveDraw
    {
        public FpsCounter FpsCounter;
        public Text Text;
        public Surface Surface;

        public CurveDraw()
        {
            FpsCounter = new FpsCounter();
            Text = new Text("fuckyou\nTOO!", new TextFormat());
            Text.Input = new TextInputListener();

            Surface = new Surface(SurfaceType.Rect);
            Surface.Transform = true;
        }

        public void Draw()
        {
            //Text.Draw();
            Surface.Draw();
            FpsCounter.Draw();
        }
    }

    public class TextInputListener : InputListener
    {
        public override void Input(InputEvent Event)
        {
            if(Event.Is(InputType.Button) && Event.Button.IsDown)
            {
                (Sender as Text).String += "A";
            }
        }
    }
}
