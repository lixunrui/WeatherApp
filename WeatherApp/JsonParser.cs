using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using Newtonsoft.Json;
using System.Reflection;

namespace WeatherApp
{
    static class JsonParser
    {
        internal static List<City> ConvertListToCities()
        {
            using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("WeatherApp.Resources.city.list.json"))
            {
                using (StreamReader reader = new StreamReader(stream))
                {
                    string json = reader.ReadToEnd();
                    try
                    {
                        List<City> cities = JsonConvert.DeserializeObject<List<City>>(json);
                        return cities;
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        return null;
                    }
                }
            }
        }

        internal static List<CityWeather> GetWeatherObject(string weatherString, RequestType type)
        {
            List<CityWeather> returnCityWeathers = new List<CityWeather>();
            try
            {
                switch (type)
                {
                    case RequestType.MultipleCity:
                        GroupCityWeather group = JsonConvert.DeserializeObject<GroupCityWeather>(weatherString);
                        returnCityWeathers = group.CityList.ToList();
                       // return group;
                        break;

                    case RequestType.SingleCity:
                    default:
                        CityWeather cityWeather = JsonConvert.DeserializeObject<CityWeather>(weatherString);
                        //return cityWeather;
                        returnCityWeathers.Clear();
                        returnCityWeathers.Add(cityWeather);
                        break;
                       
                }
                return returnCityWeathers;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }
    }
}
