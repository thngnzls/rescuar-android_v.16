using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.DataTransfer;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Media;
using Microsoft.Maui.Storage;
using RescuAR.App.Models;
using RescuAR.App.Services.Cloud;

namespace RescuAR.App.ViewModels.Profile;

public partial class SafetyCircleSettingsViewModel : ObservableObject
{
    private readonly SafetyCircleService _safetyCircleService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsNotInCircle))]
    private bool _isInCircle;

    public bool IsNotInCircle => !IsInCircle;

    [ObservableProperty]
    private string _activeCircleName = string.Empty;

    [ObservableProperty]
    private string _activeCircleId = string.Empty;

    [ObservableProperty]
    private string _inviteCode = string.Empty;

    [ObservableProperty]
    private string _newCircleName = string.Empty;

    [ObservableProperty]
    private string _joinInviteCode = string.Empty;

    [ObservableProperty]
    private SupabaseSafetyCircle? _selectedCircle;

    [ObservableProperty]
    private bool _isDeleteModalVisible;

    [ObservableProperty]
    private SupabaseSafetyCircle? _circleToDelete;

    public ObservableCollection<SupabaseSafetyCircle> AllMyCircles { get; } = new();
    public ObservableCollection<User> Members { get; } = new();

    public SafetyCircleSettingsViewModel(SafetyCircleService? safetyCircleService = null)
    {
        _safetyCircleService = safetyCircleService ?? new SafetyCircleService();
        _ = LoadCircleDataAsync();
    }

    public async Task LoadCircleDataAsync()
    {
        try
        {
            var myCircles = await _safetyCircleService.GetMyCirclesAsync();
            AllMyCircles.Clear();
            foreach (var c in myCircles)
            {
                AllMyCircles.Add(c);
            }

            var savedSelectedId = Preferences.Default.Get("SelectedCircleId", string.Empty);
            var activeCircle = AllMyCircles.FirstOrDefault(c => c.Id == savedSelectedId)
                ?? AllMyCircles.FirstOrDefault();

            if (activeCircle != null)
            {
                SelectedCircle = activeCircle;
                IsInCircle = true;
                ActiveCircleId = activeCircle.Id;
                ActiveCircleName = activeCircle.Name;
                InviteCode = activeCircle.InviteCode;
                Preferences.Default.Set("SelectedCircleId", activeCircle.Id);

                var membersList = await _safetyCircleService.GetCircleMembersAsync(activeCircle.Id);
                Members.Clear();
                foreach (var member in membersList)
                {
                    Members.Add(member);
                }
            }
            else
            {
                SelectedCircle = null;
                IsInCircle = false;
                ActiveCircleId = string.Empty;
                ActiveCircleName = string.Empty;
                InviteCode = string.Empty;
                Members.Clear();
                Preferences.Default.Remove("SelectedCircleId");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadCircleDataAsync error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task SelectActiveCircleAsync(SupabaseSafetyCircle circle)
    {
        if (circle == null) return;
        SelectedCircle = circle;
        IsInCircle = true;
        ActiveCircleId = circle.Id;
        ActiveCircleName = circle.Name;
        InviteCode = circle.InviteCode;
        Preferences.Default.Set("SelectedCircleId", circle.Id);

        try
        {
            var membersList = await _safetyCircleService.GetCircleMembersAsync(circle.Id);
            Members.Clear();
            foreach (var member in membersList)
            {
                Members.Add(member);
            }
        }
        catch { }
    }

    [RelayCommand]
    private async Task RenameCircleAsync(SupabaseSafetyCircle circle)
    {
        if (circle == null || Shell.Current == null) return;

        string currentName = circle.Name;
        string newName = await Shell.Current.DisplayPromptAsync(
            "Rename Safety Circle",
            "Enter a new name for this circle:",
            initialValue: currentName,
            maxLength: 40);

        if (string.IsNullOrWhiteSpace(newName) || newName.Trim() == currentName) return;

        try
        {
            circle.Name = newName.Trim();
            _safetyCircleService.SaveLocalCircle(circle);

            if (SelectedCircle?.Id == circle.Id)
            {
                ActiveCircleName = circle.Name;
            }

            await LoadCircleDataAsync();
            await Shell.Current.DisplayAlert("Renamed", $"Safety Circle renamed to '{circle.Name}'.", "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task UpdateCirclePhotoAsync(SupabaseSafetyCircle circle)
    {
        if (circle == null || Shell.Current == null) return;

        try
        {
            var file = await MediaPicker.Default.PickPhotoAsync();
            if (file != null)
            {
                using var stream = await file.OpenReadAsync();
                var uploadedUrl = await CloudinaryService.UploadImageStreamAsync(stream, file.FileName);
                if (!string.IsNullOrWhiteSpace(uploadedUrl))
                {
                    Preferences.Default.Set($"circle_avatar_{circle.Id}", uploadedUrl);
                    await Shell.Current.DisplayAlert("Photo Updated", "Group photo updated successfully!", "OK");
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private void RequestDeleteCircle(SupabaseSafetyCircle circle)
    {
        if (circle == null) return;
        CircleToDelete = circle;
        IsDeleteModalVisible = true;
    }

    [RelayCommand]
    private void CancelDelete()
    {
        IsDeleteModalVisible = false;
        CircleToDelete = null;
    }

    [RelayCommand]
    private async Task ConfirmDeleteAsync()
    {
        if (CircleToDelete == null) return;
        var circle = CircleToDelete;
        IsDeleteModalVisible = false;
        CircleToDelete = null;

        try
        {
            await _safetyCircleService.DeleteCircleAsync(circle.Id);
            AllMyCircles.Remove(circle);

            if (SelectedCircle?.Id == circle.Id)
            {
                await LoadCircleDataAsync();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Delete error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task CreateCircleAsync()
    {
        if (string.IsNullOrWhiteSpace(NewCircleName))
        {
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Circle Name", "Please enter a name for your safety circle.", "OK");
            return;
        }

        try
        {
            var created = await _safetyCircleService.CreateCircleAsync(NewCircleName.Trim());
            if (created != null)
            {
                NewCircleName = string.Empty;
                await LoadCircleDataAsync();
                if (Shell.Current != null)
                    await Shell.Current.DisplayAlert("Circle Created", $"Safety Circle '{created.Name}' created! Invite Code: {created.InviteCode}", "OK");
            }
        }
        catch (Exception ex)
        {
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Error", $"Could not create circle: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task JoinCircleAsync()
    {
        if (string.IsNullOrWhiteSpace(JoinInviteCode))
        {
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Invite Code", "Please enter a valid 6-character invite code.", "OK");
            return;
        }

        try
        {
            var joined = await _safetyCircleService.JoinCircleWithCodeAsync(JoinInviteCode.Trim());
            if (joined != null)
            {
                JoinInviteCode = string.Empty;
                await LoadCircleDataAsync();
                if (Shell.Current != null)
                    await Shell.Current.DisplayAlert("Joined", $"Successfully joined '{joined.Name}'!", "OK");
            }
        }
        catch (Exception ex)
        {
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task CopyInviteCodeAsync()
    {
        if (string.IsNullOrWhiteSpace(InviteCode)) return;

        await Clipboard.Default.SetTextAsync(InviteCode);
        if (Shell.Current != null)
            await Shell.Current.DisplayAlert("Copied", $"Invite code '{InviteCode}' copied to clipboard!", "OK");
    }

    [RelayCommand]
    private void LeaveCircle()
    {
        if (SelectedCircle != null)
        {
            RequestDeleteCircle(SelectedCircle);
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
