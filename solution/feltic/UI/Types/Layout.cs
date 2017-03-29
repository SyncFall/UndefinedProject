using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.UI
{
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

        public static bool IsValid(Size Size)
        {
            if(Size == null) return false;
            return (Size.Width != -1f && Size.Height != -1f);
        }
    }
}
