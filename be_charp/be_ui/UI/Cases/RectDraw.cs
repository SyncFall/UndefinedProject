using Be.Runtime.Types;
using Be.UI.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.UI.Cases
{
    public class RectDraw
    {
        public WindowType WindowType;
        public RectNode RectType;

        public RectDraw(WindowType Window)
        {
            this.WindowType = Window;
            this.WindowType.Mouse.AddListener(new RectDrawMouseListener(this));
        }

        public void Draw()
        {
            if(RectType != null)
            {
                GL.MatrixMode(MatrixMode.Projection);
                GL.LoadIdentity();
                GL.Ortho(0, WindowType.Width, WindowType.Height, 0, 0, 1);

                GL.DepthFunc(DepthFunction.Always);

                RectType.Draw();
            }
        }
    }

    public class RectDrawMouseListener : MouseListener
    {
        public RectDraw RectDraw;

        public RectDrawMouseListener(RectDraw RectDraw)
        {
            this.RectDraw = RectDraw;
        }

        public override void MouseEvent(MouseResult Result)
        {
            if(Result.Type == MouseType.BUTTON_EVENT)
            {
                if (RectDraw.RectType == null && Button.Key == ButtonKey.LEFT && Button.Event == ButtonEvent.DOWN)
                {
                    RectDraw.RectType = new RectNode(null);
                    RectDraw.RectType.PositionAbsolute = new PositionType(Cursor.X, Cursor.Y);
                    RectDraw.RectType.SizeTransform = true;
                }
                else if(RectDraw.RectType != null && RectDraw.RectType.SizeTransform && Button.Key == ButtonKey.LEFT && Button.Event == ButtonEvent.UP)
                {
                    RectDraw.RectType.SizeTransform = false;
                    RectDraw.RectType.FreeSizeTransform = true;
                }
                else if(Button.Key == ButtonKey.RIGHT && Button.Event == ButtonEvent.UP)
                {
                    RectDraw.RectType = null;
                }
            }
            if(RectDraw.RectType != null)
            {
                //RectDraw.RectType.MouseEvent(Result);
            }
        }
    }
}
