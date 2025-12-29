using Cinema.MAUI.Models;
using Cinema.MAUI.Services;

namespace Cinema.MAUI.Pages;

public partial class MovieDetailPage : ContentPage
{
    private bool _isDescriptionExpanded = false;
    public MovieDetailPage(int id)
	{
		InitializeComponent();
		MovieDetail(id);
        // Show only 3 lines of description initially
        LblMovieDescription.MaxLines = 3;
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

    private void TapGestureRecognizer_Tapped(object sender, TappedEventArgs e)
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
}