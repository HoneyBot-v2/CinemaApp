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
            // This is functionally similar to: "MainPage = new NavigationPage(new RegistrationPage());"
            // but using CreateWindow is preferred in .NET MAUI because:
            // - It gives you direct control over the Window instance, which is important on desktop.
            // - It enables multi-window scenarios and per-window configuration.
            // - It provides a consistent startup path for activation, deep links, and file associations.
            // Use Shell as the app root so Shell.Current is available
            return new Window(new AppShell());
        }
    }
}