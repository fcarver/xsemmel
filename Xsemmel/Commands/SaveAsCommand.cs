using System;
using System.Windows;
using Microsoft.Win32;
using XSemmel.Editor;

namespace XSemmel.Commands
{
    class SaveAsCommand :  XSemmelCommand
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
            SaveFileDialog dlgSaveFile = new SaveFileDialog();
            dlgSaveFile.Filter = "Xml Files|*.xml|Xsd Files|*.xsd|Xslt Files|*.xslt|All Files|*.*";
            dlgSaveFile.Title = "Select a file to save";
            dlgSaveFile.FileName = ef.XSDocument.Filename;

            if (dlgSaveFile.ShowDialog() == true)
            {
                if (ef.XSDocument.Filename == dlgSaveFile.FileName)
                {
                    MessageBox.Show("Cannot save document to same file as opened");
                    return;
                }
                try
                {
                    MainWindow mainWnd = (MainWindow)Application.Current.MainWindow;
                    mainWnd.SaveFile(dlgSaveFile.FileName);
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
