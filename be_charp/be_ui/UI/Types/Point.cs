using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.UI.Types
{
    public struct BeePoint
    {
        public static readonly int SizeInBytes = 3 * 4;
        public float x;
        public float y;
        public float z;

        public BeePoint(float x, float y)
        {
            this.x = x;
            this.y = y;
            this.z = 0;
        }

        public BeePoint(float x, float y, float z)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
