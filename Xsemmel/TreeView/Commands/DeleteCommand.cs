using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using XSemmel.Commands;
using XSemmel.Editor;
using XSemmel.Helpers;

namespace XSemmel.TreeView.Commands
{
    class DeleteCommand : XSemmelCommand
    {

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

            return true;
        }

        protected override void Execute(EditorFrame ef)
        {
            try
            {
                TreeViewItem selected = ((TreeViewItem)ef._editorTree.tree.SelectedItem);

                if (selected.Tag != null)
                {
                    ViewerNode selectedNode = (ViewerNode)selected.Tag;
                    XmlDocument xmldoc = selectedNode.OriginalNode.OwnerDocument;
                    XmlNode node = selectedNode.OriginalNode;

                    Debug.Assert(xmldoc != null, "Checked in CanExecute");

                    Execute(ef, xmldoc, node);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(Application.Current.MainWindow, "Error: " + e.Message, "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        private void Execute(EditorFrame ef, XmlDocument xmldoc, XmlNode node)
        {
            if (node.NodeType == XmlNodeType.Element)
            {
                XmlElement oldElement = (XmlElement)node;

                if (oldElement.ParentNode != null)
                {
                    oldElement.ParentNode.RemoveChild(oldElement);
                }
            }
            else if (node.NodeType == XmlNodeType.Attribute)
            {
                XmlAttribute attr = (XmlAttribute)node;

                XmlNode owner = attr.OwnerElement;
                if (owner != null && owner.Attributes != null)
                {
                    owner.Attributes.Remove(attr);
                }
            }
            else
            {
                return;
            }
            ef.XmlEditor.Text = xmldoc.ToUTF8String();
        }


    }
}
