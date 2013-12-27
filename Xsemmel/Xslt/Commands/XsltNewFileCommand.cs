using System;
using System.IO;
using System.Windows;
using Microsoft.Win32;

namespace XSemmel.Xslt.Commands
{
    class XsltNewFileCommand : AbstractXsltCommand
    {

        protected override void SetResult(string result)
        {
            SaveFileDialog dlgSaveFile = new SaveFileDialog();
            dlgSaveFile.Filter = "All Files|*.*";
            dlgSaveFile.Title = "Select a file to save";

            if (dlgSaveFile.ShowDialog() == true)
            {
                try
                {
                    File.WriteAllText(dlgSaveFile.FileName, result);
                    MessageBox.Show(Application.Current.MainWindow, "Transformation done, file saved.", "Information", 
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Application.Current.MainWindow, ex.Message, "Error", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }

        }

    }
}
