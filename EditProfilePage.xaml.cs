// EditProfilePage.xaml.cs
using System;
using System.Windows;
using System.Windows.Controls;

namespace Supplier
{
    public partial class EditProfilePage : Page
    {
        private SupplierDataModel _supplierToEdit;

        // Constructor that accepts the supplier data to be edited
        public EditProfilePage(SupplierDataModel supplier)
        {
            InitializeComponent();
            _supplierToEdit = supplier;
            LoadSupplierData();
        }

        private void LoadSupplierData()
        {
            if (_supplierToEdit == null) return;

            // Populate TextBoxes and DatePickers with current data
            FullNameTextBox.Text = _supplierToEdit.FullName;
            GenderTextBox.Text = _supplierToEdit.Gender; // Consider ComboBox
            DobDatePicker.SelectedDate = _supplierToEdit.DateOfBirth;
            // If using TextBox for DOB: DobTextBox.Text = _supplierToEdit.DateOfBirth?.ToString("yyyy-MM-dd");
            NationalityTextBox.Text = _supplierToEdit.Nationality;
            EmailTextBox.Text = _supplierToEdit.Email;
            PhoneNumberTextBox.Text = _supplierToEdit.PhoneNumber;

            SupplierIdTextBox.Text = _supplierToEdit.SupplierID; // Usually read-only
            RegistrationDatePicker.SelectedDate = _supplierToEdit.RegistrationDate;
            // If using TextBox for RegDate: RegistrationDateTextBox.Text = _supplierToEdit.RegistrationDate?.ToString("yyyy-MM-dd");
            RoleOrTypeTextBox.Text = _supplierToEdit.RoleOrType;
            AccountStatusTextBox.Text = _supplierToEdit.AccountStatus;
            VerificationStatusTextBox.Text = _supplierToEdit.VerificationStatus;

            LanguagePreferenceTextBox.Text = _supplierToEdit.LanguagePreference;
            TimeZoneTextBox.Text = _supplierToEdit.TimeZone;
            ImagePathTextBox.Text = _supplierToEdit.ImagePath;
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (_supplierToEdit == null) return;

            // Update the _supplierToEdit object with values from TextBoxes
            _supplierToEdit.FullName = FullNameTextBox.Text;
            _supplierToEdit.Gender = GenderTextBox.Text; // Get value from ComboBox if used

            // Handle DatePicker, ensuring nullable DateTime compatibility
            _supplierToEdit.DateOfBirth = DobDatePicker.SelectedDate;
            // If using TextBox for DOB, parse carefully:
            // if (DateTime.TryParse(DobTextBox.Text, out DateTime dob))
            // _supplierToEdit.DateOfBirth = dob;
            // else
            // _supplierToEdit.DateOfBirth = null; // Or handle error

            _supplierToEdit.Nationality = NationalityTextBox.Text;
            _supplierToEdit.Email = EmailTextBox.Text;
            _supplierToEdit.PhoneNumber = PhoneNumberTextBox.Text;

            // SupplierID is usually not changed here

            _supplierToEdit.RegistrationDate = RegistrationDatePicker.SelectedDate;
            // If using TextBox for RegDate:
            // if (DateTime.TryParse(RegistrationDateTextBox.Text, out DateTime regDate))
            // _supplierToEdit.RegistrationDate = regDate;
            // else
            // _supplierToEdit.RegistrationDate = null; // Or handle error

            _supplierToEdit.RoleOrType = RoleOrTypeTextBox.Text;
            _supplierToEdit.AccountStatus = AccountStatusTextBox.Text;
            _supplierToEdit.VerificationStatus = VerificationStatusTextBox.Text;

            _supplierToEdit.LanguagePreference = LanguagePreferenceTextBox.Text;
            _supplierToEdit.TimeZone = TimeZoneTextBox.Text;
            _supplierToEdit.ImagePath = ImagePathTextBox.Text;

            // In a real app, here you would persist changes to a database or service.
            // For now, we are just updating the object in memory.

            MessageBox.Show("Profile updated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            // Navigate back to the profile page
            if (this.NavigationService != null && this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back without saving
            if (this.NavigationService != null && this.NavigationService.CanGoBack)
            {
                this.NavigationService.GoBack();
            }
        }
    }
}