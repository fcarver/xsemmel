﻿using System;
using System.Diagnostics;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.IO;
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


        public XSDocument(string xml)
        {
            Debug.Assert(xml != null);
            if (xml == null)
            {
                throw new ArgumentNullException("xml");
            }
            Xml = xml;

            try
            {
                string xsd = GetEmbeddedXsdFile();
                XsdFile = xsd;
            }
            catch (Exception)
            {
            }
        }

        public XSDocument(string xml, string xsd) : this(xml)
        {
            XsdFile = xsd;
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
                    throw new ArgumentNullException("value");
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
                    XmlDocument document = new XmlDocument() { PreserveWhitespace = true };
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
//                try
//                {
//                    if (_xsdFile == null)
//                    {
//                        string xsd = GetEmbeddedXsdFile();
//                        if (File.Exists(xsd))
//                        {
//                            _xsdFile = xsd;
//                        }
//                    }
//                    return _xsdFile;
//                }
//                catch (Exception)
//                {
//                    return null;
//                }

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