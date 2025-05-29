using System;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Data;
// Ensure EmployeeProfileDataViewModel is accessible, e.g., defined in this namespace or via a using directive
// using YourProject.ViewModels; // If it's in a ViewModels folder/namespace

namespace Employee // Your namespace
{
    public partial class Profile : Page
    {
        public EmployeeProfileDataViewModel CurrentEmployee { get; set; }

        private readonly string? connectionString;
        private const string DbConnectionStringEnvVar = "PRIMETECH_DB_CONN_STRING";
        private int _loadedEmployeeId;

        public Profile()
        {
            InitializeComponent();
            CurrentEmployee = new(); // 'new' expression simplified
            this.DataContext = CurrentEmployee;

            _loadedEmployeeId = 1; // <<<<< TEMPORARY: Assuming Employee_ID 1 for now

            connectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVar);

            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Database connection string not configured.", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            LoadEmployeeProfile(_loadedEmployeeId);
        }

        public Profile(int employeeIdToLoad)
        {
            InitializeComponent();
            CurrentEmployee = new(); // 'new' expression simplified
            this.DataContext = CurrentEmployee;

            _loadedEmployeeId = employeeIdToLoad;
            connectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVar);

            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Database connection string not configured.", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            LoadEmployeeProfile(_loadedEmployeeId);
        }

        public void LoadEmployeeProfile(int employeeId)
        {
            _loadedEmployeeId = employeeId;
            if (string.IsNullOrEmpty(connectionString))
            {
                Debug.WriteLine("Profile.xaml.cs: Connection string is null or empty in LoadEmployeeProfile.");
                return;
            }

            try
            {
                using MySqlConnection connection = new(connectionString); // 'new' expression simplified
                connection.Open();

                string query = @"
                    SELECT e.*, a.Name as ManagingAdminName 
                    FROM employee e
                    LEFT JOIN admin a ON e.FK_Admin_ID = a.Admin_ID
                    WHERE e.Employee_ID = @EmployeeId";

                using MySqlCommand command = new(query, connection); // 'new' expression simplified
                command.Parameters.AddWithValue("@EmployeeId", employeeId);

                using MySqlDataReader reader = command.ExecuteReader();
                if (reader.Read())
                {
                    CurrentEmployee.EmployeeId = reader.GetInt32("Employee_ID");
                    CurrentEmployee.FullName = reader.IsDBNull("Name") ? "N/A" : reader.GetString("Name");
                    CurrentEmployee.Gender = reader.IsDBNull("Gender") ? "N/A" : reader.GetString("Gender");
                    CurrentEmployee.DateOfBirth = reader.IsDBNull("Date_Of_Birth") ? null : reader.GetDateTime("Date_Of_Birth");
                    CurrentEmployee.Email = reader.IsDBNull("Email") ? "N/A" : reader.GetString("Email");
                    CurrentEmployee.PhoneNumber = reader.IsDBNull("Phone_Number") ? "N/A" : reader.GetString("Phone_Number");
                    CurrentEmployee.Address = reader.IsDBNull("Address") ? "N/A" : reader.GetString("Address");
                    CurrentEmployee.Role = reader.IsDBNull("Role") ? "N/A" : reader.GetString("Role");
                    CurrentEmployee.ManagingAdminName = reader.IsDBNull("ManagingAdminName") ? "N/A" : reader.GetString("ManagingAdminName");
                    CurrentEmployee.Status = reader.IsDBNull("Status") ? "N/A" : reader.GetString("Status");

                    CurrentEmployee.Nationality = reader.IsDBNull("Nationality") ? "N/A" : reader.GetString("Nationality");
                    CurrentEmployee.HireDate = reader.IsDBNull("Hire_Date") ? null : reader.GetDateTime("Hire_Date");
                    CurrentEmployee.LastLogin = reader.IsDBNull("Last_Login") ? null : reader.GetDateTime("Last_Login");
                    CurrentEmployee.LoginNotificationsEnabled = !reader.IsDBNull("Login_Notifications_Enabled") && reader.GetBoolean("Login_Notifications_Enabled");
                    CurrentEmployee.LanguagePreference = reader.IsDBNull("Language_Preference") ? "N/A" : reader.GetString("Language_Preference");
                    CurrentEmployee.TimeZone = reader.IsDBNull("Time_Zone") ? "N/A" : reader.GetString("Time_Zone");

                    if (this.FindName("SidebarEmployeeName") is TextBlock nameBlock) nameBlock.Text = CurrentEmployee.FullName;
                    if (this.FindName("SidebarEmployeeEmail") is TextBlock emailBlock) emailBlock.Text = CurrentEmployee.Email;
                }
                else
                {
                    MessageBox.Show($"Employee profile with ID {employeeId} not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading employee profile: {ex}");
                MessageBox.Show($"Error loading profile: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void EditProfileButton_Click(object sender, RoutedEventArgs e)
        {
            EditProfile editProfilePage = new(CurrentEmployee);

            if (Application.Current.MainWindow is MainWindow mainWindow && mainWindow.MainContentFrame is not null)
            {
                mainWindow.MainContentFrame.Navigate(editProfilePage);
            }
            else if (this.NavigationService is { CanGoBack: true })
            {
                // If EditProfilePage is designed to be navigated to by Profile page's NavigationService
                this.NavigationService.Navigate(editProfilePage);
            }
            else
            {
                MessageBox.Show("Could not navigate to edit page. Appropriate Frame not found.", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
