using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.ApplicationModel;

namespace RescuAR.App.ViewModels.Profile
{
    internal class SystemInformationViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private string _cameraPermission = "Unknown";
        public string CameraPermission
        {
            get => _cameraPermission;
            set { _cameraPermission = value; OnPropertyChanged(); }
        }

        private string _locationPermission = "Unknown";
        public string LocationPermission
        {
            get => _locationPermission;
            set { _locationPermission = value; OnPropertyChanged(); }
        }

        private string _motionPermission = "Unknown";
        public string MotionPermission
        {
            get => _motionPermission;
            set { _motionPermission = value; OnPropertyChanged(); }
        }

        private string _notificationsPermission = "Unknown";
        public string NotificationsPermission
        {
            get => _notificationsPermission;
            set { _notificationsPermission = value; OnPropertyChanged(); }
        }

        public string AppVersion => AppInfo.Current.VersionString;
        public string LastUpdated => System.IO.File.GetLastWriteTime(System.Reflection.Assembly.GetExecutingAssembly().Location).ToString("MM/dd/yyyy hh:mm tt");

        public ICommand ManagePermissionsCommand { get; }

        public SystemInformationViewModel()
        {
            ManagePermissionsCommand = new Command(() => AppInfo.ShowSettingsUI());
        }

        public async Task LoadPermissionsAsync()
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
                NotificationsPermission = "Allowed"; // Below 33 is implicitly allowed
            }
#else
            NotificationsPermission = "Allowed";
#endif
        }

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
