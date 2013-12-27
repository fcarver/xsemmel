using System.Collections.Generic;

namespace XSemmel.Differ
{

    /// <summary>
    /// This list overrides the Equals method and checks each element for equality.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class EqualsList<T> : List<T>
    {

        public bool Equals(EqualsList<T> other)
        {
            if (Count != other.Count)
            {
                return false;
            }

            for (int i = 0; i < Count; i++)
            {
                object thisEntry = this[i];
                object otherEntry = other[i];

                if (!Equals(thisEntry, otherEntry))
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() == GetType())
            {
                return Equals((EqualsList<T>)obj);
            }
            return false;
        }

    }
}
