using XSemmel.Editor;

namespace XSemmel.Commands
{
    class StartSearchCommand :  XSemmelCommand
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
            ef.StartSearch();
        }

    }
}
