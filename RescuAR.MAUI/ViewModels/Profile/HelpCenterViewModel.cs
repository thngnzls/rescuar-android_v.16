using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Maui.ApplicationModel;
using Microsoft.Maui.ApplicationModel.Communication;
using Microsoft.Maui.Controls;

namespace RescuAR.App.ViewModels.Profile;

public partial class HelpFaqItem : ObservableObject
{
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(ArrowIcon))]
    private bool _isExpanded;

    public string ArrowIcon => IsExpanded ? "▲" : "▼";

    [RelayCommand]
    private void ToggleExpand()
    {
        IsExpanded = !IsExpanded;
    }
}

public partial class HelpCenterViewModel : ObservableObject
{
    private readonly List<HelpFaqItem> _allFaqs = new()
    {
        new HelpFaqItem
        {
            Question = "How does AR Evacuation Navigation work?",
            Answer = "RescuAR uses your smartphone's camera, GPS, and motion sensors to project 3D digital arrows and hazard warnings directly onto your physical surroundings, guiding you to the safest Marikina evacuation center step-by-step.",
            Category = "Navigation"
        },
        new HelpFaqItem
        {
            Question = "Can I use RescuAR without an active internet connection?",
            Answer = "Yes! RescuAR is built offline-first. Marikina street graphs, shelter locations, and Hybrid A* routing algorithms run locally on your phone without cellular reception or Wi-Fi.",
            Category = "Offline Use"
        },
        new HelpFaqItem
        {
            Question = "What is a Safety Circle and how do I use it?",
            Answer = "A Safety Circle is a private emergency group for your family or neighborhood. Members can view each other's live location on the offline map, check battery levels, and send emergency group messages.",
            Category = "Safety Circle"
        },
        new HelpFaqItem
        {
            Question = "What should I do if my route gets flooded or blocked?",
            Answer = "If a road is impassable, tap 'Recalculate Route' or select an alternative shelter. The algorithm instantly recalculates using high-elevation terrain to steer you away from inundated streets.",
            Category = "Navigation"
        },
        new HelpFaqItem
        {
            Question = "How do I report a hazard or impassable street?",
            Answer = "Go to the Reports tab, tap 'Submit Community Report', select the hazard category (Flooding, Debris, Fallen Wire, Road Blockage), take a photo, and send. Responders and other citizens will be alerted immediately.",
            Category = "Community Reports"
        },
        new HelpFaqItem
        {
            Question = "How do I contact Marikina Rescue 161 directly?",
            Answer = "Navigate to Profile > Emergency Contacts to find Marikina Rescue 161, DRRMO, BFP, and PNP with one-tap dialing.",
            Category = "Emergency"
        }
    };

    [ObservableProperty]
    private string _searchText = string.Empty;

    public ObservableCollection<HelpFaqItem> FilteredFaqs { get; } = new();

    public HelpCenterViewModel()
    {
        FilterFaqs();
    }

    partial void OnSearchTextChanged(string value)
    {
        FilterFaqs();
    }

    private void FilterFaqs()
    {
        FilteredFaqs.Clear();
        var query = SearchText?.Trim().ToLowerInvariant() ?? string.Empty;

        var items = string.IsNullOrWhiteSpace(query)
            ? _allFaqs
            : _allFaqs.Where(f => f.Question.ToLowerInvariant().Contains(query) || f.Answer.ToLowerInvariant().Contains(query) || f.Category.ToLowerInvariant().Contains(query));

        foreach (var item in items)
        {
            FilteredFaqs.Add(item);
        }
    }

    [RelayCommand]
    private async Task EmailSupportAsync()
    {
        try
        {
            if (Email.Default.IsComposeSupported)
            {
                var message = new EmailMessage
                {
                    Subject = "RescuAR App Support / Inquiries",
                    Body = "Hello RescuAR Support Team,\n\nI have a question regarding...",
                    To = new List<string> { "support@rescuar.app" }
                };
                await Email.Default.ComposeAsync(message);
            }
            else
            {
                if (Shell.Current != null)
                    await Shell.Current.DisplayAlert("Support Email", "Please send your inquiries to: support@rescuar.app", "OK");
            }
        }
        catch (Exception ex)
        {
            if (Shell.Current != null)
                await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task CallRescue161Async()
    {
        try
        {
            if (PhoneDialer.Default.IsSupported)
            {
                PhoneDialer.Default.Open("161");
            }
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

