using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI.Triangulation
{
    public class Point2f : Tuple2f
    {
        public Point2f(float paramFloat1, float paramFloat2) : base(paramFloat1, paramFloat2)
        { }

        public Point2f(float[] paramArrayOfFloat) : base(paramArrayOfFloat)
        { }

        public Point2f(Point2f paramPoint2f) : base(paramPoint2f)
        { }

        public Point2f(Tuple2f paramTuple2f) : base(paramTuple2f)
        { }

        public Point2f()
        { }

        public float distanceSquared(Point2f paramPoint2f)
        {
            float f1 = this.x - paramPoint2f.x;
            float f2 = this.y - paramPoint2f.y;
            return (f1 * f1 + f2 * f2);
        }

        public float distance(Point2f paramPoint2f)
        {
            float f1 = this.x - paramPoint2f.x;
            float f2 = this.y - paramPoint2f.y;
            return (float)Math.Sqrt(f1 * f1 + f2 * f2);
        }

        public float distanceL1(Point2f paramPoint2f)
        {
            return (Math.Abs(this.x - paramPoint2f.x) + Math.Abs(this.y - paramPoint2f.y));
        }

        public float distanceLinf(Point2f paramPoint2f)
        {
            return Math.Max(Math.Abs(this.x - paramPoint2f.x), Math.Abs(this.y - paramPoint2f.y));
        }
    }
}
