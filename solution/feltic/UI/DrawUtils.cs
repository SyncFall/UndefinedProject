using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.UI
{
    public class DrawUtils
    {
        public static void DrawBox()
        {
            // White side - FRONT
            GL.Begin(PrimitiveType.Polygon);
                GL.Color3(System.Drawing.Color.LawnGreen);
                GL.Vertex3(-0.5, -0.5, -0.5);
                GL.Vertex3(-0.5, 0.5, -0.5);
                GL.Vertex3(0.5, 0.5, -0.5);
                GL.Vertex3(0.5, -0.5, -0.5);
            GL.End();

            // White side - BACK
            GL.Begin(PrimitiveType.Polygon);
                GL.Color3(System.Drawing.Color.Aqua);
                GL.Vertex3(0.5, -0.5, 0.5);
                GL.Vertex3(0.5, 0.5, 0.5);
                GL.Vertex3(-0.5, 0.5, 0.5);
                GL.Vertex3(-0.5, -0.5, 0.5);
            GL.End();

            // Purple side - RIGHT
            GL.Begin(PrimitiveType.Polygon);
                GL.Color3(System.Drawing.Color.Beige);
                GL.Vertex3(0.5, -0.5, -0.5);
                GL.Vertex3(0.5, 0.5, -0.5);
                GL.Vertex3(0.5, 0.5, 0.5);
                GL.Vertex3(0.5, -0.5, 0.5);
            GL.End();

            // Green side - LEFT
            GL.Begin(PrimitiveType.Polygon);
                GL.Color3(System.Drawing.Color.Lavender);
                GL.Vertex3(-0.5, -0.5, 0.5);
                GL.Vertex3(-0.5, 0.5, 0.5);
                GL.Vertex3(-0.5, 0.5, -0.5);
                GL.Vertex3(-0.5, -0.5, -0.5);
            GL.End();

            // Blue side - TOP
            GL.Begin(PrimitiveType.Polygon);
                GL.Color3(System.Drawing.Color.Coral);
                GL.Vertex3(0.5, 0.5, 0.5);
                GL.Vertex3(0.5, 0.5, -0.5);
                GL.Vertex3(-0.5, 0.5, -0.5);
                GL.Vertex3(-0.5, 0.5, 0.5);
            GL.End();

            // Red side - BOTTOM
            GL.Begin(PrimitiveType.Polygon);
                GL.Color3(System.Drawing.Color.Cyan);
                GL.Vertex3(0.5, -0.5, -0.5);
                GL.Vertex3(0.5, -0.5, 0.5);
                GL.Vertex3(-0.5, -0.5, 0.5);
                GL.Vertex3(-0.5, -0.5, -0.5);
            GL.End();
        }

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
