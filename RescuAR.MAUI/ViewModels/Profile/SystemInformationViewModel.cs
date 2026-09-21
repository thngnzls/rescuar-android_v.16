using System;
using System.ComponentModel;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;

namespace RescuAR.App.ViewModels.Profile;

public partial class SystemInformationViewModel : ObservableObject
{
    [ObservableProperty]
    private string _cameraPermission = "Checking...";

    [ObservableProperty]
    private string _locationPermission = "Checking...";

    [ObservableProperty]
    private string _motionPermission = "Checking...";

    [ObservableProperty]
    private string _notificationsPermission = "Checking...";

    public string AppVersion
    {
        get
        {
            try
            {
                var v = AppInfo.Current.VersionString;
                return string.IsNullOrWhiteSpace(v) ? "v1.0.0" : $"v{v}";
            }
            catch
            {
                return "v1.0.0";
            }
        }
    }

    public string LastUpdated
    {
        get
        {
            try
            {
                var dir = AppContext.BaseDirectory;
                if (!string.IsNullOrWhiteSpace(dir) && System.IO.Directory.Exists(dir))
                {
                    return System.IO.Directory.GetLastWriteTime(dir).ToString("MM/dd/yyyy hh:mm tt");
                }
            }
            catch { }
            return DateTime.Now.ToString("MM/dd/yyyy hh:mm tt");
        }
    }


    public SystemInformationViewModel()
    {
        _ = LoadPermissionsAsync();
    }

    public async Task LoadPermissionsAsync()
    {
        try
        {
            var camera = await Permissions.CheckStatusAsync<Permissions.Camera>();
            CameraPermission = camera == PermissionStatus.Granted ? "Allowed" : "Denied";

            var location = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            LocationPermission = location == PermissionStatus.Granted ? "Allowed" : "Denied";

            var motion = await Permissions.CheckStatusAsync<Permissions.Sensors>();
            MotionPermission = motion == PermissionStatus.Granted ? "Allowed" : "Denied";

#if ANDROID
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                var push = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
                NotificationsPermission = push == PermissionStatus.Granted ? "Allowed" : "Denied";
            }
            else
            {
                NotificationsPermission = "Allowed";
            }
#else
            NotificationsPermission = "Allowed";
#endif
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error checking permissions: {ex.Message}");
        }
    }

    [RelayCommand]
    private void ManagePermissions()
    {
        try
        {
            AppInfo.ShowSettingsUI();
        }
        catch { }
    }

    [RelayCommand]
    private async Task BackAsync()
    {
        if (Shell.Current != null)
        {
            await Shell.Current.GoToAsync("..");
        }
    }
}

