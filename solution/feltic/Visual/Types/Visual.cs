using feltic.Language;
using feltic.Library;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Visual
{
    public class VisualElement
    {
        // node-definition
        public VisualType Type;
        public VisualElement Parent;
        public VisualElementList Nodes;
        // public-properties
        public Room Room;
        public Spacing Margin;
        public Spacing Padding;
        public Position Offset;
        public Size Clip;
        public Position Position;
        public Size Size;
        public bool Display = true;
        public Color Color;
        // render-properties
        public bool Render = true;
        public Position RenderPosition;
        public Size RenderSize;
        public Size RenderBound;
        public Position RenderOffset;
        public Size RenderClip;
        // visual input-state
        public InputListener InputListener;
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
                value.Type = Type;
                this.add(value);
            }
        }

        public VisualType type
        {
            get { return Type; }
            set { this.Type = value; }
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

        public float marginLeft
        {
            set { Margin = Spacing.Combine(Margin, new Spacing(value, 0, 0, 0)); }
        }

        public float paddingLeft
        {
            set { Padding = Spacing.Combine(Padding, new Spacing(value, 0, 0, 0)); }
        }

        public string source
        {
            set { (this as VisualImage).Source = value; }
        }

        public virtual void Draw()
        {
            if (!Display || !Render) return;

            if (RenderSize.Width > 0f && RenderSize.Height > 0f)
            {
                GL.Color4(1f, 1f, 1f, 0.2f);
                GL.LineWidth(1f);
                GL.Begin(PrimitiveType.LineLoop);
                GL.Vertex2(RenderPosition.X, RenderPosition.Y);
                GL.Vertex2(RenderPosition.X + RenderSize.Width, RenderPosition.Y);
                GL.Vertex2(RenderPosition.X + RenderSize.Width, RenderPosition.Y + RenderSize.Height);
                GL.Vertex2(RenderPosition.X, RenderPosition.Y + RenderSize.Height);
                GL.End();
            }

            if (Type == VisualType.Text)
            {
                (this as VisualText).DrawText(RenderPosition, RenderSize, Color, RenderOffset, RenderClip);
                return;
            }

            if(Type == VisualType.Image)
            {
                (this as VisualImage).DrawImage(RenderPosition, RenderSize, RenderOffset, RenderClip);
                return;
            }

            if (Type == VisualType.Scroll)
            {
                if (Nodes != null && Nodes.Size == 1)
                {
                    VisualElement child = Nodes[0];
                    VisualScrollElement scroll = this as VisualScrollElement;
                    float factorHeight = (child.RenderSize.Height / scroll.RenderSize.Height);
                    float offsetHeight = (scroll.ScrollYPosition * factorHeight);
                    float factorWidth = (child.RenderSize.Width / scroll.RenderSize.Width);
                    float offsetWidth = (scroll.ScrollXPosition * factorWidth);
                    child.Draw();
                    //scroll.DrawScrollY(X, Y);
                    //scroll.DrawScrollX(X, Y);
                    return;
                }
            }

            // calculate childrens
            if(Nodes == null) return;
            for (int i = 0; i < Nodes.Size; i++){
                Nodes[i].Draw();
            }
        }

        public void ClearRenderState()
        {
            Render = false;
            Position = null;
            Size = null;
            RenderPosition = null;
            RenderSize = null;
            RenderBound = null;
            RenderOffset = null;
        }

        public void SetRenderState(Position BasePosition, Position MarginStart, Position PaddingStart, Size BaseSize, Size PreferSize, Position BaseOffset)
        {
            // clear state
            ClearRenderState();

            if(BaseSize == null) return;

            // size
            RenderSize = new Size(BaseSize);
            if (PreferSize != null){
                // prefer-size without padding
                Size preferSize = new Size(PreferSize);
                preferSize.Minus(GetTopLeftSpacing(new Position(0, 0), Padding));
                preferSize.Minus(GetRightBottomSpacing(new Position(0, 0), Padding));
                if (preferSize.Width > BaseSize.Width)
                    RenderSize.Width = preferSize.Width;
                if (preferSize.Height > BaseSize.Height)
                    RenderSize.Height = preferSize.Height;
            }

            // position
            Position = new Position(MarginStart);
            RenderPosition = new Position(PaddingStart);

            // bound
            RenderBound = new Size(RenderSize);
            RenderBound.Plus(Position.Minus(RenderPosition, BasePosition));
            RenderBound.Plus(GetRightBottomSpacing(new Position(0, 0), Padding));
            RenderBound.Plus(GetRightBottomSpacing(new Position(0, 0), Margin));

            // offset
            if (BaseOffset != null && (BaseOffset.X > 0 || BaseOffset.Y > 0))
            {
                Position diff = GetMinusDifference(new Position(RenderSize.Width, RenderSize.Height), BaseOffset);
                RenderOffset = diff;
                BaseOffset.Minus(diff);
                RenderSize.Minus(diff);
                RenderBound.Minus(diff);
            }

            // clip
            if(Clip != null && (Clip.Width > 0 || Clip.Height > 0))
            {
                Position diff = GetMinusDifference(new Position(RenderSize.Width, RenderSize.Height), new Position(Clip.Width, Clip.Height));
                Position size = Position.Minus(new Position(RenderSize.Width, RenderSize.Height), diff);
                // update children state
                SetRenderStateSecond(new Size(size.X, size.Y));
            }

            // render
            if (RenderSize != null && (RenderSize.Width > 0 && RenderSize.Height > 0)){
                Render = true;
            }else{
                ClearRenderState();
            }
        }

        public void SetRenderStateSecond(Size MaxSize)
        {
            Position diff = new Position();
            if (MaxSize.Width < RenderBound.Width)
                diff.X = (RenderBound.Width - MaxSize.Width);
            if (MaxSize.Height < RenderBound.Height)
                diff.Y = (RenderBound.Height - MaxSize.Height);
            RenderSize.Minus(diff);
            RenderBound.Minus(diff);
            RenderClip = new Size(diff.X, diff.Y);
            if (RenderBound.Width <= 0 || RenderBound.Height <= 0)
            {
                ClearRenderState();
                return;
            }
            if (Nodes == null) return;
            Size maxSize = new Size(RenderBound);
            for (int i = 0; i < Nodes.Size; i++)
            {
                VisualElement child = Nodes[i];
                if(!child.Render) continue;
                Size boundBefore = new Size(child.RenderBound);
                child.SetRenderStateSecond(maxSize);
                Size boundAfter = new Size(child.RenderBound);
                if (child.Type == VisualType.Block || child.Type == VisualType.Scroll)
                {
                    maxSize = new Size(MaxSize.Width, maxSize.Height - (boundAfter.Height > 0 &&  boundAfter.Height < boundBefore.Height ? boundBefore.Height - boundAfter.Height : boundBefore.Height));
                }
                else if(child.Type == VisualType.Inline || child.Type == VisualType.Column ||
                        child.Type == VisualType.Text || child.Type == VisualType.Image || child.Type == VisualType.Input
                ){
                    maxSize = new Size((boundAfter.Width > 0 && boundAfter.Width < boundBefore.Width ? boundBefore.Width - boundAfter.Width : boundBefore.Width), MaxSize.Height);
                }
            }
        }

        public static Position GetMinusDifference(Position Base, Position Minus)
        {
            if (Minus == null || (Minus.X == 0 && Minus.Y == 0))
                return new Position(0, 0);
            float dx, dy;
            if (Minus.X >= Base.X)
                dx = Base.X;
            else
                dx = Minus.X;
            if (Minus.Y >= Base.Y)
                dy = Base.Y;
            else
                dy = Minus.Y;
            return new Position(dx, dy);
        }

        public void Metrics(Position BasePosition=null, Position BaseOffset=null)
        {
            // not to display
            if (!Display){
                SetRenderState(BasePosition, null, null, null, null, null);
                return;
            }

            // offset
            if (BaseOffset == null)
                BaseOffset = new Position(0f, 0f);
            if (Offset != null)
                BaseOffset.Plus(Offset);
            Position offset = new Position(BaseOffset);

            // element start
            Position position;
            position = GetTopLeftSpacing(new Position(BasePosition), Margin);
            position.Minus(GetMinusDifference(Position.Minus(position, BasePosition), BaseOffset));
            Position marginStart = new Position(position);
            position = GetTopLeftSpacing(new Position(BasePosition), Margin);
            position = GetTopLeftSpacing(new Position(position), Padding);
            position.Minus(GetMinusDifference(Position.Minus(position, BasePosition), BaseOffset));
            Position paddingStart = new Position(position);

            // text-size
            if (Type == VisualType.Text){
                SetRenderState(BasePosition, marginStart, paddingStart, (this as VisualText).TextHandle.Size, null, BaseOffset);
                return;
            }

            // break-size
            if (Type == VisualType.Break){
                SetRenderState(BasePosition, marginStart, paddingStart, new Size(1, new Visual.Text(" ", null).Size.Height), null, BaseOffset);
                return;
            }

            // size from definition
            float width = GetWidth(this);
            float height = GetHeight(this);

            // default image size
            if (Type == VisualType.Image)
            {
                Size imageSize = (this as VisualImage).ImageHandle.Size;
                if(width <= 0f && height <= 0f)
                {
                    width = imageSize.Width;
                    height = imageSize.Height;
                }
                if (width <= 0f)
                {
                    float factor = (imageSize.Width / imageSize.Height);
                    width = height * factor;
                }
                else if(height <= 0f)
                {
                    float factor = (imageSize.Height / imageSize.Width);
                    height = width * factor;
                }
                SetRenderState(BasePosition, marginStart, paddingStart, new Size(width, height), null, BaseOffset);
                return;
            }

            // default input size
            if (Type == VisualType.Input)
            {
                Size textSize = new Text(new string(' ', 15), null).Size;
                if(width <= 0f) width = textSize.Width;
                if(height <= 0f) height = textSize.Height;
                SetRenderState(BasePosition, marginStart, paddingStart, new Size(width, height), null, BaseOffset);
                return;
            }

            // total width from parent
            if (width <= 0f && (Type == VisualType.Block || Type == VisualType.Scroll))
            {
                if ((width = ParentWidth(this)) != 0f)
                {
                    ;
                }
            }

            // total height from parent
            if (height <= 0f && (Type == VisualType.Column || Type == VisualType.Scroll))
            {
                if ((height = ParentHeight(this)) != 0f)
                {
                    ;
                }
            }

            // calculate childrens
            Size allBound = new Size(0f, 0f);
            float currentLeft = 0f;
            float currentTop = 0f;
            float currentHeight = 0f;
            for (int i = 0; Nodes != null && i < Nodes.Size; i++)
            {
                VisualElement child = Nodes[i];
                Position childPosition = new Position(paddingStart.X + currentLeft, paddingStart.Y + currentTop);
                if(currentLeft == 0f && offset != null && offset.X > 0)
                    BaseOffset.X = offset.X; // xoffset on left-begin for all childs
                child.Metrics(childPosition, BaseOffset);
                if (!child.display || !child.Render) continue;
                Size bound = child.RenderBound;
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
                        child.Type == VisualType.Text ||  child.Type == VisualType.Image || child.Type == VisualType.Input
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

            // overall size
            SetRenderState(BasePosition, marginStart, paddingStart, allBound, new Size(width, height), BaseOffset);

            // done, may zero or unknown
            return;
        }

        public static Position GetTopLeftSpacing(Position Position, Spacing Spacing)
        {
            Position newPosition = new Position(Position);
            if (Spacing == null)
                return newPosition;
            if (Spacing.Left != null)
                newPosition.X += Spacing.Left.way;
            if (Spacing.Top != null)
                newPosition.Y += Spacing.Top.way;
            return newPosition;
        }

        public static Position GetRightBottomSpacing(Position Position, Spacing Spacing)
        {
            Position newPosition = new Position(Position);
            if (Spacing == null)
                return newPosition;
            if (Spacing.Right != null)
                newPosition.X += Spacing.Right.way;
            if (Spacing.Bottom != null)
                newPosition.Y += Spacing.Bottom.way;
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
            if (Element == null || Element.RenderSize == null || Element.RenderPosition == null)
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

    public class VisualText : VisualElement
    {
        public Text TextHandle;

        public string Text
        {
            get{ return (TextHandle.String); }
            set{ TextHandle = new Visual.Text(value, null); }
        }

        public VisualText(string String, Color Color=null) : base(VisualType.Text)
        {
            this.Text = String;
            this.Color = Color;
        }

        public void DrawText(Position Position, Size Size, Color Color, Position Offset =null, Size Clip=null)
        {
            this.TextHandle.Draw(Position, Size, Color, Offset, Clip);
        }
    }

    public class VisualImage : VisualElement
    {
        public Image ImageHandle;

        public VisualImage() : base(VisualType.Image)
        { }

        public VisualImage(string Filepath) : base(VisualType.Image)
        {
            this.source = Filepath;
        }

        public string Source
        {
            set{
                this.ImageHandle = new Image(value);
            }
        }

        public void DrawImage(Position Position, Size Size, Position Offset=null, Size Clip=null)
        {
            this.ImageHandle.Draw(Position, Size, Offset, Clip);
        }
    }

    public class VisualInputElement : VisualElement
    {
        public string Text
        {
            get
            {
                if (Nodes != null && Nodes[0].Type == VisualType.Text)
                    return (Nodes[0] as VisualText).Text;
                return "";
            }
            set
            {
                if (Nodes != null && Nodes[0].Type == VisualType.Text)
                    (Nodes[0] as VisualText).Text = (value != null ? value : "");
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
            Size childSize = child.RenderSize;

            float factor = (RenderSize.Height / childSize.Height);
            ScrollYSize.Height = (RenderSize.Height * factor);
            
            float w = ScrollYSize.Width;
            float h = ScrollYSize.Height;
            float x = RenderPosition.X + RenderSize.Width - w;
            float y = RenderPosition.Y + ScrollYPosition;

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
            Size childSize = child.RenderSize;

            float factor = (RenderSize.Width / childSize.Width);
            ScrollXSize.Width = (RenderSize.Width * factor);

            float w = ScrollXSize.Width;
            float h = ScrollXSize.Height;
            float x = RenderPosition.X + ScrollXPosition;
            float y = RenderPosition.Y + RenderSize.Height - h;

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
                            (int)(Element.RenderPosition.X + Element.RenderSize.Width - Element.ScrollYSize.Width), (int)Element.ScrollYSize.Width,
                            (int)(Element.RenderPosition.Y + Element.ScrollYPosition), (int)Element.ScrollYSize.Height,
                            Mouse.Cursor.x, Mouse.Cursor.y))
                    {
                        ActiveScrollY = true;
                        ScrollYOffset = (Mouse.Cursor.y - Element.ScrollYPosition);
                    }
                    else if(GeometryUtils.IntersectBound(
                           (int)(Element.RenderPosition.X + Element.ScrollXPosition), (int)Element.ScrollXSize.Width,
                           (int)(Element.RenderPosition.Y + Element.RenderSize.Height - Element.ScrollXSize.Height), (int)Element.ScrollXSize.Height,
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
                    if (offset > (Element.RenderSize.Height - Element.ScrollYSize.Height))
                        offset = (Element.RenderSize.Height - Element.ScrollYSize.Height);
                    Element.ScrollYPosition = offset;
                }
                else if(ActiveScrollX && Event.IsCursor)
                {
                    float offset = (Mouse.Cursor.x - ScrollXOffset);
                    if(offset < 0)
                        offset = 0;
                    if (offset > (Element.RenderSize.Width - Element.ScrollXSize.Width))
                        offset = (Element.RenderSize.Width - Element.ScrollXSize.Width);
                    Element.ScrollXPosition = offset;
                }
            }
        }
    }
}
