using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XSemmel.Test
{
    [TestClass]
    public class PrettyPrinterTest
    {

        [TestMethod]
        public void XTest()
        {
        }
    }
}

#if JAVA

package de.schnitzer.xmlchecker.prettyprinter;

import java.io.File;
import java.io.FileReader;

import org.junit.Assert;
import org.junit.Test;

public class PrettyPrinterTest {


	private File xmlFile = new File("test/ugly.xml");
	
	
	@Test
	public void prettyprint() throws Exception {
		
		String res = new PrettyPrinter().process(new FileReader(xmlFile), null);
		Assert.assertEquals("<?xml version=\"1.0\" encoding=\"UTF-8\"?>\n" +
				"<books>\n" +
				"   <book>Preety-print me</book>\n" +
				"</books>", res);
	}

}

#endif