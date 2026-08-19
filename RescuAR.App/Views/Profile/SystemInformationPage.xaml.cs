using Microsoft.Maui.Controls;

namespace RescuAR.App.Views.Profile
{
    public partial class SystemInformationPage : ContentPage
    {
        public SystemInformationPage()
        {
            InitializeComponent();
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            if (BindingContext is RescuAR.App.ViewModels.Profile.SystemInformationViewModel vm)
            {
                await vm.LoadPermissionsAsync();
            }
        }

        private async void OnBackTapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
