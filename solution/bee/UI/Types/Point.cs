using feltic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.UI
{
    public class Point
    {
        public float x;
        public float y;
        public float z;

        public Point()
        { }

        public Point(float x, float y, float z=0)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }

        public Point Copy()
        {
            return new Point(x, y, z);
        }
    }

    public class PointList : ListCollection<Point>
    {
        public void Add(float x, float y, float z=0)
        {
            base.Add(new Point(x, y, z));
        }
    }
}
