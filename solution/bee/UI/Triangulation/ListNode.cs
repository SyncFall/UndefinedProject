using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI.Triangulation
{ 
    public class ListNode
    {
        public int index;
        public int prev;
        public int next;
        public int convex;
        public int vcntIndex;      // Vertex, Color, Normal, Texture Index



        public ListNode(int ind)
        {
            index = ind;
            prev = -1;
            next = -1;
            convex = 0;
            vcntIndex = -1;
        }

        public void setCommonIndex(int comIndex)
        {
            vcntIndex = comIndex;

        }

        public int getCommonIndex()
        {
            return vcntIndex;
        }
    }
}
