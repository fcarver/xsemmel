using System;
using System.IO;
using System.Security;

namespace XSemmel.Generator.Datatypes
{


    /**
     * Die Staaten der Welt
     * @author Frank Schnitzer
     */
    public class StaatenType : DataType
    {


        private String[] BASE =
			{
			"Abchasien", "Afghanistan", "Agypten Ägypten", "Albanien", "Algerien",
			"Andorra",
			"Angola", "Antigua und Barbuda", "Aquatorialguinea Äquatorialguinea",
			"Argentinien",
			"Armenien", "Aserbaidschan", "Athiopien Äthiopien", "Australien", "Bahamas",
			"Bahrain",
			"Bangladesch", "Barbados", "Belgien", "Belize", "Benin", "Bergkarabach",
			"Bhutan",
			"Bolivien", "Bosnien und Herzegowina", "Botsuana", "Brasilien", "Brunei",
			"Bulgarien",
			"Burkina Faso", "Burundi", "Chile", "China", "Cookinseln", "Costa Rica",
			"Danemark Dänemark", "Deutschland", "Dominica", "Dominikanische Republik",
			"Dschibuti",
			"Ecuador", "El Salvador", "Elfenbeinküste", "Eritrea", "Estland", "Fidschi",
			"Finnland", "Frankreich", "Gabun", "Gambia", "Georgien", "Ghana", "Grenada",
			"Griechenland", "Guatemala", "Guinea", "Guinea-Bissau", "Guyana", "Haiti",
			"Honduras",
			"Indien", "Indonesien", "Irak", "Iran", "Irland", "Island", "Israel",
			"Italien",
			"Jamaika", "Japan", "Jemen", "Jordanien", "Kambodscha", "Kamerun", "Kanada",
			"Kap Verde", "Kasachstan", "Katar", "Kenia", "Kirgisistan", "Kiribati",
			"Kolumbien",
			"Komoren", "Kongo", "Nord Korea", "Süd Korea", "Kosovo", "Kroatien", "Kuba",
			"Kuwait",
			"Laos", "Lesotho", "Lettland", "Libanon", "Liberia", "Libyen",
			"Liechtenstein",
			"Litauen", "Luxemburg", "Madagaskar", "Malawi", "Malaysia", "Malediven",
			"Mali",
			"Malta", "Marokko", "Marshallinseln", "Mauretanien", "Mauritius",
			"Mazedonien",
			"Mexiko", "Mikronesien", "Moldawien", "Monaco", "Mongolei", "Montenegro",
			"Mosambik",
			"Myanmar", "Namibia", "Nauru", "Nepal", "Neuseeland", "Nicaragua",
			"Niederlande",
			"Niger", "Nigeria", "Niue", "Nordzypern Nordzypern", "Norwegen", "Oman",
			"Osterreich Österreich", "Osttimor", "Pakistan", "Palastina Palästina",
			"Palau",
			"Panama", "Papua-Neuguinea", "Paraguay", "Peru", "Philippinen", "Polen",
			"Portugal",
			"Ruanda", "Rumanien Rumänien", "Russland", "Salomonen", "Sambia", "Samoa",
			"San Marino", "Sao Tome und Principe", "Saudi-Arabien", "Schweden",
			"Schweiz",
			"Senegal", "Serbien", "Seychellen", "Sierra Leone", "Simbabwe", "Singapur",
			"Slowakei",
			"Slowenien", "Somalia", "iland)", "Somaliland", "Spanien", "Sri Lanka",
			"St. Kitts und Nevis", "St. Lucia", "St. Vincent und die Grenadinen",
			"Sudafrika Südafrika", "Sudan", "Sudossetien Südossetien", "Suriname",
			"Swasiland",
			"Syrien", "Tadschikistan", "Tansania", "Thailand", "Togo", "Tonga",
			"Transnistrien",
			"Trinidad und Tobago", "Tschad", "Tschechien", "Tunesien", "Turkei Türkei",
			"Turkmenistan Turkmenistan", "Tuvalu", "Uganda", "Ukraine", "Ungarn",
			"Uruguay",
			"Usbekistan", "Vanuatu", "Vatikanstadt", "Venezuela",
			"Vereinigte Arabische Emirate",
			"Vereinigte Staaten", "Vereinigtes Königreich", "Vietnam", "Weißrussland",
			"Westsahara", "Zentralafrikanische Republik", "Zypern"
			};

        public static DataType constructOf(String s)
        {
            if ("STAATEN".Equals(s))
            {
                return new StaatenType();
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