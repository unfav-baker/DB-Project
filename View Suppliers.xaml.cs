using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Linq;

namespace Employee // Your namespace
{
    public partial class View_Suppliers : Page, INotifyPropertyChanged
    {
        public ObservableCollection<SupplierDisplayData> SuppliersList { get; set; }
        public ObservableCollection<ReportDisplayData> SelectedSupplierOrders { get; set; }

        private SupplierDisplayData? _selectedSupplier;
        public SupplierDisplayData? SelectedSupplier
        {
            get => _selectedSupplier;
            set
            {
                if (_selectedSupplier != value)
                {
                    if (_selectedSupplier is not null) // Use pattern matching
                        _selectedSupplier.IsSelectedAsCurrent = false;

                    _selectedSupplier = value;

                    if (_selectedSupplier is not null) // Use pattern matching
                        _selectedSupplier.IsSelectedAsCurrent = true;

                    OnPropertyChanged();
                    LoadOrdersForSelectedSupplier();
                }
            }
        }

        private readonly string? connectionString;
        private const string DbConnectionStringEnvVar = "PRIMETECH_DB_CONN_STRING";

        public View_Suppliers()
        {
            InitializeComponent();
            SuppliersList = new(); // Collection initialization simplified
            SelectedSupplierOrders = new(); // Collection initialization simplified
            this.DataContext = this;

            connectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVar);

            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show($"Database connection string not configured. Please check application setup.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                LoadSuppliersFromDatabase();
            }
        }

        private void LoadSuppliersFromDatabase()
        {
            if (string.IsNullOrEmpty(connectionString)) return;

            SuppliersList.Clear();
            try
            {
                using MySqlConnection connection = new(connectionString); // 'new' expression simplified
                connection.Open();
                string query = "SELECT Supplier_ID, Name, Phone_Number, Email, Plant_Name, Status FROM supplier WHERE Status = 'Active' ORDER BY Name;";
                using MySqlCommand command = new(query, connection); // 'new' expression simplified
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    // Using GetOrdinal for robustness with IsDBNull and GetString/GetInt32
                    int supplierIdOrdinal = reader.GetOrdinal("Supplier_ID");
                    int nameOrdinal = reader.GetOrdinal("Name");
                    int phoneOrdinal = reader.GetOrdinal("Phone_Number");
                    int emailOrdinal = reader.GetOrdinal("Email");
                    int plantNameOrdinal = reader.GetOrdinal("Plant_Name");
                    int statusOrdinal = reader.GetOrdinal("Status");

                    SuppliersList.Add(new SupplierDisplayData // 'new' expression simplified
                    {
                        SupplierId = reader.GetInt32(supplierIdOrdinal),
                        Name = reader.IsDBNull(nameOrdinal) ? "N/A" : reader.GetString(nameOrdinal),
                        PhoneNumber = reader.IsDBNull(phoneOrdinal) ? "N/A" : reader.GetString(phoneOrdinal),
                        Email = reader.IsDBNull(emailOrdinal) ? "N/A" : reader.GetString(emailOrdinal),
                        PlantName = reader.IsDBNull(plantNameOrdinal) ? "N/A" : reader.GetString(plantNameOrdinal),
                        Status = reader.IsDBNull(statusOrdinal) ? "N/A" : reader.GetString(statusOrdinal)
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading suppliers: {ex}"); // Use ex directly for Debug.WriteLine
                MessageBox.Show($"Error loading suppliers: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadOrdersForSelectedSupplier()
        {
            SelectedSupplierOrders.Clear();
            if (SelectedSupplier is null || string.IsNullOrEmpty(connectionString)) // Use pattern matching
            {
                return;
            }

            Debug.WriteLine($"Loading orders for Supplier ID: {SelectedSupplier.SupplierId}");
            try
            {
                using MySqlConnection connection = new(connectionString); // 'new' expression simplified
                connection.Open();
                string query = @"
                    SELECT 
                        o.Order_ID, o.Order_Date, c.Name AS PartyName, o.Export_Through,
                        o.Plant, o.Importer, o.Phyto_Number, o.Carton, o.Weight,
                        o.Rate, o.Total_Amount, o.Amount_Received
                    FROM orders o
                    INNER JOIN customer c ON o.FK_Customer_ID = c.Customer_ID
                    WHERE o.FK_Supplier_ID = @SupplierId
                    ORDER BY o.Order_Date DESC, o.Order_ID DESC;";

                using MySqlCommand command = new(query, connection); // 'new' expression simplified
                command.Parameters.AddWithValue("@SupplierId", SelectedSupplier.SupplierId);
                using MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    // Using GetOrdinal for robustness
                    int orderIdOrdinal = reader.GetOrdinal("Order_ID");
                    int orderDateOrdinal = reader.GetOrdinal("Order_Date");
                    int partyNameOrdinal = reader.GetOrdinal("PartyName");
                    int exportThroughOrdinal = reader.GetOrdinal("Export_Through");
                    int plantOrdinal = reader.GetOrdinal("Plant");
                    int importerOrdinal = reader.GetOrdinal("Importer");
                    int phytoNumOrdinal = reader.GetOrdinal("Phyto_Number");
                    int cartonOrdinal = reader.GetOrdinal("Carton");
                    int weightOrdinal = reader.GetOrdinal("Weight");
                    int rateOrdinal = reader.GetOrdinal("Rate");
                    int totalAmountOrdinal = reader.GetOrdinal("Total_Amount");
                    int amountReceivedOrdinal = reader.GetOrdinal("Amount_Received");

                    decimal totalAmount = reader.IsDBNull(totalAmountOrdinal) ? 0m : reader.GetDecimal(totalAmountOrdinal);
                    decimal amountReceived = reader.IsDBNull(amountReceivedOrdinal) ? 0m : reader.GetDecimal(amountReceivedOrdinal);

                    SelectedSupplierOrders.Add(new ReportDisplayData // 'new' expression simplified
                    {
                        OrderId = reader.GetInt32(orderIdOrdinal),
                        OrderDate = reader.IsDBNull(orderDateOrdinal) ? DateTime.MinValue : reader.GetDateTime(orderDateOrdinal),
                        PartyName = reader.IsDBNull(partyNameOrdinal) ? "N/A" : reader.GetString(partyNameOrdinal),
                        ExportThrough = reader.IsDBNull(exportThroughOrdinal) ? string.Empty : reader.GetString(exportThroughOrdinal),
                        Plant = reader.IsDBNull(plantOrdinal) ? string.Empty : reader.GetString(plantOrdinal),
                        Importer = reader.IsDBNull(importerOrdinal) ? string.Empty : reader.GetString(importerOrdinal),
                        PhytoNumber = reader.IsDBNull(phytoNumOrdinal) ? string.Empty : reader.GetString(phytoNumOrdinal),
                        Carton = reader.IsDBNull(cartonOrdinal) ? null : reader.GetInt32(cartonOrdinal),
                        Weight = reader.IsDBNull(weightOrdinal) ? null : reader.GetDecimal(weightOrdinal),
                        Rate = reader.IsDBNull(rateOrdinal) ? null : reader.GetDecimal(rateOrdinal),
                        TotalAmount = totalAmount,
                        AmountReceived = amountReceived,
                        Balance = totalAmount - amountReceived
                    });
                }
                Debug.WriteLine($"Loaded {SelectedSupplierOrders.Count} orders for supplier {SelectedSupplier.Name}.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading orders for supplier ID {SelectedSupplier.SupplierId}: {ex}"); // Use ex directly
                MessageBox.Show($"Error loading orders: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SupplierCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement { DataContext: SupplierDisplayData supplier }) // Use pattern matching
            {
                SelectedSupplier = supplier;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
