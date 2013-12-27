using System;
using System.Windows;
using System.Windows.Input;
using XSemmel.Configuration;

namespace XSemmel.Commands
{
    class QuitCommand :  ICommand
    {

        public void Execute(object parameter)
        {
            try
            {
                XSConfiguration.Instance.Save();    
            }
            catch
            {
                //ignore
            }
            Application.Current.Shutdown();
        }

        public bool CanExecute(object parameter)
        {
            return true;
        }

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

    }
}
