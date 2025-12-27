using Cinema.MAUI.Attributes;
using Cinema.MAUI.Models;

namespace Cinema.MAUI.Pages;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
        Token token = PreferenceHelper.Load<Token>();
		LblUserName.Text = token.UserName;
	}
}