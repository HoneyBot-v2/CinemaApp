
using Cinema.MAUI.Models;

namespace Cinema.MAUI.Pages;

public partial class TicketDetailPage : ContentPage, IQueryAttributable
{
    /// <summary>
    /// Gets or sets the reservation details associated with this instance.
    /// </summary>
    /// <remarks>
    /// This property is required and must be provided via <see
    /// cref="ApplyQueryAttributes"/>
    /// </remarks>>
    public required Reservation Reservation { get; set; }
    public TicketDetailPage()
	{
		InitializeComponent();
	}

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.TryGetValue(nameof(Reservation), out var value) && value is Reservation r)
        {
            Reservation = r;
        }

        if (Reservation == null)
        {
            DisplayAlertAsync("Error", "No reservation data provided.", "OK");
            return;
        }

        ImgMovieUrl.Source = Reservation.MovieImgUrl;
        LblMovieTitle.Text = Reservation.MovieTitle;
        LblDate.Text = Reservation.ReservationDate.ToString("dd MMM yyyy");
        LblPrice.Text = $"{Reservation.Amount}";
        LblTicketId.Text = $"{Reservation.Id}";
        LblSeats.Text = Reservation.SeatNumbersDisplay;
        BarcodeValue.Value = $"Ticket Number ${LblTicketId.Text}";
    }

    private async void ImgBtnBack_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }
}