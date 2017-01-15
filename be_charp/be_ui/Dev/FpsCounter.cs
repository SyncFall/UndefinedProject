using Be.UI;
using Be.UI.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.Integrator
{
    public class FpsCounter
    {
        public static readonly int Second = 1000;
        public GlyphContainer GlyphContainer;
        public bool Started;
        public long Last;
        public int Counter;
        public int DisplayCounter;

        public FpsCounter()
        {
            GlyphContainer = new GlyphContainer(new Font(@"D:\dev\UndefinedProject\be-output\source-code-pro-regular.ttf"));
            Last = DateTime.Now.Ticks;
            Counter = 0;
            DisplayCounter = 0;
        }

        public void Draw()
        {
            long now = DateTime.Now.Ticks;
            long span = (now - Last) / 10000;
            if (span < Second)
            {
                Counter++;
                if (!Started)
                {
                    return;
                }
            }
            else
            {
                int multi = (int)(span / Second);
                float rest = (span % Second) / (float)Second;
                DisplayCounter = (int)Math.Floor(Counter * (multi + rest));
                Counter = 0;
                Last = now;
                Started = true;
            }
            GL.Color3(220/255f, 220/255f, 220/255f);
            GlyphContainer.Draw("FPS: " + DisplayCounter, 650, 5);
        }
    }
}
