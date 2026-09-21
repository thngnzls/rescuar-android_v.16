using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;

namespace RescuAR.App.ViewModels.Profile;

public partial class AppSettingsViewModel : ObservableObject
{
    private bool _isInitializing = true;

    [ObservableProperty]
    private bool _pushNotificationsEnabled;

    [ObservableProperty]
    private bool _emergencySirenEnabled;

    [ObservableProperty]
    private bool _offlineMapCachingEnabled;

    [ObservableProperty]
    private bool _hapticFeedbackEnabled;

    [ObservableProperty]
    private string _cacheSizeText = "Calculating...";

    [ObservableProperty]
    private bool _isCameraGranted;

    [ObservableProperty]
    private bool _isLocationGranted;

    [ObservableProperty]
    private bool _isNotificationsGranted;

    public AppSettingsViewModel()
    {
        _ = InitializeSettingsAsync();
    }

    public async Task InitializeSettingsAsync()
    {
        _isInitializing = true;
        try
        {
            PushNotificationsEnabled = Preferences.Default.Get("PushNotificationsEnabled", true);
            EmergencySirenEnabled = Preferences.Default.Get("EmergencySirenEnabled", true);
            OfflineMapCachingEnabled = Preferences.Default.Get("OfflineMapCachingEnabled", true);
            HapticFeedbackEnabled = Preferences.Default.Get("HapticFeedbackEnabled", true);

            await CheckPermissionsAsync();
            CalculateCacheSize();
        }
        finally
        {
            _isInitializing = false;
        }
    }

    public async Task CheckPermissionsAsync()
    {
        var camera = await Permissions.CheckStatusAsync<Permissions.Camera>();
        IsCameraGranted = camera == PermissionStatus.Granted;

        var loc = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        IsLocationGranted = loc == PermissionStatus.Granted;

#if ANDROID
        if (OperatingSystem.IsAndroidVersionAtLeast(33))
        {
            var notif = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
            IsNotificationsGranted = notif == PermissionStatus.Granted;
        }
        else
        {
            IsNotificationsGranted = true;
        }
#else
        IsNotificationsGranted = true;
#endif
    }

    partial void OnPushNotificationsEnabledChanged(bool value)
    {
        if (_isInitializing) return;
        Preferences.Default.Set("PushNotificationsEnabled", value);
    }

    partial void OnEmergencySirenEnabledChanged(bool value)
    {
        if (_isInitializing) return;
        Preferences.Default.Set("EmergencySirenEnabled", value);

        // Sync with Dashboard Quick Actions
        try
        {
            var json = Preferences.Default.Get("CustomQuickActionsList_v18", "[]");
            var actions = Newtonsoft.Json.JsonConvert.DeserializeObject<System.Collections.Generic.List<RescuAR.App.Models.QuickActionItem>>(json) ?? new();
            var siren = actions.FirstOrDefault(a => a.Id == "siren" || a.ActionType == "Siren");
            if (siren != null)
            {
                siren.IsEnabled = value;
                Preferences.Default.Set("CustomQuickActionsList_v18", Newtonsoft.Json.JsonConvert.SerializeObject(actions));
            }
        }
        catch { }
    }

    partial void OnOfflineMapCachingEnabledChanged(bool value)
    {
        if (_isInitializing) return;
        Preferences.Default.Set("OfflineMapCachingEnabled", value);
    }

    partial void OnHapticFeedbackEnabledChanged(bool value)
    {
        if (_isInitializing) return;
        Preferences.Default.Set("HapticFeedbackEnabled", value);
    }

    [RelayCommand]
    private void OpenDeviceSettings()
    {
        try
        {
            AppInfo.ShowSettingsUI();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"ShowSettingsUI error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task ClearCacheAsync()
    {
        if (Shell.Current != null)
        {
            bool confirm = await Shell.Current.DisplayAlert(
                "Clear Map & Media Cache",
                "Are you sure you want to clear cached map tiles, temporary media files, and downloaded navigation assets?\n\nOffline maps will automatically reload fresh layers when needed.",
                "Clear Cache",
                "Cancel");

            if (!confirm) return;
        }

        try
        {
            var cacheDir = FileSystem.CacheDirectory;
            if (Directory.Exists(cacheDir))
            {
                var di = new DirectoryInfo(cacheDir);
                foreach (var file in di.GetFiles("*", SearchOption.AllDirectories))
                {
                    try { file.Delete(); } catch { }
                }
                foreach (var dir in di.GetDirectories())
                {
                    try { dir.Delete(true); } catch { }
                }
            }

            CacheSizeText = "0 KB";

            if (Shell.Current != null)
            {
                await Shell.Current.DisplayAlert("Cache Cleared", "Local map and media cache has been cleared successfully.", "OK");
            }
        }
        catch (Exception ex)
        {
            if (Shell.Current != null)
            {
                await Shell.Current.DisplayAlert("Error", $"Could not clear cache: {ex.Message}", "OK");
            }
        }
    }

    private void CalculateCacheSize()
    {
        try
        {
            var cacheDir = FileSystem.CacheDirectory;
            if (Directory.Exists(cacheDir))
            {
                var files = Directory.GetFiles(cacheDir, "*", SearchOption.AllDirectories);
                long totalBytes = 0;
                foreach (var f in files)
                {
                    try { totalBytes += new FileInfo(f).Length; } catch { }
                }

                if (totalBytes < 1024 * 1024)
                {
                    CacheSizeText = $"{totalBytes / 1024.0:F1} KB";
                }
                else
                {
                    CacheSizeText = $"{totalBytes / (1024.0 * 1024.0):F2} MB";
                }
            }
            else
            {
                CacheSizeText = "0 KB";
            }
        }
        catch
        {
            CacheSizeText = "0 KB";
        }
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
