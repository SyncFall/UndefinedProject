using Be.Runtime.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.UI.Types
{
    public enum InputType
    {
        Keyboard,
        MouseCursor,
        MouseButton,
        MouseScroll,
    }

    public class Input
    {
        private static ListCollection<InputListener> InputListeners = new ListCollection<InputListener>();

        private static MouseState Mouse;
        private static KeyboardState Keyboard;

        public static void Inititialize(OpenTK.GameWindow GameWindow)
        {
            Mouse = new MouseState(GameWindow);
            Keyboard = new KeyboardState(GameWindow);
        }

        public static void FireListeners(InputEvent InputEvent)
        {
            for (int i = 0; i < InputListeners.Size(); i++)
            {
                InputListener inputListener = InputListeners.Get(i);
                if (!inputListener.Enabled)
                {
                    continue;
                }
                inputListener.InputEvent(InputEvent);
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
            for (int i = 0; i < InputListeners.Size(); i++)
            {
                if (InputListeners.Get(i).Id == InputListener.Id)
                {
                    InputListeners.RemoveAt(i);
                    break;
                }
            }
        }
    }

    public abstract class InputListener
    {
        private static int IdInstance = 0;
   
        public readonly int Id;
        public bool Enabled;

        public MouseState Mouse;
        public KeyboardState Keyboard;

        public InputListener()
        {
            Id = ++IdInstance;
            Enabled = true;
            Input.Add(this);
        }

        public void Remove()
        {
            Enabled = false;
            Input.Add(this);
        }

        public abstract void InputEvent(InputEvent InputEvent);
    }

    public abstract class InputEvent
    {
        public InputType InputType;
        
        public InputEvent(InputType InputType)
        {
            this.InputType = InputType;
        }

        public bool IsKeyboard()
        {
            return InputType == InputType.Keyboard;
        }

        public KeyEvent GetKeyboardEvent()
        {
            return this as KeyEvent;
        }

        public bool IsMouse()
        {
            return (IsMouseButton() || IsMouseCursor());
        }

        public bool IsMouseCursor()
        {
            return InputType == InputType.MouseCursor;
        }

        public MouseCursorEvent GetMouseCursorEvent()
        {
            return this as MouseCursorEvent;
        }

        public bool IsMouseButton()
        {
            return InputType == InputType.MouseButton;
        }

        public MouseButtonEvent GetMouseButtonEvent()
        {
            return this as MouseButtonEvent;
        }
    }

    public class KeyboardState
    {
        private MapCollection<Key, KeyState> KeyStates = new MapCollection<Key, KeyState>();

        public KeyboardState(OpenTK.GameWindow GameWindow)
        {
            GameWindow.Keyboard.KeyRepeat = true;
            OpenTK.Input.KeyboardState keyboardState = OpenTK.Input.Keyboard.GetState();
            Key[] keys = typeof(Key).GetEnumValues().Cast<Key>().ToArray();
            for(int i=0; i<keys.Length; i++)
            {
                KeyState keyState = new KeyState(keys[i]);
                keyState.IsClick = false;
                keyState.IsDown = keyboardState.IsKeyDown((OpenTK.Input.Key)keys[i]);
                keyState.IsUp = !keyState.IsDown;
                KeyStates.Add(keys[i], keyState);
            }
            GameWindow.KeyDown += (object sender, OpenTK.Input.KeyboardKeyEventArgs e) =>
            {
                KeyState keyState = KeyStates.GetValue((Key)e.Key);
                keyState.IsClick = (!keyState.IsDown ? true : false);
                keyState.IsDown = true;
                keyState.IsUp = false;
                Input.FireListeners(new KeyEvent(keyState));
            };
            GameWindow.KeyUp += (object sender, OpenTK.Input.KeyboardKeyEventArgs e) =>
            {
                KeyState keyState = KeyStates.GetValue((Key)e.Key);
                keyState.IsUp = true;
                keyState.IsDown = false;
                keyState.IsClick = false;
                Input.FireListeners(new KeyEvent(keyState));
            };
        }

        public KeyState GetKeyState(Key key)
        {
            return KeyStates.GetValue(key);
        }

        public bool IsKeyClicked(Key key)
        {
            return KeyStates.GetValue(key).IsClick;
        }

        public bool IsKeyDown(Key key)
        {
            return KeyStates.GetValue(key).IsDown;
        }

        public bool IsKeyUp(Key key)
        {
            return KeyStates.GetValue(key).IsUp;
        }
    }

    public class KeyEvent : InputEvent
    {
        public KeyState State;

        public KeyEvent(KeyState State) : base(InputType.Keyboard)
        {
            this.State = State;
        }
    }

    public class KeyState
    {
        public static readonly string Alphabet = "abcdefghijklmnopqrstuvwxyz";
        public static readonly string Numbers = "0123456789";

        public Key Key;
        public bool IsClick;
        public bool IsDown;
        public bool IsUp;

        public KeyState(Key key)
        {
            this.Key = key;
        }

        public bool IsAlphabetChar()
        {
            return (Key >= Key.A && Key <= Key.Z);
        }

        public char GetAlphabetChar()
        {
            return Alphabet[(int)Key - 83];
        }

        public bool IsNumberChar()
        {
            return (Key >= Key.Number0 && Key <= Key.Number9);
        }

        public char GetNumberChar()
        {
            return Numbers[(int)Key - 109];
        }
    }

    public class MouseState
    {
        private MouseCursorState Cursor = new MouseCursorState();
        private MapCollection<MouseButton, MouseButtonState> MouseButtonStates = new MapCollection<MouseButton, MouseButtonState>();

        public MouseState(OpenTK.GameWindow GameWindow)
        {
            OpenTK.Input.MouseState mouseState = OpenTK.Input.Mouse.GetCursorState();
            Cursor.X = mouseState.X;
            Cursor.Y = mouseState.Y;
            MouseButton[] buttons = typeof(MouseButton).GetEnumValues().Cast<MouseButton>().ToArray();
            for(int i=0; i<buttons.Length; i++)
            {
                MouseButtonState mouseButton = new MouseButtonState(buttons[i]);
                mouseButton.IsDown = mouseState.IsButtonDown((OpenTK.Input.MouseButton)buttons[i]);
                mouseButton.IsUp = !mouseButton.IsDown;
                MouseButtonStates.Add(mouseButton.Button, mouseButton);
            }
            GameWindow.MouseMove += (object sender, OpenTK.Input.MouseMoveEventArgs e) =>
            {
                Cursor.X = e.X;
                Cursor.Y = e.Y;
                Input.FireListeners(new MouseCursorEvent(Cursor));
            };
            GameWindow.MouseDown += (object sender, OpenTK.Input.MouseButtonEventArgs e) =>
            {
                MouseButtonState mouseButton = MouseButtonStates.GetValue((MouseButton)e.Button);
                mouseButton.IsDown = true;
                mouseButton.IsUp = false;
                Input.FireListeners(new MouseButtonEvent(mouseButton));
            };
            GameWindow.MouseUp += (object sender, OpenTK.Input.MouseButtonEventArgs e) =>
            {
                MouseButtonState mouseButton = MouseButtonStates.GetValue((MouseButton)e.Button);
                mouseButton.IsDown = false;
                mouseButton.IsUp = true;
                Input.FireListeners(new MouseButtonEvent(mouseButton));
            };
        }

        public MouseCursorState GetCursorState()
        {
            return Cursor;
        }

        public bool IsButtonDown(MouseButton Button)
        {
            return MouseButtonStates.GetValue(Button).IsDown;
        }

        public bool IsButtonUp(MouseButton Button)
        {
            return MouseButtonStates.GetValue(Button).IsUp;
        }
    }

    public class MouseCursorState
    {
        public int X;
        public int Y;
    }

    public class MouseCursorEvent : InputEvent
    {
        public MouseCursorState State;

        public MouseCursorEvent(MouseCursorState State) : base(InputType.MouseCursor)
        {
            this.State = State;
        }
    }

    public class MouseButtonEvent : InputEvent
    {
        public MouseButtonState State;

        public MouseButtonEvent(MouseButtonState State) : base(InputType.MouseButton)
        {
            this.State = State;
        }
    }

    public class MouseButtonState
    {
        public MouseButton Button;
        public bool IsClick;
        public bool IsDoubleClick;
        public bool IsDown;
        public bool IsUp;

        public MouseButtonState(MouseButton Button)
        {
            this.Button = Button;
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

    public enum MouseButton
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
