using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace App1_malliksi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TabbedPage1 : TabbedPage
    {
        public TabbedPage1()
        {
            InitializeComponent();
            trackerList.ItemsSource = new string[] { "" };
           // trackerList.ItemSelected += TrackerList_ItemSelected;
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
            string[] tmp = tracker.Split(' ');
            string trackerID = tmp[0];
            if (!string.IsNullOrEmpty(tracker))
            {

                try
                {
                    HttpClient client = new HttpClient();
                    client.BaseAddress = new Uri("https://paikkatietoback1.azurewebsites.net");

                    string json = await client.GetStringAsync("/api/tracker/info?trackerID=" + trackerID);
                    //string[] trackerinfo = JsonConvert.DeserializeObject<string[]>(json);
                    string[] tmp2 = json.Split(' ');
                    string trackerRelay = tmp2[0];
                    string trackerActive = tmp2[1];

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
    }
}