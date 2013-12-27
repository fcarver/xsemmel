using System;
using System.Threading;
using System.Windows;
using XSemmel.Helpers;

namespace XSemmel.Xslt.Commands
{
    class XsltNewXSemmelCommand : AbstractXsltCommand
    {

        protected override void SetResult(string result)
        {
            string pipename = NamedPipeHelper.StartNewListeningXSemmel();
            new Thread(text =>
            {
                try
                {
                    NamedPipeHelper.Write(pipename, text.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Application.Current.MainWindow, ex.Message, "Error", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }) { IsBackground = true }.Start(result);
        }

    }
}
