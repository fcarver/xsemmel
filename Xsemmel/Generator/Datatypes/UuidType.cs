using System;
using System.IO;

namespace XSemmel.Generator.Datatypes
{

    /**
     * Eine UUID
     * @see UUID
     * @author Frank Schnitzer
     */
    public class UuidType : DataType
    {

        public static DataType constructOf(String s)
        {
            if ("UUID".Equals(s))
            {
                return new UuidType();
            }
            return null;
        }

        public override void write(TextWriter w)
        {
            w.Write(Guid.NewGuid());
        }

    }
}