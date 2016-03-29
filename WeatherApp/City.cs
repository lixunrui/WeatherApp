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
            internal double lon; // longitude;
            [JsonProperty]
            internal double lat; //latitude;
        }

        /// <summary>
        /// Weather info
        /// </summary>

        internal class Weather
        {
            [JsonProperty]
            internal int id; // weather condition id
            [JsonProperty]
            internal string main; // group of weather parameters
            [JsonProperty]
            internal string description; // Weather condition within the group
            [JsonProperty]
            internal string icon; // weather icon id
        }

        /// <summary>
        /// Weather Main
        /// </summary>
       
        internal class WeatherBody
        {
            [JsonProperty, JsonConverter(typeof(TemperatureConverter))]
            internal double temp; //temperature, unit default: Kelvin, Metric: Celsius, Imperial: Fahrenheit.
            [JsonProperty]
            internal double pressure; // Atmospheric pressure (on the sea level, if there is no sea_level or grnd_level data), hPa
            [JsonProperty]
            internal double humidity; //Humidity, %
            [JsonProperty, JsonConverter(typeof(TemperatureConverter))]
            internal double temp_min; // Minimum temperature at the moment. 
            //This is deviation from current temp that is possible for large cities and megalopolises geographically expanded (use these parameter optionally). 
            //Unit Default: Kelvin, Metric: Celsius, Imperial: Fahrenheit.
            [JsonProperty, JsonConverter(typeof(TemperatureConverter))]
            internal double temp_max; // Maximum temperature at the moment.
            [JsonProperty]
            internal double sea_level; //Atmospheric pressure on the sea level, hPa
            [JsonProperty]
            internal double grnd_level; //Atmospheric pressure on the ground level, hPa
        }


        internal class Wind
        {
            [JsonProperty]
            internal double speed; //Wind speed. Unit Default: meter/sec, Metric: meter/sec, Imperial: miles/hour.
            [JsonProperty, JsonConverter(typeof(WindDegreeConver))]
            internal string deg; //Wind direction, degrees (meteorological)
        }


        internal class Rain
        {
            [JsonProperty(PropertyName="3h")]
            internal double lastThree; // volume for the last three hours
        }


        internal class Clouds
        {
            [JsonProperty(PropertyName="all")]
            internal double cloudiness; // cloudiness %
        }


        internal class Snow
        {
            [JsonProperty(PropertyName = "3h")]
            internal double lastThree; // volume for the last three hours 
        }


        internal class CitySystem
        {
            [JsonProperty]
            internal int type;
            [JsonProperty]
            internal double id;
            [JsonProperty]
            internal string message;
            [JsonProperty]
            internal string country;
            [JsonConverter(typeof(TimeConverter)),JsonProperty]
            internal DateTime sunrise;
            [JsonConverter(typeof(TimeConverter)),JsonProperty]
            internal DateTime sunset;
        }

        [JsonProperty]
        internal Coordinate coord;
        [JsonProperty]
        internal Weather[] weather;
        [JsonProperty(PropertyName="base")]
        internal string internalParameter;
        [JsonProperty]
        internal WeatherBody main;
        [JsonProperty]
        internal Wind wind;
        [JsonProperty]
        internal Rain rain;
        [JsonProperty]
        internal Clouds clouds;
        [JsonProperty]
        internal Snow snow;
        [JsonConverter(typeof(TimeConverter)), JsonProperty(PropertyName="dt")] // convert UTC time to DateTime
        internal DateTime updateTime;// utc time 
        [JsonProperty]
        internal CitySystem sys;
        [JsonProperty]
        internal double id; //city id
        [JsonProperty]
        internal string name; // city name
        [JsonProperty]
        internal int cod; // internal parameter 
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

    /// <summary>
    /// Convert data to user setting temperature symbol
    /// </summary>
    public class TemperatureConverter : JsonConverter
    {
        String userTempSymbolSetting = AppSetting.GetTempSymbol();

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                // cannot convert null to symbol
                return null;
            }

            double currentTemp;
            try
            {
                double temp = Convert.ToDouble(reader.Value);

                if (userTempSymbolSetting.ToUpper().StartsWith("C"))
                {
                    currentTemp = (temp - 273.15);
                    return currentTemp;
                }
            }
            catch { }

            return reader.Value;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteValue(value);
        }
    }

    public class WindDegreeConver : JsonConverter
    {

        public override bool CanConvert(Type objectType)
        {
            return true;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
                return null;

            double windDegree = Convert.ToDouble(reader.Value);

            windDegree = (windDegree - 180) > 0 ? (windDegree - 180): (windDegree + 180);

            string degreeInString= string.Empty;

            var DegreeConvert = new Dictionary<Func<double, bool>, Action>
            {
                {
                    degree => (degree < 11.25 || degree > 348.75), ()=>degreeInString="N"
                },
                {
                    degree => (degree > 11.25 && degree < 33.75), ()=>degreeInString="NNE"
                },
                {
                    degree => (degree<56.25 && degree > 33.75), ()=>degreeInString="NE"
                },
                {
                    degree => (degree<78.75 && degree > 56.25), ()=>degreeInString="ENE"
                },
                {
                    degree => (degree<101.25 && degree > 78.75), ()=>degreeInString="E"
                },
                {
                    degree => (degree<123.75 && degree > 101.25), ()=>degreeInString="ESE"
                },
                {
                    degree => (degree<146.25 && degree > 123.75), ()=>degreeInString="SE"
                },
                {
                    degree => (degree<168.75 && degree > 146.25), ()=>degreeInString="SSE"
                },
                {
                    degree => (degree<191.25 && degree > 168.75), ()=>degreeInString="S"
                },
                {
                    degree => (degree<213.75 && degree > 191.25), ()=>degreeInString="SSW"
                },
                {
                    degree => (degree < 236.25 && degree > 213.75), ()=>degreeInString="SW"
                },
                {
                    degree => (degree < 258.75 && degree > 236.25), ()=>degreeInString="WSW"
                },
                {
                    degree => (degree < 281.25 && degree > 258.75), ()=>degreeInString="W"
                },
                {
                    degree => (degree < 303.75 && degree > 281.25), ()=>degreeInString="WNW"
                },
                {
                    degree => (degree < 326.25 && degree > 303.75), ()=>degreeInString="NW"
                },
                {
                    degree => (degree < 348.75 && degree > 326.25), ()=>degreeInString="NNW"
                },
            };

            DegreeConvert.First(result => result.Key(windDegree)).Value();

            return degreeInString;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }
            writer.WriteValue(value);
        }
    }
}
