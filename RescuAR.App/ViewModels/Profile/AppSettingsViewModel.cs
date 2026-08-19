using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.Maui.ApplicationModel;

namespace RescuAR.App.ViewModels.Profile
{
    internal class AppSettingsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        
        private bool _isInitializing = false;

        private bool _isCameraEnabled;
        public bool IsCameraEnabled
        {
            get => _isCameraEnabled;
            set 
            { 
                if (_isCameraEnabled != value) 
                {
                    _isCameraEnabled = value; 
                    OnPropertyChanged(); 
                    if (!_isInitializing) AppInfo.ShowSettingsUI();
                }
            }
        }

        private bool _isLocationEnabled;
        public bool IsLocationEnabled
        {
            get => _isLocationEnabled;
            set 
            { 
                if (_isLocationEnabled != value) 
                {
                    _isLocationEnabled = value; 
                    OnPropertyChanged(); 
                    if (!_isInitializing) AppInfo.ShowSettingsUI();
                }
            }
        }

        private bool _isMotionEnabled;
        public bool IsMotionEnabled
        {
            get => _isMotionEnabled;
            set 
            { 
                if (_isMotionEnabled != value) 
                {
                    _isMotionEnabled = value; 
                    OnPropertyChanged(); 
                    if (!_isInitializing) AppInfo.ShowSettingsUI();
                }
            }
        }

        private bool _isNotificationsEnabled;
        public bool IsNotificationsEnabled
        {
            get => _isNotificationsEnabled;
            set 
            { 
                if (_isNotificationsEnabled != value) 
                {
                    _isNotificationsEnabled = value; 
                    OnPropertyChanged(); 
                    if (!_isInitializing) AppInfo.ShowSettingsUI();
                }
            }
        }

        public async Task LoadPermissionsAsync()
        {
            _isInitializing = true;
            
            var camera = await Permissions.CheckStatusAsync<Permissions.Camera>();
            IsCameraEnabled = camera == PermissionStatus.Granted;

            var location = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            IsLocationEnabled = location == PermissionStatus.Granted;

            var motion = await Permissions.CheckStatusAsync<Permissions.Sensors>();
            IsMotionEnabled = motion == PermissionStatus.Granted;

#if ANDROID
            if (OperatingSystem.IsAndroidVersionAtLeast(33))
            {
                var push = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
                IsNotificationsEnabled = push == PermissionStatus.Granted;
            }
            else
            {
                IsNotificationsEnabled = true;
            }
#else
            IsNotificationsEnabled = true;
#endif

            _isInitializing = false;
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
