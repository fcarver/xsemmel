using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using Microsoft.Win32;
using XSemmel.Helpers;

namespace XSemmel.Xslt.Ribbon
{
    public partial class XsltTab
    {
        public XsltTab()
        {
            InitializeComponent();
        }

        private void mnuCopyXsdPath_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(_edtFile.Text);
        }

        private void mnuSelectXsltFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlgOpenFile = new OpenFileDialog();
            dlgOpenFile.Filter = "Supported files|*.xsl;*.xslt;*.xml|Xslt Files (*.xsl,*.xslt)|*.xsl,*.xslt|Xml Files (*.xml)|*.xml|All files|*.*";
            dlgOpenFile.Title = "Select a file to open";
            if (FileHelper.FileExists(_edtFile.Text))
            {
                dlgOpenFile.FileName = _edtFile.Text;
            }
            if (dlgOpenFile.ShowDialog() == true)
            {
                _edtFile.Text = dlgOpenFile.FileName;

                string extension = Path.GetExtension(_edtFile.Text).ToLower();
                if (extension == ".xml")
                {
                    _chkXsltInEditor.IsChecked = true;
                }
                else if (extension == ".xsl" || extension== ".xslt")
                {
                    _chkXmlInEditor.IsChecked = true;
                }
            }
        }

        private void mnuEditXslt_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrEmpty(_edtFile.Text))
            {
                string exePath = Environment.GetCommandLineArgs()[0];
                Process.Start(exePath, _edtFile.Text.Trim('\"'));
            }
            else
            {
                MessageBox.Show("No file selected");
            }
        }

    }
}
