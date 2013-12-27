using System;
using ICSharpCode.AvalonEdit;
using System.Windows;
using XSemmel.Editor;

namespace XSemmel.Commands
{
    class StartXQueryCommand :  XSemmelCommand
    {
        protected override bool CanExecute(EditorFrame ef)
        {
            if (ef.XmlEditor == null)
            {
                return false;
            }
            return true;
        }

        protected override void Execute(EditorFrame ef)
        {
            TextEditor textEditor = ef.XmlEditor;

            if (textEditor == null)
            {
                throw new Exception("Parameter must be TextEditor");
            }

            new XQuery.XQuery(ef.XSDocument) { Owner = Application.Current.MainWindow }.ShowDialog();
        }

    }
}
