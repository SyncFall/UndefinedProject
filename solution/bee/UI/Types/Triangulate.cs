using Bee.Library;
using Bee.UI.Triangulation;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI
{
    public class Triangulate
    {
        public static void Draw(Surface Surface)
        {
            ListCollection<Point3f> Points = new ListCollection<Point3f>();
            Curve curveNode = Surface.CurveRoot;
            while (curveNode != null)
            {

                int detail = curveNode.Detail;
                int i = 0;
                for (float t = 0, step = (1 / (float)detail); t <= 1; t += step, i++)
                {
                    if (i == detail - 1)
                    {
                        t = .999999f;
                    }
                    Point point = curveNode.GetPoint(t);
                    Points.Add(new Point3f(point.x, point.y, point.z));
                }
                curveNode = curveNode.Next;
            }
            if (Points.Size() < 3)
            {
                return;
            }

            Triangulator Triangulator = new Triangulator();
            Triangulator.triangulate(Points.ToArray());

            Point3f[] trianglePoints = new Point3f[Triangulator.TrianglePointIndexArray.Length];
            for (int i = 0; i < trianglePoints.Length; i++)
            {
                int pointIndex = Triangulator.TrianglePointIndexArray[i];
                Point3f point = Triangulator.vertices[pointIndex];
                trianglePoints[i] = point;
            }

            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
            //GL.Scale(3f, 5f, 5f);
            GL.Color3(0, 100 / 255f, 150 / 255f);
            GL.PointSize(2f);
            GL.LineWidth(2f);
            GL.Begin(PrimitiveType.Triangles);
            for (int i = 0; i < trianglePoints.Length; i++)
            {
                Point3f point = trianglePoints[i];
                if (i % 3 == 1)
                    GL.Color3(1f, 1f, 1f);
                else if (i % 3 == 2)
                    GL.Color3(.5f, .5f, .5f);
                else
                    GL.Color3(0f, 0f, 0f);
                GL.Color3(0f, 100 / 255f, 150 / 255f);
                GL.Vertex3(point.x, point.y, point.z);
            }
            GL.End();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
        }
    }
}
