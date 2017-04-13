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
        public VisualType Type;
        public VisualElement Parent;
        public Room Room;
        public Spacing Margin;
        public Spacing Padding;
        public Size Size;
        public Size Bound;
        public Position Position;
        public VisualElementList Nodes;
        public InputListener InputListener;
        public bool Display = true;
        public Color Color;
        public bool Active;
        public bool Focus;

        public VisualElement() : this(VisualType.Block)
        { }

        public VisualElement(int Type) : this((VisualType)Type)
        { }

        public VisualElement(VisualType Type)
        {
            this.Type = Type;
        }

        public VisualElement add(VisualElement Element)
        {
            if (Nodes == null)
                Nodes = new VisualElementList();
            Element.Parent = this;
            Nodes.Add(Element);
            return this;
        }

        public VisualElement add(VisualElement a, VisualElement b)
        {
            this.add(a);
            this.add(b);
            return this;
        }

        public void clear()
        {
            if (Nodes != null){
                Nodes.Clear();
                Nodes = null;
            }
        }

        public VisualElement this[int Index]
        {
            get { return (Nodes != null && Nodes.Size > Index ? Nodes[Index] : null); }
            set { Nodes[Index] = value; }
        }


        public VisualElement content
        {
            get { return (Nodes != null ? Nodes[0] : null); }
            set {
                Nodes = new VisualElementList();
                this.add(value);
            }
        }

        public string color
        {
            set { Color = Color.Try(value); }
        }

        public bool display
        {
            set { Display = value; }
            get { return Display; }
        }

        public int marginLeft
        {
            set { Margin = new Spacing(value, 0, 0, 0); }
        }

        public int paddingLeft
        {
            set { Padding = new Spacing(value, 0, 0, 0); }
        }

        public string source
        {
            set { (this as VisualImageElement).Source = value; }
        }

        public virtual void Draw(float X=0, float Y=0, float OffsetX=0, float OffsetY=0, float Width=0, float Height=0)
        {
            if(!Display) return;

            Position position = GetPositionOffset(GetPositionOffset(new Position(X, Y), Margin), Padding);
            X = position.x;
            Y = position.y;

            if (Type == VisualType.Text)
            {
                (this as VisualTextElement).DrawText(this.Color, X, Y, OffsetX, OffsetY, Width, Height);
                return;
            }

            if(Type == VisualType.Image)
            {
                (this as VisualImageElement).DrawImage(X, Y, OffsetX, OffsetY, Size.Width, Size.Height);
                return;
            }

            if (Type == VisualType.Scroll)
            {
                if (Nodes != null && Nodes.Size == 1)
                {
                    VisualElement child = Nodes[0];
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
                /*
                GL.Color4(1f, 1f, 1f, 0.2f);
                GL.LineWidth(1f);
                GL.Begin(PrimitiveType.LineLoop);
                GL.Vertex2(X, Y);
                GL.Vertex2(X + w, Y);
                GL.Vertex2(X + w, Y + h);
                GL.Vertex2(X, Y + h);
                GL.End();
                */
            }

            // calculate childrens
            if (Nodes == null) return;
            for (int i = 0; i < Nodes.Size; i++)
            {
                VisualElement child = Nodes[i];
                child.Draw(child.Position.x, child.Position.y);
            }
        }

        public void SetSize(Size BaseSize)
        {
            this.Size = GetSizeWithSpacing(BaseSize, this.Padding);
            this.Bound = GetSizeWithSpacing(Size, this.Margin);
            int i = 0;
        }

        public void CalculateSizeAndPosition(Position InitialPosition)
        {
            // initial position with margin
            Position = GetPositionOffset(InitialPosition, Margin);

            // not to display
            if (!Display){
                SetSize(new Size(0f, 0f));
                return;
            }

            // text-size
            if (Type == VisualType.Text){
                SetSize((this as VisualTextElement).TextHandle.Size);
                return;
            }

            // break-size
            if (Type == VisualType.Break){
                SetSize(new Size(1, new UI.Text(" ", null).Size.Height));
                return;
            }

            // size from definition
            float width = GetWidth(this);
            float height = GetHeight(this);

            // default image size
            if (Type == VisualType.Image && (width <= 0f || height <= 0f))
            {
                VisualImageElement imageElement = (this as VisualImageElement);
                if(imageElement.ImageHandle == null) return;
                Size imgSize = imageElement.ImageHandle.Size;
                if(width <= 0f && height <= 0f)
                {
                    width = imgSize.Width;
                    height = imgSize.Height;
                }
                if (width <= 0f)
                {
                    float factor = (imgSize.Width / imgSize.Height);
                    width = height * factor;
                }
                else if(height <= 0f)
                {
                    float factor = (imgSize.Height / imgSize.Width);
                    height = width * factor;
                }
                SetSize(new Size(width, height));
                return;
            }

            // default input size
            if (Type == VisualType.Input && (width <= 0f || height <= 0f))
            {
                Size textSize = new Text(new string(' ', 15), null).Size;
                if(width == 0f) width = textSize.Width;
                if(height == 0f) height = textSize.Height;
                SetSize(new Size(width, height));
                return;
            }

            // total width from parent
            if (width <= 0f && (Type == VisualType.Block || Type == VisualType.Scroll)){
                if ((width = ParentWidth(this)) != 0f){
                    ;
                }
            }

            // total height from parent
            if (height <= 0f && (Type == VisualType.Column || Type == VisualType.Scroll)){
                if ((height = ParentHeight(this)) != 0f){
                    ;
                }
            }

            // base size without padding
            Size baseSize = GetSizeWithoutSpacing(new Size(width, height), Padding);

            // calculate childrens
            Size allBound = new Size(0f, 0f);
            Position childsPosition = GetPositionOffset(Position, Padding);
            float currentLeft = 0f;
            float currentTop = 0f;
            float currentHeight = 0f;
            for (int i = 0; Nodes != null && i < Nodes.Size; i++)
            {
                VisualElement child = Nodes[i];
                child.CalculateSizeAndPosition(new Position(childsPosition.x + currentLeft, childsPosition.y + currentTop));
                Size bound = child.Bound;

                if (child.Type == VisualType.Block || child.Type == VisualType.Scroll)
                {
                    if(bound.Width > allBound.Width)
                        allBound.Width = bound.Width;
                    allBound.Height += bound.Height;
                    currentTop += bound.Height;
                    currentLeft = 0f;
                    currentHeight = 0f;
                }
                else if(child.Type == VisualType.Inline || child.Type == VisualType.Column || 
                        child.Type == VisualType.Text || child.Type == VisualType.Input || child.Type == VisualType.Image
                ){
                    allBound.Width += bound.Width;
                    currentLeft += bound.Width;
                    if (bound.Height > currentHeight){
                        allBound.Height += (bound.Height - currentHeight);
                        currentHeight = bound.Height;
                    }
                }
                else if (child.Type == VisualType.Break)
                {
                    currentTop += currentHeight;
                    currentLeft = 0f;
                    currentHeight = 0f;
                }
            }

            // may extend element size 
            if (Type != VisualType.Scroll)
            {
                if (allBound.Width > baseSize.Width)
                    baseSize.Width = allBound.Width;
                if (allBound.Height > baseSize.Height)
                    baseSize.Height = allBound.Height;
            }

            // overall size
            SetSize(baseSize);

            // done, may zero or unknown
            return;
        }


        public static Size GetSizeWithSpacing(Size Size, Spacing Spacing, float factor = 1f)
        {
            Size newSize = new Size(Size);
            if (Spacing == null)
                return newSize;
            if (Spacing.Left != null)
                newSize.Width += (factor * Spacing.Left.way);
            if (Spacing.Top != null)
                newSize.Height += (factor * Spacing.Top.way);
            if (Spacing.Right != null)
                newSize.Width += (factor * Spacing.Right.way);
            if (Spacing.Bottom != null)
                newSize.Height += (factor * Spacing.Bottom.way);
            return newSize;
        }

        public static Size GetSizeWithoutSpacing(Size Size, Spacing Spacing)
        {
            return GetSizeWithSpacing(Size, Spacing, -1f);
        }

        public static Position GetPositionOffset(Position Position, Spacing Spacing)
        {
            Position newPosition = new Position(Position);
            if (Spacing == null)
                return newPosition;
            if (Spacing.Left != null)
                newPosition.x += Spacing.Left.way;
            if (Spacing.Top != null)
                newPosition.y += Spacing.Top.way;
            return newPosition;
        }

        public static float GetWidth(VisualElement Element)
        {
            Room room = Element.Room;
            float width = 0f;

            // width definition
            if (room != null && room.Width != null)
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
                    width = (parentWidth != -0f ? room.Width.way * parentWidth : -0f);
                }
            }
            return width;
        }

        public static float GetHeight(VisualElement Element)
        {
            Room room = Element.Room;
            float height = 0f;

            // height definition
            if (room != null && room.Height != null)
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
                    height = (parentHeight != 0f ? (room.Height.way * parentHeight) : 0f);
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
                if ((width = GetWidth(parentElement)) != 0f)
                {
                    return width;
                }
                parentElement = parentElement.Parent;
            }
            return 0f;
        }

        public static float ParentHeight(VisualElement Element)
        {
            float height;

            // try to get absolute size from parent
            VisualElement parentElement = Element.Parent;
            while (parentElement != null)
            {
                if ((height = GetHeight(parentElement)) != 0f)
                {
                    return height;
                }
                parentElement = parentElement.Parent;
            }
            return 0f;
        }

    }

    public abstract class VisualListener : InputListener
    {
        public VisualElement Element;

        public VisualListener(VisualElement Element)
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
                bool doFocus = GeometryUtils.IntersectVisual(Element, Event.Cursor);
                if(!Element.Focus && doFocus)
                    this.Event(new InputEvent(InputType.Visual, new VisualState(VisualEventState.Focus, true)));
                if(Element.Focus && !doFocus)
                    this.Event(new InputEvent(InputType.Visual, new VisualState(VisualEventState.Focus, false)));
                Element.Focus = doFocus;
                if (doFocus)
                    this.Event(Event);
            }
            else if(Event.IsButton)
            {
                bool doActive = GeometryUtils.IntersectVisual(Element, Cursor);
                if(!Element.Active && doActive)
                    this.Event(new InputEvent(InputType.Visual, new VisualState(VisualEventState.Active, true)));
                if(Element.Active && !doActive)
                    this.Event(new InputEvent(InputType.Visual, new VisualState(VisualEventState.Active, false)));
                Element.Active = doActive;
                if (doActive)
                    this.Event(Event);
            }
            else if(Element.Active && Event.IsKey)
            {
                this.Event(Event);
            }
            else if(Element.Active && Event.IsText)
            {
               this.Event(Event);
            }
        }

        public abstract void Event(InputEvent Event);

    }

    public class VisualElementList : ListCollection<VisualElement>
    { }

    public class VisualTextElement : VisualElement
    {
        public Text TextHandle;

        public string Text
        {
            get{ return (TextHandle.String); }
            set{ TextHandle = new UI.Text(value, null); }
        }

        public VisualTextElement(string String, Color Color=null) : base(VisualType.Text)
        {
            this.Text = String;
            this.Color = Color;
        }

        public void DrawText(Color Color, float X =0, float Y=0, float OffsetX=0, float OffsetY=0, float Width=0, float Height=0)
        {
            this.TextHandle.Draw(Color, X, Y, OffsetX, OffsetY, Width, Height);
        }
    }

    public class VisualImageElement : VisualElement
    {
        public Image ImageHandle;

        public VisualImageElement() : base(VisualType.Image)
        { }

        public string Source
        {
            set{
                this.ImageHandle = new Image(value);
            }
        }

        public void DrawImage(float X = 0, float Y = 0, float OffsetX = 0, float OffsetY = 0, float Width = 0, float Height = 0)
        {
            if(this.ImageHandle == null) return;
            this.ImageHandle.Draw(X, Y, Width, Height);
        }
    }

    public class VisualInputElement : VisualElement
    {
        public string Text
        {
            get
            {
                if (Nodes != null && Nodes[0].Type == VisualType.Text)
                    return (Nodes[0] as VisualTextElement).Text;
                return "";
            }
            set
            {
                if (Nodes != null && Nodes[0].Type == VisualType.Text)
                    (Nodes[0] as VisualTextElement).Text = (value != null ? value : "");
            }
        }

        public VisualInputElement() : base(VisualType.Input)
        {
            this.InputListener = new TextListener(this);
        }

        public class TextListener : VisualListener
        {
            public TextListener(VisualInputElement Element) : base(Element)
            { }

            public override void Event(InputEvent Event)
            {
                //if (!Active) return;
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

        public VisualScrollElement() : base(VisualType.Scroll)
        {
            this.InputListener = new ScrollListener(this);
        }

        public void DrawScrollY(float X = 0, float Y = 0)
        {
            if(Nodes == null|| Nodes.Size != 1)
                return;

            VisualElement child = Nodes[0];
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
            if (Nodes == null || Nodes.Size != 1)
                return;
    
            VisualElement child = Nodes[0];
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
                    if(GeometryUtils.IntersectBound(
                            (int)(Element.Position.x + Element.Size.Width - Element.ScrollYSize.Width), (int)Element.ScrollYSize.Width,
                            (int)(Element.Position.y + Element.ScrollYPosition), (int)Element.ScrollYSize.Height,
                            Mouse.Cursor.x, Mouse.Cursor.y))
                    {
                        ActiveScrollY = true;
                        ScrollYOffset = (Mouse.Cursor.y - Element.ScrollYPosition);
                    }
                    else if(GeometryUtils.IntersectBound(
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
