using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using XSemmel.Schema.Parser;

namespace XSemmel.Schema
{
    internal static class VisualizerHelper
    {

        internal static string GetAttr(XmlNode n, string name)
        {
            XmlAttributeCollection nnm = n.Attributes;
            XmlNode att = nnm[name];
            if (att == null)
            {
                return null;
            }
            return att.Value;
        }

        internal static void ToStringCountable(IXsdCountable node, StringBuilder sb)
        { 
            const string UNBOUNDED = "unbounded";
            if (!string.IsNullOrEmpty(node.MinOccurs) || !string.IsNullOrEmpty(node.MaxOccurs))
            {
                sb.Append(", ");
                if (UNBOUNDED.Equals(node.MinOccurs))
                {
                    sb.Append('∞');
                }
                else
                {
                    sb.Append(node.MinOccurs ?? "");
                }

                sb.Append(":");
                if (UNBOUNDED.Equals(node.MaxOccurs))
                {
                    sb.Append('∞');
                }
                else
                {
                    sb.Append(node.MaxOccurs ?? "");
                }
            }
        }

    }
}
