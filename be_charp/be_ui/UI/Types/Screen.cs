using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.UI.Types
{
    public class ScreenType
    {
        public WindowType WindowType;
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
        public RectRoot RectTree;
        public ToolBox Toolbox;

        public ScreenType(WindowType WindowType)
        {
            this.WindowType = WindowType;
            this.Left = 0;
            this.Top = 0;
            this.Right = WindowType.Width;
            this.Bottom = WindowType.Height;
            this.RectTree = new RectRoot(this);
            this.Toolbox = new ToolBox(this);
        }

        public void Draw()
        {
            GL.MatrixMode(MatrixMode.Projection);
            GL.LoadIdentity();
            GL.Ortho(0, WindowType.Width, WindowType.Height, 0, 0, 1);

            Toolbox.Draw();
        }

        public void MouseEvent(MouseState Result)
        {
            Toolbox.MouseEvent(Result);
        }
    }
}
