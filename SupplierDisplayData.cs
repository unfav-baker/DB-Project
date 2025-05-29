using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Employee // Or your appropriate namespace
{
    public class SupplierDisplayData : INotifyPropertyChanged
    {
        private int _supplierId;
        public int SupplierId { get => _supplierId; set { _supplierId = value; OnPropertyChanged(); } }

        private string _name = string.Empty;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }

        private string _phoneNumber = string.Empty;
        public string PhoneNumber { get => _phoneNumber; set { _phoneNumber = value; OnPropertyChanged(); } }

        private string _email = string.Empty;
        public string Email { get => _email; set { _email = value; OnPropertyChanged(); } }

        private string _plantName = string.Empty;
        public string PlantName { get => _plantName; set { _plantName = value; OnPropertyChanged(); } }

        private string _status = string.Empty;
        public string Status { get => _status; set { _status = value; OnPropertyChanged(); } }

        // Add any other supplier fields you want to display on the card
        // e.g., Address, SupplierRating

        // For selection state if using ListBox or similar
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
