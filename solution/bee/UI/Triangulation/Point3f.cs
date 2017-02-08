using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI.Triangulation
{
    public class Point3f : Tuple3f
    {
        public Point3f(float paramFloat1, float paramFloat2, float paramFloat3) : base(paramFloat1, paramFloat2, paramFloat3)
        { }

        public Point3f(float[] paramArrayOfFloat) : base(paramArrayOfFloat)
        { }

        public Point3f(Point3f paramPoint3f) : base(paramPoint3f)
        { }

        public Point3f(Tuple3f paramTuple3f) : base(paramTuple3f)
        { }

        public Point3f()
        { }

        public float distanceSquared(Point3f paramPoint3f)
        {
            float f1 = this.x - paramPoint3f.x;
            float f2 = this.y - paramPoint3f.y;
            float f3 = this.z - paramPoint3f.z;
            return (f1 * f1 + f2 * f2 + f3 * f3);
        }

        public float distance(Point3f paramPoint3f)
        {
            float f1 = this.x - paramPoint3f.x;
            float f2 = this.y - paramPoint3f.y;
            float f3 = this.z - paramPoint3f.z;
            return (float)Math.Sqrt(f1 * f1 + f2 * f2 + f3 * f3);
        }

        public float distanceL1(Point3f paramPoint3f)
        {
            return (Math.Abs(this.x - paramPoint3f.x) + Math.Abs(this.y - paramPoint3f.y)
                    + Math.Abs(this.z - paramPoint3f.z));
        }

        public float distanceLinf(Point3f paramPoint3f)
        {
            float f = Math.Max(Math.Abs(this.x - paramPoint3f.x), Math.Abs(this.y - paramPoint3f.y));
            return Math.Max(f, Math.Abs(this.z - paramPoint3f.z));
        }

        /*
        public void project(Point4f paramPoint4f) {
            float f = 1.0F / paramPoint4f.w;
            this.x = (paramPoint4f.x * f);
            this.y = (paramPoint4f.y * f);
            this.z = (paramPoint4f.z * f);
        }
        */
    }
}
