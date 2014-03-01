using System;
using System.Windows.Input;
using ICSharpCode.AvalonEdit;
using XSemmel.Helpers.WPF;
using System.Windows;

namespace XSemmel.Editor
{
    class GotoLine
    {
        private readonly TextEditor _editor;

        public GotoLine(TextEditor editor)
        {
            _editor = editor;
            _editor.TextArea.KeyDown += textEditor_TextArea_KeyDown;
        }

        private void textEditor_TextArea_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.G && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                ShowDialog();
                e.Handled = true;
            }
        }

        public void ShowDialog()
        {
            int maxLines = _editor.Document.Lines.Count;

            string linenum = InputBox.Show(Application.Current.MainWindow, "Go To Line", "Line number ( 1 - " + maxLines + "):");
            if (linenum != null)
            {
                int line;
                if (int.TryParse(linenum, out line))
                {
                    try
                    {
                        var docline = _editor.Document.GetLineByNumber(line);
                        _editor.CaretOffset = docline.Offset;
                        _editor.ScrollToLine(line);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(Application.Current.MainWindow, ex.Message, "Error",
                                        MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    MessageBox.Show(Application.Current.MainWindow, "Only numbers are allowed", "Error",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }

    }
}
