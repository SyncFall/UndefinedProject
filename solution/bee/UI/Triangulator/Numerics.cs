﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI.Triangulator
{
    public class Numerics
    {
        public static double max3(double a, double b, double c)
        {
            return (((a) > (b)) ? (((a) > (c)) ? (a) : (c))
                : (((b) > (c)) ? (b) : (c)));
        }

        public static double min3(double a, double b, double c)
        {
            return (((a) < (b)) ? (((a) < (c)) ? (a) : (c))
                : (((b) < (c)) ? (b) : (c)));
        }

        public static bool lt(double a, double eps)
        {
            return ((a) < -eps);
        }

        public static bool le(double a, double eps)
        {
            return (a <= eps);
        }

        public static bool ge(double a, double eps)
        {
            return (!((a) <= -eps));
        }

        public static bool eq(double a, double eps)
        {
            return (((a) <= eps) && !((a) < -eps));
        }

        public static bool gt(double a, double eps)
        {
            return !((a) <= eps);
        }

        public static double baseLength(Tuple2f u, Tuple2f v)
        {
            double x, y;
            x = (v).x - (u).x;
            y = (v).y - (u).y;
            return Math.Abs(x) + Math.Abs(y);
        }

        public static double sideLength(Tuple2f u, Tuple2f v)
        {
            double x, y;
            x = (v).x - (u).x;
            y = (v).y - (u).y;
            return x * x + y * y;
        }

        /**
         * This checks whether  i3,  which is collinear with  i1, i2,  is
         * between  i1, i2. note that we rely on the lexicographic sorting of the
         * points!
         */
        public static bool inBetween(int i1, int i2, int i3)
        {
            return ((i1 <= i3) && (i3 <= i2));
        }

        public static bool strictlyInBetween(int i1, int i2, int i3)
        {
            return ((i1 < i3) && (i3 < i2));
        }

        /**
         * this method computes the determinant  det(points[i],points[j],points[k])
         * in a consistent way.
         */
        public static double stableDet2D(Triangulator triRef, int i, int j, int k)
        {
            double det;
            Point2f numericsHP, numericsHQ, numericsHR;

            //      if((triRef.inPointsList(i)==false)||(triRef.inPointsList(j)==false)||
            // (triRef.inPointsList(k)==false))
            //  System.out.println("Numerics.stableDet2D Not inPointsList " + i + " " + j
            //		     + " " + k);

            if ((i == j) || (i == k) || (j == k))
            {
                det = 0.0;
            }
            else
            {
                numericsHP = triRef.points[i];
                numericsHQ = triRef.points[j];
                numericsHR = triRef.points[k];

                if (i < j)
                {
                    if (j < k)            /* i < j < k  */
                        det = Basic.det2D(numericsHP, numericsHQ, numericsHR);
                    else if (i < k)       /* i < k < j  */
                        det = -Basic.det2D(numericsHP, numericsHR, numericsHQ);
                    else                  /* k < i < j  */
                        det = Basic.det2D(numericsHR, numericsHP, numericsHQ);
                }
                else
                {
                    if (i < k)            /* j < i < k  */
                        det = -Basic.det2D(numericsHQ, numericsHP, numericsHR);
                    else if (j < k)      /* j < k < i  */
                        det = Basic.det2D(numericsHQ, numericsHR, numericsHP);
                    else                  /* k < j < i */
                        det = -Basic.det2D(numericsHR, numericsHQ, numericsHP);
                }
            }

            return det;
        }

        /**
         * Returns the orientation of the triangle.
         * @return +1 if the points  i, j, k are given in CCW order;
         * -1 if the points  i, j, k are given in CW order;
         * 0 if the points  i, j, k are collinear.
         */
        public static int orientation(Triangulator triRef, int i, int j, int k)
        {
            int ori;
            double numericsHDet;
            numericsHDet = stableDet2D(triRef, i, j, k);
            // System.out.println("orientation : numericsHDet " + numericsHDet);
            if (lt(numericsHDet, triRef.epsilon)) ori = -1;
            else if (gt(numericsHDet, triRef.epsilon)) ori = 1;
            else ori = 0;
            return ori;
        }

        /**
         * This method checks whether  l  is in the cone defined by  i, j  and  j, k
         */
        public static bool isInCone(Triangulator triRef, int i, int j, int k,
                    int l, bool convex)
        {
            bool flag;
            int numericsHOri1, numericsHOri2;

            //      if((triRef.inPointsList(i)==false)||(triRef.inPointsList(j)==false)||
            //	 (triRef.inPointsList(k)==false)||(triRef.inPointsList(l)==false))
            //	   System.out.println("Numerics.isInCone Not inPointsList " + i + " " + j
            //	      + " " + k + " " + l);

            flag = true;
            if (convex)
            {
                if (i != j)
                {
                    numericsHOri1 = orientation(triRef, i, j, l);
                    // System.out.println("isInCone : i != j, numericsHOri1 = " + numericsHOri1);
                    if (numericsHOri1 < 0) flag = false;
                    else if (numericsHOri1 == 0)
                    {
                        if (i < j)
                        {
                            if (!inBetween(i, j, l)) flag = false;
                        }
                        else
                        {
                            if (!inBetween(j, i, l)) flag = false;
                        }
                    }
                }
                if ((j != k) && (flag == true))
                {
                    numericsHOri2 = orientation(triRef, j, k, l);
                    // System.out.println("isInCone : ((j != k)  &&  (flag == true)), numericsHOri2 = " +
                    // numericsHOri2);
                    if (numericsHOri2 < 0) flag = false;
                    else if (numericsHOri2 == 0)
                    {
                        if (j < k)
                        {
                            if (!inBetween(j, k, l)) flag = false;
                        }
                        else
                        {
                            if (!inBetween(k, j, l)) flag = false;
                        }
                    }
                }
            }
            else
            {
                numericsHOri1 = orientation(triRef, i, j, l);
                if (numericsHOri1 <= 0)
                {
                    numericsHOri2 = orientation(triRef, j, k, l);
                    if (numericsHOri2 < 0) flag = false;
                }
            }
            return flag;
        }


        /**
         * Returns convex angle flag.
         * @return 0 ... if angle is 180 degrees <br>
         *         1 ... if angle between 0 and 180 degrees <br>
         *         2 ... if angle is 0 degrees <br>
         *        -1 ... if angle between 180 and 360 degrees <br>
         *        -2 ... if angle is 360 degrees <br>
         */
        public static int isConvexAngle(Triangulator triRef, int i, int j, int k, int ind)
        {
            int angle;
            double numericsHDot;
            int numericsHOri1;
            Point2f numericsHP, numericsHQ;

            //      if((triRef.inPointsList(i)==false)||(triRef.inPointsList(j)==false)||
            //	 (triRef.inPointsList(k)==false))
            //	  System.out.println("Numerics.isConvexAngle: Not inPointsList " + i + " " + j
            //			     + " " + k);

            if (i == j)
            {
                if (j == k)
                {
                    // all three vertices are identical; we set the angle to 1 in
                    // order to enable clipping of  j.
                    return 1;
                }
                else
                {
                    // two of the three vertices are identical; we set the angle to 1
                    // in order to enable clipping of  j.
                    return 1;
                }
            }
            else if (j == k)
            {
                // two vertices are identical. we could either determine the angle
                // by means of yet another lengthy analysis, or simply set the
                // angle to -1. using -1 means to err on the safe side, as all the
                // incarnations of this vertex will be clipped right at the start
                // of the ear-clipping algorithm. thus, eventually there will be no
                // other duplicates at this vertex position, and the regular
                // classification of angles will yield the correct answer for j.
                return -1;
            }
            else
            {
                numericsHOri1 = orientation(triRef, i, j, k);
                // System.out.println("i " + i + " j " + j + " k " + k + " ind " + ind +
                //		   ". In IsConvexAngle numericsHOri1 is " +
                //		   numericsHOri1);
                if (numericsHOri1 > 0)
                {
                    angle = 1;
                }
                else if (numericsHOri1 < 0)
                {
                    angle = -1;
                }
                else
                {
                    // 0, 180, or 360 degrees.
                    numericsHP = new Point2f();
                    numericsHQ = new Point2f();
                    Basic.vectorSub2D(triRef.points[i], triRef.points[j], numericsHP);
                    Basic.vectorSub2D(triRef.points[k], triRef.points[j], numericsHQ);
                    numericsHDot = Basic.dotProduct2D(numericsHP, numericsHQ);
                    if (numericsHDot < 0.0)
                    {
                        // 180 degrees.
                        angle = 0;
                    }
                    else
                    {
                        // 0 or 360 degrees? this cannot be judged locally, and more
                        // work is needed.

                        angle = spikeAngle(triRef, i, j, k, ind);
                        // System.out.println("SpikeAngle return is "+ angle);
                    }
                }
            }
            return angle;
        }


        /**
         * This method checks whether point  i4  is inside of or on the boundary
         * of the triangle  i1, i2, i3.
         */
        public static bool pntInTriangle(Triangulator triRef, int i1, int i2, int i3, int i4)
        {
            bool inside;
            int numericsHOri1;

            inside = false;
            numericsHOri1 = orientation(triRef, i2, i3, i4);
            if (numericsHOri1 >= 0)
            {
                numericsHOri1 = orientation(triRef, i1, i2, i4);
                if (numericsHOri1 >= 0)
                {
                    numericsHOri1 = orientation(triRef, i3, i1, i4);
                    if (numericsHOri1 >= 0) inside = true;
                }
            }
            return inside;
        }


        /**
         * This method checks whether point  i4  is inside of or on the boundary
         * of the triangle  i1, i2, i3. it also returns a classification if  i4  is
         * on the boundary of the triangle (except for the edge  i2, i3).
         */
        public static bool vtxInTriangle(Triangulator triRef, int i1, int i2, int i3,
                     int i4, int[] type)
        {
            bool inside;
            int numericsHOri1;

            inside = false;
            numericsHOri1 = orientation(triRef, i2, i3, i4);
            if (numericsHOri1 >= 0)
            {
                numericsHOri1 = orientation(triRef, i1, i2, i4);
                if (numericsHOri1 > 0)
                {
                    numericsHOri1 = orientation(triRef, i3, i1, i4);
                    if (numericsHOri1 > 0)
                    {
                        inside = true;
                        type[0] = 0;
                    }
                    else if (numericsHOri1 == 0)
                    {
                        inside = true;
                        type[0] = 1;
                    }
                }
                else if (numericsHOri1 == 0)
                {
                    numericsHOri1 = orientation(triRef, i3, i1, i4);
                    if (numericsHOri1 > 0)
                    {
                        inside = true;
                        type[0] = 2;
                    }
                    else if (numericsHOri1 == 0)
                    {
                        inside = true;
                        type[0] = 3;
                    }
                }
            }
            return inside;
        }


        /**
         * Checks whether the line segments  i1, i2  and  i3, i4  intersect. no
         * intersection is reported if they intersect at a common vertex.
         * the function assumes that  i1 <= i2  and  i3 <= i4. if  i3  or  i4  lies
         * on  i1, i2  then an intersection is reported, but no intersection is
         * reported if  i1  or  i2  lies on  i3, i4. this function is not symmetric!
         */
        public static bool segIntersect(Triangulator triRef, int i1, int i2, int i3,
                    int i4, int i5)
        {
            int ori1, ori2, ori3, ori4;

            //      if((triRef.inPointsList(i1)==false)||(triRef.inPointsList(i2)==false)||
            //	 (triRef.inPointsList(i3)==false)||(triRef.inPointsList(i4)==false))
            //	System.out.println("Numerics.segIntersect Not inPointsList " + i1 + " " + i2
            //			   + " " + i3 + " " + i4);
            //
            //      if((i1 > i2) || (i3 > i4))
            //	System.out.println("Numerics.segIntersect i1>i2 or i3>i4 " + i1 + " " + i2
            //      		   + " " + i3 + " " + i4);

            if ((i1 == i2) || (i3 == i4)) return false;
            if ((i1 == i3) && (i2 == i4)) return true;

            if ((i3 == i5) || (i4 == i5)) ++(triRef.identCntr);

            ori3 = orientation(triRef, i1, i2, i3);
            ori4 = orientation(triRef, i1, i2, i4);
            if (((ori3 == 1) && (ori4 == 1)) ||
                ((ori3 == -1) && (ori4 == -1)))
                return false;

            if (ori3 == 0)
            {
                if (strictlyInBetween(i1, i2, i3)) return true;
                if (ori4 == 0)
                {
                    if (strictlyInBetween(i1, i2, i4)) return true;
                }
                else return false;
            }
            else if (ori4 == 0)
            {
                if (strictlyInBetween(i1, i2, i4)) return true;
                else return false;
            }

            ori1 = orientation(triRef, i3, i4, i1);
            ori2 = orientation(triRef, i3, i4, i2);
            if (((ori1 <= 0) && (ori2 <= 0)) ||
                ((ori1 >= 0) && (ori2 >= 0)))
                return false;

            return true;
        }


        /**
         * this function computes a quality measure of a triangle  i, j, k.
         * it returns the ratio  `base / height', where   base  is the length of the
         * longest side of the triangle, and   height  is the normal distance
         * between the vertex opposite of the base side and the base side. (as
         * usual, we again use the l1-norm for distances.)
         */
        public static double getRatio(Triangulator triRef, int i, int j, int k)
        {
            double area, a, b, c, bse, ratio;
            Point2f p, q, r;

            //      if((triRef.inPointsList(i)==false)||(triRef.inPointsList(j)==false)||
            //	 (triRef.inPointsList(k)==false))
            //	System.out.println("Numerics.getRatio: Not inPointsList " + i + " " + j
            //			   + " " + k);

            p = triRef.points[i];
            q = triRef.points[j];
            r = triRef.points[k];


            a = baseLength(p, q);
            b = baseLength(p, r);
            c = baseLength(r, q);
            bse = max3(a, b, c);

            if ((10.0 * a) < Math.Min(b, c)) return 0.1;

            area = stableDet2D(triRef, i, j, k);
            if (lt(area, triRef.epsilon))
            {
                area = -area;
            }
            else if (!gt(area, triRef.epsilon))
            {
                if (bse > a) return 0.1;
                else return Double.MaxValue;
            }

            ratio = bse * bse / area;

            if (ratio < 10.0) return ratio;
            else
            {
                if (a < bse) return 0.1;
                else return ratio;
            }
        }


        public static int spikeAngle(Triangulator triRef, int i, int j, int k, int ind)
        {
            int ind1, ind2, ind3;
            int i1, i2, i3;

            //      if((triRef.inPointsList(i)==false)||(triRef.inPointsList(j)==false)||
            //	 (triRef.inPointsList(k)==false))
            //	System.out.println("Numerics.spikeAngle: Not inPointsList " + i + " " + j
            //			   + " " + k);

            ind2 = ind;
            i2 = triRef.fetchData(ind2);

            //      if(i2 != j)
            //	System.out.println("Numerics.spikeAngle: i2 != j " + i2 + " " + j );

            ind1 = triRef.fetchPrevData(ind2);
            i1 = triRef.fetchData(ind1);

            //      if(i1 != i)
            //	System.out.println("Numerics.spikeAngle: i1 != i " + i1 + " " + i );

            ind3 = triRef.fetchNextData(ind2);
            i3 = triRef.fetchData(ind3);

            //      if(i3 != k)
            //	System.out.println("Numerics.spikeAngle: i3 != k " + i3 + " " + k );

            return recSpikeAngle(triRef, i, j, k, ind1, ind3);
        }



        public static int recSpikeAngle(Triangulator triRef, int i1, int i2, int i3,
                     int ind1, int ind3)
        {
            int ori, ori1, ori2, i0, ii1, ii2;
            Point2f pq, pr;
            double dot;

            if (ind1 == ind3)
            {
                // all points are collinear???  well, then it does not really matter
                // which angle is returned. perhaps, -2 is the best bet as my code
                // likely regards this contour as a hole.
                return -2;
            }

            if (i1 != i3)
            {
                if (i1 < i2)
                {
                    ii1 = i1;
                    ii2 = i2;
                }
                else
                {
                    ii1 = i2;
                    ii2 = i1;
                }
                if (inBetween(ii1, ii2, i3))
                {
                    i2 = i3;
                    ind3 = triRef.fetchNextData(ind3);
                    i3 = triRef.fetchData(ind3);

                    if (ind1 == ind3) return 2;
                    ori = orientation(triRef, i1, i2, i3);
                    if (ori > 0) return 2;
                    else if (ori < 0) return -2;
                    else return recSpikeAngle(triRef, i1, i2, i3, ind1, ind3);
                }
                else
                {
                    i2 = i1;
                    ind1 = triRef.fetchPrevData(ind1);
                    i1 = triRef.fetchData(ind1);
                    if (ind1 == ind3) return 2;
                    ori = orientation(triRef, i1, i2, i3);
                    if (ori > 0) return 2;
                    else if (ori < 0) return -2;
                    else return recSpikeAngle(triRef, i1, i2, i3, ind1, ind3);
                }
            }
            else
            {
                i0 = i2;
                i2 = i1;
                ind1 = triRef.fetchPrevData(ind1);
                i1 = triRef.fetchData(ind1);

                if (ind1 == ind3) return 2;
                ind3 = triRef.fetchNextData(ind3);
                i3 = triRef.fetchData(ind3);
                if (ind1 == ind3) return 2;
                ori = orientation(triRef, i1, i2, i3);
                if (ori > 0)
                {
                    ori1 = orientation(triRef, i1, i2, i0);
                    if (ori1 > 0)
                    {
                        ori2 = orientation(triRef, i2, i3, i0);
                        if (ori2 > 0) return -2;
                    }
                    return 2;
                }
                else if (ori < 0)
                {
                    ori1 = orientation(triRef, i2, i1, i0);
                    if (ori1 > 0)
                    {
                        ori2 = orientation(triRef, i3, i2, i0);
                        if (ori2 > 0) return 2;
                    }
                    return -2;
                }
                else
                {
                    pq = new Point2f();
                    Basic.vectorSub2D(triRef.points[i1], triRef.points[i2], pq);
                    pr = new Point2f();
                    Basic.vectorSub2D(triRef.points[i3], triRef.points[i2], pr);
                    dot = Basic.dotProduct2D(pq, pr);
                    if (dot < 0.0)
                    {
                        ori = orientation(triRef, i2, i1, i0);
                        if (ori > 0) return 2;
                        else return -2;
                    }
                    else
                    {
                        return recSpikeAngle(triRef, i1, i2, i3, ind1, ind3);
                    }
                }
            }
        }


        /**
         * computes the signed angle between  p, p1  and  p, p2.
         *
         * warning: this function does not handle a 180-degree angle correctly!
         *          (this is no issue in our application, as we will always compute
         *           the angle centered at the mid-point of a valid diagonal.)
         */
        public static double angle(Triangulator triRef, Point2f p, Point2f p1, Point2f p2)
        {
            int sign;
            double angle1, angle2, angle;
            Point2f v1, v2;

            sign = Basic.signEps(Basic.det2D(p2, p, p1), triRef.epsilon);

            if (sign == 0) return 0.0;

            v1 = new Point2f();
            v2 = new Point2f();
            Basic.vectorSub2D(p1, p, v1);
            Basic.vectorSub2D(p2, p, v2);

            angle1 = Math.Atan2(v1.y, v1.x);
            angle2 = Math.Atan2(v2.y, v2.x);

            if (angle1 < 0.0) angle1 += 2.0 * Math.PI;
            if (angle2 < 0.0) angle2 += 2.0 * Math.PI;

            angle = angle1 - angle2;
            if (angle > Math.PI) angle = 2.0 * Math.PI - angle;
            else if (angle < -Math.PI) angle = 2.0 * Math.PI + angle;

            if (sign == 1)
            {
                if (angle < 0.0) return -angle;
                else return angle;
            }
            else
            {
                if (angle > 0.0) return -angle;
                else return angle;
            }
        }


    }
}
