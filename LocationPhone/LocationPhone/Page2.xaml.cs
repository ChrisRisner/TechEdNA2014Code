using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using Windows.Devices.Geolocation;
using Microsoft.Phone.Maps.Services;
using System.Device.Location;
using System.Net.Http;
using Newtonsoft.Json.Linq;

namespace LocationPhone
{
    public partial class Page2 : PhoneApplicationPage
    {
        private static string BingKey = "AmPyMvmAqzKTu-j_oSo9iBhnNUwEjH2K1_xUAtKMq9oR0WRs4SqIVRfMcZ6rrRiN";
        
        public Page2()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (App.Geolocator == null)
            {
                App.Geolocator = new Geolocator();
                App.Geolocator.DesiredAccuracy = PositionAccuracy.High;
                App.Geolocator.MovementThreshold = 100; // The units are meters.
                App.Geolocator.PositionChanged += geolocator_PositionChanged;
            }
        }

        protected override void OnRemovedFromJournal(System.Windows.Navigation.JournalEntryRemovedEventArgs e)
        {
            App.Geolocator.PositionChanged -= geolocator_PositionChanged;
            App.Geolocator = null;
        }

        async void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {            
            if (!App.RunningInBackground)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    LatitudeTextBlock.Text = args.Position.Coordinate.Latitude.ToString("0.00");
                    LongitudeTextBlock.Text = args.Position.Coordinate.Longitude.ToString("0.00");
                });
            }

            // contact Bing
            var client = new HttpClient();
            var url = String.Format("http://dev.virtualearth.net/REST/v1/Locations/{0},{1}?o=json&key={2}", args.Position.Coordinate.Latitude.ToString(), args.Position.Coordinate.Longitude.ToString(), BingKey);
            HttpResponseMessage response = await client.GetAsync(new Uri(url));
            response.EnsureSuccessStatusCode();
            var jsonString = await response.Content.ReadAsStringAsync();

            JObject json = JObject.Parse(jsonString);
            JToken jsonAddress = json["resourceSets"][0]["resources"][0]["address"];
            var address = new {
                PostalCode = (string)jsonAddress["postalCode"],
                Locality = (string)json["resourceSets"][0]["resources"][0]["address"]["locality"],
                County = (string)json["resourceSets"][0]["resources"][0]["address"]["adminDistrict2"],
                State = (string)json["resourceSets"][0]["resources"][0]["address"]["adminDistrict"],
                Country = (string)json["resourceSets"][0]["resources"][0]["address"]["countryRegion"]
            };

            App.Address = address;
            App.RegisterForNotificationsWithAddress();

            if (!App.RunningInBackground)
            {
                Dispatcher.BeginInvoke(() =>
                {
                    Zip.Text = address.PostalCode;
                    Locality.Text = address.Locality;
                    County.Text = address.County;
                    State.Text = address.State;
                    Country.Text = address.Country;
                });
            }
            else
            {
                Microsoft.Phone.Shell.ShellToast toast = new Microsoft.Phone.Shell.ShellToast();
                toast.Content = address.PostalCode;
                toast.Title = "Location: ";
                toast.NavigationUri = new Uri("/Page2.xaml", UriKind.Relative);
                toast.Show();

            }
        }


    }
}