using Bing.Maps;
using Microsoft.WindowsAzure.Messaging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace GeoPush
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private Location myLocation;
        private MapShapeLayer shapeLayer;
        private string BingAPIKey = "BingAPIKey";

        public MainPage()
        {
            this.InitializeComponent();
            this.LoadMap();
        }

        private async void LoadMap()
        {
            this.BingMap.Credentials = this.BingAPIKey;
            this.BingMap.MapType = Bing.Maps.MapType.Road;

            // Add shape layer to the map
            this.shapeLayer = new MapShapeLayer();
            this.BingMap.ShapeLayers.Add(this.shapeLayer);
            await this.RefreshMap();
            this.DrawBuffer();
        }

        

        private async Task RefreshMap()
        {
            this.BingMap.Children.Clear();
            await this.SetMyLocation();
        }

        private async Task SetMyLocation()
        {
            var position = await this.GetCurrentPosition();
            this.DataContext = position;
            this.myLocation = new Location(position.Latitude, position.Longitude);
            this.BingMap.Center = this.myLocation;
            this.AddMyLocationPushpin();
        }

        private async Task<Position> GetCurrentPosition()
        {
            try
            {
                var geolocator = new Windows.Devices.Geolocation.Geolocator();
                var location = await geolocator.GetGeopositionAsync();
                var position = new Position
                {
                    Latitude = location.Coordinate.Point.Position.Latitude,
                    Longitude = location.Coordinate.Point.Position.Longitude
                };
                return position;
            }
            catch (Exception ex)
            {
                //Set position manually in case of failure
                //Approximate location of TechEd NA 2014
                Position location = new Position {
                    Latitude = 29.751826,
                    Longitude = -95.360190
                };
                return location;
            }
        }

        private void AddMyLocationPushpin()
        {
            var pushPin = new Pushpin()
            {
                Text = "!"
            };

            pushPin.Background = new SolidColorBrush(Colors.Red);
            pushPin.DataContext = new Place { Latitude = this.myLocation.Latitude, Longitude = this.myLocation.Longitude, Title = "My Location" };
            MapLayer.SetPosition(pushPin, this.myLocation);
            this.BingMap.Children.Add(pushPin);
        }






        private void AddCircleRadius(double meters)
        {
            if (this.myLocation != null)
            {
                this.shapeLayer.Shapes.Clear();
                var circlePoints = new LocationCollection();

                var earthRadius = 6371;
                var lat = (this.myLocation.Latitude * Math.PI) / 180.0; // radians
                var lon = (this.myLocation.Longitude * Math.PI) / 180.0; // radians
                var d = meters / 1000 / earthRadius; // d = angular distance covered on earths surface

                for (int x = 0; x <= 360; x++)
                {
                    var brng = (x * Math.PI) / 180.0; // radians
                    var latRadians = Math.Asin((Math.Sin(lat) * Math.Cos(d)) + ((Math.Cos(lat) * Math.Sin(d)) * Math.Cos(brng)));
                    var lngRadians = lon + Math.Atan2((Math.Sin(brng) * Math.Sin(d)) * Math.Cos(lat), Math.Cos(d) - (Math.Sin(lat) * Math.Sin(latRadians)));

                    var pt = new Location(180.0 * latRadians / Math.PI, 180.0 * lngRadians / Math.PI);
                    circlePoints.Add(pt);
                }

                MapPolygon circlePolygon = new MapPolygon();
                circlePolygon.FillColor = Color.FromArgb(80, 20, 20, 200);
                circlePolygon.Locations = circlePoints;
                this.shapeLayer.Shapes.Add(circlePolygon);
            }
        }

        private async void Radius_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.Radius != null)
            {
                await this.RefreshMap();
                this.DrawBuffer();
            }
        }

        private void DrawBuffer()
        {
            switch (this.Radius.SelectedIndex)
            {
                case 0:
                    this.BingMap.Center = this.myLocation;
                    this.BingMap.ZoomLevel = 15;
                    this.AddCircleRadius(1000);
                    break;
                case 1:
                    this.BingMap.Center = this.myLocation;
                    this.BingMap.ZoomLevel = 14;
                    this.AddCircleRadius(2000);
                    break;
                case 2:
                    this.BingMap.Center = this.myLocation;
                    this.BingMap.ZoomLevel = 12.7;
                    this.AddCircleRadius(5000);
                    break;
                case 3:
                    this.BingMap.Center = this.myLocation;
                    this.BingMap.ZoomLevel = 11.7;
                    this.AddCircleRadius(10000);
                    break;
            }
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            String geocoords = myLocation.Latitude + ", " + myLocation.Longitude;
            
            Bing.Maps.Search.GeocodeRequestOptions requestOptions = new Bing.Maps.Search.GeocodeRequestOptions(geocoords);
            
            Bing.Maps.Search.SearchManager searchManager = BingMap.SearchManager;
            Bing.Maps.Search.LocationDataResponse response = await searchManager.GeocodeAsync(requestOptions);
          
            string city = response.LocationData[0].Address.Locality;
            MessageDialog dialog = new MessageDialog("City is " + city);
            
            await dialog.ShowAsync();

            var hub = new NotificationHub("HubName", "ListenConnectionString");
            
            var result = await hub.RegisterNativeAsync(App.ChannelUri, new[] { city });

            // Displays the registration ID so you know it was successful
            if (result.RegistrationId != null)
            {
                dialog = new MessageDialog("Registration successful: " + result.RegistrationId);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
        }

        private async void btnTemplates_Click(object sender, RoutedEventArgs e)
        {
            var hub = new NotificationHub("HubName", "ListenConnectionString");

            var template = @"<toast><visual><binding template=""ToastText01""><text id=""1"">$(message)</text></binding></visual></toast>";
            var result = await hub.RegisterTemplateAsync(App.ChannelUri, template, "message");

            // Displays the registration ID so you know it was successful
            if (result.RegistrationId != null)
            {
                var dialog = new MessageDialog("Registration successful: " + result.RegistrationId);
                dialog.Commands.Add(new UICommand("OK"));
                await dialog.ShowAsync();
            }
        }
    }
}
