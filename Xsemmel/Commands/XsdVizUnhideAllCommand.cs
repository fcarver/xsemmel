using XSemmel.Schema;

namespace XSemmel.Commands
{
    class XsdVizUnhideAllCommand : XsdVizCommand
    {

        protected override void Execute(XsdVisualizer xsdViz)
        {
            xsdViz.UnhideAll();
        }

    }
}
