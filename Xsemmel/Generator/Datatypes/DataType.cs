using System.IO;

namespace XSemmel.Generator.Datatypes {

    /**
     * Abstrakte Klasse für einen Datentyp. Gemäß dieses Datentyps werden später konkrete
     * Zufallswerte erzeugt,
     * @author Frank Schnitzer
     */
    public abstract class DataType {

	    public static DataType get(string s) {
		    DataType type = null;
		    if (type == null) {
			    type = HardCodedType.constructOf(s);
		    }
		    if (type == null) {
			    type = FloatType.constructOf(s);
		    }
		    if (type == null) {
			    type = IntType.constructOf(s);
		    }
		    if (type == null) {
			    type = BoolType.constructOf(s);
		    }
		    if (type == null) {
			    type = StringType.constructOf(s);
		    }
		    if (type == null) {
			    type = UuidType.constructOf(s);
		    }
		    if (type == null) {
			    type = IncnumberType.constructOf(s);
		    }
		    if (type == null) {
			    type = VornamenType.constructOf(s);
		    }
		    if (type == null) {
			    type = NachnamenType.constructOf(s);
		    }
		    if (type == null) {
			    type = StrassenType.constructOf(s);
		    }
		    if (type == null) {
			    type = HauptstaedteType.constructOf(s);
		    }
		    if (type == null) {
			    type = StaatenType.constructOf(s);
		    }

		    //TODO Datentypen: SHORTSTRING (kurzer oder leerer String), LONGSTRING (Prosatext)

		    return type;
	    }

	    public abstract void write(TextWriter w);

    }
}