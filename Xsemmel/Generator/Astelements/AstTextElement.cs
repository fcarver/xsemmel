using System.IO;
using XSemmel.Generator.Datatypes;

namespace XSemmel.Generator.Astelements {


    /**
     * Dieses AstElement repräsentiert ein XML-Textelement.
     * @author Frank Schnitzer
     */
    public class AstTextElement : AstElement {

	    private DataType texttype;

	    public AstTextElement(DataType texttype, Repeater repeater) : base(repeater) {
		    this.texttype = texttype;
	    }

	    internal override void printBefore(TextWriter w) {
		    texttype.write(w);
	    }

	    internal override void printAfter(TextWriter w) {
		    //Ein AstTextElement kann keine Kinder haben, somit ist printAfter unnötig.
	    }
    }

}