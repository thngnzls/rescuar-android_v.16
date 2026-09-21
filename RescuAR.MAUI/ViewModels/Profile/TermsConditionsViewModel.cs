using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace RescuAR.App.ViewModels.Profile;

public class TermsSectionItem
{
    public string Number { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> BulletPoints { get; set; } = new();
    public bool HasBullets => BulletPoints != null && BulletPoints.Count > 0;
}

public partial class TermsConditionsViewModel : ObservableObject
{
    public List<TermsSectionItem> TermsSections { get; } = new()
    {
        new TermsSectionItem
        {
            Number = "1",
            Title = "Use of the Application",
            Description = "This application is intended to assist users in disaster preparedness and evacuation. It should be used as a support tool, not as a sole source of decision-making during emergencies."
        },
        new TermsSectionItem
        {
            Number = "2",
            Title = "Accuracy of Information",
            Description = "While the app uses verified data sources, real-time conditions may change rapidly. Users are advised to remain aware of their surroundings at all times."
        },
        new TermsSectionItem
        {
            Number = "3",
            Title = "AR Guidance Limitations",
            Description = "Augmented Reality (AR) navigation depends on device sensors and environmental conditions.",
            BulletPoints = new()
            {
                "Accuracy may be affected by poor lighting, obstructions, or signal loss",
                "Directions provided should be followed with caution"
            }
        },
        new TermsSectionItem
        {
            Number = "4",
            Title = "Connectivity Requirements",
            Description = "Some features require internet access, including hazard updates and route recalculations. Limited functionality may be available offline."
        },
        new TermsSectionItem
        {
            Number = "5",
            Title = "User Responsibility",
            Description = "Users are responsible for:",
            BulletPoints = new()
            {
                "Staying alert during evacuation",
                "Following official instructions from authorities",
                "Using the app appropriately in emergency situations"
            }
        },
        new TermsSectionItem
        {
            Number = "6",
            Title = "Limitation of Liability",
            Description = "The developers are not liable for any damages, injuries, or losses resulting from the use or inability to use the application."
        },
        new TermsSectionItem
        {
            Number = "7",
            Title = "Updates and Changes",
            Description = "The application may be updated to improve performance and features. Continued use signifies acceptance of any changes."
        }
    };

    [RelayCommand]
    private async Task BackAsync()
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

