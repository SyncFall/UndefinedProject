using Bee.UI.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI.Cases
{
    public class PolygonDraw
    {
        public WindowType WindowType;
        public Polygon Polygon;

        public PolygonDraw(WindowType Window)
        {
            this.WindowType = Window;
            this.WindowType.Mouse.AddListener(new PolygonDrawMouseListener(this));

            Polygon = new Polygon();
            Polygon.Points.Add(new BeePoint(100, 100));
            Polygon.Points.Add(new BeePoint(200, 100));
            Polygon.Points.Add(new BeePoint(200, 200));
            Polygon.Points.Add(new BeePoint(150, 150));
            Polygon.Points.Add(new BeePoint(100, 200));
        }

        public void Draw()
        {
            if(Polygon != null)
            {
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(0, WindowType.Width, WindowType.Height, 0, 0, 1);

                GL.DepthFunc(DepthFunction.Always);

                Polygon.Draw();
            }
        }
    }

    public class PolygonDrawMouseListener : MouseListener
    {
        public PolygonDraw PolygonDraw;

        public PolygonDrawMouseListener(PolygonDraw PolygonDraw)
        {
            this.PolygonDraw = PolygonDraw;
        }
        /*
        public override void MouseEvent(MouseResult Result)
        {
            if(PolygonDraw.Polygon != null)
            {
                PolygonDraw.Polygon.MouseEvent(Result);
            }
        }
        */
    }
}
