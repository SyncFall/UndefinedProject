using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI.Triangulation
{
    public class Triangle
    {
        public int v1, v2, v3; // This store the index into the list array.
                        // Not the index into vertex pool yet!

        public Triangle(int a, int b, int c)
        {
            v1 = a; v2 = b; v3 = c;
        }
    }
}
