using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feltic.Integrator
{
    public class CodeColor
    {
        public static readonly float[] Normal = new float[] { 220 / 255f, 220 / 255f, 220 / 255f };
        public static readonly float[] Keyword = new float[] { 34 / 255f, 147 / 255f, 178 / 255f };
        public static readonly float[] Comment = new float[] { 83 / 255f, 178 / 255f, 94 / 255f };
        public static readonly float[] Region = new float[] { 155 / 255f, 155 / 255f, 155 / 255f };
        public static readonly float[] Error = new float[] { 255 / 255f, 83 / 255f, 81 / 255f };
        public static readonly float[] String = new float[] { 255 / 255f, 169 / 255f, 97 / 255f };
    }
}
