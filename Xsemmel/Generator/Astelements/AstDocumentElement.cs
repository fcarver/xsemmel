using System;
using System.Collections.Generic;
using System.IO;

namespace XSemmel.Generator.Astelements {


    /**
     * Dieses AstElement repräsentiert das Document-Element. 
     * @author Frank Schnitzer
     */
    public class AstDocumentElement : AstElement {

	    private String baseFile;

	    public AstDocumentElement(String outputFile) : base(null) {

		    if (!outputFile.ToLower().EndsWith(".xml")) {
			    outputFile += ".xml";
		    }

		    int idx = outputFile.Length - 4;

		    String s1 = outputFile.Substring(0, idx);
		    String s2 = outputFile.Substring(idx);

		    baseFile = s1 + "$$" + s2; //"$$" wird durch eine fortlaufende Zahl ersetzt
	    }

	    public override void generateXML(TextWriter w) {
		    //Da es nur ein Rootelement geben kann, sollen mehrere Kinder zu mehreren Dateien führen

		    List<AstElement> kids = getChildred();

		    foreach (AstElement kid in kids) {
			    Repeater r = kid.getRepeater();
			    r.run(i =>  {
					              string file = baseFile.Replace("$$", i.ToString());
			                      Console.WriteLine("Schreibe " + file);

                        using (TextWriter fos = new StreamWriter(new FileStream(file, FileMode.Create)))
                        {
                            kid.generateXML(fos);
                        }
				    }
			    );
		    }

	    }

	    internal override void printAfter(TextWriter w) {
		    //
	    }

	    internal override void printBefore(TextWriter w) {
		    //        
	    }
    }

}