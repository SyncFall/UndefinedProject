using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Visual
{
    public class Position
    {
        public float X;
        public float Y;
        public float Z;

        public Position(float x = 0, float y=0, float z = 0)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Position(Position Position)
        {
            if(Position == null)
            {
                this.X = 0f;
                this.Y = 0f;
                this.Z = 0f;
            }
            else
            {
                this.X = Position.X;
                this.Y = Position.Y;
                this.Z = Position.Z;
            }
        }

        public static Position Add(Position A, Position B)
        {
            if (A == null && B == null) return null;
            if (A == null) return B;
            if (B == null) return A;
            Position c = new Position(A);
            c.X += B.X;
            c.Y += B.Y;
            c.Z += B.Z;
            return c;
        }

        public static Position Minus(Position A, Position B)
        {
            if (A == null && B == null) return null;
            if (A == null) return B;
            if (B == null) return A;
            Position c = new Position(A);
            c.X -= B.X;
            c.Y -= B.Y;
            c.Z -= B.Z;
            return c;
        }
    }

    public class Size
    {
        public float Width;
        public float Height;
        public float Depth;

        public Size()
        { }

        public Size(Size Size)
        {
            if(Size == null)
            {
                Width = 0f;
                Height = 0f;
                Depth = 0f;
            }
            else
            {
                this.Width = Size.Width;
                this.Height = Size.Height;
                this.Depth = Size.Depth;
            }
        }

        public Size(float width, float height, float depth=0f)
        {
            this.Width = width;
            this.Height = height;
            this.Depth = depth;
        }


        public static Size Add(Size A, Size B)
        {
            if (A == null && B == null) return null;
            if (A == null) return B;
            if (B == null) return A;
            Size c = new Size(A);
            c.Width += B.Width;
            c.Height += B.Height;
            c.Depth += B.Depth;
            return c;
        }

        public static Size Minus(Size A, Size B)
        {
            if (A == null && B == null) return null;
            if (A == null) return B;
            if (B == null) return A;
            Size c = new Size(A);
            c.Width -= B.Width;
            c.Height -= B.Height;
            c.Depth -= B.Depth;
            return c;
        }
    }
}
