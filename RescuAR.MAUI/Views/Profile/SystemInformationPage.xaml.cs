using Microsoft.Maui.Controls;
using RescuAR.App.ViewModels.Profile;

namespace RescuAR.App.Views.Profile
{
    public partial class SystemInformationPage : ContentPage
    {
        public SystemInformationPage(SystemInformationViewModel? viewModel = null)
        {
            InitializeComponent();
            BindingContext = viewModel ?? new SystemInformationViewModel();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is SystemInformationViewModel vm)
            {
                await vm.LoadPermissionsAsync();
            }
        }
    }
}

