using System.Collections.Generic;

namespace XSemmel.Differ
{

    /// <summary>
    /// This dictionary overrides the Equals method and checks each element for equality.
    /// </summary>
    /// <typeparam name="K"></typeparam>
    /// <typeparam name="V"></typeparam>
    public class EqualsDictionary<K, V> : Dictionary<K, V>
    {

        public bool Equals(EqualsDictionary<K,V> obj)
        {
            if (Count != obj.Count)
                return false;

            foreach (KeyValuePair<K, V> pair in this)
            {
                if (!obj.ContainsKey(pair.Key))
                    return false;

                if (!Equals(obj[pair.Key], pair.Value))
                    return false;
            }

            return true;
        }

        public override bool Equals(object obj)
        {
            if (obj == null) return false;
            if (obj.GetType() == GetType())
            {
                return Equals((EqualsDictionary<K, V>)obj);
            }
            return false;
        }

    }
}
