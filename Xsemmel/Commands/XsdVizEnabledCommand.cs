using XSemmel.Schema;

namespace XSemmel.Commands
{
    class XsdVizEnabledCommand : XsdVizCommand
    {
        protected override void Execute(XsdVisualizer xsdViz)
        {
            //dieser Command führt nichts aus. Er sorgt durch sein CanProcess aber dafür, der GUI Feedback zu geben, 
            //ob der Visualizer aktiv ist oder nicht
        }

    }
}
