using System;
using System.Windows;
using XSemmel.Editor;

namespace XSemmel.Commands
{
    class InsertCurrentDateTimeCommand : XSemmelCommand
    {
        protected override bool CanExecute(EditorFrame ef)
        {
            if (ef.XmlEditor == null)
            {
                return false;
            }
            if (!ef.XmlEditor.TextArea.IsFocused)
            {
                return false;
            }
            return true;
        }

        protected override void Execute(EditorFrame ef)
        {
            try
            {
                string stringToInsert = DateTime.Now.ToString("yyyy-MM-dd'T'HH:mm:ss.fffffffZ");
                ef.XmlEditor.SelectedText = stringToInsert;
            } 
            catch (Exception e)
            {
                MessageBox.Show(Application.Current.MainWindow, "Error: " + e.Message, "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

    }
}
