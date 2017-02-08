using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI.Triangulation
{
    public class HeapNode
    {
        public int index, prev, next;
        public double ratio;

        public HeapNode()
        {
        }

        public void copy(HeapNode hNode)
        {
            index = hNode.index;
            prev = hNode.prev;
            next = hNode.next;
            ratio = hNode.ratio;
        }
    }
}
