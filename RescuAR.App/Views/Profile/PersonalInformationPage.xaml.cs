using Microsoft.Maui.Controls;
using RescuAR.App.ViewModels.Profile;

namespace RescuAR.App.Views.Profile
{
    public partial class PersonalInformationPage : ContentPage
    {
        public PersonalInformationPage()
        {
            InitializeComponent();
            BindingContext = new ProfileViewModel();
        }

        private async void OnBackTapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
