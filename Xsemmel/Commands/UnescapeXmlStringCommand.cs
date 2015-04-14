using System;
using System.Security;
using XSemmel.Editor;
using XSemmel.Helpers;

namespace XSemmel.Commands
{
    class UnescapeXmlStringCommand : XSemmelCommand
    {
        protected override bool CanExecute(EditorFrame ef)
        {
            if (ef.XmlEditor == null)
            {
                return false;
            }
//            if (!ef.XmlEditor.TextArea.IsFocused)
//            {
//                return false;
//            }
            return true;
        }

        protected override void Execute(EditorFrame ef)
        {
            if (ef.XmlEditor.SelectionLength != 0)
            {
                ef.XmlEditor.SelectedText = XmlEscape.UnescapeXml(ef.XmlEditor.SelectedText);
            }
            else
            {
                ef.XmlEditor.Text = XmlEscape.UnescapeXml(ef.XmlEditor.Text);   
            }
        }

        

    }
}
