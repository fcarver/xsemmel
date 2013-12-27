using XSemmel.Commands;
using XSemmel.Editor;

namespace XSemmel.TreeView.Commands
{
    class UpdateCommand : XSemmelCommand
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
            ef._editorTree.Update();
        }


    }
}
