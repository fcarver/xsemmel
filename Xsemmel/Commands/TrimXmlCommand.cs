using System;
using System.Windows;
using XSemmel.Editor;
using XSemmel.Helpers;
using XSemmel.PrettyPrinter;

namespace XSemmel.Commands
{
    class TrimXmlCommand : XSemmelCommand
    {
        protected override bool CanExecute(EditorFrame ef)
        {
            return ef.XmlEditor != null;
        }

        protected override void Execute(EditorFrame ef)
        {
            try
            {
                string xmlToProcess;

                try
                {
                    ef.XmlEditor.Text.ToXmlDocument();  //we are interested if this throws an exception, nothing else
                    xmlToProcess = ef.XmlEditor.Text;
                    //MessageBox.Show(Application.Current.MainWindow, "Document is well-formed. No need to trim.",
                    //                "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch
                {
                    xmlToProcess = XParser.Trim(ef.XmlEditor.Text);
                }

                try
                {
                    ef.XmlEditor.Text = PrettyPrint.Execute(
                        xmlToProcess,
                        ef.Data.PrettyPrintData.Indent,
                        ef.Data.PrettyPrintData.NewLineOnAttributes
                    );
                }
                catch
                {
                    ef.XmlEditor.Text = xmlToProcess;
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(Application.Current.MainWindow, "Error: " + e.Message, "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

    }
}
