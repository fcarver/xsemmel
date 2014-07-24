using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XSemmel.Editor;

namespace XSemmel.Helpers
{
    public static class XActor
    {

        public static void ExpandEmptyTag(string text, int cursor, out int newCursor, out int startReplace, out int lengthReplace, out string replaceWith)
        {
            Debug.Assert(XParser.IsInsideEmptyElement(text, cursor));


//            int idxStart = text.LastIndexOf('<', cursor);
            int idxEnd = text.IndexOf('>', cursor);
            Debug.Assert(idxEnd != -1, "Can only happen of XParser.IsInsideEmptyElement is incorrect");
            

            string tag = XParser.GetElementAtCursor(text, cursor - 1);

            startReplace = idxEnd - 1;

            string element = string.Format("></{0}>", tag);

//            startReplace = idxStart;
            lengthReplace = 2;  //we assume that endtag is always "/>" and not something with spaces, like "/    >"
            replaceWith = element;
            newCursor = idxEnd + 1;
        }

    }
}
