using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Highlighting.Xshd;
using Microsoft.Win32;
using XSemmel.Editor;
using XSemmel.Helpers;
using Clipboard = System.Windows.Clipboard;
using System.Collections.Generic;

namespace XSemmel.XPath
{

    public partial class XPathSearchAndReplace
    {
        
        private EditorFrame _editor;

        private readonly IHighlightingDefinition _xpathHighlighter;
        private readonly IHighlightingDefinition _xmlHighlighter;

        public XPathSearchAndReplace()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("XSemmel.XPath.xpath.xshd"))
            using (XmlTextReader reader = new XmlTextReader(stream))
            {
                var x = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                _xpathHighlighter = x;
            }

            _xmlHighlighter = HighlightingManager.Instance.GetDefinition("XML");

            InitializeComponent();
        }

        public EditorFrame Editor
        {
            set
            {
                if (_editor != null)
                {
                    throw new Exception("Editor already set");
                }
                _editor = value;
            }
        }

        private void performSnR(XPathNavigator navigator, XPathExpression expression)
        {
            XPathNodeIterator xpi = navigator.Select(expression);
            int count = xpi.Count;
            if (count > 0)
            {
                while (xpi.MoveNext())
                {
                    if (_chkReplaceByDelete.IsChecked == true)
                    {
                        xpi.Current.DeleteSelf();
                        xpi = navigator.Select(expression);
                    }
                    else if (_chkReplaceByText.IsChecked == true)
                    {
                        xpi.Current.InnerXml = _edtReplaceBy.Text;
                    }
                    else
                    {
                        string res;

                        XPathExpression query = xpi.Current.Compile(_edtReplaceBy.Text);

                        object o = navigator.Evaluate(query, xpi);
                        if (o is string || o is double || o is bool)
                        {
                            res = o.ToString();
                        }
                        else if (o is XPathNodeIterator)
                        {
                            XPathNodeIterator xni = (XPathNodeIterator) o;
                            Debug.Assert(xni.Count == 1);
                            res = xni.Current.OuterXml;
                        }
                        else
                        {
                            Debug.Fail("");
                            res = "";
                        }
                        xpi.Current.InnerXml = res;
                    }
                }

                lblStatus.Content = "Replaced " + count + " occurrences";
            }
            else
            {
                lblStatus.Content = "Nothing found";
            }
        }

        private XPathExpression createExpression(XmlDocument xmlDoc, XPathNavigator nav, string xpath)
        {
            XmlNamespaceManager xmlnsManager = new XmlNamespaceManager(xmlDoc.NameTable);
            if (_chkUseNamespaces.IsChecked == true)
            {
                nav.MoveToFollowing(XPathNodeType.Element);
                IDictionary<string, string> whatever = nav.GetNamespacesInScope(XmlNamespaceScope.All);
                if (whatever != null)
                {
                    foreach (KeyValuePair<string, string> keyValuePair in whatever)
                    {
                        xmlnsManager.AddNamespace(keyValuePair.Key, keyValuePair.Value);
                    }
                }
            }
            else
            {
                Debug.Assert(_chkUseUserdefinedFunctions.IsChecked == true);
                xmlnsManager = getContext();
            }

            return XPathExpression.Compile(xpath, xmlnsManager);
        }


        private void query()
        {
            lblStatus.Content = "";
            string xpath = _edtXPath.Text;
            if (_edtXPath.SelectionLength > 0)
            {
                xpath = _edtXPath.SelectedText;
            }


            XmlDocument xmldoc;
            try
            {
                xmldoc = _editor.XmlEditor.Text.ToXmlDocument();
            }
            catch (Exception e)
            {
                lblStatus.Content = "Error in Xml document: " + e.Message;
                return;
            }

            try
            {
                XPathNavigator nav = xmldoc.CreateNavigator();

                XPathExpression expression = createExpression(xmldoc, nav, xpath);
                switch (expression.ReturnType)
                {
                    case XPathResultType.String:
                    case XPathResultType.Number:
                    case XPathResultType.Boolean:
                    {
                        MessageBox.Show(Application.Current.MainWindow, "Error",
                                        "Search expression must evaluate to a node set", MessageBoxButton.OK,
                                        MessageBoxImage.Error);
                        break;
                    }
                    case XPathResultType.NodeSet:
                    {
                        performSnR(nav, expression);
                        break;
                    }
                }

                _editor.XmlEditor.Text =  xmldoc.ToUTF8String();

            }
            catch (Exception exception2)
            {
                lblStatus.Content = "Error: " + exception2.Message;
            }
        }

        private void btnExecute_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                if (_chkPlainText.IsChecked == true)
                {
                    snrPlainText();
                }
                else
                {
                    query();    
                }
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void snrPlainText()
        {
            Debug.Assert(_chkReplaceByText.IsChecked == true || _chkReplaceByDelete.IsChecked == true);

            lblStatus.Content = "";
            string searchFor = _edtXPath.Text;
            if (_edtXPath.SelectionLength > 0)
            {
                searchFor = _edtXPath.SelectedText;
            }
            if (string.IsNullOrEmpty(searchFor))
            {
                lblStatus.Content = "Nothing found";
                return;
            }

            string replaceBy = _edtReplaceBy.Text;
            if (_chkReplaceByDelete.IsChecked == true)
            {
                replaceBy = "";
            }

            string searchIn = _editor.XmlEditor.Text;

            //Anzahl der occurrences zählen
            int count = 0, n = 0;
            while ((n = searchIn.IndexOf(searchFor, n, StringComparison.InvariantCulture)) != -1)
            {
                n += searchFor.Length;
                ++count;
            }

            //alles ersetzen
            searchIn = searchIn.Replace(searchFor, replaceBy);
            _editor.XmlEditor.Text = searchIn;
                
            if (count > 0)
            {
                lblStatus.Content = "Replaced " + count + " occurrences";
            }
            else
            {
                lblStatus.Content = "Nothing found";
            }
        }

        private XsltContext getContext()
        {
            return XsltContextScript.Instance.Get();
        }

        private void mnuPaste_Click(object sender, RoutedEventArgs e)
        {
            _edtXPath.SelectedText = Clipboard.GetText();
        }

        private void mnuCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(_edtXPath.Text);
        }

        private void mnuLoadFromFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlgOpenFile = new OpenFileDialog();
            dlgOpenFile.Filter = "All files|*.*";
            dlgOpenFile.Title = "Select an file to load XPath from";

            if (dlgOpenFile.ShowDialog() == true)
            {
                _edtXPath.Load(dlgOpenFile.FileName);
            }
        }

        private void mnuDelete_Click(object sender, RoutedEventArgs e)
        {
            if (_edtXPath.SelectionLength > 0)
            {
                _edtXPath.SelectedText = "";
            }
            else
            {
                _edtXPath.Text = "";
            }
        }

        private void chkReplaceByXPath_Checked(object sender, RoutedEventArgs e)
        {
            if (_edtReplaceBy == null)
            {
                return;
            }
            if (_chkReplaceByText.IsChecked == true)
            {
                _edtReplaceBy.SyntaxHighlighting = _xmlHighlighter;
                _edtReplaceBy.IsEnabled = true;
            }
            else if (_chkReplaceByDelete.IsChecked == true)
            {
                _edtReplaceBy.IsEnabled = false;
            }
            else
            {
                _edtReplaceBy.SyntaxHighlighting = _xpathHighlighter;
                _edtReplaceBy.IsEnabled = true;
            }
        }

    }
}
