using System;
using System.Threading;
using System.Windows;
using System.Xml;
using ICSharpCode.AvalonEdit.Folding;
using WmHelp.XmlGrid;
using XSemmel.Configuration;
using XSemmel.Helpers;
using XSemmel.PrettyPrinter;

namespace XSemmel.Editor
{

    public partial class FragmentView
    {

        private readonly FoldingManager _foldingManager;
        private readonly AbstractFoldingStrategy _foldingStrategy;

        public FragmentView()
        {
            InitializeComponent();
            _foldingStrategy = new XmlFoldingStrategy();
            _foldingManager = FoldingManager.Install(_edtFragment.TextArea);
        }

        public void SetText(string fragment)
        {
            fragment = fragment.Trim();
            if (fragment.StartsWith("<"))  //performance optimization, gridview will take long time if it is no xml document
            {
                if (XSConfiguration.Instance.Config.AlwaysPrettyprintFragments)
                {
                    try
                    {
                        _edtFragment.Text = PrettyPrint.Execute(fragment, true, false, true);
                    }
                    catch
                    {
                        _edtFragment.Text = fragment;
//                    MessageBox.Show(Application.Current.MainWindow, "Cannot prettyprint selected text:\n" + e.Message,
//                        "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
                else
                {
                    _edtFragment.Text = fragment;
                }

                try
                {
                    GridBuilder builder = new GridBuilder();
                    GridCellGroup root = new GridCellGroup();

                    //XmlDocument xmldoc = fragment.ToXmlDocument();
                    XmlDocument xmldoc = new XmlDocument();
                    //do this to remove #whitespace nodes
                    xmldoc.LoadXml(fragment.ToXmlDocument().OuterXml);

                    builder.ParseNodes(root, null, xmldoc.ChildNodes);
                    _gridFragment.Cell = root;
                    _gridFragment.FullExpand();
                }
                catch (Exception)
                {
                    _gridFragment.Cell = null;
                }
            }
            else
            {
                _edtFragment.Text = fragment;
                _gridFragment.Cell = null;
            }

            updateFolding();
        }

        private void updateFolding()
        {
            if (_foldingStrategy != null)
            {
                _foldingStrategy.UpdateFoldings(_foldingManager, _edtFragment.Document);
            }
        }

        private void mnuOpenInNewXSemmel_Click(object sender, RoutedEventArgs e)
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
            }) { IsBackground = true }.Start(_edtFragment.Text);
        }
    }
}
