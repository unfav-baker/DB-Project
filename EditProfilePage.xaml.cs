using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation; // For NavigationService
using System.Diagnostics;    // For Debug.WriteLine
using System.Threading.Tasks; // For async Task

namespace Adminn
{
    public partial class EditProfilePage : Page
    {
        public ProfileData ProfileToEdit { get; private set; } // Data context for bindings

        // Store a copy of the original data to revert on cancel, or to compare if only changed fields should be saved
        private ProfileData originalProfileDataCopy;

        public EditProfilePage(ProfileData profileDataInstance)
        {
            InitializeComponent();

            // Create a deep copy for editing, so original instance isn't modified until save
            originalProfileDataCopy = new ProfileData
            {
                AdminId = profileDataInstance.AdminId,
                FullName = profileDataInstance.FullName,
                DisplayName = profileDataInstance.DisplayName,
                DateOfBirth = profileDataInstance.DateOfBirth,
                Gender = profileDataInstance.Gender,
                Nationality = profileDataInstance.Nationality,
                Address = profileDataInstance.Address,
                PhoneNumber = profileDataInstance.PhoneNumber,
                Email = profileDataInstance.Email,
                LanguagePreference = profileDataInstance.LanguagePreference,
                TimeZone = profileDataInstance.TimeZone,
                // Read-only fields that shouldn't be edited here but might be needed for context
                AccountCreated = profileDataInstance.AccountCreated,
                AccountVerification = profileDataInstance.AccountVerification, // Status
                Role = profileDataInstance.Role,
                Username = profileDataInstance.Username,
                StatusFromDB = profileDataInstance.StatusFromDB
            };

            ProfileToEdit = originalProfileDataCopy; // Bind to the copy
            this.DataContext = ProfileToEdit;
            LoadDataIntoForm(); // Not strictly needed if direct binding works, but good for ComboBoxes etc.
        }

        private void LoadDataIntoForm()
        {
            // TextBoxes will be populated by DataBinding.
            // This method is useful if you need to set ComboBox selected items programmatically
            // based on the string value from ProfileToEdit, if ComboBoxes are not directly bound
            // to a list of strings or if items are more complex.

            // Example for Gender ComboBox:
            if (GenderEditComboBox != null && !string.IsNullOrEmpty(ProfileToEdit.Gender))
            {
                foreach (ComboBoxItem item in GenderEditComboBox.Items)
                {
                    if (item.Content?.ToString() == ProfileToEdit.Gender)
                    {
                        GenderEditComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
        }

        private async void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            // Update ProfileToEdit properties from controls IF NOT using TwoWay binding
            // With TwoWay binding (default for TextBox), ProfileToEdit is already updated.
            // For ComboBoxes, ensure SelectedValue is bound to ProfileToEdit property.
            // For DatePicker, SelectedDate is bound.

            // Example: If ComboBox SelectedItem is used instead of SelectedValue binding:
            if (GenderEditComboBox.SelectedItem is ComboBoxItem selectedGenderItem)
            {
                ProfileToEdit.Gender = selectedGenderItem.Content?.ToString() ?? string.Empty;
            }

            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] EditProfilePage: Attempting to save profile for Admin_ID: {ProfileToEdit.AdminId}");
            bool success = await ProfileToEdit.SaveProfileAsync();

            if (success)
            {
                MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                // The original instance passed to Profile.xaml needs to be updated IF EditProfilePage modified a copy.
                // If EditProfilePage modified the original instance directly, then Profile.xaml will see changes.
                // Since we are passing the shared instance _sharedUserProfileInstance directly to EditProfilePage,
                // any changes made to _profileToEdit (which is _sharedUserProfileInstance) will reflect back.
                // The ProfileUpdated event is an alternative way to signal changes.
                // ProfileUpdated?.Invoke(this, _profileToEdit); // This was in your original code, can be used.

                if (this.NavigationService is { CanGoBack: true })
                {
                    this.NavigationService.GoBack(); // This will trigger Navigated event in Profile.xaml.cs
                }
            }
            else
            {
                MessageBox.Show("Failed to update profile. Please check details or try again.", "Update Failed", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] EditProfilePage: Cancel clicked.");
            if (this.NavigationService is { CanGoBack: true })
            {
                this.NavigationService.GoBack();
            }
        }
    }
}
