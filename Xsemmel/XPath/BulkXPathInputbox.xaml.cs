using System.IO;
using System.Reflection;
using System.Windows;
using System.Xml;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Microsoft.Win32;

namespace XSemmel.XPath
{

    public partial class BulkXPathInputBox
    {
        public BulkXPathInputBox()
        {
            InitializeComponent();

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("XSemmel.XPath.xpath.xshd"))
            using (XmlTextReader reader = new XmlTextReader(stream))
            {
                var x = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                EdtXPath.SyntaxHighlighting = x;
            }
        }


        private void mnuPaste_Click(object sender, RoutedEventArgs e)
        {
            EdtXPath.SelectedText = Clipboard.GetText();
        }

        private void mnuCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(EdtXPath.Text);
        }

        private void mnuLoadFromFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlgOpenFile = new OpenFileDialog();
            dlgOpenFile.Filter = "All files|*.*";
            dlgOpenFile.Title = "Select an file to load XPath from";

            if (dlgOpenFile.ShowDialog() == true)
            {
                EdtXPath.Load(dlgOpenFile.FileName);
            }
        }

        private void mnuDelete_Click(object sender, RoutedEventArgs e)
        {
            if (EdtXPath.SelectionLength > 0)
            {
                EdtXPath.SelectedText = "";
            }
            else
            {
                EdtXPath.Text = "";
            }
        }



    }
}
