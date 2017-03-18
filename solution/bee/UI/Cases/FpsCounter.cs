using feltic.UI;
using feltic.UI.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.UI
{
    public class FpsCounter
    {
        public static readonly int Timer = 1000;
        public static readonly int Second = 1000;
        public GlyphContainer GlyphContainer;
        public bool Started;
        public long Last;
        public int Counter;
        public int DisplayCounter;

        public FpsCounter()
        {
            GlyphContainer = new GlyphContainer(new Font(@"D:\dev\UndefinedProject\output\source-code-pro-regular.ttf"));
            Last = DateTime.Now.Ticks;
            Counter = 0;
            DisplayCounter = 0;
        }

        public void Draw()
        {
            long now = DateTime.Now.Ticks;
            long span = (now - Last) / 10000;
            if (span < Timer)
            {
                Counter++;
                if (!Started)
                {
                    return;
                }
            }
            else
            {
                float multi = (span / Timer);
                float rest = (span % Timer) / (float)Timer;
                float fps = (Counter * (multi + rest));
                DisplayCounter = (int)Math.Round(fps * Second / Timer);
                Counter = 0;
                Last = now;
                Started = true;
            }
            GL.Color3(220/255f, 220/255f, 220/255f);
            GlyphContainer.Draw("fps(" + DisplayCounter+")", 750, 5);
        }
    }
}
