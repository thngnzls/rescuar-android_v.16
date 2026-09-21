using RescuAR.App.ViewModels.Authentication;

namespace RescuAR.App.Views.Authentication;

public partial class SplashPage : ContentPage
{
    public SplashPage(SplashViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // Smooth modern entrance animations
        _ = Task.Run(async () =>
        {
            await Task.Delay(100);
            await MainThread.InvokeOnMainThreadAsync(async () =>
            {
                await Task.WhenAll(
                    BrandContainer.FadeTo(1, 600, Easing.CubicOut),
                    BrandContainer.TranslateTo(0, 0, 600, Easing.CubicOut),
                    LoadingContainer.FadeTo(1, 800, Easing.CubicOut)
                );
            });
        });

        if (BindingContext is SplashViewModel viewModel)
        {
            await viewModel.InitializeAsync();
        }
    }
}


