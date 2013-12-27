using System;
using System.Windows.Input;
using XSemmel.Editor;

namespace XSemmel.Commands
{
    public abstract class XSemmelCommand : ICommand
    {

        public void Execute(object parameter)
        {
            Data data = parameter as Data;
            if (data != null && data.EditorFrame != null)
            {
                Execute(data.EditorFrame);
            }
        }

        public bool CanExecute(object parameter)
        {
            Data data = parameter as Data;
            if (data != null && data.EditorFrame != null)
            {
                return CanExecute(data.EditorFrame);
            }
            return false;
        }

        protected abstract void Execute(EditorFrame ef);

        protected abstract bool CanExecute(EditorFrame ef);

        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

    }
}
