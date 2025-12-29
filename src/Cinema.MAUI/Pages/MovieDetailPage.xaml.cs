
using Cinema.MAUI.Models;
using Cinema.MAUI.Services;

namespace Cinema.MAUI.Pages;

public partial class MovieDetailPage : ContentPage
{
	public MovieDetailPage(int id)
	{
		InitializeComponent();
		MovieDetail(id);
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
}