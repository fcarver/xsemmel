using System;
using System.Text;
using System.Windows;
using XSemmel.Configuration;
using XSemmel.Editor;

namespace XSemmel.Commands
{
    class EscapeBase64StringCommand : XSemmelCommand
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
                Encoding enc = ef.XmlEditor.Encoding;
                if (enc == null)
                {
                    enc = XSConfiguration.Instance.Config.Encoding;
                }

                if (ef.XmlEditor.SelectionLength != 0)
                {
                    byte[] encodedBytes = enc.GetBytes(ef.XmlEditor.SelectedText);
                    ef.XmlEditor.SelectedText = Convert.ToBase64String(encodedBytes);
                }
                else
                {
                    byte[] encodedBytes = enc.GetBytes(ef.XmlEditor.Text);
                    ef.XmlEditor.Text = Convert.ToBase64String(encodedBytes);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current.MainWindow, ex.Message, "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        

    }
}
