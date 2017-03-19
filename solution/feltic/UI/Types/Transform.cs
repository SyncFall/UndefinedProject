using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.UI.Types
{
    public enum TransformType
    {
        Scale,
        Rotate,
        Translate,
        Perspective,
    }

    public class Transform
    {
        public Scale Scale;
        public Translate Translate;
    }

    public class Scale
    {
        public float x;
        public float y;
        public float z;

        public Scale(float x=0, float y=0, float z=0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }

    public class Translate
    {
        public float x;
        public float y;
        public float z;

        public Translate(float x = 0, float y = 0, float z = 0)
        {
            this.x = x;
            this.y = y;
            this.z = z;
        }
    }
}
