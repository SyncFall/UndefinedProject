using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Integrator
{
    public class CodeColor
    {
        public static readonly float[] Normal = new float[] { 220 / 255f, 220 / 255f, 220 / 255f };
        public static readonly float[] Keyword = new float[] { 57 / 255f, 135 / 255f, 214 / 255f };
        public static readonly float[] Comment = new float[] { 87 / 255f, 166 / 255f, 74 / 255f };
        public static readonly float[] Region = new float[] { 155 / 255f, 155 / 255f, 155 / 255f };
        public static readonly float[] Error = new float[] { 162 / 255f, 49 / 255f, 44 / 255f };
        public static readonly float[] String = new float[] { 214 / 255f, 134 / 255f, 75 / 255f };
    }
}
