using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Cloudy
{
    public class OpenWeatherMapCore
    {
        public static async Task<WeatherData> GetWeatherData(string cityName)
        {
            HttpClient wClient = new HttpClient();
            string response = await wClient.GetStringAsync($"http://api.openweathermap.org/data/2.5/weather?q={cityName}&appid=15eba336d0c3ebc07b5ce34e0981af39&units=metric");

            WeatherData wData = JsonConvert.DeserializeObject<WeatherData>(response);
            return wData;

        }

        public static async Task<bool> IsCityCloudy(string cityName)
        {
            WeatherData dat = await OpenWeatherMapCore.GetWeatherData(cityName);

            if (dat.clouds.all == 0)
            {
                return false;
            }
            else
            {
                return true;
            }

            //string msg = $"It's {dat.weather.FirstOrDefault().description} w/ {dat.main.temp}° in {cityName} City!";
            //WeatherCond.Text = msg;
        }
    }


    public class WeatherData
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

    public class Coord
    {
        public float lon { get; set; }
        public float lat { get; set; }
    }

    public class Main
    {
        public float temp { get; set; }
        public float pressure { get; set; }
        public int humidity { get; set; }
        public float temp_min { get; set; }
        public float temp_max { get; set; }
        public float sea_level { get; set; }
        public float grnd_level { get; set; }
    }

    public class Wind
    {
        public float speed { get; set; }
        public float deg { get; set; }
    }

    public class Clouds
    {
        public int all { get; set; }
    }

    public class Sys
    {
        public float message { get; set; }
        public string country { get; set; }
        public int sunrise { get; set; }
        public int sunset { get; set; }
    }

    public class Weather
    {
        public int id { get; set; }
        public string main { get; set; }
        public string description { get; set; }
        public string icon { get; set; }
    }

}
