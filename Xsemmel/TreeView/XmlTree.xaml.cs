using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using System;
using ICSharpCode.AvalonEdit.Document;
using TaskDialogInterop;
using WpfTreeView;
using System.Diagnostics;
using System.Windows.Media;
using XSemmel.Helpers;

namespace XSemmel.TreeView
{
    public partial class XmlTree
    {

        private string _filter;
        private ViewerNode _rootNode;
        private readonly TreeViewDragDropHandler _ddHandler;
        private XSDocument _lastXmlUsedInUpdate;

        private readonly BackgroundChildExpander _backgroundExpander = new BackgroundChildExpander();

        public XmlTree()
        {
            InitializeComponent();

            _ddHandler = new TreeViewDragDropHandler();
            _ddHandler.OnDropped += moveItem;
            _ddHandler.BackgroundOfItemUnderDrag = Brushes.LightBlue;
            _ddHandler.Parent = _tree;
        }

        public event Action<ViewerNode> SelectionChanged;

        public TreeViewItem SelectNodeBasedOnCursor(TextLocation loc)
        {
            ItemCollection items = _tree.Items;
            Debug.Assert(items.Count <= 1);
            if (items.Count != 1)
            {
                return null;
            }

            LazyTreeViewItem root = (LazyTreeViewItem)_tree.Items[0];
            IEnumerable<LazyTreeViewItem> allChilds = root.AsDepthFirstEnumerable(
                x => {
                         x.Expand();
                         return x.Items.Cast<LazyTreeViewItem>();
                     }
                );

            TreeViewItem match = null;
            foreach (LazyTreeViewItem child in allChilds)
            {
                if (child.Tag != null)
                {
                    ViewerNode node = (ViewerNode)child.Tag;
                    IXmlLineInfo lineInfo = node.LineInfo;
                    if (lineInfo != null)
                    {
                        if (lineInfo.LineNumber == loc.Line && lineInfo.LinePosition <= loc.Column)
                        {
                            //last one counts
                            match = child;
                        }
                        if (lineInfo.LineNumber > loc.Line || (lineInfo.LineNumber == loc.Line && lineInfo.LinePosition > loc.Column))
                        {
                            break;
                        }
                    }
                }
            }

            if (match != null)
            {
                _tree.SelectedItemChanged -= tree_SelectedItemChanged;
                match.IsSelected = true;
                match.BringIntoView();
                _tree.SelectedItemChanged += tree_SelectedItemChanged;
            }
            return match;
        }

        private void highlightFragment(ViewerNode selectedNode)
        {
            if (SelectionChanged == null)
            {
                return;
            }

            //show fragment in fragment view
            if (selectedNode == null)
            {
                SelectionChanged(null);
            }
            else
            {
                SelectionChanged(selectedNode);
            }
        }

        public void Update(XSDocument xsdoc)
        {
            _lastXmlUsedInUpdate = new XSDocument(xsdoc.Xml, xsdoc.XsdFile);

            _tree.Items.Clear();
            Cursor = Cursors.Wait;

            _lblUpdateInProgress.Visibility = Visibility.Visible;
            _tree.Visibility = Visibility.Collapsed;
                
            try
            {
                _rootNode = new ViewerNode(xsdoc);
                _backgroundExpander.SetNodeToExpand(_rootNode);
                if (_filter != null)
                {
                    _tree.Background = Brushes.LightYellow;
                    _tree.Items.Add(TreeViewHelper.BuildTreeView(_rootNode,
                        x =>
                            !((ViewerNode)x.Tag).OriginalNode.OuterXml.Contains(_filter, StringComparison.CurrentCultureIgnoreCase)));
                }
                else
                {
                    _tree.Background = Brushes.White;
                    _tree.Items.Add(TreeViewHelper.BuildTreeView(_rootNode));
                }
                ((TreeViewItem)_tree.Items[0]).IsExpanded = true;
            }
            catch (Exception)
            {
                _tree.Items.Clear();
            }
            finally
            {
                _lblUpdateInProgress.Visibility = Visibility.Collapsed;
                _tree.Visibility = Visibility.Visible;
                Cursor = null;
            }
        }

        private void tree_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (_tree.SelectedItem == null)
            {
                return;
            }

            TreeViewItem selected = ((TreeViewItem)_tree.SelectedItem);

            //expand on click
            selected.IsExpanded = true;

            if (selected.Tag != null)
            {
                ViewerNode selectedNode = (ViewerNode) ((TreeViewItem)_tree.SelectedItem).Tag;
                highlightFragment(selectedNode);
            }
        }

        /// <summary>
        /// select items on right click
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void tree_PreviewMouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            IInputElement element = _tree.InputHitTest(e.GetPosition(_tree));
            while (!((element is System.Windows.Controls.TreeView) || element == null))
            {
                if (element is TreeViewItem)
                    break;

                if (element is FrameworkElement)
                {
                    FrameworkElement fe = (FrameworkElement)element;
                    element = (IInputElement)(fe.Parent ?? fe.TemplatedParent);
                }
                else
                    break;
            }
            if (element is TreeViewItem)
            {
                element.Focus();
//                e.Handled = true;
            }
        }

        private ItemCollection Items
        {
            get { return _tree.Items; }
        }

        public object SelectedItem
        {
            get { return _tree.SelectedItem; }
        }

        public void CollapseAll()
        {
            if (Items.Count == 0)
            {
                return;
            }

            TreeViewItem root = (TreeViewItem)Items[0];
            var allChilds = root.AsDepthFirstEnumerable(
                x => x.Items.Cast<TreeViewItem>());

            foreach (TreeViewItem treeViewItem in allChilds)
            {
                treeViewItem.IsExpanded = false;
            }
        }

        public void ExpandAll()
        {
            foreach (TreeViewItem item in Items)
            {
                item.ExpandSubtree();
            }
        }

        public void ExpandToLevel(uint i)
        {
            if (Items.Count == 0)
            {
                return;
            }

            CollapseAll();

            TreeViewItem root = (TreeViewItem)Items[0];

            ExpandToLevel(root, i);
        }

        public void ExpandToLevel(TreeViewItem root, uint i)
        {
            root.IsExpanded = true;
            if (i <= 0)
            {
                return;
            }

            foreach (TreeViewItem item in root.Items.Cast<TreeViewItem>())
            {
                ExpandToLevel(item, i-1);
            }
        }

        public string Filter 
        { 
            get
            {
                return _filter;
            }
            set
            {
                _filter = value;
                try
                {
                    Mouse.OverrideCursor = Cursors.Wait;
                    Update(_lastXmlUsedInUpdate);
                }
                finally
                {
                    Mouse.OverrideCursor = null;
                }
            } 
        }

        public event Action<string> XmlChanged;

        private void moveItem(TreeViewItem sourceItem, TreeViewItem targetItem)
        {
            ViewerNode source = (ViewerNode)sourceItem.Tag;
            ViewerNode target = (ViewerNode)targetItem.Tag;

            if (source == target)
            {
                return;
            }

            TaskDialogResult res = TaskDialog.Show(
                new TaskDialogOptions
                {
                    AllowDialogCancellation = true,
                    Owner = Application.Current.MainWindow,
                    Title = "Move node",
                    MainInstruction = "Where do you like to drop to source node?",
                    Content = "Source: " + source + "\nTarget: " + target,
                    CommandButtons = new[] {"Make child", "Insert before", "Insert after", "Cancel"},
                    MainIcon = VistaTaskDialogIcon.Information,
                    ExpandedInfo = "Source: " + source.XPath + "\nTarget: " + target.XPath
                });
            try
            {
                string newXml = null;
                switch (res.CommandButtonResult)
                {
                    case 0: //make child
                        newXml = NodeMover.Move(source.OriginalNode, target.OriginalNode);
                        break;
                    case 1: //insert before
                        newXml = NodeMover.InsertBefore(source.OriginalNode, target.OriginalNode);
                        break;
                    case 2: //insert after
                        newXml = NodeMover.InsertAfter(source.OriginalNode, target.OriginalNode);
                        break;
                    case 3: //cancel
                        break;
                    case null: //cancel by closing window
                        break;
                    default:
                        Debug.Fail("");
                        break;
                }

                if (newXml != null)
                {
                    Update(new XSDocument(newXml));
                    if (XmlChanged != null)
                    {
                        XmlChanged(newXml);
                    }
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(Application.Current.MainWindow, e.Message, "Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

    }
}
