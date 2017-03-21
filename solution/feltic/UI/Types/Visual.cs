using feltic.Language;
using feltic.Library;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.UI.Types
{
    public class VisualElement
    {
        public VisualElementType Type;
        public VisualElement Parent;
        public Room Room;
        public Size Size;
        public Position Position;
        public VisualElementList Childrens;
        public InputListener InputListener;
        public Color Color;

        public VisualElement(VisualElementType Type, VisualElement Parent)
        {
            this.Type = Type;
            this.Parent = Parent;
            this.Room = new Room();
            if (Parent != null)
                Parent.AddChild(this);
        }

        public void AddChild(VisualElement VisualElement)
        {
            if (Childrens == null)
                Childrens = new VisualElementList();
            Childrens.Add(VisualElement);
        }

        public virtual void Draw(float X=0, float Y=0, float OffsetX=0, float OffsetY=0, float Width=0, float Height=0)
        {
            if (Type == VisualElementType.Text)
            {
                (this as VisualTextElement).DrawText(this.Color, X, Y - 3, OffsetX, OffsetY, Width, Height);
                return;
            }

            if (Type == VisualElementType.Scroll)
            {
                if (Childrens != null && Childrens.Size == 1)
                {
                    VisualElement child = Childrens[0];
                    VisualScrollElement scroll = this as VisualScrollElement;
                    float factorHeight = (child.Size.Height / scroll.Size.Height);
                    float offsetHeight = (scroll.ScrollYPosition * factorHeight);
                    float factorWidth = (child.Size.Width / scroll.Size.Width);
                    float offsetWidth = (scroll.ScrollXPosition * factorWidth);
                    child.Draw(X, Y, offsetWidth, offsetHeight, Size.Width, Size.Height);
                    scroll.DrawScrollY(X, Y);
                    scroll.DrawScrollX(X, Y);
                    return;
                }
            }

            float w = Width!=0 ? Width : (Size!=null&&Size.Width!=-1f?Size.Width:0f);
            float h = Height!=0 ? Height : (Size!=null&&Size.Height!=-1f?Size.Height:0f);
            if(w > 0f && h > 0f)
            {
                GL.Color3(1f, 1f, 1f);
                GL.LineWidth(1f);
                GL.Begin(PrimitiveType.LineLoop);
                GL.Vertex2(X, Y);
                GL.Vertex2(X + w, Y);
                GL.Vertex2(X + w, Y + h);
                GL.Vertex2(X, Y + h);
                GL.End();
            }

            if (Childrens == null)
                return;

            float currentX = X;
            float currentY = Y;
            float realX = currentX;
            float realY = currentY;
            for (int i = 0; i < Childrens.Size; i++)
            {
                VisualElement childElement = Childrens[i];
                Size childSize = childElement.Size;
                if (childSize == null) return;
                bool smaller = (currentY + childSize.Height < Y + OffsetY);
                bool taller = false;
                bool notVisible = false;
                if (childElement.Type == VisualElementType.Text && Childrens.Size > 1)
                {
                    taller = (realY - Y + childSize.Height > (Height > 0 ? Height : Size.Height));
                    notVisible = (currentX + childSize.Width < X + OffsetX) || (currentX > X + OffsetX + (Width > 0 ? Width : Size.Width));
                    float offset = ((X + OffsetX) - currentX);
                    float offset2 = ((X + OffsetX + (Width > 0 ? Width : Size.Width)) - currentX);
                    if (!smaller && !notVisible && offset > 0 && offset < childSize.Width)
                    {
                        float oo = (int)Math.Ceiling(offset);
                        float ww = childSize.Width - offset;
                        childElement.Draw(realX, realY, oo, 0, ww, 0);
                        realX += ww;
                        currentX += childSize.Width;
                        continue;
                    }
                    else if (!smaller && !notVisible && offset2 < childSize.Width)
                    {
                        float ww = (int)Math.Round(offset2);
                        childElement.Draw(realX, realY, 0, 0, ww, 0);
                        realX += ww;
                        currentX += childSize.Width;
                        continue;
                    }
                    else if (!smaller && !taller && !notVisible)
                    {
                        childElement.Draw(realX, realY, 0, 0, 0, 0);
                    }
                }
                else if (!smaller && !taller)
                {
                    float childWidth = (childSize.Width > w ? w : 0);
                    float childHeight = (childSize.Height > h ? h : 0);
                    childElement.Draw(currentX, currentY, OffsetX, OffsetY, childWidth, childHeight);
                }
                if (childSize != null && childSize.Width != -1f && childSize.Height != -1f)
                {
                    if(childElement.Type == VisualElementType.Block)
                    {
                        currentY += childSize.Height;
                    }
                    else if(childElement.Type == VisualElementType.Inline || childElement.Type == VisualElementType.Column)
                    {
                        currentX += childSize.Width;
                    }
                    else if(childElement.Type == VisualElementType.Text)
                    {
                        if (Childrens.Size == 1)
                            break;
                        else
                        {
                            currentX += childSize.Width;
                            if(!smaller && !taller && !notVisible)
                            {
                                realX += childSize.Width;
                            }                            
                        }
                    }
                    else if(childElement.Type == VisualElementType.Break)
                    {
                        currentY += childSize.Height;
                        currentX = X;
                        if (!smaller && !taller && !notVisible)
                        {
                            realY += childSize.Height;
                        }
                        realX = X;
                    }
                }
                if(taller) break;
            }
        }
    }

    public abstract class VisualInputListener : InputListener
    {
        public VisualElement Element;
        public bool Active = false;

        public VisualInputListener(VisualElement Element)
        {
            this.Element = Element;
        }

        public override void Input(InputEvent Event)
        {
            if (Element == null || Element.Size == null || Element.Position == null)
            {
                return;
            }
            if(Event.IsCursor)
            {
                if(GeometryUtils.IntersetBound((int)Element.Position.x, (int)Element.Size.Width, (int)Element.Position.y, (int)Element.Size.Height, Event.Cursor.x, Event.Cursor.y))
                {
                    this.Event(Event);
                }
            }
            else if(Event.IsButton)
            {
                Active = (GeometryUtils.IntersetBound((int)Element.Position.x, (int)Element.Size.Width, (int)Element.Position.y, (int)Element.Size.Height, Mouse.Cursor.x, Mouse.Cursor.y));
                if (Active) {
                    this.Event(Event);
                }
            }
            else if(Active && Event.IsKey)
            {
                this.Event(Event);
            }
            else if(Active && Event.IsText)
            {
               this.Event(Event);
            }
        }

        public abstract void Event(InputEvent Event);
    }

    public class VisualElementMetrics
    {
        public static float GetWidth(VisualElement Element)
        {
            Room room = Element.Room;
            float width = -1;
            // width definition
            if (room.Width != null)
            {
                // absolute pixel size
                if (room.Width != null && room.Width.Type == WayType.Pixel)
                {
                    width = room.Width.way;
                }
                // absolute display size
                else if (room.Width != null && room.Width.Type == WayType.DisplayUnit)
                {
                    width = (room.Width.way * 5);
                }
                // relative parent size
                else if (room.Width != null && room.Width.Type == WayType.Percent)
                {
                    float parentWidth = ParentWidth(Element);
                    width = (parentWidth != -1f ? room.Width.way * parentWidth : -1f);
                }
            }
            return width;
        }

        public static float GetHeight(VisualElement Element)
        {
            Room room = Element.Room;
            float height = -1;
            // height definition
            if (room.Height != null)
            {
                // absolute pixel size
                if (room.Height.Type == WayType.Pixel)
                {
                    height = room.Height.way;
                }
                // absolute display size
                else if (room.Height.Type == WayType.DisplayUnit)
                {
                    height = (room.Height.way * 20);
                }
                // relative parent size
                else if (room.Height.Type == WayType.Percent)
                {
                    float parentHeight = ParentHeight(Element);
                    height = (parentHeight != -1f ? (room.Height.way * parentHeight) : -1f);
                }
            }
            return height;
        }

        public static float ParentWidth(VisualElement Element)
        {
            float width;
            // try to get absolute size from parent
            VisualElement parentElement = Element.Parent;
            while (parentElement != null)
            {
                if ((width = GetWidth(parentElement)) != -1f)
                {
                    return width;
                }
                parentElement = parentElement.Parent;
            }
            return -1f;
        }

        public static float ParentHeight(VisualElement Element)
        {
            float height;
            // try to get absolute size from parent
            VisualElement parentElement = Element.Parent;
            while (parentElement != null)
            {
                if ((height = GetHeight(parentElement)) != -1f)
                {
                    return height;
                }
                parentElement = parentElement.Parent;
            }
            return -1f;
        }

        public static Position GetPosition(VisualElement Element, float X, float Y)
        {
            if (Element.Position != null)
            {
                return Element.Position;
            }

            if (Element.Childrens != null)
            {
                float currentX = X;
                float currentY = Y;
                for (int i = 0; i < Element.Childrens.Size; i++)
                {
                    VisualElement childElement = Element.Childrens[i];
                    Size childSize = GetSize(childElement);
                    Position childPosition = GetPosition(childElement, currentX, currentY);
                    if (childElement.Type == VisualElementType.Block)
                    {
                        currentY += childSize.Height;
                    }
                    else if (childElement.Type == VisualElementType.Inline || childElement.Type == VisualElementType.Column)
                    {
                        currentX += childSize.Width;
                    }
                    else if (childElement.Type == VisualElementType.Text || childElement.Type == VisualElementType.Input)
                    {
                        if(Element.Childrens.Size==1)
                        {
                            break;
                        }
                        else
                        {
                            currentX += childSize.Width;   
                        }
                    }
                    else if (childElement.Type == VisualElementType.Break)
                    {
                        currentY += childSize.Height;
                        currentX = 0;
                    }
                }
            }

            return (Element.Position = new Position(X, Y));
        }

        public static Size GetSize(VisualElement Element)
        {
            // return already calculated
            if (Element.Size != null)
            {
                return Element.Size;
            }

            // text-size
            if (Element.Type == VisualElementType.Text)
            {
                return (Element.Size = (Element as VisualTextElement)._TextHandle.Size);
            }

            // break-size
            if(Element.Type == VisualElementType.Break)
            {
                return (Element.Size = new Size(0, new UI.Text(" ", null).Size.Height));
            }

            // size from definition
            float width = GetWidth(Element);
            float height = GetHeight(Element);

            // total width from parents for block element
            if (width == -1f && (Element.Type == VisualElementType.Block || Element.Type == VisualElementType.Scroll))
            {
                if ((width = ParentWidth(Element)) != -1f)
                {
                    ;
                }
            }

            // total height from parents for columen element
            if (height == -1f && (Element.Type == VisualElementType.Column || Element.Type == VisualElementType.Scroll))
            {
                if ((height = ParentHeight(Element)) != -1f)
                {
                    ;
                }
            }
            
            // default sizes for input-field
            if ((width == -1f || height == -1f) && Element.Type == VisualElementType.Input)
            {
                Size size = new Text(new string(' ', 15), null).Size;
                if(width == -1f) width = size.Width;
                if(height == -1f) height = size.Height;
            }

            // size from childrens
            float childsWidth = 0f;
            float currentWidth = 0f;
            float childsHeight = 0f;
            for (int i = 0; Element.Childrens != null && i < Element.Childrens.Size; i++)
            {
                VisualElement childElement = Element.Childrens[i];
                Size childSize = GetSize(childElement);
                if (childElement.Type == VisualElementType.Block || childElement.Type == VisualElementType.Scroll)
                {
                    if (childSize.Width > childsWidth)
                    {
                        childsWidth = childSize.Width;
                    }
                    childsHeight += childSize.Height;
                }
                else if(childElement.Type == VisualElementType.Inline || childElement.Type == VisualElementType.Column){
                    childsWidth += childSize.Width;
                    if (childSize.Height > childsHeight)
                    {
                        childsHeight = childSize.Height;
                    }
                }
                else if(childElement.Type == VisualElementType.Text || childElement.Type == VisualElementType.Input)
                {
                    if(Element.Childrens.Size == 1)
                    {
                        childsHeight = childSize.Height;
                        childsWidth = childSize.Width;
                        break;
                    }
                    else
                    {
                        currentWidth += childSize.Width;
                    }
                }
                else if (childElement.Type == VisualElementType.Break)
                {
                    childsHeight += childSize.Height;
                    if(currentWidth > childsWidth)
                    {
                        childsWidth = currentWidth;
                    }
                    currentWidth = 0;
                }
            }

            // extend element size
            if(Element.Type != VisualElementType.Scroll)
            {
                if (childsWidth > width)
                    width = childsWidth;
                if (childsHeight > height)
                    height = childsHeight;
            }

            // may complete, zero or unknown
            return (Element.Size = new Size(width != -1 ? width : 0, height != -1 ? height : 0));
        }
    }

    public class VisualElementList : ListCollection<VisualElement>
    { }

    public class VisualTextElement : VisualElement
    {
        public Text _TextHandle;

        public string Text
        {
            get
            {
                return (_TextHandle.String);
            }
            set
            {
                _TextHandle = new UI.Text(value, null);
            }
        }

        public VisualTextElement(string String, VisualElement Parent, Color Color=null) : base(VisualElementType.Text, Parent)
        {
            this.Text = String;
            this.Color = Color;
        }

        public void DrawText(Color Color, float X =0, float Y=0, float OffsetX=0, float OffsetY=0, float Width=0, float Height=0)
        {
            _TextHandle.Draw(Color, X, Y, OffsetX, OffsetY, Width, Height);
        }
    }


    public class VisualInputElement : VisualElement
    {
        public string Text
        {
            get
            {
                if (Childrens != null && Childrens[0].Type == VisualElementType.Text)
                    return (Childrens[0] as VisualTextElement).Text;
                return "";
            }
            set
            {
                if (Childrens != null && Childrens[0].Type == VisualElementType.Text)
                    (Childrens[0] as VisualTextElement).Text = (value != null ? value : "");
            }
        }

        public VisualInputElement(VisualElement Parent) : base(VisualElementType.Input, Parent)
        {
            this.InputListener = new TextListener(this);
        }

        public class TextListener : VisualInputListener
        {
            public TextListener(VisualInputElement Element) : base(Element)
            { }

            public override void Event(InputEvent Event)
            {
                if (!Active) return;
                if (Event.IsText)
                {
                    (Element as VisualInputElement).Text += Event.Text.TextContent;
                }
                else if (Event.IsKey && Event.Key.IsDown)
                {
                    if (Event.Key.Type == Key.BackSpace)
                    {
                        string text = (Element as VisualInputElement).Text;
                        if (text.Length > 0)
                        {
                            text = text.Substring(0, text.Length - 1);
                        }
                        (Element as VisualInputElement).Text = text;
                    }
                }
            }
        }
    }

    public class VisualScrollElement : VisualElement
    {
        public static readonly int ScrollThickness = 12;
        public float ScrollYPosition = 0f;
        public float ScrollXPosition = 0f;
        public Size ScrollYSize = new Size(ScrollThickness, 0);
        public Size ScrollXSize = new Size(0, ScrollThickness);

        public VisualScrollElement(VisualElement Parent) : base(VisualElementType.Scroll, Parent)
        {
            this.InputListener = new ScrollListener(this);
        }

        public void DrawScrollY(float X = 0, float Y = 0)
        {
            if(Childrens==null||Childrens.Size != 1)
            {
                return;
            }
            VisualElement child = Childrens[0];
            Size childSize = child.Size;

            float factor = (Size.Height / childSize.Height);
            ScrollYSize.Height = (Size.Height * factor);
            
            float w = ScrollYSize.Width;
            float h = ScrollYSize.Height;
            float x = Position.x + Size.Width - w;
            float y = Position.y + ScrollYPosition;

            GL.Color3(75 / 255f, 75 / 255f, 75 / 255f);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(x, y);
            GL.Vertex2(x + w, y);
            GL.Vertex2(x + w, y + h);
            GL.Vertex2(x, y + h);
            GL.End();
        }

        public void DrawScrollX(float X=0, float Y=0)
        {
            if (Childrens == null || Childrens.Size != 1)
            {
                return;
            }
            VisualElement child = Childrens[0];
            Size childSize = child.Size;

            float factor = (Size.Width / childSize.Width);
            ScrollXSize.Width = (Size.Width * factor);

            float w = ScrollXSize.Width;
            float h = ScrollXSize.Height;
            float x = Position.x + ScrollXPosition;
            float y = Position.y + Size.Height - h;

            GL.Color3(75 / 255f, 75 / 255f, 75 / 255f);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(x, y);
            GL.Vertex2(x + w, y);
            GL.Vertex2(x + w, y + h);
            GL.Vertex2(x, y + h);
            GL.End();
        }

        public class ScrollListener : InputListener
        {
            public VisualScrollElement Element;
            public bool ActiveScrollY;
            public bool ActiveScrollX;
            public float ScrollYOffset = 0f;
            public float ScrollXOffset = 0f;

            public ScrollListener(VisualScrollElement Element)
            { 
                this.Element = Element;
            }

            public override void Input(InputEvent Event)
            {
                if (Event.IsButton && Event.Button.IsClick && Event.Button.Type == Button.Left)
                {
                    if(GeometryUtils.IntersetBound((int)(Element.Position.x + Element.Size.Width - Element.ScrollYSize.Width), (int)Element.ScrollYSize.Width,
                                                   (int)(Element.Position.y + Element.ScrollYPosition), (int)Element.ScrollYSize.Height,
                                                   Mouse.Cursor.x, Mouse.Cursor.y))
                    {
                        ActiveScrollY = true;
                        ScrollYOffset = (Mouse.Cursor.y - Element.ScrollYPosition);
                    }
                    else if(GeometryUtils.IntersetBound(
                                (int)(Element.Position.x + Element.ScrollXPosition), (int)Element.ScrollXSize.Width,
                                (int)(Element.Position.y + Element.Size.Height - Element.ScrollXSize.Height), (int)Element.ScrollXSize.Height,
                                Mouse.Cursor.x, Mouse.Cursor.y))
                    {
                        ActiveScrollX = true;
                        ScrollXOffset = (Mouse.Cursor.x - Element.ScrollXPosition);
                    }
                }
                else if(Event.IsButton && Event.Button.IsUp)
                {
                    ActiveScrollY = false;
                    ActiveScrollX = false;
                }
                else if(ActiveScrollY && Event.IsCursor)
                {
                    float offset = (Mouse.Cursor.y - ScrollYOffset);
                    if (offset < 0)
                        offset = 0;
                    if (offset > (Element.Size.Height - Element.ScrollYSize.Height))
                        offset = (Element.Size.Height - Element.ScrollYSize.Height);
                    Element.ScrollYPosition = offset;
                }
                else if(ActiveScrollX && Event.IsCursor)
                {
                    float offset = (Mouse.Cursor.x - ScrollXOffset);
                    if(offset < 0)
                        offset = 0;
                    if (offset > (Element.Size.Width - Element.ScrollXSize.Width))
                        offset = (Element.Size.Width - Element.ScrollXSize.Width);
                    Element.ScrollXPosition = offset;
                }
            }
        }
    }
}
