using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;

namespace Be.UI
{
    public class WindowType
    {
        public GameWindow _Window;
        public int Width;
        public int Height;
        public Mouse Mouse;

        public WindowType(GameWindow OpenTkWindow)
        {
            this._Window = OpenTkWindow;
            this.Width = OpenTkWindow.Width;
            this.Height = OpenTkWindow.Height;
            this.Mouse = new Mouse(this);
        }
    }
}
