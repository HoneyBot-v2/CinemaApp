using System;
using Cinema.MAUI.Services;
using Microsoft.Maui.Controls;

namespace Cinema.MAUI.Pages
{
    public partial class LoginPage : ContentPage
    {
        public LoginPage()
        {
            InitializeComponent();
        }

        private async void BtnLogin_Clicked(object sender, EventArgs e)
        {
            try
            {
                var response = await ApiService.Login(EntEmail.Text, EntPassword.Text);

                if (response)
                {
                    // (recommended) Newer approach with Shell
                    await Shell.Current.GoToAsync($"//home");
                }
                else
                {
                    await DisplayAlertAsync("Oops", "Failed to login.", "Ok");
                }
            }
            catch (Exception ex)
            {
                var errors = new List<string>();
                for (var i = 0; i < 10; i++)
                {
                    if (ex == null)
                    {
                        break;
                    }

                    errors.Add(ex.Message);
                    ex = ex.InnerException;
                }
                var msg = string.Join($"{Environment.NewLine}\u2192", errors);
                await DisplayAlertAsync("Error", msg, "Ok");
            }
        }

        private void TapGester_Tapped(object sender, TappedEventArgs e)
        {

        }
    }
}