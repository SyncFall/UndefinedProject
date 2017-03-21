using feltic.UI;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Integrator
{
    public class CodeColor
    {
        public static readonly Color Normal = new Color(220, 220, 220);
        public static readonly Color Keyword = new Color( 34, 147, 178);
        public static readonly Color Comment = new Color( 83, 178, 94);
        public static readonly Color Region = new Color(155, 155, 155);
        public static readonly Color Error = new Color(255, 83, 81);
        public static readonly Color String = new Color(255, 169, 97);
    }
}
