using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Supplier
{
    public class OrderSummaryData : INotifyPropertyChanged
    {
        private bool _isSelected;
        public bool IsSelected { get => _isSelected; set { _isSelected = value; OnPropertyChanged(); } }

        private int _orderId; // From orders table
        public int OrderId { get => _orderId; set { _orderId = value; OnPropertyChanged(); } }

        private DateTime _orderDate;
        public DateTime OrderDate { get => _orderDate; set { _orderDate = value; OnPropertyChanged(); } }

        private string _partyName = string.Empty; // From customer table
        public string PartyName { get => _partyName; set { _partyName = value; OnPropertyChanged(); } }

        private string _exportThrough = string.Empty;
        public string ExportThrough { get => _exportThrough; set { _exportThrough = value; OnPropertyChanged(); } }

        private string _plant = string.Empty;
        public string Plant { get => _plant; set { _plant = value; OnPropertyChanged(); } }

        private string _importer = string.Empty;
        public string Importer { get => _importer; set { _importer = value; OnPropertyChanged(); } }

        private string _phytoNumber = string.Empty;
        public string PhytoNumber { get => _phytoNumber; set { _phytoNumber = value; OnPropertyChanged(); } }

        private int? _carton;
        public int? Carton { get => _carton; set { _carton = value; OnPropertyChanged(); } }

        private decimal? _weight;
        public decimal? Weight { get => _weight; set { _weight = value; OnPropertyChanged(); } }

        private decimal? _rate;
        public decimal? Rate { get => _rate; set { _rate = value; OnPropertyChanged(); } }

        private decimal _totalAmount;
        public decimal TotalAmount { get => _totalAmount; set { _totalAmount = value; OnPropertyChanged(); } }

        private decimal? _amountReceived;
        public decimal? AmountReceived { get => _amountReceived; set { _amountReceived = value; OnPropertyChanged(); } }

        private decimal _balance;
        public decimal Balance { get => _balance; set { _balance = value; OnPropertyChanged(); } }

        // Additional properties from your original Adminn.ReportData if needed for other actions
        // For example, if the "View Reports" button needs more context from the selected row
        // private int? _employeeId;
        // public int? EmployeeId { get => _employeeId; set { _employeeId = value; OnPropertyChanged(); } }
        // private string _reportType = string.Empty; // This might not be relevant if showing order data
        // public string ReportType { get => _reportType; set { _reportType = value ?? string.Empty; OnPropertyChanged(); } }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
