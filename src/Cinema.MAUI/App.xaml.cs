using Cinema.MAUI.Pages;
using Microsoft.Extensions.DependencyInjection;

namespace Cinema.MAUI
{
    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        // We override CreateWindow instead of setting MainPage to:
        // - control the actual Window instance (needed for desktop platforms)
        // - enable window-specific configuration and multi-window scenarios
        // - handle activation/deep-link/file-association startup paths
        //   consistently
        //
        // Note: Many tutorials set MainPage directly (e.g., "MainPage = new
        // RegistrationPage();"). That is valid for simple single-window apps or
        // when using AppShell. We use CreateWindow here to keep control over
        // the Window for desktop and future multi-window needs.
        protected override Window CreateWindow(IActivationState? activationState)
        {
            return new Window(new RegistrationPage());
        }
    }
}