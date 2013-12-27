using System;
using System.Diagnostics;
using ICSharpCode.AvalonEdit;
using System.Windows.Input;
using ICSharpCode.AvalonEdit.Document;

namespace XSemmel.Editor
{
    class Commenter
    {
        private readonly TextEditor _editor;


        public Commenter(TextEditor editor)
        {
            _editor = editor;
            _editor.TextArea.KeyDown += textEditor_TextArea_KeyDown;
        }


        private void textEditor_TextArea_KeyDown(object sender, KeyEventArgs e)
        {
            var modifiers = Keyboard.Modifiers;
            if (e.Key == Key.D7 
                && (modifiers & ModifierKeys.Control) == ModifierKeys.Control
                && (modifiers & ModifierKeys.Alt) == 0
                )
            {
                int offset = _editor.CaretOffset;
                if (XParser.IsInsideComment(_editor.Text, offset))
                {
                    //uncomment
                    int idxClosing = _editor.Text.IndexOf("-->", offset);
                    if (idxClosing >= 0)
                    {
                        _editor.Text = _editor.Text.Remove(idxClosing, 3);
                    }
                    int idxOpening = _editor.Text.LastIndexOf("<!--", offset);
                    Debug.Assert(idxOpening >= 0);
                    if (idxOpening >= 0)
                    {
                        _editor.Text = _editor.Text.Remove(idxOpening, 4);
                    }
                    _editor.CaretOffset = Math.Max(offset - 4, 0);
                }
                else
                {
                    if (_editor.SelectionLength == 0)
                    {
                        //comment whole line
                        DocumentLine line = _editor.Document.GetLineByOffset(offset);
                        _editor.Text = _editor.Text.Insert(line.EndOffset, "-->");
                        _editor.Text = _editor.Text.Insert(line.Offset, "<!--");
                    }
                    else
                    {
                        //comment selection
                        int selStart = _editor.SelectionStart;
                        int selEnd = _editor.SelectionStart + _editor.SelectionLength;
                        _editor.Text = _editor.Text.Insert(selEnd, "-->");
                        _editor.Text = _editor.Text.Insert(selStart, "<!--");
                    }
                    _editor.CaretOffset = offset + 4;
                }
                e.Handled = true;
            }
        }


    }
}
