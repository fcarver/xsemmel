using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XSemmel.Test
{
    [TestClass]
    public class WellformedCheckerTest
    {

        [TestMethod]
        public void XTest()
        {
        }
    }
}

#if JAVA
package de.schnitzer.xmlchecker.wellformed;

import java.io.File;
import java.io.FileReader;
import java.io.IOException;

import junit.framework.Assert;

import org.junit.Test;

public class WellformedCheckerTest {
	
	
	@Test
	public void isWellformed() throws IOException {
		File xmlFile = new File("test/books.xml");
		
		String res = new WellformedChecker().check(new FileReader(xmlFile));
		
		Assert.assertEquals("XML is well-formed",
				res);
	}

	@Test
	public void notWellformed() throws IOException {
		File xmlFile = new File("test/booksNotWellformed.xml");

		String res = new WellformedChecker().check(new FileReader(xmlFile));
		
		Assert.assertEquals("XML isn't well-formed:\n" +
				"FatalError: [10|3] org.xml.sax.SAXParseException: The element type \"book3\" must be terminated by the matching end-tag \"</book3>\".\n",
				res);
	}

}


#endif