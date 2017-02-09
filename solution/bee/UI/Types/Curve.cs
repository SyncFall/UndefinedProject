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
    public enum CurveType
    {
        Line,
        Quadratic,
        Cubic,
        Nurbs,
    }

    public class CurvePointList : ListCollection<CurvePoint>
    {
        public Curve Curve;
        public bool Intersect;

        public CurvePointList(CurveBase Curve=null)
        {
            this.Curve = Curve as Curve;
        }

        public void Add(float x, float y, float z=0)
        {
            base.Add(new CurvePoint(Curve, x, y, z));
        }

        public bool IntersectAll
        {
            set
            {
                Intersect = value;
                for (int i = 0; i < Size(); i++)
                {
                    this.Get(i).Intersect = value;
                }
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

    public class Curve : CurveBase
    {        
        public int Detail;
        public Curve Next;
        public Curve Prev;
        public bool Intersect;
        public bool Selected;

        public Curve(CurveType Type, int Degree=2, int Detail=50) : base(Type, Degree)
        {
            this.Detail = Detail;
        }

        public void AddPoint(float X, float Y, float Z=0)
        {
            this.Points.Add(X, Y, Z);
        }

        public bool UpdateIntersectStatus(float X, float Y)
        {
            Intersect = false;
            Points.Intersect = false;
            for (int i = 0; i < Points.Size(); i++)
            {
                CurvePoint point = Points.Get(i);
                point.Intersect = GeometryUtils.IntersectMargin((int)point.x, (int)point.y, (int)X, (int)Y, 10, 10);
                if(point.Intersect)
                {
                    Points.Intersect = true;
                    Intersect = true;
                }
            }
            if(Intersect)
            {
                return Intersect;
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
                if(GeometryUtils.IntersectMargin((int)point.x, (int)point.y, (int)X, (int)Y, 10, 10))
                {
                    Intersect = true;
                    break;
                }
            }
            return Intersect;
        }

        public void Draw()
        {
            if (Points.Size() < Degree + 1)
            {
                return;
            }
            if(Selected)
            {
                GL.Color3(1f, 128 / 255f, 0f);
            }
            else if(Intersect)
            {
                GL.Color3(175 / 255f, 175 / 255f, 175 / 255f);
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
        }

        public void DrawPoints()
        {
            if (!Intersect && !Selected)
            {
                return;
            }
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

    public class CurveBase
    {
        public CurveType Type;
        public CurvePointList Points;
        public ListCollection<float> Knots;
        public int Degree;
        public int Order;

        public CurveBase(CurveType Type, int Degree)
        {
            this.Type = Type;
            this.Points = new CurvePointList(this);
            this.Knots = new ListCollection<float>();
            this.Degree = Degree;
            this.Order = (Degree + 1);
        }

        public void BuildKnots()
        {
            Knots.Clear();
            int knotCount = (Points.Size() + Degree + 1);
            if (Type == CurveType.Nurbs)
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
                    Knots.Add((i < knotCount / 2 ? 0 : 1));
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
            if (k == 1)
            {
                if (Knots.Get(i) <= u && u < Knots.Get(i + 1))
                    return 1;
                else
                    return 0;
            }

            float d1 = (Knots.Get(i + k - 1) - Knots.Get(i));
            float n1 = (u - Knots.Get(i));
            float s1 = 0;
            if (d1 != 0 && n1 != 0)
                s1 = n1 * Blend(i, k - 1, u) / d1;

            float d2 = (Knots.Get(i + k) - Knots.Get(i + 1));
            float n2 = (-u + Knots.Get(i + k));
            float s2 = 0;
            if (d2 != 0 && n2 != 0)
                s2 = n2 * Blend(i + 1, k - 1, u) / d2;

            return s1 + s2;
        }
    }
}
