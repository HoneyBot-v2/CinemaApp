using Cinema.MAUI.Attributes;

namespace Cinema.MAUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Initial navigation is handled in App.CreateWindow to avoid flicker.
            // Register non-visual routes
            //Routing.RegisterRoute("registration", typeof(Pages.RegistrationPage));
            //Routing.RegisterRoute("login", typeof(Pages.LoginPage));
            Routing.RegisterRoute("seats", typeof(Pages.SeatsPage));
            Routing.RegisterRoute("ticketdetail", typeof(Pages.TicketDetailPage));
        }
    }
}
