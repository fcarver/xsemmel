using System;
using System.Windows.Input;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;

namespace XSemmel.Editor
{
    class HighlightOccurrences
    {
        private readonly TextEditor _editor;
        private readonly MarkBackgroundRenderer _marker;

        private string _highlightedWord;

        public HighlightOccurrences(TextEditor editor)
        {
            _editor = editor;

            _marker = new MarkBackgroundRenderer(editor);
            _editor.TextArea.TextView.BackgroundRenderers.Add(_marker);

            _editor.TextArea.KeyDown += textEditor_TextArea_KeyDown;
            _editor.TextChanged += _editor_TextChanged;
        }

        void _editor_TextChanged(object sender, EventArgs e)
        {
            if (_highlightedWord != null)
            {
                if (_marker.MarkCount > 0)
                {
                    _marker.ClearMarks();
                }
                highlightWord(_highlightedWord);
            }
        }

        private void textEditor_TextArea_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F11 
              //  && (Keyboard.Modifiers & ModifierKeys.Alt) == ModifierKeys.Alt
              //  && (Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift
                )
            {
                if (_highlightedWord != null || _marker.MarkCount > 0)
                {
                    _marker.ClearMarks();
                    _highlightedWord = null;
                }
                else
                {
                    string word;
                    if (string.IsNullOrEmpty(_editor.SelectedText))
                    {
                        word = getWordUnderCursor(_editor.Text, _editor.CaretOffset);
                    }
                    else
                    {
                        word = _editor.SelectedText;
                    }

                    highlightWord(word);
                }
                e.Handled = true;
            }
        }


        private void highlightWord(string word)
        {
            if (string.IsNullOrWhiteSpace(word))
            {
                return;
            }

            int startIdx = 0;
            while (true)
            {
                int idx = _editor.Text.IndexOf(word, startIdx);

                if (idx < 0)
                {
                    break;
                }
                startIdx = idx + 1;

                _marker.AddOffsetToMark(new MarkBackgroundRenderer.Mark
                {
                    Offset = idx,
                    Length = word.Length,
                    Brush = Brushes.MediumSpringGreen
                });
            }
            _highlightedWord = word;
        }


        private string getWordUnderCursor(string text, int offset)
        {
            int startIdx = 0;
            for (int i = offset; i >= 0; i--)
            {
                char c = text[i];
                if (!char.IsLetterOrDigit(c))
                {
                    startIdx = i + 1;
                    break;
                }
            }

            int endIdx = text.Length-1;
            for (int i = offset; i < text.Length; i++)
            {
                char c = text[i];
                if (!char.IsLetterOrDigit(c))
                {
                    endIdx = i - 1;
                    break;
                }
            }

            int length = endIdx - startIdx + 1;
            if (length > 0)
            {
                return text.Substring(startIdx, length);
            }
            return null;
        }


    }
}
