using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using App1_malliksi.Models;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Xamarin.Forms.GoogleMaps;
using System.Globalization;

namespace App1_malliksi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedPage2 : TabbedPage
    {
        public string trackerID =null;
        public string trackerRelay = "0";
        public string trackerRelayButton = "0";
        public string trackerActive = "0";
        public string trackerActiveButton = "0";
        public string rowId = "0";
        public Double latitude = 0;
        public Double longitude = 0;
        public TabbedPage2()
        {
            InitializeComponent();
            AddMarkers();
            //  assignmentList.ItemsSource = new string[] { "" };
            trackerList.ItemsSource = new string[] { "" };
            trackerList.ItemSelected += TrackerList_ItemSelected;
            trackerIdLabel.Text = "Tracker Name";
            trackerNameLabel.Text = "?";
            Task.Factory.StartNew(async () =>
            {
                while (true)
                {
                    Device.BeginInvokeOnMainThread(async () =>
                    {
                        if (trackerID != null )//  trackerList == null)
                        {

                            HttpClient client = new HttpClient();
                            client.BaseAddress = new Uri("https://paikkatietoback1.azurewebsites.net");
                            string json2 = await client.GetStringAsync("/api/tracker/info?trackerID=" + trackerID);
                            //string[] trackerinfo = JsonConvert.DeserializeObject<string[]>(json);
                            string[] tmp2 = json2.Split(' ');
                            trackerRelay = tmp2[0];
                            if (trackerRelay == "0") { relayLabel.Text = "OFF"; }
                            if (trackerRelay == "1") { relayLabel.Text = "ON"; }
                            trackerRelayButton = trackerRelay;
                            trackerActive = tmp2[1];
                            if (trackerActive == "0") { activeLabel.Text = "OFF"; }
                            if (trackerActive == "1") { activeLabel.Text = "ON"; }
                            trackerActiveButton = trackerActive;
                            rowId = tmp2[2];
                            trackerIdLabel.Text = trackerID;
                            

                            string json3 = await client.GetStringAsync("/api/tracker/gpsinfo?trackerID=" + trackerID);
                            string[] tmp3 = json3.Split(' ');
                            latitudeLabel.Text = tmp3[0];
                            longitudeLabel.Text = tmp3[1];
                            speedLabel.Text = tmp3[2];
                            tempLabel.Text = tmp3[3];

                            latitude = Double.Parse(latitudeLabel.Text, CultureInfo.InvariantCulture);
                            longitude = Double.Parse(longitudeLabel.Text, CultureInfo.InvariantCulture);

                            Position loc1 = new Position(latitude, longitude);
                            //Position loc1 = new Position(61.6899715, 27.2627075);

                            Pin marker1 = new Pin()
                            {
                                Address = "",
                                IsVisible = true,
                                Label = trackerNameLabel.Text,
                                Position = loc1,
                                Type = PinType.Place

                            };
                           

                            mapView.Pins.Add(marker1);
                          
                            mapView.MoveToRegion(MapSpan.FromCenterAndRadius(marker1.Position, Distance.FromMeters(1000)));
                        }
                    });
                    await Task.Delay(5000);
                }
            });
        }
        public async void LoadTrackers(object sender, EventArgs e)
        {

            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://paikkatietoback1.azurewebsites.net");
                string json = await client.GetStringAsync("/api/tracker/online/");
                string[] trackers = JsonConvert.DeserializeObject<string[]>(json);

                trackerList.ItemsSource = trackers;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetType().Name + ": " + ex.Message;
                trackerList.ItemsSource = new string[] { errorMessage };
            }

        }
        public async void TrackerList_ItemSelected(object sender, SelectedItemChangedEventArgs e)
        {
            string tracker = trackerList.SelectedItem?.ToString();
            //await Navigation.PushAsync(new TabbedPage2());
            string[] tmp = tracker.Split(' ');
             trackerID = tmp[0];
            string trackerName = tmp[1];
            if (!string.IsNullOrEmpty(tracker))
            {

                try
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("https://paikkatietoback1.azurewebsites.net");
                    //client.BaseAddress = new Uri("https://localhost:44349");
                    string json2 = await client.GetStringAsync("/api/tracker/info?trackerID=" + trackerID);
                    //string[] trackerinfo = JsonConvert.DeserializeObject<string[]>(json);
                    string[] tmp2 = json2.Split(' ');
                    trackerRelay = tmp2[0];
                    if(trackerRelay == "0") { relayLabel.Text = "OFF"; }
                    if(trackerRelay == "1") { relayLabel.Text = "ON"; }
                    trackerActive = tmp2[1];
                    if (trackerActive == "0") { activeLabel.Text = "OFF"; }
                    if (trackerActive == "1") { activeLabel.Text = "ON"; }
                    rowId = tmp2[2];
                    trackerIdLabel.Text = trackerID;
                    trackerNameLabel.Text = trackerName;

                    string json3 = await client.GetStringAsync("/api/tracker/gpsinfo?trackerID=" + trackerID);
                    string[] tmp3 = json3.Split(' ');
                    latitudeLabel.Text = tmp3[0];
                    longitudeLabel.Text = tmp3[1];
                    speedLabel.Text = tmp3[2];
                    tempLabel.Text = tmp3[3];

                }
                catch (Exception ex)
                {
                    string errorMessage = ex.GetType().Name + ": " + ex.Message;
                    trackerList.ItemsSource = new string[] { errorMessage };
                }
            }
        }
        private async void ListWorkAssignments(object sender, EventArgs e)
        {
            string tracker = trackerList.SelectedItem?.ToString();
            if (string.IsNullOrEmpty(tracker))
            {
                await DisplayAlert("List Work", "You must select employee first,", "Ok");
            }
            else
            {
                await Navigation.PushAsync(new TabbedPage2());
            }
        }

        //public async void LoadWorkAssignments(object sender, EventArgs e)
        //{
        //    try
        //    {
        //        HttpClient client = new HttpClient();
        //        client.BaseAddress = new Uri("https://paikkatietoback1.azurewebsites.net");
        //        string json = await client.GetStringAsync("/api/Workassignment");
        //        string[] assignments = JsonConvert.DeserializeObject<string[]>(json);

        //        assignmentList.ItemsSource = assignments;
        //    }
        //    catch (Exception ex)
        //    {
        //        string errorMessage = ex.GetType().Name + ": " + ex.Message;
        //        assignmentList.ItemsSource = new string[] { errorMessage };
        //    }
        //}
        public async void ClickRelayON(object sender, EventArgs e)
        {
            if (trackerActive == null) { trackerActive = "0"; }

            //string testTime = "2020-09-10T11:29:19";
            string TimeNow = DateTime.Now.ToString("yyyy'-'MM'-'ddTHH:mm:ss");
            TrackerControlModel data = new TrackerControlModel()
            {
                TrackerID = 1,
                LastChangeTime = TimeNow,//"2020-09-10T11:29:19",
                RelayControl = "1",
                GpsActive = trackerActiveButton,
                Queue = "Yes"
            };

            try
            {
          
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://paikkatietoback1.azurewebsites.net");
                //client.BaseAddress = new Uri("https://localhost:44349");
                string input = JsonConvert.SerializeObject(data);
                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");

                HttpResponseMessage message = await client.PostAsync("/api/tracker/control", content);

                trackerRelayButton = "1";
                relayON.BackgroundColor = Color.Green;
                relayOFF.BackgroundColor = Color.Gray;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetType().Name + ": " + ex.Message;
                trackerList.ItemsSource = new string[] { errorMessage };
            }

        }
        public async void ClickRelayOFF(object sender, EventArgs e)
        {
           
            TrackerControlModel data = new TrackerControlModel()
            {
                TrackerID = 1,
                LastChangeTime = DateTime.Now.ToString("yyyy'-'MM'-'ddTHH:mm:ss"),
                RelayControl = "0",
                GpsActive = trackerActiveButton,
                Queue = "Yes"
            };

            try
            {

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://paikkatietoback1.azurewebsites.net");

                string input = JsonConvert.SerializeObject(data);
                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");

                HttpResponseMessage message = await client.PostAsync("/api/tracker/control", content);

                trackerRelayButton = "0";
                relayON.BackgroundColor = Color.Gray;
                relayOFF.BackgroundColor = Color.Red;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetType().Name + ": " + ex.Message;
                trackerList.ItemsSource = new string[] { errorMessage };
            }

        }

        public async void ClickActiveON(object sender, EventArgs e)
        {
           
            TrackerControlModel data = new TrackerControlModel()
            {
                TrackerID = 1,
                LastChangeTime = DateTime.Now.ToString("yyyy'-'MM'-'ddTHH:mm:ss"),
                RelayControl = trackerRelayButton,
                GpsActive = "1",
                Queue = "Yes"
            };
           
            try
            {
                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://paikkatietoback1.azurewebsites.net");

                string input = JsonConvert.SerializeObject(data);
                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");

                HttpResponseMessage message = await client.PostAsync("/api/tracker/control", content);

                trackerActiveButton = "1";
                activeON.BackgroundColor = Color.Green;
                activeOFF.BackgroundColor = Color.Gray;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetType().Name + ": " + ex.Message;
                trackerList.ItemsSource = new string[] { errorMessage };
            }
        }
        public async void ClickActiveOFF(object sender, EventArgs e)
        {
            if (trackerRelay == null) { trackerRelay = "0"; }
            TrackerControlModel data = new TrackerControlModel()
            {
                TrackerID = 1,
                LastChangeTime = DateTime.Now.ToString("yyyy'-'MM'-'ddTHH:mm:ss"),
                RelayControl = trackerRelayButton,
                GpsActive = "0", 
                Queue = "Yes"
            };

            try
            {

                HttpClient client = new HttpClient();
                client.BaseAddress = new Uri("https://paikkatietoback1.azurewebsites.net");

                string input = JsonConvert.SerializeObject(data);
                StringContent content = new StringContent(input, Encoding.UTF8, "application/json");

                HttpResponseMessage message = await client.PostAsync("/api/tracker/control", content);

                trackerActiveButton = "0";
                activeON.BackgroundColor = Color.Gray;
                activeOFF.BackgroundColor = Color.Red;
            }
            catch (Exception ex)
            {
                string errorMessage = ex.GetType().Name + ": " + ex.Message;
                trackerList.ItemsSource = new string[] { errorMessage };
            }
        }
        private void AddMarkers()
        {

            //lat = Convert.ToDouble(latitudeLabel.Text) ;
            //lon = Convert.ToDouble(longitudeLabel.Text);
            Position loc1 = new Position(17.427579, 78.342017);
            Position loc2 = new Position(17.427579, 78.342017);
            
            Pin marker1 = new Pin()
            {
                Address = "Gachibowli",
                IsVisible = true,
                Label = "Microsoft Hyderabad",
                Position = loc1,
                Type = PinType.Place

            };
            Pin marker2 = new Pin()
            {
                Address = "Gachibowli",
                IsVisible = true,
                Label = "Wipro Hyderabad",
                Position = loc2,
                Type = PinType.Place

            };

            mapView.Pins.Add(marker1);
            mapView.Pins.Add(marker2);
            mapView.MoveToRegion(MapSpan.FromCenterAndRadius(marker1.Position, Distance.FromMeters(1000)));
        }
    }
}