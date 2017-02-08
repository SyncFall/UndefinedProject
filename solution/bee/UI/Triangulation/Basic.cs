using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI.Triangulation
{
    public class Basic
    {
        public const double D_RND_MAX = 2147483647.0;

        public static double detExp(double u_x, double u_y, double u_z,
                 double v_x, double v_y, double v_z,
                 double w_x, double w_y, double w_z)
        {

            return ((u_x) * ((v_y) * (w_z) - (v_z) * (w_y)) -
                (u_y) * ((v_x) * (w_z) - (v_z) * (w_x)) +
                (u_z) * ((v_x) * (w_y) - (v_y) * (w_x)));
        }


        public static double det3D(Tuple3f u, Tuple3f v, Tuple3f w)
        {
            return ((u).x * ((v).y * (w).z - (v).z * (w).y) -
                (u).y * ((v).x * (w).z - (v).z * (w).x) +
                (u).z * ((v).x * (w).y - (v).y * (w).x));
        }


        public static double det2D(Tuple2f u, Tuple2f v, Tuple2f w)
        {
            return (((u).x - (v).x) * ((v).y - (w).y) + ((v).y - (u).y) * ((v).x - (w).x));
        }


        public static double length2(Tuple3f u)
        {
            return (((u).x * (u).x) + ((u).y * (u).y) + ((u).z * (u).z));
        }

        public static double lengthL1(Tuple3f u)
        {
            return (Math.Abs((u).x) + Math.Abs((u).y) + Math.Abs((u).z));
        }

        public static double lengthL2(Tuple3f u)
        {
            return Math.Sqrt(((u).x * (u).x) + ((u).y * (u).y) + ((u).z * (u).z));
        }


        public static double dotProduct(Tuple3f u, Tuple3f v)
        {
            return (((u).x * (v).x) + ((u).y * (v).y) + ((u).z * (v).z));
        }


        public static double dotProduct2D(Tuple2f u, Tuple2f v)
        {
            return (((u).x * (v).x) + ((u).y * (v).y));
        }


        public static void vectorProduct(Tuple3f p, Tuple3f q, Tuple3f r)
        {
            (r).x = (p).y * (q).z - (q).y * (p).z;
            (r).y = (q).x * (p).z - (p).x * (q).z;
            (r).z = (p).x * (q).y - (q).x * (p).y;
        }


        public static void vectorAdd(Tuple3f p, Tuple3f q, Tuple3f r)
        {
            (r).x = (p).x + (q).x;
            (r).y = (p).y + (q).y;
            (r).z = (p).z + (q).z;
        }

        public static void vectorSub(Tuple3f p, Tuple3f q, Tuple3f r)
        {
            (r).x = (p).x - (q).x;
            (r).y = (p).y - (q).y;
            (r).z = (p).z - (q).z;
        }


        public static void vectorAdd2D(Tuple2f p, Tuple2f q, Tuple2f r)
        {
            (r).x = (p).x + (q).x;
            (r).y = (p).y + (q).y;
        }


        public static void vectorSub2D(Tuple2f p, Tuple2f q, Tuple2f r)
        {
            (r).x = (p).x - (q).x;
            (r).y = (p).y - (q).y;
        }

        public static void invertVector(Tuple3f p)
        {
            (p).x = -(p).x;
            (p).y = -(p).y;
            (p).z = -(p).z;
        }

        public static void divScalar(float scalar, Tuple3f u)
        {
            (u).x /= scalar;
            (u).y /= scalar;
            (u).z /= scalar;
        }

        public static void multScalar2D(float scalar, Tuple2f u)
        {
            (u).x *= scalar;
            (u).y *= scalar;
        }


        public static int signEps(double x, double eps)
        {
            return ((x <= eps) ? ((x < -eps) ? -1 : 0) : 1);
        }
    }
}