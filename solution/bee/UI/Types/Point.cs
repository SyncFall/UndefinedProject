using Bee.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI
{
    public class Point
    {
        public float x;
        public float y;
        public float z;

        public Point()
        { }

        public Point(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }

        public Point(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public class PointList : ListCollection<Point>
    { }

    public class Size
    {
        public float Width;
        public float Height;
        public float Depth;

        public Size()
        { }

        public Size(float width, float height)
        {
            this.Width = width;
            this.Height = height;
        }
    }
}
