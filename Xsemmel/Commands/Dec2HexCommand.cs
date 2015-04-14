using System.Globalization;
using System.Windows;
using XSemmel.Editor;

namespace XSemmel.Commands
{
    class Dec2HexCommand : XSemmelCommand
    {
        protected override bool CanExecute(EditorFrame ef)
        {
            if (ef.XmlEditor == null)
            {
                return false;
            }
//            if (!ef.XmlEditor.TextArea.IsFocused)
//            {
//                return false;
//            }
            if (ef.XmlEditor.SelectionLength == 0)
            {
                return false;
            }
            return true;
        }

        protected override void Execute(EditorFrame ef)
        {
            uint result;
            if (uint.TryParse(ef.XmlEditor.SelectedText, NumberStyles.Integer, CultureInfo.InvariantCulture, out result))
            {
                ef.XmlEditor.SelectedText = result.ToString("x2", CultureInfo.InvariantCulture);
            }
            else
            {
                MessageBox.Show(Application.Current.MainWindow, "Selection is not a number.", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);    
            }
        }


    }
}
