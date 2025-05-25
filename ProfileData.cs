using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Adminn // Your project's namespace
{
    public class ProfileData : INotifyPropertyChanged
    {
        private string _fullName = string.Empty;
        public string FullName
        {
            get => _fullName;
            set { _fullName = value ?? string.Empty; OnPropertyChanged(); }
        }

        private string _displayName = string.Empty;
        public string DisplayName
        {
            get => _displayName;
            set { _displayName = value ?? string.Empty; OnPropertyChanged(); }
        }

        private DateTime? _dateOfBirth; // Already nullable, which is fine
        public DateTime? DateOfBirth
        {
            get => _dateOfBirth;
            set { _dateOfBirth = value; OnPropertyChanged(); }
        }

        private string _gender = string.Empty;
        public string Gender
        {
            get => _gender;
            set { _gender = value ?? string.Empty; OnPropertyChanged(); }
        }

        private string _nationality = string.Empty;
        public string Nationality
        {
            get => _nationality;
            set { _nationality = value ?? string.Empty; OnPropertyChanged(); }
        }

        private string _address = string.Empty;
        public string Address
        {
            get => _address;
            set { _address = value ?? string.Empty; OnPropertyChanged(); }
        }

        private string _phoneNumber = string.Empty;
        public string PhoneNumber
        {
            get => _phoneNumber;
            set { _phoneNumber = value ?? string.Empty; OnPropertyChanged(); }
        }

        private string _email = string.Empty;
        public string Email
        {
            get => _email;
            set { _email = value ?? string.Empty; OnPropertyChanged(); }
        }

        // Initialize non-nullable properties
        public DateTime AccountCreated { get; set; } = DateTime.MinValue;
        public string AccountVerification { get; set; } = string.Empty;

        private string _languagePreference = string.Empty;
        public string LanguagePreference
        {
            get => _languagePreference;
            set { _languagePreference = value ?? string.Empty; OnPropertyChanged(); }
        }

        private string _timeZone = string.Empty;
        public string TimeZone
        {
            get => _timeZone;
            set { _timeZone = value ?? string.Empty; OnPropertyChanged(); }
        }

        // Declare the event as nullable to match INotifyPropertyChanged and satisfy NRT rules
        public event PropertyChangedEventHandler? PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        // Method to load dummy data or from a source
        public static ProfileData LoadCurrentProfile()
        {
            // In a real app, load from DB, file, API etc.
            return new ProfileData
            {
                FullName = "Muhammad Fayaz",
                DisplayName = "Muhammad Fayyaz",
                DateOfBirth = new DateTime(1987, 1, 1),
                Gender = "Male",
                Nationality = "Pakistani",
                Address = "S-4 AB Heights, Airport Road, Lhr",
                PhoneNumber = "0301-8515333",
                Email = "Fayyaz@Primefoods.com.pk",
                AccountCreated = new DateTime(2020, 3, 20),
                AccountVerification = "Verified", // This will overwrite the default string.Empty
                LanguagePreference = "English",
                TimeZone = "(UTC + 5)"
            };
        }

        // This method might not be strictly necessary if using INotifyPropertyChanged correctly
        // and the ProfileData object itself is shared and updated.
        // However, if EditProfilePage creates a new instance or modifies a copy, this could be relevant.
        // For now, we assume EditProfilePage modifies the passed instance.
        public void UpdateFrom(ProfileData source)
        {
            if (source == null) return;

            this.FullName = source.FullName;
            this.DisplayName = source.DisplayName;
            this.DateOfBirth = source.DateOfBirth;
            this.Gender = source.Gender;
            this.Nationality = source.Nationality;
            this.Address = source.Address;
            this.PhoneNumber = source.PhoneNumber;
            this.Email = source.Email;
            // AccountCreated and AccountVerification are typically not edited by the user in this way.
            this.LanguagePreference = source.LanguagePreference;
            this.TimeZone = source.TimeZone;
        }
    }
}