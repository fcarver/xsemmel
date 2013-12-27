using XSemmel.Schema;

namespace XSemmel.Commands
{
    class XsdVizExportCommand : XsdVizCommand
    {

        protected override void Execute(XsdVisualizer xsdViz)
        {
            xsdViz.Export();
        }

    }
}
