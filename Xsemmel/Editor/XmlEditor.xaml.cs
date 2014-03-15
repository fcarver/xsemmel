using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using System.Xml.Schema;
using ICSharpCode.AvalonEdit.Folding;
using ICSharpCode.AvalonEdit.Indentation;
using ICSharpCode.AvalonEdit;
using XSemmel.Commands;
using XSemmel.Configuration;

namespace XSemmel.Editor
{

    public partial class XmlEditor
    {

        private EditorFrame _editorFrame;

        private readonly CodeCompletion _codeCompletion;
        private readonly GotoLine _gotoLine;
        private readonly ToolTip _toolTip = new ToolTip();

        public XmlEditor()
        {
            InitializeComponent();

            _textEditor.FontSize = XSConfiguration.Instance.Config.FontSize;

            DispatcherTimer foldingUpdateTimer = new DispatcherTimer();
            foldingUpdateTimer.Interval = TimeSpan.FromSeconds(2);
            foldingUpdateTimer.Tick += foldingUpdateTimer_Tick;
            foldingUpdateTimer.Start();

            _foldingManager = FoldingManager.Install(_textEditor.TextArea);

            _codeCompletion = new CodeCompletion(_textEditor);
            _presenterFindString.Content = new FindString(_textEditor);
            new Commenter(_textEditor);
            new BracketMatcher(_textEditor);
            new HighlightOccurrences(_textEditor);
            new HighlightCurrentLineBackgroundRenderer(_textEditor);
            _gotoLine = new GotoLine(_textEditor);
            new ShortcutNavigator(_textEditor);
            
            _textEditor.TextArea.Caret.PositionChanged += textEditor_CursorPositionChanged;
            _textEditor.TextArea.KeyDown += textEditor_TextArea_KeyDown;
        }

        private void textEditor_TextArea_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.S && (Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                SaveCommand.SaveWithoutQuestion(_editorFrame);
                e.Handled = true;
            }
        }

        public EditorFrame Editor
        {
            set
            {
                if (_editorFrame != null)
                {
                    throw new Exception("XSDocument already set");
                }
                _editorFrame = value;

                Open(_editorFrame.XSDocument);
                _textEditor.IsModified = false;
            }
        }

        public TextEditor TextEditor
        {
            get
            {
                return _textEditor;
            }
        }

        public void Open(XSDocument xml)
        {
            _textEditor.Text = xml.Xml;
            installFoldingManager();

            try
            {
//                XmlDocument xmldoc = xml.XmlDoc;  //throws exception if it is not wellformed or invalid

                string xsd = xml.XsdFile;
                if (xsd != null)
                {
                    SetXsdFile(xsd);
                }
            }
            catch (XmlException)
            {
                //ignore
            }
//            catch (XmlSchemaValidationException)
//            {
//                //ignore
//            }
        }


        internal void SetXsdFile(string xsdFile)
        {
            _lblXsdFile.ToolTip = xsdFile;
            try
            {
                _codeCompletion.SetXsdFile(xsdFile);
                _lblXsdFile.Text = xsdFile;

            } 
            catch (Exception ex)
            {
                _codeCompletion.SetXsdFile(null);
                _lblXsdFile.Text = "Invalid xsd: " + ex.Message;
            }
        }

        private void textEditor_CursorPositionChanged(object sender, EventArgs e)
        {
            var loc = _textEditor.Document.GetLocation(_textEditor.CaretOffset);
            _lblCurrentColumn.Text = "Col " + loc.Column;
            _lblCurrentLine.Text = "Ln " + loc.Line;
        }

        #region Folding
        private readonly FoldingManager _foldingManager;
        private AbstractFoldingStrategy _foldingStrategy;

        void installFoldingManager()
        {
            _foldingStrategy = new XmlFoldingStrategy();
            _textEditor.TextArea.IndentationStrategy = new DefaultIndentationStrategy();
            
            _foldingStrategy.UpdateFoldings(_foldingManager, _textEditor.Document);
        }

        void foldingUpdateTimer_Tick(object sender, EventArgs e)
        {
            if (_foldingStrategy != null)
            {
                _foldingStrategy.UpdateFoldings(_foldingManager, _textEditor.Document);
            }
        }

        private void textEditor_MouseHover(object sender, MouseEventArgs e)
        {
            if (_foldingManager == null)
            {
                return;
            }

            var pos = _textEditor.GetPositionFromPoint(e.GetPosition(_textEditor));
            if (pos != null)
            {
                int off = _textEditor.Document.GetOffset(pos.Value.Line, pos.Value.Column);
                foreach (var fld in _foldingManager.AllFoldings)
                {
                    if (fld.StartOffset <= off && off <= fld.EndOffset && fld.IsFolded)
                    {
                        _toolTip.PlacementTarget = this;
                        _toolTip.Content = _textEditor.Document.Text.Substring(
                            fld.StartOffset, fld.EndOffset - fld.StartOffset);
                        _toolTip.IsOpen = true;
                        e.Handled = true;
                    }
                }
            }
        }

        private void textEditor_MouseHoverStopped(object sender, MouseEventArgs e)
        {
            _toolTip.IsOpen = false;
        }


        private void mnuCollapseAll_Click(object sender, RoutedEventArgs e)
        {
            if (_foldingManager == null || _foldingStrategy == null)
            {
                return;
            }
            bool first = true;
            foreach (FoldingSection fold in _foldingManager.AllFoldings)
            {
                if (!first)
                {
                    fold.IsFolded = true;
                }
                first = false;
            }
            _foldingStrategy.UpdateFoldings(_foldingManager, _textEditor.Document);
        }

        private void mnuExpandAll_Click(object sender, RoutedEventArgs e)
        {
            if (_foldingManager == null || _foldingStrategy == null)
            {
                return;
            }
            foreach (FoldingSection fold in _foldingManager.AllFoldings)
            {
                fold.IsFolded = false;
            }
            _foldingStrategy.UpdateFoldings(_foldingManager, _textEditor.Document);
        }


        #endregion

        private void _textEditor_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var position = _textEditor.GetPositionFromPoint(e.GetPosition(_textEditor));
            if (position.HasValue)
            {
                _textEditor.TextArea.Caret.Position = position.Value;
            }
        }

        private void mnuSelectionAsFragment_Click(object sender, RoutedEventArgs e)
        {
            _editorFrame.SetFragmentText(_textEditor.SelectedText, true);
        }

        private void UIElement_OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ClickCount == 2)
            {
                _gotoLine.ShowDialog();    
            }
        }

    }
}
