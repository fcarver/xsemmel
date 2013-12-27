using XSemmel.Schema;

namespace XSemmel.Commands
{
    class XsdVizFontPlusCommand : XsdVizCommand
    {

        protected override void Execute(XsdVisualizer xsdViz)
        {
            xsdViz.ZoomIn();
        }

    }
}
