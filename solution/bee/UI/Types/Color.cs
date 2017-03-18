using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.UI
{
    public class Color
    {
        public int R;
        public int G;
        public int B;

        public Color(int r, int g, int b)
        {
            this.R = r;
            this.G = g;
            this.B = b;
        }

        public GlColor GetGlColor()
        {
            return new GlColor(this);
        }
    }

    public class GlColor
    {
        public float[] Rgb;

        public GlColor(Color color)
        {
            this.Rgb = new float[] { color.R/255f, color.G/255f, color.B/255f };
        }

        public GlColor(float r, float g, float b)
        { 
            this.Rgb = new float[] { r, g, b };
        }
    }
}
