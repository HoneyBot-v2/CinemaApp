using Cinema.MAUI.Attributes;
using Cinema.MAUI.Pages;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui;
using Microsoft.Maui.Controls;

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
        // - enable multi-window scenarios
        // - handle activation/deep-link/file-association startup paths
        //   consistently
        //
        // Note: Many tutorials set MainPage directly (e.g., "MainPage = new
        // RegistrationPage();"). That is valid for simple single-window apps or
        // when using AppShell. We use CreateWindow here to keep control over
        // the Window for desktop and future multi-window needs.
        protected override Window CreateWindow(IActivationState? activationState)
        {
            var shell = new AppShell();
            var token = PreferenceHelper.Load<Models.Token>();

            // Ensure Shell is always the root so Shell.Current is non-null and
            // routes are registered
            var window = new Window(shell);

            if (token != null && token.Validate())
            {
                // Authenticated: stay on default shell content (e.g., Home)
                // Optionally navigate to a home route if needed (but this
                // should not be needed):
                // await shell.GoToAsync("home");
            }
            else
            {
                // Unauthenticated: navigate to registration within shell
                // Avoid awaiting here to prevent startup flicker;
                // fire-and-forget is acceptable during window creation.
                _ = shell.GoToAsync("//registration");
            }

            return window;
        }
    }
}