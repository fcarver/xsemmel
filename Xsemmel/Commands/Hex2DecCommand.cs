using System;
using System.Globalization;
using System.Windows;
using XSemmel.Editor;

namespace XSemmel.Commands
{
    class Hex2DecCommand : XSemmelCommand
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
            try
            {
                uint result = Convert.ToUInt32(ef.XmlEditor.SelectedText, 16);
                ef.XmlEditor.SelectedText = result.ToString(CultureInfo.InvariantCulture);
            }
            catch
            {
                MessageBox.Show(Application.Current.MainWindow, "Selection is not a hexadecimal number.", "Error", MessageBoxButton.OK,
                    MessageBoxImage.Error);    
            }
        }

        

    }
}
