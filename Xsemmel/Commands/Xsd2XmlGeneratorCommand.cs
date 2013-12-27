using System;
using System.Windows;
using XSemmel.Editor;
using XSemmel.SampleGenerator;

namespace XSemmel.Commands
{
    class Xsd2XmlGeneratorCommand : XSemmelCommand
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
            if (ef.XSDocument == null)
            {
                throw new Exception("Parameter must be TextEditor");
            }

            new Xsd2XmlGenerator(ef.XSDocument.Filename) {Owner = Application.Current.MainWindow}.ShowDialog();
        }

    }
}
