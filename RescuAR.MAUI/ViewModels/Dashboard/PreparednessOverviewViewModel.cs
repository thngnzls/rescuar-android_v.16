using System;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;
using RescuAR.App.Services.Dashboard;

namespace RescuAR.App.ViewModels.Dashboard;

public partial class PreparednessOverviewViewModel : ObservableObject
{
    private readonly IDashboardDataService _dataService;

    [ObservableProperty]
    private string _title = string.Empty;

    [ObservableProperty]
    private string _subtitle = string.Empty;

    [ObservableProperty]
    private string _actionText = string.Empty;

    [ObservableProperty]
<<<<<<< HEAD:RescuAR.App/ViewModels/Dashboard/PreparednessOverviewViewModel.cs
    public partial string ModuleRoute { get; set; } = "Prepare/Checklist";
=======
    private string _moduleRoute = "PreparePage";
>>>>>>> upstream/dev:RescuAR.MAUI/ViewModels/Dashboard/PreparednessOverviewViewModel.cs

    [ObservableProperty]
    private string _moduleName = string.Empty;

    public PreparednessOverviewViewModel() : this(DashboardDataService.Instance)
    {
    }

    public PreparednessOverviewViewModel(IDashboardDataService dataService)
    {
        _dataService = dataService;
        _ = LoadDataAsync();
    }

    private async Task LoadDataAsync()
    {
        var data = await _dataService.GetPreparednessDataAsync();
        Title = $"{data.PercentReady}% Ready";
        Subtitle = $"{data.PreparedItems} of {data.TotalItems} items prepared";
        ActionText = data.ActionText;
<<<<<<< HEAD:RescuAR.App/ViewModels/Dashboard/PreparednessOverviewViewModel.cs
        ModuleRoute = "Prepare/Checklist";
=======
        ModuleRoute = "PreparePage";
>>>>>>> upstream/dev:RescuAR.MAUI/ViewModels/Dashboard/PreparednessOverviewViewModel.cs
        ModuleName = data.ModuleName;
    }

    [RelayCommand]
    private async Task NavigateToPreparationProgressAsync()
    {
        if (Shell.Current != null)
        {
            try
            {
<<<<<<< HEAD:RescuAR.App/ViewModels/Dashboard/PreparednessOverviewViewModel.cs
                if (Shell.Current.Navigation != null)
                {
                    await Shell.Current.Navigation.PushAsync(new Views.Prepare.ChecklistPage());
                    return;
                }
                await Shell.Current.GoToAsync("Prepare/Checklist");
=======
                await Shell.Current.GoToAsync("PreparePage");
>>>>>>> upstream/dev:RescuAR.MAUI/ViewModels/Dashboard/PreparednessOverviewViewModel.cs
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Navigation error: {ex.Message}");
            }
        }
    }
}
