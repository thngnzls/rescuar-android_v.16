using Microsoft.Maui.Controls;
using RescuAR.App.ViewModels.Profile;

namespace RescuAR.App.Views.Profile
{
    public partial class EmergencyContactsPage : ContentPage
    {
        public EmergencyContactsPage(EmergencyContactsViewModel? viewModel = null)
        {
            InitializeComponent();
            BindingContext = viewModel ?? new EmergencyContactsViewModel();
        }
    }
}

