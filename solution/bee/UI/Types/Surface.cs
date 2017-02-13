using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI
{
    public class Surface : Compose
    {
        public CurveType CurveType;
        public Curve CurveRoot;
        public bool Intersect;

        public Surface() : base(ComposeType.Surface)
        { }
        public Curve AddCurve(CurveType Type, int Detail = 25)
        {
            Curve Curve = null;
            if (Type == CurveType.Line)
            {
                Curve = new Curve(Type, 1, Detail);
            }
            else if (Type == CurveType.Quadratic)
            {
                Curve = new Curve(Type, 2, Detail);
            }
            else if (Type == CurveType.Cubic)
            {
                Curve = new Curve(Type, 3, Detail);
            }
            else if (Type == CurveType.Nurbs)
            {
                Curve = new Curve(Type, 2, Detail);
            }
            else
            {
                throw new Exception("invalid state");
            }
            return this.AddCurve(Curve);
        }


        public Curve AddCurve(Curve Curve)
        {
            if (CurveRoot == null)
            {
                CurveRoot = Curve;
                Curve.Prev = null;
            }
            else
            {
                Curve curveNode = CurveRoot;
                while (curveNode.Next != null)
                {
                    curveNode = curveNode.Next;
                }
                curveNode.Next = Curve;
                Curve.Prev = curveNode;
            }
            return Curve;
        }

        public void RemoveCurve(Curve Curve)
        {
            if (Curve.Prev == null)
            {
                if (Curve.Next != null)
                {
                    CurveRoot = Curve.Next;
                    CurveRoot.Prev = null;
                }
                else
                {
                    CurveRoot = null;
                }
            }
            else
            {
                Curve.Prev.Next = Curve.Next;
                if (Curve.Next != null)
                {
                    Curve.Next.Prev = Curve.Prev;
                }
            }
        }

        public bool UpdateIntersectStatus(int X, int Y)
        {
            Intersect = false;
            Curve curveNode = CurveRoot;
            while (curveNode != null)
            {
                if (curveNode.UpdateIntersectStatus(X, Y))
                {
                    Intersect = true;
                }
                curveNode = curveNode.Next;
            }
            return Intersect;
        }

        public override void Draw()
        {
            Curve curveNode = CurveRoot;
            while (curveNode != null)
            {
                curveNode.Draw();
                curveNode = curveNode.Next;
            }
            curveNode = CurveRoot;
            while (curveNode != null)
            {
                curveNode.DrawPoints();
                curveNode = curveNode.Next;
            }
        }

        public override Size Size
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
