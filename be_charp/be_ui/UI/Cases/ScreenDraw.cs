using Bee.UI.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI.Cases
{
    public class ScreenDraw
    {
        public WindowType WindowType;
        public ScreenType ScreenType;

        public ScreenDraw(WindowType WindowType)
        {
            this.WindowType = WindowType;
            this.ScreenType = new ScreenType(WindowType);
            this.WindowType.Mouse.AddListener(new ScreenDrawMouseListener(this));
        }

        public void Draw()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, WindowType.Width, WindowType.Height, 0, 0, 1);

            ScreenType.Draw();
        }
    }

    public class ScreenDrawMouseListener : MouseListener
    {
        public ScreenDraw ScreenDraw;

        public ScreenDrawMouseListener(ScreenDraw ScreenDraw)
        {
            this.ScreenDraw = ScreenDraw;
        }

        public override void MouseEvent(MouseResult Result)
        {
            //ScreenDraw.ScreenType.MouseEvent(Result);
        }
    }
}
