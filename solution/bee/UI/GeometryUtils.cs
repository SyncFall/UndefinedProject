using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feltic.UI
{
    public class GeometryUtils
    {
        public static bool IntersectMargin(int X, int Y, int MouseX, int MouseY, int MarginX, int MarginY)
        {
            int minX = X - MarginX;
            int maxX = X + MarginX;
            int minY = Y - MarginY;
            int maxY = Y + MarginY;
            bool intersectX = (MouseX >= minX && MouseX <= maxX);
            bool intersectY = (MouseY >= minY && MouseY <= maxY);
            return (intersectX && intersectY);
        }

        public static bool IntersetBound(int StartX, int Width, int StartY, int Height, int MouseX, int MouseY)
        {
            int minX = StartX;
            int maxX = StartX + Width;
            int minY = StartY;
            int maxY = StartY + Height;
            bool intersectX = (MouseX >= minX && MouseX <= maxX);
            bool intersectY = (MouseY >= minY && MouseY <= maxY);
            return (intersectX && intersectY);
        }

        public static bool IntersetMarginBound(int StartX, int Width, int StartY, int Height, int Margin, int MouseX, int MouseY)
        {
            int minX = StartX - Margin;
            int maxX = StartX + Width + Margin;
            int minY = StartY - Margin;
            int maxY = StartY + Height + Margin;
            bool intersectX = (MouseX >= minX && MouseX <= maxX);
            bool intersectY = (MouseY >= minY && MouseY <= maxY);
            return (intersectX && intersectY);
        }
    }
}
