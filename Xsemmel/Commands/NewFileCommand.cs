using System;
using System.Windows;
using System.Windows.Input;

namespace XSemmel.Commands
{
    class NewFileCommand :  ICommand
    {

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            MainWindow mainWnd = (MainWindow)Application.Current.MainWindow;
            mainWnd.OpenFile(MainWindow.NEW);
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

    }
}
