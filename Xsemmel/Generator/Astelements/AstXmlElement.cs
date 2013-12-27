using System.Collections.Generic;
using System.IO;
using System.Text;
using XSemmel.Generator.Datatypes;

namespace XSemmel.Generator.Astelements
{

    /**
     * Dieses AstElement repräsentiert ein "normales" XML-Element.
     * @author Frank Schnitzer
     */
    public class AstXmlElement : AstElement {

	    private DataType element;
	    private Dictionary<DataType, DataType> attributes;
	    private StringWriter cache;

	    private int deepness;

	    public AstXmlElement(
			    int deepness,
			    DataType element,
			    Dictionary<DataType, DataType> attributes,
			    Repeater repeater) : base(repeater) {
		    this.element = element;
		    this.attributes = attributes;
		    this.deepness = deepness;
	    }

	    internal override void printBefore(TextWriter w) {
		    w.Write('\n');
		    w.Write(ind(deepness));
		    w.Write('<');

		    //Ein DataType kann bei jedem write-Aufruf ein anderes Ergebnis liefern. 
		    //Damit printAfter dasselbe Element schreibt, wird das hier gelieferte Element gecacht.
		    cache = new StringWriter();
		    element.write(cache);

		    w.Write(cache.ToString());

		    foreach (var attr in attributes) {
			    w.Write(' ');
			    attr.Key.write(w);
			    w.Write("='");
			    attr.Value.write(w);
			    w.Write("'");
		    }

		    w.Write('>');
	    }


        /**
         * Liefert einen String mit Leerzeichen. Die Anzahl der Leerzeichen berechnet sich wie folgt:
         * 4*deepness.
         *
         * @param deepness Ein Wert >=0
         * @return (erwähnt)
         */
        public static string ind(int deepness)
        {
            //TODO Methode in Utility-Klasse verschieben
            StringBuilder sb = new StringBuilder();
            for (int i = 1; i < deepness; i++)
            {
                sb.Append("    ");
            }
            return sb.ToString();
        }

	    internal override void printAfter(TextWriter w) {
		    w.Write("</");
		    w.Write(cache.ToString());
		    w.Write('>');
	    }

    }
}