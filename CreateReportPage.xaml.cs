using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Input;
using System.Linq;

namespace Employee
{
    public partial class CreateReportPage : Page
    {
        public NewOrderEntryData OrderEntry { get; set; }
        public ObservableCollection<CustomerSelectionItem> AvailableCustomers { get; set; }
        public ObservableCollection<ProductSelectionItem> AvailableProducts { get; set; }
        public ObservableCollection<SupplierSelectionItem> AvailableSuppliers { get; set; }

        private readonly string? connectionString;
        private const string DbConnectionStringEnvVar = "PRIMETECH_DB_CONN_STRING";

        public CreateReportPage()
        {
            InitializeComponent();
            OrderEntry = new NewOrderEntryData();
            AvailableCustomers = new ObservableCollection<CustomerSelectionItem>();
            AvailableProducts = new ObservableCollection<ProductSelectionItem>();
            AvailableSuppliers = new ObservableCollection<SupplierSelectionItem>();

            this.DataContext = OrderEntry;
            CustomerComboBox.ItemsSource = AvailableCustomers;
            ProductComboBox.ItemsSource = AvailableProducts;
            SupplierComboBox.ItemsSource = AvailableSuppliers;

            connectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVar);

            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Database connection string not configured. Cannot save new entries.", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (this.FindName("SaveOrderButton") is Button saveBtn)
                {
                    saveBtn.IsEnabled = false;
                }
            }
            else
            {
                LoadComboBoxData();
            }
        }

        private void LoadComboBoxData()
        {
            LoadCustomers();
            LoadProducts();
            LoadSuppliers();
        }

        private void LoadCustomers()
        {
            if (string.IsNullOrEmpty(connectionString)) return;
            AvailableCustomers.Clear();
            try
            {
                using MySqlConnection connection = new(connectionString);
                connection.Open();
                string query = "SELECT Customer_ID, Name FROM customer WHERE Status = 'Active' ORDER BY Name;";
                using MySqlCommand command = new(query, connection);
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    AvailableCustomers.Add(new CustomerSelectionItem
                    {
                        CustomerId = reader.GetInt32("Customer_ID"),
                        Name = reader.GetString("Name")
                    });
                }
            }
            catch (Exception ex) { Debug.WriteLine($"Error loading customers: {ex.Message}"); }
        }

        private void LoadProducts()
        {
            if (string.IsNullOrEmpty(connectionString)) return;
            AvailableProducts.Clear();
            try
            {
                using MySqlConnection connection = new(connectionString);
                connection.Open();
                string query = "SELECT Product_ID, Category, Price FROM product WHERE Status = 'In Stock' ORDER BY Category;";
                using MySqlCommand command = new(query, connection);
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    AvailableProducts.Add(new ProductSelectionItem
                    {
                        ProductId = reader.GetInt32("Product_ID"),
                        Name = reader.GetString("Category"),
                        Price = reader.GetDecimal("Price")
                    });
                }
            }
            catch (Exception ex) { Debug.WriteLine($"Error loading products: {ex.Message}"); }
        }

        // ProductComboBox_SelectionChanged is implicitly handled by binding SelectedItem to OrderEntry.CurrentSelectedProduct
        // The ViewModel (NewOrderEntryData) handles updating the Rate when CurrentSelectedProduct changes.

        private void LoadSuppliers()
        {
            if (string.IsNullOrEmpty(connectionString)) return;
            AvailableSuppliers.Clear();
            try
            {
                using MySqlConnection connection = new(connectionString);
                connection.Open();
                string query = "SELECT Supplier_ID, Name FROM supplier WHERE Status = 'Active' ORDER BY Name;";
                using MySqlCommand command = new(query, connection);
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    AvailableSuppliers.Add(new SupplierSelectionItem
                    {
                        SupplierId = reader.GetInt32("Supplier_ID"),
                        Name = reader.GetString("Name")
                    });
                }
            }
            catch (Exception ex) { Debug.WriteLine($"Error loading suppliers: {ex.Message}"); }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Database connection is not configured.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!OrderEntry.SelectedCustomerId.HasValue) { MessageBox.Show("Please select a Customer.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning); CustomerComboBox.Focus(); return; }
            if (!OrderEntry.SelectedProductId.HasValue) { MessageBox.Show("Please select a Product.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning); ProductComboBox.Focus(); return; }
            if (OrderEntry.Quantity <= 0) { MessageBox.Show("Quantity must be greater than 0.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            OrderEntry.CalculateTotalAmount();
            if (OrderEntry.Rate <= 0 || OrderEntry.TotalAmount <= 0) { MessageBox.Show("Rate and Total Amount must be valid and greater than 0.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning); return; }

            try
            {
                using MySqlConnection connection = new(connectionString);
                connection.Open();

                // Removed Importer and Batch_No from INSERT as per your requirements
                string query = @"INSERT INTO orders 
                                (FK_Customer_ID, FK_Product_ID, FK_Supplier_ID, Order_Date, Delivery_Date, 
                                 Quantity, Carton, Weight, Rate, Total_Amount, Amount_Received, 
                                 Order_Status, Payment_Status, Phyto_Number, Export_Through, Plant,  
                                 Vehicle_No, Created_At, Updated_At)
                                VALUES 
                                (@CustomerId, @ProductId, @SupplierId, @OrderDate, @DeliveryDate,
                                 @Quantity, @Carton, @Weight, @Rate, @TotalAmount, @AmountReceived,
                                 @OrderStatus, @PaymentStatus, @PhytoNumber, @ExportThrough, @Plant, 
                                 @VehicleNo, @CreatedAt, @UpdatedAt);
                                SELECT LAST_INSERT_ID();";

                using MySqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@CustomerId", OrderEntry.SelectedCustomerId.Value);
                command.Parameters.AddWithValue("@ProductId", OrderEntry.SelectedProductId.Value);
                command.Parameters.AddWithValue("@SupplierId", OrderEntry.SelectedSupplierId as object ?? DBNull.Value);
                command.Parameters.AddWithValue("@OrderDate", OrderEntry.OrderDate);
                command.Parameters.AddWithValue("@DeliveryDate", OrderEntry.DeliveryDate as object ?? DBNull.Value);
                command.Parameters.AddWithValue("@Quantity", OrderEntry.Quantity);
                command.Parameters.AddWithValue("@Carton", OrderEntry.Carton as object ?? DBNull.Value);
                command.Parameters.AddWithValue("@Weight", OrderEntry.Weight as object ?? DBNull.Value);
                command.Parameters.AddWithValue("@Rate", OrderEntry.Rate);
                command.Parameters.AddWithValue("@TotalAmount", OrderEntry.TotalAmount);
                command.Parameters.AddWithValue("@AmountReceived", OrderEntry.AmountReceived as object ?? DBNull.Value);
                command.Parameters.AddWithValue("@OrderStatus", OrderEntry.OrderStatus);
                command.Parameters.AddWithValue("@PaymentStatus", OrderEntry.PaymentStatus);
                command.Parameters.AddWithValue("@PhytoNumber", string.IsNullOrWhiteSpace(OrderEntry.PhytoNumber) ? DBNull.Value : (object)OrderEntry.PhytoNumber);
                command.Parameters.AddWithValue("@ExportThrough", string.IsNullOrWhiteSpace(OrderEntry.ExportThrough) ? DBNull.Value : (object)OrderEntry.ExportThrough);
                command.Parameters.AddWithValue("@Plant", string.IsNullOrWhiteSpace(OrderEntry.Plant) ? DBNull.Value : (object)OrderEntry.Plant);
                // Importer is not set from form
                // BatchNo is not set from form (PhytoNumber is used)
                command.Parameters.AddWithValue("@VehicleNo", string.IsNullOrWhiteSpace(OrderEntry.VehicleNo) ? DBNull.Value : (object)OrderEntry.VehicleNo);
                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                object? result = command.ExecuteScalar();
                int newOrderId = 0;
                if (result is not (null or DBNull)) newOrderId = Convert.ToInt32(result);

                if (newOrderId > 0)
                {
                    MessageBox.Show($"New order entry (ID: {newOrderId}) saved successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigateBackToReportsList(true);
                }
                else
                {
                    MessageBox.Show("Failed to save new order entry or retrieve its ID. The insert might have succeeded without returning an ID.", "Save Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    NavigateBackToReportsList(true);
                }
            }
            catch (MySqlException myEx)
            {
                Debug.WriteLine($"MySQL Error saving order entry: {myEx.ToString()}");
                MessageBox.Show($"Database Error (MySQL): {myEx.Message} (Code: {myEx.Number})", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Generic error saving order entry: {ex.ToString()}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Renamed from CancelButton_Click to ClearFormButton_Click to match XAML
        private void ClearFormButton_Click(object sender, RoutedEventArgs e)
        {
            OrderEntry.Clear(); // Call Clear method on ViewModel
            // Optionally reset ComboBox selections if not covered by ViewModel Clear
            CustomerComboBox.SelectedIndex = -1;
            ProductComboBox.SelectedIndex = -1;
            SupplierComboBox.SelectedIndex = -1;
            // Set focus to the first field
            CustomerComboBox.Focus();
            MessageBox.Show("Form has been cleared.", "Form Cleared", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // New event handler for the top-right close button
        private void ClosePageButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateBackToReportsList(false); // Navigate back without forcing a refresh from here
        }


        private void NavigateBackToReportsList(bool refreshNeeded)
        {
            Generate_Reports reportsPage = new Generate_Reports();

            if (Application.Current.MainWindow is MainWindow mainWindow && mainWindow.MainContentFrame != null)
            {
                mainWindow.MainContentFrame.Navigate(reportsPage);
            }
            else if (this.NavigationService != null)
            {
                this.NavigationService.Navigate(reportsPage);
            }
            else
            {
                Debug.WriteLine("NavigateBackToReportsList: Could not determine how to navigate back. Main frame or NavigationService not found.");
            }
        }

        private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DecimalOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                string currentText = textBox.Text;
                int caretIndex = textBox.CaretIndex;
                string newText = currentText.Insert(caretIndex, e.Text);

                if (!Regex.IsMatch(newText, @"^[0-9]*(\.[0-9]{0,2})?$") && newText != "." && !string.IsNullOrEmpty(newText))
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
