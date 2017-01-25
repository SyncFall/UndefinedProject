using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI
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
            this.Rgb = new float[] { 1f/color.R, 1f/color.G, 1f/color.B };
        }

        public GlColor(float r, float g, float b)
        { 
            this.Rgb = new float[] { r, g, b };
        }
    }
}
