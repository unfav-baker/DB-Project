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
    public partial class Supplier : Page
    {
        public ObservableCollection<SupplierData> Suppliers { get; set; }

        // Replace this with your actual MySQL connection string
        private readonly string connectionString = "Server=127.0.0.1;Port=3306;Database=prime_tech;Uid=root;Pwd=Abubaker85@@;";

        public Supplier()
        {
            InitializeComponent();
            Suppliers = new ObservableCollection<SupplierData>();

            // Load supplier data from database
            LoadSupplierDataFromDatabase();

            // Set the DataContext for binding
            DataContext = this;
        }

        private void AddSupplier_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to AddSupplier page within the parent frame
            AddSupplier addSupplierPage = new AddSupplier();
            // Find the main window's frame and navigate
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.MainContentFrame.Navigate(addSupplierPage);
            }
        }

        private void ViewSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (SupplierDataGrid.SelectedItem is SupplierData selectedSupplier)
            {
                MessageBox.Show($"Supplier Details:\nID: {selectedSupplier.SupplierId}\nName: {selectedSupplier.Name}\nPhone: {selectedSupplier.PhoneNumber}\nRole: {selectedSupplier.Role}\nPlant Name: {selectedSupplier.PlantName}\nStatus: {selectedSupplier.Status}",
                    "Supplier Details", MessageBoxButton.OK, MessageBoxImage.Information);
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
                // Here you can navigate to an edit page or open an edit dialog
                MessageBox.Show($"Edit functionality for Supplier ID: {selectedSupplier.SupplierId}", "Edit Supplier", MessageBoxButton.OK, MessageBoxImage.Information);
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
                var result = MessageBox.Show($"Are you sure you want to delete supplier '{selectedSupplier.Name}'?",
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

        private void LoadSupplierDataFromDatabase()
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = @"SELECT Supplier_ID, S_Name, Phone_Number, S_Role, Plant_Name, S_Status, Created_At, Updated_At 
                               FROM supplier ORDER BY Supplier_ID";

                using var command = new MySqlCommand(query, connection);
                using var reader = command.ExecuteReader();

                Suppliers.Clear();

                while (reader.Read())
                {
                    var supplier = new SupplierData
                    {
                        SupplierId = reader.GetInt32("Supplier_ID"),
                        Name = reader.IsDBNull("S_Name") ? string.Empty : reader.GetString("S_Name"),
                        PhoneNumber = reader.IsDBNull("Phone_Number") ? string.Empty : reader.GetString("Phone_Number"),
                        Role = reader.IsDBNull("S_Role") ? string.Empty : reader.GetString("S_Role"),
                        PlantName = reader.IsDBNull("Plant_Name") ? string.Empty : reader.GetString("Plant_Name"),
                        Status = reader.IsDBNull("S_Status") ? string.Empty : reader.GetString("S_Status"),
                        CreatedAt = reader.IsDBNull("Created_At") ? DateTime.Now : reader.GetDateTime("Created_At"),
                        UpdatedAt = reader.IsDBNull("Updated_At") ? DateTime.Now : reader.GetDateTime("Updated_At")
                    };

                    Suppliers.Add(supplier);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading supplier data: {ex.Message}", "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteSupplierFromDatabase(int supplierId)
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = "DELETE FROM suppliers WHERE Supplier_ID = @SupplierId";

                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@SupplierId", supplierId);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Supplier deleted successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Refresh the data
                    LoadSupplierDataFromDatabase();
                }
                else
                {
                    MessageBox.Show("Supplier not found or could not be deleted.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting supplier: {ex.Message}", "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void RefreshData()
        {
            LoadSupplierDataFromDatabase();
        }
    }

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

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
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

        public string Name
        {
            get => _name;
            set
            {
                _name = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public string PhoneNumber
        {
            get => _phoneNumber;
            set
            {
                _phoneNumber = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public string Role
        {
            get => _role;
            set
            {
                _role = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public string PlantName
        {
            get => _plantName;
            set
            {
                _plantName = value ?? string.Empty;
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

        public DateTime UpdatedAt
        {
            get => _updatedAt;
            set
            {
                _updatedAt = value;
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