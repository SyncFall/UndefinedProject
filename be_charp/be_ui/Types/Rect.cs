using Be.Runtime.Types;
using OpenTK.Graphics.OpenGL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.UI.Types
{ 
    public class RectRoot : RectNode
    {
        public ScreenType ScreenType;

        public RectRoot(ScreenType ScreenType) : base(null)
        {
            this.ScreenType = ScreenType;
        }
    }

    public class RectNodeCollection : ListCollection<RectNode>
    {
        public void MouseEvent(MouseState Result)
        {
            for (int i = 0; i<this.Size(); i++)
            {
                this.Get(i).MouseEvent(Result);
            }
        }

        public void Draw()
        {
            for (int i = 0; i < this.Size(); i++)
            {
                this.Get(i).Draw();
            }
        }
    }

    public class RectNode
    {
        // node-tree
        public RectNode Parent;
        public RectNodeCollection _Childrens;
        // absolute-position
        private int _Left;
        private int _Top;
        private int _Right;
        private int _Bottom;
        // relative-position
        private int _MarginLeft;
        private int _MarginTop;
        private int _MarginRight;
        private int _MarginBottom;
        // padding
        private int _PaddingLeft;
        private int _PaddingTop;
        private int _PaddingRight;
        private int _PaddingBottom;
        // dimension
        private int _Width;
        private int _Height;
        // interactive-transforms
        private bool _SizeTransform;
        private RectTransform _FreeSizeTransform;
        
        public RectNode(RectNode Parent)
        {
            this.Parent = Parent;
        }

        #region BASIC_PROPERTIES

        public RectRoot Root
        {
            get
            {
                RectNode rectNode = this;
                while((rectNode = Parent) != null)
                {
                    ;
                }
                return rectNode as RectRoot;
            }
            set
            {
                throw new Exception("not implemented yet");
            }
        }

        public RectNodeCollection Childrens
        {
            get
            {
                return _Childrens;
            }
            set
            {
                throw new Exception("not implemented yet");
            }
        }

        public int Top
        {
            get
            {
                return _Top;
            }
            set
            {
                _Top = value;
                if(_Bottom == -1)
                {
                    _Bottom = _Top;
                }
            }
        }

        public int Left
        {
            get
            {
                return _Left;
            }
            set
            {
                _Left = value;
                if(_Right == -1)
                {
                    _Right = _Left;
                }
            }
        }

        public PositionType Position
        {
            get
            {
                return new PositionType(_Top, _Left);
            }
            set
            {
                Top = value.Top;
                Left = value.Left;
            }
        }

        public PositionType PositionAbsolute
        {
            get
            {
                if(Parent != null)
                {
                    PositionType parentPostion = Parent.PositionAbsolute;
                    return new PositionType(parentPostion.Left, parentPostion.Top);
                }
                else
                {
                    return new PositionType(0, 0);
                }
            }
            set
            {
                Position = value;
            }
        }

        public PositionType PositionRelative
        {
            get
            {
                if (Parent != null)
                {
                    PositionType parentPostion = Parent.PositionRelative;
                    return new PositionType(parentPostion.Left, PositionRelative.Top);
                }
                else
                {
                    return new PositionType(0, 0);
                }
            }
            set
            {
                Position = value;
            }
        }

        public int Bottom
        {
            get
            {
                return _Bottom;
            }
            set
            {
                if(value < _Top)
                {
                    throw new Exception("bottom can not less top");
                }
                _Bottom = value;
                _Height = (_Bottom - _Height);
            }
        }

        public int Right
        {
            get
            {
                return _Right;
            }
            set
            {
                if(value < _Left)
                {
                    throw new Exception("right can not less left");
                }
                _Right = value;
                _Width = (_Right - _Left);
            }
        }

        public int Width
        {
            get
            {
                return _Width;
            }
            set
            {
                if(Width < 0)
                {
                    throw new Exception("width can not negative");
                }
                _Width = value;
                _Right = (_Left + _Width);
            }
        }

        public int Height
        {
            get
            {
                return _Height;
            }
            set
            {
                if(Height < 0)
                {
                    throw new Exception("height can not nagative");
                }
                _Height = value;
                _Bottom = (_Top + _Height);
            }
        }

        public SizeType Size
        {
            get
            {
                return new SizeType(_Width, _Height);
            }
            set
            {
                Width = value.Width;
                Height = value.Height;
            }
        }

        public bool SizeTransform
        {
            get
            {
                return _SizeTransform;
            }
            set
            {
                _SizeTransform = value;
            }
        }
        
        public bool FreeSizeTransform
        {
            get
            {
                return (_FreeSizeTransform != null);
            }
            set
            {
                if (value && _FreeSizeTransform == null)
                {
                    _FreeSizeTransform = new RectTransform(this);
                }
                else
                {
                    _FreeSizeTransform = null;
                }
            }
        }

        #endregion

        public void MouseEvent(MouseState Result)
        {
            /*
            if (_SizeTransform)
            {
                Right = Result.Cursor.X;
                Bottom = Result.Cursor.Y;
            }

            if (_FreeSizeTransform != null)
            {
                _FreeSizeTransform.MouseEvent(Result);
            }

            if (_Childrens != null)
            {
                _Childrens.MouseEvent(Result);
            }
            */
        }

        public void Draw()
        {
            GL.LineWidth(2);
            GL.Color3(Color.White);

            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex2(Left, Top);
            GL.Vertex2(Right, Top);
            GL.Vertex2(Right, Bottom);
            GL.Vertex2(Left, Bottom);
            GL.End();

            if (_FreeSizeTransform != null)
            {
                _FreeSizeTransform.Draw();
            }

            if (_Childrens != null)
            {
                _Childrens.Draw();
            }
        }
    }


    public enum RectPointerEnum
    {
        TOP,
        LEFT,
        RIGHT,
        BOTTOM,
        TOP_LEFT,
        TOP_RIGHT,
        BUTTON_LEFT,
        BUTTON_RIGHT,
    }

    public class RectPointer
    {
        public static readonly int MARGIN = 5;
        public static readonly int MOUSE_MARGIN_ADD = 5;
        public RectPointerEnum Type;
        public RectNode RectType;
        public int X;
        public int Y;
        public bool IsMouseOver;
        public bool IsActive;

        public RectPointer(RectNode RectType, RectPointerEnum Type)
        {
            this.Type = Type;
            this.RectType = RectType;
        }

        public void MouseEvent(MouseState Result)
        {
            /*
            if (Result.InputType == MouseType.CURSOR_EVENT)
            {
                IsMouseOver = IntersectMouse(Result.Cursor.X, Result.Cursor.Y);
            }
            if (Result.InputType == MouseType.BUTTON_EVENT && Result.Button.Key == ButtonKey.LEFT && Result.Button.Event == ButtonEvent.DOWN)
            {
                IsActive = IsMouseOver;
            }
            else if (Result.InputType == MouseType.BUTTON_EVENT && Result.Button.Key == ButtonKey.LEFT && Result.Button.Event == ButtonEvent.UP)
            {
                IsActive = false;
            }
            if (Result.InputType == MouseType.CURSOR_EVENT && IsActive)
            {
                UpdateRectDimension(Result.Cursor.X, Result.Cursor.Y);
            }
            */
        }

        public bool IntersectMouse(int MouseX, int MouseY)
        {
            return GeometryUtils.IntersectPositionWithMargin(X, Y, MouseX, MouseY, MARGIN + MOUSE_MARGIN_ADD, MARGIN + MOUSE_MARGIN_ADD);
        }

        public void UpdateRectDimension(int MouseX, int MouseY)
        {
            if (Type == RectPointerEnum.TOP)
            {
                RectType.Top = MouseY;
            }
            else if (Type == RectPointerEnum.BOTTOM)
            {
                RectType.Bottom = MouseY;
            }
            else if (Type == RectPointerEnum.LEFT)
            {
                RectType.Left = MouseX;
            }
            else if (Type == RectPointerEnum.RIGHT)
            {
                RectType.Right = MouseX;
            }
            else if (Type == RectPointerEnum.TOP_LEFT)
            {
                RectType.Top = MouseY;
                RectType.Left = MouseX;
            }
            else if (Type == RectPointerEnum.TOP_RIGHT)
            {
                RectType.Top = MouseY;
                RectType.Right = MouseX;
            }
            else if (Type == RectPointerEnum.BUTTON_LEFT)
            {
                RectType.Bottom = MouseY;
                RectType.Left = MouseX;
            }
            else if (Type == RectPointerEnum.BUTTON_RIGHT)
            {
                RectType.Bottom = MouseY;
                RectType.Right = MouseX;
            }
        }

        public void UpdatePosition()
        {
            if (Type == RectPointerEnum.TOP)
            {
                this.X = (RectType.Left + RectType.Right) / 2;
                this.Y = RectType.Top;
            }
            else if (Type == RectPointerEnum.BOTTOM)
            {
                this.X = (RectType.Left + RectType.Right) / 2;
                this.Y = RectType.Bottom;
            }
            else if (Type == RectPointerEnum.LEFT)
            {
                this.X = RectType.Left;
                this.Y = (RectType.Top + RectType.Bottom) / 2;
            }
            else if (Type == RectPointerEnum.RIGHT)
            {
                this.X = RectType.Right;
                this.Y = (RectType.Top + RectType.Bottom) / 2;
            }
            else if (Type == RectPointerEnum.TOP_LEFT)
            {
                this.X = RectType.Left;
                this.Y = RectType.Top;
            }
            else if (Type == RectPointerEnum.TOP_RIGHT)
            {
                this.X = RectType.Right;
                this.Y = RectType.Top;
            }
            else if (Type == RectPointerEnum.BUTTON_LEFT)
            {
                this.X = RectType.Left;
                this.Y = RectType.Bottom;
            }
            else if (Type == RectPointerEnum.BUTTON_RIGHT)
            {
                this.X = RectType.Right;
                this.Y = RectType.Bottom;
            }
        }

        public void Draw()
        {
            GL.LineWidth(1.5f);
            if (IsActive)
            {
                GL.Color3(Color.Black);
            }
            else if (IsMouseOver)
            {
                GL.Color3(Color.Black);
            }
            else
            {
                GL.Color3(Color.LightGray);
            }
            GL.Begin(PrimitiveType.LineLoop);
            GL.Vertex2(X - MARGIN, Y - MARGIN);
            GL.Vertex2(X + MARGIN, Y - MARGIN);
            GL.Vertex2(X + MARGIN, Y + MARGIN);
            GL.Vertex2(X - MARGIN, Y + MARGIN);
            GL.End();
        }
    }

    public class RectTransform
    {
        public RectNode RectType;
        public ListCollection<RectPointer> RectPointers = new ListCollection<RectPointer>();

        public RectTransform(RectNode RectType)
        {
            this.RectType = RectType;
            this.RectPointers.Add(new RectPointer(RectType, RectPointerEnum.TOP));
            this.RectPointers.Add(new RectPointer(RectType, RectPointerEnum.LEFT));
            this.RectPointers.Add(new RectPointer(RectType, RectPointerEnum.RIGHT));
            this.RectPointers.Add(new RectPointer(RectType, RectPointerEnum.BOTTOM));
            this.RectPointers.Add(new RectPointer(RectType, RectPointerEnum.TOP_LEFT));
            this.RectPointers.Add(new RectPointer(RectType, RectPointerEnum.TOP_RIGHT));
            this.RectPointers.Add(new RectPointer(RectType, RectPointerEnum.BUTTON_LEFT));
            this.RectPointers.Add(new RectPointer(RectType, RectPointerEnum.BUTTON_RIGHT));
        }

        public void MouseEvent(MouseState Result)
        {
            for (int i = 0; i < this.RectPointers.Size(); i++)
            {
                this.RectPointers.Get(i).UpdatePosition();
                this.RectPointers.Get(i).MouseEvent(Result);
            }
        }

        public void Draw()
        {
            for (int i = 0; i < this.RectPointers.Size(); i++)
            {
                this.RectPointers.Get(i).UpdatePosition();
                this.RectPointers.Get(i).Draw();
            }
        }
    }
}
