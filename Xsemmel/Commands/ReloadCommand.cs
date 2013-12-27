using System.Diagnostics;
using System.Windows;
using XSemmel.Editor;

namespace XSemmel.Commands
{
    class ReloadCommand :  XSemmelCommand
    {
        protected override bool CanExecute(EditorFrame ef)
        {
            if (ef.XSDocument == null)
            {
                return false;
            }
            if (ef.XSDocument.Filename == null)
            {
                return false;
            }
            return true;
        }


        protected override void Execute(EditorFrame ef)
        {
            MainWindow mainWnd = (MainWindow) Application.Current.MainWindow;
            mainWnd.OpenFile(ef.XSDocument.Filename);
        }

    }
}
