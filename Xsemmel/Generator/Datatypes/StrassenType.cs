using System;
using System.IO;
using System.Security;

namespace XSemmel.Generator.Datatypes
{

    /**
     * Häufige Straßennamen
     * @author Frank Schnitzer
     */
    public class StrassenType : DataType {


	    private String[] BASE =
			    {
			    "Hauptstraße", "Dorfstraße", "Schulstraße", "Bahnhofstraße", "Gartenstraße",
			    "Bergstraße", "Lindenstraße", "Birkenweg", "Waldstraße", "Kirchstraße",
			    "Ringstraße",
			    "Wiesenweg", "Schillerstraße", "Goethestraße", "Mühlenweg", "Amselweg",
			    "Feldstraße",
			    "Wiesenstraße", "Jahnstraße", "Am Aportplatz", "Buchenweg", "Friedhofstraße",
			    "Eichenweg", "Finkenweg", "Ahornweg", "Mühlenstraße", "Rosenstraße",
			    "Talstraße",
			    "Erlenweg", "Blumenstraße", "Brunnenstraße", "Kirchweg", "Lindenweg",
			    "Raiffeisenstraße", "Bachstraße", "Industriestraße", "Tannenweg",
			    "Mittelstraße",
			    "Gartenweg", "Rosenweg", "Nozartstraße", "Am Bahnhof", "Lerchenweg",
			    "Waldweg",
			    "Drosselweg", "Poststraße", "Schlossstraße", "Neue Straße", "Mühlweg",
			    "Kirchplatz",
			    "Beethovenstraße", "Kirchgasse", "Burgstraße", "Schulweg",
			    "Breslauer Straße",
			    "Im Winkel", "Birkenstraße", "Meisenweg", "Lessingstraße", "Fliederweg",
			    "Kiefernweg",
			    "Grüner Weg", "Königsberger Straße", "Berliner Straße", "Fasanenweg",
			    "Parkstraße",
			    "Uhlandstraße", "Schützenstraße", "Römerstraße", "Kapellenweg",
			    "Kastanienweg",
			    "Narktplatz", "Danziger straße", "Tulpenweg", "Heideweg", "Mittelweg"
			    };

	    public static DataType constructOf(String s) {
		    if ("STRASSEN".Equals(s)) {
			    return new StrassenType();
		    }
		    return null;
	    }

	    public override void write(TextWriter w) {
		    String s = Random.oneOf(BASE);
		    s = SecurityElement.Escape(s);
		    w.Write(s);
	    }
    }
}