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

            // Determine the first page based on the presence of a saved token
            Models.Token tok = PreferenceHelper.Load<Models.Token>();
            string firstPage = string.Empty;

            if (tok.Validate())
            {
                firstPage = "//home";                
            }
            else
            {
                firstPage = "registration";
            }

            // Navigate to the Registration on startup as a modal
            Dispatcher.DispatchAsync(async () =>
            {
                // Modal presentation defined on the RegistrationPage via Shell attributes
                await Shell.Current.GoToAsync(firstPage);
            });
        }
    }
}
