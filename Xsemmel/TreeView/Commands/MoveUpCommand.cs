using System.Windows.Controls;
using System.Xml;
using XSemmel.Editor;
using XSemmel.Helpers;

namespace XSemmel.TreeView.Commands
{
    class MoveUpCommand : TreeViewItemCommand
    {

        protected override void Execute(EditorFrame ef, TreeViewItem parameter, ViewerNode selectedNode, XmlDocument xmldoc, XmlNode node, XmlNode parentNode)
        {
            ef.XmlEditor.Text = NodeMover.MoveUp(node);
            ef._editorTree.Update();
            ef._editorTree.tree.ExpandAll();
        }

    }
}
