using System.IO;
using System.Text;
using System.Xml;
using XSemmel.Configuration;
using XSemmel.Helpers;

namespace XSemmel.PrettyPrinter
{
    static class PrettyPrint
    {

        public static string Execute(string xml, bool indent, bool newLineOnAttributes, bool omitXmlDeclaration = false)
        {
            XmlDocument doc = xml.ToXmlDocument(false);
            
            Encoding enc = XSConfiguration.Instance.Config.Encoding;
            XmlNode declNode = doc.DocumentElement.PreviousSibling;
            if (declNode != null)
            {
                XmlDeclaration decl = declNode as XmlDeclaration;
                if (decl != null)
                {
                    if (!string.IsNullOrEmpty(decl.Encoding))
                    {
                        enc = Encoding.GetEncoding(decl.Encoding);
                        if (enc is UTF8Encoding)
                        {
                            enc = new UTF8Encoding(false);
                        }
                    }
                }
            }
            
            XmlWriterSettings settings = new XmlWriterSettings
            {
                ConformanceLevel = ConformanceLevel.Auto,
                CheckCharacters = true,
                Indent = indent,
                NewLineOnAttributes = newLineOnAttributes,
                Encoding = enc,
                OmitXmlDeclaration = omitXmlDeclaration
            };

            MemoryStream ms = new MemoryStream();
            using (XmlWriter w = XmlWriter.Create(ms, settings))
            {
                doc.WriteTo(w);
            }

            return enc.GetString(ms.ToArray());
        }

    }
}
