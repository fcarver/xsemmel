using System;
using System.Diagnostics;
using XSemmel.Editor;

namespace XSemmel.Commands
{
    class NewWindowCommand :  XSemmelCommand
    {
        protected override bool CanExecute(EditorFrame ef)
        {
            return true;
        }


        protected override void Execute(EditorFrame ef)
        {
            string exePath = Environment.GetCommandLineArgs()[0];
            Process.Start(exePath);
        }

    }
}
