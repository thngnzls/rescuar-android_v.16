using Microsoft.Maui.Controls;

namespace RescuAR.App.Views.Profile
{
    public partial class TermsConditionsPage : ContentPage
    {
        public TermsConditionsPage()
        {
            InitializeComponent();
        }

        private async void OnBackTapped(object sender, TappedEventArgs e)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}
