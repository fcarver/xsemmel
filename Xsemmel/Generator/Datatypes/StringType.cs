using System;
using System.IO;
using System.Text;

namespace XSemmel.Generator.Datatypes
{

    /**
     * Ein String
     * @author Frank Schnitzer
     */
    public class StringType : DataType
    {
        private String BASE = "abcdefghijklmnopqrstuvwxyz";

        public static DataType constructOf(String s)
        {
            if ("STRING".Equals(s))
            {
                return new StringType();
            }
            return null;
        }

        public override void write(TextWriter w)
        {
            StringBuilder sb = new StringBuilder();
            int maxLength = 1 + (int)(19 * new System.Random().NextDouble());
            for (int i = 0; i < maxLength; i++)
            {
                int idx = (int)(new System.Random().NextDouble() * BASE.Length);
                sb.Append(BASE[idx]);
            }
            w.Write(sb);
        }
    }
}