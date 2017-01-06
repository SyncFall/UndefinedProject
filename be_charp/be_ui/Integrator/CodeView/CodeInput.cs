﻿using Be.Integrator;
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
            if (!InputEvent.IsMouse())
            {
                return false;
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
                    return false;
                }

                // code-selection
                if(Keyboard.IsKeyDown(Key.ShiftLeft) || Keyboard.IsKeyDown(Key.ShiftRight))
                {
                    CodeText.CodeSelection.End(CodeText.CodeCursor.LinePosition, CodeText.CodeCursor.CursorPosition);
                }
                else
                {
                    CodeText.CodeSelection.Begin(CodeText.CodeCursor.LinePosition, CodeText.CodeCursor.CursorPosition);
                }

                return true;
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
                CodeText.CodeCursor.CursorBlink.Reset();
            }
            // cut
            else if(isClick && (Keyboard.IsKeyDown(Key.ControlLeft) || Keyboard.IsKeyDown(Key.ControlRight)) && key == Key.X)
            {
                if(isClick)
                {
                    ;//
                }
                CodeText.CodeCursor.CursorBlink.Reset();
            }
            // undo
            else if (isDown && (Keyboard.IsKeyDown(Key.ControlLeft) || Keyboard.IsKeyDown(Key.ControlRight)) && key == Key.Y /* todo: z */)
            {
                CodeText.CodeHistory.UndoStep();
                CodeText.CodeCursor.CursorBlink.Reset();
            }
            // redo
            else if (isDown && (Keyboard.IsKeyDown(Key.ControlLeft) || Keyboard.IsKeyDown(Key.ControlRight)) && key == Key.Z /* todo: y */)
            {
                CodeText.CodeHistory.RedoStep();
                CodeText.CodeCursor.CursorBlink.Reset();
            }
            // none
            else
            {
                return false;
            }

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
            return true;
        }
    }
}
