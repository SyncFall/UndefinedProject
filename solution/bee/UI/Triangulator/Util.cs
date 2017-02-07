using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI.Triangulator
{
    public class Util
    {
        /*
        * This routine will return an index list for any array of objects.
        */
        public static int[] getListIndices(Object[] list)
        {
            // Create list of indices to return
            int[] indices = new int[list.Length];

            // Create hash table with initial capacity equal to the number
            // of components (assuming about half will be duplicates)
            System.Collections.Generic.Dictionary<Object, Int32> table = new Dictionary<Object, Int32>(list.Length);

            Int32 idx;
            for (int i = 0; i < list.Length; i++)
            {
                // Find index associated with this object
                if (!table.ContainsKey(list[i]))
                {
                    // We haven't seen this object before
                    indices[i] = i;

                    // Put into hash table and remember the index
                    table[list[i]] = (Int32)i;

                }
                else
                {
                    // We've seen this object
                    indices[i] = table[list[i]];
                }
            }
            return indices;
        }
    }
}
