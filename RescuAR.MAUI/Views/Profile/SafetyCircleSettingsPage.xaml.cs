using Microsoft.Maui.Controls;
using RescuAR.App.ViewModels.Profile;

namespace RescuAR.App.Views.Profile
{
    public partial class SafetyCircleSettingsPage : ContentPage
    {
        public SafetyCircleSettingsViewModel ViewModel => (SafetyCircleSettingsViewModel)BindingContext;

        public SafetyCircleSettingsPage(SafetyCircleSettingsViewModel? viewModel = null)
        {
            InitializeComponent();
            BindingContext = viewModel ?? new SafetyCircleSettingsViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is SafetyCircleSettingsViewModel vm)
            {
                await vm.LoadCircleDataAsync();
            }
        }
    }
}

