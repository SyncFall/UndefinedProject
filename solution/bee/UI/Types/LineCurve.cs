using Bee.Library;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI.Types
{
    public enum BeeLineCurveType
    {
        Linear,
        Conic,
        Cubic,
    }

    // two-point line, quadratic one control point bezier line, and two control point cubic bezier line
    public class LineCurve
    {
        public static readonly int DefaultDetailIterations = 50;

        public BeeLineCurveType Type;
        public BeePoint[] Points = new BeePoint[4];

        public LineCurve(BeeLineCurveType Type)
        {
            this.Type = Type;
        }

        public void Draw()
        {
            GL.LineWidth(2.5f);
            GL.Color3(System.Drawing.Color.White);
            GL.Begin(PrimitiveType.Lines);

            if(Type == BeeLineCurveType.Linear)
            {
                GL.Vertex2(Anchor1.x, Anchor1.y);
                GL.Vertex2(Anchor2.x, Anchor2.y);
            }
            else if(Type == BeeLineCurveType.Conic)
            {
                double x1 = Anchor1.x, x2;
                double y1 = Anchor1.y, y2;
                double t2, mt, mt2;
                for (double t = 0; t <= 1.0; t += 1 / (double)DefaultDetailIterations)
                {
                    t2 = t * t;
                    mt = 1 - t;
                    mt2 = mt * mt;

                    x2 = Anchor1.x * mt2 + Control1.x * 2 * mt * t + Anchor2.x * t2;
                    y2 = Anchor1.y * mt2 + Control1.y * 2 * mt * t + Anchor2.y * t2;

                    GL.Vertex2(x1, y1);
                    GL.Vertex2(x2, y2);
                    x1 = x2;
                    y1 = y2;
                }
                GL.Vertex2(x1, y1);
                GL.Vertex2(Anchor2.x, Anchor2.y);
            }
            else if(Type == BeeLineCurveType.Cubic)
            {
                double x1 = Anchor1.x, x2;
                double y1 = Anchor1.y, y2;
                double t2, t3, mt, mt2, mt3;
                for (double t = 0; t <= 1.0; t += 1 / (double)DefaultDetailIterations)
                {
                    t2 = t * t;
                    t3 = t2 * t;
                    mt = 1 - t;
                    mt2 = mt * mt;
                    mt3 = mt2 * mt;

                    x2 = Anchor1.x * mt3 + 3 * Control1.x * mt2 * t + 3 * Control2.x * mt * t2 + Anchor2.x * t3;
                    y2 = Anchor1.y * mt3 + 3 * Control1.y * mt2 * t + 3 * Control2.y * mt * t2 + Anchor2.y * t3;

                    GL.Vertex2(x1, y1);
                    GL.Vertex2(x2, y2);
                    x1 = x2;
                    y1 = y2;
                }
                GL.Vertex2(x1, y1);
                GL.Vertex2(Anchor2.x, Anchor2.y);
            }
            GL.End();

            /*
            GL.PointSize(12.5f);
            GL.Color3(Color.Blue);
            GL.Begin(PrimitiveType.Points);
            GL.Vertex2(Anchor1.x, Anchor1.y);
            GL.Vertex2(Anchor2.x, Anchor2.y);
            GL.End();

            GL.PointSize(12.5f);
            GL.Color3(Color.Red);
            GL.Begin(PrimitiveType.Points);
            if(Type == BeeLineCurveType.Conic || Type == BeeLineCurveType.Cubic)
            {
                GL.Vertex2(Control1.x, Control1.y);
            }
            if(Type == BeeLineCurveType.Cubic)
            {
                GL.Vertex2(Control2.x, Control2.y);
            }
            GL.End();
            */
        }

        #region Properties
        public BeePoint Anchor1
        {
            get
            {
                return Points[0];
            }
            set
            {
                Points[0] = value;
            }
        }

        public BeePoint Anchor2
        {
            get
            {
                return Points[3];
            }
            set
            {
                Points[3] = value;
            }
        }

        public BeePoint Control1
        {
            get
            {
                return Points[1];
            }
            set
            {
                Points[1] = value;
            }
        }

        public BeePoint Control2
        {
            get
            {
                return Points[2];
            }
            set
            {
                Points[2] = value;
            }
        }
        #endregion
    }

    public class NurbsCurve
    {
        public static readonly int DEFAULT_DETAIL_ITERATIONS = 10;
        public ListCollection<BeePoint> ControlPoints = new ListCollection<BeePoint>();

        public NurbsCurve()
        { }

        public void Draw()
        {
            GL.LineWidth(2.5f);
            GL.Color3(System.Drawing.Color.White);
            GL.Begin(PrimitiveType.LineStrip);

            double[] fu = new double[4];
            for(int i=0; i < DEFAULT_DETAIL_ITERATIONS; i++)
            {
                float u = (i / (float)DEFAULT_DETAIL_ITERATIONS);
                float u2 = u * u;
                float u3 = u2 * u;

                fu[0] = -u3 / 6.0 + u2 / 2.0 - u / 2.0 + 1.0 / 6.0;
                fu[1] = u3 / 2.0 - u2 + 2.0 / 3.0;
                fu[2] = -u3 / 2.0 + u2 / 2.0 + u / 2.0 + 1.0 / 6.0;
                fu[3] = u3 / 6.0;

                double x = 0.0f;
                double y = 0.0f;

                for (int j = 0; j < 4; j++)
                {
                    x += fu[j] * ControlPoints.Get(j).x;
                    y += fu[j] * ControlPoints.Get(j).y;
                }
                GL.Vertex2(x, y);
            }

            GL.End();

            GL.PointSize(5.0f);
            GL.Color3(System.Drawing.Color.Red);
            GL.Begin(PrimitiveType.Points);
            for(int i=0; i<4; i++)
            {
                GL.Vertex2(ControlPoints.Get(i).x, ControlPoints.Get(i).y);
            }
            GL.End();
        }
    }
}
