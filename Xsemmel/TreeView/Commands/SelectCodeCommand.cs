using System;
using System.Windows.Controls;
using System.Xml;
using XSemmel.Commands;
using XSemmel.Editor;

namespace XSemmel.TreeView.Commands
{
    class SelectCodeCommand : XSemmelCommand
    {
        protected override void Execute(EditorFrame ef)
        {
            TreeViewItem selected = ((TreeViewItem)ef._editorTree.tree.SelectedItem);
            ViewerNode node = (ViewerNode)selected.Tag;
            IXmlLineInfo lineInfo = node.LineInfo;
            if (lineInfo != null)
            {
                int lineNum = lineInfo.LineNumber;
                int linePos = lineInfo.LinePosition;

                string outerXml = node.OriginalNode.OuterXml;

                int offset = ef.XmlEditor.Document.GetOffset(lineNum, linePos);
                offset -= 1;

                outerXml = outerXml.Replace("\n", "\r\n");

                int length = Math.Min(outerXml.Length, ef.XmlEditor.Text.Length);

                ef.XmlEditor.Select(offset, length);
                ef.XmlEditor.Focus();
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
                //info: there are no correct Linepositions for attributes
                return false;
            }

            return true;
        }

    }
}
