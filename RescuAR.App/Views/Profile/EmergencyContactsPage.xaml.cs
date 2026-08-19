using Microsoft.Maui.Controls;
using RescuAR.App.ViewModels.Profile;

namespace RescuAR.App.Views.Profile
{
    public partial class EmergencyContactsPage : ContentPage
    {
        public EmergencyContactsPage()
        {
            InitializeComponent();
            BindingContext = new ProfileViewModel();
        }

        private async void OnBackTapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }

        private void OnCallContact1Clicked(object sender, EventArgs e)
        {
            if (BindingContext is RescuAR.App.ViewModels.Profile.ProfileViewModel vm && !string.IsNullOrWhiteSpace(vm.CurrentUser?.EmergencyContact1Phone))
            {
                if (Microsoft.Maui.ApplicationModel.Communication.PhoneDialer.Default.IsSupported)
                    Microsoft.Maui.ApplicationModel.Communication.PhoneDialer.Default.Open(vm.CurrentUser.EmergencyContact1Phone);
            }
        }

        private void OnCallContact2Clicked(object sender, EventArgs e)
        {
            if (BindingContext is RescuAR.App.ViewModels.Profile.ProfileViewModel vm && !string.IsNullOrWhiteSpace(vm.CurrentUser?.EmergencyContact2Phone))
            {
                if (Microsoft.Maui.ApplicationModel.Communication.PhoneDialer.Default.IsSupported)
                    Microsoft.Maui.ApplicationModel.Communication.PhoneDialer.Default.Open(vm.CurrentUser.EmergencyContact2Phone);
            }
        }
    }
}
