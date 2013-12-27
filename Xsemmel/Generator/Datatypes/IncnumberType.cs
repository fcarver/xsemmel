using System;
using System.IO;

namespace XSemmel.Generator.Datatypes
{

    /**
     * Ein Integer, der fortläufend vergrößert wird.
     * @author Frank Schnitzer
     */
    public class IncnumberType : DataType
    {

        private int lastNumber = 0;

        public static DataType constructOf(String s)
        {
            if ("INCNUMBER".Equals(s))
            {
                return new IncnumberType();
            }
            return null;
        }

        public override void write(TextWriter w)
        {
            lastNumber++;
            w.Write(lastNumber);
        }

    }

}