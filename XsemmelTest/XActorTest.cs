using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XSemmel.Editor;
using XSemmel.Helpers;

namespace XSemmel.Test
{
    [TestClass]
    public class XActorTest
    {
        [TestMethod]
        public void ExpandEmptyTag1Test()
        {
            const string text = @"<element/>";
            string after = expandEmptyTag(text, 7);
            Assert.AreEqual("<element></element>", after);
        }

        [TestMethod]
        public void ExpandEmptyTag2Test()
        {
            const string text = @"<a href=""xxx""/>";
            string after = expandEmptyTag(text, 7);
            Assert.AreEqual(@"<a href=""xxx""></a>", after);
        }


        private string expandEmptyTag(string text, int cursor)
        {
            int newCursor;
            int startReplace;
            int lengthReplace;
            string replaceWith;
            XActor.ExpandEmptyTag(text, cursor, out newCursor, out startReplace, out lengthReplace, out replaceWith);

            return Replace(text, startReplace, lengthReplace, replaceWith);
        }

        private string Replace(string text, int start, int length, string replaceWith)
        {
            return new StringBuilder(text).Remove(start, length).Insert(start, replaceWith).ToString();
        }

    }
}
