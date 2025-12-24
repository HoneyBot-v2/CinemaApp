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

            // Navigate to the Registration on startup as a modal
            Dispatcher.DispatchAsync(async () =>
            {
                // Modal presentation defined on the RegistrationPage via Shell attributes
                await Shell.Current.GoToAsync("registration");
            });
        }
    }
}
