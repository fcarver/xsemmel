using XSemmel.Schema;

namespace XSemmel.Commands
{
    class XsdVizHistoryBackCommand : XsdVizCommand
    {

        protected override void Execute(XsdVisualizer xsdViz)
        {
            xsdViz.HistoryBack();
        }

    }
}
