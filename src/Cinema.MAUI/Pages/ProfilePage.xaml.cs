using Cinema.MAUI.Attributes;

namespace Cinema.MAUI.Pages;

public partial class ProfilePage : ContentPage
{
	public ProfilePage()
	{
		InitializeComponent();
	}

    private async void TapLogout_Tapped(object sender, TappedEventArgs e)
    {
		var confirm = await DisplayAlertAsync("Confirm Logout", "Are you sure you want to logout?", "Yes", "No");
		if (!confirm)
		{
			return;
		}

		PreferenceHelper.RemoveAll<Cinema.MAUI.Models.Token>();
		// This will still allow you to go back but for a sandbox app i am okay
		// with it... for now...
		await Shell.Current.GoToAsync("//registration");
    }
}