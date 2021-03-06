﻿using System;
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

        public Position(float x=0f, float y=0f, float z=0f)
        {
            this.X = x;
            this.Y = y;
            this.Z = z;
        }

        public Position(Position Position)
        {
            if(Position != null)
            {
                this.X = Position.X;
                this.Y = Position.Y;
                this.Z = Position.Z;
            }
        }

        public Position(Size Size)
        {
            if (Size != null)
            {
                this.X = Size.Width;
                this.Y = Size.Height;
                this.Z = Size.Depth;
            }
        }

        public Position Plus(Position B)
        {
            if (B == null) return this;
            this.X += B.X;
            this.Y += B.Y;
            this.Z += B.Z;
            return this;
        }

        public Position Minus(Position B)
        {
            if (B == null) return this;
            this.X -= B.X;
            this.Y -= B.Y;
            this.Z -= B.Z;
            return this;
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
    }

    public class Size
    {
        public float Width;
        public float Height;
        public float Depth;

        public Size()
        { }

        public Size(float width, float height, float depth=0f)
        {
            this.Width = width;
            this.Height = height;
            this.Depth = depth;
        }

        public Size(Size Size)
        {
            if (Size != null)
            {
                this.Width = Size.Width;
                this.Height = Size.Height;
                this.Depth = Size.Depth;
            }
        }

        public Size(Position Position)
        {
            if (Position != null)
            {
                this.Width = Position.X;
                this.Height = Position.Y;
                this.Depth = Position.Z;
            }
        }

        public static Size Plus(Size A, Size B)
        {
            if (A == null && B == null) return null;
            if (A == null) return new Size(B);
            if (B == null) return new Size(A);
            return new Size(A).Plus(B);
        }

        public static Size Minus(Size A, Size B)
        {
            if (A == null && B == null) return null;
            if (A == null) return new Size(B);
            if (B == null) return new Size(A);
            return new Size(A).Minus(B);
        }

        public Size Plus(Size Size)
        {
            if (Size == null) return this;
            this.Width += Size.Width;
            this.Height += Size.Height;
            this.Depth += Size.Depth;
            return this;
        }

        public Size Minus(Size Size)
        {
            if (Size == null) return this;
            this.Width -= Size.Width;
            this.Height -= Size.Height;
            this.Depth -= Size.Depth;
            return this;
        }

        public Size Plus(Position Position)
        {
            if (Position == null) return this;
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
