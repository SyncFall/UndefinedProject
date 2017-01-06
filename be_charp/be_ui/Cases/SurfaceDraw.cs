using Be.Runtime.Types;
using Be.UI.Types;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.UI
{
    public class SurfaceDraw
    {
        public WindowType WindowType;
        public BeeSurfacePatch Surface;

        public SurfaceDraw(WindowType Window)
        {
            this.WindowType = Window;
            this.Surface = new BeeSurfacePatch(BeeSurfacePatchType.BiCubic);

            this.Surface.Points[0, 0] = new BeePoint(100, 100);
            this.Surface.Points[0, 1] = new BeePoint(250, 75);
            this.Surface.Points[0, 2] = new BeePoint(300, 50);
            this.Surface.Points[0, 3] = new BeePoint(450, 150);

            this.Surface.Points[1, 0] = new BeePoint(75, 150);
            this.Surface.Points[1, 1] = new BeePoint(200, 125);
            this.Surface.Points[1, 2] = new BeePoint(300, 100);
            this.Surface.Points[1, 3] = new BeePoint(450, 200);

            this.Surface.Points[2, 0] = new BeePoint(125, 200);
            this.Surface.Points[2, 1] = new BeePoint(200, 150);
            this.Surface.Points[2, 2] = new BeePoint(300, 110);
            this.Surface.Points[2, 3] = new BeePoint(450, 210);

            this.Surface.Points[3, 0] = new BeePoint(125, 275);
            this.Surface.Points[3, 1] = new BeePoint(225, 175);
            this.Surface.Points[3, 2] = new BeePoint(300, 125);
            this.Surface.Points[3, 3] = new BeePoint(475, 225);

            this.Surface.Build();
        }

        public void Draw()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, WindowType.Width, WindowType.Height, 0, 0, 1);

            Surface.Draw();
        }
    }
}
