using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.Controls;

namespace RescuAR.App.ViewModels.Profile;

public class PolicySectionItem
{
    public string Number { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public List<string> BulletPoints { get; set; } = new();
    public bool HasBullets => BulletPoints != null && BulletPoints.Count > 0;
}

public partial class PrivacyPolicyViewModel : ObservableObject
{
    public List<PolicySectionItem> PolicySections { get; } = new()
    {
        new PolicySectionItem
        {
            Number = "1",
            Title = "Information We Collect",
            Description = "We collect limited data necessary to provide evacuation guidance:",
            BulletPoints = new()
            {
                "Location Data – to determine your position and nearby evacuation routes",
                "Camera Access – to enable Augmented Reality (AR) navigation",
                "Motion & Orientation Data – to align AR directions with your movement",
                "Device Information – for system performance and compatibility"
            }
        },
        new PolicySectionItem
        {
            Number = "2",
            Title = "How We Use Your Information",
            Description = "Your data is used solely to:",
            BulletPoints = new()
            {
                "Provide real-time evacuation guidance",
                "Display hazard alerts and safe routes",
                "Improve navigation accuracy and system performance"
            }
        },
        new PolicySectionItem
        {
            Number = "3",
            Title = "Data Storage and Security",
            Description = "We do not store personal location data permanently. All data is used in real-time and handled securely to prevent unauthorized access."
        },
        new PolicySectionItem
        {
            Number = "4",
            Title = "Data Sharing",
            Description = "We do not sell or share your personal data with third parties. Data may only be used with verified sources for disaster-related updates."
        },
        new PolicySectionItem
        {
            Number = "5",
            Title = "User Control",
            Description = "You may disable permissions (location, camera, motion) at any time through your device settings. However, doing so may limit core app functionality."
        },
        new PolicySectionItem
        {
            Number = "6",
            Title = "Updates to This Policy",
            Description = "This policy may be updated to reflect improvements in the system. Continued use of the app means you accept these updates."
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

