using System.Threading.Tasks;
using Cinema.MAUI.Services;

namespace Cinema.MAUI.Pages;

public partial class RegistrationPage : ContentPage
{
	public RegistrationPage()
	{
		InitializeComponent();
	}

    private async void BtnRegister_Clicked(object sender, EventArgs e)
    {
        try
        {
            bool result = await ApiService.RegisterUser(EntName.Text, EntEmail.Text, EntPassword.Text);

            if (result)
            {
                // navigate to login page
                await DisplayAlertAsync("Success", "Registration successful!", "Ok");
                await Navigation.PushAsync(new LoginPage());
            }
            else
            {
                await DisplayAlertAsync("Opps", "Registration failed. Please try again.", "Cancel");
            }
        }
        catch (Exception? ex)
        {
            // Collect all error messages from the exception chain
            var errors = new List<string>();
            while (ex != null)
            {
                errors.Add(ex.Message);
                ex = ex.InnerException;
            }

            // Join errors with arrow symbol for better readability
            var msg = string.Join($"{Environment.NewLine}\u2192 ", errors);
            await DisplayAlertAsync("Error", msg, "Ok");
        }
    }

    private async void TapLogin_Tapped(object sender, TappedEventArgs e)
    {
        await Navigation.PushAsync(new LoginPage());
    }
}