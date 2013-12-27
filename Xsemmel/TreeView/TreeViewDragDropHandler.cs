using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace WpfTreeView
{
    public class TreeViewDragDropHandler
    {
        private TreeViewItem _draggedItem, _target;
        private TreeView _parent;

        public TreeViewDragDropHandler()
        {
            BackgroundOfItemUnderDrag = Brushes.Aqua;
            BackgroundOfNormalItem = new TreeViewItem().Background;
        }

        public TreeView Parent
        {
            set
            {
                _parent = value;
                _parent.AllowDrop = true;

                var style = new Style(typeof(TreeViewItem));
                style.Setters.Add(new EventSetter(UIElement.DragOverEvent, new DragEventHandler(treeView_DragOver)));
                style.Setters.Add(new EventSetter(UIElement.DragLeaveEvent, new DragEventHandler(treeView_DragLeave)));
                style.Setters.Add(new EventSetter(UIElement.DropEvent, new DragEventHandler(treeView_Drop)));
                style.Setters.Add(new EventSetter(UIElement.MouseMoveEvent, new MouseEventHandler(treeView_MouseMove)));
                _parent.ItemContainerStyle = style;
            }
        }

        public Brush BackgroundOfItemUnderDrag { get; set; }
        private Brush BackgroundOfNormalItem { get; set; }

        private void treeView_MouseMove(object sender, MouseEventArgs e)
        {
            try
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    _draggedItem = (TreeViewItem)_parent.SelectedItem;
                    if (_draggedItem != null)
                    {
                        DragDropEffects finalDropEffect = DragDrop.DoDragDrop(
                            _parent, _parent.SelectedValue, DragDropEffects.Move);

                        //Checking target is not null and item is dragging(moving)
                        if ((finalDropEffect == DragDropEffects.Move) && (_target != null))
                        {
                            //A Move drop was accepted
                            if (OnDropped != null)
                            {
                                OnDropped(_draggedItem, _target);
                            }
                            _target.Background = BackgroundOfNormalItem;
                            _target = null;
                            _draggedItem = null;
                        }
                    }
                }
            }
            catch (Exception)
            {
                if (_target != null)
                {
                    _target.Background = BackgroundOfNormalItem;
                }
            }
        }

        public delegate void DropEventHandler(TreeViewItem draggedItem, TreeViewItem target);

        public event DropEventHandler OnDropped;

        private void treeView_DragOver(object sender, DragEventArgs e)
        {
            try
            {
                // Verify that this is a valid drop and then store the drop target
                TreeViewItem item = getNearestContainer(e.OriginalSource as UIElement);
                if (checkDropTarget(_draggedItem, item))
                {
                    e.Effects = DragDropEffects.Move;
                }
                else
                {
                    e.Effects = DragDropEffects.None;
                }
                item.Background = BackgroundOfItemUnderDrag;
                e.Handled = true;
            }
            catch (Exception)
            {
            }
        }

        private void treeView_DragLeave(object sender, DragEventArgs e)
        {
            try
            {
                TreeViewItem item = getNearestContainer(e.OriginalSource as UIElement);
                item.Background = BackgroundOfNormalItem;
                e.Handled = true;
            }
            catch (Exception)
            {
            }
        }

        private void treeView_Drop(object sender, DragEventArgs e)
        {
            try
            {
                e.Effects = DragDropEffects.None;
                e.Handled = true;

                // Verify that this is a valid drop and then store the drop target
                TreeViewItem targetItem = getNearestContainer(e.OriginalSource as UIElement);
                if (targetItem != null && _draggedItem != null)
                {
                    _target = targetItem;
                    e.Effects = DragDropEffects.Move;
                }
            }
            catch (Exception)
            {
            }
        }

        private bool checkDropTarget(TreeViewItem sourceItem, TreeViewItem targetItem)
        {
            return sourceItem != targetItem;
        }

        private TreeViewItem getNearestContainer(UIElement element)
        {
            // Walk up the element tree to the nearest tree view item.
            TreeViewItem container = element as TreeViewItem;
            while ((container == null) && (element != null))
            {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                container = element as TreeViewItem;
            }
            return container;
        }


    }
}
