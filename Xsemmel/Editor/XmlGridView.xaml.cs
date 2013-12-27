using System;
using System.Windows;
using System.Xml;
using WmHelp.XmlGrid;
using XSemmel.Helpers;
using ContextMenu = System.Windows.Forms.ContextMenu;
using MenuItem = System.Windows.Forms.MenuItem;

namespace XSemmel.Editor
{

    public partial class XmlGridView
    {
        private EditorFrame _editor;

        public XmlGridView()
        {
            InitializeComponent();

            _xmlGridView.ContextMenu = new ContextMenu(
                new[]
                    {
                        new MenuItem("Refresh", (sender, evt) => load()),
                        new MenuItem("Expand all", (sender, evt) => _xmlGridView.FullExpand()),
                    });
        }


        private void load()
        {
            try
            {
                GridBuilder builder = new GridBuilder();
                GridCellGroup root = new GridCellGroup();

                //XmlDocument xmldoc = _editor.XmlEditor.Text.ToXmlDocument();
                XmlDocument xmldoc = new XmlDocument();
                //do this to remove #whitespace nodes
                xmldoc.LoadXml(_editor.XmlEditor.Text.ToXmlDocument().OuterXml);

                builder.ParseNodes(root, null, xmldoc.ChildNodes);
                _xmlGridView.Cell = root;

                _lblErrors.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                _xmlGridView.Cell = null;

                _lblErrors.Content = "Errors in XML document: " + ex.Message;
                _lblErrors.Visibility = Visibility.Visible;
            }
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
            }
        }

        private void UserControl_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_editor != null && _editor.XmlEditor != null)
            {
                if (true.Equals(e.NewValue))
                {
                    load();
                    _editor.XmlEditor.TextChanged += editor_TextChanged;
                }
                else
                {
                    _editor.XmlEditor.TextChanged -= editor_TextChanged;
                }
            }
        }

        private void editor_TextChanged(object sender, EventArgs e)
        {
            load();
        }
    }
}
