using System.Collections.Generic;
using System.IO;

namespace XSemmel.Generator.Astelements {


    /**
     * Diese Klasse repräsentiert einen abstrakten Knoten eines XML-Elements.
     * @author Frank Schnitzer
     */
    public abstract class AstElement {

	    private readonly List<AstElement> children = new List<AstElement>();

	    protected readonly Repeater repeater;

	    protected AstElement(Repeater repeater) {
		    this.repeater = repeater;
	    }

	    public Repeater getRepeater() {
		    return repeater;
	    }

	    public void addChild(AstElement l) {
		    children.Add(l);
	    }

	    public List<AstElement> getChildred() {
		    return children;
	    }

	    public AstElement getLastChild() {
		    return children[children.Count - 1];
	    }

	    /**
	     * Dieses AstElement als XML ausgeben.
	     * @param w Der Writer, in den das XML geschrieben werden soll.
	     * @throws Exception
	     */
	    public virtual void generateXML(TextWriter w) {
		    List<AstElement> kids = getChildred();

		    printBefore(w);

		    foreach (AstElement kid in kids) {
			    Repeater r = kid.getRepeater();
			    r.run( i =>  {
					    kid.generateXML(w);
				    }
			    );
		    }

		    printAfter(w);
	    }

	    /**
	     * Diese Methode wird aufgerufen, bevor das XML der Kindelemente geschrieben wird.
	     * @param w Der Writer, in den Zeichen geschrieben werden sollen.
	     * @throws IOException
	     * @see #generateXML(Writer)
	     */
        internal abstract void printBefore(TextWriter w);

	    /**
	     * Diese Methode wird aufgerufen, nachdem das XML der Kindelemente geschrieben wird.
	     * @param w Der Writer, in den Zeichen geschrieben werden sollen.
	     * @throws IOException
	     * @see #generateXML(Writer)
	     */
        internal abstract void printAfter(TextWriter w);

    }

}