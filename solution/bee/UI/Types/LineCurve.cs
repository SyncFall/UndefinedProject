using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI
{
    public class LineCurve
    {
        public CurveType CurveType;
        public CurveList Curves = new CurveList();
        public bool Intersect;

        public LineCurve()
        { }

        public Curve AddSegment(CurveType Type, int Detail = 25)
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
            Curves.Add(Curve);
            return Curve;
        }

        public bool UpdateIntersectStatus(int X, int Y)
        {
            Intersect = false;
            for (int i = 0; i < Curves.Size; i++)
            {
                if (Curves[i].UpdateIntersectStatus(X, Y))
                {
                    Intersect = true;
                }
            }
            return Intersect;
        }

        public void Draw()
        {
            for (int i = 0; i < Curves.Size; i++)
            {
                Curves[i].Draw();
            }
            for (int i = 0; i < Curves.Size; i++)
            {
                Curves[i].DrawPoints();
            }
        }

        public Size Size
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
