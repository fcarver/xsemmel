using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;
using System.Diagnostics;

namespace XSemmel
{
    public class ViewerNode
    {

        private readonly XmlNode _originalNode;
        private readonly ViewerNode _parent;
        private readonly XSDocument _document;
        private string _nodePath;
        private string _nonRecurringNodePath;
        private string _xpath;
        private List<ViewerNode> _childNodes;
        private IXmlLineInfo _lineInfo;

        private readonly object _lockObjForExpandingChildNodes = new object();

        public ViewerNode(XSDocument doc)
            : this(doc, doc.XmlDoc.DocumentElement, null)
        {
        }

        private ViewerNode(XSDocument doc, XmlNode originalNode, 
            ViewerNode parent, int occurrenceIndex = -1)
        {
            _document = doc;
           // ChildNodes = new List<ViewerNode>();
            Attributes = new List<ViewerNode>();
            NodeType = NodeType.Unknown;
            _originalNode = originalNode;
            _parent = parent;
            OccurrenceIndex = occurrenceIndex;
            Build();
        }

        private void Build()
        {
            Name = _originalNode.Name;
            LocalName = _originalNode.LocalName;
            Namespace = _originalNode.NamespaceURI;
            Value = _originalNode.Value;
            if (_originalNode is XmlElement)
            {
                NodeType = NodeType.Element;
            }
            else if (_originalNode is XmlAttribute)
            {
                NodeType = NodeType.Attribute;
                if (_originalNode.Name.EndsWith(":type"))
                {
                    AttributeType = AttributeType.Type;
                    if (!string.IsNullOrEmpty(_originalNode.Value))
                    {
                        if (_parent != null)
                        {
                            string str = _originalNode.Value;
                            string str2;
                            if (str.IndexOf(':') > -1)
                            {
                                str2 = str.Substring(str.IndexOf(':') + 1);
                            }
                            else
                            {
                                str2 = str;
                            }
                            _parent.TypeName = str2;
                        }
                    }
                }
                if (_originalNode.Name.StartsWith("xmlns"))
                {
                    AttributeType = AttributeType.Xmlns;
                }
                if (AttributeType == AttributeType.None)
                {
                    _parent.NormalAttributeCount++;
                }
            }
            if ((_originalNode.Attributes != null) && (_originalNode.Attributes.Count > 0))
            {
                for (int i = 0; i < _originalNode.Attributes.Count; i++)
                {
                    Attributes.Add(new ViewerNode(_document, _originalNode.Attributes[i], this));
                }
            }
//            if ((_originalNode.ChildNodes != null) && (_originalNode.ChildNodes.Count > 0))
//            {
//                for (int j = 0; j < _originalNode.ChildNodes.Count; j++)
//                {
//                    if (_originalNode.ChildNodes[j].Name == "#text")
//                    {
//                        Value = _originalNode.ChildNodes[j].Value;
//                    }
//                    else
//                    {
//                        int childNodeRepeatingIndex = GetChildNodeRepeatingIndex(_originalNode.ChildNodes[j], _originalNode.ChildNodes);
//                        ChildNodes.Add(new ViewerNode(_document, _originalNode.ChildNodes[j], this, childNodeRepeatingIndex));
//                    }
//                }
//            }
        }

        private void BuildXPath()
        {
            //TODO consider using XPathBuilder
            StringBuilder builder = new StringBuilder();
            if (_parent != null)
            {
                builder.Append(_parent.XPath);
            }
            builder.Append(NodePath);
            _xpath = builder.ToString();
        }

        private int GetChildNodeRepeatingIndex(XmlNode node, XmlNodeList list)
        {
            int num = 0;
            int num2 = 0;
            bool flag = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (string.Compare(list[i].LocalName, node.LocalName, true) == 0)
                {
                    num++;
                    if ((list[i] != node) && !flag)
                    {
                        num2++;
                    }
                    else
                    {
                        flag = true;
                    }
                    if ((num > 1) && flag)
                    {
                        break;
                    }
                }
            }
            if (num > 1)
            {
                return num2;
            }
            return -1;
        }

        public string ToDetailsString()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("(NodeSet/");
            builder.Append(NodeType.ToString());
            builder.Append(": ");
            builder.Append(LocalName);
            builder.Append(")");
            builder.AppendLine();
            if (!string.IsNullOrEmpty(Value))
            {
                builder.AppendLine($"Value: {Value}");
            }
            if (!string.IsNullOrEmpty(Namespace))
            {
                builder.AppendLine($"Namespace: {Namespace}");
            }
            if (NormalAttributeCount > 0)
            {
                builder.AppendLine($"Attribute Count: {NormalAttributeCount}");
            }
            if (ChildNodes.Count > 0)
            {
                builder.AppendLine($"Child Node Count: {ChildNodes.Count}");
            }
            return builder.ToString();
        }

        public override string ToString()
        {
            string str;
            switch (NodeType)
            {
                case NodeType.Attribute:
                    str = "@" + LocalName;
                    break;
                case NodeType.Element:
                    if (string.IsNullOrEmpty(TypeName))
                    {
                        str = LocalName;
                    }
                    else
                    {
                        str = $"{LocalName} [{TypeName}]";
                    }
                    break;
                default:
                    str = Name;
                    break;
            }
            if ((ChildNodes.Count == 0) && !string.IsNullOrEmpty(Value))
            {
                str = $"{str}: {Value}";
            }
            return str;
        }

        public List<ViewerNode> Attributes{ get; private set; }
      
        public AttributeType AttributeType { get; private set; }


        public List<ViewerNode> ChildNodes
        {
            get
            {
                lock (_lockObjForExpandingChildNodes)
                {
                    if (_childNodes == null)
                    {
                        _childNodes = new List<ViewerNode>();
                        if (_originalNode.ChildNodes.Count > 0)
                        {
                            for (int j = 0; j < _originalNode.ChildNodes.Count; j++)
                            {
                                if (_originalNode.ChildNodes[j].Name == "#text")
                                {
                                    Value = _originalNode.ChildNodes[j].Value;
                                }
                                else
                                {
                                    int childNodeRepeatingIndex = GetChildNodeRepeatingIndex(
                                        _originalNode.ChildNodes[j], _originalNode.ChildNodes);
                                    ChildNodes.Add(new ViewerNode(_document, _originalNode.ChildNodes[j],
                                                                  this, childNodeRepeatingIndex));
                                }
                            }
                        }

                    }
                    return _childNodes;
                }
            }
        }

        public string LocalName { get; private set; }

        public string Name { get; private set; }

        public string Namespace { get; private set; }

        public string NodePath
        {
            get
            {
                if (_nodePath == null)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append(NonRecurringNodePath);
                    if (OccurrenceIndex > -1)
                    {
                        builder.Append("[");
                        builder.Append(OccurrenceIndex + 1);
                        builder.Append("]");
                    }
                    _nodePath = builder.ToString();
                }
                return _nodePath;
            }
        }

        public NodeType NodeType { get; private set; }

        public string NonRecurringNodePath
        {
            get
            {
                if (_nonRecurringNodePath == null)
                {
                    StringBuilder builder = new StringBuilder();
                    builder.Append("/");
                    if (NodeType == NodeType.Attribute)
                    {
                        builder.Append("@");
                    }

                    if (string.IsNullOrEmpty(Namespace))
                    {
                        builder.Append(LocalName);    
                    }
                    else
                    {
                        builder.Append("*");
                        builder.AppendFormat("[local-name()='{0}' and namespace-uri()='{1}']", LocalName, Namespace);    
                    }
                    
                    _nonRecurringNodePath = builder.ToString();
                }
                return _nonRecurringNodePath;
            }
        }

        public int NormalAttributeCount { get; private set; }

        public int OccurrenceIndex { get; private set; }

        public string TypeName { get; private set; }

        public string Value { get; private set; }

        public string XPath
        {
            get
            {
                if (_xpath == null)
                {
                    BuildXPath();
                }
                return _xpath;
            }
        }

        public XmlNode OriginalNode
        {
            get { return _originalNode; }
        }


        public IXmlLineInfo LineInfo
        {
            get
            {
//                lock (_lockObjForCalculatingLineInfo)
                {
                    if (_lineInfo == null)
                    {
                        try
                        {
                            var nav = _document.Navigator;
                            XPathNavigator sel = nav.SelectSingleNode(XPath);
                            _lineInfo = sel as IXmlLineInfo;
                            Debug.Assert(_lineInfo != null);
                        }
                        catch (Exception)
                        {
                            return _lineInfo = null;
                        }
                    }

                    return _lineInfo;
                }
            }
        }


        public IXmlSchemaInfo SchemaInfo
        {
            get
            {
                //there's a b ug in XPathNavigator preventing to access nav.SelectSingleNode(XPath).SchemaInfo
                XmlNode node = _document.XmlDoc.SelectSingleNode(XPath);
                return node?.SchemaInfo;
            }
        }


    }


}
