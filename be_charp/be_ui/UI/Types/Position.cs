using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI.Types
{
    public struct PositionType
    {
        public int Top;
        public int Left;
        
        public PositionType(int X, int Y)
        {
            this.Left = X;
            this.Top = Y;
        }
    }
}
