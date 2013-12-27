using System;
using System.Windows;
using System.Xml;
using XSemmel.Commands;
using XSemmel.Editor;
using XSemmel.Helpers;

namespace XSemmel.Schema.Commands
{
    class InsertXsdLocationCommand : XSemmelCommand
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
            if (string.IsNullOrWhiteSpace(ef.Data.ValidationData.Xsd))
            {
                return false;
            }
            return true;
        }

        protected override void Execute(EditorFrame ef)
        {
            try
            {
                XmlDocument xmldoc = ef.XmlEditor.Text.ToXmlDocument();

                XmlElement root = xmldoc.DocumentElement;

                root.SetAttribute("noNamespaceSchemaLocation",
                                  "http://www.w3.org/2001/XMLSchema-instance",
                                  ef.Data.ValidationData.Xsd);

                ef.XmlEditor.Text = xmldoc.ToUTF8String();
            } 
            catch (Exception e)
            {
                MessageBox.Show(Application.Current.MainWindow, "Error: " + e.Message, "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

    }
}
