using Microsoft.Maui.Controls;
using RescuAR.App.ViewModels.Profile;

namespace RescuAR.App.Views.Profile
{
    public partial class AppSettingsPage : ContentPage
    {
        public AppSettingsPage(AppSettingsViewModel? viewModel = null)
        {
            InitializeComponent();
            BindingContext = viewModel ?? new AppSettingsViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is AppSettingsViewModel vm)
            {
                await vm.CheckPermissionsAsync();
            }
        }
    }
}

