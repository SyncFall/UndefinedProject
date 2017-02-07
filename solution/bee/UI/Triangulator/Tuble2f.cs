using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI.Triangulator
{
    public abstract class Tuple2f
    {
        public float x;
        public float y;

        public Tuple2f(float paramFloat1, float paramFloat2)
        {
            this.x = paramFloat1;
            this.y = paramFloat2;
        }

        public Tuple2f(float[] paramArrayOfFloat)
        {
            this.x = paramArrayOfFloat[0];
            this.y = paramArrayOfFloat[1];
        }

        public Tuple2f(Tuple2f paramTuple2f)
        {
            this.x = paramTuple2f.x;
            this.y = paramTuple2f.y;
        }

        public Tuple2f()
        {
            this.x = 0.0F;
            this.y = 0.0F;
        }

        public void set(float paramFloat1, float paramFloat2)
        {
            this.x = paramFloat1;
            this.y = paramFloat2;
        }

        public void set(float[] paramArrayOfFloat)
        {
            this.x = paramArrayOfFloat[0];
            this.y = paramArrayOfFloat[1];
        }

        public void set(Tuple2f paramTuple2f)
        {
            this.x = paramTuple2f.x;
            this.y = paramTuple2f.y;
        }

        public void get(float[] paramArrayOfFloat)
        {
            paramArrayOfFloat[0] = this.x;
            paramArrayOfFloat[1] = this.y;
        }

        public void add(Tuple2f paramTuple2f1, Tuple2f paramTuple2f2)
        {
            this.x = (paramTuple2f1.x + paramTuple2f2.x);
            this.y = (paramTuple2f1.y + paramTuple2f2.y);
        }

        public void add(Tuple2f paramTuple2f)
        {
            this.x += paramTuple2f.x;
            this.y += paramTuple2f.y;
        }

        public void sub(Tuple2f paramTuple2f1, Tuple2f paramTuple2f2)
        {
            this.x = (paramTuple2f1.x - paramTuple2f2.x);
            this.y = (paramTuple2f1.y - paramTuple2f2.y);
        }

        public void sub(Tuple2f paramTuple2f)
        {
            this.x -= paramTuple2f.x;
            this.y -= paramTuple2f.y;
        }

        public void negate(Tuple2f paramTuple2f)
        {
            this.x = (-paramTuple2f.x);
            this.y = (-paramTuple2f.y);
        }

        public void negate()
        {
            this.x = (-this.x);
            this.y = (-this.y);
        }

        public void scale(float paramFloat, Tuple2f paramTuple2f)
        {
            this.x = (paramFloat * paramTuple2f.x);
            this.y = (paramFloat * paramTuple2f.y);
        }

        public void scale(float paramFloat)
        {
            this.x *= paramFloat;
            this.y *= paramFloat;
        }

        public void scaleAdd(float paramFloat, Tuple2f paramTuple2f1, Tuple2f paramTuple2f2)
        {
            this.x = (paramFloat * paramTuple2f1.x + paramTuple2f2.x);
            this.y = (paramFloat * paramTuple2f1.y + paramTuple2f2.y);
        }

        public void scaleAdd(float paramFloat, Tuple2f paramTuple2f)
        {
            this.x = (paramFloat * this.x + paramTuple2f.x);
            this.y = (paramFloat * this.y + paramTuple2f.y);
        }

        /*
        public int hashCode()
        {
            long l = 1L;
            l = 31L * l + VecMathUtil.floatToIntBits(this.x);
            l = 31L * l + VecMathUtil.floatToIntBits(this.y);
            return (int)(l ^ l >> 32);
        }
        */

        public bool equals(Tuple2f paramTuple2f)
        {
            try
            {
                return ((this.x == paramTuple2f.x) && (this.y == paramTuple2f.y));
            }
            catch (Exception e)
            { }
            return false;
        }

        public bool equals(Object paramObject)
        {
            try
            {
                Tuple2f localTuple2f = (Tuple2f)paramObject;
                return ((this.x == localTuple2f.x) && (this.y == localTuple2f.y));
            }
            catch (Exception e)
            {
                return false;
            }
        }

        /*
        public bool epsilonEquals(Tuple2f paramTuple2f, float paramFloat)
        {
            float f = this.x - paramTuple2f.x;
            if (Float.isNaN(f))
                return false;
            if (((f < 0.0F) ? -f : f) > paramFloat)
                return false;

            f = this.y - paramTuple2f.y;
            if (Float.isNaN(f))
                return false;
            return (((f < 0.0F) ? -f : f) <= paramFloat);
        }
        */

        public String toString()
        {
            return "(" + this.x + ", " + this.y + ")";
        }

        public void clamp(float paramFloat1, float paramFloat2, Tuple2f paramTuple2f)
        {
            if (paramTuple2f.x > paramFloat2)
                this.x = paramFloat2;
            else if (paramTuple2f.x < paramFloat1)
                this.x = paramFloat1;
            else
            {
                this.x = paramTuple2f.x;
            }

            if (paramTuple2f.y > paramFloat2)
                this.y = paramFloat2;
            else if (paramTuple2f.y < paramFloat1)
                this.y = paramFloat1;
            else
                this.y = paramTuple2f.y;
        }

        public void clampMin(float paramFloat, Tuple2f paramTuple2f)
        {
            if (paramTuple2f.x < paramFloat)
                this.x = paramFloat;
            else
            {
                this.x = paramTuple2f.x;
            }

            if (paramTuple2f.y < paramFloat)
                this.y = paramFloat;
            else
                this.y = paramTuple2f.y;
        }

        public void clampMax(float paramFloat, Tuple2f paramTuple2f)
        {
            if (paramTuple2f.x > paramFloat)
                this.x = paramFloat;
            else
            {
                this.x = paramTuple2f.x;
            }

            if (paramTuple2f.y > paramFloat)
                this.y = paramFloat;
            else
                this.y = paramTuple2f.y;
        }

        public void absolute(Tuple2f paramTuple2f)
        {
            this.x = Math.Abs(paramTuple2f.x);
            this.y = Math.Abs(paramTuple2f.y);
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
                this.y = paramFloat1;
        }

        public void clampMin(float paramFloat)
        {
            if (this.x < paramFloat)
                this.x = paramFloat;
            if (this.y >= paramFloat)
                return;
            this.y = paramFloat;
        }

        public void clampMax(float paramFloat)
        {
            if (this.x > paramFloat)
                this.x = paramFloat;
            if (this.y <= paramFloat)
                return;
            this.y = paramFloat;
        }

        public void absolute()
        {
            this.x = Math.Abs(this.x);
            this.y = Math.Abs(this.y);
        }

        public void interpolate(Tuple2f paramTuple2f1, Tuple2f paramTuple2f2, float paramFloat)
        {
            this.x = ((1.0F - paramFloat) * paramTuple2f1.x + paramFloat * paramTuple2f2.x);
            this.y = ((1.0F - paramFloat) * paramTuple2f1.y + paramFloat * paramTuple2f2.y);
        }

        public void interpolate(Tuple2f paramTuple2f, float paramFloat)
        {
            this.x = ((1.0F - paramFloat) * this.x + paramFloat * paramTuple2f.x);
            this.y = ((1.0F - paramFloat) * this.y + paramFloat * paramTuple2f.y);
        }

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
    }
}
