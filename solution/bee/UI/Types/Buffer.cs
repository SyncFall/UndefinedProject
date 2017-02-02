using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI.Types
{
    public enum BeeBufferType
    {
        Vertex,
    }

    public class BeeBuffer
    {
        public int VertexId;
        public int ColorId;
        public Vec3[] Points;
        public Vec3[] Colors;

        public BeeBuffer(Vec3[] Points, Vec3[] Colors)
        {
            this.Points = Points;
            this.Colors = Colors;
        }

        public void Build()
        {
            VertexId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexId);
            //GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Points.Length * 12), Points, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            ColorId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, ColorId);
            //GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Colors.Length * 12), Colors, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Draw()
        {
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexId);
            GL.VertexPointer(3, VertexPointerType.Float, 12, IntPtr.Zero);

            GL.EnableClientState(ArrayCap.ColorArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, ColorId);
            GL.ColorPointer(3, ColorPointerType.Float, 12, IntPtr.Zero);

            GL.DrawArrays(PrimitiveType.Triangles, 0, Points.Length);

            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.ColorArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
