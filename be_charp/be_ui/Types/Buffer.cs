using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.UI.Types
{
    public enum BeeBufferType
    {
        Vertex,
    }

    public class BeeBuffer
    {
        public int VertexId;
        public int ColorId;
        public BeePoint[] Points;
        public BeePoint[] Colors;

        public BeeBuffer(BeePoint[] Points, BeePoint[] Colors)
        {
            this.Points = Points;
            this.Colors = Colors;
        }

        public void Build()
        {
            VertexId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexId);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Points.Length * BeePoint.SizeInBytes), Points, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);

            ColorId = GL.GenBuffer();
            GL.BindBuffer(BufferTarget.ArrayBuffer, ColorId);
            GL.BufferData(BufferTarget.ArrayBuffer, (IntPtr)(Colors.Length * BeePoint.SizeInBytes), Colors, BufferUsageHint.StaticDraw);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }

        public void Draw()
        {
            GL.EnableClientState(ArrayCap.VertexArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexId);
            GL.VertexPointer(3, VertexPointerType.Float, BeePoint.SizeInBytes, IntPtr.Zero);

            GL.EnableClientState(ArrayCap.ColorArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, ColorId);
            GL.ColorPointer(3, ColorPointerType.Float, BeePoint.SizeInBytes, IntPtr.Zero);

            GL.DrawArrays(PrimitiveType.Triangles, 0, Points.Length);

            GL.DisableClientState(ArrayCap.VertexArray);
            GL.DisableClientState(ArrayCap.ColorArray);
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
        }
    }
}
