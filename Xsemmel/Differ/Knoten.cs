using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security;
using System.Text;
using System.Xml;

namespace XSemmel.Differ
{
    /// <summary>
    /// Diese Klasse repräsentiert einen XML-Knoten (Node).
    /// </summary>
    public class Knoten
    {
        /**
	     * Der Typ des Knotens
	     * @author Frank Schnitzer
	     */

        public enum KnotenTyp
        {
            /**
		    * Knoten ist in beiden XML-Dateien identisch
		    */
            ELEMENT,
            /**
		    * Knoten enthält nur Text und ist in beiden XML-Dateien identisch
		    */
            TEXT,
            /**
		    * Knoten wurde in der 2. XML-Datei gelöscht
		    */
            GELOESCHT,
            /**
		    * Knoten ist nur in der 2. XML-Datei vorhanden
		    */
            HINZUGEFUEGT
        }

        private readonly EqualsDictionary<string, string> attributes;

        private readonly string elementName;


        //	private final Set<Knoten> kinder = new HashSet<Knoten>();
        private readonly EqualsList<Knoten> kinder = new EqualsList<Knoten>();

        private readonly KnotenTyp typ;

        private readonly string text;

        /// <summary>
        /// Zeigt an, dass dieser Knoten beim traversieren ignoriert werden soll. Dies ist
        /// z.B. immer dann der Fall, wenn er bereits zum Löschen markiert ist oder wenn
        /// der Knoten ein generierter Knoten (zur Darstellung des Ergebnisses) ist.
        /// </summary>
        private bool ignoreMe = false;

        /**
	     * Initialisiert einen neuen Knoten.
	     * @param typ Der Typ
	     * @param name Der Elementname
	     * @param text Der Elementtext oder Leerstring, falls das Element keinen Text hat.
	     * @param attrs Die Attribute des Elements oder eine leere Map, falls das Element
	     * keine Attribute hat. 
	     */
        public Knoten(KnotenTyp typ, String name, String text, EqualsDictionary<string, string> attrs)
        {
            if (attrs == null)
            {
                throw new ArgumentNullException("attrs");
            }
            this.typ = typ;
            this.elementName = name;
            this.text = text;
            this.attributes = attrs;
        }

        /**
	     * Fügt den angegebenen Knoten als Kind dieses Knotens hinzu.
	     * @param kind Das neue Kind.
	     */

        public void addKind(Knoten kind)
        {
            kinder.Add(kind);
        }

        /**
	     * Ermittelt eine Stringrepräsentation für den angegebenen Knoten.
	     * @param k Der Knoten.
	     * @return Die Stringdarstellung.
	     */

        private static String getKnotenAsString(Knoten k)
        {
            StringBuilder sb = new StringBuilder();
            sb.Append(k.elementName);
            foreach (var e in k.attributes)
            {
                sb.Append(" ");
                sb.Append(e.Key);
                sb.Append("=\"");
                sb.Append(e.Value);
                sb.Append("\"");
            }

            return sb.ToString();
        }

        /**
	     * {@inheritDoc}
	     */

        public override string ToString()
        {
            StringBuilder sb = new StringBuilder();

            switch (typ)
            {
                case KnotenTyp.ELEMENT:
                    sb.Append("<");
                    sb.Append(getKnotenAsString(this));
                    sb.Append(">");
                    sb.Append(SecurityElement.Escape(this.text.Trim()));
                    foreach (Knoten kind in kinder)
                    {
                        sb.Append(kind.ToString());
                    }
                    sb.Append("</");
                    sb.Append(elementName);
                    sb.Append(">");
                    break;
                case KnotenTyp.GELOESCHT:
                    sb.Append("<!-- REMOVED: ");
                    sb.Append(getKnotenAsString(this));
                    sb.Append(" (");
                    sb.Append(SecurityElement.Escape(this.text.Trim()));
                    sb.Append(") -->");
                    break;
                case KnotenTyp.HINZUGEFUEGT:
                    sb.Append("<!-- ADDED:   ");
                    sb.Append(getKnotenAsString(this));
                    sb.Append(" (");
                    sb.Append(SecurityElement.Escape(this.text.Trim()));
                    sb.Append(") -->");
                    break;
                case KnotenTyp.TEXT:
                    sb.Append(text);
                    break;
                default:
                    Debug.Fail("Unbekannter Typ=" + typ);
                    break;
            }

            return sb.ToString();
        }

        /**
	     * {@link #ignoreMe}
	     * @return
	     */

        internal bool isIgnoreMe()
        {
            return ignoreMe;
        }

        /**
	     * {@link #ignoreMe}
	     * @return
	     */

        internal void setIgnoreMe()
        {
            ignoreMe = true;
        }

        /**
	     * Ermittelt die Kinder dieses Knotens.
	     * @return Liefert eine Liste mit den Kindern.
	     */

        public ICollection<Knoten> getKinder()
        {
            //unmodifiableList ist eigentlich unnötig, da Knoten mutable sind. Es ist
            //jedoch hilfreich beim debuggen
            return kinder.AsReadOnly();
        }

        /**
	     * Ermittelt, ob dieser Knoten Kinder besitzt
	     * @return <code>true</code>, falls Kinder vorhanden sind.
	     */

        public bool hasKinder()
        {
            return kinder.Count > 0;
        }

        /**
	     * Löscht das angegebene Kind 
	     * @param kind Das zu löschende Kind
	     */

        public void removeKind(Knoten kind)
        {
            bool success = kinder.Remove(kind);
            Debug.Assert(success, "Kind nicht gefunden: " + kind.ToString());
        }

        /**
	     * {@inheritDoc}
	     */

        public override int GetHashCode()
        {
            int prime = 31;
            int result = 1;
            result = prime*result + ((attributes == null) ? 0 : attributes.GetHashCode());
            result = prime*result + ((elementName == null) ? 0 : elementName.GetHashCode());
            result = prime*result + ((kinder == null) ? 0 : kinder.GetHashCode());
            result = prime*result + ((text == null) ? 0 : text.GetHashCode());
            result = prime*result + ((typ == null) ? 0 : typ.GetHashCode());
            return result;
        }

        /**
	     * Prüft, ob dieser und der angegebene Knoten gleich sind. Die Kinder werden dabei
	     * nicht berücksichtigt. Ebenso wird Whitespace im Text nicht berücksichtigt.
	     * @param other Der zu vergleichende Knoten.
	     * @return <code>true</code>, falls die beiden Knoten gleich sind.
	     * @see #equals(Object)
	     */

        public bool equalsWithoutChildren(Knoten other)
        {
            if (GetType() != other.GetType())
            {
                Debug.Fail("");
                return false;
            }
            if (attributes == null)
            {
                if (other.attributes != null)
                {
                    return false;
                }
            }
            else if (!attributes.Equals(other.attributes))
            {
                return false;
            }
            if (elementName == null)
            {
                if (other.elementName != null)
                {
                    return false;
                }
            }
            else if (!elementName.Equals(other.elementName))
            {
                return false;
            }
            //		if (kinder == null) {
            //			if (other.kinder != null) {
            //				return false;
            //			}
            //		} else if (!kinder.equals(other.kinder)) {
            //			return false;
            //		}
            if (text == null)
            {
                if (other.text != null)
                {
                    return false;
                }
            }
            else
            {
                //Bei unterschiedlichen Kindern stimmt oft auch der Whitespace nicht
                String othertext = other.text == null ? null : other.text.Trim();
                if (!text.Trim().Equals(othertext))
                {
                    return false;
                }
            }
            if (typ == null)
            {
                if (other.typ != null)
                {
                    return false;
                }
            }
            else if (!typ.Equals(other.typ))
            {
                return false;
            }
            return true;
        }

        /**
	     * {@inheritDoc}
	     */

        public override bool Equals(Object obj)
        {
            if (this == obj)
            {
                return true;
            }
            if (obj == null)
            {
                return false;
            }
            if (GetType() != obj.GetType())
            {
                return false;
            }
            Knoten other = (Knoten) obj;
            if (attributes == null)
            {
                if (other.attributes != null)
                {
                    return false;
                }
            }
            else if (!attributes.Equals(other.attributes))
            {
                return false;
            }
            if (elementName == null)
            {
                if (other.elementName != null)
                {
                    return false;
                }
            }
            else if (!elementName.Equals(other.elementName))
            {
                return false;
            }
            if (kinder == null)
            {
                if (other.kinder != null)
                {
                    return false;
                }
            }
            else if (!kinder.Equals(other.kinder))
            {
                return false;
            }
            if (text == null)
            {
                if (other.text != null)
                {
                    return false;
                }
            }
            else if (!text.Equals(other.text))
            {
                return false;
            }
            if (typ == null)
            {
                if (other.typ != null)
                {
                    return false;
                }
            }
            else if (!typ.Equals(other.typ))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Ermittelt den (Element-)Namen dieses Knotens.
        /// </summary>
        /// <returns>Liefert den Namen</returns>
        public String getName()
        {
            return elementName;
        }

        /**
	     * Ermittelt den Text dieses Knotens.
	     * @return Liefert den Text oder einen Leerstring, falls dieser Knoten keinen
	     * Text beinhaltet.
	     */

        public String getText()
        {
            return text;
        }

        /**
	     * Ermittelt die Attribute dieses Knotens.
	     * @return Liefert die Attribute dieses Knotens in einer nicht-modifizierbaren Map.
	     * Der Key ist dabei der Name des Attributs, der Value der Wert des Attributs.
	     */

        public EqualsDictionary<string, string> getAttributes()
        {
//		    return Collections.unmodifiableMap(attributes);
            return attributes;
        }


        /**
	     * Erzeugt aus dem angegebenen Node einen Knoten. Die Kinder werden berücksichtigt.
	     * @param node Der zu konvertierende Node.
	     * @return Ein Knoten, der dem Node entspricht.
	     */

        public static Knoten createOf(XmlNode node)
        {
            Knoten parent = createKnoten(node);
            if (parent == null)
            {
                return null;
            }

            XmlNodeList kids = node.ChildNodes;

            for (int i = 0, cnt = kids.Count; i < cnt; i++)
            {
                Knoten kind = createOf(kids.Item(i));
                if (kind != null)
                {
                    parent.addKind(kind);
                }
            }

            return parent;
        }


        /**
	     * Erzeugt aus dem angegebenen Node einen Knoten. Die Kinder werden dabei <b>nicht</b>
	     * berücksichtigt.
	     * @param node Der zu konvertierende Node.
	     * @return Ein Knoten, der dem Node entspricht.
	     */

        private static Knoten createKnoten(XmlNode node)
        {
            String name = node.Name;

            String text = getTextContent(node);

            KnotenTyp typ;
            if (node.NodeType == XmlNodeType.Element)
            {
                typ = KnotenTyp.ELEMENT;
            }
            else if (node.NodeType == XmlNodeType.Text)
            {
                typ = KnotenTyp.TEXT;
                if (string.IsNullOrEmpty(text.Trim()))
                {
                    //Whitespace ignorieren
                    return null;
                }
            }
            else
            {
                return null;
                //wird nicht unterstützt
            }

            EqualsDictionary<string, string> attrs = new EqualsDictionary<string, string>();
            XmlAttributeCollection nnm = node.Attributes;
            if (nnm != null)
            {
                Debug.Assert(typ != KnotenTyp.TEXT);
                for (int i = 0; i < nnm.Count; i++)
                {
                    XmlNode attr = nnm.Item(i);

                    String attrName = attr.Name;
                    String attrValue = attr.Value;
                    Debug.Assert(attr.NodeType == XmlNodeType.Attribute);
                    attrs.Add(attrName, attrValue);
                }
            }

            return new Knoten(typ, name, text, attrs);
        }

        /**
	     * Ermittelt den Text des angegebenen Nodes. Im Gegensatz zu {@link Node#getTextContent()}
	     * wird der Text der geschwister und Kinder nicht extrahiert.
	     * @param p_Node
	     * @return Den Text oder ein Leerstring, falls der Node keinen Text beinhaltet.
	     */

        private static String getTextContent(XmlNode p_Node)
        {
            XmlNodeList nodeLst = p_Node.ChildNodes;
            for (int s = 0; s < nodeLst.Count; s++)
            {
                XmlNode fstNode = nodeLst.Item(s);
                //TODO hier wird nur der erste TEXTNode benutzt
                if (fstNode.NodeType == XmlNodeType.Text)
                {
                    return fstNode.Value;
                }
            }
            return "";
        }
    }
}