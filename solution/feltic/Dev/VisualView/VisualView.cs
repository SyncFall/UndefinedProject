using feltic.Lib;
using feltic.Library;
using feltic.Visual;
using DelaunayTriangulator;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Integrator
{
    public class VisualView
    {
        public LineCurve Surface;
        public ActionSelect ActionSelect;
        public VisualInput VisualInput;
        public Point SelectPoint;
        public object LockObject = new object();

        public VisualView()
        {
            this.VisualInput = new VisualInput(this);
            this.Surface = new LineCurve();
        }

        public void Draw()
        {
            List<Vertex> Points = new List<Vertex>();
            for(int i=0; i<Surface.Curves.Size; i++)
            {
                Curve curve = Surface.Curves[i];
                int x = 0;
                for (float t = 0, step = (1 / (float)curve.Detail); t <= 1; t += step, x++)
                {
                    if (x == curve.Detail-1)
                    {
                        t = .999999f;
                    }
                    Point point = curve.GetPoint(t);
                    Vertex pointVertex = new Vertex((int)point.x, (int)point.y);
                    if(!Points.Contains(pointVertex))
                    { 
                        Points.Add(pointVertex);
                    }
                }
            }

            if(Points.Count >= 3)
            {
                Triangulator angulator = new Triangulator();
                List<Triad> triangles = angulator.Triangulation(Points, false);

                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                GL.LineWidth(2f);
                GL.Color3(0f, 100 / 255f, 150 / 255f);
                GL.Begin(PrimitiveType.Triangles);
                for (int i = 0; i < triangles.Count; i++)
                {
                    Triad triad = triangles[i];
                    GL.Vertex2(Points[triad.a].x, Points[triad.a].y);
                    GL.Vertex2(Points[triad.b].x, Points[triad.b].y);
                    GL.Vertex2(Points[triad.c].x, Points[triad.c].y);
                }
                GL.End();
                GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
            }
         
            CurvePointList intersectPoints = new CurvePointList();
            for (int i = 0; i < Surface.Curves.Size; i++)
            {
                for (int j = 0; j < Surface.Curves.Size; j++)
                {
                    if (i == j)
                    {
                        continue;
                    }
                    CurvePointList curveIntersecPoints = Surface.Curves[i].IntersectCurve(Surface.Curves[j]);
                    intersectPoints.AddAll(curveIntersecPoints);
                }
            }

            if (Surface != null)
            {
                Surface.Draw();
            }

            if (intersectPoints.Size > 0)
            {
                GL.PointSize(10f);
                GL.Color3(1f, 0f, 0f);
                GL.Begin(PrimitiveType.Points);
                for (int i = 0; i < intersectPoints.Size; i++)
                {
                    Point point = intersectPoints[i];
                    GL.Vertex2(point.x, point.y);
                }
                GL.End();
            }

            if (ActionSelect != null)
            {
                ActionSelect.Draw();
            }
        }

        public void InputEvent(InputEvent Event)
        {
            if(InProgress)
            {
                float x = (SelectPoint != null ? SelectPoint.x : Input.Mouse.Cursor.x);
                float y = (SelectPoint != null ? SelectPoint.y : Input.Mouse.Cursor.y);
                if (ActionCurve.Type == CurveType.Line && Event.IsCursor)
                {
                    ActionCurve.Points[1].x = x;
                    ActionCurve.Points[1].y = y;
                    ActionCurve.BuildKnots();
                }
                if (ActionCurve.Type == CurveType.Quadratic && Event.IsCursor)
                {
                    ActionCurve.Points[1].x = x;
                    ActionCurve.Points[1].y = ActionCurve.Points.First.y;
                    ActionCurve.Points[2].x = x;
                    ActionCurve.Points[2].y = y;
                    ActionCurve.BuildKnots();
                }
                if (ActionCurve.Type == CurveType.Cubic && Event.IsCursor)
                {
                    ActionCurve.Points[1].x = ((ActionCurve.Points.First.x + x) / 2);
                    ActionCurve.Points[1].y = ActionCurve.Points.Get(0).y;
                    ActionCurve.Points[2].x = x;
                    ActionCurve.Points[2].y = ((ActionCurve.Points.First.y + y) / 2);
                    ActionCurve.Points[3].x = x;
                    ActionCurve.Points[3].y = y;
                    ActionCurve.BuildKnots();
                }
                if (Event.IsButton && Event.Button.Type == Button.Left && Event.Button.IsClick)
                {
                    InProgress = false;
                    ActionCurve.Selected = true;
                    ActionCurve.Points.Last.Selected = true;
                }
                if (Event.IsCursor)
                {
                    Surface.UpdateIntersectStatus(Event.Cursor.x, Event.Cursor.y);
                    SelectPoint = null;
                    for(int i=0; i<Surface.Curves.Size; i++)
                    {
                        Curve curve = Surface.Curves[i];
                        curve.Intersect = false;
                        for (int j = 0; j < curve.Points.Size; j++)
                        {
                            CurvePoint curvePoint = curve.Points[j];
                            if (curvePoint.Intersect)
                            {
                                SelectPoint = curvePoint;
                                break;
                            }
                        }
                    }
                    Surface.UpdateIntersectStatus(-1, -1);
                }
                if (Event.IsKey && Event.Key.Type == Key.Escape && Event.Key.IsClick)
                {
                    Surface.Curves.Remove(ActionCurve);
                    InProgress = false;
                }
            }
            else if(!InProgress)
            {
                if (Event.IsButton && Event.Button.Type == Button.Left && Event.Button.IsClick)
                {
                    for(int i=0; i<Surface.Curves.Size; i++)
                    {
                        Curve curve = Surface.Curves[i];
                        curve.Selected = (curve.Intersect || curve.Points.Intersect);
                        for (int j = 0; j < curve.Points.Size; j++)
                        {
                            CurvePoint point = curve.Points[j];
                            point.Selected = (point.Intersect);
                        }
                    }
                }
                if (Event.IsCursor)
                {
                    Surface.UpdateIntersectStatus(Event.Cursor.x, Event.Cursor.y);
                    if (Input.Mouse.Buttons[Button.Left].IsDown)
                    {
                        CurvePoint selectedPoint = null;
                        for(int i=0; i<Surface.Curves.Size; i++)
                        {
                            Curve curve = Surface.Curves[i];
                            for (int j = 0; j < curve.Points.Size; j++)
                            {
                                CurvePoint point = curve.Points[j];
                                if (point.Selected)
                                {
                                    point.x = Event.Cursor.x;
                                    point.y = Event.Cursor.y;
                                    selectedPoint = point;
                                }
                            }
                        }
                        if(selectedPoint != null)
                        {
                            for (int i = 0; i < Surface.Curves.Size; i++)
                            {
                                Curve curve = Surface.Curves[i];
                                for (int j = 0; j < curve.Points.Size; j++)
                                {
                                    CurvePoint point = curve.Points[j];
                                    if (point.Selected)
                                    {
                                        selectedPoint.x = point.x;
                                        selectedPoint.y = point.y;
                                    }
                                }
                            }
                        }
                    }
                }
                if (Event.IsKey && Event.Key.Type == Key.Delete && Event.Key.IsClick)
                {
                    for (int i = 0; i < Surface.Curves.Size; i++)
                    {
                        Curve curve = Surface.Curves[i];
                        if (curve.Selected)
                        {
                            Surface.Curves.RemoveAt(i);
                        }
                    }
                }
            }
        }

        public bool InProgress = false;
        public Curve ActionCurve = null;

        public void ActionEvent(ActionButton ActionButton)
        {
            float x = ActionSelect.Point.x;
            float y = ActionSelect.Point.y;
            if (!InProgress && ActionButton.Type == ActionButtonType.LinePath)
            {
                ActionCurve = Surface.AddSegment(CurveType.Line, 2);
                ActionCurve.AddPoint(x, y);
                ActionCurve.AddPoint(x, y);
            }
            else if(!InProgress && ActionButton.Type == ActionButtonType.QuadraticPath)
            {
                ActionCurve = Surface.AddSegment(CurveType.Quadratic, 20);
                ActionCurve.AddPoint(x, y);
                ActionCurve.AddPoint(x, y);
                ActionCurve.AddPoint(x, y);
            }
            else if(!InProgress && ActionButton.Type == ActionButtonType.CubicPath)
            {
                ActionCurve = Surface.AddSegment(CurveType.Cubic, 28);
                ActionCurve.AddPoint(x, y);
                ActionCurve.AddPoint(x, y);
                ActionCurve.AddPoint(x, y);
                ActionCurve.AddPoint(x, y);
            }
            else
            {
                return;
            }
            ActionCurve.BuildKnots();
            InProgress = true;
            ActionSelect.Dispose();
            ActionSelect = null;
        }
    }

    public class VisualInput : InputListener
    {
        public VisualView VisualView;

        public VisualInput(VisualView VisualView)
        {
            this.VisualView = VisualView;
        }

        public override void Input(InputEvent Event)
        {
            if (Event.IsKey && Event.Key.Type == Key.Space && Event.Key.IsDown)
            {
                if(VisualView.InProgress)
                {
                    ;
                }
                else if(VisualView.ActionSelect == null)
                {             
                    CurvePoint selectedPoint = null;
                    for(int i=0; i<VisualView.Surface.Curves.Size; i++)
                    {
                        Curve curve = VisualView.Surface.Curves[i];
                        for (int j = 0; j < curve.Points.Size; j++)
                        {
                            CurvePoint point = curve.Points[j];
                            if (point.Selected)
                            {
                                selectedPoint = point;
                                break;
                            }
                        }
                    }
                    float x = (selectedPoint != null ? selectedPoint.x : Mouse.Cursor.x);
                    float y = (selectedPoint != null ? selectedPoint.y : Mouse.Cursor.y);
                    VisualView.ActionSelect = new ActionSelect(VisualView, new Point(x, y));
                }
                else
                {
                    VisualView.ActionSelect.Dispose();
                    VisualView.ActionSelect = null;
                }
            
            }
            if(Event.IsKey && Event.Key.Type == Key.S && Event.Key.IsClick && Keyboard.Keys[Key.ControlLeft].IsDown)
            {
                ;
            }
            VisualView.InputEvent(Event);   
        }
    }

}
