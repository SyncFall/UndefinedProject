using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI.Triangulation
{
    public class Vector3f : Tuple3f
    {
        public Vector3f(float paramFloat1, float paramFloat2, float paramFloat3) : base(paramFloat1, paramFloat2, paramFloat3)
        { }

        public Vector3f(float[] paramArrayOfFloat) : base(paramArrayOfFloat)
        { }

        public Vector3f(Vector3f paramVector3f) : base(paramVector3f)
        { }

        public Vector3f(Tuple3f paramTuple3f) : base(paramTuple3f)
        { }

        public Vector3f()
        { }

        public float lengthSquared()
        {
            return (this.x * this.x + this.y * this.y + this.z * this.z);
        }

        public float length()
        {
            return (float)Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z);
        }

        public void cross(Vector3f paramVector3f1, Vector3f paramVector3f2)
        {
            float f1 = paramVector3f1.y * paramVector3f2.z - (paramVector3f1.z * paramVector3f2.y);
            float f2 = paramVector3f2.x * paramVector3f1.z - (paramVector3f2.z * paramVector3f1.x);
            this.z = (paramVector3f1.x * paramVector3f2.y - (paramVector3f1.y * paramVector3f2.x));
            this.x = f1;
            this.y = f2;
        }

        public float dot(Vector3f paramVector3f)
        {
            return (this.x * paramVector3f.x + this.y * paramVector3f.y + this.z * paramVector3f.z);
        }

        public void normalize(Vector3f paramVector3f)
        {
            float f = (float)(1.0D / Math.Sqrt(paramVector3f.x * paramVector3f.x + paramVector3f.y * paramVector3f.y + paramVector3f.z * paramVector3f.z));
            this.x = (paramVector3f.x * f);
            this.y = (paramVector3f.y * f);
            this.z = (paramVector3f.z * f);
        }

        public void normalize()
        {
            float f = (float)(1.0D / Math.Sqrt(this.x * this.x + this.y * this.y + this.z * this.z));
            this.x *= f;
            this.y *= f;
            this.z *= f;
        }

        public float angle(Vector3f paramVector3f)
        {
            double d = dot(paramVector3f) / length() * paramVector3f.length();
            if (d < -1.0D)
                d = -1.0D;
            if (d > 1.0D)
                d = 1.0D;
            return (float)Math.Acos(d);
        }
    }
}
