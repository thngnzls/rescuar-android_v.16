using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using RescuAR.App.Models;
using RescuAR.Services;

namespace RescuAR.App.ViewModels.Profile;

public partial class PersonalInformationViewModel : ObservableObject
{
    [ObservableProperty]
    private string _firstName = string.Empty;

    [ObservableProperty]
    private string _lastName = string.Empty;

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _phoneNumber = string.Empty;

    [ObservableProperty]
    private string _streetAddress = string.Empty;

    [ObservableProperty]
    private string _selectedBarangay = "Malanday";

    [ObservableProperty]
    private string _city = "Marikina City";

    [ObservableProperty]
    private bool _isSaving;

    public List<string> MarikinaBarangays { get; } = new()
    {
        "Barangka",
        "Concepcion Uno",
        "Concepcion Dos",
        "Fortune",
        "Industrial Valley Complex",
        "Jesus Dela Peña",
        "Malanday",
        "Marikina Heights",
        "Nangka",
        "Parang",
        "San Roque",
        "Santa Elena",
        "Santo Niño",
        "Tañong",
        "Tumana"
    };

    public PersonalInformationViewModel()
    {
        _ = LoadUserDataAsync();
    }

    public async Task LoadUserDataAsync()
    {
        try
        {
            // 1. Read from local preferences first for instantaneous load
            FirstName = Preferences.Default.Get("UserFirstName", string.Empty);
            LastName = Preferences.Default.Get("UserLastName", string.Empty);
            Email = Preferences.Default.Get("UserEmail", string.Empty);
            PhoneNumber = Preferences.Default.Get("UserPhone", string.Empty);
            StreetAddress = Preferences.Default.Get("UserStreet", string.Empty);
            if (string.IsNullOrWhiteSpace(StreetAddress))
            {
                var house = Preferences.Default.Get("UserHouseLot", string.Empty);
                var street = Preferences.Default.Get("UserStreetName", string.Empty);
                if (!string.IsNullOrWhiteSpace(house) || !string.IsNullOrWhiteSpace(street))
                {
                    StreetAddress = $"{house} {street}".Trim();
                }
            }
            SelectedBarangay = Preferences.Default.Get("UserBarangay", "Malanday");
            City = Preferences.Default.Get("UserCity", "Marikina City");

            // 2. Refresh from Supabase
            var client = SupabaseService.Instance.Client;
            var userId = client?.Auth.CurrentUser?.Id ?? Preferences.Default.Get("current_user_id", string.Empty);

            if (client != null && !string.IsNullOrWhiteSpace(userId))
            {
                try
                {
                    var userRow = await client.From<User>().Where(u => u.Id == userId).Single();
                    if (userRow != null)
                    {
                        if (!string.IsNullOrWhiteSpace(userRow.FirstName)) FirstName = userRow.FirstName;
                        if (!string.IsNullOrWhiteSpace(userRow.LastName)) LastName = userRow.LastName;
                        if (!string.IsNullOrWhiteSpace(userRow.Email)) Email = userRow.Email;
                        if (!string.IsNullOrWhiteSpace(userRow.PhoneNumber)) PhoneNumber = userRow.PhoneNumber;
                        if (!string.IsNullOrWhiteSpace(userRow.Address))
                        {
                            var parts = userRow.Address.Split(',');
                            if (parts.Length > 0 && !string.IsNullOrWhiteSpace(parts[0])) StreetAddress = parts[0].Trim();
                            if (parts.Length > 1)
                            {
                                var b = parts[1].Trim();
                                if (MarikinaBarangays.Contains(b)) SelectedBarangay = b;
                            }
                        }
                    }
                }
                catch { }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Personal info load error: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task SaveInformationAsync()
    {
        if (IsSaving) return;

        if (string.IsNullOrWhiteSpace(FirstName) || string.IsNullOrWhiteSpace(LastName))
        {
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Required Fields", "Please provide both your First Name and Last Name.", "OK");
            return;
        }

        IsSaving = true;
        try
        {
            // Save to Preferences
            Preferences.Default.Set("UserFirstName", FirstName.Trim());
            Preferences.Default.Set("UserLastName", LastName.Trim());
            Preferences.Default.Set("UserEmail", Email.Trim());
            Preferences.Default.Set("UserPhone", PhoneNumber.Trim());
            Preferences.Default.Set("UserStreet", StreetAddress.Trim());
            Preferences.Default.Set("UserBarangay", SelectedBarangay);
            Preferences.Default.Set("UserCity", City.Trim());

            string fullAddress = $"{StreetAddress.Trim()}, {SelectedBarangay}, {City.Trim()}".Trim().Trim(',');

            // Save to Supabase
            var client = SupabaseService.Instance.Client;
            var userId = client?.Auth.CurrentUser?.Id ?? Preferences.Default.Get("current_user_id", string.Empty);

            if (client != null && !string.IsNullOrWhiteSpace(userId))
            {
                var userRecord = new User
                {
                    Id = userId,
                    FirstName = FirstName.Trim(),
                    LastName = LastName.Trim(),
                    Email = Email.Trim(),
                    PhoneNumber = PhoneNumber.Trim(),
                    Address = fullAddress
                };

                try
                {
                    await client.From<User>().Upsert(userRecord);
                }
                catch { }
            }

            if (Shell.Current != null)
            {
                await Shell.Current.DisplayAlert("Saved Successfully", "Your personal information has been updated.", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
        catch (Exception ex)
        {
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Error", $"Could not save: {ex.Message}", "OK");
        }
        finally
        {
            IsSaving = false;
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
