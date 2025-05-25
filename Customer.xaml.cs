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
    public partial class Customer : Page
    {
        public ObservableCollection<CustomerData> Customers { get; set; }

        // Replace this with your actual MySQL connection string
        private readonly string connectionString = "Server=127.0.0.1;Port=3306;Database=prime_tech;Uid=root;Pwd=Abubaker85@@;";

        public Customer()
        {
            InitializeComponent();
            Customers = new ObservableCollection<CustomerData>();

            // Load customer data from database
            LoadCustomerDataFromDatabase();

            // Set the DataContext for binding
            DataContext = this;
        }


        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to AddCustomer page within the parent frame
            AddCustomer addCustomerPage = new AddCustomer();
            // Find the main window's frame and navigate
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.MainContentFrame.Navigate(addCustomerPage);
            }
        }

        private void ViewCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerDataGrid.SelectedItem is CustomerData selectedCustomer)
            {
                MessageBox.Show($"Customer Details:\n\n\n\n" +
                    $"1:   Date: {selectedCustomer.Date:dd/MM/yyyy}\n\n" +
                    $"2:   Phyto Number: {selectedCustomer.PhytoNumber}\n\n" +
                    $"3:   Party Name: {selectedCustomer.PartyName}\n\n" +
                    $"4:   Export Through: {selectedCustomer.ExportThrough}\n\n" +
                    $"5:   Plant: {selectedCustomer.Plant}\n\n" +
                    $"6:   Importer: {selectedCustomer.Importer}\n\n" +
                    $"7:   Carton: {selectedCustomer.Carton}\n\n" +
                    $"8:   Weight: {selectedCustomer.Weight}\n\n" +
                    $"9:   Rate: {selectedCustomer.Rate:C}\n\n" +
                    $"10:  Amount: {selectedCustomer.Amount:C}\n\n" +
                    $"11:  Received: {selectedCustomer.Received}\n",
                    "Customer Details", MessageBoxButton.OK, MessageBoxImage.Information);
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
                // Here you can navigate to an edit page or open an edit dialog
                MessageBox.Show($"Edit functionality for Phyto Number: {selectedCustomer.PhytoNumber}", "Edit Customer", MessageBoxButton.OK, MessageBoxImage.Information);
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
                var result = MessageBox.Show($"Are you sure you want to delete customer '{selectedCustomer.PartyName}'?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteCustomerFromDatabase(selectedCustomer.PhytoNumber);
                }
            }
            else
            {
                MessageBox.Show("Please select a customer to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoadCustomerDataFromDatabase()
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = @"SELECT Date, Phyto_Number, Party_Name, Export_Through, Plant, 
                               Importer, Carton, Weight, Rate, Amount, Received 
                               FROM customer ORDER BY Date";

                using var command = new MySqlCommand(query, connection);
                using var reader = command.ExecuteReader();

                Customers.Clear();

                while (reader.Read())
                {
                    var customer = new CustomerData
                    {
                        Date = reader.IsDBNull("Date") ? DateTime.Now : reader.GetDateTime("Date"),
                        PhytoNumber = reader.IsDBNull("Phyto_Number") ? string.Empty : reader.GetString("Phyto_Number"),
                        PartyName = reader.IsDBNull("Party_Name") ? string.Empty : reader.GetString("Party_Name"),
                        ExportThrough = reader.IsDBNull("Export_Through") ? string.Empty : reader.GetString("Export_Through"),
                        Plant = reader.IsDBNull("Plant") ? string.Empty : reader.GetString("Plant"),
                        Importer = reader.IsDBNull("Importer") ? string.Empty : reader.GetString("Importer"),
                        Carton = reader.IsDBNull("Carton") ? 0 : reader.GetInt32("Carton"),
                        Weight = reader.IsDBNull("Weight") ? 0 : reader.GetDecimal("Weight"),
                        Rate = reader.IsDBNull("Rate") ? 0 : reader.GetDecimal("Rate"),
                        Amount = reader.IsDBNull("Amount") ? 0 : reader.GetDecimal("Amount"),
                        Received = reader.IsDBNull("Received") ? string.Empty : reader.GetString("Received")
                    };

                    Customers.Add(customer);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customer data: {ex.Message}", "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteCustomerFromDatabase(string phytoNumber)
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = "DELETE FROM customer WHERE Phyto_Number = @PhytoNumber";

                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@PhytoNumber", phytoNumber);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Customer deleted successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Refresh the data
                    LoadCustomerDataFromDatabase();
                }
                else
                {
                    MessageBox.Show("Customer not found or could not be deleted.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting customer: {ex.Message}", "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void RefreshData()
        {
            LoadCustomerDataFromDatabase();
        }
    }

    public class CustomerData : INotifyPropertyChanged
    {
        private bool _isSelected;
        private DateTime _date;
        private string _phytoNumber = string.Empty;
        private string _partyName = string.Empty;
        private string _exportThrough = string.Empty;
        private string _plant = string.Empty;
        private string _importer = string.Empty;
        private int _carton;
        private decimal _weight;
        private decimal _rate;
        private decimal _amount;
        private string _received = string.Empty; // Changed to string based on your data

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public DateTime Date
        {
            get => _date;
            set
            {
                _date = value;
                OnPropertyChanged();
            }
        }

        public string PhytoNumber
        {
            get => _phytoNumber;
            set
            {
                _phytoNumber = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public string PartyName
        {
            get => _partyName;
            set
            {
                _partyName = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public string ExportThrough
        {
            get => _exportThrough;
            set
            {
                _exportThrough = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public string Plant
        {
            get => _plant;
            set
            {
                _plant = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public string Importer
        {
            get => _importer;
            set
            {
                _importer = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public int Carton
        {
            get => _carton;
            set
            {
                _carton = value;
                OnPropertyChanged();
            }
        }

        public decimal Weight
        {
            get => _weight;
            set
            {
                _weight = value;
                OnPropertyChanged();
            }
        }

        public decimal Rate
        {
            get => _rate;
            set
            {
                _rate = value;
                OnPropertyChanged();
            }
        }

        public decimal Amount
        {
            get => _amount;
            set
            {
                _amount = value;
                OnPropertyChanged();
            }
        }

        public string Received
        {
            get => _received;
            set
            {
                _received = value ?? string.Empty;
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