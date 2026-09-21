using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Storage;
using RescuAR.App.Views.Authentication;
using RescuAR.App.Services.Authentication;
using RescuAR.MAUI;

namespace RescuAR.App.ViewModels.Authentication;

public partial class RegistrationViewModel : ObservableObject
{
    private readonly IServiceProvider _serviceProvider;
    private readonly AuthenticationService _authService;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsStep1))]
    [NotifyPropertyChangedFor(nameof(IsStep2))]
    private int _currentStep = 1;

    public static DateTime TodayDate => DateTime.Today;

    public bool IsStep1 => CurrentStep == 1;
    public bool IsStep2 => CurrentStep == 2;

    // --- Step 1: Marikina Resident Information ---

    [ObservableProperty]
    private string _firstName = string.Empty;

    [ObservableProperty]
    private string _lastName = string.Empty;

    [ObservableProperty]
    private string _contactNumber = string.Empty;

    [ObservableProperty]
    private DateTime _birthday = DateTime.Today.AddYears(-20);

    [ObservableProperty]
    private string _houseNumber = string.Empty;

    [ObservableProperty]
    private string _streetName = string.Empty;

    [ObservableProperty]
    private string _selectedBarangay = "Malanday";

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

    // --- Step 2: Account Credentials ---

    [ObservableProperty]
    private string _email = string.Empty;

    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private string _confirmPassword = string.Empty;

    [ObservableProperty]
    private bool _hasMinLength;

    [ObservableProperty]
    private bool _hasSpecialChar;

    [ObservableProperty]
    private bool _hasDigit;

    [ObservableProperty]
    private bool _hasUpperCase;

    partial void OnPasswordChanged(string value)
    {
        if (value == null) value = string.Empty;
        HasMinLength = value.Length >= 8;
        HasSpecialChar = Regex.IsMatch(value, @"[!@#$%^&*(),.?\"":{}|<>]");
        HasDigit = Regex.IsMatch(value, @"\d");
        HasUpperCase = Regex.IsMatch(value, @"[A-Z]");
    }

    [ObservableProperty]
    private bool _isTermsAccepted = false;

    [ObservableProperty]
    private bool _isPasswordVisible = false;

    [ObservableProperty]
    private bool _isConfirmPasswordVisible = false;

    public bool IsPasswordHidden => !IsPasswordVisible;
    public bool IsConfirmPasswordHidden => !IsConfirmPasswordVisible;

    [ObservableProperty]
    private bool _isLoading = false;

    public bool IsNotLoading => !IsLoading;

    partial void OnIsLoadingChanged(bool value)
    {
        OnPropertyChanged(nameof(IsNotLoading));
    }

    [ObservableProperty]
    private string _errorMessage = string.Empty;

    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    public RegistrationViewModel(IServiceProvider serviceProvider, AuthenticationService authService)
    {
        _serviceProvider = serviceProvider;
        _authService = authService;
    }

    [RelayCommand]
    private void TogglePasswordVisibility()
    {
        IsPasswordVisible = !IsPasswordVisible;
        OnPropertyChanged(nameof(IsPasswordHidden));
    }

    [RelayCommand]
    private void ToggleConfirmPasswordVisibility()
    {
        IsConfirmPasswordVisible = !IsConfirmPasswordVisible;
        OnPropertyChanged(nameof(IsConfirmPasswordHidden));
    }

    [RelayCommand]
    private async Task ProceedToStep2()
    {
        ErrorMessage = string.Empty;
        OnPropertyChanged(nameof(HasError));

        if (string.IsNullOrWhiteSpace(FirstName))
        {
            await ShowErrorAsync("First Name is required.");
            return;
        }

        if (string.IsNullOrWhiteSpace(LastName))
        {
            await ShowErrorAsync("Last Name is required.");
            return;
        }

        if (string.IsNullOrWhiteSpace(ContactNumber))
        {
            await ShowErrorAsync("Mobile Number is required.");
            return;
        }

        if (string.IsNullOrWhiteSpace(HouseNumber))
        {
            await ShowErrorAsync("House / Lot / Block number is required.");
            return;
        }

        if (string.IsNullOrWhiteSpace(StreetName))
        {
            await ShowErrorAsync("Street name is required.");
            return;
        }

        if (string.IsNullOrWhiteSpace(SelectedBarangay))
        {
            await ShowErrorAsync("Please select your Marikina City Barangay.");
            return;
        }

        // Validate age requirement (e.g. at least 10 years old)
        int age = DateTime.Today.Year - Birthday.Year;
        if (Birthday > DateTime.Today.AddYears(-age)) age--;
        if (age < 10)
        {
            await ShowErrorAsync("Please provide a valid birthdate.");
            return;
        }

        // Cache resident profile locally
        Preferences.Default.Set("UserFirstName", FirstName.Trim());
        Preferences.Default.Set("UserLastName", LastName.Trim());
        Preferences.Default.Set("UserPhone", ContactNumber.Trim());
        Preferences.Default.Set("UserBirthday", Birthday.ToString("yyyy-MM-dd"));
        Preferences.Default.Set("UserHouseLot", HouseNumber.Trim());
        Preferences.Default.Set("UserStreetName", StreetName.Trim());
        Preferences.Default.Set("UserStreet", $"{HouseNumber.Trim()} {StreetName.Trim()}".Trim());
        Preferences.Default.Set("UserBarangay", SelectedBarangay);
        Preferences.Default.Set("UserCity", "Marikina City");

        CurrentStep = 2;
    }

    [RelayCommand]
    private void BackToStep1()
    {
        CurrentStep = 1;
    }

    [RelayCommand]
    private async Task CreateAccount()
    {
        if (IsLoading) return;

        ErrorMessage = string.Empty;
        OnPropertyChanged(nameof(HasError));

        if (string.IsNullOrWhiteSpace(Email))
        {
            await ShowErrorAsync("Email Address is required.");
            return;
        }

        if (!IsValidEmail(Email))
        {
            await ShowErrorAsync("Please enter a valid email address.");
            return;
        }

        if (string.IsNullOrWhiteSpace(Password))
        {
            await ShowErrorAsync("Password is required.");
            return;
        }

        if (!HasMinLength || !HasSpecialChar || !HasDigit || !HasUpperCase)
        {
            await ShowErrorAsync("Password must be at least 8 characters and contain an uppercase letter, a number, and a special character.");
            return;
        }

        if (Password != ConfirmPassword)
        {
            await ShowErrorAsync("Passwords do not match.");
            return;
        }

        if (!IsTermsAccepted)
        {
            await ShowErrorAsync("You must accept the Terms & Conditions and Privacy Policy.");
            return;
        }

        IsLoading = true;
        try
        {
            Preferences.Default.Set("UserEmail", Email.Trim());

            // Save full resident address
            string fullAddress = $"{HouseNumber.Trim()} {StreetName.Trim()}, {SelectedBarangay}, Marikina City";

            // Register via Supabase Authentication
            var session = await _authService.SignUpWithEmailAsync(
                Email.Trim(), 
                Password, 
                FirstName.Trim(), 
                LastName.Trim(), 
                null, 
                ContactNumber.Trim());

            // Save user profile details in Supabase database
            _ = Task.Run(async () =>
            {
                try
                {
                    var client = RescuAR.Services.SupabaseService.Instance.Client;
                    var userId = session?.User?.Id ?? Preferences.Default.Get("current_user_id", string.Empty);
                    if (client != null && !string.IsNullOrWhiteSpace(userId))
                    {
                        var userRecord = new Models.User
                        {
                            Id = userId,
                            FirstName = FirstName.Trim(),
                            LastName = LastName.Trim(),
                            Email = Email.Trim(),
                            PhoneNumber = ContactNumber.Trim(),
                            Address = fullAddress
                        };
                        await client.From<Models.User>().Upsert(userRecord);
                    }
                }
                catch { }
            });

            MainThread.BeginInvokeOnMainThread(async () =>
            {
                if (session != null && !string.IsNullOrEmpty(session.AccessToken))
                {
                    Preferences.Default.Set("IsLoggedIn", true);
                    if (Application.Current != null)
                    {
                        AuthenticationNavigation.TrySetRootPage(new AppShell());
                    }
                }
                else if (AuthenticationNavigation.RootPage is NavigationPage navPage)
                {
                    var otpPage = _serviceProvider.GetRequiredService<OtpVerificationPage>();
                    var vm = (OtpVerificationViewModel)otpPage.BindingContext;
                    vm.Email = Email.Trim();
                    await navPage.PushAsync(otpPage);
                }
            });
        }
        catch (Exception ex)
        {
            var msg = ex.Message;
            if (!string.IsNullOrEmpty(msg) && msg.Trim().StartsWith("{"))
            {
                try
                {
                    var json = JsonDocument.Parse(msg);
                    if (json.RootElement.TryGetProperty("msg", out var msgProp) || json.RootElement.TryGetProperty("message", out msgProp) || json.RootElement.TryGetProperty("error_description", out msgProp))
                    {
                        msg = msgProp.GetString();
                    }
                }
                catch { }
            }
            
            string finalError = msg ?? "An error occurred during registration. Please try again.";
            await ShowErrorAsync(finalError);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void GoogleSignUp()
    {
        var googleAuthPage = _serviceProvider.GetRequiredService<GoogleAuthPage>();
        MainThread.BeginInvokeOnMainThread(() =>
        {
            if (Application.Current != null)
            {
                AuthenticationNavigation.TrySetRootPage(googleAuthPage);
            }
        });
    }

    private async Task ShowErrorAsync(string message)
    {
        ErrorMessage = message;
        OnPropertyChanged(nameof(HasError));
        
        if (AuthenticationNavigation.RootPage != null)
        {
            await AuthenticationNavigation.RootPage!.DisplayAlert("Registration Notice", message, "OK");
        }
    }

    [RelayCommand]
    private void Back()
    {
        if (CurrentStep == 2)
        {
            CurrentStep = 1;
            return;
        }

        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (AuthenticationNavigation.RootPage is NavigationPage navPage)
            {
                await navPage.PopAsync();
            }
        });
    }

    [RelayCommand]
    private void GoToSignIn()
    {
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (AuthenticationNavigation.RootPage is NavigationPage navPage)
            {
                var loginPage = _serviceProvider.GetRequiredService<LoginPage>();
                await navPage.PushAsync(loginPage);
            }
        });
    }

    [RelayCommand]
    private void GoToTerms()
    {
        var termsPage = _serviceProvider.GetRequiredService<TermsAndConditionsPage>();
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (AuthenticationNavigation.RootPage is NavigationPage navPage)
            {
                await navPage.PushAsync(termsPage);
            }
        });
    }

    [RelayCommand]
    private void GoToPrivacy()
    {
        var privacyPage = _serviceProvider.GetRequiredService<PrivacyPolicyPage>();
        MainThread.BeginInvokeOnMainThread(async () =>
        {
            if (AuthenticationNavigation.RootPage is NavigationPage navPage)
            {
                await navPage.PushAsync(privacyPage);
            }
        });
    }

    private bool IsValidEmail(string email)
    {
        try
        {
            return Regex.IsMatch(email,
                @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
        }
        catch (RegexMatchTimeoutException)
        {
            return false;
        }
    }
}
