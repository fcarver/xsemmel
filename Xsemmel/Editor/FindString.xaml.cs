using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using ICSharpCode.AvalonEdit;

namespace XSemmel.Editor
{
    public partial class FindString
    {
        
        private readonly TextEditor _editor;
        private readonly MarkBackgroundRenderer _marker;

        private readonly IList<int> _foundOffsets = new List<int>();


        private FindString()
        {
            InitializeComponent();
        }

        public FindString(TextEditor editor)
        {
            InitializeComponent();
            _editor = editor;

            _marker = new MarkBackgroundRenderer(editor);
            _editor.TextArea.TextView.BackgroundRenderers.Add(_marker);

            _editor.TextArea.KeyDown += keyDown;
            _edtSearchText.KeyDown += keyDown;

            _editor.TextChanged += _editor_TextChanged;
        }

        void _editor_TextChanged(object sender, EventArgs e)
        {
            startSearch();
        }

        private void keyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F3)
            {
                gotoNext();
                e.Handled = true;
            }
            if (e.Key == Key.F && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                if (Visibility == Visibility.Visible)
                {
                    if (_edtSearchText.IsFocused)
                    {
                        close();
                    }
                    else
                    {
                        Keyboard.Focus(_edtSearchText);
                        bool success = _edtSearchText.Focus();
                        Debug.Assert(success);
                    }
                }
                else
                {
                    Visibility = Visibility.Visible;
                    startSearch();
                    gotoFirst();

                    //http://stackoverflow.com/questions/3109080/focus-on-textbox-when-usercontrol-change-visibility
                    Dispatcher.BeginInvoke((Action)delegate
                    {
                        Keyboard.Focus(_edtSearchText);
                        bool success = _edtSearchText.Focus();
                        Debug.Assert(success);
                    }, DispatcherPriority.Render);
                }

                e.Handled = true;
            }
        }

        private void close()
        {
            Visibility = Visibility.Collapsed;
            _marker.ClearMarks();
            _editor.Focus();
        }

        private void startSearch()
        {
            _marker.ClearMarks();
            _foundOffsets.Clear();

            string word = _edtSearchText.Text;
            if (string.IsNullOrEmpty(word))
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

                _foundOffsets.Add(idx);
                _marker.AddOffsetToMark(new MarkBackgroundRenderer.Mark
                {
                    Offset = idx,
                    Length = word.Length,
                    Brush = new SolidColorBrush(Colors.Yellow)
                });
            }
        }

        private void _edtSearchText_TextChanged(object sender, TextChangedEventArgs e)
        {
            startSearch();
            gotoFirst();
        }

        private void scrollToOffset(int offset, int length)
        {
            _editor.CaretOffset = offset;
            _editor.TextArea.Caret.Offset = offset;
            _editor.Select(offset, length);

            var line = _editor.Document.GetLineByOffset(offset);
            int lineNumber = line.LineNumber;
            _editor.ScrollToLine(lineNumber);
        }

        private void _edtSearchText_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.F3)
            {
                gotoNext();
                e.Handled = true;
            }
            else if (e.Key == Key.Escape)
            {
                close();
                e.Handled = true;
            }
        }

        private void gotoFirst()
        {
            if (_foundOffsets.Count > 0)
            {
                int idx = _foundOffsets[0];
                scrollToOffset(idx, _edtSearchText.Text.Length);
            }
        }

        private void gotoNext()
        {
            if (_foundOffsets.Count > 0)
            {
                int caretAt = _editor.CaretOffset;

                if (_foundOffsets.Count == 1)
                {
                    int idx = _foundOffsets[0];
                    scrollToOffset(idx, _edtSearchText.Text.Length);
                }
                else
                {
                    if (_foundOffsets[_foundOffsets.Count - 1] < caretAt) //Cursor steht hinter der letzten Fundstelle -> zu erster Fundstelle springen
                    {
                        int idx = _foundOffsets[0];
                        scrollToOffset(idx, _edtSearchText.Text.Length);
                    }
                    else if (_foundOffsets[0] > caretAt) //Cursor steht vor der ersten Fundstelle -> zu erster Fundstelle springen
                    {
                        int idx = _foundOffsets[0];
                        scrollToOffset(idx, _edtSearchText.Text.Length);
                    }
                    else
                    {
                        for (int i = 1; i < _foundOffsets.Count; i++)
                        {
                            Debug.Assert(_foundOffsets[i] > _foundOffsets[i-1]);

                            if (_foundOffsets[i] > caretAt && _foundOffsets[i - 1] <= caretAt)
                            {
                                int idx = _foundOffsets[i];
                                scrollToOffset(idx, _edtSearchText.Text.Length);
                                return;
                            }
                        }                    
                    }
                }
            }
        }

        private void btnClose_Click(object sender, RoutedEventArgs e)
        {
            close();
        }

        private void btnFindNext_Click(object sender, RoutedEventArgs e)
        {
            gotoNext();
        }
    }
}
