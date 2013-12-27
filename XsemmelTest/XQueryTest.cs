using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace XSemmel.Test
{
    [TestClass]
    public class XQueryTest
    {
        [TestMethod]
        public void XTest()
        {
        }
    }
}

#if JAVA
package de.schnitzer.xmlchecker.xquery;

import java.io.File;
import java.io.FileReader;
import java.io.StringReader;

import junit.framework.Assert;

import org.junit.Test;

public class XQueryTest
{

    private File xmlFile = new File("test/books.xml");

    @Test
    public void cheapBook() throws Exception
    {
        String query =
            "" + "declare option saxon:output \"omit-xml-declaration=yes\";\n"
                + "declare option saxon:output \"indent=yes\";" + "/bookstore/book[price<30]";
        String res = new XQuery().query(new FileReader(xmlFile), new StringReader(query));

        Assert.assertEquals("<book category=\"CHILDREN\">\n"
            + "  <title lang=\"en\">Harry Potter</title>\n" + "  <author>J K. Rowling</author>\n"
            + "  <year>2005</year>\n" + "  <price>29.99</price>\n" + "</book>", res);
    }

}


#endif