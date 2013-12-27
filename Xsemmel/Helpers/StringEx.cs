using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace XSemmel.Helpers
{
    /// <summary>
    /// Extension methods for strings
    /// </summary>
    public static class StringEx
    {
        /// <summary>
        /// http://stackoverflow.com/questions/444798/case-insensitive-containsstring
        /// </summary>
        /// <param name="source"></param>
        /// <param name="toCheck"></param>
        /// <param name="comp"></param>
        /// <returns></returns>
        public static bool Contains(this string source, string toCheck, StringComparison comp)
        {
            return source.IndexOf(toCheck, comp) >= 0;
        }

        /// <summary>
        /// Creates a Stream and considers the encoding
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static Stream ToStream(this string source)
        {
            Regex getEncoding = new Regex(@"<\?.*encoding=\""(.+)\""\?>");
            Match m = getEncoding.Match(source);

            byte[] encodedString;
            if (m.Success)
            {
                var group = m.Groups[1];
                var encoding = group.Value;

                Encoding enc = Encoding.GetEncoding(encoding);
                encodedString = enc.GetBytes(source);
            }
            else
            {
                //XmlReader unterstützt erst ab .net 4.0 Stream in Unicode-Encoding; zuvor nur UTF-8
                encodedString = Encoding.Unicode.GetBytes(source);
            }

            MemoryStream ms = new MemoryStream(encodedString);
            ms.Flush();
            ms.Position = 0;
            return ms;
        }

        /// <summary>
        /// Creates a XmlDocument and considers the encoding
        /// </summary>
        /// <param name="source"></param>
        /// <param name="preserveWhitespace"></param>
        /// <returns></returns>
        public static XmlDocument ToXmlDocument(this string source, bool preserveWhitespace = true)
        {
            XmlDocument xmldoc = new XmlDocument { PreserveWhitespace = preserveWhitespace };

            Regex getEncoding = new Regex(@"<\?.*encoding=\""(.+)\""\?>");
            Match m = getEncoding.Match(source);

            if (m.Success)
            {
                var group = m.Groups[1];
                var encoding = group.Value;

                Encoding enc = Encoding.GetEncoding(encoding);
                // Encode the XML string in a UTF-8 byte array
                byte[] encodedString = enc.GetBytes(source);

                // Put the byte array into a stream and rewind it to the beginning
                MemoryStream ms = new MemoryStream(encodedString);
                ms.Flush();
                ms.Position = 0;

                // Build the XmlDocument from the MemorySteam of encoded bytes
                //  XmlDocument xmlDoc = new XmlDocument();
                xmldoc.Load(ms);
            }
            else
            {
                xmldoc.LoadXml(source);
            }

            return xmldoc;
        }

    }
}
