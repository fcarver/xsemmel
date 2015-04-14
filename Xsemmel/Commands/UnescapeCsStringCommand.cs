using System;
using System.Text.RegularExpressions;
using XSemmel.Editor;

namespace XSemmel.Commands
{
    class UnescapeCsStringCommand : XSemmelCommand
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
            if (ef.XSDocument == null)
            {
                throw new Exception("XSDocument is null");
            }

            if (ef.XmlEditor.SelectionLength != 0)
            {
                ef.XmlEditor.SelectedText = Regex.Unescape(ef.XmlEditor.SelectedText);
            }
            else
            {
                ef.XmlEditor.Text = Regex.Unescape(ef.XmlEditor.Text);   
            }
        }

    }
}
