using System.Windows.Controls;
using System.Xml;
using XSemmel.Editor;
using XSemmel.Helpers;

namespace XSemmel.TreeView.Commands
{
    class CommentOutCommand : TreeViewItemCommand
    {

        protected override void Execute(EditorFrame ef, TreeViewItem parameter, ViewerNode selectedNode, XmlDocument xmldoc, XmlNode node, XmlNode parentNode)
        {
            var comment = xmldoc.CreateComment(node.OuterXml);
            parentNode.ReplaceChild(comment, node);
            ef.XmlEditor.Text = xmldoc.ToUTF8String();
        }
    }
}
