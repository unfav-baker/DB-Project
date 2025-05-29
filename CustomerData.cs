using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Adminn
{
    public class CustomerData : INotifyPropertyChanged
    {
        private bool _isSelected;
        public bool IsSelected { get => _isSelected; set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } } }

        private int _customerId; // From Customer_ID
        public int CustomerId { get => _customerId; set { if (_customerId != value) { _customerId = value; OnPropertyChanged(); } } }

        private string _name = string.Empty; // Party Name
        public string Name { get => _name; set { if (_name != value) { _name = value ?? string.Empty; OnPropertyChanged(); } } }

        private string _phoneNumber = string.Empty;
        public string PhoneNumber { get => _phoneNumber; set { if (_phoneNumber != value) { _phoneNumber = value ?? string.Empty; OnPropertyChanged(); } } }

        private string _email = string.Empty;
        public string Email { get => _email; set { if (_email != value) { _email = value ?? string.Empty; OnPropertyChanged(); } } }

        private string _address = string.Empty;
        public string Address { get => _address; set { if (_address != value) { _address = value ?? string.Empty; OnPropertyChanged(); } } }

        private string _customerType = string.Empty;
        public string CustomerType { get => _customerType; set { if (_customerType != value) { _customerType = value ?? string.Empty; OnPropertyChanged(); } } }

        private string _businessName = string.Empty;
        public string BusinessName { get => _businessName; set { if (_businessName != value) { _businessName = value ?? string.Empty; OnPropertyChanged(); } } }

        private string _status = string.Empty;
        public string Status { get => _status; set { if (_status != value) { _status = value ?? string.Empty; OnPropertyChanged(); } } }

        private DateTime _createdAt;
        public DateTime CreatedAt { get => _createdAt; set { if (_createdAt != value) { _createdAt = value; OnPropertyChanged(); } } }

        // Removed fields that belong to 'orders' table:
        // Date, PhytoNumber, ExportThrough, Plant, Importer, Carton, Weight, Rate, Amount, Received

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
