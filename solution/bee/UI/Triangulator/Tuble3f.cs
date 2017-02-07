using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI.Triangulator
{
    public abstract class Tuple3f
    {
        public float x;
        public float y;
        public float z;

        public Tuple3f(float paramFloat1, float paramFloat2, float paramFloat3)
        {
            this.x = paramFloat1;
            this.y = paramFloat2;
            this.z = paramFloat3;
        }

        public Tuple3f(float[] paramArrayOfFloat)
        {
            this.x = paramArrayOfFloat[0];
            this.y = paramArrayOfFloat[1];
            this.z = paramArrayOfFloat[2];
        }

        public Tuple3f(Tuple3f paramTuple3f)
        {
            this.x = paramTuple3f.x;
            this.y = paramTuple3f.y;
            this.z = paramTuple3f.z;
        }

        public Tuple3f()
        {
            this.x = 0.0F;
            this.y = 0.0F;
            this.z = 0.0F;
        }

        public override string ToString()
        {
            return "(" + this.x + ", " + this.y + ", " + this.z + ")";
        }

        public void set(float paramFloat1, float paramFloat2, float paramFloat3)
        {
            this.x = paramFloat1;
            this.y = paramFloat2;
            this.z = paramFloat3;
        }

        public void set(float[] paramArrayOfFloat)
        {
            this.x = paramArrayOfFloat[0];
            this.y = paramArrayOfFloat[1];
            this.z = paramArrayOfFloat[2];
        }

        public void set(Tuple3f paramTuple3f)
        {
            this.x = paramTuple3f.x;
            this.y = paramTuple3f.y;
            this.z = paramTuple3f.z;
        }

        public void get(float[] paramArrayOfFloat)
        {
            paramArrayOfFloat[0] = this.x;
            paramArrayOfFloat[1] = this.y;
            paramArrayOfFloat[2] = this.z;
        }

        public void get(Tuple3f paramTuple3f)
        {
            paramTuple3f.x = this.x;
            paramTuple3f.y = this.y;
            paramTuple3f.z = this.z;
        }

        public void add(Tuple3f paramTuple3f1, Tuple3f paramTuple3f2)
        {
            this.x = (paramTuple3f1.x + paramTuple3f2.x);
            this.y = (paramTuple3f1.y + paramTuple3f2.y);
            this.z = (paramTuple3f1.z + paramTuple3f2.z);
        }

        public void add(Tuple3f paramTuple3f)
        {
            this.x += paramTuple3f.x;
            this.y += paramTuple3f.y;
            this.z += paramTuple3f.z;
        }

        public void sub(Tuple3f paramTuple3f1, Tuple3f paramTuple3f2)
        {
            this.x = (paramTuple3f1.x - paramTuple3f2.x);
            this.y = (paramTuple3f1.y - paramTuple3f2.y);
            this.z = (paramTuple3f1.z - paramTuple3f2.z);
        }

        public void sub(Tuple3f paramTuple3f)
        {
            this.x -= paramTuple3f.x;
            this.y -= paramTuple3f.y;
            this.z -= paramTuple3f.z;
        }

        public void negate(Tuple3f paramTuple3f)
        {
            this.x = (-paramTuple3f.x);
            this.y = (-paramTuple3f.y);
            this.z = (-paramTuple3f.z);
        }

        public void negate()
        {
            this.x = (-this.x);
            this.y = (-this.y);
            this.z = (-this.z);
        }

        public void scale(float paramFloat, Tuple3f paramTuple3f)
        {
            this.x = (paramFloat * paramTuple3f.x);
            this.y = (paramFloat * paramTuple3f.y);
            this.z = (paramFloat * paramTuple3f.z);
        }

        public void scale(float paramFloat)
        {
            this.x *= paramFloat;
            this.y *= paramFloat;
            this.z *= paramFloat;
        }

        public void scaleAdd(float paramFloat, Tuple3f paramTuple3f1, Tuple3f paramTuple3f2)
        {
            this.x = (paramFloat * paramTuple3f1.x + paramTuple3f2.x);
            this.y = (paramFloat * paramTuple3f1.y + paramTuple3f2.y);
            this.z = (paramFloat * paramTuple3f1.z + paramTuple3f2.z);
        }

        public void scaleAdd(float paramFloat, Tuple3f paramTuple3f)
        {
            this.x = (paramFloat * this.x + paramTuple3f.x);
            this.y = (paramFloat * this.y + paramTuple3f.y);
            this.z = (paramFloat * this.z + paramTuple3f.z);
        }

        public bool equals(Tuple3f paramTuple3f)
        {
            try
            {
                return ((this.x == paramTuple3f.x) && (this.y == paramTuple3f.y) && (this.z == paramTuple3f.z));
            }
            catch (Exception e)
            { }
            return false;
        }

        public bool equals(Object paramObject)
        {
            try
            {
                Tuple3f localTuple3f = (Tuple3f)paramObject;
                return ((this.x == localTuple3f.x) && (this.y == localTuple3f.y) && (this.z == localTuple3f.z));
            }
            catch (Exception e)
            { }
            return false;
        }

        /*
        public bool epsilonEquals(Tuple3f paramTuple3f, float paramFloat)
        {
            float f = this.x - paramTuple3f.x;
            if (Float.isNaN(f))
                return false;
            if (((f < 0.0F) ? -f : f) > paramFloat)
                return false;

            f = this.y - paramTuple3f.y;
            if (Float.isNaN(f))
                return false;
            if (((f < 0.0F) ? -f : f) > paramFloat)
                return false;

            f = this.z - paramTuple3f.z;
            if (Float.isNaN(f))
                return false;
            return (((f < 0.0F) ? -f : f) <= paramFloat);
        }
        */

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                hash = hash * 23 + x.GetHashCode();
                hash = hash * 23 + y.GetHashCode();
                hash = hash * 23 + z.GetHashCode();
                return hash;
            }
        }

        public void clamp(float paramFloat1, float paramFloat2, Tuple3f paramTuple3f)
        {
            if (paramTuple3f.x > paramFloat2)
                this.x = paramFloat2;
            else if (paramTuple3f.x < paramFloat1)
                this.x = paramFloat1;
            else
            {
                this.x = paramTuple3f.x;
            }

            if (paramTuple3f.y > paramFloat2)
                this.y = paramFloat2;
            else if (paramTuple3f.y < paramFloat1)
                this.y = paramFloat1;
            else
            {
                this.y = paramTuple3f.y;
            }

            if (paramTuple3f.z > paramFloat2)
                this.z = paramFloat2;
            else if (paramTuple3f.z < paramFloat1)
                this.z = paramFloat1;
            else
                this.z = paramTuple3f.z;
        }

        public void clampMin(float paramFloat, Tuple3f paramTuple3f)
        {
            if (paramTuple3f.x < paramFloat)
                this.x = paramFloat;
            else
            {
                this.x = paramTuple3f.x;
            }

            if (paramTuple3f.y < paramFloat)
                this.y = paramFloat;
            else
            {
                this.y = paramTuple3f.y;
            }

            if (paramTuple3f.z < paramFloat)
                this.z = paramFloat;
            else
                this.z = paramTuple3f.z;
        }

        public void clampMax(float paramFloat, Tuple3f paramTuple3f)
        {
            if (paramTuple3f.x > paramFloat)
                this.x = paramFloat;
            else
            {
                this.x = paramTuple3f.x;
            }

            if (paramTuple3f.y > paramFloat)
                this.y = paramFloat;
            else
            {
                this.y = paramTuple3f.y;
            }

            if (paramTuple3f.z > paramFloat)
                this.z = paramFloat;
            else
                this.z = paramTuple3f.z;
        }

        public void absolute(Tuple3f paramTuple3f)
        {
            this.x = Math.Abs(paramTuple3f.x);
            this.y = Math.Abs(paramTuple3f.y);
            this.z = Math.Abs(paramTuple3f.z);
        }

        public void clamp(float paramFloat1, float paramFloat2)
        {
            if (this.x > paramFloat2)
                this.x = paramFloat2;
            else if (this.x < paramFloat1)
            {
                this.x = paramFloat1;
            }

            if (this.y > paramFloat2)
                this.y = paramFloat2;
            else if (this.y < paramFloat1)
            {
                this.y = paramFloat1;
            }

            if (this.z > paramFloat2)
                this.z = paramFloat2;
            else if (this.z < paramFloat1)
                this.z = paramFloat1;
        }

        public void clampMin(float paramFloat)
        {
            if (this.x < paramFloat)
                this.x = paramFloat;
            if (this.y < paramFloat)
                this.y = paramFloat;
            if (this.z >= paramFloat)
                return;
            this.z = paramFloat;
        }

        public void clampMax(float paramFloat)
        {
            if (this.x > paramFloat)
                this.x = paramFloat;
            if (this.y > paramFloat)
                this.y = paramFloat;
            if (this.z <= paramFloat)
                return;
            this.z = paramFloat;
        }

        public void absolute()
        {
            this.x = Math.Abs(this.x);
            this.y = Math.Abs(this.y);
            this.z = Math.Abs(this.z);
        }

        public void interpolate(Tuple3f paramTuple3f1, Tuple3f paramTuple3f2, float paramFloat)
        {
            this.x = ((1.0F - paramFloat) * paramTuple3f1.x + paramFloat * paramTuple3f2.x);
            this.y = ((1.0F - paramFloat) * paramTuple3f1.y + paramFloat * paramTuple3f2.y);
            this.z = ((1.0F - paramFloat) * paramTuple3f1.z + paramFloat * paramTuple3f2.z);
        }

        public void interpolate(Tuple3f paramTuple3f, float paramFloat)
        {
            this.x = ((1.0F - paramFloat) * this.x + paramFloat * paramTuple3f.x);
            this.y = ((1.0F - paramFloat) * this.y + paramFloat * paramTuple3f.y);
            this.z = ((1.0F - paramFloat) * this.z + paramFloat * paramTuple3f.z);
        }

        /*
        public Object clone()
        {
            try
            {
                return super.clone();
            }
            catch (CloneNotSupportedException localCloneNotSupportedException)
            {
                throw new InternalError();
            }
        }
        */

        public float getX()
        {
            return this.x;
        }

        public void setX(float paramFloat)
        {
            this.x = paramFloat;
        }

        public float getY()
        {
            return this.y;
        }

        public void setY(float paramFloat)
        {
            this.y = paramFloat;
        }

        public float getZ()
        {
            return this.z;
        }

        public void setZ(float paramFloat)
        {
            this.z = paramFloat;
        }
    }
}
