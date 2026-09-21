using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using RescuAR.App.Views.Authentication;

using RescuAR.App.Services.Authentication;

using RescuAR.MAUI;

namespace RescuAR.App.ViewModels.Authentication
{
    public partial class SplashViewModel : ObservableObject
    {
        private readonly IServiceProvider _serviceProvider;

        [ObservableProperty]
        private string _statusText = "Loading safety resources...";

        [ObservableProperty]
        private string _versionText = "v0.0.1a";

        public SplashViewModel(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task InitializeAsync()
        {
            try
            {
                StatusText = "Connecting to disaster telemetry...";
                var client = await RescuAR.Services.SupabaseService.Instance.GetClientAsync();
                if (client?.Auth.CurrentUser != null)
                {
                    Preferences.Default.Set("current_user_id", client.Auth.CurrentUser.Id);
                    if (!string.IsNullOrWhiteSpace(client.Auth.CurrentUser.Email))
                    {
                        Preferences.Default.Set("UserEmail", client.Auth.CurrentUser.Email);
                    }
                }
            }
            catch { }

            StatusText = "Loading offline evacuation maps...";
            await Task.Delay(1200);

            StatusText = "Starting safety system...";
            await Task.Delay(400);

            bool isLoggedIn = Preferences.Default.Get("IsLoggedIn", false);
            bool hasSignedUp = Preferences.Default.Get("HasSignedUp", false);

            MainThread.BeginInvokeOnMainThread(() =>
            {
                if (Application.Current != null)
                {
                    if (isLoggedIn)
                    {
                        bool hasPermissions = Preferences.Default.Get("HasCompletedPermissions", false);
                        if (hasPermissions)
                        {
                            AuthenticationNavigation.TrySetRootPage(new AppShell());
                        }
                        else
                        {
                            var permissionsPage = _serviceProvider.GetRequiredService<PermissionsPage>();
                            AuthenticationNavigation.TrySetRootPage(permissionsPage);
                        }
                    }
                    else if (hasSignedUp)
                    {
                        var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
                        AuthenticationNavigation.TrySetRootPage(new NavigationPage(loginPage));
                    }
                    else
                    {
                        var onboardingPage = _serviceProvider.GetRequiredService<OnboardingPage>();
                        AuthenticationNavigation.TrySetRootPage(new NavigationPage(onboardingPage));
                    }
                }
            });
        }
    }
}



