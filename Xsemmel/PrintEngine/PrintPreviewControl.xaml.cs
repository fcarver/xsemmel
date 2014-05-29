// this *** needs System.Printing reference
using System.IO;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Xps;
using System.Windows.Xps.Packaging;

// these bastards are hidden in the ReachFramework reference
using PrintEngine;

namespace XSemmel.PrintEngine
{

    /// <summary>
    /// Represents the PrintPreviewDialog class to preview documents 
    /// of type FlowDocument, IDocumentPaginatorSource or DocumentPaginatorWrapper
    /// using the PrintPreviewDocumentViewer class.
    /// </summary>
    /// <remarks>Source: http://community.sharpdevelop.net/forums/t/12012.aspx</remarks>
    public partial class PrintPreviewControl
    {

        /// <summary>
        /// Initialize a new instance of the PrintEngine.PrintPreviewDialog class.
        /// </summary>
        public PrintPreviewControl()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Gets or sets the document viewer.
        /// </summary>
        public PrintPreviewDocumentViewer DocumentViewer
        {
            get { return _documentViewer; }
            set { _documentViewer = value; }
        }

        /// <summary>
        /// Loads the specified FlowDocument document for print preview.
        /// </summary>
        public void LoadDocument(FlowDocument document)
        {
            string temp = Path.GetTempFileName();

            if (File.Exists(temp) == true)
                File.Delete(temp);

            XpsDocument xpsDoc = new XpsDocument(temp, FileAccess.ReadWrite);
            XpsDocumentWriter xpsWriter = XpsDocument.CreateXpsDocumentWriter(xpsDoc);
            xpsWriter.Write(((FlowDocument) document as IDocumentPaginatorSource).DocumentPaginator);

            _documentViewer.Document = xpsDoc.GetFixedDocumentSequence();

            xpsDoc.Close();
        }

        /// <summary>
        /// Loads the specified DocumentPaginatorWrapper document for print preview.
        /// </summary>
        public void LoadDocument(DocumentPaginatorWrapper document)
        {
            string temp = Path.GetTempFileName();

            if (File.Exists(temp))
                File.Delete(temp);

            XpsDocument xpsDoc = new XpsDocument(temp, FileAccess.ReadWrite);
            XpsDocumentWriter xpsWriter = XpsDocument.CreateXpsDocumentWriter(xpsDoc);
            xpsWriter.Write(document);

            _documentViewer.Document = xpsDoc.GetFixedDocumentSequence();

            xpsDoc.Close();
        }

        /// <summary>
        /// Loads the specified IDocumentPaginatorSource document for print preview.
        /// </summary>
        public void LoadDocument(IDocumentPaginatorSource document)
        {
            _documentViewer.Document = (IDocumentPaginatorSource) document;
        }

    }
}