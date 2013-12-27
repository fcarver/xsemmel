using System.IO;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XSemmel.Helpers;
using XSemmel.Xslt;

namespace XSemmel.Test
{
    [TestClass]
    public class XsltTest
    {
        [TestMethod]
        [DeploymentItem(@"SampleFiles\Xml\books.xml")]
        [DeploymentItem(@"SampleFiles\Xml\booktransform.xsl")]
        public void TestTitleAsHtml()
        {
            const string xmlFile = "books.xml";
	        const string xslFile = "booktransform.xsl";

            string xml = File.ReadAllText(xmlFile);
            string xslt = File.ReadAllText(xslFile);

            string res = new XsltTransformer().Transform(xml.ToXmlDocument(), xslt, Encoding.UTF8);
            Assert.AreEqual("<h1>Everyday Italian</h1>\r\n" +
                    "<h2>Giada De Laurentiis</h2>",
                    res);
        }
    }
}
