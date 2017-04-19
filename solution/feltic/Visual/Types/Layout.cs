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

        public Position(float x = 0f, float y=0f, float z = 0f)
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

        public Position Plus(Position B)
        {
            if (B == null) return null;
            this.X += B.X;
            this.Y += B.Y;
            this.Z += B.Z;
            return this;
        }

        public Position Minus(Position B)
        {
            if (B == null) return null;
            this.X -= B.X;
            this.Y -= B.Y;
            this.Z -= B.Z;
            return this;
        }

        public bool GreaterAny(Position B)
        {
            if (B == null) return false;
            return (B.X > X || B.Y > Y || B.Z > Z);
        }

        public static Position Plus(Position A, Position B)
        {
            if (A == null && B == null) return null;
            if (A == null) return new Position(B);
            if (B == null) return new Position(A);
            return new Position(A).Plus(B); 
        }

        public static Position Minus(Position A, Position B)
        {
            if (A == null && B == null) return null;
            if (A == null) return new Position(B);
            if (B == null) return new Position(A);
            return new Position(A).Minus(B);
        }

        public static bool GreaterAny(Position A, Position B)
        {
            if (A == null || B == null) return false;
            return A.GreaterAny(B);
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


        public static Size Plus(Size A, Size B)
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

        public Size Plus(Position Position)
        {
            this.Width += Position.X;
            this.Height += Position.Y;
            this.Depth += Position.Z;
            return this;
        }

        public Size Minus(Position Position)
        {
            if (Position == null) return this;
            this.Width -= Position.X;
            this.Height -= Position.Y;
            this.Depth -= Position.Z;
            return this;
        }
    }
}
