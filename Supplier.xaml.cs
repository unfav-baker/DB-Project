using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Text;

namespace Adminn
{
    public partial class Supplier : Page
    {
        public ObservableCollection<SupplierData> Suppliers { get; set; }

        private readonly string? connectionString;
        private const string DbConnectionStringEnvVar = "PRIMETECH_DB_CONN_STRING";

        public Supplier()
        {
            InitializeComponent();
            Suppliers = new ObservableCollection<SupplierData>();
            this.DataContext = this;

            connectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVar);

            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show($"Database connection string environment variable '{DbConnectionStringEnvVar}' was not found or is empty. " +
                                $"Please ensure your .env file is correctly set up and loaded at application startup (App.xaml.cs).\n\n" +
                                "Supplier data cannot be loaded.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                LoadSupplierDataFromDatabase();
            }
        }

        private void LoadSupplierDataFromDatabase()
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                Debug.WriteLine("Supplier.xaml.cs: Connection string is null/empty in LoadSupplierDataFromDatabase.");
                return;
            }

            Suppliers.Clear();
            try
            {
                using MySqlConnection connection = new(connectionString);
                connection.Open();

                // CORRECTED: Using actual column names from your database table:
                // Name (instead of S_Name)
                // Role (instead of S_Role)
                // Status (instead of S_Status)
                // Phone_Number, Plant_Name, Created_At, Updated_At seem correct based on your screenshot.
                string query = @"SELECT Supplier_ID, Name, Phone_Number, Role, Plant_Name, Status, Created_At, Updated_At 
                                 FROM supplier ORDER BY Supplier_ID";

                using MySqlCommand command = new(query, connection);
                using MySqlDataReader reader = command.ExecuteReader();

                int supplierIdOrdinal = reader.GetOrdinal("Supplier_ID");
                int nameOrdinal = reader.GetOrdinal("Name"); // Was S_Name
                int phoneOrdinal = reader.GetOrdinal("Phone_Number");
                int roleOrdinal = reader.GetOrdinal("Role"); // Was S_Role
                int plantNameOrdinal = reader.GetOrdinal("Plant_Name");
                int statusOrdinal = reader.GetOrdinal("Status"); // Was S_Status
                int createdAtOrdinal = reader.GetOrdinal("Created_At");
                int updatedAtOrdinal = reader.GetOrdinal("Updated_At");

                while (reader.Read())
                {
                    Suppliers.Add(new SupplierData
                    {
                        SupplierId = reader.GetInt32(supplierIdOrdinal),
                        Name = reader.IsDBNull(nameOrdinal) ? string.Empty : reader.GetString(nameOrdinal),
                        PhoneNumber = reader.IsDBNull(phoneOrdinal) ? string.Empty : reader.GetString(phoneOrdinal),
                        Role = reader.IsDBNull(roleOrdinal) ? string.Empty : reader.GetString(roleOrdinal),
                        PlantName = reader.IsDBNull(plantNameOrdinal) ? string.Empty : reader.GetString(plantNameOrdinal),
                        Status = reader.IsDBNull(statusOrdinal) ? string.Empty : reader.GetString(statusOrdinal),
                        CreatedAt = reader.IsDBNull(createdAtOrdinal) ? DateTime.MinValue : reader.GetDateTime(createdAtOrdinal),
                        UpdatedAt = reader.IsDBNull(updatedAtOrdinal) ? DateTime.MinValue : reader.GetDateTime(updatedAtOrdinal)
                    });
                }
            }
            catch (MySqlException myEx)
            {
                Debug.WriteLine($"MySQL Error loading supplier data: {myEx.ToString()}");
                MessageBox.Show($"Database Error (MySQL): {myEx.Message} (Code: {myEx.Number})", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Generic error loading supplier data: {ex.ToString()}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // AddSupplier_Click, ViewSupplier_Click, EditSupplier_Click, DeleteSupplier_Click,
        // DeleteSupplierFromDatabase, and RefreshData methods remain the same
        // as provided in the previous 'Adminn/Supplier.xaml.cs' (ID: supplier_page_cs_adminn_v2)
        // Just ensure their SQL queries (especially in DeleteSupplierFromDatabase if it references columns by name)
        // also use the correct column names like 'Name', 'Role', 'Status' if needed,
        // and the table name is 'supplier'.

        private void AddSupplier_Click(object sender, RoutedEventArgs e)
        {
            AddSupplier addSupplierPage = new();
            if (Application.Current.MainWindow is MainWindow mainWindow && mainWindow.MainContentFrame != null)
            {
                mainWindow.MainContentFrame.Navigate(addSupplierPage);
            }
            else
            {
                MessageBox.Show("Cannot navigate to Add Supplier page. Main frame not found.", "Navigation Error");
            }
        }

        private void ViewSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (SupplierDataGrid.SelectedItem is SupplierData selectedSupplier)
            {
                StringBuilder details = new StringBuilder();
                details.AppendLine("Supplier Details:\n");
                details.AppendLine($"ID: {selectedSupplier.SupplierId}");
                details.AppendLine($"Name: {selectedSupplier.Name}");
                details.AppendLine($"Phone: {selectedSupplier.PhoneNumber}");
                details.AppendLine($"Role: {selectedSupplier.Role}");
                details.AppendLine($"Plant Name: {selectedSupplier.PlantName}");
                details.AppendLine($"Status: {selectedSupplier.Status}");
                details.AppendLine($"Registered On: {selectedSupplier.CreatedAt:dd/MM/yyyy HH:mm}");
                details.AppendLine($"Last Updated: {selectedSupplier.UpdatedAt:dd/MM/yyyy HH:mm}");

                MessageBox.Show(details.ToString(), "Supplier Details", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select a supplier to view.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void EditSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (SupplierDataGrid.SelectedItem is SupplierData selectedSupplier)
            {
                MessageBox.Show($"Edit functionality for Supplier ID: {selectedSupplier.SupplierId} needs to be implemented.", "Edit Supplier", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select a supplier to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (SupplierDataGrid.SelectedItem is SupplierData selectedSupplier)
            {
                var result = MessageBox.Show($"Are you sure you want to delete supplier '{selectedSupplier.Name}' (ID: {selectedSupplier.SupplierId})?",
                                             "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteSupplierFromDatabase(selectedSupplier.SupplierId);
                }
            }
            else
            {
                MessageBox.Show("Please select a supplier to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteSupplierFromDatabase(int supplierId)
        {
            if (string.IsNullOrEmpty(this.connectionString))
            {
                MessageBox.Show("Database connection is not configured. Cannot delete supplier.", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                using MySqlConnection connection = new(this.connectionString);
                connection.Open();

                string query = "DELETE FROM supplier WHERE Supplier_ID = @SupplierIdParam";

                using MySqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@SupplierIdParam", supplierId);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Supplier deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadSupplierDataFromDatabase();
                }
                else
                {
                    MessageBox.Show("Supplier not found or could not be deleted.", "Deletion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (MySqlException myEx)
            {
                Debug.WriteLine($"MySQL Error deleting supplier: {myEx.ToString()}");
                string message = $"Database Error (MySQL): {myEx.Message} (Code: {myEx.Number}).";
                if (myEx.Number == 1451)
                {
                    message += "\nThis supplier might be linked to existing products or orders and cannot be deleted directly.";
                }
                MessageBox.Show(message, "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Generic error deleting supplier: {ex.ToString()}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void RefreshData()
        {
            if (!string.IsNullOrEmpty(connectionString))
            {
                LoadSupplierDataFromDatabase();
            }
            else
            {
                MessageBox.Show("Database connection is not configured. Cannot refresh data.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
    }

    // The SupplierData class should be the same as you provided
    // (ensure its properties like Name, Role, Status, etc. match the DataGrid bindings)
    public class SupplierData : INotifyPropertyChanged
    {
        private bool _isSelected;
        private int _supplierId;
        private string _name = string.Empty;
        private string _phoneNumber = string.Empty;
        private string _role = string.Empty;
        private string _plantName = string.Empty;
        private string _status = string.Empty;
        private DateTime _createdAt;
        private DateTime _updatedAt;

        public bool IsSelected { get => _isSelected; set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } } }
        public int SupplierId { get => _supplierId; set { if (_supplierId != value) { _supplierId = value; OnPropertyChanged(); } } }
        public string Name { get => _name; set { if (_name != value) { _name = value ?? string.Empty; OnPropertyChanged(); } } }
        public string PhoneNumber { get => _phoneNumber; set { if (_phoneNumber != value) { _phoneNumber = value ?? string.Empty; OnPropertyChanged(); } } }
        public string Role { get => _role; set { if (_role != value) { _role = value ?? string.Empty; OnPropertyChanged(); } } }
        public string PlantName { get => _plantName; set { if (_plantName != value) { _plantName = value ?? string.Empty; OnPropertyChanged(); } } }
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
