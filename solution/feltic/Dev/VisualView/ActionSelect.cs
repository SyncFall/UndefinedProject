using feltic.UI;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Integrator
{
    public enum ActionButtonType
    {
        LinePath,
        QuadraticPath,
        CubicPath,
        NurbsPath,
        Confirm,
        Cancel,
    }

    public class ActionSelect
    {
        public VisualView VisualView;
        public Point Point;
        public Point StartPosition;
        public Dictionary<ActionButtonType, ActionButton> ActionButtons = new Dictionary<ActionButtonType, ActionButton>();

        public ActionSelect(VisualView VisualView, Point Point)
        {
            this.VisualView = VisualView;
            this.Point = Point;
            this.StartPosition = new Point(Input.Mouse.Cursor.x, Input.Mouse.Cursor.y);
            this.ActionButtons[ActionButtonType.LinePath] = new ActionButton(ActionButtonType.LinePath, this, new Image("line_path.png"));
            this.ActionButtons[ActionButtonType.QuadraticPath] = new ActionButton(ActionButtonType.QuadraticPath, this, new Image("quadratic_path.png"));
            this.ActionButtons[ActionButtonType.CubicPath] = new ActionButton(ActionButtonType.CubicPath, this, new Image("cubic_path.png"));
            this.ActionButtons[ActionButtonType.NurbsPath] = new ActionButton(ActionButtonType.NurbsPath, this, new Image("nurbs_path.png"));
        }

        public void Draw()
        {
            int x = (int)StartPosition.x;
            int y = (int)StartPosition.y;
            int padding = (ActionButton.Size / 4);
            int width = (padding + ActionButton.Size + padding);
            int height = (padding + ((ActionButton.Size + padding) * ActionButtons.Count));
            GL.Color3(75 / 255f, 75 / 255f, 75 / 255f);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(x, y);
            GL.Vertex2(x + width, y);
            GL.Vertex2(x + width, y + height);
            GL.Vertex2(x, y + height);
            GL.End();
            int xOffset = x + padding;
            int yOffset = y + padding;
            ActionButtons[ActionButtonType.LinePath].Draw(xOffset, yOffset);
            yOffset += (ActionButton.Size + padding);
            ActionButtons[ActionButtonType.QuadraticPath].Draw(xOffset, yOffset);
            yOffset += (ActionButton.Size + padding);
            ActionButtons[ActionButtonType.CubicPath].Draw(xOffset, yOffset);
            yOffset += (ActionButton.Size + padding);
            ActionButtons[ActionButtonType.NurbsPath].Draw(xOffset, yOffset);
        }

        public void ActionEvent(ActionButton ActionButton)
        {
            VisualView.ActionEvent(ActionButton);
        }

        public void Dispose()
        {
            ActionButtons[ActionButtonType.LinePath].Dispose();
            ActionButtons[ActionButtonType.QuadraticPath].Dispose();
            ActionButtons[ActionButtonType.CubicPath].Dispose();
            ActionButtons[ActionButtonType.NurbsPath].Dispose();
        }
    }

    public class ActionButton
    {
        public static int Size = 25;
        public ActionButtonType Type;
        public ActionSelect ActionSelect;
        public Image Image;
        public ActionButtonInput Input;
        public bool Hover;
        public int X = -1;
        public int Y = -1;

        public ActionButton(ActionButtonType Type, ActionSelect ActionSelect, Image Image)
        {
            this.Type = Type;
            this.ActionSelect = ActionSelect;
            this.Image = Image;
            this.Input = new ActionButtonInput(this);
        }

        public void Draw(int X, int Y)
        {
            this.X = X;
            this.Y = Y;
            if (Hover)
            {
                GL.Color3(0f, 100 / 255f, 150 / 255f);
            }
            else
            {
                GL.Color3(230 / 255f, 230 / 255f, 230 / 255f);
            }
            Image.Bind();
            GL.Begin(PrimitiveType.Quads);
            GL.TexCoord2(0, 0); GL.Vertex2(X, Y);
            GL.TexCoord2(1, 0); GL.Vertex2(X + Size, Y);
            GL.TexCoord2(1, 1); GL.Vertex2(X + Size, Y + Size);
            GL.TexCoord2(0, 1); GL.Vertex2(X, Y + Size);
            GL.End();
            Image.Unbind();
        }

        public void Dispose()
        {
            this.Input.Dispose();
            this.Input = null;
        }

        public class ActionButtonInput : InputListener
        {
            public ActionButton ActionButton;

            public ActionButtonInput(ActionButton ActionButton)
            {
                this.ActionButton = ActionButton;
            }

            public override void Input(InputEvent Event)
            {
                if (ActionButton.X == -1 || ActionButton.Y == -1)
                {
                    return;
                }
                if (Event.IsCursor)
                {
                    ActionButton.Hover = GeometryUtils.IntersectBound(ActionButton.X, Size, ActionButton.Y, Size, Mouse.Cursor.x, Mouse.Cursor.y);
                }
                if (Event.IsButton && Event.Button.Type == Button.Left && Event.Button.IsClick && GeometryUtils.IntersectBound(ActionButton.X, Size, ActionButton.Y, Size, Mouse.Cursor.x, Mouse.Cursor.y))
                {
                    ActionButton.ActionSelect.ActionEvent(ActionButton);
                }
            }
        }
    }
}
