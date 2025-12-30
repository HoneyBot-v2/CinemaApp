using Cinema.MAUI.Models;
using Cinema.MAUI.Services;

namespace Cinema.MAUI.Pages;

public partial class TicketsPage : ContentPage
{
	public TicketsPage()
	{
		InitializeComponent();
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        // Yield to ensure the UI thread is free before starting data fetch.
        await Task.Yield();

        try
        {
            await GetReservations();
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

    private async Task GetReservations()
    {
        List<Reservation> reservations = await ApiService.GetReservations();
		CvReservations.ItemsSource = reservations;
    }

    private async void CvReservations_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        Reservation? reservation = e.CurrentSelection.FirstOrDefault() as Reservation;

        // this can guard agains re-entry after se set the selected item to null
        if (reservation == null)
        {
            return;
        }

        await Shell.Current.GoToAsync("ticketdetail", new Dictionary<string, object>
        {
            { "Reservation", reservation }
        });

        // Deselect item. This will trigger another selection change we must
        // guard against.
        ((CollectionView) sender).SelectedItem = null;
    }
}