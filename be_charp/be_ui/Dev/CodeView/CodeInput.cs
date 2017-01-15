using Be.Integrator;
using Be.UI;
using Be.UI.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Be.Integrator
{
    public class CodeInput : InputListener
    {
        public CodeText CodeText;

        public CodeInput(CodeText CodeText)
        {
            this.CodeText = CodeText;
        }

        public override void InputEvent(InputEvent InputEvent)
        {
            if( MouseInputs(InputEvent) ||
                CursorInputs(InputEvent) ||
                KeyInputs(InputEvent) ||
                TextInputs(InputEvent)
            ){
                ; // event
            }
        }

        public bool MouseInputs(InputEvent InputEvent)
        {
            if(InputEvent.IsMouseButton())
            {
                MouseButtonState buttonState = InputEvent.GetMouseButtonEvent().State;
                if(buttonState.IsDown && buttonState.Button == MouseButton.Left)
                {
                    MouseCursorState cursorState = Input.Mouse.GetCursorState();
                    CodeText.CodeCursor.SetCursor(cursorState.X, cursorState.Y);
                    CodeText.CodeSelection.Begin(CodeText.CodeCursor.LineNumber, CodeText.CodeCursor.CursorPosition);
                    CodeText.CodeCursor.CursorBlink.Reset();
                    return true;
                }
            }
            else if(InputEvent.IsMouseCursor())
            {
                MouseCursorState cursorState = InputEvent.GetMouseCursorEvent().State;
                if (Input.Mouse.IsButtonDown(MouseButton.Left))
                {
                    CodeText.CodeCursor.SetCursor(cursorState.X, cursorState.Y);
                    CodeText.CodeSelection.End(CodeText.CodeCursor.LineNumber, CodeText.CodeCursor.CursorPosition);
                    CodeText.CodeCursor.CursorBlink.Reset();
                    return true;
                }
            }
            return false;
        }

        public bool CursorInputs(InputEvent InputEvent)
        {
            if (InputEvent.IsKeyboard())
            {
                KeyState keyState = InputEvent.GetKeyboardEvent().State;
                Key key = keyState.Key;
                bool isDown = keyState.IsDown;
                bool isClick = keyState.IsClick;

                // cursor-navigation
                bool cursorNavigation = true;
                if (key == Key.Left && isDown)
                {
                    CodeText.CodeCursor.CursorLeft();
                }
                else if (key == Key.Right && isDown)
                {
                    CodeText.CodeCursor.CursorRight();
                }
                else if (key == Key.Up && isDown)
                {
                    CodeText.CodeCursor.CursorUp();
                }
                else if (key == Key.Down && isDown)
                {
                    CodeText.CodeCursor.CursorDown();
                }
                else
                {
                    cursorNavigation = false;
                }

                // code-selection
                if (keyState.IsClick && (keyState.Key == Key.ShiftLeft || keyState.Key == Key.ShiftRight))
                {
                    CodeText.CodeSelection.Begin(CodeText.CodeCursor.LineNumber, CodeText.CodeCursor.CursorPosition);
                }
                else if(cursorNavigation && (Keyboard.IsKeyDown(Key.ShiftLeft) || Keyboard.IsKeyDown(Key.ShiftRight)))
                {
                    CodeText.CodeSelection.End(CodeText.CodeCursor.LineNumber, CodeText.CodeCursor.CursorPosition);
                }
                else if(cursorNavigation)
                {
                    CodeText.CodeSelection.Clear();
                }

                CodeText.CodeCursor.CursorBlink.Reset();
                return false;
            }
            return false;
        }

        public bool KeyInputs(InputEvent InputEvent)
        {
            if (!InputEvent.IsKeyboard())
            {
                return false;
            }

            KeyState keyState = InputEvent.GetKeyboardEvent().State;
            Key key = keyState.Key;
            bool isDown = keyState.IsDown;
            bool isClick = keyState.IsClick;

            // backspace
            if (isDown && key == Key.BackSpace)
            {
                CodeText.CodeCursor.KeyBackspace();
            }
            // delete
            else if (isDown && key == Key.Delete)
            {
                CodeText.CodeCursor.KeyDelete();
            }
            // enter
            else if(isDown && key == Key.Enter)
            {
                CodeText.CodeCursor.KeyEnter();
            }
            // save
            else if (isDown && (Keyboard.IsKeyDown(Key.ControlLeft) || Keyboard.IsKeyDown(Key.ControlRight)) && key == Key.S)
            {
                if (isClick)
                {
                    CodeText.CodeContainer.Save();
                }
            }
            // paste
            else if (isClick && (Keyboard.IsKeyDown(Key.ControlLeft) || Keyboard.IsKeyDown(Key.ControlRight)) && key == Key.V)
            {
                if (isClick)
                {
                    CodeText.CodeCursor.Paste();
                }
            }
            // cut
            else if(isClick && (Keyboard.IsKeyDown(Key.ControlLeft) || Keyboard.IsKeyDown(Key.ControlRight)) && key == Key.X)
            {
                if(isClick)
                {
                    CodeText.CodeCursor.Cut();
                }
            }
            // copy
            else if (isClick && (Keyboard.IsKeyDown(Key.ControlLeft) || Keyboard.IsKeyDown(Key.ControlRight)) && key == Key.C)
            {
                if (isClick)
                {
                    CodeText.CodeCursor.Copy();
                }
            }
            // undo
            else if (isDown && (Keyboard.IsKeyDown(Key.ControlLeft) || Keyboard.IsKeyDown(Key.ControlRight)) && key == Key.Y /* todo: z */)
            {
                CodeText.CodeHistory.UndoStep();
            }
            // redo
            else if (isDown && (Keyboard.IsKeyDown(Key.ControlLeft) || Keyboard.IsKeyDown(Key.ControlRight)) && key == Key.Z /* todo: y */)
            {
                CodeText.CodeHistory.RedoStep();    
            }
            // none
            else
            {
                return false;
            }

            CodeText.CodeCursor.CursorBlink.Reset();
            return true;
        }

        public bool TextInputs(InputEvent InputEvent)
        {
            if (!InputEvent.IsKeyboard())
            {
                return false;
            }

            KeyState keyState = InputEvent.GetKeyboardEvent().State;
            Key key = keyState.Key;

            if (!keyState.IsDown)
            {
                return false;
            }

            char textChar = '\0';

            // alphabetic and specials
            if (keyState.IsAlphabetChar())
            {
                textChar = keyState.GetAlphabetChar();
                // todo: y/z
                if (textChar == 'z')
                {
                    textChar = 'y';
                }
                else if(textChar == 'y')
                {
                    textChar = 'z';
                }
                if (Keyboard.IsKeyDown(Key.ShiftLeft) || Keyboard.IsKeyDown(Key.ShiftRight))
                {
                    textChar = Char.ToUpper(textChar);
                }
                else if (Keyboard.IsKeyDown(Key.AltRight))
                {
                    textChar = '@';
                }
            }
            // numeric and specials
            else if (keyState.IsNumberChar())
            {
                textChar = keyState.GetNumberChar();
                if(Keyboard.IsKeyDown(Key.ShiftLeft) || Keyboard.IsKeyDown(Key.ShiftRight))
                {
                    if(textChar == '0')
                    {
                        textChar = '=';
                    }
                    else if(textChar == '1')
                    {
                        textChar = '!';
                    }
                    else if(textChar == '2')
                    {
                        textChar = '"';
                    }
                    else if(textChar == '3')
                    {
                        textChar = '§';
                    }
                    else if(textChar == '4')
                    {
                        textChar = '$';
                    }
                    else if(textChar == '5')
                    {
                        textChar = '%';
                    }
                    else if (textChar == '6')
                    {
                        textChar = '&';
                    }
                    else if (textChar == '7')
                    {
                        textChar = '/';
                    }
                    else if (textChar == '8')
                    {
                        textChar = '(';
                    }
                    else if (textChar == '9')
                    {
                        textChar = ')';
                    }
                }
                else if(Keyboard.IsKeyDown(Key.AltLeft) || Keyboard.IsKeyDown(Key.AltRight))
                {
                    if(textChar == '7')
                    {
                        textChar = '{';
                    }
                    else if(textChar == '8')
                    {
                        textChar = '[';
                    }
                    else if(textChar == 9)
                    {
                        textChar = ']';
                    }
                    else if(textChar == '0')
                    {
                        textChar = '}';
                    }
                }
            }
            // space
            else if (key == Key.Space)
            {
                textChar = ' ';
            }
            // tab
            else if (key == Key.Tab)
            {
                textChar = '\t';
            }
            // ß
            else if(key == Key.Minus)
            {
                if(Keyboard.IsKeyDown(Key.ShiftLeft) || Keyboard.IsKeyDown(Key.ShiftRight))
                {
                    textChar = '?';
                }
                else if(Keyboard.IsKeyDown(Key.AltLeft) || Keyboard.IsKeyDown(Key.AltRight))
                {
                    textChar = '\\';
                }
                else
                {
                    textChar = 'ß';
                }
            }
            // +
            else if(key == Key.BracketRight)
            {
                if (Keyboard.IsKeyDown(Key.ShiftLeft) || Keyboard.IsKeyDown(Key.ShiftRight))
                {
                    textChar = '*';
                }
                else if (Keyboard.IsKeyDown(Key.AltLeft) || Keyboard.IsKeyDown(Key.AltRight))
                {
                    textChar = '~';
                }
                else
                {
                    textChar = '+';
                }
            }
            // -
            else if (key == Key.Slash)
            {
                if (Keyboard.IsKeyDown(Key.ShiftLeft) || Keyboard.IsKeyDown(Key.ShiftRight))
                {
                    textChar = '_';
                }
                else
                {
                    textChar = '-';
                }
            }
            // #
            else if (key == Key.BackSlash)
            {
                if (Keyboard.IsKeyDown(Key.ShiftLeft) || Keyboard.IsKeyDown(Key.ShiftRight))
                {
                    textChar = '\'';
                }
                else
                {
                    textChar = '#';
                }
            }
            // .
            else if (key == Key.Period)
            {
                if (Keyboard.IsKeyDown(Key.ShiftLeft) || Keyboard.IsKeyDown(Key.ShiftRight))
                {
                    textChar = ':';
                }
                else
                {
                    textChar = '.';
                }
            }
            // ,
            else if (key == Key.Comma)
            {
                if (Keyboard.IsKeyDown(Key.ShiftLeft) || Keyboard.IsKeyDown(Key.ShiftRight))
                {
                    textChar = ';';
                }
                else
                {
                    textChar = ',';
                }
            }
            // <
            else if(key == Key.NonUSBackSlash)
            {
                if (Keyboard.IsKeyDown(Key.ShiftLeft) || Keyboard.IsKeyDown(Key.ShiftRight))
                {
                    textChar = '>';
                }
                else if (Keyboard.IsKeyDown(Key.AltLeft) || Keyboard.IsKeyDown(Key.AltRight))
                {
                    textChar = '|';
                }
                else
                {
                    textChar = '<';
                }
            }
            // none
            else
            {
                return false;
            }

            // insert
            CodeText.CodeCursor.TextInsert(textChar + "");
            CodeText.CodeCursor.CursorBlink.Reset();
            return true;
        }
    }
}
