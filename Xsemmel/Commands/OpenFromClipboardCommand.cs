using System;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace XSemmel.Commands
{
    class OpenFromClipboardCommand :  ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            if (Clipboard.ContainsText())
            {
                MainWindow mainWnd = (MainWindow)Application.Current.MainWindow;
                
                string clip = Clipboard.GetText();
                if (!clip.StartsWith("<") && File.Exists(clip))
                {
                    //Clipboard contains filename
                    mainWnd.OpenFile(clip);
                }
                else
                {
                    mainWnd.OpenFile(MainWindow.CLIPBOARD);
                }
            }
            else
            {
                MessageBox.Show(Application.Current.MainWindow, "No text data in clipboard", "Information", MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
        }

//        public static bool IsValidXmlInClipboard()
//        {
//            if (!Clipboard.ContainsText())
//            {
//                return false;
//            }
//            string clip = Clipboard.GetText();
//            if (clip.StartsWith("<"))
//            {
//                return true;
//            }
//            if (File.Exists(clip))
//            {
//                return true;
//            }
//
//            return false;
//        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

    }
}
