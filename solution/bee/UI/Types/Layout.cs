using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.UI
{
    public enum LayoutType
    {
        Size,
        Absolute,
        Relative,
        Margin,
        Padding,
        Surface,
    }

    public class Layout
    {
        public Size Size;
        public Position Absolute;
        public Position Relative;
        public Spacing Margin;
        public Spacing Padding;

        public Layout()
        { }
    }

    public class Position
    {
        public float x;
        public float y;
        public float z;

        public Position(float x=0, float y=0, float z=0)
        {
            this.x = x;
            this.y = y;
            this.z = y;
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
    }

    public class Spacing
    {
        public float Top;
        public float Right;
        public float Bottom;
        public float Left;

        public Spacing()
        { }

        public Spacing(float top, float right, float bottom, float left)
        {
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
            this.Left = left;
        }
    }
}
