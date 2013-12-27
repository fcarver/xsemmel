using System;
using System.IO;

namespace XSemmel.Generator.Datatypes
{

    /**
     * Eine Gleitkommazahl
     * @author Frank Schnitzer
     */
    public class FloatType : DataType
    {

        public static DataType constructOf(String s)
        {
            if ("FLOAT".Equals(s))
            {
                return new FloatType();
            }
            return null;
        }

        public override void write(TextWriter w)
        {
            float value = (float)new System.Random().NextDouble(); ;
            w.Write(value);
        }

    }
}