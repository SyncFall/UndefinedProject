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
        public Room RoomFromDefinition;
        public Size AbsoluteSize;
        public Position AbsolutePosition;
        public VisualElementList Childrens;
        public VisualInputListener InputListener;

        public VisualElement(VisualElementType Type, VisualElement Parent)
        {
            this.Type = Type;
            this.Parent = Parent;
            this.RoomFromDefinition = new Room();
            if (Parent != null)
                Parent.AddChild(this);
        }

        public void AddChild(VisualElement VisualElement)
        {
            if (Childrens == null)
                Childrens = new VisualElementList();
            Childrens.Add(VisualElement);
        }

        public virtual void Draw(float X = 0, float Y = 0)
        {
            if (Type == VisualElementType.Text)
            {
                (this as VisualTextElement)._TextHandle.Draw(X, Y - 3);
                return;
            }

            if (AbsoluteSize != null && AbsoluteSize.Width != -1f && AbsoluteSize.Height != -1f)
            {
                GL.Color3(1f, 1f, 1f);
                GL.LineWidth(1f);
                GL.Begin(PrimitiveType.LineLoop);
                GL.Vertex3(X, Y, 0);
                GL.Vertex3(X + AbsoluteSize.Width, Y, 0);
                GL.Vertex3(X + AbsoluteSize.Width, Y + AbsoluteSize.Height, 0);
                GL.Vertex3(X, Y + AbsoluteSize.Height, 0);
                GL.End();
            }

            if (Childrens == null)
                return;

            float currentX = X;
            float currentY = Y;
            for (int i = 0; i < Childrens.Size; i++)
            {
                VisualElement childElement = Childrens[i];
                childElement.Draw(currentX, currentY);
                Size childSize = childElement.AbsoluteSize;
                if (childSize != null && childSize.Width != -1f && childSize.Height != -1f)
                {
                    if (childElement.Type == VisualElementType.Block)
                    {
                        currentY += childSize.Height;
                    }
                    else if (childElement.Type == VisualElementType.Inline || childElement.Type == VisualElementType.Column)
                    {
                        currentX += childSize.Width;
                    }
                }
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
            if (Element == null || Element.AbsoluteSize == null || Element.AbsolutePosition == null)
            {
                return;
            }
            if(Event.IsCursor)
            {
                if(GeometryUtils.IntersetBound((int)Element.AbsolutePosition.x, (int)Element.AbsoluteSize.Width, (int)Element.AbsolutePosition.y, (int)Element.AbsoluteSize.Height, Event.Cursor.x, Event.Cursor.y))
                {
                    this.Event(Event);
                }
            }
            else if(Event.IsButton)
            {
                Active = (GeometryUtils.IntersetBound((int)Element.AbsolutePosition.x, (int)Element.AbsoluteSize.Width, (int)Element.AbsolutePosition.y, (int)Element.AbsoluteSize.Height, Mouse.Cursor.x, Mouse.Cursor.y));
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
            Room room = Element.RoomFromDefinition;
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
            Room room = Element.RoomFromDefinition;
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
            if (Element.AbsolutePosition != null)
            {
                return Element.AbsolutePosition;
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
                }
            }

            return (Element.AbsolutePosition = new Position(X, Y));
        }

        public static Size GetSize(VisualElement Element)
        {
            // return already calculated
            if (Element.AbsoluteSize != null)
            {
                return Element.AbsoluteSize;
            }

            // text-size
            if (Element.Type == VisualElementType.Text)
            {
                return (Element.AbsoluteSize = (Element as VisualTextElement)._TextHandle.Size);
            }

            // size from definition
            float width = GetWidth(Element);
            float height = GetHeight(Element);

            // total width from parents for block element
            if (width == -1f && Element.Type == VisualElementType.Block)
            {
                if ((width = ParentWidth(Element)) != -1f)
                {
                    ;
                }
            }

            // total height from parents for columen element
            if (height == -1f && Element.Type == VisualElementType.Column)
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
                if (width == -1f) width = (size.Width);
                if (height == -1f) height = (size.Height);
            }

            // size from childrens
            float childsWidth = 0f;
            float childsHeight = 0f;
            for (int i = 0; Element.Childrens != null && i < Element.Childrens.Size; i++)
            {
                VisualElement childElement = Element.Childrens[i];
                Size childSize = GetSize(childElement);
                if (childElement.Type == VisualElementType.Block)
                {
                    if (childSize.Width > childsWidth)
                    {
                        childsWidth = childSize.Width;
                    }
                    childsHeight += childSize.Height;
                }
                else if (childElement.Type == VisualElementType.Inline || childElement.Type == VisualElementType.Column)
                {
                    childsWidth += childSize.Width;
                    if (childSize.Height > childsHeight)
                    {
                        childsHeight = childSize.Height;
                    }
                }
                else if (childElement.Type == VisualElementType.Text || childElement.Type == VisualElementType.Input)
                {
                    childsWidth = childSize.Width;
                    childsHeight = childSize.Height;
                }
            }

            // extend element size
            if (childsWidth > width)
                width = childsWidth;
            if (childsHeight > height)
                height = childsHeight;

            // may complete, zero or unknown
            return (Element.AbsoluteSize = new Size(width != -1 ? width : 0, height != -1 ? height : 0));
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

        public VisualTextElement(string String, VisualElement Parent) : base(VisualElementType.Text, Parent)
        {
            this.Text = String;
        }
    }
}
