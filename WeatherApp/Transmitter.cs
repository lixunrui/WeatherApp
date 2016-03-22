using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace WeatherApp
{
    internal static class OpenWeatherMap
    {
        internal static String APIKEY = @"APPID=47bf756ed11690f7575c31b0b2a1b03d";
        internal static String HostAddress = @"api.openweathermap.org";
        internal static String DataType = @"/data/2.5/";
        internal static String CurrentWeatherQuery = "weather";
        internal static String ForecastWeatherQuery = "forecast";
        internal static String CurrentGroupWeatherQuery = "group";
        internal static String CityID = "&id=";
        internal static String Unit = "&units=";
    }

    internal enum RequestType
    {
        SingleCity,
        MultipleCity,
    }

    internal class Transmitter
    {
        double cityID;
        public double CityID
        {
            get { return cityID; }
            set { cityID = value; }
        }

        internal List<CityWeather> SendAndGetWeatherStream(string cityIDs)
        {
            string[] citys = cityIDs.Split(',');
            RequestType type = RequestType.SingleCity;
            if (citys.Length > 1)
                type = RequestType.MultipleCity;

            UriBuilder urlBuilder = new UriBuilder();
            urlBuilder.Scheme = "http";
            urlBuilder.Host = OpenWeatherMap.HostAddress;
            urlBuilder.Path = OpenWeatherMap.DataType + ((type == RequestType.SingleCity)? OpenWeatherMap.CurrentWeatherQuery : OpenWeatherMap.CurrentGroupWeatherQuery);
            urlBuilder.Query = OpenWeatherMap.APIKEY + OpenWeatherMap.CityID + cityIDs;
            Uri weatherURi = urlBuilder.Uri;

            return JsonParser.GetWeatherObject(HttpGet(weatherURi), type);
        }

        private string HttpGet(Uri URI)
        {
            System.Net.WebRequest weatherRequest = System.Net.WebRequest.Create(URI);

            //weatherRequest.Proxy = new System.Net.WebProxy(ProxyString, )

           // weatherRequest.Timeout = 9;

            System.Net.WebResponse weatherResponse = weatherRequest.GetResponse();

            StreamReader reader = new StreamReader(weatherResponse.GetResponseStream());

            return reader.ReadToEnd().Trim();
        }
    }
}
