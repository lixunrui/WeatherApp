using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace WeatherApp
{
    internal static class AppSetting
    {
        internal const String FavourCityList = "FavourCityList";
        internal const String TempSymbol = "TempSetting";

        internal static List<String> GetFavourCityList()
        {
            System.Collections.Specialized.StringCollection cities = Properties.app.Default.FavourCityList;

            List<String> newCities = cities.Cast<String>().ToList();

            return newCities;
        }

        internal static void AddFavourCity(string cityID)
        {
            System.Collections.Specialized.StringCollection cities = Properties.app.Default.FavourCityList;
            cities.Add(cityID);
            Properties.app.Default.Save();
        }

        internal static void RemoveFavourCity(string cityID)
        {
            System.Collections.Specialized.StringCollection cities = Properties.app.Default.FavourCityList;
            try 
            {
                cities.Remove(cityID);
            }
            catch
            {
                Console.WriteLine("Unable to find the city {0}.", cityID);
            }
            
            Properties.app.Default.Save();
        }

        internal static string GetTempSymbol(string newSymbol=null)
        {
            string defaultSymbol = "C";

            if (newSymbol != null)
            {
                Properties.app.Default.TempSetting = newSymbol;
                Properties.app.Default.Save();
            }

            // get the new symbol
            string symbol = Properties.app.Default.TempSetting;

            if (symbol != null)
                return symbol;
            else
                return defaultSymbol;
        }

        static bool GetSettingValue(string key)
        {
            // config file with tag appSettings 

            /*
            System.Collections.Specialized.NameValueCollection appSettings = System.Configuration.ConfigurationManager.AppSettings;

            if (appSettings.Count == 0)
                return false;

            string[] values = appSettings.GetValues(key);
            if (values.Length > 0)
                return true;
            else return false;
            */

           // the settings file Properties.app.Default.FavourCityList

            System.Collections.Specialized.StringCollection cities = Properties.app.Default.FavourCityList;

            //cities.Add("asdasd");

            //cities.Remove("asdasd");

            // Properties.app.Default.Save();

            if (cities.Count > 0)
                return true;
            else return false;
            
        }
    }
}
