using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.UI
{
    public enum WayType
    {
        None=0,
        Pixel,
        DisplayUnit,
        Percent,
    }

    public class Spacing
    {
        public Way Left;
        public Way Top;
        public Way Right;
        public Way Bottom;

        public Spacing()
        { }

        public Spacing(float leftPixel, float topPixel, float rightPixel=0, float bottomPixel=0)
        {
            this.Left = new Way(WayType.Pixel, leftPixel);
            this.Top = new Way(WayType.Pixel, topPixel);
            this.Right = new Way(WayType.Pixel, rightPixel);
            this.Bottom = new Way(WayType.Pixel, bottomPixel);
        }

        public Spacing(Way left, Way top, Way right, Way bottom)
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }
    }

    public class Room
    {
        public Way Width;
        public Way Height;
        public Way Depth;
        
        public Room()
        { }

        public Room(Room Room) : this(Room.Width, Room.Height, Room.Depth)
        { }

        public Room(Way Width, Way Height, Way Depth=null)
        {
            this.Width = Width;
            this.Height = Height;
            this.Depth = Depth;
        }

        public Room Copy()
        {
            return new Room(Width.Copy(), Height.Copy(), (Depth != null ? Depth.Copy() : null));
        }
    }

    public class Way
    {
        public WayType Type;
        public float way;

        public Way(int Type, float way)
        {
            this.Type = (WayType)Type;
            this.way = way;
        }

        public Way(WayType Type, float way)
        {
            this.Type = Type;
            this.way = way;
        }

        public Way Copy()
        {
            return new Way(Type, way);
        }

        public static Way Try(string str)
        {
            try
            {
                if (str.EndsWith("px"))
                {
                    return new Way(WayType.Pixel, float.Parse(str.Replace("px", "")));
                }
                else if (str.EndsWith("%"))
                {
                    return new Way(WayType.Percent, float.Parse(str.Replace("%", ""))/100f);
                }
                else if (str.EndsWith("em"))
                {
                    return new Way(WayType.DisplayUnit, float.Parse(str.Replace("em", "")));
                }
                else
                {
                    return new Way(WayType.Pixel, float.Parse(str));
                }
            }
            catch(Exception e)
            {
                return null;
            }
        }
    }
}
