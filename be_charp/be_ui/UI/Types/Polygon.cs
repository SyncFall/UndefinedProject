using Be.Runtime.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.UI.Types
{
    public class Polygon
    {
        public ListCollection<BeePoint> Points = new ListCollection<BeePoint>();
        public PolygonTransform PolygonTransform;

        public Polygon()
        {
            PolygonTransform = new PolygonTransform(this);
        }

        public void MouseEvent(MouseState Result)
        {
            if(PolygonTransform != null)
            {
                PolygonTransform.MouseEvent(Result);
            }
        }

        public void Draw()
        {
            GL.LineWidth(2.5f);
            GL.Color3(System.Drawing.Color.White);
            GL.Begin(PrimitiveType.LineLoop);
            for (int i = 0; i < Points.Size(); i++)
            {
                GL.Vertex2(Points.Get(i).x, Points.Get(i).y);
            }
            GL.End();

            if (PolygonTransform != null)
            {
                PolygonTransform.Draw();
            }
        }
    }

    public class PolygonPointer
    {
        public Polygon Polygon;
        public BeePoint Point;
        public static readonly int MARGIN = 5;
        public static readonly int MOUSE_MARGIN_ADD = 5;
        public bool IsMouseOver;
        public bool IsActive;

        public PolygonPointer(Polygon Polygon)
        {
            this.Polygon = Polygon;
        }

        private bool IntersectMouse(BeePoint Point, int MouseX, int MouseY)
        {
            return GeometryUtils.IntersectPositionWithMargin((int)Point.x, (int)Point.y, MouseX, MouseY, MARGIN + MOUSE_MARGIN_ADD, MARGIN + MOUSE_MARGIN_ADD);
        }

        public void MouseEvent(BeePoint Point, MouseState Result)
        {
            /*
            if(Result.InputType == MouseType.CURSOR_EVENT)
            {
                bool intersectPoint = IntersectMouse(Point, Result.Cursor.X, Result.Cursor.Y);
                if(intersectPoint)
                {
                    IsMouseOver = true;
                    this.Point = Point;
                }
            }
            if (Result.InputType == MouseType.BUTTON_EVENT && Result.Button.Key == ButtonKey.LEFT && Result.Button.Event == ButtonEvent.DOWN)
            {
                IsActive = IsMouseOver;
            }
            else if (Result.InputType == MouseType.BUTTON_EVENT && Result.Button.Key == ButtonKey.LEFT && Result.Button.Event == ButtonEvent.UP)
            {
                IsActive = false;
            }
            if (Result.InputType == MouseType.CURSOR_EVENT && IsActive)
            {
                UpdatetDimension(Result.Cursor.X, Result.Cursor.Y);
            }
            */
        }

        public void UpdatetDimension(int MouseX, int MouseY)
        {
            Point.x = (float)MouseX;
            Point.y = (float)MouseY;
        }

        public void Draw()
        {
            if(/*Point == null||*/ (!IsMouseOver && !IsActive))
            {
                return;
            }
            GL.LineWidth(1.5f);
            if (IsActive)
            {
                GL.Color3(System.Drawing.Color.Black);
            }
            else if (IsMouseOver)
            {
                GL.Color3(System.Drawing.Color.LightGray);
            }
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex2(Point.x - MARGIN, Point.y - MARGIN);
            GL.Vertex2(Point.x + MARGIN, Point.y - MARGIN);
            GL.Vertex2(Point.x + MARGIN, Point.y + MARGIN);
            GL.Vertex2(Point.x - MARGIN, Point.y + MARGIN);
            GL.End();
        }
    }

    public class PolygonTransform
    {
        public Polygon Polygon;
        public PolygonPointer PolygonPointer;

        public PolygonTransform(Polygon Polygon)
        {
            this.Polygon = Polygon;
            this.PolygonPointer = new PolygonPointer(Polygon);
        }

        public void MouseEvent(MouseState Result)
        {
            for(int i=0; i<Polygon.Points.Size(); i++)
            {
                PolygonPointer.MouseEvent(Polygon.Points.Get(i), Result);
            }
        }

        public void Draw()
        {
            GL.PointSize(8);
            GL.Color3(System.Drawing.Color.Firebrick);
            GL.Begin(PrimitiveType.Points);
            for (int i = 0; i < Polygon.Points.Size(); i++)
            {
                GL.Vertex2(Polygon.Points.Get(i).x, Polygon.Points.Get(i).y);
            }
            GL.End();

            PolygonPointer.Draw();
        }
    }
}
