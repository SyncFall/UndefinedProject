using Bee.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI
{
    public class Vec3
    {
        public float x;
        public float y;
        public float z;

        public Vec3()
        { }

        public Vec3(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }

        public Vec3(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public class Vec3List : ListCollection<Vec3>
    { }
}
