using System.Diagnostics;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;

namespace XSemmel.Editor
{
    class ShortcutNavigator
    {
        private readonly TextEditor _editor;

        public ShortcutNavigator(TextEditor editor)
        {
            _editor = editor;
            _editor.TextArea.KeyDown += textEditor_TextArea_KeyDown;
        }

        private void textEditor_TextArea_KeyDown(object sender, KeyEventArgs e)
        {
            if (    e.Key == Key.Right 
                && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control
                && (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt)
            {
                string xml = _editor.Document.Text;
                int offset = _editor.CaretOffset;
                
                try
                {
                    if (XParser.IsClosingElement(xml, offset))
                    {
                        int i = xml.IndexOf('>', offset);
                        if (i > 0)
                        {
                            _editor.CaretOffset = i + 1;
                        }
                    }
                    else if (XParser.IsInsideElementDeclaration(xml, offset))
                    {
                        if (XParser.IsInsideAttributeKey(xml, offset))
                        {
                            int i = xml.IndexOf('=', offset);
                            if (i > 0)
                            {
                                _editor.CaretOffset = i + 1;
                            }
                        }
                        else if (XParser.IsInsideAttributeValue(xml, offset))
                        {
                            int i = xml.IndexOf('>', offset);
                            if (i > 0)
                            {
                                _editor.CaretOffset = i + 1;
                            }
                        }
                        else
                        {
                            //element name
                            int i = xml.IndexOf('>', offset);
                            if (i > 0)
                            {
                                _editor.CaretOffset = i + 1;
                            }
                        }
                    }
                    else if (XParser.IsInsideComment(xml, offset))
                    {
                        int i = xml.IndexOf("-->", offset);
                        if (i > 0)
                        {
                            _editor.CaretOffset = i + 4;
                        }
                    }
                    else
                    {
                        //text, goto next element
                        int i = xml.IndexOf('<', offset);
                        if (i > 0)
                        {
                            if (i == offset)
                            {
                                i = xml.IndexOf('>', offset);
                            }
                            _editor.CaretOffset = i + 1;
                        }
                    }
                } 
                catch
                {
                    Debug.Fail("");
                }

                e.Handled = true;
            }
        }

    }
}
