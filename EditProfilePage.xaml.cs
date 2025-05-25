using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace Adminn // Your project's namespace
{
    public partial class EditProfilePage : Page
    {
        private ProfileData? _profileToEdit;
        public event EventHandler<ProfileData?>? ProfileUpdated;

        public EditProfilePage(ProfileData? profileData)
        {
            InitializeComponent();
            _profileToEdit = profileData;
            LoadDataIntoForm();
        }

        private void LoadDataIntoForm()
        {
            if (_profileToEdit == null)
            {
                MessageBox.Show("No profile data to edit.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (this.NavigationService != null && this.NavigationService.CanGoBack)
                {
                    this.NavigationService.GoBack();
                }
                return;
            }

            FullNameEditTextBox.Text = _profileToEdit.FullName;
            DisplayNameEditTextBox.Text = _profileToEdit.DisplayName;
            DobEditDatePicker.SelectedDate = _profileToEdit.DateOfBirth;

            // MODIFIED: Load Gender into ComboBox
            GenderEditComboBox.Items.Refresh(); // Ensure items are available if dynamically added (not the case here but good practice)
            if (!string.IsNullOrEmpty(_profileToEdit.Gender))
            {
                bool genderSet = false;
                foreach (ComboBoxItem item in GenderEditComboBox.Items)
                {
                    if (item.Content.ToString() == _profileToEdit.Gender)
                    {
                        GenderEditComboBox.SelectedItem = item;
                        genderSet = true;
                        break;
                    }
                }
                if (!genderSet) // If the saved gender isn't one of the predefined items
                {
                    GenderEditComboBox.SelectedIndex = -1; // Or select a default, e.g., "Other"
                }
            }
            else
            {
                GenderEditComboBox.SelectedIndex = -1; // No selection
            }

            NationalityEditTextBox.Text = _profileToEdit.Nationality;
            AddressEditTextBox.Text = _profileToEdit.Address;
            PhoneNumberEditTextBox.Text = _profileToEdit.PhoneNumber;
            EmailEditTextBox.Text = _profileToEdit.Email;
            LanguagePreferenceEditTextBox.Text = _profileToEdit.LanguagePreference;
            TimeZoneEditTextBox.Text = _profileToEdit.TimeZone;
        }

        private void SaveChangesButton_Click(object sender, RoutedEventArgs e)
        {
            if (_profileToEdit == null)
            {
                MessageBox.Show("Cannot save, profile data is missing.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            _profileToEdit.FullName = FullNameEditTextBox.Text;
            _profileToEdit.DisplayName = DisplayNameEditTextBox.Text;
            _profileToEdit.DateOfBirth = DobEditDatePicker.SelectedDate;

            // MODIFIED: Save Gender from ComboBox
            if (GenderEditComboBox.SelectedItem != null)
            {
                _profileToEdit.Gender = (GenderEditComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? string.Empty;
            }
            else
            {
                _profileToEdit.Gender = string.Empty; // Or a default value if preferred
            }

            _profileToEdit.Nationality = NationalityEditTextBox.Text;
            _profileToEdit.Address = AddressEditTextBox.Text;
            _profileToEdit.PhoneNumber = PhoneNumberEditTextBox.Text;
            _profileToEdit.Email = EmailEditTextBox.Text;
            _profileToEdit.LanguagePreference = LanguagePreferenceEditTextBox.Text;
            _profileToEdit.TimeZone = TimeZoneEditTextBox.Text;

            ProfileUpdated?.Invoke(this, _profileToEdit);

            if (this.NavigationService != null && this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService != null && this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }
    }
}