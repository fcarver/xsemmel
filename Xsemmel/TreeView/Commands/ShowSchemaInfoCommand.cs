using System;
using System.Windows.Controls;
using System.Xml;
using XSemmel.Commands;
using XSemmel.Editor;

namespace XSemmel.TreeView.Commands
{
    class ShowSchemaInfoCommand : XSemmelCommand
    {
        protected override void Execute(EditorFrame ef)
        {
            TreeViewItem selected = ((TreeViewItem)ef._editorTree.tree.SelectedItem);
            ViewerNode node = (ViewerNode)selected.Tag;
            ef.SetSchemaInfo(node.SchemaInfo);
        }

        protected override bool CanExecute(EditorFrame ef)
        {
            if (ef.XmlEditor == null)
            {
                return false;
            }
            if (ef.XSDocument.XsdFile == null)
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

            return true;
        }

    }
}
