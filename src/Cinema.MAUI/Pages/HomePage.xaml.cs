using Cinema.MAUI.Attributes;
using Cinema.MAUI.Models;
using Cinema.MAUI.Services;

namespace Cinema.MAUI.Pages;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
        Token token = PreferenceHelper.Load<Token>();
		LblUserName.Text = token.UserName;
		TrendingMovies();
    }

	private async void TrendingMovies()
	{
        List<Movie> movies = await ApiService.GetMovies("trending");
		CvTrending.ItemsSource = movies;
	}
}