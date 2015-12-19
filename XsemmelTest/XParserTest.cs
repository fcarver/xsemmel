using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XSemmel.Editor;

namespace XSemmel.Test
{
    [TestClass]
    public class XParserTest
    {
        [TestMethod]
        public void GetElementAtCursorTest()
        {
            string s = XParser.GetElementAtCursor(@"<element attr1=""hjdf"">", 21);
            Assert.AreEqual("element", s);

            string s2 = XParser.GetElementAtCursor(@"</element>", 9);
            Assert.AreEqual("element", s2);

            string s3 = XParser.GetElementAtCursor(@"<element/>", 7);
            Assert.AreEqual("element", s3);

            string s4 = XParser.GetElementAtCursor("<?xml version=\"1.0\" encoding=\"utf-8\" ?>\r\n<sd>", 45);
            Assert.AreEqual("sd", s4);
        }

        [TestMethod]
        public void IsClosingElementTest()
        {
            const string x1 = @"<element attr1=""hjdf"">";
            Assert.IsFalse(XParser.IsClosingElement(x1, 21, "element"));

            const string x2 = @"</element>";
            Assert.IsTrue(XParser.IsClosingElement(x2, 9, "element"));

            const string x3 = @"<element/>";
            Assert.IsTrue(XParser.IsClosingElement(x3, 9, "element"));

            const string x4 = @"<element>";
            Assert.IsFalse(XParser.IsClosingElement(x4, 6, "element"));

            Assert.IsFalse(XParser.IsClosingElement(@"<element attr1=""hjdf"">", 21));
            Assert.IsTrue(XParser.IsClosingElement(@"</element>", 9));
            Assert.IsTrue(XParser.IsClosingElement(@"<element/>", 9));
            Assert.IsFalse(XParser.IsClosingElement(@"<element>", 6));
        }


        [TestMethod]
        public void IsClosingElementTest2()
        {
            const string xml =
                "<file id=\"1\">\r\n        <name>uImage-2.6.33-omap-pm</name>\r\n        <checksum type=\"md5\">0E49369FCCD724CB7E44AC401902E2D8</checksum>\r\n      </file>";
            Assert.IsFalse(XParser.IsClosingElement(xml, 79));
        }


        [TestMethod]
        public void GetElementAtCursorFuzzyTest()
        {
            const string x = @"<element attr1=""hjdf"">";

            string s = XParser.GetElementAtCursorFuzzy(x, 9);
            Assert.AreEqual("element", s);
        }

        [TestMethod]
        public void GetParentElementAtCursorTest1()
        {
            const string x = @"<a><b></b><";

            string s = XParser.GetParentElementAtCursor(x, 11);
            Assert.AreEqual("a", s);
        }

        [TestMethod]
        public void GetParentElementAtCursorTest2()
        {
            const string x1 = @"<a><b></b><";

            string s1 = XParser.GetParentElementAtCursor(x1, 0);
            Assert.AreEqual("", s1);

            string s2 = XParser.GetParentElementAtCursor(x1, 1);
            Assert.AreEqual("", s2);

            const string x2 = @"<ActivationResponse>
	<";
            string s3 = XParser.GetParentElementAtCursor(x2, x2.Length-1);
            Assert.AreEqual("ActivationResponse", s3);
        }

        [TestMethod]
        public void GetParentElementAtCursorTest3()
        {
            const string x3 = @"<ActivationResponse><</ActivationResponse>";
            string s4 = XParser.GetParentElementAtCursor(x3, 21);
            Assert.AreEqual("ActivationResponse", s4);
        }

        [TestMethod]
        public void GetParentElementAtCursorTest4()
        {
            const string x3 = @"<A><files><file></file><file></file></files></A>";
            string s4 = XParser.GetParentElementAtCursor(x3, 23);
            Assert.AreEqual("files", s4);
        }


        [TestMethod]
        public void IsInsideAttributeValueTest()
        {
            Assert.AreEqual(false, XParser.IsInsideAttributeValue(@"<ActivationResponse><</ActivationResponse>", 5));
            Assert.AreEqual(false, XParser.IsInsideAttributeValue(@"<hallo abcd=""xxx""></hallo>", 3));
            Assert.AreEqual(false, XParser.IsInsideAttributeValue(@"<hallo abcd=""xxx""></hallo>", 9));
            Assert.AreEqual(true, XParser.IsInsideAttributeValue(@"<hallo abcd=""xxx""></hallo>", 15));
        }

        [TestMethod]
        public void IsInsideAttributeKeyTest()
        {
            Assert.AreEqual(false, XParser.IsInsideAttributeKey(@"<ActivationResponse><</ActivationResponse>", 5));
            Assert.AreEqual(false, XParser.IsInsideAttributeKey(@"<hallo abcd=""xxx""></hallo>", 3));
            Assert.AreEqual(true, XParser.IsInsideAttributeKey(@"<hallo abcd=""xxx""></hallo>", 9));
            Assert.AreEqual(false, XParser.IsInsideAttributeKey(@"<hallo abcd=""xxx""></hallo>", 15));

            const string xml =
                "<file id=\"1\">\r\n        <name>uImage-2.6.33-omap-pm</name>\r\n        <checksum type=\"md5\">0E49369FCCD724CB7E44AC401902E2D8</checksum>\r\n      </file>";
            Assert.IsFalse(XParser.IsInsideAttributeKey(xml, 9));
        }

        [TestMethod]
        public void IsInsideElementDeclarationTest()
        {
            Assert.IsFalse(XParser.IsInsideElementDeclaration("", 0));

            Assert.IsTrue(XParser.IsInsideElementDeclaration("<elem ", 5));

            Assert.IsFalse(XParser.IsInsideElementDeclaration("<a></a>", 3));
        }

        [TestMethod]
        public void IsInsideEmptyElementTest()
        {
            Assert.IsFalse(XParser.IsInsideEmptyElement("", 0));

            Assert.IsFalse(XParser.IsInsideEmptyElement("<elem></elem> ", 3));
            Assert.IsTrue(XParser.IsInsideEmptyElement("<elem/> ", 3));

            string s = @"<hallo datum=""2014-02-24 15:45:00"">
		                    <lastName>Duck</lastName>
		                    <proto>GUSTAV</proto>
	                    </hallo>";
            for (int i = 1; i < s.Length; i++)
            {
                Assert.IsFalse(XParser.IsInsideEmptyElement(s, i));
            }

        }

        [TestMethod]
        public void IsInsideCommentTest()
        {
            Assert.IsFalse(XParser.IsInsideComment("xxx", 2));
            Assert.IsTrue(XParser.IsInsideComment("<!-- xxx --> ", 6));
        }

        [TestMethod]
        public void TrimTest()
        {
            const string a1 = "2014-09-11 16:36:07,184 INFO [30] myclassname - [d9b4b140-71a4-44ff-9fdd-dcdfd2757191] [<?xml version=\"1.0\" encoding=\"UTF-8\"?><Request RequestId=\"Transfer\" Version=\"1\" MsgId=\"6\"><Customer><Field Type=\"Id\">1</Field><Field Type=\"LocationId\">Home</Field><Field Type=\"LastName\">Duck</Field><Field Type=\"CustomerType\">Adult</Field></Customer><Record Id=\"2B075C23-0E85-E232-E14A-48D9EFC888B0\" PrtId=\"7EC25208-88A2-4B46-A813-71A61EBD04EC\" State=\"Valid\" CreationDateTime=\"20140911164711+1200\"><Parameter Type=\"Numeric\" Code=\"0002-4a04\" Label=\"Count\" IsManual=\"0\" CreationDateTime=\"20140911164630+1200\"></Parameter></Record></Request>]";
            const string e1 = "<?xml version=\"1.0\" encoding=\"UTF-8\"?><Request RequestId=\"Transfer\" Version=\"1\" MsgId=\"6\"><Customer><Field Type=\"Id\">1</Field><Field Type=\"LocationId\">Home</Field><Field Type=\"LastName\">Duck</Field><Field Type=\"CustomerType\">Adult</Field></Customer><Record Id=\"2B075C23-0E85-E232-E14A-48D9EFC888B0\" PrtId=\"7EC25208-88A2-4B46-A813-71A61EBD04EC\" State=\"Valid\" CreationDateTime=\"20140911164711+1200\"><Parameter Type=\"Numeric\" Code=\"0002-4a04\" Label=\"Count\" IsManual=\"0\" CreationDateTime=\"20140911164630+1200\"></Parameter></Record></Request>";
            Assert.AreEqual(e1, XParser.Trim(a1));

            Assert.AreEqual("<hallo/>", XParser.Trim("<hallo/>"));
            
            try
            {
                Assert.AreEqual("<", XParser.Trim("<"));
            }
            catch (Exception e)
            {
                Assert.AreEqual("No XML found", e.Message);
            }

            try
            {
                XParser.Trim("xxx");
            }
            catch (Exception e)
            {
                Assert.AreEqual("No XML found", e.Message);
            }
        }

    }
}
