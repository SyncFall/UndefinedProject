using Bee.Lib;
using Bee.Library;
using Bee.UI;
using Bee.UI.Triangulation;
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
       
        public VisualView()
        {
            this.VisualInput = new VisualInput(this);
            this.Surface = new Surface();
        }

        public void Draw()
        {
            Triangulate.Draw(Surface);
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
            if(InProgress && CurrentCurve.Type == PathType.Line)
            {
                if(Event.IsCursor)
                {
                    CurrentCurve.Points.Get(1).x = Input.Mouse.Cursor.x;
                    CurrentCurve.Points.Get(1).y = Input.Mouse.Cursor.y;
                }
                if(Event.IsButton && Event.Button.Type == Button.Left && Event.Button.IsClick)
                {
                    InProgress = false;
                    ActionSelect = new ActionSelect(this, new Point(Input.Mouse.Cursor.x, Input.Mouse.Cursor.y));
                }
                CurrentCurve.BuildKnots();
            }
            else if (InProgress && CurrentCurve.Type == PathType.Quadratic)
            {
                if (Event.IsCursor)
                {
                    CurrentCurve.Points.Get(1).x = Input.Mouse.Cursor.x;
                    CurrentCurve.Points.Get(1).y = CurrentCurve.Points.Get(0).y;
                    CurrentCurve.Points.Get(2).x = Input.Mouse.Cursor.x;
                    CurrentCurve.Points.Get(2).y = Input.Mouse.Cursor.y;
                }
                if (Event.IsButton && Event.Button.Type == Button.Left && Event.Button.IsClick)
                {
                    InProgress = false;
                    ActionSelect = new ActionSelect(this, new Point(Input.Mouse.Cursor.x, Input.Mouse.Cursor.y));
                }
                CurrentCurve.BuildKnots();
            }
            else if (InProgress && CurrentCurve.Type == PathType.Cubic)
            {
                if (Event.IsCursor)
                {
                    CurrentCurve.Points.Get(1).x = ((CurrentCurve.Points.Get(0).x + Input.Mouse.Cursor.x) / 2);
                    CurrentCurve.Points.Get(1).y = CurrentCurve.Points.Get(0).y;
                    CurrentCurve.Points.Get(2).x = Input.Mouse.Cursor.x;
                    CurrentCurve.Points.Get(2).y = ((CurrentCurve.Points.Get(0).y + Input.Mouse.Cursor.y) / 2);
                    CurrentCurve.Points.Get(3).x = Input.Mouse.Cursor.x;
                    CurrentCurve.Points.Get(3).y = Input.Mouse.Cursor.y;
                }
                if (Event.IsButton && Event.Button.Type == Button.Left && Event.Button.IsClick)
                {
                    InProgress = false;
                    ActionSelect = new ActionSelect(this, new Point(Input.Mouse.Cursor.x, Input.Mouse.Cursor.y));
                }
                CurrentCurve.BuildKnots();
            }
            else
            {
                if (Event.IsCursor)
                {
                    Surface.UpdateIntersectState(Event.Cursor.x, Event.Cursor.y);
                }
                if (Event.IsButton && Event.Button.Type == Button.Left && Event.Button.IsClick)
                {
                    if (Surface.UpdateIntersectState(Input.Mouse.Cursor.x, Input.Mouse.Cursor.y))
                    {
                        for(int i=0; i<Surface.CurvesIntersected.Size(); i++)
                        {
                            MovePoints.Add(Surface.CurvesIntersected.Get(i).PointsIntersected);
                        }
                        MovePoints.SetSelected(true);
                    }
                }
                if(MovePoints != null && Event.IsButton && Event.Button.Type == Button.Left && Event.Button.IsUp)
                {
                    MovePoints.SetSelected(false);
                    MovePoints.Clear(); ;
                }
                if(MovePoints != null && Event.IsCursor)
                {
                    for (int i=0; i<MovePoints.Size(); i++)
                    {
                        CurvePoint point = MovePoints.Get(i);
                        point.x = Event.Cursor.x;
                        point.y = Event.Cursor.y;
                    }
                }

            }

        }

        public bool InProgress = false;
        public Curve CurrentCurve = null;
        public CurvePointList MovePoints = new CurvePointList(null);

        public void ActionEvent(ActionButton ActionButton)
        {
            if(!InProgress && ActionButton.Type == ActionButtonType.LinePath)
            {
                Curve Curve = Surface.AddPath(PathType.Line);
                Curve.Add(ActionSelect.Point.x, ActionSelect.Point.y);
                Curve.Add(Input.Mouse.Cursor.x, Input.Mouse.Cursor.y);
                CurrentCurve = Curve;
                InProgress = true;
            }
            else if(!InProgress && ActionButton.Type == ActionButtonType.QuadraticPath)
            {
                Curve Curve = Surface.AddPath(PathType.Quadratic);
                Curve.Add(ActionSelect.Point.x, ActionSelect.Point.y);
                Curve.Add(Input.Mouse.Cursor.x, Input.Mouse.Cursor.y);
                Curve.Add(Input.Mouse.Cursor.x, Input.Mouse.Cursor.y);
                CurrentCurve = Curve;
                InProgress = true;
            }
            else if (!InProgress && ActionButton.Type == ActionButtonType.CubicPath)
            {
                Curve Curve = Surface.AddPath(PathType.Cubic);
                Curve.Add(ActionSelect.Point.x, ActionSelect.Point.y);
                Curve.Add(Input.Mouse.Cursor.x, Input.Mouse.Cursor.y);
                Curve.Add(Input.Mouse.Cursor.x, Input.Mouse.Cursor.y);
                Curve.Add(Input.Mouse.Cursor.x, Input.Mouse.Cursor.y);
                CurrentCurve = Curve;
                InProgress = true;
            }
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
                if(VisualView.ActionSelect == null)
                {
                    VisualView.ActionSelect = new Integrator.ActionSelect(VisualView, new Point(Mouse.Cursor.x, Mouse.Cursor.y));
                }
                else
                {
                    VisualView.ActionSelect.Dispose();
                    VisualView.ActionSelect = null;
                }
            }
            if(Event.IsKey && Event.Key.Type == Key.S && Event.Key.IsClick && Keyboard.Keys[Key.ControlLeft].IsDown)
            {
                StringBuilder strBuilder = new StringBuilder();
                Curve curveNode = VisualView.Surface.CurvePath.CurveNodeBegin;
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
                File.WriteAllText("D:\\dev\\EclipseJavaWorkspace2\\tri\\bin\\points.txt", strBuilder.ToString());
            }
            VisualView.InputEvent(Event);
        }
    }

}
