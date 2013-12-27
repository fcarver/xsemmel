using System.Diagnostics;
using System.IO;
using System.Text;
using System.Xml;
using XSemmel.Configuration;

namespace XSemmel.Helpers
{

    public static class XmlDocumentEx
    {
        public static string ToUTF8String(this XmlDocument xmldoc)
        {
            Debug.Assert(XSConfiguration.Instance.Config.Encoding is UTF8Encoding);

            Encoding enc = XSConfiguration.Instance.Config.Encoding;
            MemoryStream ms = new MemoryStream();
            using (TextWriter sw = new StreamWriter(ms, enc)) //Set encoding
            {
                xmldoc.Save(sw);
            }

            return enc.GetString(ms.ToArray());
        }

    }
}
