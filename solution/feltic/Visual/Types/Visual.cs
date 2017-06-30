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
        public bool Render;
        public Position RenderPosition;
        public Size RenderSize;
        public Size RenderBound;
        public Position RenderOffset;
        public Size RenderClip;
        // input-state
        public InputListener Listener;
        public bool Active;
        public bool Focus;

        public VisualElement()
        {
            this.Type = VisualType.Inline;
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
            if(!Display || !Render || Type == VisualType.Break) return;

            if(Render)
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
                VisualScroll scroll = this as VisualScroll;
                VisualElement child = Nodes[0];
                Size scrollSize = new Size(scroll.RenderSize);
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
            Render = false;
            Position = null;
            Size = null;
            RenderPosition = null;
            RenderSize = null;
            RenderBound = null;
            RenderOffset = null;
        }

        public void SetRenderState(Position BasePosition, Position BaseOffset, Position MarginStart, Position PaddingStart, Size BaseSize, Size PreferSize)
        {
            // clear state
            ClearRenderState();
            if(BaseSize == null) return;

            // size
            RenderSize = new Size(BaseSize);
            if(PreferSize != null){
                // prefer size
                Size preferSize = new Size(PreferSize);
                preferSize.Minus(GetTopLeftSpacing(Padding));
                preferSize.Minus(GetRightBottomSpacing(Padding));
                if(Type == VisualType.Scroll){
                    RenderSize.Width = preferSize.Width;
                    RenderSize.Height = preferSize.Height;
                }else{
                    if(preferSize.Width > BaseSize.Width)
                        RenderSize.Width = preferSize.Width;
                    if(preferSize.Height > BaseSize.Height)
                        RenderSize.Height = preferSize.Height;
                }
            }

            // position
            Position = new Position(MarginStart);
            RenderPosition = new Position(PaddingStart);

            // size offset
            Position diff = GetMinusDifference(new Position(RenderSize.Width, RenderSize.Height), BaseOffset);
            if(PreferSize != null && PreferSize.Width > BaseSize.Width)
                RenderSize.Minus(new Size(diff.X, 0));
            if(PreferSize != null && PreferSize.Height > BaseSize.Height)
                RenderSize.Minus(new Size(0, diff.Y));
            if(PreferSize == null)
                RenderSize.Minus(diff);
            RenderOffset = new Position(diff);
            BaseOffset.Minus(diff);

            /*
            // padding offset
            diff = GetMinusDifference(GetRightBottomSpacing(Padding), BaseOffset);
            Position paddingEnd = Position.Plus(Position.Plus(RenderPosition, new Position(RenderSize.Width, RenderSize.Height)), diff);
            BaseOffset.Minus(diff);

            // margin offset
            diff = GetMinusDifference(GetRightBottomSpacing(Margin), BaseOffset);
            Position marginEnd = Position.Plus(paddingEnd, diff);
            BaseOffset.Minus(diff);
            */

            // bound
            RenderBound = new Size(RenderSize);
            //RenderBound.Plus(Position.Minus(PaddingStart, MarginStart));
            //RenderBound.Plus(Position.Minus(marginEnd, paddingEnd));

            // check
            if(!CheckForRender()) return;

            // update children state
            if (Clip != null)
                SetRenderStateSecond(new Position(Clip));

            // check
            CheckForRender();
        }

        public bool CheckForRender(bool Any=true)
        {
            if(Any && RenderBound != null && (RenderBound.Width > 0f || RenderBound.Height > 0f)){
                return (Render = true);
            }else if (!Any && RenderBound != null && (RenderBound.Width > 0f && RenderBound.Height > 0f)){ 
                return (Render = true);
            }else{
                ClearRenderState();
                return false;
            } 
        }

        public void SetRenderStateSecond(Position BaseClip)
        {
            /*
            // margin offset
            Position diff = GetMinusDifference(Position.Minus(MarginEnd, PaddingEnd), BaseClip);
            RenderBound.Minus(diff);
            BaseClip.Minus(diff);

            // padding offset
            diff = GetMinusDifference(Position.Minus(PaddingEnd, Position.Plus(RenderPosition, new Position(RenderSize.Width, RenderSize.Height))), BaseClip);
            RenderBound.Minus(diff);
            BaseClip.Minus(diff);
            */

            // size offset
            Position diff = GetMinusDifference(new Position(RenderSize.Width, RenderSize.Height), BaseClip);
            RenderSize.Minus(diff);
            RenderClip = new Size(diff);
            RenderBound.Minus(diff);
            BaseClip.Minus(diff);

            /*
            // margin offset
            diff = GetMinusDifference(Position.Minus(PaddingStart, MarginStart), BaseClip);
            RenderBound.Minus(diff);
            BaseClip.Minus(diff);

            // padding offset
            diff = GetMinusDifference(Position.Minus(MarginStart, BasePosition), BaseClip);
            RenderBound.Minus(diff);
            BaseClip.Minus(diff);
            */

            if(!CheckForRender(false)) return;

            if(Nodes == null) return;
            Size maxSize = new Size(RenderBound);
            float currentWidth = 0f;
            float currentHeight = 0f;
            for(int i = 0; i < Nodes.Size; i++)
            {
                VisualElement child = Nodes[i];
                if(!child.Display) continue;
                Size bound = new Size(child.RenderBound);
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
                if(child.Render)
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

        public void Metrics(Position BasePosition=null, Position BaseOffset=null)
        {
            // not to display
            if(!Display){
                SetRenderState(BasePosition, null, null, null, null, null);
                return;
            }

            // initial offset
            if(BaseOffset == null)
                BaseOffset = new Position(0f, 0f);

            // margin offset
            Position diff = GetMinusDifference(GetTopLeftSpacing(Margin), BaseOffset);
            Position marginStart = Position.Plus(BasePosition, diff);
            BaseOffset.Minus(diff);

            // element offset
            if (Offset != null)
                BaseOffset.Plus(Offset);
            Position offset = new Position(BaseOffset);

            // padding offset
            diff = GetMinusDifference(GetTopLeftSpacing(Padding), BaseOffset);
            Position paddingStart = Position.Plus(marginStart, diff);
            BaseOffset.Minus(diff);

            // text-size
            if (Type == VisualType.Text){
                SetRenderState(BasePosition, BaseOffset, marginStart, paddingStart, (this as VisualText).TextHandle.Size, null);
                return;
            }

            // break-size
            if(Type == VisualType.Break){
                SetRenderState(BasePosition, BaseOffset, marginStart, paddingStart, new Size(1, new Text(" ", null).Size.Height), null);
                return;
            }

            // prefered size definition
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
                SetRenderState(BasePosition, BaseOffset, marginStart, paddingStart, new Size(width, height), null);
                return;
            }

            // default input size
            if (Type == VisualType.Input)
            {
                Size textSize = new Text(new string(' ', 15), null).Size;
                if(width <= 0f) width = textSize.Width;
                if(height <= 0f) height = textSize.Height;
                SetRenderState(BasePosition, BaseOffset, marginStart, paddingStart, new Size(width, height), null);
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
                child.Metrics(new Position(BasePosition), new Position(BaseOffset));
                scroll.UncutSize = new Size(child.RenderSize);
                child.Offset = off;
                child.Clip = clip;
                child.Metrics(BasePosition, BaseOffset);
                SetRenderState(BasePosition, BaseOffset, marginStart, paddingStart, new Size(width, height), null);
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
                Position childPosition = new Position(paddingStart.X + currentLeft, paddingStart.Y + currentTop);   
                child.Metrics(childPosition, offset);
                if(!child.Display) continue;
                Size bound = new Size(child.RenderBound);
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
            SetRenderState(BasePosition, BaseOffset, marginStart, paddingStart, allBound, new Size(width, height));
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
            if (Element == null || Element.RenderSize == null || Element.RenderPosition == null)
            {
                return;
            }
            if(Event.IsCursor)
            {
                bool intersect = GeometryUtils.IntersectVisual(Element, Event.Cursor);
                if(!Element.Focus && intersect)
                {
                    Element.Focus = true;
                    this.Event(new InputEvent(InputType.Visual, new VisualState(VisualEventState.Focus, true))); 
                }
                else if(Element.Focus && !intersect)
                {
                    Element.Focus = false;
                    this.Event(new InputEvent(InputType.Visual, new VisualState(VisualEventState.Focus, false)));
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
                    this.Event(new InputEvent(InputType.Visual, new VisualState(VisualEventState.Active, true)));               
                }
                else if(Element.Active && !intersect)
                {
                    Element.Active = false;
                    this.Event(new InputEvent(InputType.Visual, new VisualState(VisualEventState.Active, false)));
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
            float factorY = (RenderSize.Height / OriginalSize.Height);
            ScrollYSize.Height = (RenderSize.Height * factorY);
            if (ScrollYPosition + ScrollYSize.Height > RenderSize.Height)
                ScrollYPosition = RenderSize.Height - ScrollYSize.Height;

            float factorX = (RenderSize.Width / OriginalSize.Width);
            ScrollXSize.Width = (RenderSize.Width * factorX);
            if (ScrollXPosition + ScrollXSize.Width > RenderSize.Width)
                ScrollXPosition = RenderSize.Width - ScrollXSize.Width;
        }

        public void DrawScrollY()
        {
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

        public void DrawScrollX()
        {
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
                    if(GeometryUtils.IntersectBound(
                            (int)(Element.RenderPosition.X + Element.RenderSize.Width - Element.ScrollYSize.Width), (int)Element.ScrollYSize.Width,
                            (int)(Element.RenderPosition.Y + Element.ScrollYPosition), (int)Element.ScrollYSize.Height,
                            Mouse.Cursor.x, Mouse.Cursor.y))
                    {
                        ActiveScrollY = true;
                        ScrollYOffset = (Mouse.Cursor.y - Element.ScrollYPosition);
                    }
                    if(GeometryUtils.IntersectBound(
                           (int)(Element.RenderPosition.X + Element.ScrollXPosition), (int)Element.ScrollXSize.Width,
                           (int)(Element.RenderPosition.Y + Element.RenderSize.Height - Element.ScrollXSize.Height), (int)Element.ScrollXSize.Height,
                           Mouse.Cursor.x, Mouse.Cursor.y))
                    {
                        ActiveScrollX = true;
                        ScrollXOffset = (Mouse.Cursor.x - Element.ScrollXPosition);
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
                    if (offset > (Element.RenderSize.Height - Element.ScrollYSize.Height))
                        offset = (Element.RenderSize.Height - Element.ScrollYSize.Height);
                    Element.ScrollYPosition = offset;
                }
                if(ActiveScrollX && Event.IsCursor)
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
