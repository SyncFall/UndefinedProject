using Bee.UI.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bee.UI.Triangulator;
using System.IO;

namespace Bee.UI
{
    public class CurveDraw
    {
        public FpsCounter FpsCounter;
        public Text Text;
        public Surface Surface;
        public Triangulator.Triangulator Triangulator;
        Point3f[] Points;

        public CurveDraw()
        {
            FpsCounter = new FpsCounter();
            Text = new Text("fuckyou\nTOO!", new TextFormat());
            Text.Input = new TextInputListener();

            Surface = new Surface(PathType.Curve);
            Surface.CurvePath.CurveNode.BuildKnotes();
            Surface.AddPath(PathType.Line);

            StringBuilder strBuilder = new StringBuilder();
            int detail = 20;
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
            File.WriteAllText("D:\\dev\\EclipseJavaWorkspace2\\tri\\bin\\points.txt", strBuilder.ToString());
            Triangulator = new UI.Triangulator.Triangulator();
            Triangulator.triangulate(Points);
        }

        public void Draw()
        {
            //Text.Draw();
            
            FpsCounter.Draw();

            Point3f[] trianglePoints = new Point3f[Triangulator.TrianglePointIndexArray.Length];
            for(int i=0; i< trianglePoints.Length; i++)
            {
                int pointIndex = Triangulator.TrianglePointIndexArray[i];
                Point3f point = Triangulator.vertices[pointIndex];
                trianglePoints[i] = point;
            }

            GL.Scale(5f, 5f, 5f);
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
                GL.Vertex3(point.x, point.y, point.z);
            }
            GL.End();

            Surface.Draw();
        }
    }

    public class TextInputListener : InputListener
    {
        public override void Input(InputEvent Event)
        {
            if(Event.Is(InputType.Button) && Event.Button.IsDown)
            {
                (Sender as Text).String += "A";
            }
        }
    }
}
