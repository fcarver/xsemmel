using System.Collections.Generic;
using System.Xml;

namespace XSemmel.Schema.Parser {

    public class XsdSchema : XsdNode, IXsdHasAttribute
    {

        public XsdSchema(XmlNode node) : base(node)
        {
            Name = "(Root)";
            {
                string attr = VisualizerHelper.GetAttr(node, "attributeFormDefault");
                if (attr != null)
                {
                    AttributeFormDefault = attr;
                }
            }
            {
                string attr = VisualizerHelper.GetAttr(node, "blockDefault");
                if (attr != null)
                {
                    BlockDefault = attr;
                }
            }
            {
                string attr = VisualizerHelper.GetAttr(node, "elementFormDefault");
                if (attr != null)
                {
                    ElementFormDefault = attr;
                }
            }
            {
                string attr = VisualizerHelper.GetAttr(node, "finalDefault");
                if (attr != null)
                {
                    FinalDefault = attr;
                }
            }
            {
                string attr = VisualizerHelper.GetAttr(node, "id");
                if (attr != null)
                {
                    Id = attr;
                }
            }
            {
                string attr = VisualizerHelper.GetAttr(node, "targetNamespace");
                if (attr != null)
                {
                    TargetNamespace = attr;
                }
            }
            {
                string attr = VisualizerHelper.GetAttr(node, "version");
                if (attr != null)
                {
                    Version = attr;
                }
            }
            {
                string attr = VisualizerHelper.GetAttr(node, "xml:lang");
                if (attr != null)
                {
                    XmlLang = attr;
                }
            }
        }

        public string AttributeFormDefault { get; set; }
        public string BlockDefault { get; set; }
        public string ElementFormDefault { get; set; }
        public string FinalDefault { get; set; }
        public string Id { get; set; }
        public string TargetNamespace { get; set; }
        public string Version { get; set; }
        public string XmlLang { get; set; }

        public void AddAtts(XsdAttribute attr)
        {
            //TODO
        }

        public ICollection<XsdAttribute> GetAttributes()
        {
            //TODO
            return new List<XsdAttribute>();
        }
    }
}