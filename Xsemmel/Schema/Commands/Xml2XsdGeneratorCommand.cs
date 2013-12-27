using System;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using Microsoft.Win32;
using XSemmel.Commands;
using XSemmel.Editor;
using XSemmel.Helpers;

namespace XSemmel.Schema.Commands
{
    class Xml2XsdGeneratorCommand : XSemmelCommand
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
            try
            {
                XmlReader reader = XmlReader.Create(ef.XmlEditor.Text.ToStream());

                SaveFileDialog dlgOpenFile = new SaveFileDialog();
                dlgOpenFile.Filter = "Xsd Files (*.xsd)|*.xsd";
                dlgOpenFile.Title = "Select an Xsd File to generate";
                dlgOpenFile.FileName = Path.ChangeExtension(Path.GetFileName(ef.XSDocument.Filename), "xsd");
                dlgOpenFile.InitialDirectory = Path.GetDirectoryName(ef.XSDocument.Filename);

                if (dlgOpenFile.ShowDialog() == true)
                {
                    XmlSchemaInference schema = new XmlSchemaInference();

                    XmlSchemaSet schemaSet = schema.InferSchema(reader);

                    int i = 0;
                    foreach (XmlSchema s in schemaSet.Schemas())
                    {
                        string filename = dlgOpenFile.FileName;
                        if (i > 0)
                        {
                            string extension = Path.GetExtension(filename);
                            if (string.IsNullOrWhiteSpace(extension))
                            {
                                extension = ".xsd";
                            }
                            string name = Path.GetFileNameWithoutExtension(filename);
                            filename = Path.Combine(Path.GetDirectoryName(filename) ?? "", name + "." + i + extension);
                        }

                        if (File.Exists(filename))
                        {
                            var res = MessageBox.Show(Application.Current.MainWindow, "File "+ filename + " already exists. Overwrite?", "Question", MessageBoxButton.OKCancel,
                                    MessageBoxImage.Question);
                            if (res == MessageBoxResult.Cancel)
                            {
                                throw new Exception("Action was cancelled");
                            }
                        }

                        using (FileStream output = new FileStream(filename, FileMode.Create))
                        {
                            s.Write(output);
                        }
                        i++;
                    }
                    MessageBox.Show(Application.Current.MainWindow, i.ToString() + " files were created.", "Information", MessageBoxButton.OK,
                                    MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current.MainWindow, "Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
