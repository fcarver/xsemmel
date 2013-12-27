namespace XSemmel.Helpers
{
    public static class XmlEscape
    {

        public static string UnescapeXml(string xmlString)
        {
            xmlString = xmlString.Replace("&amp;", "&");
            xmlString = xmlString.Replace("&lt;", "<");
            xmlString = xmlString.Replace("&gt;", ">");
            xmlString = xmlString.Replace("&quot;", "\"");
            xmlString = xmlString.Replace("&apos;", "'");
            xmlString = xmlString.Replace("&#39;", "'");
            return xmlString;
        }

        public static string EscapeXml(string xmlString)
        {
            xmlString = xmlString.Replace("&", "&amp;");
            xmlString = xmlString.Replace("<", "&lt;");
            xmlString = xmlString.Replace(">", "&gt;");
            xmlString = xmlString.Replace("\"", "&quot;");
            xmlString = xmlString.Replace("'", "&apos;");
            xmlString = xmlString.Replace("'", "&#39;");
            return xmlString;
        }
    }
}
