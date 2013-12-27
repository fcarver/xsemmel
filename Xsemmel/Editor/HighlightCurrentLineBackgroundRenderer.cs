using System.Windows;
using System.Windows.Media;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Rendering;
using XSemmel.Helpers;

namespace XSemmel.Editor
{
    public class HighlightCurrentLineBackgroundRenderer : IBackgroundRenderer
    {
        private readonly TextEditor _editor;
        private readonly Brush _brush;

        public HighlightCurrentLineBackgroundRenderer(TextEditor editor)
        {
            var color = ColorHelper.Create(30, Colors.Gold);
            _brush = new SolidColorBrush(color);

            _editor = editor;
            _editor.TextArea.TextView.BackgroundRenderers.Add(this);
            _editor.TextArea.Caret.PositionChanged += (sender, e) =>
                _editor.TextArea.TextView.InvalidateLayer(KnownLayer.Background);
        }

        public KnownLayer Layer
        {
            get { return KnownLayer.Caret; }
        }

        public void Draw(TextView textView, DrawingContext drawingContext)
        {
            if (_editor.Document == null)
                return;
            
            textView.EnsureVisualLines();
            DocumentLine currentLine = _editor.Document.GetLineByOffset(_editor.CaretOffset);

            //do not call GetRectsForSegment for long lines; performance is terrible
            if (currentLine.Length < 1000)
            {
                foreach (var rect in BackgroundGeometryBuilder.GetRectsForSegment(textView, currentLine))
                {
                    drawingContext.DrawRectangle(
                        _brush, null, new Rect(rect.Location, new Size(textView.ActualWidth, rect.Height)));
                }
            }
        }

    }
}
