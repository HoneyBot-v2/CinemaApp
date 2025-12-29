using System.Collections.ObjectModel;
using Cinema.MAUI.Attributes;
using Cinema.MAUI.Models;
using Cinema.MAUI.Services;

namespace Cinema.MAUI.Pages;

public partial class SeatsPage : ContentPage, IQueryAttributable
{
    /// <summary>
    /// Gets or sets the <see cref="Screening"/> associated with this page.
    /// </summary>
    /// <remarks>
    /// This property is required and is typically populated via <see cref="ApplyQueryAttributes"/> 
    /// from the query parameters. It must be set before seat retrieval or navigation occurs.
    /// </remarks>
    public required Screening Screening { get; set; }
    public ObservableCollection<Seat> Seats { get; set; } = new ObservableCollection<Seat>();
    public List<int> SelectedSeats { get; set; } = new List<int>();
    /// <summary>
    /// Flag indicating whether the page has completed its initial load.
    /// </summary>
    bool _pageLoaded = false;

    public SeatsPage()
	{
		InitializeComponent();
        // Set the page itself as the binding source so XAML bindings like
        // `{Binding Seats}` and `{Binding Screening}` resolve to the public
        // properties on this class. Placed after `InitializeComponent()`
        // because the XAML for this page does not require the BindingContext
        // during initialization; assigning it here is sufficient and ensures
        // the visual tree is created before we attach the context.
        BindingContext = this;
	}

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        if (_pageLoaded) return;
        _pageLoaded = true;

        // Yield to ensure the UI thread is free before starting data fetch.
        // This helps with the previous page sometimes showing and pausing the
        // flyover. 
        await Task.Yield();
        await GetSeats();
    }

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        // Retrieve the passed Screening object from the query parameters
        if (query.TryGetValue(nameof(Screening), out var value) && value is Screening s)
        {
            Screening = s;
        }

        if (Screening == null)
        {
            DisplayAlertAsync("Error", "No screening data provided.", "OK");
            return;
        }

        Screening.ScreeningTime.ToString("dd MMM yyyy");
    }

    private async void ImgBtnBack_Clicked(object sender, EventArgs e)
    {
        await Shell.Current.GoToAsync("..");
    }

    private async void ImgBtn_Clicked(object sender, EventArgs e)
    {
        // Ensure the sender is an ImageButton
        if (sender is not ImageButton button)
        {
            return;
        }

        // Get the Seat object from the button's BindingContext
        if (button.BindingContext is not Seat seat)
        {
            return;
        }

        // Toggle seat selection if available
        if (seat.IsAvailable && !seat.IsSelected)
        {
            seat.IsSelected = true;
            SelectedSeats.Add(seat.Id);
        }
        else
        {
            seat.IsSelected = false;
            SelectedSeats.Remove(seat.Id);
        }

        // Update the seat numbers label
        var selectedSeatNumbers = string.Join(",", Seats.Where(s => s.IsSelected).Select(s => s.Row + s.SeatNumber));
        if (string.IsNullOrEmpty(selectedSeatNumbers))
        {
            LblSeatNumber.Text = "Seats: None";
        }
        else
        {
            LblSeatNumber.Text = $"Seats: {selectedSeatNumbers}";
        }

        decimal totalPrice = Seats.Count(x => x.IsSelected) * Screening.PricePerSeat;
        LblTicketPrice.Text = $"Total: ${totalPrice:0.00}";
    }

    private async void TapBuy_Tapped(object sender, EventArgs e)
    {
        // get user id from saved token
        Token token = PreferenceHelper.Load<Token>();
        var userId = token.UserId;

        // Reserve seats via API
        ReservationResponse reservationResponse = await ApiService.ReserveSeats(userId, Screening.Id, SelectedSeats);
        if (reservationResponse.Seats == null)
        {
            await DisplayAlertAsync("Error", "Failed to reserve seats. Please try again.", "Ok");
            return;
        }

        // Show success message and refresh seats
        await DisplayAlertAsync("Success", "Seats reserved successfully!", "Ok");
        await GetSeats();

        SelectedSeats.Clear();
        LblSeatNumber.Text = "Seats: None";
        LblTicketPrice.Text = "Total: $0.00";
    }

    private async Task GetSeats()
    {
        // Show loader and hide content while fetching data
        AiLoader.IsRunning = true;
        AiLoader.IsVisible = true;
        CvSeats.IsVisible = false;

        List<Seat> seats = await ApiService.GetAllSeats(Screening.Id);
        if (seats.Count == 0)
        {
            // if there are no seats, show alert and go back
            await DisplayAlertAsync("Info", "No seats for this screening.", "Ok");
            await Shell.Current.GoToAsync("..");
            return;
        }

        // Refresh the Seats collection in one go to avoid multiple change
        // notifications
        Seats = new ObservableCollection<Seat>(seats);
        OnPropertyChanged(nameof(Seats));

        // Hide loader and show content after data is fetched
        AiLoader.IsVisible = false;
        AiLoader.IsRunning = false;
        CvSeats.IsVisible = true;
    }
}