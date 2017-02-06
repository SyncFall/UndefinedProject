﻿using Bee.Library;
using Bee.UI.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI
{ 
    public class Curve
    {
        public static readonly int DefaultDetailIteration = 100;
        public PointList Points = new PointList();
        public ListCollection<float> Knots = new ListCollection<float>();
        public int Degree;
        public int Order;
        public CurveInput Input;

        public Curve(int Degree=2)
        {
            this.Degree = Degree;
            this.Order = Degree + 1;
            //this.Input = new CurveInput(this);
        }

        public void Add(float X, float Y, float Z=0)
        {
            this.Points.Add(new Point(X, Y, Z));
        }

        private void BuildKnotes()
        {
            Knots.Clear();
            int knotCount = (Points.Size() + Degree + 1);
            float uniformVal = (1 / (float)knotCount);
            float uniformSum = 0;
            for(int i=0; i<knotCount; i++)
            {
                Knots.Add(uniformSum);
                uniformSum += uniformVal;
            }
        }

        public Point GetPoint(float t)
        {
            float u = (1 - t) * Knots.Get(Order - 1) + t * Knots.Get(Points.Size());

            int i_plus_1 = 0;
            for (; Knots.Get(i_plus_1) <= u; i_plus_1++) { }
            int i_plus_1_minus_k = i_plus_1 - Order;

            Point point = new Point();
            for (int i = i_plus_1_minus_k; i < i_plus_1; i++)
            {
                float n = Blend(i, Order, u);
                point.x += Points.Get(i).x * n;
                point.y += Points.Get(i).y * n;
                point.z += Points.Get(i).z * n;
            }
            return point;
        }

        public float Blend(int i, int k, float u)
        {
            if(k == 1)
            {
                if(Knots.Get(i) <= u && u < Knots.Get(i+1))
                    return 1;
                else
                    return 0;
            }

            float d1 = (Knots.Get(i+k-1) - Knots.Get(i));
            float n1 = (u - Knots.Get(i));
            float s1 = 0;
            if(d1 != 0 && n1 != 0)
                s1 = n1 * Blend(i, k-1, u) / d1;

            float d2 = (Knots.Get(i+k) - Knots.Get(i+1));
            float n2 = (-u + Knots.Get(i+k));
            float s2 = 0;
            if(d2 != 0 && n2 != 0)
                s2 = n2 * Blend(i+1, k-1, u) / d2;

            return s1 + s2;
        }

        public void Draw(bool Withpoints=false)
        {
            if(Points.Size() < Degree + 1)
            {
                return;
            }
            BuildKnotes();
            GL.Color3(System.Drawing.Color.White);
            GL.LineWidth(2.5f);
            GL.Begin(PrimitiveType.LineStrip);
            for(float t=0; t<=1f; t += (1/(float)DefaultDetailIteration))
            {
                Point point = GetPoint(t);
                GL.Vertex2(point.x, point.y);
            }
            GL.End();
            if(Withpoints)
            {
                DrawPoints();
            }
        }

        public void DrawPoints()
        {
            GL.PointSize(15f);
            GL.Color3(System.Drawing.Color.Green);
            GL.Begin(PrimitiveType.Points);
            for(int i=0; i<Points.Size(); i++)
            {
                Point point = Points.Get(i);
                GL.Vertex2(point.x, point.y);
            }
            GL.End();
        }
    }

    public class CurveInput : InputListener
    {
        public Curve Curve;
        public Point MovePoint;

        public CurveInput(Curve Curve)
        {
            this.Curve = Curve;
        }

        public override void Input(InputEvent Event)
        {
            if(Event.Is(InputType.Button))
            {
                ButtonState buttonState = Event.Button;
                if(buttonState.Type == Button.Left && buttonState.IsClick)
                {
                    for(int i=0; i< Curve.Points.Size(); i++)
                    {
                        if (GeometryUtils.IntersectPositionWithMargin((int)Curve.Points.Get(i).x, (int)Curve.Points.Get(i).y, Mouse.Cursor.X, Mouse.Cursor.Y, 25, 25))
                        {
                            MovePoint = Curve.Points.Get(i);
                            break;
                        }
                    }
                    if(MovePoint == null)
                    {
                        Point point = new Point(Mouse.Cursor.X, Mouse.Cursor.Y);
                        Curve.Points.Add(point);
                    }
                }
                if(buttonState.Type == Button.Left && buttonState.IsUp)
                {
                    MovePoint = null;
                }
            }
            if(Event.Is(InputType.Cursor))
            {
                CursorState cursorState = Event.Cursor;
                if(MovePoint != null)
                {
                    MovePoint.x = cursorState.X;
                    MovePoint.y = cursorState.Y;
                }
            }
            if(Event.Is(InputType.Key))
            {
                KeyState keyState = Event.Key;
                if(keyState.Key == Key.X && keyState.IsClick)
                {
                    Curve.Points.Clear();
                }
                if (keyState.Key == Key.C && keyState.IsClick)
                {
                    if(Curve.Points.Size()>0)
                    {
                        Curve.Points.RemoveAt(Curve.Points.Size()-1);
                    }
                }
                if(keyState.Key == Key.S && keyState.IsClick)
                {
                    StringBuilder strBuilder = new StringBuilder();
                    int detailIteration = 150;
                    for(float t=0; t<=1; t += 1/(float)detailIteration)
                    {
                        Point point = Curve.GetPoint(t);
                        strBuilder.AppendLine((point.x + ":" + point.y + ":" + point.z).Replace(',', '.'));
                    }
                    File.WriteAllText("C:\\out\\points.txt", strBuilder.ToString());
                }
            }
        }
    }
}
