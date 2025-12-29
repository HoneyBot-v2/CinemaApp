using Cinema.MAUI.Attributes;

namespace Cinema.MAUI
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // Register non-visual routes
            Routing.RegisterRoute("registration", typeof(Pages.RegistrationPage));
            Routing.RegisterRoute("login", typeof(Pages.LoginPage));
            // Initial navigation is handled in App.CreateWindow to avoid flicker.
        }
    }
}
