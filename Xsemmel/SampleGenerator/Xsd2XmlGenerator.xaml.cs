using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Xml;
using System.Xml.Schema;
using ICSharpCode.AvalonEdit.Folding;
using Microsoft.Win32;
using Microsoft.Xml.XMLGen;
using XSemmel.Configuration;
using XSemmel.Helpers;
using XSemmel.Schema.Parser;
using System.Threading;

namespace XSemmel.SampleGenerator
{

    public partial class Xsd2XmlGenerator
    {

        private readonly string _currentFileName;
        private readonly FoldingManager _foldingManager;
        private readonly AbstractFoldingStrategy _foldingStrategy;

        public Xsd2XmlGenerator(string currentFileName)
        {
            InitializeComponent();
            _currentFileName = currentFileName;

            try
            {
                SchemaParser schema = new SchemaParser(currentFileName, false);
                IEnumerable<IXsdNode> nodes = schema.GetVirtualRoot().GetChildren();
                foreach (IXsdNode xsdNode in nodes)
                {
                    XsdElement ele = xsdNode as XsdElement;
                    if (ele != null)
                    {
                        cboRoot.Items.Add(ele.Name);
                    }
                }    
            }
            catch
            {
                //do nothing
            }

            _foldingManager = FoldingManager.Install(edtResult.TextArea);
            _foldingStrategy = new XmlFoldingStrategy();
        }

        private void btnGenerate_Click(object sender, RoutedEventArgs e)
        {
            XmlSchemaSet schemas = new XmlSchemaSet();
            XmlQualifiedName qname;
            object selItem = cboRoot.SelectedItem;
            if (selItem == null)
            {
                qname = XmlQualifiedName.Empty;
            }
            else
            {
                qname = new XmlQualifiedName(selItem.ToString());
            }
            
            int max = 0;
            int listLength = 0;
            
            try
            {
                schemas.Add(null, _currentFileName);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current.MainWindow, "Error in XSD file: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!string.IsNullOrEmpty(edtMaxThreshold.Text))
            {
                max = Int16.Parse(edtMaxThreshold.Text);
            }
            if (!string.IsNullOrEmpty(edtListLength.Text))
            {
                listLength = Int16.Parse(edtListLength.Text);
            }

            try
            {
                using (StringWriter writer = new StringWriter())
                using (XmlTextWriter textWriter = new XmlTextWriter(writer))
                {
                    textWriter.Formatting = Formatting.Indented;
                    XmlSampleGenerator genr = new XmlSampleGenerator(schemas, qname);

                    if (max > 0)
                    {
                        genr.MaxThreshold = max;
                    }
                    if (listLength > 0)
                    {
                        genr.ListLength = listLength;
                    }
                    genr.WriteXml(textWriter);

                    edtResult.Text = writer.ToString();
                }

                _foldingStrategy.UpdateFoldings(_foldingManager, edtResult.Document);
            }
            catch (Exception ex)
            {
                MessageBox.Show(Application.Current.MainWindow, "Error: " + ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "Xml Files|*.xml";
            sfd.Title = "Select a file to save";
            if (sfd.ShowDialog(Application.Current.MainWindow) == true)
            {
                try
                {
                    Debug.Assert(edtResult.Encoding == null || edtResult.Encoding == XSConfiguration.Instance.Config.Encoding);
                    edtResult.Save(sfd.FileName);

                    MessageBox.Show(Application.Current.MainWindow, "Generated file\n" + sfd.FileName,
                        "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Application.Current.MainWindow, "Error: " + ex.Message, "Error", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }
        }

        private void btnOpenInNewWindow_Click(object sender, RoutedEventArgs e)
        {
            string pipename = NamedPipeHelper.StartNewListeningXSemmel();
            new Thread(text =>
            {
                try
                {
                    NamedPipeHelper.Write(pipename, text.ToString());
                }
                catch (Exception ex)
                {
                    MessageBox.Show(Application.Current.MainWindow, ex.Message, "Error", MessageBoxButton.OK,
                                    MessageBoxImage.Error);
                }
            }) {IsBackground = true}.Start(edtResult.Text);
        }

    }
}
