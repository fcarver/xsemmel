using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using System.Xml;
using System;
using XSemmel.Editor;
using System.Diagnostics;

namespace XSemmel.TreeView
{
    public partial class EditorTree
    {

        private EditorFrame _editor;

        private readonly DispatcherTimer _updateTimer;

        private volatile bool _updateNeccessary;
        private volatile bool _updateSelectedBasedOnCursorNeccessary;

        private bool _updateAutomatically = true;


        public EditorTree()
        {
            InitializeComponent();

            tree.SelectionChanged += tree_SelectionChanged;
            tree.XmlChanged += tree_XmlChanged;

            _updateTimer = new DispatcherTimer {Interval = new TimeSpan(0, 0, 2)};
            _updateTimer.Tick += (x, y) => {
                if (UpdateAutomatically)
                {
                    _updateTimer.Stop();
                    try
                    {
                        if (_updateNeccessary)
                        {
                            Update();
                        }
                        if (_updateSelectedBasedOnCursorNeccessary)
                        {
                            selectNodeBasedOnCursor();
                        }                        
                    }
                    catch (Exception e)
                    {
                        Debug.Fail(e.Message);
                        //TODO
                    }
                    finally
                    {
                        _updateTimer.Start();                        
                    }
                }
            };
        }

        public bool UpdateAutomatically
        {
            get { return _updateAutomatically; }
            set { _updateAutomatically = value; }
        }

        public EditorFrame Editor
        {
            set
            {
                if (_editor != null)
                {
                    throw new Exception("Editor already set");
                }
                _editor = value;
                _editor.XmlEditor.TextChanged += (a, b) => _updateNeccessary = true;
                _editor.XmlEditor.TextArea.Caret.PositionChanged +=
                    (a, b) => _updateSelectedBasedOnCursorNeccessary = true;

                _updateNeccessary = true;

                _updateTimer.Start();
                if (UpdateAutomatically)
                {
                    Update();    
                }
            }
        }


        private void selectNodeBasedOnCursor()
        {
            _updateSelectedBasedOnCursorNeccessary = false;

            try
            {
                Cursor = Cursors.Wait;
                var loc = _editor.XmlEditor.TextArea.Caret.Location;
                TreeViewItem selectedItem = tree.SelectNodeBasedOnCursor(loc);
                if (selectedItem != null)
                {
                    ViewerNode node = (ViewerNode)selectedItem.Tag;
                    _editor.SetSchemaInfo(node.SchemaInfo);
                }
            }
            finally
            {
                Cursor = null;
            }
        }

        private void highlightFragment(ViewerNode selectedNode)
        {
            if (_editor == null)
            {
                return;
            }

            //show fragment in fragment view
            if (selectedNode == null)
            {
                _editor.SetFragmentText("");
            }
            else
            {
                StringBuilder sb = new StringBuilder();

                XmlWriterSettings settings = new XmlWriterSettings
                                                 {
                                                     ConformanceLevel = ConformanceLevel.Auto,
                                                     Indent = true
                                                 };

                using (XmlWriter w = XmlWriter.Create(sb, settings))
                {
                    selectedNode.OriginalNode.WriteTo(w);
                }

                _editor.SetFragmentText(sb.ToString());
            }
            
            //select node in editor
            if (selectedNode != null)
            {
                IXmlLineInfo lineInfo = selectedNode.LineInfo;
                if (lineInfo != null)
                {
                    var offset = _editor.XmlEditor.Document.GetOffset(lineInfo.LineNumber, lineInfo.LinePosition);
                    _editor.XmlEditor.Select(offset, selectedNode.Name.Length);
                    Debug.Assert(_editor.XmlEditor.SelectedText == selectedNode.Name);
                    _editor.XmlEditor.ScrollTo(lineInfo.LineNumber, lineInfo.LinePosition);
                }
            }
        }

        public void Update()
        {
            if (_editor != null)
            {
                tree.Update(new XSDocument(_editor.XmlEditor.Text, _editor.XSDocument.XsdFile));
                _updateNeccessary = false;
            }
        }


        void tree_SelectionChanged(ViewerNode obj)
        {
            if (obj == null)
            {
                return;
            }

            highlightFragment(obj);
        }

        private void btnApplyFilter_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(_edtFilter.Text))
            {
                tree.Filter = _edtFilter.Text;
            }
            else
            {
                tree.Filter = null;
            }
        }

        private void btnRemoveFilter_Click(object sender, RoutedEventArgs e)
        {
            _edtFilter.Text = "";
            btnApplyFilter_Click(sender, e);
        }

        private void edtFilter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.Enter)
            {
                btnApplyFilter_Click(sender, e);
                e.Handled = true;
            }
        }

        void tree_XmlChanged(string obj)
        {
            _editor.XmlEditor.Text = obj;
        }

    }
}
