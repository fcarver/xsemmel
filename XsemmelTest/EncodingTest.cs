using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XSemmel.Helpers;

namespace XSemmel.Test
{
    [TestClass]
    public class EncodingTest
    {

        [TestMethod]
        public void Encoding1Test()
        {
            XmlDocument xmldoc = new XmlDocument();
            xmldoc.LoadXml("<hallo></hallo>");
            string s = xmldoc.ToUTF8String();
            Assert.IsTrue(s.Contains("encoding=\"utf-8\""));

            XmlDocument xmldoc2 = new XmlDocument();
            xmldoc2.LoadXml("<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<hallo>\r\n</hallo>");
            string s2 = xmldoc2.ToUTF8String();
            Assert.IsTrue(s2.Contains("encoding=\"utf-8\""));

        }

    }
}
