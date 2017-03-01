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
        public Surface Surface;

        public SurfaceDraw()
        {
            FpsCounter = new FpsCounter();
            Surface = new RectSurface(200, 50);
        }

        public void Draw()
        {
            Surface.Draw(50, 50);
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
