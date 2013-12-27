using System;
using System.IO;

namespace XSemmel.Generator.Datatypes
{

    /**
     * Ein Integer
     * @author Frank Schnitzer
     */
    public class IntType : DataType
    {

        public static DataType constructOf(String s)
        {
            if ("INT".Equals(s))
            {
                return new IntType();
            }
            return null;
        }

        public override void write(TextWriter w)
        {
            int value = (int)(1000 * new System.Random().NextDouble());
            w.Write(value);
        }

    }
}