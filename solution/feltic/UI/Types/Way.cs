using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.UI
{
    public enum WayType
    {
        Pixel,
        DisplayUnit,
        Percent,
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
