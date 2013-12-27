using XSemmel.Editor;
using XSemmel.Helpers;

namespace XSemmel.Commands
{
    class EscapeXmlStringCommand : XSemmelCommand
    {
        protected override bool CanExecute(EditorFrame ef)
        {
            if (ef.XmlEditor == null)
            {
                return false;
            }
            if (!ef.XmlEditor.TextArea.IsFocused)
            {
                return false;
            }
            return true;
        }

        protected override void Execute(EditorFrame ef)
        {
            if (ef.XmlEditor.SelectionLength != 0)
            {
                ef.XmlEditor.SelectedText = XmlEscape.EscapeXml(ef.XmlEditor.SelectedText);
            }
            else
            {
                ef.XmlEditor.Text = XmlEscape.EscapeXml(ef.XmlEditor.Text);
            }
        }

        

    }
}
