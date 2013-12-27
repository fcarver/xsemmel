using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;

namespace XSemmel.Editor
{
    class BracketMatcher
    {
        private readonly TextEditor _editor;
        private readonly MarkBackgroundRenderer _marker;

        private readonly SolidColorBrush CYAN = new SolidColorBrush(Color.FromArgb(128, 0x00, 0xFF, 0xFF));

        public BracketMatcher(TextEditor editor)
        {
            _editor = editor;

            _marker = new MarkBackgroundRenderer(editor);
            _editor.TextArea.TextView.BackgroundRenderers.Add(_marker);

            _editor.TextArea.Caret.PositionChanged += OnCaretOnPositionChanged;
        }

        private void OnCaretOnPositionChanged(object x, EventArgs y)
        {
            _marker.ClearMarks();
            int offset = _editor.CaretOffset;
            if (offset >= 0 && offset < _editor.Text.Length)
            {
                char c = _editor.Text[offset];
                if (_supportedBrackets.Contains(c))
                {
                    try
                    {
                        int matching = findMatchingBrackets(_editor.Text, c, offset);
                        if (matching >= 0)
                        {
                            _marker.AddOffsetToMark(new MarkBackgroundRenderer.Mark
                            {
                                Offset = offset, Length = 1, Brush = CYAN
                            });
                            _marker.AddOffsetToMark(new MarkBackgroundRenderer.Mark
                            {
                                Offset = matching, Length = 1, Brush = CYAN
                            });
                        }
                    }
                    catch (Exception e)
                    {
                        Debug.Fail(e.Message);
                    }
                }
            }
        }

        private readonly HashSet<char> _supportedBrackets = new HashSet<char> {'(', ')', '[', ']', '<', '>', '{', '}'};

        private readonly char[][] _bracketTypes = new[]
            {
                new[] { '(', ')' },
                new[] { '[', ']' },
                new[] { '{', '}' },
                new[] { '<', '>' },
            };

        private int findMatchingBrackets(string s, char bracketAtOffset, int offset)
        {
            const int MAX_SEARCH_RADIUS = 1000;
            bool isOpeningBracket = false;
            char searchFor = ' ';

            int bracketCount = 0;
            char bracket = bracketAtOffset;
            Debug.Assert(bracketAtOffset == s[offset]);

            foreach (char[] keyValuePair in _bracketTypes)
            {
                if (keyValuePair[0] == bracket)
                {
                    isOpeningBracket = true;
                    searchFor = keyValuePair[1];
                    break;
                }
                else if (keyValuePair[1] == bracket)
                {
                    isOpeningBracket = false;
                    searchFor = keyValuePair[0];
                    break;
                }
            }
            if (searchFor == ' ')
            {
                return -1;
            }

            if (isOpeningBracket)
            {
                if (offset < s.Length - 1)
                {
                    int end = Math.Min(s.Length, offset + MAX_SEARCH_RADIUS);
                    for (int i = offset + 1; i < end; i++)
                    {
                        if (s[i] == bracket)
                        {
                            bracketCount++;
                        }
                        else if (s[i] == searchFor)
                        {
                            bracketCount--;
                        }
                        if (bracketCount < 0)
                        {
                            return i;
                        }
                    }
                }
            }
            else
            {
                if (offset > 0)
                {
                    int end = Math.Max(0, offset - MAX_SEARCH_RADIUS);
                    for (int i = offset - 1; i >= end; i--)
                    {
                        if (s[i] == bracket)
                        {
                            bracketCount++;
                        }
                        else if (s[i] == searchFor)
                        {
                            bracketCount--;
                        }
                        if (bracketCount < 0)
                        {
                            return i;
                        }
                    }
                }
            }


            return -1;
        }

    }
}
