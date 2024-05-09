using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IQAirSP_Projekat1
{
    public class IQAir
    {
        public string status { get; set; }
        public Data data { get; set; }

        // Konstruktor
        public IQAir(string status)
        {
            this.status = status;
        }
    }
    public class Current
    {
        public Pollution pollution { get; set; }
        public Weather weather { get; set; }
    }

    public class Data
    {
        public string city { get; set; }
        public string state { get; set; }
        public string country { get; set; }
        public Location location { get; set; }
        public Current current { get; set; }
    }

    public class Location
    {
        public string type { get; set; }
        public List<double> coordinates { get; set; }
    }

    public class Pollution
    {
        public DateTime ts { get; set; }
        public int aqius { get; set; }
        public string mainus { get; set; }
        public int aqicn { get; set; }
        public string maincn { get; set; }
    }

    public class Weather
    {
        public DateTime ts { get; set; }
        public int tp { get; set; }
        public int pr { get; set; }
        public int hu { get; set; }
        public double ws { get; set; }
        public int wd { get; set; }
        public string ic { get; set; }
    }
}
