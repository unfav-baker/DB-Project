using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Supplier // Or your appropriate namespace
{
    public class ReportDisplayData : INotifyPropertyChanged
    {
        private DateTime _orderDate;
        public DateTime OrderDate { get => _orderDate; set { _orderDate = value; OnPropertyChanged(); } }

        private string _partyName = string.Empty;
        public string PartyName { get => _partyName; set { _partyName = value; OnPropertyChanged(); } }

        private string _exportThrough = string.Empty;
        public string ExportThrough { get => _exportThrough; set { _exportThrough = value; OnPropertyChanged(); } }

        private string _plant = string.Empty;
        public string Plant { get => _plant; set { _plant = value; OnPropertyChanged(); } }

        private string _importer = string.Empty;
        public string Importer { get => _importer; set { _importer = value; OnPropertyChanged(); } }

        private string _phytoNumber = string.Empty;
        public string PhytoNumber { get => _phytoNumber; set { _phytoNumber = value; OnPropertyChanged(); } }

        private int? _carton; // Nullable int
        public int? Carton { get => _carton; set { _carton = value; OnPropertyChanged(); } }

        private decimal? _weight; // Nullable decimal
        public decimal? Weight { get => _weight; set { _weight = value; OnPropertyChanged(); } }

        private decimal? _rate; // Nullable decimal
        public decimal? Rate { get => _rate; set { _rate = value; OnPropertyChanged(); } }

        private decimal _totalAmount;
        public decimal TotalAmount { get => _totalAmount; set { _totalAmount = value; OnPropertyChanged(); } }

        private decimal? _amountReceived; // Nullable decimal for what's received
        public decimal? AmountReceived { get => _amountReceived; set { _amountReceived = value; OnPropertyChanged(); } }

        private decimal _balance; // Calculated
        public decimal Balance { get => _balance; set { _balance = value; OnPropertyChanged(); } }

        // For selection in DataGrid if needed
        private bool _isSelected;
        public bool IsSelected { get => _isSelected; set { _isSelected = value; OnPropertyChanged(); } }

        public int OrderId { get; set; } // Hidden ID for potential actions

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
