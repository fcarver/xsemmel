using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using XSemmel.Commands;
using XSemmel.Editor;

namespace XSemmel.TreeView.Commands
{
    abstract class TreeViewItemCommand : XSemmelCommand
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
            if (node.ParentNode == null)
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
                    Debug.Assert(node.ParentNode != null, "Checked in CanExecute");
                    
                    Execute(ef, selected, selectedNode, xmldoc, node, node.ParentNode);
                }
            } 
            catch (Exception e)
            {
                MessageBox.Show(Application.Current.MainWindow, "Error: " + e.Message, "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        protected abstract void Execute(EditorFrame ef, TreeViewItem treeViewItem, ViewerNode selectedNode, XmlDocument xmldoc, XmlNode node, XmlNode parentNode);

    }
}
