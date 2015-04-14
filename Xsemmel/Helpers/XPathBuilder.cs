using System;
using System.Xml;

namespace XSemmel.Helpers
{

    //Source: http://stackoverflow.com/questions/241238/how-to-get-xpath-from-an-xmlnode-instance-c-sharp
    public static class XPathBuilder
    {

        public static string GetXPathToNode(XmlNode node)
        {
            if (node.NodeType == XmlNodeType.Attribute)
            {
                // attributes have an OwnerElement, not a ParentNode; also they have
                // to be matched by name, not found by position
                return String.Format(
                    "{0}/@{1}",
                    GetXPathToNode(((XmlAttribute)node).OwnerElement),
                    node.Name
                    );
            }
            if (node.ParentNode == null)
            {
                // the only node with no parent is the root node, which has no path
                return "";
            }
            // the path to a node is the path to its parent, plus "/node()[n]", where 
            // n is its position among its siblings.
            return String.Format(
                "{0}/node()[{1}]",
                GetXPathToNode(node.ParentNode),
                GetNodePosition(node)
                );
        }

        private static int GetNodePosition(XmlNode child)
        {
            for (int i = 0; i < child.ParentNode.ChildNodes.Count; i++)
            {
                if (child.ParentNode.ChildNodes[i] == child)
                {
                    // tricksy XPath, not starting its positions at 0 like a normal language
                    return i + 1;
                }
            }
            throw new InvalidOperationException("Child node somehow not found in its parent's ChildNodes property.");
        }

    }
}
