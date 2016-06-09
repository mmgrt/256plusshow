using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;

namespace BackgroundCore
{
 

    public sealed class WeatherData
    {
        public Coord coord { get; set; }
        public Weather[] weather { get; set; }
        public string _base { get; set; }
        public Main main { get; set; }
        public Wind wind { get; set; }
        public Clouds clouds { get; set; }
        public int dt { get; set; }
        public Sys sys { get; set; }
        public int id { get; set; }
        public string name { get; set; }
        public int cod { get; set; }
    }

    public sealed class Coord
    {
        public float lon { get; set; }
        public float lat { get; set; }
    }

    public sealed class Main
    {
        public float temp { get; set; }
        public float pressure { get; set; }
        public int humidity { get; set; }
        public float temp_min { get; set; }
        public float temp_max { get; set; }
        public float sea_level { get; set; }
        public float grnd_level { get; set; }
    }

    public sealed class Wind
    {
        public float speed { get; set; }
        public float deg { get; set; }
    }

    public sealed class Clouds
    {
        public int all { get; set; }
    }

    public sealed class Sys
    {
        public float message { get; set; }
        public string country { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }

    public sealed class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

}
