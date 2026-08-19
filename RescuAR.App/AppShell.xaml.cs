namespace RescuAR.App;

public partial class AppShell : Shell
{
    public AppShell()
    {
        InitializeComponent();

        Routing.RegisterRoute("Prepare/Checklist", typeof(Views.Prepare.ChecklistPage));
        Routing.RegisterRoute("Prepare/PASS", typeof(Views.Prepare.PASSPage));
        Routing.RegisterRoute("Prepare/Assessment", typeof(Views.Prepare.AssessmentPage));
        Routing.RegisterRoute("Prepare/EvacuationCenterInfo", typeof(Views.Prepare.EvacuationCenterInfoPage));
        Routing.RegisterRoute("Prepare/HotlineDirectory", typeof(Views.Prepare.EvacuationCenterInfoPage));
        Routing.RegisterRoute("AdvisoryFeedPage", typeof(Views.Reports.AdvisoryFeedPage));
        Routing.RegisterRoute("Reports/AdvisoryFeed", typeof(Views.Reports.AdvisoryFeedPage));
        Routing.RegisterRoute("ProfilePage", typeof(Views.Profile.ProfilePage));
        Routing.RegisterRoute("PersonalInformationPage", typeof(Views.Profile.PersonalInformationPage));
        Routing.RegisterRoute("HealthInformationPage", typeof(Views.Profile.HealthInformationPage));
        Routing.RegisterRoute("SafetyCircleSettingsPage", typeof(Views.Profile.SafetyCircleSettingsPage));
        Routing.RegisterRoute("EmergencyContactsPage", typeof(Views.Profile.EmergencyContactsPage));
        Routing.RegisterRoute("AppSettingsPage", typeof(Views.Profile.AppSettingsPage));
        Routing.RegisterRoute("HelpCenterPage", typeof(Views.Profile.HelpCenterPage));
        Routing.RegisterRoute("PrivacyPolicyPage", typeof(Views.Profile.PrivacyPolicyPage));
        Routing.RegisterRoute("TermsConditionsPage", typeof(Views.Profile.TermsConditionsPage));
        Routing.RegisterRoute("SystemInformationPage", typeof(Views.Profile.SystemInformationPage));
        
        Routing.RegisterRoute("ReportDetails", typeof(Views.Reports.ReportDetailsPage));
        Routing.RegisterRoute("SafetyCirclePage", typeof(Views.Map.SafetyCirclePage));
        Routing.RegisterRoute("SummaryPage", typeof(Views.Summary.SummaryPage));
    }
}
