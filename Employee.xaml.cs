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
    public partial class Employee : Page
    {
        public ObservableCollection<EmployeeData> Employees { get; set; }

        // Replace this with your actual MySQL connection string
        private readonly string connectionString = "Server=127.0.0.1;Port=3306;Database=prime_tech;Uid=root;Pwd=Abubaker85@@;";
        // Alternative format: "Server=localhost;Port=3306;Database=prime_tech;Uid=root;Pwd=YOUR_PASSWORD;";

        public Employee()
        {
            InitializeComponent();
            Employees = [];

            // Load employee data from database
            LoadEmployeeDataFromDatabase();

            // Set the DataContext for binding
            DataContext = this;
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to AddEmployee page within the parent frame
            AddEmployee addEmployeePage = new AddEmployee();
            // Find the main window's frame and navigate
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.MainContentFrame.Navigate(addEmployeePage);
            }
        }

        private void ViewEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeDataGrid.SelectedItem is EmployeeData selectedEmployee)
            {
                MessageBox.Show($"Employee Details:\nID: {selectedEmployee.EmployeeId}\nName: {selectedEmployee.Name}\nRole: {selectedEmployee.Role}\nPhone: {selectedEmployee.PhoneNumber}\nSalary: {selectedEmployee.Salary:C}\nStatus: {selectedEmployee.Status}",
                    "Employee Details", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select an employee to view.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void EditEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeDataGrid.SelectedItem is EmployeeData selectedEmployee)
            {
                // Here you can navigate to an edit page or open an edit dialog
                MessageBox.Show($"Edit functionality for Employee ID: {selectedEmployee.EmployeeId}", "Edit Employee", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select an employee to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeDataGrid.SelectedItem is EmployeeData selectedEmployee)
            {
                var result = MessageBox.Show($"Are you sure you want to delete employee '{selectedEmployee.Name}'?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteEmployeeFromDatabase(selectedEmployee.EmployeeId);
                }
            }
            else
            {
                MessageBox.Show("Please select an employee to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoadEmployeeDataFromDatabase()
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = @"SELECT AdminId, Name, Role, PhoneNumber, Salary, Status, CreatedAt, UpdatedAt 
                               FROM employees ORDER BY AdminId";

                using var command = new MySqlCommand(query, connection);
                using var reader = command.ExecuteReader();

                Employees.Clear();

                while (reader.Read())
                {
                    var employee = new EmployeeData
                    {
                        EmployeeId = reader.GetInt32("AdminId"), // Using AdminId as EmployeeId
                        Name = reader.IsDBNull("Name") ? string.Empty : reader.GetString("Name"),
                        Role = reader.IsDBNull("Role") ? string.Empty : reader.GetString("Role"),
                        PhoneNumber = reader.IsDBNull("PhoneNumber") ? string.Empty : reader.GetString("PhoneNumber"),
                        Salary = reader.IsDBNull("Salary") ? 0 : reader.GetDecimal("Salary"),
                        AdminId = reader.IsDBNull("AdminId") ? 0 : reader.GetInt32("AdminId"),
                        Status = reader.IsDBNull("Status") ? string.Empty : reader.GetString("Status"),
                        CreatedAt = reader.IsDBNull("CreatedAt") ? DateTime.Now : reader.GetDateTime("CreatedAt"),
                        UpdatedAt = reader.IsDBNull("UpdatedAt") ? DateTime.Now : reader.GetDateTime("UpdatedAt")
                    };

                    Employees.Add(employee);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading employee data: {ex.Message}", "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteEmployeeFromDatabase(int employeeId)
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = "DELETE FROM employees WHERE AdminId = @AdminId";

                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@AdminId", employeeId);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Employee deleted successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Refresh the data
                    LoadEmployeeDataFromDatabase();
                }
                else
                {
                    MessageBox.Show("Employee not found or could not be deleted.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting employee: {ex.Message}", "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void RefreshData()
        {
            LoadEmployeeDataFromDatabase();
        }
    }

    public class EmployeeData : INotifyPropertyChanged
    {
        private bool _isSelected;
        private int _employeeId;
        private string _name = string.Empty;
        private string _role = string.Empty;
        private string _phoneNumber = string.Empty;
        private decimal _salary;
        private int _adminId;
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

        public int EmployeeId
        {
            get => _employeeId;
            set
            {
                _employeeId = value;
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

        public string Role
        {
            get => _role;
            set
            {
                _role = value ?? string.Empty;
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

        public decimal Salary
        {
            get => _salary;
            set
            {
                _salary = value;
                OnPropertyChanged();
            }
        }

        public int AdminId
        {
            get => _adminId;
            set
            {
                _adminId = value;
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