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

        public MainWindow()
        {
            InitializeComponent();

            cities = JsonParser.ConvertListToCities();

            cities.Sort();

          //  comboBoxCityList.ItemsSource = c;

         //   comboBoxCityList.Loaded += ComboBox_Loaded;

            transmit = new Transmitter();

            //transmit.SendAndGetWeatherStream(s);
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
            List<CityWeather> cityWeathers = transmit.SendAndGetWeatherStream(cityIDForSearch[0].ToString());
            int i = 0;
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
            }
        }
    }
}
