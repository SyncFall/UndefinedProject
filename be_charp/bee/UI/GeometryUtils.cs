using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bee.UI
{
    public class GeometryUtils
    {
        public static bool IntersectPositionWithMargin(int X, int Y, int MouseX, int MouseY, int MarginX, int MarginY)
        {
            int minX = X - MarginX;
            int maxX = X + MarginX;
            int minY = Y - MarginY;
            int maxY = Y + MarginY;
            bool intersectX = (MouseX >= minX && MouseX <= maxX);
            bool intersectY = (MouseY >= minY && MouseY <= maxY);
            return (intersectX && intersectY);
        }

        public static bool IntersectPositionWithBound(int StartX, int Width, int StartY, int Height, int MouseX, int MouseY)
        {
            int minX = StartX;
            int maxX = StartX + Width;
            int minY = StartY;
            int maxY = StartY + Height;
            bool intersectX = (MouseX >= minX && MouseX <= maxX);
            bool intersectY = (MouseY >= minY && MouseY <= maxY);
            return (intersectX && intersectY);
        }
    }
}
