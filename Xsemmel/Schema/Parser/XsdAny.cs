using System.Text;
using System.Xml;

namespace XSemmel.Schema.Parser 
{

	public class XsdAny : XsdNode, IXsdCountable
    {

        public XsdAny(XmlNode node) : base(node)
	    {
	        {
                string attr = VisualizerHelper.GetAttr(node, "maxOccurs");
                if (attr != null)
                {
                    MaxOccurs = attr;
                }            
	        }
	        {
	            string attr = VisualizerHelper.GetAttr(node, "minOccurs");
                if (attr != null)
                {
                    MinOccurs = attr;
                }
            }
            {
                string attr = VisualizerHelper.GetAttr(node, "namespace");
                if (attr != null)
                {
                    Namespace = attr;
                }
            }
            {
                string attr = VisualizerHelper.GetAttr(node, "processContents");
                if (attr != null)
                {
                    ProcessContents = attr;
                }            
	        }
        }

        public string Namespace { get; set; }
        public string ProcessContents { get; set; }
        public string MinOccurs { get; set; }
        public string MaxOccurs { get; set; }

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder("<Any> ");
            VisualizerHelper.ToStringCountable(this,sb);
            return sb.ToString();
        }

    }
}