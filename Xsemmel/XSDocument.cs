using System;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.IO;
using XSemmel.Configuration;
using XSemmel.Helpers;
using XSemmel.Schema;

namespace XSemmel
{
    /// <summary>
    /// XSemmel Document
    /// </summary>
    public class XSDocument
    {

        private string _xml;
        private XmlDocument _xmlDoc;
        private XPathNavigator _navigator;
        private string _xsdFile;

        private static readonly bool _workaroundUtf8BomBug = XSConfiguration.Instance.Config.WorkaroundUtf8BomBug;

        public XSDocument(string xml, string xsdfile = null, string xmlfile = null)
        {
            if (_workaroundUtf8BomBug)
            {
                if (xml.Substring(0, Math.Min(50, xml.Length)).Contains("\"?>"))
                {
                    xml = xml.Replace("\"?>", "\" ?>");
                }
            }

            Debug.Assert(xml != null);
            if (xml == null)
            {
                throw new ArgumentNullException(nameof(xml));
            }
            Xml = xml;
            if (xmlfile != null)
            {
                Filename = xmlfile;
            }

            if (xsdfile != null)
            {
                XsdFile = xsdfile;
            }
            else
            {
                try
                {
                    string xsd2 = GetEmbeddedXsdFile();
                    XsdFile = xsd2;
                }
                catch (Exception)
                {
                    // ignored
                }
            }
        }


        public string Xml
        {
            get 
            {
                return _xml;
            }
            set 
            {
                Debug.Assert(value != null);
                if (value == null)
                {
                    throw new ArgumentNullException(nameof(value));
                }

                _xmlDoc = null;
                _navigator = null;
                _xsdFile = null;

                _xml = value;
            }
        }


        private XmlReader getXmlReader()
        {
            XmlReaderSettings settings = null;
            if (XsdFile != null)
            {
                settings = new XmlReaderSettings();
                settings.Schemas.Add(null, XsdFile);
                settings.ValidationType = ValidationType.Schema;
            }

            return XmlReader.Create(Xml.ToStream(), settings);
        }

        public string Filename
        {
            get;
            set;
        }

        public XmlDocument XmlDoc
        {
            get
            {
                if (_xmlDoc == null)
                {
                    XmlDocument document = new XmlDocument { PreserveWhitespace = true };
                    document.Load(getXmlReader());

                    if (XsdFile != null)
                    {
                        ValidationEventHandler eventHandler = (sender, e) => {};
                        document.Validate(eventHandler);
                    }

                    _xmlDoc = document;
                }
                return _xmlDoc;
            }
        }

        public XPathNavigator Navigator
        {
            get
            {
                if (_navigator == null)
                {
                    var xpathdoc = new XPathDocument(getXmlReader());
                    _navigator = xpathdoc.CreateNavigator();
                }
                return _navigator;
            }
        }


        public string GetEmbeddedXsdFile()
        {
            string currentDirectory = ".";
            if (Filename != null)
            {
                currentDirectory = Path.GetDirectoryName(Filename);
            }

            return XsdValidationHelper.GetXsdFile(Xml.ToXmlDocument(), currentDirectory);
        }


        public string XsdFile
        {
            get
            {
                return _xsdFile;
            }
            set
            {
                if (value == null || File.Exists(value))
                {
                    _xsdFile = value;
                }
                else
                {
                    _xsdFile = null;
                }

                _navigator = null;
                _xmlDoc = null;
            }
        }

    }
}
