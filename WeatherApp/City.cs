using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Newtonsoft.Json;

namespace WeatherApp
{
    /// <summary>
    /// Weather details for a city
    /// </summary>

    internal class CityWeather
    {
        /// <summary>
        /// City Location
        /// </summary>
      
        internal class Coordinate
        {
            [JsonProperty]
            double lon; // longitude;
            [JsonProperty]
            double lat; //latitude;
        }

        /// <summary>
        /// Weather info
        /// </summary>
        
        private class Weather
        {
            [JsonProperty]
            int id; // weather condition id
            [JsonProperty]
            string main; // group of weather parameters
            [JsonProperty]
            string description; // Weather condition within the group
            [JsonProperty]
            string icon; // weather icon id
        }

        /// <summary>
        /// Weather Main
        /// </summary>
       
        private class WeatherBody
        {
            [JsonProperty]
            double temp; //temperature, unit default: Kelvin, Metric: Celsius, Imperial: Fahrenheit.
            [JsonProperty]
            double pressure; // Atmospheric pressure (on the sea level, if there is no sea_level or grnd_level data), hPa
            [JsonProperty]
            double humidity; //Humidity, %
            [JsonProperty]
            double temp_min; // Minimum temperature at the moment. 
            //This is deviation from current temp that is possible for large cities and megalopolises geographically expanded (use these parameter optionally). 
            //Unit Default: Kelvin, Metric: Celsius, Imperial: Fahrenheit.
            [JsonProperty]
            double temp_max; // Maximum temperature at the moment.
            [JsonProperty]
            double sea_level; //Atmospheric pressure on the sea level, hPa
            [JsonProperty]
            double grnd_level; //Atmospheric pressure on the ground level, hPa
        }

        
        private class Wind
        {
            [JsonProperty]
            double speed; //Wind speed. Unit Default: meter/sec, Metric: meter/sec, Imperial: miles/hour.
            [JsonProperty]
            double deg; //Wind direction, degrees (meteorological)
        }

        
        private class Rain
        {
            [JsonProperty(PropertyName="3h")]
            double lastThree; // volume for the last three hours
        }

       
        private class Clouds
        {
            [JsonProperty(PropertyName="all")]
            double cloudiness; // cloudiness %
        }

        
        private class Snow
        {
            [JsonProperty(PropertyName = "3h")]
            double lastThree; // volume for the last three hours 
        }

        
        private class CitySystem
        {
            [JsonProperty]
            int type;
            [JsonProperty]
            double id;
            [JsonProperty]
            string message;
            [JsonProperty]
            string country;
            [JsonConverter(typeof(TimeConverter)),JsonProperty]
            DateTime sunrise;
            [JsonConverter(typeof(TimeConverter)),JsonProperty]
            DateTime sunset;
        }

        [JsonProperty]
        Coordinate coord;
        [JsonProperty]
        Weather[] weather;
        [JsonProperty(PropertyName="base")]
        string internalParameter;
        [JsonProperty]
        WeatherBody main;
        [JsonProperty]
        Wind wind;
        [JsonProperty]
        Rain rain;
        [JsonProperty]
        Clouds clouds;
        [JsonProperty]
        Snow snow;
        [JsonConverter(typeof(TimeConverter)), JsonProperty(PropertyName="dt")] // convert UTC time to DateTime
        DateTime updateTime;// utc time 
        [JsonProperty]
        CitySystem sys;
        [JsonProperty]
        double id; //city id
        [JsonProperty]
        string name; // city name
        [JsonProperty]
        int cod; // internal parameter 
    }

    internal class GroupCityWeather
    {
        [JsonProperty(PropertyName="cnt")]
        int cityCount;

        [JsonProperty(PropertyName="list")]
        CityWeather[] cityList;

        public CityWeather[] CityList
        {
            get { return cityList; }

        }
    }

    /// <summary>
    /// Convert city list json into city classes
    /// City Items:
    /// ID
    /// Name
    /// Country
    /// Coordinate (Coord)
    /// </summary>
    
    internal class City : IComparable<City>
    {
        //{"_id":1270260,"name":"State of Haryāna","country":"IN","coord":{"lon":76,"lat":29}}
        [JsonProperty]
        internal double _id;
        public double Id
        {
            get { return _id; }
            set { _id = value; }
        }
        [JsonProperty]
        string name;
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        [JsonProperty]
        string country;
        public string Country
        {
            get { return country; }
            set { country = value; }
        }
        [JsonProperty]
        CityWeather.Coordinate coord;
        public CityWeather.Coordinate Coord
        {
            get { return coord; }
            set { coord = value; }
        }

        #region IComparable<City> Members

        public int CompareTo(City other)
        {
            if (other == null)
                return 1;

            //return this.name.CompareTo(other.name);
            return this.country.CompareTo(other.country);
        }

        #endregion
    }

    /// <summary>
    /// Date time converter
    /// </summary>
    public class TimeConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                // cannot convert null to datetime
                return null;
            }

            DateTime? dateTime;

            try
            {
                double unixTimeStamp = Convert.ToDouble(reader.Value);

                System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);

                dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();

                return dtDateTime;
            }
            catch(Exception e)
            {
                Console.WriteLine(e.Message);
                dateTime = null;
            }

            return dateTime;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(((DateTime?)value).Value);
        }
    }
}
