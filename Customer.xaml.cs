using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Text; // For MessageBox details

namespace Adminn
{
    public partial class Customer : Page
    {
        public ObservableCollection<CustomerData> Customers { get; set; }
        private readonly string? connectionString;
        private const string DbConnectionStringEnvVar = "PRIMETECH_DB_CONN_STRING";

        public Customer()
        {
            InitializeComponent();
            Customers = new ObservableCollection<CustomerData>();
            this.DataContext = this;

            connectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVar);

            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show($"Database connection string environment variable '{DbConnectionStringEnvVar}' was not found or is empty. " +
                                $"Please ensure your .env file is correctly set up and loaded at application startup (App.xaml.cs).\n\n" +
                                "Customer data cannot be loaded.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                LoadCustomerDataFromDatabase();
            }
        }

        private void LoadCustomerDataFromDatabase()
        {
            if (string.IsNullOrEmpty(connectionString)) return;

            Customers.Clear();
            try
            {
                using MySqlConnection connection = new(connectionString);
                connection.Open();
                // Query to select data from the 'customer' table
                string query = @"SELECT Customer_ID, Name, Phone_Number, Email, Address, Customer_Type, Business_Name, Status, Created_At 
                                 FROM customer 
                                 ORDER BY Name;";

                using MySqlCommand command = new(query, connection);
                using MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    Customers.Add(new CustomerData
                    {
                        CustomerId = reader.GetInt32(reader.GetOrdinal("Customer_ID")),
                        Name = reader.IsDBNull(reader.GetOrdinal("Name")) ? string.Empty : reader.GetString(reader.GetOrdinal("Name")),
                        PhoneNumber = reader.IsDBNull(reader.GetOrdinal("Phone_Number")) ? string.Empty : reader.GetString(reader.GetOrdinal("Phone_Number")),
                        Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? string.Empty : reader.GetString(reader.GetOrdinal("Email")),
                        Address = reader.IsDBNull(reader.GetOrdinal("Address")) ? string.Empty : reader.GetString(reader.GetOrdinal("Address")),
                        CustomerType = reader.IsDBNull(reader.GetOrdinal("Customer_Type")) ? string.Empty : reader.GetString(reader.GetOrdinal("Customer_Type")),
                        BusinessName = reader.IsDBNull(reader.GetOrdinal("Business_Name")) ? string.Empty : reader.GetString(reader.GetOrdinal("Business_Name")),
                        Status = reader.IsDBNull(reader.GetOrdinal("Status")) ? string.Empty : reader.GetString(reader.GetOrdinal("Status")),
                        CreatedAt = reader.IsDBNull(reader.GetOrdinal("Created_At")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("Created_At"))
                    });
                }
            }
            catch (MySqlException myEx)
            {
                Debug.WriteLine($"MySQL Error loading customer data: {myEx.ToString()}");
                MessageBox.Show($"Database Error (MySQL): {myEx.Message} (Code: {myEx.Number})", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Generic error loading customer data: {ex.ToString()}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            AddCustomer addCustomerPage = new();
            if (Application.Current.MainWindow is MainWindow mainWindow && mainWindow.MainContentFrame != null)
            {
                mainWindow.MainContentFrame.Navigate(addCustomerPage);
            }
        }

        private void ViewCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerDataGrid.SelectedItem is CustomerData selectedCustomer)
            {
                StringBuilder details = new StringBuilder();
                details.AppendLine("Customer Details:\n");
                details.AppendLine($"ID: {selectedCustomer.CustomerId}");
                details.AppendLine($"Name: {selectedCustomer.Name}");
                details.AppendLine($"Phone: {selectedCustomer.PhoneNumber}");
                details.AppendLine($"Email: {selectedCustomer.Email}");
                details.AppendLine($"Address: {selectedCustomer.Address}");
                details.AppendLine($"Type: {selectedCustomer.CustomerType}");
                if (!string.IsNullOrEmpty(selectedCustomer.BusinessName))
                {
                    details.AppendLine($"Business Name: {selectedCustomer.BusinessName}");
                }
                details.AppendLine($"Status: {selectedCustomer.Status}");
                details.AppendLine($"Registered On: {selectedCustomer.CreatedAt:dd/MM/yyyy}");

                MessageBox.Show(details.ToString(), "Customer Details", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select a customer to view.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void EditCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerDataGrid.SelectedItem is CustomerData selectedCustomer)
            {
                // Navigate to an EditCustomerPage, passing selectedCustomer.CustomerId
                // For now, placeholder:
                MessageBox.Show($"Edit functionality for Customer ID: {selectedCustomer.CustomerId} needs to be implemented.", "Edit Customer", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select a customer to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerDataGrid.SelectedItem is CustomerData selectedCustomer)
            {
                var result = MessageBox.Show($"Are you sure you want to delete customer '{selectedCustomer.Name}' (ID: {selectedCustomer.CustomerId})?",
                                             "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteCustomerFromDatabase(selectedCustomer.CustomerId);
                }
            }
            else
            {
                MessageBox.Show("Please select a customer to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteCustomerFromDatabase(int customerId)
        {
            if (string.IsNullOrEmpty(this.connectionString))
            {
                MessageBox.Show("Database connection is not configured.", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                using MySqlConnection connection = new(this.connectionString);
                connection.Open();
                // IMPORTANT: Consider handling related orders before deleting a customer (e.g., set FK_Customer_ID to NULL or prevent deletion)
                // Depending on your FOREIGN KEY constraints, this might fail if orders reference this customer.
                string query = "DELETE FROM customer WHERE Customer_ID = @CustomerId";
                using MySqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@CustomerId", customerId);
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Customer deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadCustomerDataFromDatabase();
                }
                else
                {
                    MessageBox.Show("Customer not found or could not be deleted.", "Deletion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (MySqlException myEx) // Handle potential foreign key constraint violations
            {
                Debug.WriteLine($"MySQL Error deleting customer: {myEx.ToString()}");
                string message = $"Database Error (MySQL): {myEx.Message} (Code: {myEx.Number}).";
                if (myEx.Number == 1451) // Foreign key constraint fails
                {
                    message += "\nThis customer might have existing orders or other related records and cannot be deleted directly.";
                }
                MessageBox.Show(message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Generic error deleting customer: {ex.ToString()}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
