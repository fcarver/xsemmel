using System;
using System.Windows;
using XSemmel.Commands;
using XSemmel.Editor;

namespace XSemmel.TreeView.Commands
{
    abstract class ExpandToLevelCommand : XSemmelCommand
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
                tv.ExpandToLevel(GetLevel()-1);
            } 
            catch (Exception e)
            {
                MessageBox.Show(Application.Current.MainWindow, "Error: " + e.Message, "Error", MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }

        protected abstract uint GetLevel();

    }

    class ExpandToLevel1Command : ExpandToLevelCommand
    {
        protected override uint GetLevel()
        {
            return 1;
        }
    }

    class ExpandToLevel2Command : ExpandToLevelCommand
    {
        protected override uint GetLevel()
        {
            return 2;
        }
    }

    class ExpandToLevel3Command : ExpandToLevelCommand
    {
        protected override uint GetLevel()
        {
            return 3;
        }
    }

    class ExpandToLevel4Command : ExpandToLevelCommand
    {
        protected override uint GetLevel()
        {
            return 4;
        }
    }

    class ExpandToLevel5Command : ExpandToLevelCommand
    {
        protected override uint GetLevel()
        {
            return 5;
        }
    }

}
