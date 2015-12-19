using System;
using System.Windows;
using System.Windows.Input;

namespace XSemmel.TcpListener.Commands
{
    class OpenFromTcpCommand :  ICommand
    {
        public bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            MainWindow mainWnd = (MainWindow)Application.Current.MainWindow;

            ConfigureListener.Config configuration;
            if (ConfigureListener.ShowDialog(mainWnd, out configuration))
            {
                mainWnd.OpenFile(MainWindow.NEW);

                Data data = parameter as Data;
                if (data == null || data.EditorFrame == null)
                {
                    MessageBox.Show(
                        Application.Current.MainWindow, 
                        "Internal error occurred. Cannot access editor.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

                TcpListeningWindow wnd = new TcpListeningWindow(configuration, data.EditorFrame); // { Owner = mainWnd };  //kein Owner, damit das Fenster aufbleiben kann während das MainWindow minimiert ist
                wnd.Topmost = true;
                wnd.Show();
                wnd.Start();
//                wnd.Activate();
            }
        }


        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

    }
}
