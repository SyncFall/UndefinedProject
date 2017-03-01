using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI
{
    public enum SurfaceType
    {
//        Point,
        Rect,
        LineCurve,
 //       FreeHand,
    }

    public abstract class Surface
    {
        public SurfaceType Type;
        public Layout Layout;
        public LineCurve LineCurve = new LineCurve();

        public Surface(SurfaceType Type)
        {
            this.Type = Type;
        }

        public void Draw(float X, float Y)
        {
            GL.MatrixMode(MatrixMode.Modelview);
            GL.LoadIdentity();
            GL.Translate(X, Y, 0);
            LineCurve.Draw();
            GL.LoadIdentity();
        }
    }

    
    public class RectSurface : Surface
    {
        public float Width;
        public float Height;

        public RectSurface(float width, float height) : base(SurfaceType.Rect)
        {
            this.Width = width;
            this.Height = height;
            this.LineCurve.AddSegment(CurveType.Line);
            this.LineCurve.Curves[0].Points.Add(0, 0);
            this.LineCurve.Curves[0].Points.Add(width, 0);
            this.LineCurve.AddSegment(CurveType.Line);
            this.LineCurve.Curves[1].Points.Add(width, 0);
            this.LineCurve.Curves[1].Points.Add(width, height);
            this.LineCurve.AddSegment(CurveType.Line);
            this.LineCurve.Curves[2].Points.Add(width, height);
            this.LineCurve.Curves[2].Points.Add(0, height);
            this.LineCurve.AddSegment(CurveType.Line);
            this.LineCurve.Curves[3].Points.Add(0, height);
            this.LineCurve.Curves[3].Points.Add(0, 0);
        }
    }

    public class LineCurveSurface : Surface
    {
        public LineCurveSurface() : base(SurfaceType.LineCurve)
        { }
    }
}
