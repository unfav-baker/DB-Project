using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Employee // Or your appropriate namespace
{
    public class CustomerSelectionItem : INotifyPropertyChanged
    {
        private int _customerId;
        public int CustomerId { get => _customerId; set { _customerId = value; OnPropertyChanged(); } }

        private string _name = string.Empty;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class ProductSelectionItem : INotifyPropertyChanged
    {
        private int _productId;
        public int ProductId { get => _productId; set { _productId = value; OnPropertyChanged(); } }

        private string _name = string.Empty;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }

        private decimal _price;
        public decimal Price { get => _price; set { _price = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class SupplierSelectionItem : INotifyPropertyChanged
    {
        private int _supplierId;
        public int SupplierId { get => _supplierId; set { _supplierId = value; OnPropertyChanged(); } }

        private string _name = string.Empty;
        public string Name { get => _name; set { _name = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
