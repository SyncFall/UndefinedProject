using Bee.Library;
using OpenTK;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI
{
    public class LineDraw
    {
        public WindowType WindowType;
        public ListCollection<Vector2> Points = new ListCollection<Vector2>();

        public LineDraw(WindowType Window)
        {
            this.WindowType = Window;
            this.WindowType.Mouse.AddListener(new LineDrawMouseListener(this));
        }

        public void Draw()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, WindowType.Width, WindowType.Height, 0, 0, 1);

            GL.LineWidth(2.5f);
            GL.Color3(1.0f, 1.0f, 1.0f);

            GL.Begin(PrimitiveType.Lines);
            for (int i = 0; i < Points.Size() - 1; i++)
            {
                GL.Vertex2(Points.Get(i).X, Points.Get(i).Y);
                GL.Vertex2(Points.Get(i + 1).X, Points.Get(i + 1).Y);
            }
            GL.End();
        }
    }

    public class LineDrawMouseListener : MouseListener
    {
        public LineDraw LineDraw;

        public LineDrawMouseListener(LineDraw LineDraw)
        {
            this.LineDraw = LineDraw;
        }

        public override void MouseEvent(MouseResult Result)
        {
            if(Result.Type == MouseType.BUTTON_EVENT)
            {
                if (Result.Button.Key == ButtonKey.LEFT && Result.Button.Event == ButtonEvent.DOWN)
                {
                    LineDraw.Points.Add(new Vector2(Result.Cursor.X, Result.Cursor.Y));
                }
                else if (Result.Button.Key == ButtonKey.RIGHT && Result.Button.Event == ButtonEvent.DOWN)
                {
                    if (LineDraw.Points.Size() > 0)
                    {
                        LineDraw.Points.RemoveAt(LineDraw.Points.Size() - 1);
                    }
                }
            }

        }
    }
}
