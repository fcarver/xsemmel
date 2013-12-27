using System.Collections.Generic;

namespace XSemmel.Differ
{

    /// <summary>
    /// Diese Klasse ist in der Lage, zwei Knotenbäume miteinander zu vergleichen.
    /// Die Reihenfolge der Knoten wird dabei nicht berücksichtigt.
    /// </summary>
    internal class DiffEngine
    {
        
        /// <summary>
        /// Vergleicht zwei Knotenbäume. Beide Knotenbäume werden dabei verändert.
        /// </summary>
        /// <param name="k1">Der eine Knotenbaum.</param>
        /// <param name="k2">Der andere Knotenbaum.</param>
        /// <returns><code>null</code>, falls beide Knoten identisch sind oder der geänderte
        /// k1-Knoten mit den Unterschieden.</returns>
        public Knoten Diff(Knoten k1, Knoten k2)
        {
            subtract(k1, k2);

            if (!k1.hasKinder() && !k2.hasKinder())
            {
                if (k1.Equals(k2))
                {
                    //beide Bäume sind identisch
                    return null;
                }
            }

            return k1;
        }

        /**
	     * Subtrahiert die beiden Knoten voneinander.
	     * @param minuend Der Minuend.
	     * @param subtrahend Der Subtrahend.
	     */

        private void subtract(Knoten minuend, Knoten subtrahend)
        {
            //Die Kinder können nicht sofort gelöscht werden, da sich sonst die Liste ändert
            //und man nicht mehr einfach durchiterieren kann
            List<Knoten> zuLoeschendeKinderMinu = new List<Knoten>();
            List<Knoten> zuLoeschendeKinderSubt = new List<Knoten>();
            List<Knoten> hinzuzufuegendeKinder = new List<Knoten>();

            foreach (Knoten kind_minu in minuend.getKinder())
            {
                Knoten kind_subt = getKindWie(kind_minu, subtrahend);

                if (kind_subt != null)
                {
                    subtract(kind_minu, kind_subt);

                    if (!kind_minu.hasKinder() && !kind_subt.hasKinder())
                    {
                        if (kind_minu.Equals(kind_subt))
                        {
                            zuLoeschendeKinderMinu.Add(kind_minu);
                            zuLoeschendeKinderSubt.Add(kind_subt);
                            kind_minu.setIgnoreMe();
                            kind_subt.setIgnoreMe();
                        }
                    }
                    else
                    {
                        //
                    }
                }
                else
                {
                    Knoten k = new Knoten(
                        Knoten.KnotenTyp.GELOESCHT,
                        kind_minu.getName(),
                        kind_minu.getText(),
                        kind_minu.getAttributes());
                    k.setIgnoreMe();
                    hinzuzufuegendeKinder.Add(k);
                    zuLoeschendeKinderMinu.Add(kind_minu);
                }
            }

            foreach (Knoten loeschMich in zuLoeschendeKinderMinu)
            {
                minuend.removeKind(loeschMich);
            }
            foreach (Knoten loeschMich in zuLoeschendeKinderSubt)
            {
                subtrahend.removeKind(loeschMich);
            }

            if (subtrahend.hasKinder())
            {
                foreach (Knoten hinzugefuegtesKind in subtrahend.getKinder())
                {
                    Knoten kind_minu = getKindWie(hinzugefuegtesKind, minuend);

                    if (kind_minu == null)
                    {
                        Knoten k = new Knoten(
                            Knoten.KnotenTyp.HINZUGEFUEGT,
                            hinzugefuegtesKind.getName(),
                            hinzugefuegtesKind.getText(),
                            hinzugefuegtesKind.getAttributes());
                        k.setIgnoreMe();
                        hinzuzufuegendeKinder.Add(k);
                    }
                }
            }
            foreach (Knoten neu in hinzuzufuegendeKinder)
            {
                minuend.addKind(neu);
            }
        }

        /**
	     * Ermittelt aus dem Knotenbaum "parent" ein Kind, das so ähnlich ist wie "beispiel"
	     * @param beispiel
	     * @param parent
	     * @return Das ähnliche Kind oder <code>null</code>, falls kein ähnliches Kind
	     * existiert.
	     */

        private static Knoten getKindWie(Knoten beispiel, Knoten parent)
        {
            Knoten gefunden = null;
            Knoten gefunden2 = null;
            int anzGefunden = 0;

            ICollection<Knoten> kids = parent.getKinder();
            foreach (Knoten kid in kids)
            {
                if (kid.isIgnoreMe())
                {
                    continue;
                }
                if (kid.equalsWithoutChildren(beispiel))
                {
                    if (gefunden == null)
                    {
                        //erstes übernehmen
                        gefunden = kid;
                    }
                    anzGefunden++;
                }
            }

            //wenn mehrere Kinder gemäß equalsWithoutChildren gefunden wurden, dann Suche eingrenzen
            //Prüfen, ob die Knoten überhaupt Kinder haben
            if (anzGefunden > 1)
            {
                anzGefunden = 0;
                foreach (Knoten kid in kids)
                {
                    if (kid.isIgnoreMe())
                    {
                        continue;
                    }
                    if (kid.equalsWithoutChildren(beispiel))
                    {
                        if (kid.getKinder().Count == beispiel.getKinder().Count)
                        {
                            if (gefunden2 == null)
                            {
                                gefunden2 = kid;
                            }
                            anzGefunden++;
                        }
                    }
                }
            }

            //wenn mehrere Kinder gemäß equalsWithoutChildren && Kinderanzahl gefunden wurden, dann Suche eingrenzen
            //Prüfen, ob die Knoten überhaupt Kinder haben
            if (anzGefunden > 1)
            {
                foreach (Knoten kid in kids)
                {
                    if (kid.isIgnoreMe())
                    {
                        continue;
                    }
                    if (kid.Equals(beispiel))
                    {
                        return kid;
                    }
                }
            }

            if (gefunden2 != null)
            {
                return gefunden2;
            }
            else
            {
                return gefunden;
            }
        }
    }
}