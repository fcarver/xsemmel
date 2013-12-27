using XSemmel.Schema;

namespace XSemmel.Commands
{
    class XsdVizFontMinusCommand : XsdVizCommand
    {

        protected override void Execute(XsdVisualizer xsdViz)
        {
            xsdViz.ZoomOut();
        }

    }
}
