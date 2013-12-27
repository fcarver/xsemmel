using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
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
    public partial class XPathQueries
    {
        private class WorkItem
        {
            public string Xml = "";
            public XmlDocument XmlDoc;
            public XPathNavigator Navigator;
        }

        private WorkItem _cache = new WorkItem();

        private readonly Brush _xpathTextBackColor;
        private EditorFrame _editor;

        public XPathQueries()
        {
            InitializeComponent();

            _xpathTextBackColor = _edtXPath.Background;

            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("XSemmel.XPath.xpath.xshd"))
            using (XmlTextReader reader = new XmlTextReader(stream))
            {
                var x = HighlightingLoader.Load(reader, HighlightingManager.Instance);
                _edtXPath.SyntaxHighlighting = x;
            }
            new BracketMatcher(_edtXPath);

            _edtXPath.TextArea.SelectionChanged += textArea_SelectionChanged;
        }

        void textArea_SelectionChanged(object sender, EventArgs e)
        {
            if (_edtXPath.SelectionLength == 0)
            {
                _btnExecute.Content = "Execute";
            }
            else
            {
                _btnExecute.Content = "Execute selection";
            }
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

        private void btnCopy_Click(object sender, RoutedEventArgs e)
        {
            if (_edtXPath.SelectionLength > 0)
            {
                string text = _edtXPath.SelectedText;
                Clipboard.SetText(text);
            }
            else
            {
                DataObject dobj = new DataObject();
                dobj.SetText(_edtXPath.Text);

                try
                {
                    IHighlighter highlighter = new DocumentHighlighter(
                        _edtXPath.Document, _edtXPath.SyntaxHighlighting.MainRuleSet);
                    string html = HtmlClipboard.CreateHtmlFragment(
                        _edtXPath.Document, highlighter, null, new HtmlOptions());

                    HtmlClipboard.SetHtml(dobj, html);
                }
                catch
                {
                    //ignore
                }

                Clipboard.SetDataObject(dobj);
            }
        }

        private void edtXPath_TextChanged(object sender, EventArgs eventArgs)
        {
            if (_chkWhileTyping.IsChecked == true)
            {
                try
                {
                    WorkItem c = getWorkItem();
                    createExpression(c, _edtXPath.Text);
                    _edtXPath.Background = _xpathTextBackColor;
                    _edtXPath.ToolTip = null;
                }
                catch (Exception exception)
                {
                    _edtXPath.Background = Brushes.Pink;
                    _edtXPath.ToolTip = "Invalid XPath Query: " + exception.Message;
                }
            }
            else
            {
                _edtXPath.Background = _xpathTextBackColor;
                _edtXPath.ToolTip = null;
            }
        }

        private void displayNavigator(XPathNodeIterator xpi)
        {
            if ((xpi != null) && (xpi.Count > 0))
            {
                for (bool hasNext = xpi.MoveNext(); hasNext; hasNext = xpi.MoveNext())
                {
                    // IXmlLineInfo lineInfo = xpi.Current as IXmlLineInfo;

                    switch (xpi.Current.NodeType)
                    {
                        case XPathNodeType.Text:
                        {
                            TreeViewItem node = new TreeViewItem();
                            node.Header = xpi.Current.Value;
                            node.Foreground = Brushes.Brown;
                            node.ToolTip = "(Nodeset/Text)";
                            _treeResult.Items.Add(node);
                            break;
                        }
                        case XPathNodeType.Attribute:
                        {
                            TreeViewItem node = new TreeViewItem();
                            node.Header = "@" + xpi.Current.Name + ": " + xpi.Current.Value;
                            node.Foreground = Brushes.Brown;
                            node.ToolTip = "(Nodeset/Attribute)";
                            node.Tag = xpi.Current.Clone();
                            _treeResult.Items.Add(node);
                            break;
                        }
                        case XPathNodeType.Element:
                        {
                            var document = new XSDocument(xpi.Current.OuterXml);
                            ViewerNode vNode = new ViewerNode(document);
                            TreeViewItem tvi = TreeViewHelper.BuildTreeView(vNode);

                            //expand all to lazy-load all subitems
                            tvi.ExpandSubtree();
                            //now we can change the tags of all subitems
                            setTag(tvi, xpi.Current.Clone());
                            //collapse them again
                            var allChilds = tvi.AsDepthFirstEnumerable(
                                x => x.Items.Cast<TreeViewItem>());
                            foreach (TreeViewItem treeViewItem in allChilds)
                            {
                                treeViewItem.IsExpanded = false;
                            }

                            _treeResult.Items.Add(tvi);
                            break;
                        }
                    }
                    if (string.IsNullOrEmpty(editorUserControl1.Text))
                    {
                        editorUserControl1.Text = xpi.Current.OuterXml;
                    }
                    else
                    {
                        editorUserControl1.Text = editorUserControl1.Text + "\r\n" + xpi.Current.OuterXml;
                    }
                }
            }
            else
            {
                _treeResult.Items.Add("Nothing found.");
                editorUserControl1.Text = "";
            }
        }

        private void setTag(TreeViewItem tvi, object newTag)
        {
            tvi.Tag = newTag;
            foreach (TreeViewItem kid in tvi.Items)
            {
                setTag(kid, newTag);
            }
        }

        private XPathExpression createExpression(WorkItem c, string xpath)
        {
            XmlNamespaceManager xmlnsManager = new XmlNamespaceManager(c.XmlDoc.NameTable);
            if (_chkUseNamespaces.IsChecked == true)
            {
                c.Navigator.MoveToFollowing(XPathNodeType.Element);
                IDictionary<string, string> whatever = c.Navigator.GetNamespacesInScope(XmlNamespaceScope.All);
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

        private WorkItem getWorkItem()
        {
            string xml = _editor.XmlEditor.Text;
            if (_cache.Xml == xml)
            {
                return _cache;
            }

            //initialize Navigator with XPathDocument and XmlReader to obtain line/column numbers
            var xr = XmlReader.Create(xml.ToStream());
            var xpathdoc = new XPathDocument(xr);
            var navigator = xpathdoc.CreateNavigator();
            var xmldoc = xml.ToXmlDocument();

            _cache = new WorkItem { Xml = xml, XmlDoc = xmldoc, Navigator = navigator };
            return _cache;
        }

        private void query()
        {
            _treeResult.Items.Clear();
            editorUserControl1.Text = "";
            string text = _edtXPath.Text;
            if (_edtXPath.SelectionLength > 0)
            {
                text = _edtXPath.SelectedText;
            }

            WorkItem c;
            try
            {
                c = getWorkItem();
            }
            catch (Exception e)
            {
                TreeViewItem node3 = new TreeViewItem();
                node3.Header = ("Error in Xml document: " + e.Message);
                node3.Foreground = Brushes.Red;
                node3.ToolTip = e.Message;
                _treeResult.Items.Add(node3);
                editorUserControl1.Text = "Error in Xml document: " + e.Message;
                return;
            }

            try
            {
                XPathExpression expression = createExpression(c, text);
                switch (expression.ReturnType)
                {
                    case XPathResultType.String:
                    case XPathResultType.Number:
                    {
                        object result = c.Navigator.Evaluate(expression);

                        TreeViewItem node = new TreeViewItem();
                        node.Header = result;
                        node.Foreground = Brushes.Brown;
                        node.ToolTip = "("+ expression.ReturnType + ")";
                        _treeResult.Items.Add(node);
                        editorUserControl1.Text = result.ToString();
                        break;
                    }
                    case XPathResultType.NodeSet:
                    {
                        XPathNodeIterator nodes = c.Navigator.Select(expression);
                        displayNavigator(nodes);
                        break;
                    }
                    case XPathResultType.Boolean:
                    {
                        TreeViewItem node = new TreeViewItem();
                        node.Foreground = Brushes.Brown;
                        node.ToolTip = "(Boolean)";
                        if (true.Equals(c.Navigator.Evaluate(expression)))
                        {
                            node.Header = "true";
                        }
                        else
                        {
                            node.Header = "false";
                        }
                        _treeResult.Items.Add(node);
                        editorUserControl1.Text = node.Header.ToString();
                        break;
                    }
                }
            }
            catch (XPathException)
            {
                try
                {
                    object obj2 = c.Navigator.Evaluate(text);
                    if (obj2.GetType().Name == "XPathSelectionIterator")
                    {
                        XPathNodeIterator iterator2 = obj2 as XPathNodeIterator;
                        displayNavigator(iterator2);
                    }
                    else
                    {
                        TreeViewItem node = new TreeViewItem();
                        node.Header = obj2.ToString();
                        node.Foreground = Brushes.Brown;
                        node.ToolTip = "(XPathSelectionIterator)";
                        _treeResult.Items.Add(node);
                        editorUserControl1.Text = obj2.ToString();
                    }
                }
                catch (Exception exception)
                {
                    TreeViewItem node2 = new TreeViewItem();
                    node2.Header = ("Error: " + exception.Message);
                    node2.Foreground = Brushes.Red;
                    node2.ToolTip = exception.Message;
                    //                        node2.ContextMenuStrip = this.mnuTreeView;
                    _treeResult.Items.Add(node2);
                    editorUserControl1.Text = "Error: " + exception.Message;
                }
            }
            catch (Exception exception2)
            {
                TreeViewItem node3 = new TreeViewItem();
                node3.Header = ("Error: " + exception2.Message);
                node3.Foreground = Brushes.Red;
                node3.ToolTip = exception2.Message;
                //                    node3.ContextMenuStrip = this.mnuTreeView;
                _treeResult.Items.Add(node3);
                editorUserControl1.Text = "Error: " + exception2.Message;
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

        private void edtXPath_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyboardDevice.Modifiers == ModifierKeys.Control && e.Key == Key.Enter)
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
                e.Handled = true;
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

        private void treeResult_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (_treeResult.SelectedItem as TreeViewItem == null)
            {
                return;
            }

            TreeViewItem selected = ((TreeViewItem)_treeResult.SelectedItem);

            //expand on click
            selected.IsExpanded = true;

            if (selected.Tag != null)
            {
                if (selected.Tag is ViewerNode)
                {
                    ViewerNode vn = (ViewerNode) selected.Tag;
                    setCursor(vn.LineInfo, vn.Name);
                }
                else if (selected.Tag is XPathNavigator)
                {
                    XPathNavigator xpn = (XPathNavigator) selected.Tag;
                    IXmlLineInfo li = xpn as IXmlLineInfo;
                    if (li != null)
                    {
                        setCursor(li, xpn.Name);
                    }
                } 
                else
                {
                    Debug.Fail("");
                }
            }
        }

        private void setCursor(IXmlLineInfo lineInfo, string nodeName)
        {
            if (lineInfo != null)
            {
                var offset = _editor.XmlEditor.Document.GetOffset(lineInfo.LineNumber, lineInfo.LinePosition);
                _editor.XmlEditor.Select(offset, nodeName.Length);
                Debug.Assert(_editor.XmlEditor.SelectedText == nodeName);
                _editor.XmlEditor.ScrollTo(lineInfo.LineNumber, lineInfo.LinePosition);
            }
        }

        private void btnBulkXPath_OnClick(object sender, RoutedEventArgs e)
        {
            BulkXPath.ShowDialog(Application.Current.MainWindow);
        }
    }
}
