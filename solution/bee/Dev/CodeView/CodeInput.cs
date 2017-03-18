using feltic.Integrator;
using feltic.UI;
using feltic.UI.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace feltic.Integrator
{
    public class CodeInput : InputListener
    {
        public CodeText CodeText;

        public CodeInput(CodeText CodeText)
        {
            this.CodeText = CodeText;
        }

        public override void Input(InputEvent InputEvent)
        {
            if( MouseInputs(InputEvent) ||
                CursorInputs(InputEvent) ||
                KeyInputs(InputEvent) ||
                TextInputs(InputEvent)
            ){
                ; // event
            }
        }

        public bool MouseInputs(InputEvent Event)
        {
            if(Event.IsButton)
            {
                ButtonState buttonState = Event.Button;
                if(buttonState.IsDown && buttonState.Type == Button.Left)
                {
                    CursorState cursorState = Mouse.Cursor;
                    CodeText.CodeCursor.SetCursor(cursorState.x, cursorState.y);
                    CodeText.CodeSelection.Begin(CodeText.CodeCursor.LineNumber, CodeText.CodeCursor.CursorPosition);
                    CodeText.CodeCursor.CursorBlink.Reset();
                    return true;
                }
          
            }
            else if(Event.IsCursor)
            {
                CursorState cursorState = Event.Cursor;
                if (Mouse.Buttons[Button.Left].IsDown)
                {
                    CodeText.CodeCursor.SetCursor(cursorState.x, cursorState.y);
                    CodeText.CodeSelection.End(CodeText.CodeCursor.LineNumber, CodeText.CodeCursor.CursorPosition);
                    CodeText.CodeCursor.CursorBlink.Reset();
                    return true;
                }
            }
            return false;
        }

        public bool CursorInputs(InputEvent Event)
        {
            if (Event.IsKey)
            {
                KeyState keyState = Event.Key;
                Key key = keyState.Type;
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
                if (keyState.IsClick && keyState.Type == Key.ShiftLeft)
                {
                    CodeText.CodeSelection.Begin(CodeText.CodeCursor.LineNumber, CodeText.CodeCursor.CursorPosition);
                }
                else if(cursorNavigation && Keyboard.Keys[Key.ShiftLeft].IsDown)
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
            if(!InputEvent.IsKey)
            {
                return false;
            }

            KeyState keyState = InputEvent.Key;
            Key key = keyState.Type;
            bool isDown = keyState.IsDown;
            bool isClick = keyState.IsClick;

            CodeHistoryEntry historyEntry = new CodeHistoryEntry(CodeText.SourceText.StringContent, CodeText.CodeCursor, CodeText.CodeSelection);

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
            else if (isDown && Keyboard.Keys[Key.ControlLeft].IsDown && key == Key.S)
            {
                if (isClick)
                {
                    CodeText.CodeContainer.Save();
                    CodeText.Registry.WriteToTarget("D:\\dev\\UndefinedProject\\output\\csharp.cs");
                }
            }
            // paste
            else if (isClick && Keyboard.Keys[Key.ControlLeft].IsDown && key == Key.V)
            {
                if (isClick)
                {
                    CodeText.CodeCursor.Paste();
                }
            }
            // cut
            else if(isClick && Keyboard.Keys[Key.ControlLeft].IsDown && key == Key.X)
            {
                if(isClick)
                {
                    CodeText.CodeCursor.Cut();
                }
            }
            // copy
            else if (isClick && Keyboard.Keys[Key.ControlLeft].IsDown && key == Key.C)
            {
                if (isClick)
                {
                    CodeText.CodeCursor.Copy();
                }
            }
            // select-all
            else if (isClick && Keyboard.Keys[Key.ControlLeft].IsDown && key == Key.A)
            {
                if (isClick)
                {
                    CodeText.CodeSelection.Begin(0, 0);
                    CodeText.CodeSelection.End(CodeText.TokenContainer.LineCount() - 1, CodeText.TokenContainer.TextCount(CodeText.TokenContainer.LineCount() - 1));
                }
            }
            // undo
            else if (isClick && Keyboard.Keys[Key.ControlLeft].IsDown && key == Key.Y /* todo: z */)
            {
                CodeHistoryEntry undoHistory = CodeText.CodeHistory.UndoHistory();
                if(undoHistory != null)
                {
                    CodeText.SetSourceText(CodeText.SourceText.SetText(undoHistory.CodeText));
                    CodeText.CodeCursor.Bind(undoHistory.CodeCursor);
                    CodeText.CodeSelection.Bind(undoHistory.CodeSelection);
                }
                CodeText.CodeCursor.CursorBlink.Reset();
                return true;
            }
            // redo
            else if (isClick && Keyboard.Keys[Key.ControlLeft].IsDown && key == Key.Z /* todo: y */)
            {
                CodeHistoryEntry redoHistory = CodeText.CodeHistory.RedoHistory();
                if(redoHistory != null)
                {
                    CodeText.SetSourceText(CodeText.SourceText.SetText(redoHistory.CodeText));
                    CodeText.CodeCursor.Bind(redoHistory.CodeCursor);
                    CodeText.CodeSelection.Bind(redoHistory.CodeSelection);
                }
                CodeText.CodeCursor.CursorBlink.Reset();
                return true;
            }
            // none
            else
            {
                return false;
            }

            CodeText.CodeHistory.AddHistory(historyEntry);
            CodeText.CodeCursor.CursorBlink.Reset();
            return true;
        }

        public bool TextInputs(InputEvent InputEvent)
        {
            if(!InputEvent.IsKey)
                return false;

            if(!InputEvent.Key.IsDown)
                return false;

            string textContent = InputEvent.Key.TextContent;
            if(textContent == null)
                return false;

            // insert
            CodeText.CodeHistory.AddHistory(new CodeHistoryEntry(CodeText.SourceText.StringContent, CodeText.CodeCursor, CodeText.CodeSelection));
            CodeText.CodeCursor.TextInsert(textContent);
            CodeText.CodeCursor.CursorBlink.Reset();
            return true;
        }
    }
}
