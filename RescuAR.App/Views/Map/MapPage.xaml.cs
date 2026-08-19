using Microsoft.Maui.Controls;
using RescuAR.App.ViewModels.Map;

namespace RescuAR.App.Views.Map
{
    public partial class MapPage : ContentPage
    {
        public MapPage(MapViewModel viewModel)
        {
            InitializeComponent();
            BindingContext = viewModel;
        }

        protected override async void OnAppearing()
        {
            base.OnAppearing();
            
            if (BindingContext is MapViewModel vm)
            {
                await vm.InitializeMapAsync(MapControl);
                await vm.LoadMyCirclesAsync();
            }
        }
    }
}
