using System.Windows;
using System.Windows.Controls;
using System.Xml;
using XSemmel.Commands;
using XSemmel.Editor;

namespace XSemmel.TreeView.Commands
{
    class CopyXPathCommand : XSemmelCommand
    {
        protected override void Execute(EditorFrame ef)
        {
            TreeViewItem selected = ((TreeViewItem)ef._editorTree.tree.SelectedItem);
            if (selected == null)
            {
                return;
            }

            if (selected.Tag != null)
            {
                ViewerNode selectedNode = (ViewerNode)selected.Tag;
                Clipboard.SetText(selectedNode.XPath);
            }
        }

        protected override bool CanExecute(EditorFrame ef)
        {
            if (ef.XmlEditor == null)
            {
                return false;
            }

            TreeViewItem selected = ((TreeViewItem)ef._editorTree.tree.SelectedItem);
            if (selected == null)
            {
                return false;
            }
            if (selected.Tag == null)
            {
                return false;
            }

            ViewerNode selectedNode = (ViewerNode)selected.Tag;
            XmlNode node = selectedNode.OriginalNode;
            if (node == null)
            {
                return false;
            }
            if (node.OwnerDocument == null)
            {
                return false;
            }
            if (node.ParentNode == null)
            {
                return false;
            }

            return true;
        }

    }
}
