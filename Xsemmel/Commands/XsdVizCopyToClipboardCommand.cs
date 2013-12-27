using XSemmel.Schema;

namespace XSemmel.Commands
{
    class XsdVizCopyToClipboardCommand : XsdVizCommand
    {

        protected override void Execute(XsdVisualizer xsdViz)
        {
            xsdViz.CopyToClipboard();
        }

    }
}
