using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Xsl;

namespace XSemmel.Xslt
{
    class XsltTransformer
    {

        class MyXmlUrlResolver : XmlUrlResolver
        {

            public override Object GetEntity(
	            Uri absoluteUri,
	            string role,
	            Type ofObjectToReturn
            )
            {
                return base.GetEntity(absoluteUri, role, ofObjectToReturn);
            }

        }

        public string Transform(XmlDocument xml, string xslt, Encoding encoding)
        {
            StringBuilder resultString = new StringBuilder();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            settings.CloseOutput = true;
            settings.ConformanceLevel = ConformanceLevel.Auto;
            settings.Encoding = encoding;

            XsltSettings xsltSettings = new XsltSettings(true, false);

            using (XmlWriter xmlWriter = XmlWriter.Create(new StringWriter(resultString), settings))
            {
                XslCompiledTransform xslTransform = new XslCompiledTransform();
                using (XmlReader xmlReader = new XmlTextReader(new StringReader(xslt)))
                {
                    xslTransform.Load(xmlReader, xsltSettings, new MyXmlUrlResolver());
                    xslTransform.Transform(xml, xmlWriter);
                }
            }

            return resultString.ToString();
        }

    }
}
