using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace Adminn
{
    public partial class Products : Page
    {
        // Property name matches the binding in XAML (lowercase 'products')
        public ObservableCollection<ProductData> products { get; set; }

        // Replace this with your actual MySQL connection string
        private readonly string connectionString = "Server=127.0.0.1;Port=3306;Database=prime_tech;Uid=root;Pwd=Abubaker85@@;";

        public Products()
        {
            InitializeComponent();
            products = new ObservableCollection<ProductData>();

            // Load product data from database
            LoadProductDataFromDatabase();

            // Set the DataContext for binding
            DataContext = this;
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to AddProduct page within the parent frame
            AddProduct addProductPage = new AddProduct();
            // Find the main window's frame and navigate
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.MainContentFrame.Navigate(addProductPage);
            }
        }

        private void ViewProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is ProductData selectedProduct)
            {
                MessageBox.Show($"Product Details:\nID: {selectedProduct.ProductId}\nSupplier ID: {selectedProduct.SupplierId}\nCategory: {selectedProduct.Category}\nQuantity: {selectedProduct.Quantity}\nPrice: {selectedProduct.Price:C}\nBatch No: {selectedProduct.BatchNo}\nStatus: {selectedProduct.Status}",
                    "Product Details", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select a product to view.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void EditProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is ProductData selectedProduct)
            {
                // Here you can navigate to an edit page or open an edit dialog
                MessageBox.Show($"Edit functionality for Product ID: {selectedProduct.ProductId}", "Edit Product", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select a product to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is ProductData selectedProduct)
            {
                var result = MessageBox.Show($"Are you sure you want to delete product ID: {selectedProduct.ProductId}?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteProductFromDatabase(selectedProduct.ProductId);
                }
            }
            else
            {
                MessageBox.Show("Please select a product to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CreateReport_Click(object sender, RoutedEventArgs e)
        {
            // Generate a simple report showing product statistics
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = @"SELECT
                               COUNT(*) as TotalProducts,
                               SUM(Quantity) as TotalQuantity,
                               AVG(Price) as AveragePrice,
                               COUNT(CASE WHEN Status = 'In Stock' THEN 1 END) as InStockCount,
                               COUNT(CASE WHEN Status = 'Out of Stock' THEN 1 END) as OutOfStockCount,
                               COUNT(CASE WHEN Status = 'Low Stock' THEN 1 END) as LowStockCount
                               FROM products";

                using var command = new MySqlCommand(query, connection);
                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    string report = $"📊 PRODUCTS REPORT\n" +
                                  $"==================\n\n" +
                                  $"Total Products: {reader.GetInt32("TotalProducts")}\n" +
                                  $"Total Quantity: {reader.GetInt32("TotalQuantity")}\n" +
                                  $"Average Price: {reader.GetDecimal("AveragePrice"):C}\n\n" +
                                  $"STOCK STATUS:\n" +
                                  $"✅ In Stock: {reader.GetInt32("InStockCount")}\n" +
                                  $"❌ Out of Stock: {reader.GetInt32("OutOfStockCount")}\n" +
                                  $"⚠️ Low Stock: {reader.GetInt32("LowStockCount")}\n\n" +
                                  $"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}";

                    MessageBox.Show(report, "Products Report", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Report Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProductDataFromDatabase()
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = @"SELECT Product_ID, Supplier_ID, Category, Quantity, Price, Batch_No, Status, Created_At
                               FROM products ORDER BY Product_ID";

                using var command = new MySqlCommand(query, connection);
                using var reader = command.ExecuteReader();

                products.Clear();

                while (reader.Read())
                {
                    var product = new ProductData
                    {
                        ProductId = reader.GetInt32("Product_ID"),
                        SupplierId = reader.IsDBNull("Supplier_ID") ? 0 : reader.GetInt32("Supplier_ID"),
                        Category = reader.IsDBNull("Category") ? string.Empty : reader.GetString("Category"),
                        Quantity = reader.IsDBNull("Quantity") ? 0 : reader.GetInt32("Quantity"),
                        Price = reader.IsDBNull("Price") ? 0 : reader.GetDecimal("Price"),
                        BatchNo = reader.IsDBNull("Batch_No") ? string.Empty : reader.GetString("Batch_No"),
                        Status = reader.IsDBNull("Status") ? string.Empty : reader.GetString("Status"),
                        CreatedAt = reader.IsDBNull("Created_At") ? DateTime.Now : reader.GetDateTime("Created_At")
                    };

                    products.Add(product);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading product data: {ex.Message}", "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteProductFromDatabase(int productId)
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = "DELETE FROM products WHERE Product_ID = @ProductId";

                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@ProductId", productId);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Product deleted successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Refresh the data
                    LoadProductDataFromDatabase();
                }
                else
                {
                    MessageBox.Show("Product not found or could not be deleted.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting product: {ex.Message}", "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void RefreshData()
        {
            LoadProductDataFromDatabase();
        }
    }

    public class ProductData : INotifyPropertyChanged // Added INotifyPropertyChanged interface here
    {
        private bool _isSelected;
        private int _productId;
        private int _supplierId;
        private string _category = string.Empty;
        private int _quantity;
        private decimal _price;
        private string _batchNo = string.Empty;
        private string _status = string.Empty;
        private DateTime _createdAt;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public int ProductId
        {
            get => _productId;
            set
            {
                _productId = value;
                OnPropertyChanged();
            }
        }

        public int SupplierId
        {
            get => _supplierId;
            set
            {
                _supplierId = value;
                OnPropertyChanged();
            }
        }

        public string Category
        {
            get => _category;
            set
            {
                _category = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public int Quantity
        {
            get => _quantity;
            set
            {
                _quantity = value;
                OnPropertyChanged();
            }
        }

        public decimal Price
        {
            get => _price;
            set
            {
                _price = value;
                OnPropertyChanged();
            }
        }

        public string BatchNo
        {
            get => _batchNo;
            set
            {
                _batchNo = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                _status = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set
            {
                _createdAt = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}