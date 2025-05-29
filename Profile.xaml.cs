using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation; // For NavigationService
using System.Diagnostics;    // For Debug.WriteLine
using System.Threading.Tasks; // For async Task

namespace Adminn
{
    public partial class Profile : Page
    {
        // Instead of static, each instance of Profile page will have its own ViewModel instance.
        // The ViewModel itself can handle loading/saving the data for a specific Admin_ID.
        public ProfileData CurrentUserProfile { get; private set; }

        private int _currentAdminId; // This needs to be set, e.g., from login

        public Profile() // Default constructor for XAML designer
        {
            InitializeComponent();
            CurrentUserProfile = new ProfileData(); // Initialize for XAML Binding
            this.DataContext = CurrentUserProfile;

            // For testing, assume Admin_ID = 1. In a real app, get this after login.
            // This constructor will be called by default when navigating without parameters.
            // Consider having a mechanism to pass the logged-in admin's ID.
            _currentAdminId = 1; // <<<<< TEMPORARY - REPLACE WITH ACTUAL LOGGED-IN ADMIN ID

            this.Loaded += Profile_Loaded_Async;
        }

        // Constructor to be called if an Admin ID is passed during navigation
        public Profile(int adminId)
        {
            InitializeComponent();
            CurrentUserProfile = new ProfileData();
            this.DataContext = CurrentUserProfile;
            _currentAdminId = adminId;
            this.Loaded += Profile_Loaded_Async;
        }

        private async void Profile_Loaded_Async(object sender, RoutedEventArgs e)
        {
            // Unsubscribe to prevent multiple loads if Loaded fires again
            this.Loaded -= Profile_Loaded_Async;
            await LoadProfileDataAsync(_currentAdminId);
        }

        private async Task LoadProfileDataAsync(int adminId)
        {
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Profile.xaml.cs: Attempting to load profile for Admin_ID: {adminId}");
            ProfileData? loadedProfile = await ProfileData.LoadAdminProfileAsync(adminId);
            if (loadedProfile != null)
            {
                // Update properties of the existing CurrentUserProfile instance
                // to maintain data binding if it was already set.
                CurrentUserProfile.AdminId = loadedProfile.AdminId;
                CurrentUserProfile.FullName = loadedProfile.FullName;
                CurrentUserProfile.DisplayName = loadedProfile.DisplayName;
                CurrentUserProfile.DateOfBirth = loadedProfile.DateOfBirth;
                CurrentUserProfile.Gender = loadedProfile.Gender;
                CurrentUserProfile.Nationality = loadedProfile.Nationality;
                CurrentUserProfile.Address = loadedProfile.Address;
                CurrentUserProfile.PhoneNumber = loadedProfile.PhoneNumber;
                CurrentUserProfile.Email = loadedProfile.Email;
                CurrentUserProfile.AccountCreated = loadedProfile.AccountCreated;
                CurrentUserProfile.AccountVerification = loadedProfile.AccountVerification;
                CurrentUserProfile.Role = loadedProfile.Role;
                CurrentUserProfile.Username = loadedProfile.Username;
                CurrentUserProfile.LanguagePreference = loadedProfile.LanguagePreference;
                CurrentUserProfile.TimeZone = loadedProfile.TimeZone;
                CurrentUserProfile.StatusFromDB = loadedProfile.StatusFromDB;

                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Profile.xaml.cs: Profile loaded for {CurrentUserProfile.FullName}");

                // Update XAML TextBlocks directly (optional if full binding is working perfectly)
                // This ensures UI update even if there were issues with initial DataContext propagation.
                ProfileNameTextBlock.Text = CurrentUserProfile.DisplayName;
                ProfileEmailHeaderTextBlock.Text = CurrentUserProfile.Email;
                FullNameTextBlock.Text = CurrentUserProfile.FullName;
                DobTextBlock.Text = CurrentUserProfile.DateOfBirth?.ToString("MMMM d, yyyy") ?? "N/A";
                GenderTextBlock.Text = CurrentUserProfile.Gender;
                NationalityTextBlock.Text = CurrentUserProfile.Nationality;
                AddressTextBlock.Text = CurrentUserProfile.Address;
                PhoneNumberTextBlock.Text = CurrentUserProfile.PhoneNumber;
                DisplayNameTextBlock.Text = CurrentUserProfile.DisplayName; // In account details section
                AccountCreatedTextBlock.Text = CurrentUserProfile.AccountCreated.ToString("MMMM d, yyyy");
                EmailTextBlock.Text = CurrentUserProfile.Email; // In account details section
                AccountVerificationTextBlock.Text = CurrentUserProfile.AccountVerification;
                LanguagePreferenceTextBlock.Text = CurrentUserProfile.LanguagePreference;
                TimeZoneTextBlock.Text = CurrentUserProfile.TimeZone;
            }
            else
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Profile.xaml.cs: Failed to load profile for Admin_ID: {adminId}");
                // MessageBox is already shown in LoadAdminProfileAsync if connection string is missing or load fails
            }
        }

        private async void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentUserProfile == null || CurrentUserProfile.AdminId == 0)
            {
                // Attempt to load profile again if it's somehow not loaded
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] EditProfileButton_Click: CurrentUserProfile is null or AdminId is 0. Attempting reload for ID: {_currentAdminId}");
                await LoadProfileDataAsync(_currentAdminId); // Ensure data is loaded
                if (CurrentUserProfile == null || CurrentUserProfile.AdminId == 0)
                {
                    MessageBox.Show("Profile data is not available. Cannot edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }

            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] EditProfileButton_Click: Navigating to EditProfilePage for Admin_ID: {CurrentUserProfile.AdminId}");
            EditProfilePage editPage = new EditProfilePage(CurrentUserProfile); // Pass the ViewModel instance

            // Handle navigation back and refresh
            if (this.NavigationService != null)
            {
                NavigatedEventHandler? onNavigatedBackFromEdit = null;
                onNavigatedBackFromEdit = async (navSender, navArgs) =>
                {
                    if (navArgs.Content == this) // Navigated back to this Profile page
                    {
                        Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Navigated back to Profile page. Refreshing data for Admin_ID: {_currentAdminId}");
                        await LoadProfileDataAsync(_currentAdminId); // Reload data to reflect any changes
                        if (this.NavigationService != null)
                        {
                            this.NavigationService.Navigated -= onNavigatedBackFromEdit; // Unsubscribe
                        }
                    }
                };
                this.NavigationService.Navigated += onNavigatedBackFromEdit;
                this.NavigationService.Navigate(editPage);
            }
            else
            {
                MessageBox.Show("Navigation service not available.", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
