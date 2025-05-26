// SupplierDataModel.cs
using System;
using System.ComponentModel; // Required for INotifyPropertyChanged
using System.Runtime.CompilerServices; // Required for CallerMemberName

namespace Supplier
{
    public class SupplierDataModel : INotifyPropertyChanged
    {
        private string _fullName;
        public string FullName
        {
            get => _fullName;
            set { _fullName = value; OnPropertyChanged(); }
        }

        private string _gender;
        public string Gender
        {
            get => _gender;
            set { _gender = value; OnPropertyChanged(); }
        }

        private DateTime? _dateOfBirth;
        public DateTime? DateOfBirth
        {
            get => _dateOfBirth;
            set { _dateOfBirth = value; OnPropertyChanged(); }
        }

        private string _nationality;
        public string Nationality
        {
            get => _nationality;
            set { _nationality = value; OnPropertyChanged(); }
        }

        private string _email;
        public string Email
        {
            get => _email;
            set { _email = value; OnPropertyChanged(); }
        }

        private string _phoneNumber;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set { _phoneNumber = value; OnPropertyChanged(); }
        }

        private string _supplierID;
        public string SupplierID
        {
            get => _supplierID;
            set { _supplierID = value; OnPropertyChanged(); }
        }

        private DateTime? _lastLogin;
        public DateTime? LastLogin
        {
            get => _lastLogin;
            set { _lastLogin = value; OnPropertyChanged(); }
        }

        private DateTime? _registrationDate;
        public DateTime? RegistrationDate // Changed from HireDate
        {
            get => _registrationDate;
            set { _registrationDate = value; OnPropertyChanged(); }
        }

        private string _roleOrType; // Changed from Role
        public string RoleOrType
        {
            get => _roleOrType;
            set { _roleOrType = value; OnPropertyChanged(); }
        }

        private string _accountStatus; // Changed from LoginNotifications
        public string AccountStatus
        {
            get => _accountStatus;
            set { _accountStatus = value; OnPropertyChanged(); }
        }

        private string _verificationStatus; // Changed from AdminStatus
        public string VerificationStatus
        {
            get => _verificationStatus;
            set { _verificationStatus = value; OnPropertyChanged(); }
        }

        private string _languagePreference;
        public string LanguagePreference
        {
            get => _languagePreference;
            set { _languagePreference = value; OnPropertyChanged(); }
        }

        private string _timeZone;
        public string TimeZone
        {
            get => _timeZone;
            set { _timeZone = value; OnPropertyChanged(); }
        }

        private string _imagePath;
        public string ImagePath
        {
            get => _imagePath;
            set { _imagePath = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        // Using [CallerMemberName] simplifies OnPropertyChanged calls
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}