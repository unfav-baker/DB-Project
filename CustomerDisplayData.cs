using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Employee // Or your appropriate namespace
{
    public class CustomerDisplayData : INotifyPropertyChanged
    {
        private int _customerId;
        public int CustomerId { get => _customerId; set { _customerId = value; OnPropertyChanged(); } }

        private string _name = string.Empty;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }

        private string _phoneNumber = string.Empty;
        public string PhoneNumber { get => _phoneNumber; set { _phoneNumber = value; OnPropertyChanged(); } }

        private string _email = string.Empty;
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }

        private string _address = string.Empty;
        public string Address { get => _address; set { _address = value; OnPropertyChanged(); } }

        private string _status = string.Empty;
        public string Status { get => _status; set { _status = value; OnPropertyChanged(); } }

        private string _customerType = string.Empty;
        public string CustomerType { get => _customerType; set { _customerType = value; OnPropertyChanged(); } }

        private string _businessName = string.Empty;
        public string BusinessName { get => _businessName; set { _businessName = value; OnPropertyChanged(); } }

        private bool _isSelectedAsCurrent;
        public bool IsSelectedAsCurrent
        {
            get => _isSelectedAsCurrent;
            set { _isSelectedAsCurrent = value; OnPropertyChanged(); }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}