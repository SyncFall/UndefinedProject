using Bee.Lib;
using Bee.Library;
using Bee.UI;
using DelaunayTriangulator;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.Integrator
{
    public class VisualView
    {
        public Surface Surface;
        public ActionSelect ActionSelect;
        public VisualInput VisualInput;
        public Point SelectPoint;
        public object LockObject = new object();

        public VisualView()
        {
            this.VisualInput = new VisualInput(this);
            this.Surface = new Surface();
        }

        public void Draw()
        {
            if(Surface.CurveRoot != null)
            {
                List<Vertex> Points = new List<Vertex>();
                Curve curveNode = Surface.CurveRoot;
                int curveCount = 0;
                while (curveNode != null)
                {
                    curveCount++;
                    int detail = curveNode.Detail;
                    int i = 0;
                    for (float t = 0, step = (1 / (float)detail); t <= 1; t += step, i++)
                    {
                        if (i == detail - 1)
                        {
                            t = .999999f;
                        }
                        Point point = curveNode.GetPoint(t);
                        Vertex vertex = new Vertex((int)point.x, (int)point.y);
                        if (!Points.Contains(vertex))
                        {
                            Points.Add(vertex);
                        }
                    }
                    curveNode = curveNode.Next;
                }

                if(curveCount > 1)
                {
                    Triangulator angulator = new Triangulator();
                    List<Triad> triangles = angulator.Triangulation(Points, true);

                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Line);
                    GL.LineWidth(2f);
                    GL.Color3(0f, 100 / 255f, 150 / 255f);
                    GL.Begin(PrimitiveType.Triangles);
                    for (int i = 0; i < triangles.Count; i++)
                    {
                        Triad triad = triangles[i];
                        GL.Vertex3(Points[triad.a].x, Points[triad.a].y, 0);
                        GL.Vertex3(Points[triad.b].x, Points[triad.b].y, 0);
                        GL.Vertex3(Points[triad.c].x, Points[triad.c].y, 0);
                    }
                    GL.End();
                    GL.PolygonMode(MaterialFace.FrontAndBack, PolygonMode.Fill);
                }
            }
            if (Surface != null)
            {
                Surface.Draw();
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
                    ActionCurve.Points.Get(1).x = x;
                    ActionCurve.Points.Get(1).y = y;
                    ActionCurve.BuildKnots();
                }
                if (ActionCurve.Type == CurveType.Quadratic && Event.IsCursor)
                {
                    ActionCurve.Points.Get(1).x = x;
                    ActionCurve.Points.Get(1).y = ActionCurve.Points.Get(0).y;
                    ActionCurve.Points.Get(2).x = x;
                    ActionCurve.Points.Get(2).y = y;
                    ActionCurve.BuildKnots();
                }
                if (ActionCurve.Type == CurveType.Cubic && Event.IsCursor)
                {
                    ActionCurve.Points.Get(1).x = ((ActionCurve.Points.Get(0).x + x) / 2);
                    ActionCurve.Points.Get(1).y = ActionCurve.Points.Get(0).y;
                    ActionCurve.Points.Get(2).x = x;
                    ActionCurve.Points.Get(2).y = ((ActionCurve.Points.Get(0).y + y) / 2);
                    ActionCurve.Points.Get(3).x = x;
                    ActionCurve.Points.Get(3).y = y;
                    ActionCurve.BuildKnots();
                }
                if (Event.IsButton && Event.Button.Type == Button.Left && Event.Button.IsClick)
                {
                    InProgress = false;
                    ActionCurve.Selected = true;
                    ActionCurve.Points.Last().Selected = true;
                }
                if (Event.IsCursor)
                {
                    Surface.UpdateIntersectStatus(Event.Cursor.x, Event.Cursor.y);
                    SelectPoint = null;
                    Curve curveNode = Surface.CurveRoot;
                    while (curveNode != null)
                    {
                        curveNode.Intersect = false;
                        for (int i = 0; i < curveNode.Points.Size(); i++)
                        {
                            CurvePoint curvePoint = curveNode.Points.Get(i);
                            if(curvePoint.Intersect)
                            {
                                SelectPoint = curvePoint;
                                break;
                            }
                        }
                        curveNode = curveNode.Next;
                    }
                    Surface.UpdateIntersectStatus(-1, -1);
                }
                if (Event.IsKey && Event.Key.Type == Key.Escape && Event.Key.IsClick)
                {
                    Surface.RemoveCurve(ActionCurve);
                    InProgress = false;
                }
            }
            else if(!InProgress)
            {
                if (Event.IsButton && Event.Button.Type == Button.Left && Event.Button.IsClick)
                {
                    Curve curveNode = Surface.CurveRoot;
                    while (curveNode != null)
                    {
                        curveNode.Selected = (curveNode.Intersect || curveNode.Points.Intersect);
                        for (int i = 0; i < curveNode.Points.Size(); i++)
                        {
                            CurvePoint curvePoint = curveNode.Points.Get(i);
                            curvePoint.Selected = (curvePoint.Intersect);
                        }
                        curveNode = curveNode.Next;
                    }
                }
                if (Event.IsCursor)
                {
                    Surface.UpdateIntersectStatus(Event.Cursor.x, Event.Cursor.y);
                    if (Input.Mouse.Buttons[Button.Left].IsDown)
                    {
                        CurvePoint selectedCurvePoint = null;
                        Curve curveNode = Surface.CurveRoot;
                        while (curveNode != null)
                        {
                            for (int i = 0; i < curveNode.Points.Size(); i++)
                            {
                                CurvePoint curvePoint = curveNode.Points.Get(i);
                                if (curvePoint.Selected)
                                {
                                    curvePoint.x = Event.Cursor.x;
                                    curvePoint.y = Event.Cursor.y;
                                    selectedCurvePoint = curvePoint;
                                }
                            }
                            curveNode = curveNode.Next;
                        }
                        if(selectedCurvePoint != null)
                        {
                            curveNode = Surface.CurveRoot;
                            while (curveNode != null)
                            {
                                for (int i = 0; i < curveNode.Points.Size(); i++)
                                {
                                    CurvePoint curvePoint = curveNode.Points.Get(i);
                                    if (curvePoint.Intersect)
                                    {
                                        selectedCurvePoint.x = curvePoint.x;
                                        selectedCurvePoint.y = curvePoint.y;
                                    }
                                }
                                curveNode = curveNode.Next;
                            }
                        }
                    }
                }
                if (Event.IsKey && Event.Key.Type == Key.Delete && Event.Key.IsClick)
                {
                    Curve curveNode = Surface.CurveRoot;
                    while (curveNode != null)
                    {
                        if (curveNode.Selected)
                        {
                            Surface.RemoveCurve(curveNode);
                        }
                        curveNode = curveNode.Next;
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
                ActionCurve = Surface.AddCurve(CurveType.Line, 2);
                ActionCurve.AddPoint(x, y);
                ActionCurve.AddPoint(x, y);
            }
            else if(!InProgress && ActionButton.Type == ActionButtonType.QuadraticPath)
            {
                ActionCurve = Surface.AddCurve(CurveType.Quadratic, 20);
                ActionCurve.AddPoint(x, y);
                ActionCurve.AddPoint(x, y);
                ActionCurve.AddPoint(x, y);
            }
            else if(!InProgress && ActionButton.Type == ActionButtonType.CubicPath)
            {
                ActionCurve = Surface.AddCurve(CurveType.Cubic, 28);
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
                    CurvePoint selectedCurvePoint = null;
                    Curve curveNode = VisualView.Surface.CurveRoot;
                    while (curveNode != null)
                    {
                        for (int i = 0; i < curveNode.Points.Size(); i++)
                        {
                            CurvePoint curvePoint = curveNode.Points.Get(i);
                            if (curvePoint.Selected)
                            {
                                selectedCurvePoint = curvePoint;
                                break;
                            }
                        }
                        curveNode = curveNode.Next;
                    }
                    float x = (selectedCurvePoint != null ? selectedCurvePoint.x : Mouse.Cursor.x);
                    float y = (selectedCurvePoint != null ? selectedCurvePoint.y : Mouse.Cursor.y);
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
                /*
                StringBuilder strBuilder = new StringBuilder();
                Curve curveNode = VisualView.Surface.CurveRoot;
                while(curveNode != null)
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
                        strBuilder.AppendLine((point.x + ":" + point.y + ":" + point.z).Replace(',', '.'));
                    }
                    if (curveNode.Next == null)
                    {
                        break;
                    }
                    curveNode = curveNode.Next;
                }
                */
               
                StringBuilder strBuilder = new StringBuilder();
                Curve curveNode = VisualView.Surface.CurveRoot;
                while (curveNode != null)
                {
                    strBuilder.AppendLine((int)curveNode.AnchorBegin.x+","+ (int)curveNode.AnchorBegin.y+"|"+ (int)curveNode.AnchorEnd.x+","+(int)curveNode.AnchorEnd.y);
                    curveNode = curveNode.Next;
                }
                File.WriteAllText("D:\\dev\\EclipseJavaWorkspace2\\tri\\bin\\points.txt", strBuilder.ToString());
            }
            VisualView.InputEvent(Event);   
        }
    }

}
