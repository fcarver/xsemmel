using System;
using System.Windows;
using XSemmel.Editor;

namespace XSemmel.Commands
{
    class CopyFilenameCommand :  XSemmelCommand
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
            if (ef.XSDocument == null)
            {
                return false;
            }
            return ef.XSDocument.Filename != null;
        }

        protected override void Execute(EditorFrame ef)
        {
            if (ef.XSDocument == null)
            {
                throw new Exception("Parameter must be TextEditor");
            }

            Clipboard.SetText(ef.XSDocument.Filename);
        }

    }
}
