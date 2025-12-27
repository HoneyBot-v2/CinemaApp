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

		// Populate movie collection views
		TrendingMovies();
		NowPlayingMovies();
		LatestMovies();
    }

	private async void TrendingMovies()
	{
        List<Movie> movies = await ApiService.GetMovies("trending");
		CvTrending.ItemsSource = movies;
	}

	private async void NowPlayingMovies()
	{
		List<Movie> movies = await ApiService.GetMovies("nowplaying");
		CvNowPlaying.ItemsSource = movies;
    }

	private async void LatestMovies()
	{
		List<Movie> movies = await ApiService.GetMovies("latest");
		CvLatest.ItemsSource = movies;
	}
}