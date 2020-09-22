using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using System.Security.Cryptography;
using System.Net.Http;
using System.Net.Http.Headers;

namespace App1_malliksi
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }
        public async void KirjauduSisaan(object sender, EventArgs e)
        {
            string username = "";
            string password = "";

            if (!string.IsNullOrEmpty(Sana.Text) && !string.IsNullOrEmpty(Tunnus.Text))
            {
                using (var sha = SHA256.Create())
                {
                    var bytes = Encoding.UTF8.GetBytes(Sana.Text);
                    var hash = sha.ComputeHash(bytes);


                    password = Convert.ToBase64String(hash);
                    username = Tunnus.Text;
                    //return Convert.ToBase64String(hash);
                }
            }
            else
            {
                await DisplayAlert("", "Syötä käyttäjätunnus ja salasana.", "OK");
            }


            if (!string.IsNullOrEmpty(password) && !string.IsNullOrEmpty(username))
            {

                HttpClient client = new HttpClient();
                var uri = new Uri(string.Format("https://paikkatietoback1.azurewebsites.net/api/login/user?Username=" + username + "&Password=" + password));

                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                HttpResponseMessage response = await client.GetAsync(uri);
                if (response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    var errorMessage1 = response.Content.ReadAsStringAsync().Result.Replace("\\", "").Trim(new char[1] {'"'});
                    //await DisplayAlert("Login succesfully !", errorMessage1, "Close");
                   
                    await Navigation.PushAsync(new TabbedPage2());
                }
                else
                {
                    var errorMessage1 = response.Content.ReadAsStringAsync().Result.Replace("\\", "").Trim(new char[1] {'"'});
                    await DisplayAlert("", errorMessage1, "Close");
                    //Toast.MakeText(this, errorMessage1, ToastLength.Long).Show();
                }
            }

        }
    }
}