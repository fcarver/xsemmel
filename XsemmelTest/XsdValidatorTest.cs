using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XSemmel.Test
{
    [TestClass]
    public class XsdValidatorTest
    {
        [TestMethod]
        public void XTest()
        {
        }
    }
}

#if JAVA
package de.schnitzer.xmlchecker.schema;

import java.io.File;
import java.io.FileReader;

import org.junit.Assert;
import org.junit.Test;

public class XSDValidatorTest {


	private String xsdFile = "test/note.xsd";
	
	
	@Test
	public void validate() throws Exception {
		File xmlFile = new File("test/note.xml");
		
		String res = new XSDValidator().validate(new FileReader(xmlFile), xsdFile);
		Assert.assertEquals("XML/XSD valide.\n", res);
	}

	@Test
	public void validateFail() throws Exception {
		File xmlFile = new File("test/noteInvalid.xml");
		
		String res = new XSDValidator().validate(new FileReader(xmlFile), xsdFile);
		
		Assert.assertEquals("Error: [3|8] org.xml.sax.SAXParseException: cvc-elt.1: Cannot find the declaration of element 'note2'.\n"+
"--- Errors ---\n",
				res);
	}
	
}


#endif