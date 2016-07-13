using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Xml;
using TaskDialogInterop;

namespace XSemmel.Schema.Parser
{

    public class SchemaParser
    {

        private readonly XsdNode _virtualRoot;

        private readonly string _prefix = "xs:";

        private readonly string _filename;

        private class SchemaMap
        {
            private readonly IDictionary<string, SchemaParser> _map = new Dictionary<string, SchemaParser>();

            public bool OpenAllDontAsk
            {
                get; set; 
            }

            public void Add(string schema, SchemaParser parser)
            {
                if (!_map.ContainsKey(schema))
                {
                    _map.Add(schema, parser);    
                }
            }

            public SchemaParser Get(string schema)
            {
                SchemaParser sp;
                if (_map.TryGetValue(schema, out sp))
                {
                    return sp;
                }
                return null;
            }
        }

        private SchemaParser(string schemafile, SchemaMap map)
        {
            _filename = schemafile;

            XmlDocument doc = new XmlDocument();
            doc.Load(schemafile);

            XmlNode e = doc.DocumentElement;

            if (!e.Name.EndsWith("schema"))
            {
                throw new Exception("No schema");
            }

            _prefix = e.Name.Substring(0, e.Name.Length - 6);

            _virtualRoot = new XsdSchema(e);
            parse(e, _virtualRoot);

            expandImports(map);
            expandReferences();
        }

        public SchemaParser(string schemafile, bool loadImportedSchemas)
            : this(schemafile, loadImportedSchemas ? new SchemaMap() : null)
        {
        }

        private IXsdNode getNodeByName(string name)
        {
            //ATTENTION: Not namespace aware!
            if (string.IsNullOrEmpty(name))
            {
                return null;
            }
            name = removeNamespace(name);
            foreach (IXsdNode node in GetAllNodes())
            {
                IXsdHasName element = node as IXsdHasName;
                if (element != null && element.Name != null)
                {
                    string elementName = removeNamespace(element.Name);
                    if (name.Equals(elementName))
                    {
                        return (IXsdNode)element;
                    }
                }
            }
            return null;
        }

        private string removeNamespace(string s)
        {
            string elementName = s.Trim();
            if (elementName.Contains(":"))
            {
                int idx = elementName.IndexOf(":");
                elementName = elementName.Substring(idx + 1);
                elementName = elementName.Trim();
            }
            return elementName;
        }

        private void expandImports(SchemaMap map)
        {
            foreach (IXsdNode node in GetAllNodes())
            {
                XsdImportInclude inc = node as XsdImportInclude;
                if (inc != null)
                {
                    string sl = inc.SchemaLocation;
                    if (map != null)
                    {
                        if (!(sl.Contains("/") || sl.Contains("\\")))
                        {
                            sl = Path.Combine(Path.GetDirectoryName(_filename), sl);
                        }
                        SchemaParser schema = map.Get(sl);
                        if (schema == null)
                        {
                            if (map.OpenAllDontAsk)
                            {
                                schema = new SchemaParser(sl, map);
                                map.Add(sl, schema);
                            }
                            else
                            {
                                Cursor cursor = Mouse.OverrideCursor;
                                Mouse.OverrideCursor = null;
                                TaskDialogResult res = TaskDialog.Show(
                                    new TaskDialogOptions
                                    {
                                        AllowDialogCancellation = true,
                                        Owner = Application.Current.MainWindow,
                                        Title = "Update xsd location",
                                        MainInstruction =
                                            "Open schema " + sl + " ?",
                                        //Content = "Old location:\n" + Data.ValidationData.Xsd + "\nUpdated location:\n" + newXsdLocation,
                                        CommandButtons = new[] { "Yes", "Yes, all", "No" },
                                        MainIcon = VistaTaskDialogIcon.Information,
                                        //ExpandedInfo = "Source: " + source.XPath + "\nTarget: " + target.XPath
                                    });

                                switch (res.CommandButtonResult)
                                {
                                    case 0: //yes
                                        schema = new SchemaParser(sl, map);
                                        map.Add(sl, schema);
                                        break;
                                    case 1: //open all
                                        map.OpenAllDontAsk = true;
                                        schema = new SchemaParser(sl, map);
                                        map.Add(sl, schema);
                                        break;
                                    case 2: //no
                                    case null: //cancel
                                        break;
                                }
                                Mouse.OverrideCursor = cursor;
                            }
                        }
                        if (schema != null)
                        {
                            var xn = schema.GetVirtualRoot();
                            ((XsdNode)node).AddKids(xn);
                        }
                    }
                }
            }
        }

        private void expandReferences()
        {
            foreach (IXsdNode node in GetAllNodes())
            {
                if (node is IXsdHasAttribute)
                {
                    ICollection<XsdAttribute> col = ((IXsdHasAttribute) node).GetAttributes();
                    foreach (XsdAttribute xsdAttribute in col)
                    {
                        if (xsdAttribute.TypeTarget == null)
                        {
                            xsdAttribute.TypeTarget = getNodeByName(xsdAttribute.TypeName);
                        }
                    }
                }

                IXsdHasRef refable = node as IXsdHasRef;
                if (refable != null && refable.RefTarget == null)
                {
                    refable.RefTarget = getNodeByName(refable.RefName);
                }
                IXsdHasType typeable = node as IXsdHasType;
                if (typeable != null && typeable.TypeTarget == null)
                {
                    typeable.TypeTarget = getNodeByName(typeable.TypeName);
                }
                XsdExtension extension = node as XsdExtension;
                if (extension != null && extension.BaseTarget == null)
                {
                    extension.BaseTarget = getNodeByName(extension.Base);
                }
            }
        }

        public IXsdNode GetVirtualRoot() 
        {
            return _virtualRoot;
        }

        public IList<IXsdNode> GetAllNodes() 
        {
            IList<IXsdNode> all = new List<IXsdNode>();
            return _virtualRoot.GetAll(all);
        }

        public string Filename
        {
            get { return _filename; }
        }

        /// <summary>
        /// Parses the schema and adds the found items to node
        /// </summary>
        /// <param name="schema"></param>
        /// <param name="node"></param>
        private void parse(XmlNode schema, XsdNode node) 
        {
            XmlNodeList nl = schema.ChildNodes;
            for (int i = 0; i < nl.Count; i++) 
            {
                XmlNode n = nl.Item(i);

                string name = n.Name;
                if (!name.StartsWith(_prefix))
                {
                    continue;
                }
                name = name.Substring(_prefix.Length);

                switch (name)
                {
                    case "element":
                    {
                        XsdNode newNode = new XsdElement(n);
                        node.AddKids(newNode);
                        parse(n, newNode);
                        break;    
                    }
                    case "sequence":
                    case "choice":
                    case "all":
                    {
                        XsdNode newNode = new XsdAllSequenceChoice(n, name);
                        node.AddKids(newNode);
                        parse(n, newNode);
                        break;    
                    }
                    case "documentation":
                    {
                        //last element
                        node.AddAnnotation(n.InnerText);
                        break;    
                    }
                    case "attribute":
                    {
                        XsdAttribute newAttr = new XsdAttribute(n);
                        ((IXsdHasAttribute)node).AddAtts(newAttr);
                        parse(n, newAttr);
                        break;    
                    }
                    case "complexType":
                    {
                        if (!string.IsNullOrEmpty(VisualizerHelper.GetAttr(n, "name")))
                        {
                            XsdComplexType newType = new XsdComplexType(n);
                            node.AddKids(newType);
                            parse(n, newType);
                        }
                        else
                        {
                            parse(n, node);
                        }
                        break;
                    }
                    case "simpleType":
                    {
                        if (!string.IsNullOrEmpty(VisualizerHelper.GetAttr(n, "name")))
                        {
                            XsdSimpleType newType = new XsdSimpleType(n);
                            node.AddKids(newType);
                            parse(n, newType);
                        }
                        else
                        {
                            parse(n, node);
                        }
                        break;
                    }
                    case "annotation":
                    {
                        parse(n, node);
                        break;		                    
                    }
                    case "restriction":
                    {
                        XsdNode newNode = new XsdRestriction(n);
                        node.AddKids(newNode);
                        parse(n, newNode);
                        break;    
                    }
                    case "enumeration":
                    {
                        string value = VisualizerHelper.GetAttr(n, "value");
                        ((XsdRestriction)node).AddEnum(value);
                        break;		                    
                    }
                    case "pattern":
                    {
                        string value = VisualizerHelper.GetAttr(n, "value");
                        ((XsdRestriction)node).Pattern = value;
                        break;
                    }
                    case "length":
                    {
                        string value = VisualizerHelper.GetAttr(n, "value");
                        ((XsdRestriction)node).Length = value;
                        break;
                    }
                    case "maxLength":
                    {
                        string value = VisualizerHelper.GetAttr(n, "value");
                        ((XsdRestriction)node).MaxLength = value;
                        break;
                    }
                    case "minLength":
                    {
                        string value = VisualizerHelper.GetAttr(n, "value");
                        ((XsdRestriction)node).MinLength = value;
                        break;
                    }
                    case "maxInclusive":
                    {
                        string value = VisualizerHelper.GetAttr(n, "value");
                        ((XsdRestriction)node).MaxInclusive = value;
                        break;
                    }
                    case "maxExclusive":
                    {
                        string value = VisualizerHelper.GetAttr(n, "value");
                        ((XsdRestriction)node).MaxExclusive = value;
                        break;
                    }
                    case "minInclusive":
                    {
                        string value = VisualizerHelper.GetAttr(n, "value");
                        ((XsdRestriction)node).MinInclusive = value;
                        break;
                    }
                    case "minExclusive":
                    {
                        string value = VisualizerHelper.GetAttr(n, "value");
                        ((XsdRestriction)node).MinExclusive = value;
                        break;
                    }
                    case "notation":
                    case "schema":
                    case "appInfo":
                    case "group":
                    case "#comment":
                    case "#text":
                    case "key":
                    case "keyref":
                    case "unique":
                        //ignore
                        break;
                    case "any":
                    {
                        XsdNode newNode = new XsdAny(n);
                        node.AddKids(newNode);
                        parse(n, newNode);
                        break;
                    }
                    case "import":
                    case "include":
                    {
                        XsdNode newNode = new XsdImportInclude(n, name);
                        node.AddKids(newNode);
                        parse(n, newNode);
                        break;
                    }
                    case "complexContent":
                    case "simpleContent":
                    {
                        parse(n, node);
                        break;
                    }
                    case "extension":
                    {
                        XsdNode newNode = new XsdExtension(n);
                        node.AddKids(newNode);
                        parse(n, newNode);
                        break;
                    }
                    case "anyAttribute":
                    case "attributeGroup":
                    case "field":
                    case "list":
                    case "redefine":
                    case "selector":
                    case "union":
                        Debug.Fail("Not supported: " + name);
                        Console.Error.WriteLine("Not supported: " + name);
                        break;
                    default:
                        Debug.Fail("Unknown type: " + name);
                        break;
                }
            }
        }

    }
}

