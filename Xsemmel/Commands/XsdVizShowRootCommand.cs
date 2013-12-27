using XSemmel.Schema;

namespace XSemmel.Commands
{
    class XsdVizShowRootCommand : XsdVizCommand
    {

        protected override void Execute(XsdVisualizer xsdViz)
        {
            xsdViz.ShowRoot();
        }

    }
}
