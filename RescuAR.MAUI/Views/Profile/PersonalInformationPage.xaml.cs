using Microsoft.Maui.Controls;
using RescuAR.App.ViewModels.Profile;

namespace RescuAR.App.Views.Profile
{
    public partial class PersonalInformationPage : ContentPage
    {
        public PersonalInformationPage(PersonalInformationViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        public PersonalInformationPage()
        {
            InitializeComponent();
            BindingContext = new PersonalInformationViewModel();
        }
    }
}
