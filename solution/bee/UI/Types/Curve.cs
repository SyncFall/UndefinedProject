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
        public CurveList Curves;

        public CurveSurface()
        {
            this.Curves = new CurveList();
        }
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

    public class CurvePointList : ListCollection<CurvePoint>
    {
        public Curve Curve;

        public CurvePointList(Curve Curve)
        {
            this.Curve = Curve;
        }

        public void Add(float x, float y, float z=0)
        {
            base.Add(new CurvePoint(Curve, x, y, z));
        }

        public void SetSelected(bool boolean)
        {
            for(int i=0; i < this.Size(); i++)
            {
                this.Get(i).Selected = boolean;
            }
        }
    }

    public class CurvePoint : Point
    {
        public Curve Curve;
        public bool Intersect;
        public bool Selected;

        public CurvePoint(Curve Curve, float x, float y, float z=0) : base(x, y, z)
        {
            this.Curve = Curve;
        }
    }

    public class CurveList : ListCollection<Curve>
    { }

    public class Curve
    {
        public PathType Type;
        public CurvePointList Points;
        public CurvePointList PointsIntersected;
        public ListCollection<float> Knots;
        public int Detail;
        public int Degree;
        public int Order;
        public CurveInput Input;
        public Curve Next;
        public Curve Prev;
        public bool Intersect;
        public bool Selected;

        public Curve(PathType Type, int Degree=2, int Detail=50)
        {
            this.Type = Type;
            this.Points = new CurvePointList(this);
            this.PointsIntersected = new CurvePointList(this);
            this.Knots = new ListCollection<float>();
            this.Detail = Detail;
            this.Degree = Degree;
            this.Order = Degree+1;
            this.Input = new CurveInput(this);
        }

        public void Add(float X, float Y, float Z=0)
        {
            this.Points.Add(X, Y, Z);
            BuildKnots();
        }

        public bool UpdateIntersectState(float x, float y)
        {
            Intersect = false;
            PointsIntersected.Clear();
            for (int i = 0; i < Points.Size(); i++)
            {
                CurvePoint point = Points.Get(i);
                point.Intersect = GeometryUtils.IntersectPositionWithMargin((int)point.x, (int)point.y, (int)x, (int)y, 10, 10);
                if(point.Intersect)
                {
                    PointsIntersected.Add(point);
                    Intersect = true;
                }
            }
            float t = 0f;
            int detail = 100;
            float step = (1 / (float)detail);
            for (int i = 0; t <= 1f; t += step, i++)
            {
                if (i == detail - 1)
                {
                    t = 0.999999f;
                }
                Point point = GetPoint(t);
                if(GeometryUtils.IntersectPositionWithMargin((int)point.x, (int)point.y, (int)x, (int)y, 10, 10))
                {
                    Intersect = true;
                    break;
                }
            }
            return Intersect;
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
                    Knots.Add((i<knotCount/2 ? 0 : 1));
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
            if (Points.Size() < Degree + 1 || Knots.Size() == 0)
            {
                return;
            }
            if(Intersect)
            {
                GL.Color3(150/255f, 150/255f, 150/255f);
            }
            else
            {
                GL.Color3(1f, 1f, 1f);
            }
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
            GL.Begin(PrimitiveType.Points);
            for(int i=0; i<Points.Size(); i++)
            {
                CurvePoint point = Points.Get(i);
                if(point.Selected)
                {
                    GL.Color3(1f, 127 / 255f, 0f);
                }
                else if(point.Intersect)
                {
                    GL.Color3(0f, 200 / 255f, 100 / 255f);
                }
                else
                {
                    GL.Color3(0f, 150 / 255f, 100 / 255f);
                }
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
                if(Event.Button.Type == Button.Left && Event.Button.IsClick)
                {
                    for(int i=0; i< Curve.Points.Size(); i++)
                    {
                        if (GeometryUtils.IntersectPositionWithMargin((int)Curve.Points.Get(i).x, (int)Curve.Points.Get(i).y, Mouse.Cursor.x, Mouse.Cursor.y, 25, 25))
                        {
                            MovePoint = Curve.Points.Get(i);
                            break;
                        }
                    }
                }
                if(Event.Button.Type == Button.Left && Event.Button.IsUp)
                {
                    MovePoint = null;
                }
            }
            if(Event.IsCursor)
            {
                if(MovePoint != null)
                {
                    MovePoint.x = Event.Cursor.x;
                    MovePoint.y = Event.Cursor.y;
                }
            }
        }
    }
}
