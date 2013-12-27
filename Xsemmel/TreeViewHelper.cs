using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace XSemmel
{

    class LazyTreeViewItem : TreeViewItem
    {
        public Action<LazyTreeViewItem> ExpandAction { get; set; }

        public bool IsStillLazy { get; set; }

        public LazyTreeViewItem()
        {
            IsStillLazy = true;
            Expanded += LazyTreeViewItem_Expanded;
        }

        void LazyTreeViewItem_Expanded(object sender, RoutedEventArgs e)
        {
            Expand();
        }

        public void Expand()
        {
            if (ExpandAction != null)
            {
                ExpandAction(this);
            }
            IsStillLazy = false;
        }

    }

    static class TreeViewHelper
    {

        public static TreeViewItem BuildTreeView(ViewerNode vNode)
        {
            return BuildTreeView(vNode, null);
        }

        public static TreeViewItem BuildTreeView(ViewerNode vNode, Func<TreeViewItem, bool> filter)
        {
            if (vNode.NodeType != NodeType.Unknown)
            {
                LazyTreeViewItem node = null;
                if (vNode.NodeType == NodeType.Attribute)
                {
                    if (vNode.AttributeType != AttributeType.None)
                    {
                        return null;
                    }
                    node = new LazyTreeViewItem();

                    StackPanel pnl = new StackPanel {Orientation = Orientation.Horizontal};
                    pnl.Children.Add(new TextBlock {Text = "@" + vNode.LocalName, Foreground = Brushes.DarkRed});
                    if ((vNode.ChildNodes.Count == 0) && !string.IsNullOrEmpty(vNode.Value))
                    {
                        pnl.Children.Add(new TextBlock { Text = ": " + vNode.Value, Foreground = Brushes.DarkBlue});
                    }
                    node.Header = pnl;

                    node.Foreground = Brushes.Brown;
                    node.ToolTip = vNode.ToDetailsString();
                }
                else if (vNode.NodeType == NodeType.Element)
                {
                    node = new LazyTreeViewItem();

                    StackPanel pnl = new StackPanel { Orientation = Orientation.Horizontal };
                    if (string.IsNullOrEmpty(vNode.TypeName))
                    {
                        pnl.Children.Add(new TextBlock { Text = vNode.LocalName, Foreground = Brushes.Black });
                    }
                    else
                    {
                        pnl.Children.Add(new TextBlock { Text = vNode.LocalName + " [" + vNode.TypeName + "]", Foreground = Brushes.Black });
                    }
                    if ((vNode.ChildNodes.Count == 0) && !string.IsNullOrEmpty(vNode.Value))
                    {
                        pnl.Children.Add(new TextBlock { Text = ": " + vNode.Value.Trim(), Foreground = Brushes.DarkBlue });
                    }
                    
                    node.Header = pnl;
                    node.ToolTip = vNode.ToDetailsString();
                }
                node.Tag = vNode;

                if (vNode.Attributes.Count > 0 || vNode.ChildNodes.Count > 0)
                {
                    //to make item expandable
                    node.Items.Add(new LazyTreeViewItem());
                }

                node.ExpandAction += (thisNode) =>
                {
                    //check if child is DUMMY and expand...
                    if (thisNode.Items.Count == 1 && ((LazyTreeViewItem)thisNode.Items[0]).IsStillLazy)
                    {
                        thisNode.Items.Clear();
                        foreach (ViewerNode t in vNode.Attributes)
                        {
                            TreeViewItem tvi = BuildTreeView(t, filter);
                            if (tvi != null)
                            {
                                thisNode.Items.Add(tvi);
                            }
                        }
                        foreach (ViewerNode t in vNode.ChildNodes)
                        {
                            TreeViewItem tvi = BuildTreeView(t, filter);
                            if (tvi != null)
                            {
                                thisNode.Items.Add(tvi);
                            }
                        }
                    }
                };

                if (filter != null)
                {
                    if (!filter(node))
                    {
                        return node;
                    }
                    else
                    {
                        return null;
                    }
                }
                return node;
            }
            return null;
        }


    }
}
