using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Be.Runtime.Types;
using OpenTK.Input;
using OpenTK;
using Bee.Library;

namespace Be.UI
{
    public enum MouseType
    {
        CURSOR_EVENT,
        BUTTON_EVENT,
        SCROLL_EVENT,
    }

    public class MouseResult
    {
        public MouseType Type;
        public ButtonResult Button;
        public CursorResult Cursor;

        public MouseResult(MouseType Type)
        {
            this.Type = Type;
        }
    }

    public class CursorResult : MouseResult
    {
        public int X;
        public int Y;

        public CursorResult() : base(MouseType.CURSOR_EVENT)
        { }

        public CursorResult(int X, int Y) : base(MouseType.CURSOR_EVENT)
        {
            this.X = X;
            this.Y = Y;
        }
    }

    public class CursorType : CursorResult
    {
        public GameWindow _Window;

        public CursorType(GameWindow openTkWindow)
        {
            this._Window = openTkWindow;
        }

        public CursorResult UpdateState()
        {
            MouseState state = OpenTK.Input.Mouse.GetCursorState();
            this.X = state.X;
            this.Y = state.Y;
            Console.WriteLine("cursor_update_state | "+ DateTime.Now.TimeOfDay + " | mouse_cursor-x: " + this.X + " | mouse_cursor-y: " + this.Y);
            return this;
        }

        public CursorResult SetPosition(int X, int Y)
        {
            OpenTK.Input.Mouse.SetPosition(X, Y);
            Console.WriteLine("cursor_set_position | " + DateTime.Now.TimeOfDay + " | mouse_cursor-x: " + this.X + " | mouse_cursor-y: " + this.Y);
            return UpdateState();
        }
    }

    public class ButtonResult : MouseResult
    {
        public ButtonKey Key;
        public ButtonEvent Event;

        public ButtonResult(ButtonKey Key, ButtonEvent Event) : base(MouseType.BUTTON_EVENT)
        {
            this.Key = Key;
            this.Event = Event;
        }
    }

    public class ButtonResolve
    {
        public MouseButton _Key;
        public ButtonKey OwnKey;

        public static readonly ButtonResolve[] Map = new ButtonResolve[]
        {
            new ButtonResolve(MouseButton.Left, ButtonKey.LEFT),
            new ButtonResolve(MouseButton.Middle, ButtonKey.MIDDLE),
            new ButtonResolve(MouseButton.Right, ButtonKey.RIGHT),
            new ButtonResolve(MouseButton.LastButton, ButtonKey.UNKNOWN),
        };

        public static ButtonResolve GetButton(ButtonKey ButtonKey)
        {
            for (int i = 0; i < Map.Length; i++)
            {
                if (Map[i].OwnKey == ButtonKey)
                {
                    return Map[i];
                }
            }
            return null;
        }

        public static ButtonResolve GetButton(MouseButton ButtonKey)
        {
            for (int i = 0; i < Map.Length; i++)
            {
                if (Map[i]._Key == ButtonKey)
                {
                    return Map[i];
                }
            }
            return null;
        }

        public ButtonResolve(MouseButton openTk, ButtonKey own)
        {
            this._Key = openTk;
            this.OwnKey = own;
        }
    }

    public class ButtonType : ButtonResult
    {
        public GameWindow _Window;

        public ButtonType(GameWindow openTkWindow, ButtonKey Key) : base(Key, ButtonEvent.UNKNOWN)
        {
            this._Window = openTkWindow;
        }

        public void UpdateState()
        {
            MouseState state = OpenTK.Input.Mouse.GetState();
            ButtonResolve resolve = ButtonResolve.GetButton(this.Key);
            if (state.IsButtonDown(resolve._Key))
            {
                this.Event = ButtonEvent.DOWN;
            }
            else
            {
                this.Event = ButtonEvent.UP;
            }
        }
    }

    public class ButtonCollection : ListCollection<ButtonType>
    {
        public GameWindow _Window;

        public ButtonCollection(GameWindow openTkWindow)
        {
            this._Window = openTkWindow;
            this.Add(new ButtonType(openTkWindow, ButtonKey.LEFT));
            this.Add(new ButtonType(openTkWindow, ButtonKey.MIDDLE));
            this.Add(new ButtonType(openTkWindow, ButtonKey.RIGHT));
            this.Add(new ButtonType(openTkWindow, ButtonKey.UNKNOWN));
        }

        public void UpdateState()
        {
            for(int i=0; i<this.Size(); i++)
            {
                this.Get(i).UpdateState();
            }
        }
    }
    
    public class MouseListenerCollection : ListCollection<MouseListener>
    { }

    public class MouseListener
    {
        public GameWindow _Window;
        public CursorResult Cursor;
        public ButtonResult Button;

        public void Initialize()
        {
            this._Window.MouseDown += (object sender, MouseButtonEventArgs e) =>
            {
                ButtonResolve resolve = ButtonResolve.GetButton(e.Button);
                this.Button = new ButtonResult(resolve.OwnKey, ButtonEvent.DOWN);
                MouseEvent(Button);
            };
            this._Window.MouseUp += (object sender, MouseButtonEventArgs e) =>
            {
                ButtonResolve resolve = ButtonResolve.GetButton(e.Button);
                this.Button = new ButtonResult(resolve.OwnKey, ButtonEvent.UP);
                MouseEvent(Button);
            };
            this._Window.MouseMove += (object sender, MouseMoveEventArgs e) =>
            {
                this.Cursor = new CursorResult(e.X, e.Y);
                MouseEvent(Cursor);
            };
        }

        public virtual void MouseEvent(MouseResult Result)
        {
            if (Result.Type == MouseType.CURSOR_EVENT)
            {
                CursorResult cursorResult = Result as CursorResult;
                Console.WriteLine("cursor_listener | " + DateTime.Now.TimeOfDay + " | cursor-x: " + cursorResult.X + " | cursor-y: " + cursorResult.Y);
            }
            else if(Result.Type == MouseType.BUTTON_EVENT)
            {
                ButtonResult buttonResult = Result as ButtonResult;
                Console.WriteLine("button_listener | " + DateTime.Now.TimeOfDay + " | button-key: " + buttonResult.Key + " | button-event: " + buttonResult.Event);
            }
        }
    }

    public enum ButtonKey
    {
        LEFT,
        MIDDLE,
        RIGHT,
        UNKNOWN,
    }

    public enum ButtonEvent
    {
        DOWN,
        UP,
        UNKNOWN,
    }

    public class Mouse
    {
        public GameWindow _Window;
        public WindowType Window;
        public CursorType Cursor;
        public ButtonCollection Buttons;
        public MouseListenerCollection Listeners = new MouseListenerCollection();

        public Mouse(WindowType Window)
        {
            this.Window = Window;
            this._Window = this.Window._Window;
            this.Cursor = new CursorType(this._Window);
            this.Buttons = new ButtonCollection(this._Window);
        }

        public void UpdateState()
        {
            this.Cursor.UpdateState();
            this.Buttons.UpdateState();
        }

        public void AddListener(MouseListener Listener)
        {
            Listener._Window = this._Window;
            this.Listeners.Add(Listener);
            Listener.Initialize();
        }
    }
}
