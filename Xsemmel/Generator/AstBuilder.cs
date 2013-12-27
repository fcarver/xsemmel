using System;
using System.Collections.Generic;
using System.IO;
using XSemmel.Generator.Astelements;
using XSemmel.Generator.Datatypes;

namespace XSemmel.Generator {


    /**
     * Repräsentiert ein Muster für die XML-Erzeugung und kann durch {@link #generateXML()}
     * die Erzeugung einer XML-Datei anstoßen.
     * @author Frank Schnitzer
     */
    class AstBuilder {

	    private readonly AstElement root;

	    /**
	     * @param patternFile
	     * @throws Exception
	     */
	    internal AstBuilder(string patternFile, string outputFile) {
		    root = new AstDocumentElement(outputFile);

	        string[] file = File.ReadAllLines(patternFile);

            foreach (string s in file)
            {
			    String trimmed = s.Trim();

			    if (trimmed.StartsWith("//")) { //Kommentar
				    continue;
			    }
			    if (string.IsNullOrEmpty(trimmed)) { //leere Zeilen ignorieren
				    continue;
			    }

			    int deepness = countLeadingSpaces(s) / 4;

			    AstElement l = getOfLine(deepness, trimmed);

			    if (deepness == 0) {
				    root.addChild(l);
			    } else {
				    AstElement step = root.getLastChild();
				    for (int i = 1; i < deepness; i++) {
					    AstElement stepCheck = step.getLastChild();
					    if (stepCheck != null) {
						    step = stepCheck;
					    } else {
					        Console.Error.WriteLine("Fehler bei Einrückung: " + s);
					    }
				    }
				    step.addChild(l);
			    }
		    }
	    }

	    private int countLeadingSpaces(String s) {
		    int count = 0;
		    for (int i = 0; i < s.Length; i++) {
			    if (s[i] != ' ') {
				    return count;
			    }
			    count++;
		    }
		    return count;
	    }

	    /**
	     * Berechnet aus der angegebenen Sourcecode-Zeile ein AstElement.
	     * @param deepness 
	     *
	     * @param s
	     * @return
	     */
	    private AstElement getOfLine(int deepness, String s) 
        {
		    String[] tokens = s.Split(' ');
		    if (tokens.Length == 0) {
			    throw new ArgumentException("Zeile ist leer [" + s + "]");
		    }

		    Dictionary<DataType, DataType> attributes = getAttributes(tokens);
		    //        float propability = getPropability(tokens);
		    String cardinality = getCardinality(tokens);
		    Repeater repeater = new Repeater(cardinality/*, propability*/);

		    DataType element = getElement(tokens);
		    if (element == null) {
			    DataType texttype = getTextType(tokens);
			    if (texttype == null) {
				    throw new ArgumentException("Datentype der Zeile unbekannt: [" + s
						    + "]");
			    }
			    return new AstTextElement(texttype, repeater);
		    } else {
			    return new AstXmlElement(deepness, element, attributes, repeater);
		    }
	    }

	    private DataType getElement(string[] tokens) {
		    String s = tokens[0];
		    if (s.StartsWith("text()=")) {
			    return null;
		    }
		    return DataType.get(s);
	    }

	    private DataType getTextType(string[] tokens) {
		    String s = tokens[0];
		    if (s.StartsWith("text()=")) {
			    String value = s.Substring(7);
			    return DataType.get(value);
		    }
		    return null;
	    }

	    private String getCardinality(string[] tokens) {
		    foreach (String s in tokens) {
			    if (s.StartsWith("card=")) {
				    return s.Substring(5);
			    }
		    }
		    return "1";
	    }

	    //    private float getPropability(String[] tokens)
	    //    {
	    //        for (String s : tokens)
	    //        {
	    //            if (s.startsWith("p="))
	    //            {
	    //                String p = s.substring(3);
	    //                return Float.parseFloat(p);
	    //            }
	    //        }
	    //        return 0.5f;
	    //    }

	    private Dictionary<DataType, DataType> getAttributes(String[] tokens) {
		    Dictionary<DataType, DataType> map = new Dictionary<DataType, DataType>();
		    foreach (String s in tokens) {
			    if (s.StartsWith("@")) {
				    int idx = s.IndexOf('=');
				    if (idx < 0) {
					    throw new ArgumentException("Attribut [" + s
							    + "] besitzt keinen Wert");
				    }
				    String key = s.Substring(1, idx);
				    String value = s.Substring(idx + 1);
				    if (key == null) {
					    throw new ArgumentException("Attribut [" + s
							    + "] besitzt keinen gültigen Namen");
				    }
				    if (value == null) {
					    throw new ArgumentException("Attribut [" + s
							    + "] besitzt keinen gültigen Wert");
				    }

				    DataType keyData = DataType.get(key);
				    if (keyData == null) {
					    throw new ArgumentException("Name des Attributs [" + s
							    + "] besitzt keinen gültigen Datentyp ([" + key
							    + "] ist unbekannt).");
				    }
				    DataType valueData = DataType.get(value);
				    if (valueData == null) {
					    throw new ArgumentException("Wert des Attributs [" + s
							    + "] besitzt keinen gültigen Datentyp ([" + value
							    + "] ist unbekannt).");
				    }

				    map.Add(keyData, valueData);
			    }
		    }
		    return map;
	    }

	    public void generateXML() {
		    root.generateXML(null); //Parameter wird für Rootelement ignoriert
	    }

    }
}