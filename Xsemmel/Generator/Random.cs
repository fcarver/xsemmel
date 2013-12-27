namespace XSemmel.Generator {

    /**
     * Diese Klasse enthält statische Methoden, die zufällige Werte zurückgeben.
     * @author Frank Schnitzer
     */
    public static class Random
    {

        private static readonly System.Random random = new System.Random();
	    
	    /**
	     * Ermittelt ein zufälliges Element innerhalb des angegebenen Arrays.
	     *
	     * @param elements
	     * @return
	     */
	    public static T oneOf<T>(T[] elements) 
        {
            int idx = (int)(random.NextDouble() * elements.Length);
		    return elements[idx];
	    }

	    /**
	     * Ermittelt eine zufällige Zahl im angegebenen Intervall (Grenzen inklusive).
	     *
	     * @param p_min
	     * @param p_max
	     * @return
	     */
	    public static int oneBetween(int min, int max) {
		    int zufall = (int) (random.NextDouble() * ((max - min) + 1));
		    return zufall + min;
		    //        int runs = (int) (Math.random() * max);
		    //        return Math.max(runs, min);
	    }

    }

}
