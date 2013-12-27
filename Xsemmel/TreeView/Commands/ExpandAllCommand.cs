using System;
using System.Windows;
using XSemmel.Commands;
using XSemmel.Editor;

namespace XSemmel.TreeView.Commands
{
    class ExpandAllCommand : XSemmelCommand
    {
        protected override bool CanExecute(EditorFrame ef)
        {
            if (ef._editorTree == null)
            {
                return false;
            }
            if (ef._editorTree.tree == null)
            {
                return false;
            }
            return true;
        }

        protected override void Execute(EditorFrame ef)
        {
            try
            {
                var tv = ef._editorTree.tree;
                tv.ExpandAll();
            } 
            catch (Exception e)
            {
                MessageBox.Show(Application.Current.MainWindow, "Error: " + e.Message, "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

    }
}
