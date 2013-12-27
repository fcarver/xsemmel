using System;
using System.Xml;

namespace XSemmel.Helpers
{
    public static class NodeMover
    {
        public static string Move(XmlNode source, XmlNode target)
        {
            if (source.Equals(target))
            {
                throw new Exception("Cannot move node to itself");
            }
            if (source.ParentNode == null || source.ParentNode.NodeType == XmlNodeType.Document)
            {
                throw new Exception("Cannot move root node and attributes");
            }
            source.ParentNode.RemoveChild(source);
            target.AppendChild(source);
            return target.OwnerDocument.ToUTF8String();
        }

        public static string InsertBefore(XmlNode source, XmlNode target)
        {
            if (source.Equals(target))
            {
                throw new Exception("Cannot move node to itself");
            }
            if (source.ParentNode == null || source.ParentNode.NodeType == XmlNodeType.Document)
            {
                throw new Exception("Cannot move root node and attributes");
            }
            if (target.ParentNode == null || target.ParentNode.NodeType == XmlNodeType.Document)
            {
                throw new Exception("Cannot insert before root node");
            }

            source.ParentNode.RemoveChild(source);
            target.ParentNode.InsertBefore(source, target);
            return target.OwnerDocument.ToUTF8String();
        }

        public static string InsertAfter(XmlNode source, XmlNode target)
        {
            if (source.Equals(target))
            {
                throw new Exception("Cannot move node to itself");
            }
            if (source.ParentNode == null || source.ParentNode.NodeType == XmlNodeType.Document)
            {
                throw new Exception("Cannot move root node and attributes");
            }
            if (target.ParentNode == null || target.ParentNode.NodeType == XmlNodeType.Document)
            {
                throw new Exception("Cannot insert before root node");
            }

            source.ParentNode.RemoveChild(source);
            target.ParentNode.InsertAfter(source, target);
            return target.OwnerDocument.ToUTF8String();
        }


        public static string MoveUp(XmlNode node)
        {
            if (node.ParentNode == null || node.ParentNode.NodeType == XmlNodeType.Document)
            {
                throw new Exception("Cannot move root node and attributes");
            }
            if (node.Equals(node.ParentNode.FirstChild) || node.PreviousSibling == null)
            {
                throw new Exception("Node is already first node");
            }

            XmlNode prevSibling = node.PreviousSibling;
            while (prevSibling.NodeType == XmlNodeType.Whitespace)
            {
                prevSibling = prevSibling.PreviousSibling;
                if (prevSibling == null)
                {
                    throw new Exception("Node is already first node");
                }
            }
            XmlNode parent = node.ParentNode;

            node.ParentNode.RemoveChild(node);
            parent.InsertBefore(node, prevSibling);
            return node.OwnerDocument.ToUTF8String();
        }

        public static string MoveDown(XmlNode node)
        {
            if (node.ParentNode == null || node.ParentNode.NodeType == XmlNodeType.Document)
            {
                throw new Exception("Cannot move root node and attributes");
            }
            if (node.Equals(node.ParentNode.LastChild) || node.NextSibling == null)
            {
                throw new Exception("Node is already last node");
            }

            XmlNode nextSibling = node.NextSibling;
            while (nextSibling.NodeType == XmlNodeType.Whitespace)
            {
                nextSibling = nextSibling.NextSibling;
                if (nextSibling == null)
                {
                    throw new Exception("Node is already last node");
                }
            }
            XmlNode parent = node.ParentNode;


            node.ParentNode.RemoveChild(node);
            parent.InsertAfter(node, nextSibling);
            return node.OwnerDocument.ToUTF8String();
        }


    }


}
