using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Visual
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

        public static Color Try(string str)
        {
            int hex = 0;
            try
            {
                if (str.StartsWith("#"))
                {
                    hex = int.Parse(str.Substring(1), NumberStyles.HexNumber);
                }
            }
            catch(Exception e)
            { }
            int r = (hex & 0xFF0000) >> 16;
            int g = (hex & 0xFF00) >> 8;
            int b = (hex & 0xFF);
            return new Color(r, g, b);
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
