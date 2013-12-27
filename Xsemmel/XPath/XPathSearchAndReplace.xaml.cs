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

        private void performSnR(XPathNodeIterator xpi, XPathNavigator navigator)
        {
            if ((xpi != null) && (xpi.Count > 0))
            {
                for (bool hasNext = xpi.MoveNext(); hasNext; hasNext = xpi.MoveNext())
                {
                    if (chkReplaceByText.IsChecked == true)
                    {
                        xpi.Current.InnerXml = edtReplaceBy.Text;
                    }
                    else
                    {
                        string res;

                        XPathExpression query = xpi.Current.Compile(edtReplaceBy.Text);

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
                lblStatus.Content = "Replaced " + xpi.Count + " occurrences";
            }
            else
            {
                lblStatus.Content = "Nothing found";
            }
        }

        private XPathExpression createExpression(XmlDocument xmlDoc, XPathNavigator nav, string xpath)
        {
            XmlNamespaceManager xmlnsManager = new XmlNamespaceManager(xmlDoc.NameTable);
            if (chkUseNamespaces.IsChecked == true)
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
                Debug.Assert(chkUseUserdefinedFunctions.IsChecked == true);
                xmlnsManager = getContext();
            }

            return XPathExpression.Compile(xpath, xmlnsManager);
        }


        private void query()
        {
            lblStatus.Content = "";
            string text = edtXPath.Text;
            if (edtXPath.SelectionLength > 0)
            {
                text = edtXPath.SelectedText;
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

                XPathExpression expression = createExpression(xmldoc, nav, text);
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
                        XPathNodeIterator nodes = nav.Select(expression);
                        performSnR(nodes, nav);
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
                query();
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private XsltContext getContext()
        {
            return XsltContextScript.Instance.Get();
        }

        private void mnuPaste_Click(object sender, RoutedEventArgs e)
        {
            edtXPath.SelectedText = Clipboard.GetText();
        }

        private void mnuCopy_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(edtXPath.Text);
        }

        private void mnuLoadFromFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlgOpenFile = new OpenFileDialog();
            dlgOpenFile.Filter = "All files|*.*";
            dlgOpenFile.Title = "Select an file to load XPath from";

            if (dlgOpenFile.ShowDialog() == true)
            {
                edtXPath.Load(dlgOpenFile.FileName);
            }
        }

        private void mnuDelete_Click(object sender, RoutedEventArgs e)
        {
            if (edtXPath.SelectionLength > 0)
            {
                edtXPath.SelectedText = "";
            }
            else
            {
                edtXPath.Text = "";
            }
        }

        private void chkReplaceByXPath_Checked(object sender, RoutedEventArgs e)
        {
            if (edtReplaceBy == null)
            {
                return;
            }
            if (chkReplaceByText.IsChecked == true)
            {
                edtReplaceBy.SyntaxHighlighting = _xmlHighlighter;
            }
            else
            {
                Debug.Assert(chkReplaceByXPath.IsChecked == true);
                edtReplaceBy.SyntaxHighlighting = _xpathHighlighter;
            }
        }

    }
}
