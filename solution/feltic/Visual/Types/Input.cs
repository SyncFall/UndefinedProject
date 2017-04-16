using feltic.Lib;
using feltic.Library;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Visual
{
    public enum InputType
    {
        Key,
        Cursor,
        Button,
        Scroll,
        Text,
        Visual,
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
            get{ return (this.Type == InputType.Key); }
        }

        public bool IsCursor
        {
            get{ return (this.Type == InputType.Cursor); }
        }

        public bool IsButton
        {
            get{ return (this.Type == InputType.Button); }
        }

        public bool IsText
        {
            get{ return (this.Type == InputType.Text); }
        }

        public bool IsVisual
        {
            get { return (this.Type == InputType.Visual); }
        }

        public KeyState Key
        {
            get
            {
                if (IsKey)
                    return (State as KeyState);
                else
                    return new KeyState(feltic.Visual.Key.Unknown);
            }
        }

        public CursorState Cursor
        {
            get
            {
                if (IsCursor)
                    return (State as CursorState);
                else
                    return new CursorState();
            }
        }

        public ButtonState Button
        {
            get
            {
                if (IsButton)
                    return (State as ButtonState);
                else
                    return new ButtonState(feltic.Visual.Button.LastButton);
            }
        }

        public TextState Text
        {
            get
            {
                if (IsText)
                    return (State as TextState);
                else
                    return new TextState(null);
            }
        }

        public VisualState Visual
        {
            get
            {
                if (IsVisual)
                    return (State as VisualState);
                else
                    return new VisualState(VisualEventState.None, false);
            }
        }
    }

    public class Input
    {
        public static MapCollection<int, InputListener> Listeners = new MapCollection<int, InputListener>();
        public static MouseState Mouse;
        public static CursorState Cursor;
        public static KeyboardState Keyboard;

        public static void Inititialize(OpenTK.GameWindow GameWindow)
        {
            Mouse = new MouseState(GameWindow);
            Cursor = Mouse.Cursor;
            Keyboard = new KeyboardState(GameWindow);
        }

        public static void FireListeners(InputEvent InputEvent)
        {
            int[] keys = Listeners.Keys;
            for (int i=0; i<keys.Length; i++)
            {
                Listeners[keys[i]].ProcessInputEvent(InputEvent);
            }
        }

        public static void Add(InputListener InputListener)
        {
            InputListener.Mouse = Mouse;
            InputListener.Cursor = Cursor;
            InputListener.Keyboard = Keyboard;
            Listeners[InputListener.Id] = InputListener;
        }

        public static void Remove(InputListener InputListener)
        {
            Listeners.Remove(InputListener.Id);
        }
    }

    public abstract class InputListener
    {
        private static int IdCounter = 0;
        public readonly int Id;
        public object Sender;
        public MouseState Mouse;
        public CursorState Cursor;
        public KeyboardState Keyboard;

        public InputListener()
        {
            this.Id = (++IdCounter);
            Visual.Input.Add(this);
        }

        public void ProcessInputEvent(InputEvent Event)
        {
            this.Input(Event);    
        }

        public void Dispose()
        {
            Visual.Input.Remove(this);
        }

        public abstract void Input(InputEvent Event);
    }

    public enum VisualEventState
    {
        None=0,
        Active,
        Focus,
    }

    public class VisualState : InputState
    {
        public bool GainActive;
        public bool LostActive;
        public bool GainFocus;
        public bool LostFocus;

        public VisualState(VisualEventState State, bool Boolean)
        {
            if (State == VisualEventState.Active)
            {
                if (Boolean)
                    GainActive = true;
                else
                    LostActive = true;
            }
            else if(State == VisualEventState.Focus)
            {
                if (Boolean)
                    GainFocus = true;
                else
                    LostFocus = true;
            }
        }
    }

    public class TextState : InputState
    {
        public string TextContent;

        public TextState(string Text)
        {
            this.TextContent = Text;
        }
    }

    public class KeyboardState
    {
        public KeyStateCollection Keys = new KeyStateCollection();

        public bool HoldShift
        {
            get { return Keys.HoldShift; }
        }

        public bool HoldControl
        {
            get { return Keys.HoldControl; }
        }

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
                if (keyState.IsDown && keyState.TextContent != null)
                {
                    Input.FireListeners(new InputEvent(InputType.Text, new TextState(keyState.TextContent)));
                }
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
        public bool HoldShift
        {
            get { return (this[Visual.Key.ShiftLeft].IsDown || this[Visual.Key.ShiftRight].IsDown); }
        }

        public bool HoldControl
        {
            get { return (this[Visual.Key.ControlLeft].IsDown || this[Visual.Key.ControlRight].IsDown); }
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

        public bool IsAlphabet
        {
            get{ return (Type >= Key.A && Type <= Key.Z); }
        }

        public char AlphabetChar
        {
            get{ return Alphabet[(int)Type - 83]; }
        }

        public bool IsNumber
        {
            get{ return (Type >= Key.Number0 && Type <= Key.Number9); } 
        }

        public char NumberChar
        {
            get{ return Numbers[(int)Type - 109]; }
        }

        public string TextContent
        {
            get
            {
                if(IsAlphabet)
                {
                    char chr = AlphabetChar;
                    // y/z
                    if (chr == 'z')
                        chr = 'y';
                    else if (chr == 'y')
                        chr = 'z';
                    // upper char
                    if (Visual.Input.Keyboard.Keys[Key.ShiftLeft].IsDown)
                        chr = Char.ToUpper(chr);
                    // @
                    else if (Visual.Input.Keyboard.Keys[Key.AltRight].IsDown)
                        chr = '@';
                    // ret
                    return chr+"";
                }
                if(IsNumber)
                {
                    char chr = NumberChar;
                    // shift
                    if (Visual.Input.Keyboard.Keys[Key.ShiftLeft].IsDown)
                    {
                        if (chr == '0')
                            chr = '=';
                        else if (chr == '1')
                            chr = '!';
                        else if (chr == '2')
                            chr = '"';
                        else if (chr == '3')
                            chr = '§';
                        else if (chr == '4')
                            chr = '$';
                        else if (chr == '5')
                            chr = '%';
                        else if (chr == '6')
                            chr = '&';
                        else if (chr == '7')
                            chr = '/';
                        else if (chr == '8')
                            chr = '(';
                        else if (chr == '9')
                            chr = ')';
                    }
                    // alt-gr
                    else if (Visual.Input.Keyboard.Keys[Key.AltLeft].IsDown || Visual.Input.Keyboard.Keys[Key.AltRight].IsDown)
                    {
                        if (chr == '7')
                            chr = '{';
                        else if (chr == '8')
                            chr = '[';
                        else if (chr == '9')
                            chr = ']';
                        else if (chr == '0')
                            chr = '}';
                    }
                    // ret
                    return chr+"";
                }
                // space
                else if (Type == Key.Space)
                {
                    return " ";
                }
                // tab
                else if (Type == Key.Tab)
                {
                    return "\t";
                }
                // ?
                else if (Type == Key.Minus)
                {
                    if (Visual.Input.Keyboard.Keys[Key.ShiftLeft].IsDown)
                        return "?";
                    else if (Visual.Input.Keyboard.Keys[Key.AltRight].IsDown || Visual.Input.Keyboard.Keys[Key.AltRight].IsDown)
                        return "\\";
                    else
                        return "?";
                }
                // +
                else if (Type == Key.BracketRight)
                {
                    if (Visual.Input.Keyboard.Keys[Key.ShiftLeft].IsDown)
                        return "*";
                    else if (Visual.Input.Keyboard.Keys[Key.AltLeft].IsDown || Visual.Input.Keyboard.Keys[Key.AltRight].IsDown)
                        return "~";
                    else
                        return "+";
                }
                // -
                else if (Type == Key.Slash)
                {
                    if (Visual.Input.Keyboard.Keys[Key.ShiftLeft].IsDown)
                        return "_";
                    else
                        return "-";
                }
                // #
                else if (Type == Key.BackSlash)
                {
                    if (Visual.Input.Keyboard.Keys[Key.ShiftLeft].IsDown)
                        return "'";
                    else
                        return "#";
                }
                // .
                else if (Type == Key.Period)
                {
                    if (Visual.Input.Keyboard.Keys[Key.ShiftLeft].IsDown)
                        return ":";
                    else
                        return ".";
                }
                // ,
                else if (Type == Key.Comma)
                {
                    if (Visual.Input.Keyboard.Keys[Key.ShiftLeft].IsDown)
                        return ";";
                    else
                        return ",";
                }
                // <
                else if (Type == Key.NonUSBackSlash)
                {
                    if (Visual.Input.Keyboard.Keys[Key.ShiftLeft].IsDown)
                        return ">";
                    else if (Visual.Input.Keyboard.Keys[Key.AltLeft].IsDown || Visual.Input.Keyboard.Keys[Key.AltRight].IsDown)
                        return  "|";
                    else
                        return "<";
                }
                // none
                else
                {
                    return null;
                }
            }
        }
    }

    public class MouseState
    {
        private CursorState CursorState = new CursorState();
        private ButtonStateCollection ButtonStateCollection = new ButtonStateCollection();

        public CursorState Cursor
        {
            get { return CursorState; }
        }

        public ButtonStateCollection Buttons
        {
            get { return ButtonStateCollection; }
        }

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
    }

    public class ButtonStateCollection : MapCollection<Button, ButtonState>
    {
        public bool LeftClick
        {
            get { return this[Visual.Button.Left].IsClick; }
        }
    }

    public class CursorState : InputState
    {
        public int x = -1;
        public int y = -1;

        public CursorState()
        { }
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

        public bool LeftClick
        {
            get { return (Type == Button.Left && IsClick); }
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
        LastButton = 12,
        
    }
}
