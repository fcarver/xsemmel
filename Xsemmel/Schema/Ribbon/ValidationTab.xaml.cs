using System;
using System.Diagnostics;
using System.Windows;
using Microsoft.Win32;
using XSemmel.Helpers;

namespace XSemmel.Schema.Ribbon
{
    public partial class ValidationTab
    {
        public ValidationTab()
        {
            InitializeComponent();
        }

        private void chkValidateXsd_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(edtXsdFile.Text))
            {
                mnuSelectXsdFile_Click(sender, e);
            }
        }

        private void mnuCopyXsdPath_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(edtXsdFile.Text);
        }

        private void mnuSelectXsdFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlgOpenFile = new OpenFileDialog();
            dlgOpenFile.Filter = "Xsd Files (*.xsd)|*.xsd";
            dlgOpenFile.Title = "Select a Xsd File to open";
//            if (Editor.XSDocument != null && Editor.XSDocument.Filename != null)
//            {
//                dlgOpenFile.InitialDirectory = Path.GetDirectoryName(Editor.XSDocument.Filename);
//            }
            if (FileHelper.FileExists(edtXsdFile.Text))
            {
                dlgOpenFile.FileName = edtXsdFile.Text;
            }
            if ((dlgOpenFile.ShowDialog(Application.Current.MainWindow) == true) && (string.Compare(edtXsdFile.Text, dlgOpenFile.FileName, true) != 0))
            {
                edtXsdFile.Text = dlgOpenFile.FileName;
                chkValidateXsd.IsChecked = true;
            }
        }

        private void mnuEditXsd_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(edtXsdFile.Text))
            {
                string exePath = Environment.GetCommandLineArgs()[0];
                string xsdFile = edtXsdFile.Text;
                if (!xsdFile.StartsWith("\""))
                {
                    xsdFile = string.Format("\"{0}\"", xsdFile);
                }

                Process.Start(exePath, xsdFile);
            }
            else
            {
                MessageBox.Show(Application.Current.MainWindow, "No xsd file selected");
            }
        }


        private void mnuBulkValidateXsd_Click(object sender, RoutedEventArgs e)
        {
            BulkXsdValidation.ShowDialog(Application.Current.MainWindow);
        }

    }
}
