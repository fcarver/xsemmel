using System;
using System.Diagnostics;
using System.IO;
using System.Security;

namespace XSemmel.Generator.Datatypes
{


    /**
     * Ein Typ, der im Template vorgegeben (hartcodiert) ist und nicht durch einen Zufallswert
     * ersetzt werden kann.
     * @author Frank Schnitzer
     */
    public class HardCodedType : DataType
    {

        private String[] elements;

        public HardCodedType(String s)
        {
            Debug.Assert(s.StartsWith("<") && s.EndsWith(">"));

            String el = s.Substring(1, s.Length - 1);
            elements = el.Split(new[] {"><"}, StringSplitOptions.RemoveEmptyEntries);
        }

        public static DataType constructOf(String s)
        {
            if (s.StartsWith("<") && s.EndsWith(">"))
            {
                return new HardCodedType(s);
            }
            return null;
        }

        public override void write(TextWriter w)
        {
            String elementname = Random.oneOf(elements); //Tatsächliches Element auswählen
            elementname = SecurityElement.Escape(elementname);
            w.Write(elementname);
        }

    }
}
