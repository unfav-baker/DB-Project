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
    public partial class View_Customer : Page, INotifyPropertyChanged
    {
        public ObservableCollection<CustomerDisplayData> CustomersList { get; set; }
        public ObservableCollection<ReportDisplayData> SelectedCustomerOrders { get; set; }

        private CustomerDisplayData? _selectedCustomer;
        public CustomerDisplayData? SelectedCustomer
        {
            get => _selectedCustomer;
            set
            {
                if (_selectedCustomer != value)
                {
                    if (_selectedCustomer is not null)
                        _selectedCustomer.IsSelectedAsCurrent = false;

                    _selectedCustomer = value;

                    if (_selectedCustomer is not null)
                        _selectedCustomer.IsSelectedAsCurrent = true;

                    OnPropertyChanged();
                    LoadOrdersForSelectedCustomer();
                }
            }
        }

        private readonly string? connectionString;
        private const string DbConnectionStringEnvVar = "PRIMETECH_DB_CONN_STRING";

        public View_Customer()
        {
            InitializeComponent();
            CustomersList = new();
            SelectedCustomerOrders = new();
            this.DataContext = this;

            connectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVar);

            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show($"Database connection string not configured. Please check application setup.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                LoadCustomersFromDatabase();
            }
        }

        private void LoadCustomersFromDatabase()
        {
            if (string.IsNullOrEmpty(connectionString)) return;

            CustomersList.Clear();
            try
            {
                using MySqlConnection connection = new(connectionString);
                connection.Open();
                string query = "SELECT Customer_ID, Name, Phone_Number, Email, Address, Status, Customer_Type, Business_Name FROM customer WHERE Status = 'Active' ORDER BY Name;";
                using MySqlCommand command = new(query, connection);
                using MySqlDataReader reader = command.ExecuteReader();

                // Get ordinals once before the loop
                int custIdOrdinal = reader.GetOrdinal("Customer_ID");
                int nameOrdinal = reader.GetOrdinal("Name");
                int phoneOrdinal = reader.GetOrdinal("Phone_Number");
                int emailOrdinal = reader.GetOrdinal("Email");
                int addressOrdinal = reader.GetOrdinal("Address");
                int statusOrdinal = reader.GetOrdinal("Status");
                int custTypeOrdinal = reader.GetOrdinal("Customer_Type");
                int bizNameOrdinal = reader.GetOrdinal("Business_Name");

                while (reader.Read())
                {
                    CustomersList.Add(new()
                    {
                        CustomerId = reader.GetInt32(custIdOrdinal),
                        Name = reader.IsDBNull(nameOrdinal) ? "N/A" : reader.GetString(nameOrdinal),
                        PhoneNumber = reader.IsDBNull(phoneOrdinal) ? "N/A" : reader.GetString(phoneOrdinal),
                        Email = reader.IsDBNull(emailOrdinal) ? "N/A" : reader.GetString(emailOrdinal),
                        Address = reader.IsDBNull(addressOrdinal) ? "N/A" : reader.GetString(addressOrdinal),
                        Status = reader.IsDBNull(statusOrdinal) ? "N/A" : reader.GetString(statusOrdinal),
                        CustomerType = reader.IsDBNull(custTypeOrdinal) ? "N/A" : reader.GetString(custTypeOrdinal),
                        BusinessName = reader.IsDBNull(bizNameOrdinal) ? string.Empty : reader.GetString(bizNameOrdinal)
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading customers: {ex}");
                MessageBox.Show($"Error loading customers: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadOrdersForSelectedCustomer()
        {
            SelectedCustomerOrders.Clear();
            if (SelectedCustomer is null || string.IsNullOrEmpty(connectionString))
            {
                return;
            }

            Debug.WriteLine($"Loading orders for Customer ID: {SelectedCustomer.CustomerId} ({SelectedCustomer.Name})");
            try
            {
                using MySqlConnection connection = new(connectionString);
                connection.Open();
                string query = @"
                    SELECT 
                        o.Order_ID, o.Order_Date, o.Export_Through, o.Plant, o.Importer, 
                        o.Phyto_Number, o.Carton, o.Weight, o.Rate, 
                        o.Total_Amount, o.Amount_Received
                    FROM orders o
                    WHERE o.FK_Customer_ID = @CustomerId
                    ORDER BY o.Order_Date DESC, o.Order_ID DESC;";

                using MySqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@CustomerId", SelectedCustomer.CustomerId);
                using MySqlDataReader reader = command.ExecuteReader();

                // Get ordinals once before the loop
                int orderIdOrdinal = reader.GetOrdinal("Order_ID");
                int orderDateOrdinal = reader.GetOrdinal("Order_Date");
                int exportThroughOrdinal = reader.GetOrdinal("Export_Through");
                int plantOrdinal = reader.GetOrdinal("Plant");
                int importerOrdinal = reader.GetOrdinal("Importer");
                int phytoNumOrdinal = reader.GetOrdinal("Phyto_Number");
                int cartonOrdinal = reader.GetOrdinal("Carton");
                int weightOrdinal = reader.GetOrdinal("Weight");
                int rateOrdinal = reader.GetOrdinal("Rate");
                int totalAmountOrdinal = reader.GetOrdinal("Total_Amount");
                int amountReceivedOrdinal = reader.GetOrdinal("Amount_Received");

                while (reader.Read())
                {
                    decimal totalAmount = reader.IsDBNull(totalAmountOrdinal) ? 0m : reader.GetDecimal(totalAmountOrdinal);
                    decimal amountReceived = reader.IsDBNull(amountReceivedOrdinal) ? 0m : reader.GetDecimal(amountReceivedOrdinal);

                    SelectedCustomerOrders.Add(new()
                    {
                        OrderId = reader.GetInt32(orderIdOrdinal),
                        OrderDate = reader.IsDBNull(orderDateOrdinal) ? DateTime.MinValue : reader.GetDateTime(orderDateOrdinal),
                        PartyName = SelectedCustomer.Name, // Use the already selected customer's name
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
                Debug.WriteLine($"Loaded {SelectedCustomerOrders.Count} orders for customer {SelectedCustomer.Name}.");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading orders for customer ID {SelectedCustomer.CustomerId}: {ex}");
                MessageBox.Show($"Error loading orders: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CustomerCard_Click(object sender, RoutedEventArgs e)
        {
            if (sender is FrameworkElement { DataContext: CustomerDisplayData customer })
            {
                SelectedCustomer = customer;
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
