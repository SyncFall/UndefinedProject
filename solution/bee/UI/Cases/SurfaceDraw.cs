using Bee.UI.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bee.UI.Triangulation;
using System.IO;
using Bee.Library;

namespace Bee.UI
{
    public class SurfaceDraw
    {
        public FpsCounter FpsCounter;
        public Text Text;
        public Surface Surface;
        public Triangulator Triangulator;
        ListCollection<Point3f> Points = new ListCollection<Point3f>();

        public SurfaceDraw()
        {
            FpsCounter = new FpsCounter();
            Text = new Text("fuckyou\nTOO!", new TextFormat());
            Text.Input = new TextInputListener();

            Surface = new Surface();
            //Surface.AddPath(PathType.Quadratic, 10);
            Surface.AddCurve(CurveType.Cubic, 5);

            StringBuilder strBuilder = new StringBuilder();
            Curve curveNode = Surface.CurveRoot;
            while(true)
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
                    strBuilder.AppendLine((point.x + ":" + point.y + ":" + point.z).Replace(',', '.'));
                }
                if(curveNode.Next == null)
                {
                    break;
                }
                curveNode = curveNode.Next;
            }

            File.WriteAllText("D:\\dev\\EclipseJavaWorkspace2\\tri\\bin\\points.txt", strBuilder.ToString());
            
            /*
            StringBuilder strBuilder = new StringBuilder();
            int detail = 40;
            Points = new Point3f[detail];
            int i = 0;
            for(float t=0, step=(1/(float)detail); t<=1; t += step, i++)
            {
                if(i == detail - 1)
                {
                    t = .999999f;
                }
                Point point = Surface.CurvePath.CurveNode.GetPoint(t);
                Points[i] = new Point3f(point.x, point.y, point.z);
                strBuilder.AppendLine((point.x + ":" + point.y + ":" + point.z).Replace(',', '.'));
            }
            //File.WriteAllText("D:\\dev\\EclipseJavaWorkspace2\\tri\\bin\\points.txt", strBuilder.ToString());
            Triangulator = new Triangulator();
            Triangulator.triangulate(Points);
            */
        }

        public void Draw()
        {
            //Text.Draw();
            
            FpsCounter.Draw();

            Points.Clear();
            Curve curveNode = Surface.CurveRoot.Next;
            while (true)
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
                if (curveNode.Next == null)
                {
                    break;
                }
                curveNode = curveNode.Next;
            }

            Triangulator = new Triangulator();
            Triangulator.triangulate(Points.ToArray());

            Point3f[] trianglePoints = new Point3f[Triangulator.TrianglePointIndexArray.Length];
            for(int i=0; i< trianglePoints.Length; i++)
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
            for (int i=0; i< trianglePoints.Length; i++)
            {
                Point3f point = trianglePoints[i];
                if (i % 3 == 1)
                    GL.Color3(1f, 1f, 1f);
                else if (i % 3 == 2)
                    GL.Color3(.5f, .5f, .5f);
                else
                    GL.Color3(0f, 0f, 0f);
                GL.Color3(0f, 100/255f, 150/255f);
                GL.Vertex3(point.x, point.y, point.z);
            }
            GL.End();
            GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);

            Surface.Draw();
        }
    }

    public class TextInputListener : InputListener
    {
        public override void Input(InputEvent Event)
        {
            if(Event.IsButton && Event.Button.IsDown)
            {
                (Sender as Text).String += "A";
            }
        }
    }
}
