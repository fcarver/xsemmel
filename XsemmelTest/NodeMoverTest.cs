using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using XSemmel.Differ;
using System.Xml;
using System.IO;
using XSemmel.Helpers;

namespace XSemmel.Test
{
    [TestClass]
    public class NodeMoverTest
    {

        [TestMethod]
        public void MoveTest()
        {
            const string xml = "<xml><eins/><zwei/></xml>";
            XmlDocument xmldoc = xml.ToXmlDocument();

            var first = xmldoc.DocumentElement.FirstChild;
            var second = xmldoc.DocumentElement.LastChild;

            string res = NodeMover.Move(second, first);
            const string exp = "<xml><eins><zwei /></eins></xml>";

            StringAssert.Contains(res, exp);
        }

        [TestMethod]
        public void Move2Test()
        {
            const string xml = "<xml><eins/><zwei/></xml>";
            XmlDocument xmldoc = xml.ToXmlDocument();

            var first = xmldoc.DocumentElement.FirstChild;
            var second = xmldoc.DocumentElement.LastChild;

            string res = NodeMover.Move(first, second);
            const string exp = "<xml><zwei><eins /></zwei></xml>";

            StringAssert.Contains(res, exp);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Move3Test()
        {
            const string xml = "<xml><eins/><zwei/></xml>";
            XmlDocument xmldoc = xml.ToXmlDocument();

            var first = xmldoc.DocumentElement.FirstChild;
            var second = xmldoc.DocumentElement.LastChild;

            string res = NodeMover.Move(first, first);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void Move4Test()
        {
            const string xml = "<xml><eins/><zwei/></xml>";
            XmlDocument xmldoc = xml.ToXmlDocument();

            var root = xmldoc.DocumentElement;
            var first = xmldoc.DocumentElement.FirstChild;

            string res = NodeMover.Move(root, first);
        }

        [TestMethod]
        public void Move5Test()
        {
            const string xml = "<xml><eins/><zwei/></xml>";
            XmlDocument xmldoc = xml.ToXmlDocument();

            var root = xmldoc.DocumentElement;
            var first = xmldoc.DocumentElement.FirstChild;

            string res = NodeMover.Move(first, root);

            const string exp = "<xml><zwei /><eins /></xml>";
            StringAssert.Contains(res, exp);
        }

        [TestMethod]
        public void InsertBeforeTest()
        {
            const string xml = "<xml><eins/><zwei/></xml>";
            XmlDocument xmldoc = xml.ToXmlDocument();

            var root = xmldoc.DocumentElement;
            var first = xmldoc.DocumentElement.FirstChild;
            var second = xmldoc.DocumentElement.LastChild;

            string res = NodeMover.InsertBefore(second, first);
            const string exp = "<xml><zwei /><eins /></xml>";
            StringAssert.Contains(res, exp);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void InsertBefore2Test()
        {
            const string xml = "<xml><eins/><zwei/></xml>";
            XmlDocument xmldoc = xml.ToXmlDocument();

            var root = xmldoc.DocumentElement;
            var first = xmldoc.DocumentElement.FirstChild;
            var second = xmldoc.DocumentElement.LastChild;

            string res = NodeMover.InsertBefore(second, root);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void InsertBefore3Test()
        {
            const string xml = "<xml><eins/><zwei/></xml>";
            XmlDocument xmldoc = xml.ToXmlDocument();

            var root = xmldoc.DocumentElement;
            var first = xmldoc.DocumentElement.FirstChild;
            var second = xmldoc.DocumentElement.LastChild;

            string res = NodeMover.InsertBefore(root, second);
        }

        [TestMethod]
        public void MoveUpTest()
        {
            const string xml = "<xml><eins/><zwei/></xml>";
            XmlDocument xmldoc = xml.ToXmlDocument();

            var root = xmldoc.DocumentElement;
            var first = xmldoc.DocumentElement.FirstChild;
            var second = xmldoc.DocumentElement.LastChild;

            string res = NodeMover.MoveUp(second);
            const string exp = "<xml><zwei /><eins /></xml>";
            StringAssert.Contains(res, exp);
        }

        [TestMethod]
        [ExpectedException(typeof(Exception))]
        public void MoveUp2Test()
        {
            const string xml = "<xml><eins/><zwei/></xml>";
            XmlDocument xmldoc = xml.ToXmlDocument();

            var root = xmldoc.DocumentElement;
            var first = xmldoc.DocumentElement.FirstChild;
            var second = xmldoc.DocumentElement.LastChild;

            string res = NodeMover.MoveUp(first);
        }

    }
}