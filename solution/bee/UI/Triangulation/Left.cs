using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI.Triangulation
{
    public class Left
    {
        public int ind;
        public int index;

        public void copy(Left paramLeft)
        {
            this.ind = paramLeft.ind;
            this.index = paramLeft.index;
        }
    }
}
