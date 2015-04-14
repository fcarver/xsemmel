using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XSemmel.Helpers;

namespace XSemmel.Test
{
    [TestClass]
    public class XPathTest
    {
        [TestMethod]
        public void BuilderTest()
        {
            const string xml = @"
<root>
  <foo />
  <foo>
     <bar attr='value'/>
     <bar other='va' />
  </foo>
  <foo><bar /></foo>
</root>";
            XmlDocument doc = new XmlDocument();
            doc.LoadXml(xml);
            XmlNode node = doc.SelectSingleNode("//@attr");

            string xpath = XPathBuilder.GetXPathToNode(node);
            Assert.AreEqual("/node()[1]/node()[2]/node()[1]/@attr", xpath);

            Console.WriteLine(xpath);
            Assert.AreEqual(node, doc.SelectSingleNode(xpath));
        }
    }
}

#if JAVA
package de.schnitzer.xmlchecker.xpath;

import java.io.File;
import java.io.FileReader;
import java.io.IOException;

import junit.framework.Assert;

import org.junit.Test;

public class XPathTest {
	
	private File xmlFile = new File("test/books.xml");
	
	@Test
	public void allTitles() throws IOException {
		String s = "/bookstore/book/title";
		String ret = new XPath().executeXPath(new FileReader(xmlFile), s);
		
		Assert.assertEquals("<title lang=\"en\">Everyday Italian</title>\n" +
				"<title lang=\"en\">Harry Potter</title>\n" +
				"<title lang=\"en\">XQuery Kick Start</title>\n" +
				"<title lang=\"en\">Learning XML</title>\n",
				ret);
	}

}


#endif