using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Microsoft.Win32;

namespace XSemmel.Commands
{
    class OpenCommand :  ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            OpenFileDialog dlgOpenFile = new OpenFileDialog();
            dlgOpenFile.Filter = "Supported files|*.xml;*.xsd;*.xsl;*.xslt|XML files|*.xml|Schema files|*.xsd|Xslt files|*.xsl;*.xslt|All Files|*.*";
            dlgOpenFile.Title = "Select a file to open";
            dlgOpenFile.ValidateNames = false;
            Data data = parameter as Data;
            if (data != null && data.EditorFrame != null)
            {
                if (data.EditorFrame.XSDocument != null && !string.IsNullOrEmpty(data.EditorFrame.XSDocument.Filename))
                {
                    try
                    {
                        dlgOpenFile.InitialDirectory = Path.GetDirectoryName(data.EditorFrame.XSDocument.Filename);
                        dlgOpenFile.FileName = Path.GetFileName(data.EditorFrame.XSDocument.Filename);
                    }
                    catch
                    {
                        //ignore
                    }
                }    
            }
            if (dlgOpenFile.ShowDialog() == true)
            {
                MainWindow mainWnd = (MainWindow)Application.Current.MainWindow;
                mainWnd.OpenFile(dlgOpenFile.FileName);
            }
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

    }

}
