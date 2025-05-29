using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Employee // Or your appropriate namespace
{
    public class NewOrderEntryData : INotifyPropertyChanged
    {
        private int? _selectedCustomerId;
        public int? SelectedCustomerId { get => _selectedCustomerId; set { _selectedCustomerId = value; OnPropertyChanged(); } }

        private int? _selectedProductId;
        public int? SelectedProductId { get => _selectedProductId; set { _selectedProductId = value; OnPropertyChanged(); UpdateRateFromProduct(); CalculateTotalAmount(); } }

        private int? _selectedSupplierId;
        public int? SelectedSupplierId { get => _selectedSupplierId; set { _selectedSupplierId = value; OnPropertyChanged(); } }

        private DateTime _orderDate = DateTime.Today;
        public DateTime OrderDate { get => _orderDate; set { _orderDate = value; OnPropertyChanged(); } }

        private DateTime? _deliveryDate;
        public DateTime? DeliveryDate { get => _deliveryDate; set { _deliveryDate = value; OnPropertyChanged(); } }

        private string _exportThrough = string.Empty;
        public string ExportThrough { get => _exportThrough; set { _exportThrough = value; OnPropertyChanged(); } }

        private string _plant = string.Empty; // For specific plant details for this order
        public string Plant { get => _plant; set { _plant = value; OnPropertyChanged(); } }

        // Importer property removed as it's the same as Customer

        private string _phytoNumber = string.Empty; // Was BatchNo, now PhytoNumber
        public string PhytoNumber { get => _phytoNumber; set { _phytoNumber = value; OnPropertyChanged(); } }

        private int _quantity;
        public int Quantity { get => _quantity; set { if (_quantity != value) { _quantity = value; OnPropertyChanged(); CalculateTotalAmount(); } } }

        private int? _carton;
        public int? Carton { get => _carton; set { _carton = value; OnPropertyChanged(); } }

        private decimal? _weight;
        public decimal? Weight { get => _weight; set { _weight = value; OnPropertyChanged(); } }

        private decimal _rate;
        public decimal Rate { get => _rate; set { if (_rate != value) { _rate = value; OnPropertyChanged(); CalculateTotalAmount(); } } }

        private decimal _totalAmount;
        public decimal TotalAmount { get => _totalAmount; private set { if (_totalAmount != value) { _totalAmount = value; OnPropertyChanged(); } } }

        private decimal? _amountReceived;
        public decimal? AmountReceived { get => _amountReceived; set { _amountReceived = value; OnPropertyChanged(); } }

        private string _orderStatus = "Pending";
        public string OrderStatus { get => _orderStatus; set { _orderStatus = value; OnPropertyChanged(); } }

        private string _paymentStatus = "Pending";
        public string PaymentStatus { get => _paymentStatus; set { _paymentStatus = value; OnPropertyChanged(); } }

        // BatchNo property removed

        private string _vehicleNo = string.Empty;
        public string VehicleNo { get => _vehicleNo; set { _vehicleNo = value; OnPropertyChanged(); } }

        private ProductSelectionItem? _currentSelectedProduct;
        public ProductSelectionItem? CurrentSelectedProduct
        {
            get => _currentSelectedProduct;
            set
            {
                if (_currentSelectedProduct != value)
                {
                    _currentSelectedProduct = value;
                    OnPropertyChanged();
                    UpdateRateFromProduct();
                }
            }
        }

        private void UpdateRateFromProduct()
        {
            if (CurrentSelectedProduct != null)
            {
                Rate = CurrentSelectedProduct.Price;
            }
        }

        public void CalculateTotalAmount()
        {
            TotalAmount = Quantity * Rate;
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public void Clear()
        {
            SelectedCustomerId = null;
            SelectedProductId = null;
            CurrentSelectedProduct = null; // This will also trigger UpdateRateFromProduct if implemented correctly
            SelectedSupplierId = null;
            OrderDate = DateTime.Today;
            DeliveryDate = null;
            ExportThrough = string.Empty;
            Plant = string.Empty;
            PhytoNumber = string.Empty;
            Quantity = 0;
            Carton = null;
            Weight = null;
            Rate = 0; // Will be updated if a product is selected again
            // TotalAmount will be recalculated by Rate/Quantity setters
            AmountReceived = null;
            OrderStatus = "Pending";
            PaymentStatus = "Pending";
            VehicleNo = string.Empty;
        }
    }
}
