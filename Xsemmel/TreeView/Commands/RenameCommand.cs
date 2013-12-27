using System.Windows;
using System.Windows.Controls;
using System.Xml;
using XSemmel.Editor;
using XSemmel.Helpers;
using XSemmel.Helpers.WPF;

namespace XSemmel.TreeView.Commands
{
    class RenameCommand : TreeViewItemCommand
    {

        protected override void Execute(EditorFrame ef, TreeViewItem parameter, ViewerNode selectedNode, XmlDocument xmldoc, XmlNode node, XmlNode parentNode)
        {
            string newName = InputBox.Show(Application.Current.MainWindow, "Rename", "New name of the node", node.Name);
            if (newName != null)
            {
                RenameNode(xmldoc, node, node.NamespaceURI, newName);
                ef.XmlEditor.Text = xmldoc.ToUTF8String();
            }
        }


        public static XmlNode RenameNode(XmlDocument xmldoc, XmlNode node, string namespaceURI, string qualifiedName)
        {
            if (node.NodeType == XmlNodeType.Element)
            {
                XmlElement oldElement = (XmlElement)node;
                XmlElement newElement = xmldoc.CreateElement(qualifiedName, namespaceURI);

                while (oldElement.HasAttributes)
                {
                    newElement.SetAttributeNode(oldElement.RemoveAttributeNode(oldElement.Attributes[0]));
                }

                while (oldElement.HasChildNodes)
                {
                    newElement.AppendChild(oldElement.FirstChild);
                }

                if (oldElement.ParentNode != null)
                {
                    oldElement.ParentNode.ReplaceChild(newElement, oldElement);
                }

                return newElement;
            }
            else
            {
                return null;
            }
        }

    }
}
