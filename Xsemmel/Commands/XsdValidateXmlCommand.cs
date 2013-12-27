using System;
using System.IO;
using System.Windows;
using System.Xml;
using Microsoft.Win32;
using XSemmel.Editor;
using XSemmel.Schema;

namespace XSemmel.Commands
{
    class XsdValidateXmlCommand : XSemmelCommand
    {
        protected override bool CanExecute(EditorFrame ef)
        {
            if (ef.XmlEditor == null)
            {
                return false;
            }
            return true;
        }

        protected override void Execute(EditorFrame ef)
        {
            if (ef.XSDocument == null)
            {
                throw new Exception("Parameter must be TextEditor");
            }

            OpenFileDialog dlgOpenFile = new OpenFileDialog();
            dlgOpenFile.Filter = "Xml Files (*.xml)|*.xml";
            dlgOpenFile.Title = "Select an Xml file to validate";
            
            if (dlgOpenFile.ShowDialog() == true)
            {
                try
                {
                    var file = dlgOpenFile.FileName;
                    var xsd = new XmlTextReader(new StringReader(ef.XmlEditor.Text));

                    XsdValidationResult result = XsdValidationHelper.Instance.ValidateInstance(
                        xsd, File.ReadAllText(file));
                    switch (result.State)
                    {
                        case ValidationState.Success:
                            MessageBox.Show(Application.Current.MainWindow, "Document is valid", "Validation result",
                                            MessageBoxButton.OK, MessageBoxImage.Information);
                            break;

                        case ValidationState.Warning:
                            MessageBox.Show(Application.Current.MainWindow, "Document is valid with warnings", "Validation result",
                                            MessageBoxButton.OK, MessageBoxImage.Warning);
                            break;

                        case ValidationState.ValidationError:
                            MessageBox.Show(Application.Current.MainWindow, "Document is invalid", "Validation result",
                                            MessageBoxButton.OK, MessageBoxImage.Error);
                            break;

                        case ValidationState.OtherError:
                            MessageBox.Show(Application.Current.MainWindow, "Document is invalid", "Validation result",
                                            MessageBoxButton.OK, MessageBoxImage.Error);
                            
                            break;
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Application.Current.MainWindow, "Error: " + ex.Message, "Validation result",
                                            MessageBoxButton.OK, MessageBoxImage.Error);
                }
                
            }
        }

    }
}
