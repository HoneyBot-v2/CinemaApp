using System.Threading.Tasks;
using Cinema.MAUI.Attributes;
using Cinema.MAUI.Models;
using Cinema.MAUI.Services;

namespace Cinema.MAUI.Pages;

public partial class HomePage : ContentPage
{
	public HomePage()
	{
		InitializeComponent();
        // Do not access preferences or start network calls in the constructor.
        // Content loading will be handled in OnAppearing after auth checks.
    }

    protected override async void OnAppearing()
    {
        try
        {
            base.OnAppearing();
            Token? token = PreferenceHelper.Load<Token>();
            if (token is null || !token.Validate())
            {
                await Shell.Current.GoToAsync("registration");
                return;
            }

            // Update UI with authenticated user info
            LblUserName.Text = token.UserName;

            // Populate movie collection views
            // Fire and await to avoid unobserved exceptions
            await TrendingMoviesInternal();
            await NowPlayingMoviesInternal();
            await LatestMoviesInternal();
        }
        catch (Exception ex)
        {
            var errors = new List<string>();
            for (var i = 0; i < 10; i++)
            {
                errors.Add(ex.Message);
                if (ex.InnerException == null)
                { 
                    break;
                }
                ex = ex.InnerException;
            }

            var msg = string.Join($"{Environment.NewLine}\u2192", errors);
            await Dispatcher.DispatchAsync(async () =>
            {
                await DisplayAlertAsync("Error", msg, "Ok");
            });
        }
    }

    private async Task TrendingMoviesInternal()
	{
        List<Movie> movies = await ApiService.GetMovies("trending");
		CvTrending.ItemsSource = movies;
	}

    private async Task NowPlayingMoviesInternal()
	{
		List<Movie> movies = await ApiService.GetMovies("nowplaying");
		CvNowPlaying.ItemsSource = movies;
    }

    private async Task LatestMoviesInternal()
	{
		List<Movie> movies = await ApiService.GetMovies("latest");
		CvLatest.ItemsSource = movies;
	}

    private async void CvNowPlaying_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Movie? movie = e.CurrentSelection.FirstOrDefault() as Movie;
		if (movie == null)
		{
			return;
		}

        await Navigation.PushModalAsync(new MovieDetailPage(movie.Id));
		// Clear selection so the same item can be selected again and retrigger SelectionChanged
		((CollectionView)sender).SelectedItem = null;
    }

    private async Task CvLatest_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Movie? movie = e.CurrentSelection.FirstOrDefault() as Movie;
        if(movie == null)
        {
            return;
        }

        await Navigation.PushModalAsync(new MovieDetailPage(movie.Id));
        // Clear selection so the same item can be selected again and retrigger SelectionChanged
        ((CollectionView)sender).SelectedItem = null;
    }
}