using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;

namespace XSemmel.Editor
{

    public class MarkBackgroundRenderer : IBackgroundRenderer
    {

        public class Mark
        {
            public int Offset { get; set; }
            public int Length { get; set; }
            public Brush Brush { get; set; }
        }

        private readonly TextEditor _editor;
        private readonly IList<Mark> _offsetsToMark = new List<Mark>();

        public MarkBackgroundRenderer(TextEditor editor)
        {
            _editor = editor;
        }


        public KnownLayer Layer
        {
            get { return KnownLayer.Background; }
        }

        public int MarkCount
        {
            get { return _offsetsToMark.Count; }
        }

        public void AddOffsetToMark(Mark offset)
        {
            _offsetsToMark.Add(offset);
            _editor.TextArea.TextView.InvalidateLayer(KnownLayer.Background);
        }

        public void ClearMarks()
        {
            _offsetsToMark.Clear();
            _editor.TextArea.TextView.InvalidateLayer(KnownLayer.Background);
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (_editor.Document == null || _offsetsToMark.Count == 0)
            {
                return;
            }

            textView.EnsureVisualLines();

            foreach (var mark in _offsetsToMark)
            {
                foreach (Rect r in BackgroundGeometryBuilder.GetRectsForSegment(
                    textView, new TextSegment { StartOffset = mark.Offset, Length = mark.Length }))
                {
                    drawingContext.DrawRoundedRectangle(
                        mark.Brush,
                        null,
                        new Rect(r.Location, r.Size),
                        3, 3
                    );
                }
            }

            //http://danielgrunwald.de/coding/AvalonEdit/rendering.php
            //http://stackoverflow.com/questions/5072761/avalonedit-highlight-current-line-even-when-not-focused
            //DocumentLine currentLine = _editor.Document.GetLineByOffset((int)_offsetToMark);
            //foreach (Rect rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, currentLine))
            //{
            //    drawingContext.DrawRectangle(
            //        new SolidColorBrush(Color.FromArgb(0x40, 0, 0, 0xFF)), null,
            //        new Rect(rect.Location, new Size(textView.ActualWidth - 32, rect.Height)));
            //}
        }
    }
}
