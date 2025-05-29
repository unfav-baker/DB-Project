using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace Adminn
{
    public partial class Employee : Page
    {
        public ObservableCollection<EmployeeData> Employees { get; set; }

        private const string DbConnectionStringEnvVar = "PRIMETECH_DB_CONN_STRING";
        private readonly string? connectionString;

        public Employee()
        {
            InitializeComponent();
            Employees = new();

            connectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVar);

            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show($"Database connection string from environment variable '{DbConnectionStringEnvVar}' was not found or is empty. " +
                                $"Please ensure your .env file is correctly set up and loaded at application startup (App.xaml.cs).\n\n" +
                                "Employee data cannot be loaded.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            LoadEmployeeDataFromDatabase();
            DataContext = this;
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            AddEmployee addEmployeePage = new();
            if (Application.Current.MainWindow is MainWindow mainWindow && mainWindow.MainContentFrame != null)
            {
                mainWindow.MainContentFrame.Navigate(addEmployeePage);
            }
            else
            {
                MessageBox.Show("Cannot navigate to Add Employee page. Main frame not found.", "Navigation Error");
            }
        }

        private void ViewEmployee_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeeDataGrid.SelectedItem is EmployeeData selectedEmployee)
            {
                string details = $"""
                                  Employee Details:

                                  1:    ID: {selectedEmployee.EmployeeId}
                                  2:    Name: {selectedEmployee.Name}
                                  3:    Role: {selectedEmployee.Role}
                                  4:    Phone: {selectedEmployee.PhoneNumber}
                                  5:    Salary: {selectedEmployee.Salary:C} 
                                  6:    Status: {selectedEmployee.Status}
                                  7:    Created At: {selectedEmployee.CreatedAt:dd/MM/yyyy HH:mm}
                                  8:    Updated At: {selectedEmployee.UpdatedAt:dd/MM/yyyy HH:mm}
                                  """;
                MessageBox.Show(details, "Employee Details", MessageBoxButton.OK, MessageBoxImage.Information);
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
                MessageBox.Show($"Edit functionality for Employee ID: {selectedEmployee.EmployeeId} needs to be implemented (e.g., navigate to an Edit page).",
                                "Edit Employee", MessageBoxButton.OK, MessageBoxImage.Information);
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
                var result = MessageBox.Show($"Are you sure you want to delete employee '{selectedEmployee.Name}' (ID: {selectedEmployee.EmployeeId})?",
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
            if (string.IsNullOrEmpty(this.connectionString))
            {
                System.Diagnostics.Debug.WriteLine("LoadEmployeeDataFromDatabase: Connection string is missing.");
                return;
            }

            Employees.Clear();
            try
            {
                using MySqlConnection connection = new(this.connectionString);
                connection.Open();

                // CORRECTED: Changed table name from 'employees' to 'employee'
                // Also, assuming your DB table uses Employee_ID as the primary key for consistency with earlier table designs
                // If your 'employee' table uses 'AdminId' as the primary key, change 'Employee_ID' back to 'AdminId' in the query.
                string query = @"SELECT Employee_ID, Name, Role, Phone_Number, Salary, Status, Created_At, Updated_At  
                                 FROM employee ORDER BY Employee_ID"; // Using singular 'employee'

                using MySqlCommand command = new(query, connection);
                using MySqlDataReader reader = command.ExecuteReader();

                // Get ordinals for safety
                int empIdOrdinal = reader.GetOrdinal("Employee_ID"); // Or "AdminId" if that's your PK in 'employee' table
                int nameOrdinal = reader.GetOrdinal("Name");
                int roleOrdinal = reader.GetOrdinal("Role");
                int phoneOrdinal = reader.GetOrdinal("Phone_Number");
                int salaryOrdinal = reader.GetOrdinal("Salary");
                int statusOrdinal = reader.GetOrdinal("Status");
                int createdAtOrdinal = reader.GetOrdinal("Created_At");
                int updatedAtOrdinal = reader.GetOrdinal("Updated_At");

                while (reader.Read())
                {
                    var employee = new EmployeeData
                    {
                        EmployeeId = reader.GetInt32(empIdOrdinal),
                        Name = reader.IsDBNull(nameOrdinal) ? string.Empty : reader.GetString(nameOrdinal),
                        Role = reader.IsDBNull(roleOrdinal) ? string.Empty : reader.GetString(roleOrdinal),
                        PhoneNumber = reader.IsDBNull(phoneOrdinal) ? string.Empty : reader.GetString(phoneOrdinal),
                        Salary = reader.IsDBNull(salaryOrdinal) ? 0m : reader.GetDecimal(salaryOrdinal),
                        Status = reader.IsDBNull(statusOrdinal) ? string.Empty : reader.GetString(statusOrdinal),
                        CreatedAt = reader.IsDBNull(createdAtOrdinal) ? DateTime.MinValue : reader.GetDateTime(createdAtOrdinal),
                        UpdatedAt = reader.IsDBNull(updatedAtOrdinal) ? DateTime.MinValue : reader.GetDateTime(updatedAtOrdinal),
                        // Clarify what EmployeeData.AdminId should be. If it's the same as EmployeeId:
                        AdminId = reader.GetInt32(empIdOrdinal)
                    };
                    Employees.Add(employee);
                }
            }
            catch (MySqlException myEx)
            {
                Debug.WriteLine($"MySQL Error loading employee data: {myEx.ToString()}");
                MessageBox.Show($"Database Error (MySQL): {myEx.Message} (Code: {myEx.Number})", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Generic error loading employee data: {ex.ToString()}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteEmployeeFromDatabase(int employeeIdToDelete)
        {
            if (string.IsNullOrEmpty(this.connectionString))
            {
                MessageBox.Show("Database connection string is not configured. Cannot delete employee.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using MySqlConnection connection = new(this.connectionString);
                connection.Open();

                // CORRECTED: Changed table name from 'employees' to 'employee'
                // Also, ensure 'Employee_ID' is the correct primary key column name in your 'employee' table
                string query = "DELETE FROM employee WHERE Employee_ID = @EmployeeId";

                using MySqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@EmployeeId", employeeIdToDelete);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show($"Employee with ID {employeeIdToDelete} deleted successfully!", "Success",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadEmployeeDataFromDatabase();
                }
                else
                {
                    MessageBox.Show($"Employee with ID {employeeIdToDelete} not found or could not be deleted.", "Deletion Error",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (MySqlException myEx)
            {
                Debug.WriteLine($"MySQL Error deleting employee: {myEx.ToString()}");
                MessageBox.Show($"Database Error (MySQL) deleting employee: {myEx.Message} (Code: {myEx.Number})", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Generic error deleting employee: {ex.ToString()}");
                MessageBox.Show($"Error deleting employee: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } }
        }
        public int EmployeeId
        {
            get => _employeeId;
            set { if (_employeeId != value) { _employeeId = value; OnPropertyChanged(); } }
        }
        public string Name
        {
            get => _name;
            set { if (_name != value) { _name = value ?? string.Empty; OnPropertyChanged(); } }
        }
        public string Role
        {
            get => _role;
            set { if (_role != value) { _role = value ?? string.Empty; OnPropertyChanged(); } }
        }
        public string PhoneNumber
        {
            get => _phoneNumber;
            set { if (_phoneNumber != value) { _phoneNumber = value ?? string.Empty; OnPropertyChanged(); } }
        }
        public decimal Salary
        {
            get => _salary;
            set { if (_salary != value) { _salary = value; OnPropertyChanged(); } }
        }
        public int AdminId
        {
            get => _adminId;
            set { if (_adminId != value) { _adminId = value; OnPropertyChanged(); } }
        }
        public string Status
        {
            get => _status;
            set { if (_status != value) { _status = value ?? string.Empty; OnPropertyChanged(); } }
        }
        public DateTime CreatedAt
        {
            get => _createdAt;
            set { if (_createdAt != value) { _createdAt = value; OnPropertyChanged(); } }
        }
        public DateTime UpdatedAt
        {
            get => _updatedAt;
            set { if (_updatedAt != value) { _updatedAt = value; OnPropertyChanged(); } }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
