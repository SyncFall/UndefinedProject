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
    public class VisualRenderState
    {
        // common
        public bool True = false;
        public Position Base;
        public Position Position;
        public Size Size;
        public Size Bound;
        public Position Offset;
        public Size Clip;
        // specific
        public Position TopMargin;
        public Position TopPadding;
        public Position BottomPadding;
        public Position BottomMargin;  
        
        public VisualRenderState()
        { }
    }

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
        // render-state
        public VisualRenderState Render;
        // input-state
        public InputListener Listener;
        public bool Active;
        public bool Focus;

        public VisualElement()
        {
            this.Type = VisualType.Block;
        }

        public VisualElement(VisualType Type)
        {
            this.Type = Type;
        }

        public VisualElement(int Type)
        {
            this.Type = (VisualType)Type;
        }

        public VisualElement add(VisualElement Element)
        {
            if(Nodes == null)
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
            set {
                if(this as VisualImage != null)
                    (this as VisualImage).Source = value;
            }
        }

        public virtual void Draw()
        {
            if(!Display || Render == null || !Render.True || Type == VisualType.Break) return;

            GL.Color4(1f, 1f, 1f, 0.2f);
            GL.LineWidth(1f);
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex2(Render.Position.X, Render.Position.Y);
            GL.Vertex2(Render.Position.X + Render.Size.Width, Render.Position.Y);
            GL.Vertex2(Render.Position.X + Render.Size.Width, Render.Position.Y + Render.Size.Height);
            GL.Vertex2(Render.Position.X, Render.Position.Y + Render.Size.Height);
            GL.End();

            if (Type == VisualType.Text)
            {
                (this as VisualText).DrawText(Render.Position, Render.Size, Color, Render.Offset, Render.Clip);
                return;
            }

            if(Type == VisualType.Image)
            {
                (this as VisualImage).DrawImage(Render.Position, Render.Size, Render.Offset, Render.Clip);
                return;
            }

            if (Type == VisualType.Scroll)
            {
                VisualScroll scroll = this as VisualScroll;
                VisualElement child = Nodes[0];
                Size scrollSize = new Size(scroll.Render.Size);
                Size originalSize = new Size(scroll.UncutSize);
                float factorX = (originalSize.Width / scrollSize.Width);
                float factorY = (originalSize.Height / scrollSize.Height);
                scroll.ValidateScroll(originalSize);
                child.Offset = new Position(factorX * scroll.ScrollXPosition * 0.99999f, factorY * scroll.ScrollYPosition * 0.99999f);
                child.Clip = new Size(Visual.Size.Minus(originalSize, scrollSize).Minus(child.Offset));
                scroll.DrawScrollY();
                scroll.DrawScrollX();
            }

            // draw childrens
            if(Nodes == null) return;
            for(int i=0; i<Nodes.Size; i++){
                Nodes[i].Draw();
            }
        }

        public void ClearRenderState()
        {
            Render = new VisualRenderState();
        }

        public void SetRenderState(Position BasePosition, Position BaseOffset, Size BaseSize, Size PreferSize)
        {
            // size
            Render.Size = new Size(BaseSize);
            if(PreferSize != null){
                // prefer size
                Size preferSize = new Size(PreferSize);
                preferSize.Minus(Render.TopPadding);
                preferSize.Minus(GetRightBottomSpacing(Padding));
                if(Type == VisualType.Scroll){
                    Render.Size.Width = preferSize.Width;
                    Render.Size.Height = preferSize.Height;
                }else{
                    if(preferSize.Width > BaseSize.Width)
                        Render.Size.Width = preferSize.Width;
                    if(preferSize.Height > BaseSize.Height)
                        Render.Size.Height = preferSize.Height;
                }
            }

            // position
            Position = Position.Plus(Render.Base, Render.TopMargin);
            Render.Position = Position.Plus(Position, Render.TopPadding);

            // size offset
            Position diff = GetMinusDifference(new Position(Render.Size.Width, Render.Size.Height), BaseOffset);
            if(PreferSize != null && PreferSize.Width > BaseSize.Width)
                Render.Size.Minus(new Size(diff.X, 0));
            if(PreferSize != null && PreferSize.Height > BaseSize.Height)
                Render.Size.Minus(new Size(0, diff.Y));
            if(PreferSize == null)
                Render.Size.Minus(diff);
            Render.Offset = new Position(diff);
            BaseOffset.Minus(diff);

            // padding offset
            Render.BottomPadding = GetRightBottomSpacing(Padding);
            OffsetCalc(Render.BottomPadding, BaseOffset);

            // margin offset
            Render.BottomMargin = GetRightBottomSpacing(Margin);
            OffsetCalc(Render.BottomMargin, BaseOffset);

            // bound
            Render.Bound = new Size(Render.Size);
            Render.Bound.Plus(Render.TopMargin);
            Render.Bound.Plus(Render.TopPadding);
            Render.Bound.Plus(Render.BottomPadding);
            Render.Bound.Plus(Render.BottomMargin);

            // update children state
            if (Clip != null)
                SetRenderStateSecond(new Position(Clip));

            // check
            CheckForRender();
        }

        public bool CheckForRender(bool Any=true)
        {
            if(Any && Render.Bound != null && (Render.Bound.Width > 0f || Render.Bound.Height > 0f)){
                return (Render.True = true);
            }else if (!Any && Render.Bound != null && (Render.Bound.Width > 0f && Render.Bound.Height > 0f)){ 
                return (Render.True = true);
            }else{
                ClearRenderState();
                return false;
            } 
        }

        public void SetRenderStateSecond(Position BaseClip)
        {
            Position diff;

            // margin offset
            diff = OffsetCalc(Render.BottomMargin, BaseClip);
            Render.Bound.Minus(diff);

            // padding offset
            diff = OffsetCalc(Render.BottomPadding, BaseClip);
            Render.Bound.Minus(diff);

            // size offset
            diff = GetMinusDifference(new Position(Render.Size), BaseClip);
            Render.Size.Minus(diff);
            Render.Clip = new Size(diff);
            BaseClip.Minus(diff);
            Render.Bound.Minus(diff);

            // padding offset
            diff = OffsetCalc(Render.TopPadding, BaseClip);
            Render.Bound.Minus(diff);

            // margin offset
            diff = OffsetCalc(Render.TopMargin, BaseClip);
            Render.Bound.Minus(diff);

            if(!CheckForRender(false)) return;

            if(Nodes == null) return;
            Size maxSize = new Size(Render.Bound);
            float currentWidth = 0f;
            float currentHeight = 0f;
            for(int i = 0; i < Nodes.Size; i++)
            {
                VisualElement child = Nodes[i];
                if(!child.Display) continue;
                Size bound = new Size(child.Render.Bound);
                Size clip = new Size(0f, 0f);
                if(child.Type == VisualType.Block || child.Type == VisualType.Scroll)
                {
                    if(bound.Height > maxSize.Height){
                        clip.Height = bound.Height - maxSize.Height;
                        maxSize.Height = 0;
                    }else{
                        maxSize.Height -= bound.Height;
                    }
                    if(bound.Width > maxSize.Width){
                        clip.Width = bound.Width - maxSize.Width;
                    }
                }
                else if (child.Type == VisualType.Inline || child.Type == VisualType.Image || child.Type == VisualType.Text)
                {
                    if(bound.Height > maxSize.Height){
                        clip.Height = bound.Height - maxSize.Height;
                        if(maxSize.Height > currentHeight)
                            currentHeight = maxSize.Height;
                    }else{
                        if(bound.Height > currentHeight)
                            currentHeight = bound.Height;
                    }
                    if(currentWidth + bound.Width > maxSize.Width){
                        clip.Width = currentWidth + bound.Width - maxSize.Width;
                        currentWidth = maxSize.Width;
                    }else{
                        currentWidth += bound.Width;
                    }
                }
                else if(child.Type == VisualType.Break)
                {
                    maxSize.Height -= currentHeight;
                    currentWidth = 0f;
                    currentHeight = 0f;
                }
                if(child.Render.True)
                    child.SetRenderStateSecond(new Position(clip));
            }
        }

        public static Position GetMinusDifference(Position Base, Position Minus)
        {
            if(Minus == null || (Minus.X == 0 && Minus.Y == 0))
                return new Position(0f, 0f);
            float dx, dy;
            if(Minus.X >= Base.X)
                dx = Base.X;
            else
                dx = Minus.X;
            if(Minus.Y >= Base.Y)
                dy = Base.Y;
            else
                dy = Minus.Y;
            return new Position(dx, dy);
        }

        public Position OffsetCalc(Position Space, Position Offset, Position Position=null)
        {
            Position diff = GetMinusDifference(Space, Offset);
            Space.Minus(diff);
            if(Position != null)
                Position.Plus(Space);
            Offset.Minus(diff);
            return diff;
        }

        public void Metrics(Position BasePosition=null, Position BaseOffset=null)
        {
            // clear render state
            ClearRenderState();
            Render.Base = new Position(BasePosition);
            Position position = new Position(Render.Base);

            // not to display
            if(!Display){
                return;
            }

            // initial offset
            if(BaseOffset == null)
                BaseOffset = new Position(0f, 0f);

            // margin offset
            Render.TopMargin = GetTopLeftSpacing(Margin);
            OffsetCalc(Render.TopMargin, BaseOffset, position);
            
            // element offset
            if(Offset != null)
                BaseOffset.Plus(Offset);
            Position offset = new Position(BaseOffset);

            // padding offset
            Render.TopPadding = GetTopLeftSpacing(Padding);
            OffsetCalc(Render.TopPadding, BaseOffset, position);

            // break-size
            if(Type == VisualType.Break){
                SetRenderState(position, BaseOffset, new Size(1, new Text(" ", null).Size.Height), null);
                return;
            }

            // prefered size definition
            float width = GetWidth(this);
            float height = GetHeight(this);

            // text-size
            if (Type == VisualType.Text)
            {
                SetRenderState(position, BaseOffset, (this as VisualText).TextHandle.Size, null);
                return;
            }

            // default image size
            if (Type == VisualType.Image)
            {
                Size imageSize = (this as VisualImage).ImageHandle.Size;
                if(width <= 0f && height <= 0f)
                {
                    width = imageSize.Width;
                    height = imageSize.Height;
                }
                else if (width <= 0f)
                {
                    float factor = (imageSize.Width / imageSize.Height);
                    width = height * factor;
                }
                else if(height <= 0f)
                {
                    float factor = (imageSize.Height / imageSize.Width);
                    height = width * factor;
                }
                SetRenderState(position, BaseOffset, new Size(width, height), null);
                return;
            }

            // total width from parent
            if (width <= 0f && (Type == VisualType.Block || Type == VisualType.Scroll))
            {
                width = ParentWidth(this);
            }

            // total height from parent
            if (height <= 0f && (Type == VisualType.Scroll))
            {
                height = ParentHeight(this);
            }

            // scroll sub-calculation
            if(Type == VisualType.Scroll)
            {
                if (Nodes == null || Nodes.Size == 0) return;
                // group to root-element if element list (at-this-time hack)
                if(Nodes.Size > 1){
                    VisualElement[] nodes = Nodes.ToArray();
                    Nodes.Clear();
                    VisualElement top = new VisualElement();
                    add(top);
                    for(int i=0; i<nodes.Length; i++){
                        top.add(nodes[i]);
                    }
                }
                VisualElement child = Nodes[0];
                VisualScroll scroll = this as VisualScroll;
                Position off = child.Offset;
                Size clip = child.Clip;
                child.Offset = null;
                child.Clip = null;
                child.Metrics(new Position(position), new Position(BaseOffset));
                scroll.UncutSize = new Size(child.Render.Bound);
                child.Offset = off;
                child.Clip = clip;
                child.Metrics(new Position(position), new Position(BaseOffset));
                SetRenderState(position, BaseOffset, new Size(width, height), null);
                return;
            }

            // calculate childrens
            Size allBound = new Size(0f, 0f);
            float currentLeft = 0f;
            float currentTop = 0f;
            float currentHeight = 0f;
            float lastYoffset = 0f;
            for (int i = 0; Nodes != null && i < Nodes.Size; i++)
            {
                VisualElement child = Nodes[i];
                Position childPosition = new Position(position.X + currentLeft, position.Y + currentTop);   
                child.Metrics(childPosition, offset);
                if(!child.Display) continue;
                Size bound = new Size(child.Render.Bound);
                if (child.Type == VisualType.Block || child.Type == VisualType.Scroll)
                {
                    if(bound.Width > allBound.Width)
                        allBound.Width = bound.Width;
                    allBound.Height += bound.Height;
                    currentTop += bound.Height;
                    currentLeft = 0f;
                    currentHeight = 0f;
                    offset.X = BaseOffset.X;
                }
                else if(child.Type == VisualType.Inline ||  child.Type == VisualType.Image || child.Type == VisualType.Text)
                {
                    currentLeft += bound.Width;
                    if(currentLeft > allBound.Width)
                        allBound.Width = currentLeft;                    
                    if(bound.Height > currentHeight){
                        allBound.Height += (bound.Height - currentHeight);
                        currentHeight = bound.Height;
                    }
                    lastYoffset = offset.Y;
                    offset.Y = BaseOffset.Y;
                }
                else if (child.Type == VisualType.Break)
                {
                    currentTop += currentHeight;
                    currentLeft = 0f;
                    currentHeight = 0f;
                    offset.X = BaseOffset.X;
                    BaseOffset.Y = lastYoffset;
                    offset.Y = BaseOffset.Y;
                }
            }

            // children offset
            BaseOffset.Minus(Position.Minus(BaseOffset, offset));

            // overall size
            SetRenderState(position, BaseOffset, allBound, new Size(width, height));
        }

        public static Position GetTopLeftSpacing(Spacing Spacing)
        {
            Position newPosition = new Position(0f, 0f);
            if (Spacing == null)
                return newPosition;
            if (Spacing.Left != null)
                newPosition.X += Spacing.Left.way;
            if (Spacing.Top != null)
                newPosition.Y += Spacing.Top.way;
            return newPosition;
        }

        public static Position GetRightBottomSpacing(Spacing Spacing)
        {
            Position newPosition = new Position(0f, 0f);
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
            if(Element == null || Element.Render == null || !Element.Render.True)
            {
                return;
            }
            if(Event.IsCursor)
            {
                bool intersect = GeometryUtils.IntersectVisual(Element, Event.Cursor);
                if(!Element.Focus && intersect)
                {
                    Element.Focus = true;
                    this.Event(new InputEvent(InputType.Visual, new VisualState(VisualEventState.Hover, true))); 
                }
                else if(Element.Focus && !intersect)
                {
                    Element.Focus = false;
                    this.Event(new InputEvent(InputType.Visual, new VisualState(VisualEventState.Hover, false)));
                }
                if(intersect)
                    this.Event(Event);
            }
            else if(Event.IsButton)
            {
                bool intersect = GeometryUtils.IntersectVisual(Element, Cursor);
                if(!Element.Active && intersect)
                {
                    Element.Active = true;
                    this.Event(new InputEvent(InputType.Visual, new VisualState(VisualEventState.Focus, true)));               
                }
                else if(Element.Active && !intersect)
                {
                    Element.Active = false;
                    this.Event(new InputEvent(InputType.Visual, new VisualState(VisualEventState.Focus, false)));
                }
                if(intersect)
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
            set{ TextHandle = new Text(value, null); }
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
            this.Source = Filepath;
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
    
    public class VisualScroll : VisualElement
    {
        public static readonly float ScrollThickness = 12;
        public float ScrollYPosition;
        public float ScrollXPosition;
        public Size ScrollYSize = new Size(ScrollThickness, 0);
        public Size ScrollXSize = new Size(0, ScrollThickness);
        public Size UncutSize = new Size(0, 0);

        public VisualScroll() : base(VisualType.Scroll)
        {
            this.Listener = new ScrollListener(this);
        }

        public void ValidateScroll(Size OriginalSize)
        {
            float factorY = (Render.Size.Height / OriginalSize.Height);
            ScrollYSize.Height = (Render.Size.Height * factorY);
            if (ScrollYPosition + ScrollYSize.Height > Render.Size.Height)
                ScrollYPosition = Render.Size.Height - ScrollYSize.Height;

            float factorX = (Render.Size.Width / OriginalSize.Width);
            ScrollXSize.Width = (Render.Size.Width * factorX);
            if (ScrollXPosition + ScrollXSize.Width > Render.Size.Width)
                ScrollXPosition = Render.Size.Width - ScrollXSize.Width;
        }

        public void DrawScrollY()
        {
            float w = ScrollYSize.Width;
            float h = ScrollYSize.Height;
            float x = Render.Position.X + Render.Size.Width - w;
            float y = Render.Position.Y + ScrollYPosition;

            GL.Color3(75 / 255f, 75 / 255f, 75 / 255f);
            GL.Begin(PrimitiveType.Quads);
            GL.Vertex2(x, y);
            GL.Vertex2(x + w, y);
            GL.Vertex2(x + w, y + h);
            GL.Vertex2(x, y + h);
            GL.End();
        }

        public void DrawScrollX()
        {
            float w = ScrollXSize.Width;
            float h = ScrollXSize.Height;
            float x = Render.Position.X + ScrollXPosition;
            float y = Render.Position.Y + Render.Size.Height - h;

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
            public VisualScroll Element;
            public bool ActiveScrollY;
            public bool ActiveScrollX;
            public float ScrollYOffset;
            public float ScrollXOffset;

            public ScrollListener(VisualScroll Element)
            { 
                this.Element = Element;
            }

            public override void Input(InputEvent Event)
            {
                if (Event.IsButton && Event.Button.IsClick && Event.Button.Type == Button.Left)
                {
                    if(Element.Render.Position != null)
                    {
                        if(GeometryUtils.IntersectBound(
                                (int)(Element.Render.Position.X + Element.Render.Size.Width - Element.ScrollYSize.Width), (int)Element.ScrollYSize.Width,
                                (int)(Element.Render.Position.Y + Element.ScrollYPosition), (int)Element.ScrollYSize.Height,
                                Mouse.Cursor.x, Mouse.Cursor.y))
                        {
                            ActiveScrollY = true;
                            ScrollYOffset = (Mouse.Cursor.y - Element.ScrollYPosition);
                        }
                        if(GeometryUtils.IntersectBound(
                               (int)(Element.Render.Position.X + Element.ScrollXPosition), (int)Element.ScrollXSize.Width,
                               (int)(Element.Render.Position.Y + Element.Render.Size.Height - Element.ScrollXSize.Height), (int)Element.ScrollXSize.Height,
                               Mouse.Cursor.x, Mouse.Cursor.y))
                        {
                            ActiveScrollX = true;
                            ScrollXOffset = (Mouse.Cursor.x - Element.ScrollXPosition);
                        }
                    }
                }
                if(Event.IsButton && Event.Button.IsUp)
                {
                    ActiveScrollY = false;
                    ActiveScrollX = false;
                }
                if(ActiveScrollY && Event.IsCursor)
                {
                    float offset = (Mouse.Cursor.y - ScrollYOffset);
                    if (offset < 0)
                        offset = 0;
                    if (offset > (Element.Render.Size.Height - Element.ScrollYSize.Height))
                        offset = (Element.Render.Size.Height - Element.ScrollYSize.Height);
                    Element.ScrollYPosition = offset;
                }
                if(ActiveScrollX && Event.IsCursor)
                {
                    float offset = (Mouse.Cursor.x - ScrollXOffset);
                    if(offset < 0)
                        offset = 0;
                    if (offset > (Element.Render.Size.Width - Element.ScrollXSize.Width))
                        offset = (Element.Render.Size.Width - Element.ScrollXSize.Width);
                    Element.ScrollXPosition = offset;
                }
            }
        }
    }
}
