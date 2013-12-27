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
    public class DifferTest
    {
        private static XmlNode createDOM(XmlReader xmlFile)
        {
            XmlDocument doc = new XmlDocument();
            doc.Load(xmlFile);
            doc.Normalize();

            return doc.DocumentElement;
        }

        /// <summary>
        /// Vergleicht zwei unterschiedliche XML Dateien
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"SampleFiles\Xml\diff-b1.xml")]
        [DeploymentItem(@"SampleFiles\Xml\diff-b2.xml")]
        public void DiffDifferentTest()
        {
            string f_alt = @"diff-b1.xml";
            string f_neu = @"diff-b2.xml";

            Knoten alt = Knoten.createOf(createDOM(XmlReader.Create(f_alt)));
            Knoten neu = Knoten.createOf(createDOM(XmlReader.Create(f_neu)));

            DiffEngine diff = new DiffEngine();
            Knoten res = diff.Diff(alt, neu);

            Assert.IsNotNull(res);
            Assert.AreEqual(
                    "<fucc>" +
                    "<property name=\"launch\">" +
                    "<!-- ADDED:   hinzugefuegt (aslkdfh) -->" +
                    "</property>" +
                    "<program name=\"\">" +
                    "<argument startsWith=\"\">" +
                    "<dir recursive=\"false\" type=\"files\">" +
                    "<!-- REMOVED: match (.+\\.(cmd|exe|lnk|bat)) -->" +
                    "<!-- ADDED:   match (sdf.+\\.(cmd|exe|lnk|bat)) -->" +
                    "</dir>" +
                    "<!-- REMOVED: pathenv () -->" +
                    "<!-- ADDED:   pathenb () -->" +
                    "</argument>" +
                    "</program>" +
                    "<!-- REMOVED: alias name=\"gt\" appendParams=\"true\" () -->" +
                    "</fucc>",
                    res.ToString());
        }

        /// <summary>
        /// Vergleicht zwei identifsche XML Dateien
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"SampleFiles\Xml\diff-b1.xml")]
        public void DiffEqualTest()
        {
            string f_alt = @"diff-b1.xml";

            Knoten alt = Knoten.createOf(createDOM(XmlReader.Create(f_alt)));
            Knoten neu = Knoten.createOf(createDOM(XmlReader.Create(f_alt)));

            DiffEngine diff = new DiffEngine();
            Knoten res = diff.Diff(alt, neu);

            Assert.IsNull(res);
        }


        [TestMethod]
        public void DiffEqualShortTest()
        {
            string xml = "<xml><hallo/></xml>";

            Knoten alt = Knoten.createOf(createDOM(XmlReader.Create(xml.ToStream())));
            Knoten neu = Knoten.createOf(createDOM(XmlReader.Create(xml.ToStream())));

            DiffEngine diff = new DiffEngine();
            Knoten res = diff.Diff(alt, neu);

            Assert.IsNull(res);
        }

        /// <summary>
        /// Vergleicht zwei identische XML-Dateien
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"SampleFiles\Xml\diff-f1.xml")]
        [DeploymentItem(@"SampleFiles\Xml\diff-f2.xml")]
        public void DiffEqualFTest()
        {
            string f_alt = @"diff-f1.xml";
            string f_neu = @"diff-f2.xml";

            Knoten alt = Knoten.createOf(createDOM(XmlReader.Create(f_alt)));
            Knoten neu = Knoten.createOf(createDOM(XmlReader.Create(f_neu)));

            DiffEngine diff = new DiffEngine();
            Knoten res = diff.Diff(alt, neu);

            Assert.IsNull(res);
        }

        /// <summary>
        /// Vergleicht zwei identische XML-Dateien
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"SampleFiles\Xml\diff-c1.xml")]
        [DeploymentItem(@"SampleFiles\Xml\diff-c2.xml")]
        public void DiffEqualCTest()
        {
            string f_alt = @"diff-c1.xml";
            string f_neu = @"diff-c2.xml";

            Knoten alt = Knoten.createOf(createDOM(XmlReader.Create(f_alt)));
            Knoten neu = Knoten.createOf(createDOM(XmlReader.Create(f_neu)));

            DiffEngine diff = new DiffEngine();
            Knoten res = diff.Diff(alt, neu);

            Assert.IsNull(res);
        }

        /// <summary>
        /// Vergleicht zwei XML-Dateien. Eine der XML-Dateien enthält einen Zweig mehr als die andere.
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"SampleFiles\Xml\diff-d1.xml")]
        [DeploymentItem(@"SampleFiles\Xml\diff-d2.xml")]
        public void DiffAddedTest()
        {
            string f_alt = @"diff-d1.xml";
            string f_neu = @"diff-d2.xml";

            Knoten alt = Knoten.createOf(createDOM(XmlReader.Create(f_alt)));
            Knoten neu = Knoten.createOf(createDOM(XmlReader.Create(f_neu)));

            DiffEngine diff = new DiffEngine();
            Knoten res = diff.Diff(alt, neu);

            Assert.IsNotNull(res);
            Assert.AreEqual(
                    "<datenmodell>" +
                    "<tabellen>" +
                    "<table>" +
                    "<columns><!-- ADDED:   column () --></columns>" +
                    "</table>" +
                    "</tabellen>" +
                    "</datenmodell>",
                    res.ToString());
        }

        /// <summary>
        /// Vergleicht zwei XML-Dateien, die sich nur durch einen move unterscheiden
        /// </summary>
        [TestMethod]
        [DeploymentItem(@"SampleFiles\Xml\diff-a1.xml")]
        [DeploymentItem(@"SampleFiles\Xml\diff-a2.xml")]
        public void DiffEqualMovesTest()
        {
            string f_alt = @"diff-a1.xml";
            string f_neu = @"diff-a2.xml";

            Knoten alt = Knoten.createOf(createDOM(XmlReader.Create(f_alt)));
            Knoten neu = Knoten.createOf(createDOM(XmlReader.Create(f_neu)));

            DiffEngine diff = new DiffEngine();
            Knoten res = diff.Diff(alt, neu);

            Assert.IsNull(res);
        }

    }
}