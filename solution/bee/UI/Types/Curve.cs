using Bee.Library;
using Bee.UI.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI
{ 
    public class CurveSurface
    {
        public CurvePath PathNodeBegin;
    }

    public class CurvePath
    {
        public Curve CurveNodeBegin;
        public CurvePath Next;
        public CurvePath Prev;
    }

    public enum PathType
    {
        Line,
        Quadratic,
        Cubic,
        Nurbs,
    }

    public class Curve
    {
        public PathType Type;
        public PointList Points = new PointList();
        public ListCollection<float> Knots = new ListCollection<float>();
        public int Detail;
        public int Degree;
        public int Order;
        public CurveInput Input;
        public Curve Next;
        public Curve Prev;

        public Curve(PathType Type, int Degree =2, int Detail=50)
        {
            this.Type = Type;
            this.Detail = Detail;
            this.Degree = Degree;
            this.Order = Degree + 1;
            //this.Input = new CurveInput(this);
        }

        public void Add(float X, float Y, float Z=0)
        {
            this.Points.Add(new Point(X, Y, Z));
        }

        public void BuildKnots()
        {
            Knots.Clear();
            int knotCount = (Points.Size() + Degree + 1);
            if (Type == PathType.Nurbs)
            {
                float uniformVal = (1 / (float)knotCount);
                float uniformSum = 0;
                for (int i = 0; i < knotCount; i++)
                {
                    Knots.Add(uniformSum);
                    uniformSum += uniformVal;
                }
            }
            else
            {
                for (int i = 0; i < knotCount; i++)
                {
                    if (i < knotCount / 2)
                    {
                        Knots.Add(0);
                    }
                    else
                    {
                        Knots.Add(1);
                    }
                }
            }
        }

        public Point GetPoint(float t)
        {
            float u = (1 - t) * Knots.Get(Order - 1) + t * Knots.Get(Points.Size());

            int i_plus_1 = 0;
            for (; Knots.Get(i_plus_1) <= u; i_plus_1++) { }
            int i_plus_1_minus_k = i_plus_1 - Order;

            Point point = new Point();
            for (int i = i_plus_1_minus_k; i < i_plus_1; i++)
            {
                float n = Blend(i, Order, u);
                point.x += Points.Get(i).x * n;
                point.y += Points.Get(i).y * n;
                point.z += Points.Get(i).z * n;
            }
            return point;
        }

        public float Blend(int i, int k, float u)
        {
            if(k == 1)
            {
                if(Knots.Get(i) <= u && u < Knots.Get(i+1))
                    return 1;
                else
                    return 0;
            }

            float d1 = (Knots.Get(i+k-1) - Knots.Get(i));
            float n1 = (u - Knots.Get(i));
            float s1 = 0;
            if(d1 != 0 && n1 != 0)
                s1 = n1 * Blend(i, k-1, u) / d1;

            float d2 = (Knots.Get(i+k) - Knots.Get(i+1));
            float n2 = (-u + Knots.Get(i+k));
            float s2 = 0;
            if(d2 != 0 && n2 != 0)
                s2 = n2 * Blend(i+1, k-1, u) / d2;

            return s1 + s2;
        }

        public void Draw()
        {
            if (Points.Size() < Degree + 1)
            {
                return;
            }
            BuildKnots();
            GL.Color3(1f, 1f, 1f);
            GL.LineWidth(2.5f);
            GL.Begin(PrimitiveType.LineStrip);
            int i = 0;
            float step = (1/(float)Detail);
            for(float t=0; t<=1f; t += step, i++)
            {
                if(i == Detail - 1)
                {
                    t = 0.999999f;
                }
                Point point = GetPoint(t);
                GL.Vertex3(point.x, point.y, point.z);
            }
            GL.End();
            //if(Input != null)
            {
                DrawPoints();
            }
        }

        public void DrawPoints()
        {
            GL.PointSize(10f);
            GL.Color3(0f, 150f, 100f);
            GL.Begin(PrimitiveType.Points);
            for(int i=0; i<Points.Size(); i++)
            {
                Point point = Points.Get(i);
                GL.Vertex2(point.x, point.y);
            }
            GL.End();
        }
    }

    public class CurveInput : InputListener
    {
        public Curve Curve;
        public Point MovePoint;

        public CurveInput(Curve Curve)
        {
            this.Curve = Curve;
        }

        public override void Input(InputEvent Event)
        {
            if(Event.IsButton)
            {
                ButtonState buttonState = Event.Button;
                if(buttonState.Type == Button.Left && buttonState.IsClick)
                {
                    for(int i=0; i< Curve.Points.Size(); i++)
                    {
                        if (GeometryUtils.IntersectPositionWithMargin((int)Curve.Points.Get(i).x, (int)Curve.Points.Get(i).y, Mouse.Cursor.X, Mouse.Cursor.Y, 25, 25))
                        {
                            MovePoint = Curve.Points.Get(i);
                            Console.WriteLine(MovePoint.x + "|" + MovePoint.y);
                            break;
                        }
                    }
                    if(MovePoint == null)
                    {
                        Point point = new Point(Mouse.Cursor.X, Mouse.Cursor.Y);
                        Curve.Points.Add(point);
                        Curve.BuildKnots();
                    }
                }
                if(buttonState.Type == Button.Left && buttonState.IsUp)
                {
                    MovePoint = null;
                }
            }
            if(Event.IsCursor)
            {
                CursorState cursorState = Event.Cursor;
                if(MovePoint != null)
                {
                    MovePoint.x = cursorState.X;
                    MovePoint.y = cursorState.Y;
                }
            }
            if(Event.IsKey)
            {
                KeyState keyState = Event.Key;
                if(keyState.Type == Key.X && keyState.IsClick)
                {
                    Curve.Points.Clear();
                }
                if (keyState.Type == Key.C && keyState.IsClick)
                {
                    if(Curve.Points.Size()>0)
                    {
                        Curve.Points.RemoveAt(Curve.Points.Size()-1);
                    }
                }
                if(keyState.Type == Key.S && keyState.IsClick)
                {
                    /*
                    StringBuilder strBuilder = new StringBuilder();
                    int detailIteration = 25;
                    for(float t=0; t<=1; t += 1/(float)detailIteration)
                    {
                        Point point = Curve.GetPoint(t);
                        strBuilder.AppendLine((point.x + ":" + point.y + ":" + point.z).Replace(',', '.'));
                    }
                    File.WriteAllText("D:\\dev\\EclipseJavaWorkspace2\\tri\\bin\\points.txt", strBuilder.ToString());
                    */
                }
            }
        }
    }
}
