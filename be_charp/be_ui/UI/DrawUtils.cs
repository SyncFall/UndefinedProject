using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI
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
    }
}
