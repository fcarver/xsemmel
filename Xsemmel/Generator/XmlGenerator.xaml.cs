using System.Windows;
using Microsoft.Win32;
using XSemmel.Helpers;

namespace XSemmel.Generator
{
    /// <summary>
    /// Interaction logic for XmlGenerator.xaml
    /// </summary>
    public partial class XmlGenerator
    {
        public XmlGenerator()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlgOpenFile = new OpenFileDialog();
            //            dlgOpenFile.Filter = "Xml Files (*.xml)|*.xml";
            //            dlgOpenFile.Title = "Select an ile to write";
            if (FileHelper.FileExists(_edtPatternFile.Text))
            {
                dlgOpenFile.FileName = _edtPatternFile.Text;
            }
            if ((dlgOpenFile.ShowDialog() == true) && (string.Compare(_edtPatternFile.Text, dlgOpenFile.FileName, true) != 0))
            {
                _edtPatternFile.Text = dlgOpenFile.FileName;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            SaveFileDialog dlgOpenFile = new SaveFileDialog();
            dlgOpenFile.Filter = "Xml Files (*.xml)|*.xml";
            dlgOpenFile.Title = "Select an Xml File to write";
            if (FileHelper.FileExists(_edtXmlFile.Text))
            {
                dlgOpenFile.FileName = _edtXmlFile.Text;
            }
            if ((dlgOpenFile.ShowDialog() == true) && (string.Compare(_edtXmlFile.Text, dlgOpenFile.FileName, true) != 0))
            {
                _edtXmlFile.Text = dlgOpenFile.FileName;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            AstBuilder ast = new AstBuilder(_edtPatternFile.Text, _edtXmlFile.Text);
            ast.generateXML();
            MessageBox.Show(this, "Datei(en) wurden generiert.", "Information", MessageBoxButton.OK,
                            MessageBoxImage.Information);
        }
    }
}
