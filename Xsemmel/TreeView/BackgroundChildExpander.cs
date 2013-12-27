using System.Threading;
using XSemmel.Helpers;

namespace XSemmel.TreeView
{
    class BackgroundChildExpander
    {

        private volatile Thread _thread;

        private void expandDelegate(object startObj)
        {
            ViewerNode node = (ViewerNode) startObj;
            var all = node.AsDepthFirstEnumerable(x => x.ChildNodes); //this enumerable will only enumerate when accessed
            foreach (var viewerNode in all)
            {
                //call ChildNodes to expand them
                var ignore = viewerNode.ChildNodes;
                if (_thread != Thread.CurrentThread)
                {
                    break;
                }
            }
//            SystemSounds.Beep.Play();
//            foreach (var viewerNode in all)
//            {
//                var ignore = viewerNode.LineInfo;
                //call LineInfo to calculate them
//                if (_thread != Thread.CurrentThread)
//                {
//                    break;
//                }
//            }
//            SystemSounds.Beep.Play();
        }

        public void SetNodeToExpand(ViewerNode node)
        {
            _thread = new Thread(expandDelegate) {IsBackground = true};
            _thread.Start(node);
        }

    }
}
