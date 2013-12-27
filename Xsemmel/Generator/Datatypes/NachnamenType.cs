using System;
using System.IO;
using System.Security;

namespace XSemmel.Generator.Datatypes
{


    /**
     * Häufige Nachnamen
     * @author Frank Schnitzer
     */
    public class NachnamenType : DataType
    {


        private String[] BASE =
			{
			"Müller", "Schmidt", "Schneider", "Fischer", "Weber", "Meyer", "Wagner",
			"Becker",
			"Schulz", "Hoffmann", "Schäfer", "Bauer", "Koch", "Richter", "Klein", "Wolf",
			"Schröder", "Neumann", "Schwarz", "Braun", "Hofmann", "Zimmermann",
			"Schmitt",
			"Hartmann", "Krüger", "Schmid", "Werner", "Lange", "Schmitz", "Meier",
			"Krause",
			"Maier", "Lehmann", "Huber", "Mayer", "Herrmann", "Köhler", "Walter",
			"König",
			"Schulze", "Fuchs", "Kaiser", "Lang", "Weiß", "Peters", "Scholz", "Jung",
			"Möller",
			"Hahn", "Keller", "Vogel", "Schubert", "Roth", "Frank", "Friedrich", "Beck",
			"Günther",
			"Berger", "Lorenz", "Baumann", "Schuster", "Kraus", "Böhm", "Simon",
			"Franke",
			"Albrecht", "Winter", "Ludwig", "Martin", "Krämer", "Schumacher", "Vogt",
			"Jäger",
			"Stein", "Otto", "Groß", "Sommer", "Haas", "Graf", "Heinrich", "Seidel",
			"Schreiber",
			"Ziegler", "Brandt", "Kuhn", "Schulte", "Dietrich", "Kühn", "Engel", "Pohl",
			"Horn",
			"Sauer", "Arnold", "Thomas", "Bergmann", "Busch", "Pfeiffer", "Voigt",
			"Götz",
			"Seifert", "Lindner", "Ernst", "Hübner", "Kramer", "Franz", "Beyer", "Wolff",
			"Peter",
			"Jansen", "Kern", "Barth", "Wenzel", "Hermann", "Ott", "Paul", "Riedel",
			"Wilhelm",
			"Hansen", "Nagel", "Grimm", "Lenz", "Ritter", "Bock", "Langer", "Kaufmann",
			"Mohr",
			"Förster", "Zimmer", "Haase", "Lutz", "Kruse", "Jahn", "Schumann", "Fiedler",
			"Thiel",
			"Hoppe", "Kraft", "Michel", "Marx", "Fritz", "Arndt", "Eckert", "Schütz",
			"Walther",
			"Petersen", "Berg", "Schindler", "Kunz", "Reuter"
			};

        public static DataType constructOf(String s)
        {
            if ("NACHNAMEN".Equals(s))
            {
                return new NachnamenType();
            }
            return null;
        }

        public override void write(TextWriter w)
        {
            String s = Random.oneOf(BASE);
            s = SecurityElement.Escape(s);
            w.Write(s);
        }
    }
}