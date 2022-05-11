using Microsoft.Maps.MapControl.WPF;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Maps1
{
    public partial class Page2 : Page
    {


        public Page2()
        {
            InitializeComponent();
            Coordinates coords = CityCoordinates.CoordinatesMap["Kraków"];
            Location location = new Location(coords.latitude, coords.longitude);
            map.SetView(location, 12);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var comboBoxItem = CitySelect.Items[CitySelect.SelectedIndex] as ComboBoxItem;
            string cityName = comboBoxItem.Content.ToString();
            Coordinates coords = CityCoordinates.CoordinatesMap[cityName];
            int hours = (int)SliderDaysSince.Value * 24; // days to hours
            int radius = (int)SliderRadius.Value;
            radius *= 1000; //km to meters
            int maxRespSize = (int)SliderMaxSize.Value;
            WebClient webClient = new WebClient();
            webClient.Headers.Add(HttpRequestHeader.Authorization, "Bearer " + ServerState.auth_token);
            string latString = coords.latitude.ToString().Replace(',', '.');
            string lngString = coords.longitude.ToString().Replace(',', '.');
            webClient.QueryString.Add("latitude", latString);
            webClient.QueryString.Add("longitude", lngString);
            webClient.QueryString.Add("since", hours.ToString());
            webClient.QueryString.Add("maxDistance", radius.ToString());
            webClient.QueryString.Add("size", maxRespSize.ToString());
            string jsonResp = webClient.DownloadString(ServerState.SERVER_URL + "/measurement");
            var measurementList = JsonConvert.DeserializeObject<List<MeasurementDTO>>(jsonResp);
            Location location = new Location(coords.latitude, coords.longitude);
            map.SetView(location, 12);
            setMarkers(measurementList);
        }


        private void setMarkers(List<MeasurementDTO> measurements)
        {
            map.Children.Clear();
            map.Children.Clear();
            MapLayer pushpinLayer = new MapLayer();
            pushpinLayer.Name = "PushPinLayer";

            map.Children.Add(pushpinLayer);
            measurements.ForEach(m => {
                Pushpin pushpin = new Pushpin();
                pushpin.Name = "test";
                pushpin.Background = getBrush(m);
                Location location = new Location(m.latitude, m.longitude);
                ToolTip toolTip = new ToolTip();
                toolTip.Content = string.Format("PM 10: {0} \n PM 2.5: {1} \n PM 1: {2} \n Data pomiaru: {3} \n Jakość powietrza: {4}", m.pm10, m.pm2_5, m.pm1, m.date, getAirQualityString(m));
                pushpin.ToolTip = toolTip;
                pushpinLayer.AddChild(pushpin, location);
            });
        }
        private string getAirQualityString(MeasurementDTO m)
        {
            if (m.pm2_5 < 25)
            {
                return "Dobra";
            }
            else if (m.pm2_5 < 50)
            {
                return "Średnia";
            }
            else
            {
                return "Zła";
            }
        }

        private SolidColorBrush getBrush(MeasurementDTO measurement)
        {
            Color color;
            if (measurement.pm2_5 < 25)
            {
                color = Colors.Green;
            }
            else if (measurement.pm2_5 < 50)
            {
                color = Colors.Yellow;
            }
            else
            {
                color = Colors.Red;
            }
            return new SolidColorBrush(color);
        }

        private void SliderDaysSince_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int newValue = (int)e.NewValue;
            string content;
            if (newValue == 1)
            {
                content = "Ostatniego dnia.";
            }
            else
            {
                content = string.Format("Ostatnich {0} dni.", newValue);
            }
            daysText.Text = content;
        }

        private void SliderRadius_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int newValue = (int)e.NewValue;
            string content = string.Format("Promienia {0} km.", newValue);
            radiusText.Text = content;
        }

        private void SliderMaxSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            int newValue = (int)e.NewValue;
            sizeText.Text = newValue.ToString();
        }
    }
}
