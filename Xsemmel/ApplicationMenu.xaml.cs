using System.Drawing.Printing;
using System.IO;
using System.Printing;
using System.Windows;
using System.Windows.Controls;
using XSemmel.Configuration;
using XSemmel.PrintEngine;

namespace XSemmel
{
    public partial class ApplicationMenu
    {
        
        public ApplicationMenu()
        {
            InitializeComponent();
        }

        private void btnOptions_Click(object sender, RoutedEventArgs e)
        {
            new ConfigurationWnd {Owner = Application.Current.MainWindow}.ShowDialog();
        }

        private void ApplicationMenu_OnIsOpenChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (true.Equals(e.NewValue))
            {
                _mnuOpen.Children.Clear();

                var files = XSConfiguration.Instance.Config.RecentlyUsedFiles;
                foreach (string file in files)
                {
                    FileInfo fi = new FileInfo(file);
                    if (fi.Exists)
                    {
                        var item = new Fluent.Button
                            {
                                Header = fi.Name,
                                ToolTip = fi.FullName,
                                Tag = file,
                            };

                        item.Click += (x, args) => 
                            ((MainWindow)Application.Current.MainWindow).OpenFile(item.Tag.ToString());
                        _mnuOpen.Children.Add(item);
                    }
                }                
            }
        }

        private void _pnlPrinting_OnIsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            ContentPresenter cp = (ContentPresenter) sender;
            var ef = ((MainWindow)Application.Current.MainWindow).Data.EditorFrame;

            if (ef == null || ef.XmlEditor == null || ef.XSDocument == null)
            {
                cp.Content = new Label {Content = "No file loaded"};
                return;
            }

            PageSettings pageSettings = new PageSettings {Margins = new Margins(40, 40, 40, 40)};

            PrintQueue printQueue = LocalPrintServer.GetDefaultPrintQueue();
            PrintTicket printTicket = printQueue.DefaultPrintTicket;

            PrintPreviewControl printPreview = new PrintPreviewControl();
            printPreview.DocumentViewer.FitToMaxPagesAcross(1);

            printPreview.DocumentViewer.PrintQueue = printQueue;

            if (pageSettings.Landscape)
            {
                printTicket.PageOrientation = PageOrientation.Landscape;
            }

            printPreview.DocumentViewer.PrintTicket = printTicket;
            printPreview.DocumentViewer.PrintQueue.DefaultPrintTicket.PageOrientation = printTicket.PageOrientation;

            printPreview.LoadDocument(Printing.CreateDocumentPaginatorToPrint(ef.XmlEditor, pageSettings, printTicket, ef.XSDocument.Filename));

            // this is stupid, but must be done to view a whole page:
            DocumentViewer.FitToMaxPagesAcrossCommand.Execute("1", printPreview.DocumentViewer);
            
            cp.Content = printPreview;
        }
    }
}
