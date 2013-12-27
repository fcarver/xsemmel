using System;
using System.IO;

namespace XSemmel.Generator.Datatypes
{


    /**
     * Ein Boolscher Wert
     * @author Frank Schnitzer
     */
    public class BoolType : DataType {

	    public static DataType constructOf(String s) {
		    if ("BOOL".Equals(s)) {
			    return new BoolType();
		    }
		    return null;
	    }

	    public override void write(TextWriter w) {
		    double rand = new System.Random().NextDouble();
		    if (rand < 0.5) {
			    w.Write("true");
		    } else {
                w.Write("false");
		    }
	    }

    }
}