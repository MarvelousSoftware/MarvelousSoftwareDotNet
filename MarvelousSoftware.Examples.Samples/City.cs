using System;

namespace MarvelousSoftware.Examples.Samples
{
    public class City
    {
        public City()
        {
        }

        public City(string name, int population, double totalArea, int? populationDensity, string country, DateTime? created = null, bool? deathPenalty = null)
        {
            Name = name;
            Population = population;
            TotalArea = totalArea;
            PopulationDensity = populationDensity;
            Country = country;
            Created = created ?? DateTime.Now;
            DeathPenalty = deathPenalty;
        }

        public string Name { get; set; }
        public int Population { get; set; }
        public double TotalArea { get; set; }
        public int? PopulationDensity { get; set; }
        public string Country { get; set; }
        public DateTime Created { get; set; }
        public bool? DeathPenalty { get; set; }

    }
}
