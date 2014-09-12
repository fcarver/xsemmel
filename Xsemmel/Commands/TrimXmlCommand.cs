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
                ef.XmlEditor.Text.ToXmlDocument();
                MessageBox.Show(Application.Current.MainWindow, "Document is well-formed. No need to trim.",
                                "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            catch
            {
                try
                {
                    var trimmed = XParser.Trim(ef.XmlEditor.Text);

                    try
                    {
                        ef.XmlEditor.Text = PrettyPrint.Execute(
                            trimmed,
                            ef.Data.PrettyPrintData.Indent,
                            ef.Data.PrettyPrintData.NewLineOnAttributes
                        );
                    }
                    catch
                    {
                        ef.XmlEditor.Text = trimmed;
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
}
