using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Adminn
{
    /// <summary>
    /// Interaction logic for Profile.xaml
    /// </summary>
    public partial class Profile : Page
    {
        // User data model - in a real application, this would be loaded from a database or API
        private UserProfile currentUser = null!; // Use null-forgiving operator to suppress warning

        public Profile()
        {
            InitializeComponent();
            LoadUserData();
        }

        private void LoadUserData()
        {
            // In a real application, this data would come from a database or API
            currentUser = new UserProfile
            {
                FullName = "Muhammad Fayyaz",
                DisplayName = "Muhammad Fayaz",
                Username = "example",
                Email = "@example.com",
                DateOfBirth = new DateTime(1987, 1, 1),
                Gender = "Male",
                Nationality = "Pakistani",
                Address = "S-4 AB Heights, Airport Road, Lhr",
                PhoneNumber = "0301-8515333",
                AccountCreated = new DateTime(2020, 3, 20),
                LastLogin = new DateTime(2024, 8, 22),
                MembershipStatus = "Premium Member",
                IsVerified = true,
                LanguagePreference = "English",
                TimeZone = "GMT-5 (UTC + 5)",
                PasswordLastChanged = new DateTime(2024, 7, 15),
                TwoFactorEnabled = true,
                SecurityQuestionsSet = true,
                LoginNotificationsEnabled = true,
                ConnectedDevices = 3,
                SuspiciousActivity = false,
                EmailNotificationsSubscribed = true,
                SmsAlertsEnabled = true,
                ContentPreferences = "Admin",
                DefaultDashboardView = "",
                DarkModeEnabled = true
            };

            // Set data context for binding
            DataContext = currentUser;

            // If you wish to use data binding instead of hard-coded values in XAML:
            // UpdateUIWithBindings();
        }

        // Optional: Setup dynamic bindings if you want to move away from hard-coded values
        private void UpdateUIWithBindings()
        {
            // Example of how you could set up bindings programmatically
            // This is only needed if you want to replace the hard-coded values with dynamic binding

            /*
            // Find elements in your XAML and bind them to your UserProfile properties
            TextBlock fullNameText = this.FindName("fullNameTextBlock") as TextBlock;
            if (fullNameText != null)
            {
                Binding fullNameBinding = new Binding("FullName");
                fullNameBinding.Source = currentUser;
                fullNameText.SetBinding(TextBlock.TextProperty, fullNameBinding);
            }
            
            // And so on for other elements...
            */
        }
    }

    // User profile model
    public class UserProfile
    {
        public required string FullName { get; set; }
        public required string DisplayName { get; set; }
        public required string Username { get; set; }
        public required string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
        public required string Gender { get; set; }
        public required string Nationality { get; set; }
        public required string Address { get; set; }
        public required string PhoneNumber { get; set; }
        public DateTime AccountCreated { get; set; }
        public DateTime LastLogin { get; set; }
        public required string MembershipStatus { get; set; }
        public bool IsVerified { get; set; }
        public required string LanguagePreference { get; set; }
        public required string TimeZone { get; set; }
        public DateTime PasswordLastChanged { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public bool SecurityQuestionsSet { get; set; }
        public bool LoginNotificationsEnabled { get; set; }
        public int ConnectedDevices { get; set; }
        public bool SuspiciousActivity { get; set; }
        public bool EmailNotificationsSubscribed { get; set; }
        public bool SmsAlertsEnabled { get; set; }
        public required string ContentPreferences { get; set; }
        public required string DefaultDashboardView { get; set; }
        public bool DarkModeEnabled { get; set; }
    }
}