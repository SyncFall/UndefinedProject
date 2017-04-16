using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Visual
{
    public class DrawUtils
    {
        public static void Rotate(float x, float y)
        {
            GL.Rotate(x, 1.0, 0.0, 0.0);
            GL.Rotate(y, 0.0, 1.0, 0.0);
        }

        public static void DrawTriangleCycle(float x, float y, float radius, int triangleCount=20)
        {
            float twicePi = 2.0f * (float)Math.PI;
            GL.Begin(PrimitiveType.TriangleFan);
            GL.Color3(1f, 1f, 1f);
            GL.Vertex2(x, y);
            GL.Color3(0f, 0f, 0f);
            Random random = new Random(255);
            for(int i = 0; i <= triangleCount; i++)
            {
                GL.Vertex2(
                    x + (radius * Math.Cos(i * twicePi / triangleCount)),
                    y + (radius * Math.Sin(i * twicePi / triangleCount))
                );
            }
            GL.End();
        }
    }
}
