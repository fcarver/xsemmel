using System;
using System.IO;
using System.Security;

namespace XSemmel.Generator.Datatypes
{


/**
 * Die Hauptstädte der Welt
 * @author Frank Schnitzer
 */
    public class HauptstaedteType : DataType
    {


        private string[] BASE =
			{
			"Kabul", "Kairo", "Tirana", "Algier", "Andorra la Vella", "Luanda",
			"St. John’s",
			"Malabo", "Buenos Aires", "Eriwan", "Baku", "Addis Abeba", "Canberra",
			"Nassau",
			"Manama", "Dhaka", "Bridgetown", "Brüssel", "Belmopan", "Porto-Novo",
			"Cotonou",
			"Thimphu", "Sucre", "La Paz", "Sarajewo", "Gaborone", "Brasília",
			"Bandar Seri Begawan", "Sofia", "Ouagadougou", "Bujumbura",
			"Santiago de Chile",
			"Valparaíso", "Taipeh", "Peking", "San José", "Kopenhagen", "Berlin",
			"Roseau",
			"Santo Domingo", "Dschibuti", "Quito", "Yamoussoukro", "Abidjan",
			"San Salvador",
			"Asmara", "Tallinn", "Suva", "Helsinki", "Paris", "Libreville", "Banjul",
			"Tiflis",
			"Accra", "St. George’s", "Athen", "Guatemala-Stadt", "Conakry", "Bissau",
			"Georgetown",
			"Port-au-Prince", "Tegucigalpa", "Neu-Delhi", "Jakarta", "Bagdad", "Teheran",
			"Dublin",
			"Reykjavík", "Jerusalem", "Rom", "Kingston", "Tokio", "Sana'a", "Amman",
			"Phnom Penh",
			"Jaunde", "Ottawa", "Praia", "Astana", "Doha", "Nairobi", "Bischkek",
			"South Tarawa",
			"Bogotá", "Moroni", "Kinshasa", "Brazzaville", "Pjöngjang", "Seoul",
			"Priština",
			"Zagreb (Agram)", "Havanna", "Kuwait-Stadt", "Vientiane", "Maseru", "Riga",
			"Beirut",
			"Monrovia", "Tripolis", "Vaduz", "Vilnius", "Luxemburg", "Antananarivo",
			"Lilongwe",
			"Kuala Lumpur", "Putrajaya", "Malé", "Bamako", "Valletta", "Rabat", "Majuro",
			"Nouakchott", "Port Louis", "Skopje", "Mexiko-Stadt", "Palikir", "Chisinau",
			"Monaco",
			"Ulaanbaatar", "Podgorica", "Maputo", "Pyinmana", "Windhoek", "Yaren",
			"Kathmandu",
			"Wellington", "Managua", "Amsterdam", "Den Haag", "Niamey", "Abuja", "Oslo",
			"Maskat",
			"Wien", "Dili", "Islamabad", "Melekeok", "Panama-Stadt", "Port Moresby",
			"Asunción",
			"Lima", "Manila", "Warschau", "Lissabon", "Kigali", "Bukarest", "Moskau",
			"Honiara",
			"Lusaka", "Apia", "San Marino", "São Tomé", "Riad", "Stockholm", "Bern",
			"Dakar",
			"Belgrad", "Victoria", "Freetown", "Harare", "Singapur", "Bratislava",
			"Ljubljana",
			"Mogadischu", "Madrid", "Colombo", "Sri Jayawardenepura", "Basseterre",
			"Castries",
			"Kingstown", "Pretoria", "Kapstadt", "Khartum", "Paramaribo", "Mbabane",
			"Damaskus",
			"Duschanbe", "Dodoma", "Dar es Salaam", "Bangkok", "Lomé", "Nuku'alofa",
			"Port-of-Spain", "N'Djamena", "Prag", "Tunis", "Ankara", "Aschgabat",
			"Funafuti",
			"Kampala", "Kiew", "Budapest", "Montevideo", "Taschkent", "Port Vila",
			"Vatikanstadt",
			"Caracas", "Abu Dhabi", "Washington D.C.", "Hanoi", "Minsk", "Bangui",
			"Nikosia"
			};

        public static DataType constructOf(String s)
        {
            if ("ORTE".Equals(s))
            {
                return new HauptstaedteType();
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
