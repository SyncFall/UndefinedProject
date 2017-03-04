using Feltic.Lib;
using Feltic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Feltic.UI
{
    public enum InputType
    {
        Key,
        Cursor,
        Button,
        Scroll,
    }

    public abstract class InputState
    { }

    public class InputEvent
    {
        public InputType Type;
        public InputState State;

        public InputEvent(InputType Type, InputState State)
        {
            this.Type = Type;
            this.State = State;
        }

        public bool IsKey
        {
            get
            {
                return (this.Type == InputType.Key);
            }
        }

        public bool IsCursor
        {
            get
            {
                return (this.Type == InputType.Cursor);
            }
        }

        public bool IsButton
        {
            get
            {
                return (this.Type == InputType.Button);
            }

        }

        public KeyState Key
        {
            get
            {
                return (State as KeyState);
            }
        }

        public CursorState Cursor
        {
            get
            {
                return (State as CursorState);
            }
        }

        public ButtonState Button
        {
            get
            {
                return (State as ButtonState);
            }
        }
    }

    public class Input
    {
        public static ListCollection<InputListener> InputListeners = new ListCollection<InputListener>();
        public static MouseState Mouse;
        public static KeyboardState Keyboard;

        public static void Inititialize(OpenTK.GameWindow GameWindow)
        {
            Mouse = new MouseState(GameWindow);
            Keyboard = new KeyboardState(GameWindow);
        }

        public static void FireListeners(InputEvent InputEvent)
        {
            for (int i = 0; i < InputListeners.Size; i++)
            {
                InputListener inputListener = InputListeners.Get(i);
                inputListener.ProcessInputEvent(InputEvent);
            }
        }

        public static void Add(InputListener InputListener)
        {
            InputListener.Mouse = Mouse;
            InputListener.Keyboard = Keyboard;
            InputListeners.Add(InputListener);
        }

        public static void Remove(InputListener InputListener)
        {
            for(int i=0; i<InputListeners.Size; i++)
            {
                if(InputListeners.Get(i).Id == InputListener.Id)
                {
                    InputListeners.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public abstract class InputListener
    {
        private static int IdCounter = 0;
        public readonly int Id;
        public object Sender;
        public MouseState Mouse;
        public KeyboardState Keyboard;

        public InputListener()
        {
            this.Id = (++IdCounter);
            UI.Input.Add(this);
        }

        public void ProcessInputEvent(InputEvent Event)
        {   
            if (Sender != null /*&& Sender is Compose*/)
            {
                /*
                Compose compose = (Sender as Compose);
                if (Event.IsButton && Event.Button.Type == Button.Left && Event.Button.IsClick)
                {
                    CursorState cursor = Mouse.Cursor;
                    Size size = compose.Size;
                    if(cursor.x >= 0 && cursor.x <= size.Width && 
                       cursor.y >= 0 && cursor.x <= size.Height
                    ){
                        this.Input(Event);
                    }
                }
                */
            }
            else
            {
                this.Input(Event);
            }    
        }

        public void Dispose()
        {
            UI.Input.Remove(this);
        }

        public abstract void Input(InputEvent Event);
    }

    public class KeyboardState
    {
        public KeyStateCollection Keys = new KeyStateCollection();

        public KeyboardState(OpenTK.GameWindow GameWindow)
        {
            GameWindow.Keyboard.KeyRepeat = false;
            OpenTK.Input.KeyboardState keyboardState = OpenTK.Input.Keyboard.GetState();
            Key[] keys = typeof(Key).GetEnumValues().Cast<Key>().ToArray();
            for(int i=0; i<keys.Length; i++)
            {
                KeyState keyState = new KeyState(keys[i]);
                keyState.IsClick = false;
                keyState.IsDown = keyboardState.IsKeyDown((OpenTK.Input.Key)keys[i]);
                keyState.IsUp = !keyState.IsDown;
                Keys.Put(keys[i], keyState);
            }
            GameWindow.KeyDown += (object sender, OpenTK.Input.KeyboardKeyEventArgs e) =>
            {
                KeyState keyState = Keys.GetValue((Key)e.Key);
                keyState.IsClick = (!keyState.IsDown ? true : false);
                keyState.IsDown = true;
                if (keyState.IsDownStart == 0)
                {
                    keyState.IsDownStart = TimeUtils.CurrentMilliseconds;
                    keyState.IsDownMilliseconds = 0;
                }
                else
                {
                    keyState.IsDownMilliseconds = (TimeUtils.CurrentMilliseconds - keyState.IsDownStart);
                }
                keyState.IsUp = false;
                Input.FireListeners(new InputEvent(InputType.Key, keyState));
            };
            GameWindow.KeyUp += (object sender, OpenTK.Input.KeyboardKeyEventArgs e) =>
            {
                KeyState keyState = Keys.GetValue((Key)e.Key);
                keyState.IsUp = true;
                keyState.IsDown = false;
                keyState.IsDownStart = 0;
                keyState.IsDownMilliseconds = 0;
                keyState.IsClick = false;
                Input.FireListeners(new InputEvent(InputType.Key, keyState));
            };
        }
    }

    public class KeyStateCollection : MapCollection<Key, KeyState>
    {
        public KeyState this[Key key]
        {
            get
            {
                return base.GetValue(key);
            }
        }
    }

    public class KeyState : InputState
    {
        private static readonly string Alphabet = "abcdefghijklmnopqrstuvwxyz";
        private static readonly string Numbers = "0123456789";

        public Key Type;
        public bool IsClick;
        public bool IsDown;
        public long IsDownStart;
        public long IsDownMilliseconds;
        public bool IsUp;

        public KeyState(Key key)
        {
            this.Type = key;
        }

        public bool IsAlphabetChar()
        {
            return (Type >= Key.A && Type <= Key.Z);
        }

        public char GetAlphabetChar()
        {
            return Alphabet[(int)Type - 83];
        }

        public bool IsNumberChar()
        {
            return (Type >= Key.Number0 && Type <= Key.Number9);
        }

        public char GetNumberChar()
        {
            return Numbers[(int)Type - 109];
        }
    }

    public class MouseState
    {
        private CursorState CursorState = new CursorState();
        private ButtonStateCollection ButtonStateCollection = new ButtonStateCollection();

        public MouseState(OpenTK.GameWindow GameWindow)
        {
            OpenTK.Input.MouseState mouseState = OpenTK.Input.Mouse.GetCursorState();
            Cursor.x = mouseState.X;
            Cursor.y = mouseState.Y;
            Button[] buttons = typeof(Button).GetEnumValues().Cast<Button>().ToArray();
            for(int i=0; i<buttons.Length; i++)
            {
                ButtonState button = new ButtonState(buttons[i]);
                button.IsDown = mouseState.IsButtonDown((OpenTK.Input.MouseButton)buttons[i]);
                button.IsUp = !button.IsDown;
                ButtonStateCollection.Put(button.Type, button);
            }
            GameWindow.MouseMove += (object sender, OpenTK.Input.MouseMoveEventArgs e) =>
            {
                Cursor.x = e.X;
                Cursor.y = e.Y;
                Input.FireListeners(new InputEvent(InputType.Cursor, Cursor));
            };
            GameWindow.MouseDown += (object sender, OpenTK.Input.MouseButtonEventArgs e) =>
            {
                ButtonState mouseButton = ButtonStateCollection.GetValue((Button)e.Button);
                mouseButton.IsClick = (!mouseButton.IsDown ? true : false);
                mouseButton.IsDown = true;
                if(mouseButton.IsDownStart == 0)
                {
                    mouseButton.IsDownStart = TimeUtils.CurrentMilliseconds;
                    mouseButton.IsDownMilliseconds = 0;
                }
                else
                {
                    mouseButton.IsDownMilliseconds = (TimeUtils.CurrentMilliseconds - mouseButton.IsDownStart);
                }
                mouseButton.IsUp = false;
                Input.FireListeners(new InputEvent(InputType.Button, mouseButton));
            };
            GameWindow.MouseUp += (object sender, OpenTK.Input.MouseButtonEventArgs e) =>
            {
                ButtonState mouseButton = ButtonStateCollection.GetValue((Button)e.Button);
                mouseButton.IsClick = false;
                mouseButton.IsDown = false;
                mouseButton.IsDownStart = 0;
                mouseButton.IsDownMilliseconds = 0;
                mouseButton.IsUp = true;
                Input.FireListeners(new InputEvent(InputType.Button, mouseButton));
            };
        }

        public CursorState Cursor
        {
            get
            {
                return CursorState;
            }
        }

        public ButtonStateCollection Buttons
        {
            get
            {
                return ButtonStateCollection;
            }
        }
    }

    public class ButtonStateCollection : MapCollection<Button, ButtonState>
    {
        public ButtonState this[Button button]
        {
            get
            {
                return base.GetValue(button);
            }
        }
    }

    public class CursorState : InputState
    {
        public int x;
        public int y;
    }

    public class ButtonState : InputState
    {
        public Button Type;
        public bool IsClick;
        public bool IsDoubleClick;
        public bool IsDown;
        public long IsDownStart;
        public long IsDownMilliseconds;
        public bool IsUp;

        public ButtonState(Button Button)
        {
            this.Type = Button;
        }
    }

    public enum Key
    {
        Unknown = 0,
        ShiftLeft = 1,
        LShift = 1,
        ShiftRight = 2,
        RShift = 2,
        ControlLeft = 3,
        LControl = 3,
        ControlRight = 4,
        RControl = 4,
        AltLeft = 5,
        LAlt = 5,
        AltRight = 6,
        RAlt = 6,
        WinLeft = 7,
        LWin = 7,
        WinRight = 8,
        RWin = 8,
        Menu = 9,
        F1 = 10,
        F2 = 11,
        F3 = 12,
        F4 = 13,
        F5 = 14,
        F6 = 15,
        F7 = 16,
        F8 = 17,
        F9 = 18,
        F10 = 19,
        F11 = 20,
        F12 = 21,
        F13 = 22,
        F14 = 23,
        F15 = 24,
        F16 = 25,
        F17 = 26,
        F18 = 27,
        F19 = 28,
        F20 = 29,
        F21 = 30,
        F22 = 31,
        F23 = 32,
        F24 = 33,
        F25 = 34,
        F26 = 35,
        F27 = 36,
        F28 = 37,
        F29 = 38,
        F30 = 39,
        F31 = 40,
        F32 = 41,
        F33 = 42,
        F34 = 43,
        F35 = 44,
        Up = 45,
        Down = 46,
        Left = 47,
        Right = 48,
        Enter = 49,
        Escape = 50,
        Space = 51,
        Tab = 52,
        BackSpace = 53,
        Back = 53,
        Insert = 54,
        Delete = 55,
        PageUp = 56,
        PageDown = 57,
        Home = 58,
        End = 59,
        CapsLock = 60,
        ScrollLock = 61,
        PrintScreen = 62,
        Pause = 63,
        NumLock = 64,
        Clear = 65,
        Sleep = 66,
        Keypad0 = 67,
        Keypad1 = 68,
        Keypad2 = 69,
        Keypad3 = 70,
        Keypad4 = 71,
        Keypad5 = 72,
        Keypad6 = 73,
        Keypad7 = 74,
        Keypad8 = 75,
        Keypad9 = 76,
        KeypadDivide = 77,
        KeypadMultiply = 78,
        KeypadSubtract = 79,
        KeypadMinus = 79,
        KeypadAdd = 80,
        KeypadPlus = 80,
        KeypadDecimal = 81,
        KeypadPeriod = 81,
        KeypadEnter = 82,
        A = 83,
        B = 84,
        C = 85,
        D = 86,
        E = 87,
        F = 88,
        G = 89,
        H = 90,
        I = 91,
        J = 92,
        K = 93,
        L = 94,
        M = 95,
        N = 96,
        O = 97,
        P = 98,
        Q = 99,
        R = 100,
        S = 101,
        T = 102,
        U = 103,
        V = 104,
        W = 105,
        X = 106,
        Y = 107,
        Z = 108,
        Number0 = 109,
        Number1 = 110,
        Number2 = 111,
        Number3 = 112,
        Number4 = 113,
        Number5 = 114,
        Number6 = 115,
        Number7 = 116,
        Number8 = 117,
        Number9 = 118,
        Tilde = 119,
        Grave = 119,
        Minus = 120,
        Plus = 121,
        BracketLeft = 122,
        LBracket = 122,
        BracketRight = 123,
        RBracket = 123,
        Semicolon = 124,
        Quote = 125,
        Comma = 126,
        Period = 127,
        Slash = 128,
        BackSlash = 129,
        NonUSBackSlash = 130,
        LastKey = 131,
    }

    public enum Button
    {
        Left = 0,
        Middle = 1,
        Right = 2,
        Button1 = 3,
        Button2 = 4,
        Button3 = 5,
        Button4 = 6,
        Button5 = 7,
        Button6 = 8,
        Button7 = 9,
        Button8 = 10,
        Button9 = 11,
        LastButton = 12
    }
}
