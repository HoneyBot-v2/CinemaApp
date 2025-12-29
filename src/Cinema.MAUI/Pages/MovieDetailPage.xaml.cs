using Cinema.MAUI.Models;
using Cinema.MAUI.Services;

namespace Cinema.MAUI.Pages;

public partial class MovieDetailPage : ContentPage
{
    private bool _isDescriptionExpanded = false;
    private Screening _screening;

    public MovieDetailPage(int id)
	{
		InitializeComponent();
        // Show only 3 lines of description initially
        LblMovieDescription.MaxLines = 3;
		MovieDetail(id);
        MovieScreening(id);
	}

    private async void MovieDetail(int id)
    {
        MovieDetailes movie = await ApiService.GetMovieDetails(id);
		ImgMovie.Source = movie.ImageUrl;
		LblMovieDescription.Text = movie.Description;
		LblMovieTitle.Text = movie.Title;
    }

    private async void ImgBtnBack_Clicked(object sender, EventArgs e)
    {
        // use Shell navigation to go back to the previous page
        await Shell.Current.GoToAsync("..");
    }

    private async void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
    {
        if (_isDescriptionExpanded)
        {
            // Collapse the description to 3 lines
            LblMovieDescription.MaxLines = 3;
            LblReadMore.Text = "Read More";
        }
        else
        {
            // Expand the description to show all lines
            LblMovieDescription.MaxLines = int.MaxValue;
            LblReadMore.Text = "Read Less";
        }
        _isDescriptionExpanded = !_isDescriptionExpanded;
    }

    private async void BtnReservation_Clicked(object sender, EventArgs e)
    {
        if (_screening != null)
        {
            // Cource uses:
            // Navigation.PushModalAsync(new SeatsPage());

            // We use Shell navigation with query parameters instead which gives
            // us this:
            await Shell.Current.GoToAsync($"seats?screaningId={_screening.Id}");
            return;
        }

        await DisplayAlertAsync("Oops", "Please slect a movie time", "Ok");
    }

    private async void CvMovieScreening_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Screening? screening = e.CurrentSelection.FirstOrDefault() as Screening;
        if (screening == null)
        {
            return;
        }

        _screening = screening;
    }

    private async void MovieScreening(int id)
    {
        List<Screening> movieScreenings = await ApiService.GetMovieScreening(id);
        CvMovieScreening.ItemsSource = movieScreenings;
    }
}