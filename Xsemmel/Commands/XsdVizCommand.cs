using System;
using System.Windows;
using XSemmel.Editor;
using XSemmel.Schema;

namespace XSemmel.Commands
{
    abstract class XsdVizCommand : XSemmelCommand
    {
        protected override bool CanExecute(EditorFrame ef)
        {
            if (ef.XmlEditor == null)
            {
                return false;
            }
            if (ef._xsdVisualizerDockable.Visibility == Visibility.Hidden)
            {
                return false;
            }
            return ef._xsdVisualizerDockable.IsActiveContent;
        }

        protected abstract void Execute(XsdVisualizer xsdVisualizer);

        protected override void Execute(EditorFrame ef)
        {
            if (ef._xsdVisualizer == null || ef._xsdVisualizer.Visibility == Visibility.Hidden)
            {
                throw new Exception();
            }

            Execute(ef._xsdVisualizer);
        }

    }
}
