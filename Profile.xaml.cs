using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Adminn // Your project's namespace
{
    public partial class Profile : Page
    {
        // Static field to hold the single, shared instance of profile data
        private static ProfileData? _sharedUserProfileInstance;

        // Instance property to easily access the shared data
        public ProfileData? CurrentUserProfile { get; private set; }

        public Profile()
        {
            InitializeComponent();
            // LoadProfileData will be called by the Navigated event when the page is first shown
            // or when navigating back to it.
            // To ensure it loads on first display if not part of a navigation sequence initially:
            this.Loaded += Profile_Loaded;
        }

        private void Profile_Loaded(object sender, RoutedEventArgs e)
        {
            // Ensure data is loaded when the page initially becomes visible.
            // Unsubscribe to avoid multiple loads if Loaded fires again for some reason.
            this.Loaded -= Profile_Loaded;
            LoadProfileData();
        }

        private void LoadProfileData()
        {
            // If the shared instance doesn't exist yet, load it once.
            if (_sharedUserProfileInstance == null)
            {
                _sharedUserProfileInstance = ProfileData.LoadCurrentProfile();
            }

            // Use the shared instance
            CurrentUserProfile = _sharedUserProfileInstance;

            if (CurrentUserProfile == null)
            {
                MessageBox.Show("Failed to load profile data.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Update TextBlocks from the CurrentUserProfile (which is now the shared, potentially modified instance)
            ProfileNameTextBlock.Text = CurrentUserProfile.DisplayName;
            ProfileEmailHeaderTextBlock.Text = CurrentUserProfile.Email;

            FullNameTextBlock.Text = CurrentUserProfile.FullName;
            DobTextBlock.Text = CurrentUserProfile.DateOfBirth?.ToString("MMMM d, yyyy") ?? "N/A";
            GenderTextBlock.Text = CurrentUserProfile.Gender;
            NationalityTextBlock.Text = CurrentUserProfile.Nationality;
            AddressTextBlock.Text = CurrentUserProfile.Address;
            PhoneNumberTextBlock.Text = CurrentUserProfile.PhoneNumber;

            DisplayNameTextBlock.Text = CurrentUserProfile.DisplayName;
            AccountCreatedTextBlock.Text = CurrentUserProfile.AccountCreated.ToString("MMMM d, yyyy");
            EmailTextBlock.Text = CurrentUserProfile.Email;
            AccountVerificationTextBlock.Text = CurrentUserProfile.AccountVerification;
            LanguagePreferenceTextBlock.Text = CurrentUserProfile.LanguagePreference;
            TimeZoneTextBlock.Text = CurrentUserProfile.TimeZone;
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            // Ensure the shared instance is loaded before trying to edit it.
            if (_sharedUserProfileInstance == null)
            {
                LoadProfileData(); // Try to load it if it wasn't (e.g. if Loaded event didn't fire as expected)
                if (_sharedUserProfileInstance == null) // Still null after attempt
                {
                    MessageBox.Show("Profile data is not loaded. Cannot edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            // CurrentUserProfile here will point to _sharedUserProfileInstance
            if (CurrentUserProfile == null) // Should be redundant if _sharedUserProfileInstance is checked
            {
                MessageBox.Show("Profile data is not loaded. Cannot edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (this.NavigationService != null)
            {
                // Pass the *shared* instance to the EditProfilePage
                EditProfilePage editPage = new EditProfilePage(_sharedUserProfileInstance);

                // When we navigate away and then back, the Navigated event on NavigationService
                // can be used to refresh this page.
                NavigatedEventHandler? onNavigatedBack = null;
                onNavigatedBack = (navSender, navArgs) =>
                {
                    // Check if we are navigating back TO this page instance
                    if (navArgs.Content == this)
                    {
                        LoadProfileData(); // Reload/Refresh data from the (potentially modified) shared instance
                        if (this.NavigationService != null)
                        {
                            this.NavigationService.Navigated -= onNavigatedBack; // Unsubscribe after use
                        }
                    }
                };
                this.NavigationService.Navigated += onNavigatedBack;

                this.NavigationService.Navigate(editPage);
            }
            else
            {
                MessageBox.Show("Navigation service not available.");
            }
        }
    }
}