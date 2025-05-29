using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace Adminn
{
    public partial class Products : Page
    {
        public ObservableCollection<ProductData> ProductList { get; set; }

        private readonly string? connectionString;
        private const string DbConnectionStringEnvVar = "PRIMETECH_DB_CONN_STRING";

        public Products()
        {
            InitializeComponent();
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Products.xaml.cs - Constructor started.");
            connectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVar);
            ProductList = new ObservableCollection<ProductData>();
            DataContext = this;

            if (string.IsNullOrEmpty(connectionString))
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Products.xaml.cs - Connection string IS NULL or EMPTY. Env Var: '{DbConnectionStringEnvVar}'");
                MessageBox.Show($"Database connection string environment variable '{DbConnectionStringEnvVar}' was not found or is empty. " +
                                $"Please ensure your .env file is correctly set up and loaded at application startup (App.xaml.cs).\n\n" +
                                "Product data cannot be loaded.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                string connStringSnippet = connectionString.Length > 30
                    ? connectionString.Substring(0, 30) + "..."
                    : connectionString;
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Products.xaml.cs - Connection string FOUND (snippet): {connStringSnippet}");
                LoadProductDataFromDatabase();
            }
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Products.xaml.cs - Constructor finished.");
        }

        private void LoadProductDataFromDatabase()
        {
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Products.xaml.cs - LoadProductDataFromDatabase() called.");
            if (string.IsNullOrEmpty(this.connectionString))
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Products.xaml.cs - LoadProductDataFromDatabase: Connection string is missing. Data not loaded.");
                return;
            }

            try
            {
                using MySqlConnection connection = new(this.connectionString);
                connection.Open();
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Products.xaml.cs - Database connection opened successfully.");

                // CORRECTED: Table name changed to 'Product' (singular)
                string query = @"SELECT Product_ID, FK_Supplier_ID, Category, Description, Quantity, Price, Batch_No, Status, Weight, Created_At, Updated_At
                                 FROM Product ORDER BY Product_ID";
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Products.xaml.cs - Executing query: {query}");

                using MySqlCommand command = new(query, connection);
                using MySqlDataReader reader = command.ExecuteReader();

                ProductList.Clear();
                int itemsLoaded = 0;
                while (reader.Read())
                {
                    var product = new ProductData
                    {
                        ProductId = reader.GetInt32(reader.GetOrdinal("Product_ID")),
                        SupplierId = reader.IsDBNull(reader.GetOrdinal("FK_Supplier_ID")) ? 0 : reader.GetInt32(reader.GetOrdinal("FK_Supplier_ID")),
                        Category = reader.IsDBNull(reader.GetOrdinal("Category")) ? string.Empty : reader.GetString(reader.GetOrdinal("Category")),
                        Description = reader.IsDBNull(reader.GetOrdinal("Description")) ? string.Empty : reader.GetString(reader.GetOrdinal("Description")),
                        Quantity = reader.IsDBNull(reader.GetOrdinal("Quantity")) ? 0 : reader.GetInt32(reader.GetOrdinal("Quantity")),
                        Price = reader.IsDBNull(reader.GetOrdinal("Price")) ? 0m : reader.GetDecimal(reader.GetOrdinal("Price")),
                        BatchNo = reader.IsDBNull(reader.GetOrdinal("Batch_No")) ? string.Empty : reader.GetString(reader.GetOrdinal("Batch_No")),
                        Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? string.Empty : reader.GetString(reader.GetOrdinal("Status")),
                        Weight = reader.IsDBNull(reader.GetOrdinal("Weight")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Weight")),
                        CreatedAt = reader.IsDBNull(reader.GetOrdinal("Created_At")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("Created_At")),
                        UpdatedAt = reader.IsDBNull(reader.GetOrdinal("Updated_At")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("Updated_At"))
                    };
                    ProductList.Add(product);
                    itemsLoaded++;
                }
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Products.xaml.cs - Finished loading data. Total products attempted to load: {itemsLoaded}. ProductList.Count now: {ProductList.Count}");

                if (itemsLoaded == 0 && ProductList.Count == 0)
                {
                    Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Products.xaml.cs - No products were found in the database OR no products were added to the list after reading.");
                }
            }
            catch (MySqlException myEx)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] MySQL Error loading product data: {myEx.ToString()}");
                MessageBox.Show($"MySQL Database Error: {myEx.Message}\n(Number: {myEx.Number})\n\nPlease check the database connection and SQL query (table/column names).", "Database Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Generic Error loading product data: {ex.ToString()}");
                MessageBox.Show($"An unexpected error occurred while loading product data: {ex.Message}", "Application Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddProduct_Click(object sender, RoutedEventArgs e)
        {
            AddProduct addProductPage = new();
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.MainContentFrame.Navigate(addProductPage);
            }
        }

        private void ViewProduct_Click(object sender, RoutedEventArgs e)
        {
            if (ProductsDataGrid.SelectedItem is ProductData selectedProduct)
            {
                string details = $"""
                                  Product Details:

                                  ID: {selectedProduct.ProductId}
                                  Supplier ID: {(selectedProduct.SupplierId == 0 ? "N/A" : selectedProduct.SupplierId.ToString())}
                                  Category: {selectedProduct.Category}
                                  Description: {selectedProduct.Description}
                                  Quantity: {selectedProduct.Quantity}
                                  Price: {selectedProduct.Price:C}
                                  Batch No: {selectedProduct.BatchNo}
                                  Weight: {selectedProduct.Weight?.ToString("N2") ?? "N/A"} 
                                  Status: {selectedProduct.Status}
                                  Created At: {selectedProduct.CreatedAt:dd/MM/yyyy HH:mm}
                                  Updated At: {selectedProduct.UpdatedAt:dd/MM/yyyy HH:mm}
                                  """;
                MessageBox.Show(details, "Product Details", MessageBoxButton.OK, MessageBoxImage.Information);
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
                var result = MessageBox.Show($"Are you sure you want to delete product ID: {selectedProduct.ProductId} (Category: {selectedProduct.Category})?",
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
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Database connection is not configured. Cannot generate report.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            try
            {
                using MySqlConnection connection = new(connectionString);
                connection.Open();
                // CORRECTED: Table name changed to 'Product' (singular)
                string query = @"SELECT
                                 COUNT(*) as TotalProducts,
                                 SUM(Quantity) as TotalQuantity,
                                 AVG(Price) as AveragePrice,
                                 SUM(CASE WHEN Status = 'In Stock' THEN 1 ELSE 0 END) as InStockCount,
                                 SUM(CASE WHEN Status = 'Out of Stock' THEN 1 ELSE 0 END) as OutOfStockCount,
                                 SUM(CASE WHEN Status = 'Low Stock' THEN 1 ELSE 0 END) as LowStockCount
                                 FROM Product"; // Using singular 'Product'

                using MySqlCommand command = new(query, connection);
                using MySqlDataReader reader = command.ExecuteReader();

                if (reader.Read())
                {
                    int totalProducts = reader.GetInt32("TotalProducts");
                    long totalQuantity = reader.IsDBNull("TotalQuantity") ? 0L : reader.GetInt64("TotalQuantity");
                    decimal averagePrice = reader.IsDBNull("AveragePrice") ? 0m : reader.GetDecimal("AveragePrice");
                    int inStockCount = reader.GetInt32("InStockCount");
                    int outOfStockCount = reader.GetInt32("OutOfStockCount");
                    int lowStockCount = reader.GetInt32("LowStockCount");

                    string report = $"""
                                     📊 PRODUCTS REPORT
                                     ==================

                                     Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}

                                     Total Products:   {totalProducts}
                                     Total Quantity:   {totalQuantity}
                                     Average Price:    {averagePrice:C}

                                     STOCK STATUS:
                                       ✅ In Stock:     {inStockCount}
                                       ❌ Out of Stock: {outOfStockCount}
                                       ⚠️ Low Stock:    {lowStockCount}
                                     """;
                    MessageBox.Show(report, "Products Report", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("No data available to generate the report.", "Report Error", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Error generating product report: {ex.ToString()}");
                MessageBox.Show($"Error generating report: {ex.Message}", "Report Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteProductFromDatabase(int productId)
        {
            if (string.IsNullOrEmpty(this.connectionString))
            {
                MessageBox.Show("Database connection is not configured. Cannot delete product.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                using MySqlConnection connection = new(this.connectionString);
                connection.Open();
                // CORRECTED: Table name changed to 'Product' (singular)
                string query = "DELETE FROM Product WHERE Product_ID = @ProductId";
                using MySqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@ProductId", productId);
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Product deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadProductDataFromDatabase();
                }
                else
                {
                    MessageBox.Show("Product not found or could not be deleted.", "Deletion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (MySqlException myEx)
            {
                Debug.WriteLine($"MySQL Error deleting product: {myEx.ToString()}");
                MessageBox.Show($"Database Error (MySQL): {myEx.Message} (Code: {myEx.Number})", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Generic error deleting product: {ex.ToString()}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void RefreshData()
        {
            if (!string.IsNullOrEmpty(connectionString)) LoadProductDataFromDatabase();
            else MessageBox.Show("Database connection is not configured. Cannot refresh data.", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    public class ProductData : INotifyPropertyChanged
    {
        private bool _isSelected;
        private int _productId;
        private int _supplierId;
        private string _category = string.Empty;
        private string _description = string.Empty;
        private int _quantity;
        private decimal _price;
        private string _batchNo = string.Empty;
        private decimal? _weight;
        private string _status = string.Empty;
        private DateTime _createdAt;
        private DateTime _updatedAt;

        public bool IsSelected { get => _isSelected; set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } } }
        public int ProductId { get => _productId; set { if (_productId != value) { _productId = value; OnPropertyChanged(); } } }
        public int SupplierId { get => _supplierId; set { if (_supplierId != value) { _supplierId = value; OnPropertyChanged(); } } }
        public string Category { get => _category; set { if (_category != value) { _category = value ?? string.Empty; OnPropertyChanged(); } } }
        public string Description { get => _description; set { if (_description != value) { _description = value ?? string.Empty; OnPropertyChanged(); } } }
        public int Quantity { get => _quantity; set { if (_quantity != value) { _quantity = value; OnPropertyChanged(); } } }
        public decimal Price { get => _price; set { if (_price != value) { _price = value; OnPropertyChanged(); } } }
        public string BatchNo { get => _batchNo; set { if (_batchNo != value) { _batchNo = value ?? string.Empty; OnPropertyChanged(); } } }
        public decimal? Weight { get => _weight; set { if (_weight != value) { _weight = value; OnPropertyChanged(); } } }
        public string Status { get => _status; set { if (_status != value) { _status = value ?? string.Empty; OnPropertyChanged(); } } }
        public DateTime CreatedAt { get => _createdAt; set { if (_createdAt != value) { _createdAt = value; OnPropertyChanged(); } } }
        public DateTime UpdatedAt { get => _updatedAt; set { if (_updatedAt != value) { _updatedAt = value; OnPropertyChanged(); } } }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
