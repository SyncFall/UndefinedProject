using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI.Triangulation
{
    public class Distance
    {
        public int ind;
        public double dist;

        public void copy(Distance paramDistance)
        {
            this.ind = paramDistance.ind;
            this.dist = paramDistance.dist;
        }
    }
}
