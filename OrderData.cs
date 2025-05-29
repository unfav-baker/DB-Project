using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Adminn // Ensure this matches your project's namespace
{
    public class OrderData : INotifyPropertyChanged
    {
        private bool _isSelected;
        private int _orderId;
        private string _customerName = string.Empty;
        private int _productId;
        private int _quantityOrdered;
        private DateTime _orderDate;
        private DateTime _deliveryDate; // Consider making this Nullable: DateTime?
        private string _orderStatus = string.Empty;
        private decimal _totalPrice;
        private string _paymentStatus = string.Empty;
        private int? _supplierId; // Nullable int for Supplier_ID

        public bool IsSelected
        {
            get => _isSelected;
            set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } }
        }

        public int OrderId
        {
            get => _orderId;
            set { if (_orderId != value) { _orderId = value; OnPropertyChanged(); } }
        }

        public string CustomerName
        {
            get => _customerName;
            set { if (_customerName != value) { _customerName = value ?? string.Empty; OnPropertyChanged(); } }
        }

        public int ProductId
        {
            get => _productId;
            set { if (_productId != value) { _productId = value; OnPropertyChanged(); } }
        }

        public int QuantityOrdered
        {
            get => _quantityOrdered;
            set { if (_quantityOrdered != value) { _quantityOrdered = value; OnPropertyChanged(); } }
        }

        public DateTime OrderDate
        {
            get => _orderDate;
            set { if (_orderDate != value) { _orderDate = value; OnPropertyChanged(); } }
        }

        public DateTime DeliveryDate // If DeliveryDate can be null in DB, make this DateTime?
        {
            get => _deliveryDate;
            set { if (_deliveryDate != value) { _deliveryDate = value; OnPropertyChanged(); } }
        }

        public string OrderStatus
        {
            get => _orderStatus;
            set { if (_orderStatus != value) { _orderStatus = value ?? string.Empty; OnPropertyChanged(); } }
        }

        public decimal TotalPrice
        {
            get => _totalPrice;
            set { if (_totalPrice != value) { _totalPrice = value; OnPropertyChanged(); } }
        }

        public string PaymentStatus
        {
            get => _paymentStatus;
            set { if (_paymentStatus != value) { _paymentStatus = value ?? string.Empty; OnPropertyChanged(); } }
        }
        public int? SupplierId
        {
            get => _supplierId;
            set { if (_supplierId != value) { _supplierId = value; OnPropertyChanged(); } }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
