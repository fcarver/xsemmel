using System.Windows;
using XSemmel.Editor;

namespace XSemmel.Commands
{
    class ToggleSearchAndReplaceCommand :  XSemmelCommand
    {
        protected override bool CanExecute(EditorFrame ef)
        {
            if (ef.XmlEditor == null)
            {
                return false;
            }
            return true;
        }

        protected override void Execute(EditorFrame ef)
        {
            if (ef._xPathSearchAndReplaceDockable.Visibility == Visibility.Visible)
            {
                ef._xPathQueryDockable.Focus();
                ef._xPathSearchAndReplaceDockable.Visibility = Visibility.Collapsed;
            }
            else
            {
                ef._xPathSearchAndReplaceDockable.Visibility = Visibility.Visible;
                ef._xPathSearchAndReplaceDockable.Focus();
            }
        }

    }
}
