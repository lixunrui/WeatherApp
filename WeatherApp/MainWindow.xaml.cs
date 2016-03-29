using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WeatherApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        List<City> cities = new List<City>();

        List<double> cityIDForSearch = new List<double>();

        Transmitter transmit;

        Update myUpdate;

        City currentCity;

        delegate void Update(List<CityWeather> cityWeather);

        public MainWindow()
        {
            InitializeComponent();

            cities = JsonParser.ConvertListToCities();

            cities.Sort();

          //  comboBoxCityList.ItemsSource = c;

         //   comboBoxCityList.Loaded += ComboBox_Loaded;
            myUpdate = (weathers) => { UpdateWeather(weathers); };

            currentCity = null; 
            transmit = new Transmitter();

            GetLocalWeather();
        }

        private void GetLocalWeather()
        {
            string name = System.Globalization.RegionInfo.CurrentRegion.DisplayName;

        }

        private void ComboBox_Loaded(object sender, RoutedEventArgs e)
        {
            var comboBox = sender as ComboBox;
            TextBox textBox = comboBox.Template.FindName("PART_EditableTextBox", comboBox) as TextBox;
            Popup popup = comboBox.Template.FindName("PART_Popup", comboBox) as Popup;

            if (textBox != null)
            {
                textBox.TextChanged += delegate
                {
                    int showNumber = 0;

                    popup.IsOpen = true;

                    if (showNumber < 10)
                    {
                        comboBox.Items.Filter += a =>
                        {
                            var city = a as City;

                            if (city.Name.ToString().StartsWith(textBox.Text))
                            {
                                showNumber++;
                                return true;
                            }
                            return false;
                        };
                    }
                    
                };
            }

        }

        private void txtCity_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox txtBox = sender as TextBox;

            if (txtBox.Text.Length > 0)
            {
                popUp.IsOpen = true;

                List<City> c = cities.FindAll(city =>
                    city.Name.ToLower().Contains(txtBox.Text.ToLower()));// || city.Country.ToLower().Contains(txtBox.Text.ToLower())).Take(10).ToList();

                if (c.Count > 0)
                    cityList.DataContext = c;
                else
                    popUp.IsOpen = false;
            }
            else
            {
                popUp.IsOpen = false;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            // in here we only retrieve the weather from one city, 
            // TODO: we should be able to retrieve multiple cities
         //   List<CityWeather> cityWeather = transmit.SendAndGetWeatherStream(cityIDForSearch[0].ToString());

       //     UpdateWeather(cityWeather);

            string cityID = null;

            popUp.IsOpen = false;

            if (cityIDForSearch.Count > 0)
                cityID = cityIDForSearch[0].ToString();
            else
            {
                cityID = SearchCityIDFromName(txtCity.Text.Trim());
            }

            if (cityID != null)
            {
                myUpdate(transmit.SendAndGetWeatherStream(cityID));
                System.Threading.Thread searchingThread = new System.Threading.Thread(()=> SearchCityFromID(cityID));
                searchingThread.Start();
            }
            else
                txtCityName.Text = "Invalid City Name";

          //  System.Threading.ThreadPool.QueueUserWorkItem(new System.Threading.WaitCallback(UpdateWeather), transmit.SendAndGetWeatherStream(cityIDForSearch[0].ToString()));

            
            Console.WriteLine("Done");
        }

        //void UpdateWeather(object cityWeather)
        //{
        //    this.Dispatcher.BeginInvoke((Action)(() =>
        //    {
        //        UpdateWeather((List<CityWeather>)cityWeather);
        //    }));
            
        //}

        private void UpdateWeather(List<CityWeather> cityWeather)
        {
            Console.WriteLine("here");

            CityWeather city = cityWeather[0];
            // city name
            txtCityName.Text = city.name + (city.sys.country.Trim().Length > 0 ? String.Format(", {0}",city.sys.country.Trim()) : "");

            StringBuilder weatherInfo = new StringBuilder();
            for( int i = 0; i < city.weather.Length; i++)
            {// weather info
                weatherInfo.Append(city.weather[i].main);
                if( (i+1) < city.weather.Length)
                    weatherInfo.Append(", ");
            }
            txtCityWeather.Text = weatherInfo.ToString();

            // city temperature
            txtCityTemp.Text = String.Format("{0:N2}", city.main.temp);

            txtCityTempS.Content = AppSetting.GetTempSymbol();

            txtCityPressure.Text  = String.Format("{0:N2} hPa",city.main.pressure);

            txtCityHumidity.Text = String.Format("{0:N2}%", city.main.humidity);

            txtCityWind.Text = city.wind.deg.ToString() +","+ String.Format("{0:N2} m/s",city.wind.speed);

            txtCityUpdateTime.Text = city.updateTime.ToLongTimeString();
        }

        private void cityList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ListBox box = sender as ListBox;

            City c = box.SelectedItem as City;

            if (c != null)
            {
              //  txtCity
                cityIDForSearch.Clear();
                cityIDForSearch.Add(c.Id);
                txtCity.Text = c.Name;
                popUp.IsOpen = false;
            }
        }

        private string SearchCityIDFromName(string cityName)
        {
            City city = cities.Find(c => cityName.ToUpper().Trim().Equals(c.Name.ToUpper().Trim()));

            if (city != null)
            {
                return city.Id.ToString();
            }
            else
                return null;
        }

        private void SearchCityFromID(string cityID)
        {
            City city = cities.Find(c => cityID.ToUpper().Trim().Equals(c.Id.ToString().ToUpper().Trim()));

            if (city != null)
            {
                Console.WriteLine("city found");
                currentCity = city;
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            this.Close();
        }

        private void txtCityTemp_MouseDown(object sender, MouseButtonEventArgs e)
        {
            string tempS = txtCityTempS.Content.ToString().Trim().ToUpper();

            bool validTemp = true;
            double currentTemp = 0;
            try
            {
                currentTemp = Convert.ToDouble(txtCityTemp.Text);
            }
            catch
            {
                validTemp = false;
            }

            if (validTemp == false)
            {
                TextBlock tb = sender as TextBlock;
                tb.Text = "Invalid Temp";
                return;
            }

            if (tempS.Equals("C"))
            {
                currentTemp = currentTemp + 273.15;
                txtCityTempS.Content = "K";
            }
            else
            {
                currentTemp = (currentTemp - 273.15);
                txtCityTempS.Content = "C";
            }
            txtCityTemp.Text = String.Format("{0:N2}",currentTemp);
        }

        private void MenuItem_Exit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MenuItem_Default_Click(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Defatult_Click(object sender, RoutedEventArgs e)
        {
            string defaultCityID = Properties.app.Default.DefaultCity;

            if (defaultCityID.Length == 0)
            {
                MessageBox.Show("No default city setup.");
                return;
            }
            myUpdate(transmit.SendAndGetWeatherStream(defaultCityID));
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Set_Default_Click(object sender, RoutedEventArgs e)
        {
            if (currentCity == null)
                return;

            Properties.app.Default.DefaultCity = currentCity.Id.ToString();
            Properties.app.Default.Save();
            MessageBox.Show("Default City Saved");
        }

        private void Button_Add_Favour_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
