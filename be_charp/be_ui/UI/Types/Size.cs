using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI.Types
{
    public struct SizeType
    {
        public int Width;
        public int Height;

        public SizeType(int Width, int Height)
        {
            this.Width = Width;
            this.Height = Height;
        }
    }
}
