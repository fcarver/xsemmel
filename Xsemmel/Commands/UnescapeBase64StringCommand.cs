using System;
using System.Text;
using System.Windows;
using XSemmel.Configuration;
using XSemmel.Editor;

namespace XSemmel.Commands
{
    class UnescapeBase64StringCommand : XSemmelCommand
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
                    byte[] encodedBytes = Convert.FromBase64String(ef.XmlEditor.SelectedText);
                    ef.XmlEditor.SelectedText = enc.GetString(encodedBytes);
                }
                else
                {
                    byte[] encodedBytes = Convert.FromBase64String(ef.XmlEditor.Text);
                    ef.XmlEditor.Text = enc.GetString(encodedBytes);
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
