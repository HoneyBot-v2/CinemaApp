using Cinema.MAUI.Models;

namespace Cinema.MAUI.Pages;

[QueryProperty(nameof(ScreeningId), "screeningId")]
public partial class SeatsPage : ContentPage
{
	public int ScreeningId { get; set; }
    public SeatsPage()
	{
		InitializeComponent();
	}
}