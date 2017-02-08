using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI.Triangulation
{
    public class BBox
    {
        public int imin;           /* lexicographically smallest point, determines min-x */
        public int imax;           /* lexicographically largest point, determines max-x  */
        public double ymin;        /* minimum y-coordinate                               */
        public double ymax;        /* maximum y-coordinate                               */

        /**
         * This constructor computes the bounding box of a line segment whose end
         * points  i, j  are sorted according to x-coordinates.
         */
        public BBox(Triangulator triRef, int i, int j)
        {
            // assert(InPointsList(i));
            // assert(InPointsList(j));

            imin = Math.Min(i, j);
            imax = Math.Max(i, j);
            ymin = Math.Min(triRef.points[imin].y, triRef.points[imax].y);
            ymax = Math.Max(triRef.points[imin].y, triRef.points[imax].y);
        }


        public bool pntInBBox(Triangulator triRef, int i)
        {
            return (((imax < i) ? false :
                 ((imin > i) ? false :
                  ((ymax < triRef.points[i].y) ? false :
                   ((ymin > triRef.points[i].y) ? false : true)))));
        }



        public bool BBoxOverlap(BBox bb)
        {
            return (((imax < (bb).imin) ? false :
                 ((imin > (bb).imax) ? false :
                  ((ymax < (bb).ymin) ? false :
                   ((ymin > (bb).ymax) ? false : true)))));
        }

        public bool BBoxContained(BBox bb)
        {
            return ((imin <= (bb).imin) && (imax >= (bb).imax) &&
                (ymin <= (bb).ymin) && (ymax >= (bb).ymax));
        }


        public bool BBoxIdenticalLeaf(BBox bb)
        {
            return ((imin == (bb).imin) && (imax == (bb).imax));
        }


        public void BBoxUnion(BBox bb1, BBox bb3)
        {
            (bb3).imin = Math.Min(imin, (bb1).imin);
            (bb3).imax = Math.Max(imax, (bb1).imax);
            (bb3).ymin = Math.Min(ymin, (bb1).ymin);
            (bb3).ymax = Math.Max(ymax, (bb1).ymax);
        }


        double BBoxArea(Triangulator triRef)
        {
            return (triRef.points[imax].x - triRef.points[imin].x) * (ymax - ymin);
        }
    }
}
