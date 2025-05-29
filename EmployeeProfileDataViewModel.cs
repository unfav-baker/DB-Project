using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Employee // Or your appropriate namespace
{
    public class EmployeeProfileDataViewModel : INotifyPropertyChanged
    {
        private int _employeeId;
        public int EmployeeId { get => _employeeId; set { _employeeId = value; OnPropertyChanged(); } }

        private string _fullName = string.Empty;
        public string FullName { get => _fullName; set { _fullName = value; OnPropertyChanged(); } }

        private string _gender = string.Empty;
        public string Gender { get => _gender; set { _gender = value; OnPropertyChanged(); } }

        private DateTime? _dateOfBirth;
        public DateTime? DateOfBirth { get => _dateOfBirth; set { _dateOfBirth = value; OnPropertyChanged(); } }

        private string _nationality = string.Empty;
        public string Nationality { get => _nationality; set { _nationality = value; OnPropertyChanged(); } }

        private string _email = string.Empty;
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }

        private string _phoneNumber = string.Empty;
        public string PhoneNumber { get => _phoneNumber; set { _phoneNumber = value; OnPropertyChanged(); } }

        private string _address = string.Empty;
        public string Address { get => _address; set { _address = value; OnPropertyChanged(); } }

        private DateTime? _lastLogin;
        public DateTime? LastLogin { get => _lastLogin; set { _lastLogin = value; OnPropertyChanged(); } }

        private DateTime? _hireDate;
        public DateTime? HireDate { get => _hireDate; set { _hireDate = value; OnPropertyChanged(); } }

        private string _role = string.Empty;
        public string Role { get => _role; set { _role = value; OnPropertyChanged(); } }

        private bool _loginNotificationsEnabled;
        public bool LoginNotificationsEnabled { get => _loginNotificationsEnabled; set { _loginNotificationsEnabled = value; OnPropertyChanged(); } }

        private string _managingAdminName = string.Empty;
        public string ManagingAdminName { get => _managingAdminName; set { _managingAdminName = value; OnPropertyChanged(); } }

        private string _languagePreference = string.Empty;
        public string LanguagePreference { get => _languagePreference; set { _languagePreference = value; OnPropertyChanged(); } }

        private string _timeZone = string.Empty;
        public string TimeZone { get => _timeZone; set { _timeZone = value; OnPropertyChanged(); } }

        private string _status = string.Empty;
        public string Status { get => _status; set { _status = value; OnPropertyChanged(); } }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
