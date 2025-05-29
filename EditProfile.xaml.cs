using System;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Diagnostics;

namespace Employee // Your namespace
{
    public partial class EditProfile : Page
    {
        public EmployeeProfileDataViewModel EditableEmployee { get; set; }
        // Storing original data is good if you want to compare for changes or revert complex objects,
        // but for simple property-by-property update on the original view model passed in, it's also an option.
        // The current approach (copying to EditableEmployee and then updating originalEmployeeData) is fine.
        private readonly EmployeeProfileDataViewModel originalEmployeeDataViewModel;

        private readonly string? connectionString;
        private const string DbConnectionStringEnvVar = "PRIMETECH_DB_CONN_STRING";

        public EditProfile(EmployeeProfileDataViewModel employeeToEditViewModel)
        {
            InitializeComponent();
            originalEmployeeDataViewModel = employeeToEditViewModel; // Store reference to the ViewModel passed from Profile page

            // Create a new instance for editing to allow cancellation without affecting the original object immediately
            EditableEmployee = new EmployeeProfileDataViewModel
            {
                EmployeeId = employeeToEditViewModel.EmployeeId,
                FullName = employeeToEditViewModel.FullName,
                Gender = employeeToEditViewModel.Gender,
                DateOfBirth = employeeToEditViewModel.DateOfBirth,
                Nationality = employeeToEditViewModel.Nationality,
                Email = employeeToEditViewModel.Email,
                PhoneNumber = employeeToEditViewModel.PhoneNumber,
                Address = employeeToEditViewModel.Address,
                LastLogin = employeeToEditViewModel.LastLogin,
                HireDate = employeeToEditViewModel.HireDate,
                Role = employeeToEditViewModel.Role,
                LoginNotificationsEnabled = employeeToEditViewModel.LoginNotificationsEnabled,
                ManagingAdminName = employeeToEditViewModel.ManagingAdminName,
                LanguagePreference = employeeToEditViewModel.LanguagePreference,
                TimeZone = employeeToEditViewModel.TimeZone,
                Status = employeeToEditViewModel.Status
            };
            this.DataContext = EditableEmployee;

            connectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVar);

            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Database connection string not configured. Profile cannot be saved.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // Attempt to find the Save button by its x:Name to disable it
                if (this.FindName("SaveChangesButton") is Button saveBtn) // Assuming your Save button is x:Name="SaveChangesButton"
                {
                    saveBtn.IsEnabled = false;
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Database connection is not configured. Cannot save profile.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (string.IsNullOrWhiteSpace(EditableEmployee.FullName))
            {
                MessageBox.Show("Full Name cannot be empty.", "Validation Error",
                                MessageBoxButton.OK, MessageBoxImage.Warning); // Corrected: MessageBoxImage.Warning
                if (this.FindName("FullNameTextBox") is TextBox fullNameBox) fullNameBox.Focus();
                return;
            }
            if (string.IsNullOrWhiteSpace(EditableEmployee.Email))
            {
                MessageBox.Show("Email cannot be empty.", "Validation Error",
                                MessageBoxButton.OK, MessageBoxImage.Warning); // Corrected: MessageBoxImage.Warning
                if (this.FindName("EmailTextBox") is TextBox emailBox) emailBox.Focus();
                return;
            }
            // Add more validation as needed (e.g., email format, phone number format)

            try
            {
                using MySqlConnection connection = new(connectionString!); // Null-forgiving operator used as we check string.IsNullOrEmpty
                connection.Open();

                string query = @"UPDATE employee SET 
                                    Name = @Name, 
                                    Gender = @Gender, 
                                    Date_Of_Birth = @DateOfBirth, 
                                    Nationality = @Nationality, 
                                    Email = @Email, 
                                    Phone_Number = @PhoneNumber, 
                                    Address = @Address,
                                    Login_Notifications_Enabled = @LoginNotificationsEnabled,
                                    Language_Preference = @LanguagePreference,
                                    Time_Zone = @TimeZone,
                                    Updated_At = @UpdatedAt 
                                 WHERE Employee_ID = @EmployeeId;";

                using MySqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@EmployeeId", EditableEmployee.EmployeeId);
                command.Parameters.AddWithValue("@Name", EditableEmployee.FullName.Trim());
                command.Parameters.AddWithValue("@Gender", string.IsNullOrWhiteSpace(EditableEmployee.Gender) ? DBNull.Value : (object)EditableEmployee.Gender);
                command.Parameters.AddWithValue("@DateOfBirth", EditableEmployee.DateOfBirth as object ?? DBNull.Value); // Simpler null check
                command.Parameters.AddWithValue("@Nationality", string.IsNullOrWhiteSpace(EditableEmployee.Nationality) ? DBNull.Value : (object)EditableEmployee.Nationality.Trim());
                command.Parameters.AddWithValue("@Email", EditableEmployee.Email.Trim());
                command.Parameters.AddWithValue("@PhoneNumber", string.IsNullOrWhiteSpace(EditableEmployee.PhoneNumber) ? DBNull.Value : (object)EditableEmployee.PhoneNumber.Trim());
                command.Parameters.AddWithValue("@Address", string.IsNullOrWhiteSpace(EditableEmployee.Address) ? DBNull.Value : (object)EditableEmployee.Address.Trim());
                command.Parameters.AddWithValue("@LoginNotificationsEnabled", EditableEmployee.LoginNotificationsEnabled);
                command.Parameters.AddWithValue("@LanguagePreference", string.IsNullOrWhiteSpace(EditableEmployee.LanguagePreference) ? DBNull.Value : (object)EditableEmployee.LanguagePreference.Trim());
                command.Parameters.AddWithValue("@TimeZone", string.IsNullOrWhiteSpace(EditableEmployee.TimeZone) ? DBNull.Value : (object)EditableEmployee.TimeZone.Trim());
                command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Profile updated successfully!", "Success",
                                    MessageBoxButton.OK, MessageBoxImage.Information);

                    // Update the original ViewModel object that Profile.xaml is bound to,
                    // so the Profile page reflects changes when navigated back.
                    originalEmployeeDataViewModel.FullName = EditableEmployee.FullName;
                    originalEmployeeDataViewModel.Gender = EditableEmployee.Gender;
                    originalEmployeeDataViewModel.DateOfBirth = EditableEmployee.DateOfBirth;
                    originalEmployeeDataViewModel.Nationality = EditableEmployee.Nationality;
                    originalEmployeeDataViewModel.Email = EditableEmployee.Email;
                    originalEmployeeDataViewModel.PhoneNumber = EditableEmployee.PhoneNumber;
                    originalEmployeeDataViewModel.Address = EditableEmployee.Address;
                    originalEmployeeDataViewModel.LoginNotificationsEnabled = EditableEmployee.LoginNotificationsEnabled;
                    originalEmployeeDataViewModel.LanguagePreference = EditableEmployee.LanguagePreference;
                    originalEmployeeDataViewModel.TimeZone = EditableEmployee.TimeZone;
                    // Note: Properties like Role, HireDate, LastLogin, ManagingAdminName, Status, EmployeeId
                    // are not being updated here as they were treated as read-only or managed elsewhere.

                    NavigateBackToProfile();
                }
                else
                {
                    MessageBox.Show("Profile could not be updated. Either no changes were made or the employee was not found.", "Update Information",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (MySqlException myEx)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] MySQL Error updating employee profile: {myEx.ToString()}");
                MessageBox.Show($"Database error during update: {myEx.Message} (Code: {myEx.Number})", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Error updating employee profile: {ex.ToString()}");
                MessageBox.Show($"An error occurred while updating profile: {ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            NavigateBackToProfile();
        }

        private void NavigateBackToProfile()
        {
            if (this.NavigationService is { CanGoBack: true }) // Null check and CanGoBack simplified
            {
                this.NavigationService.GoBack();
                // The Profile page should ideally refresh itself if its DataContext (CurrentEmployee)
                // was the same instance passed to this edit page (originalEmployeeDataViewModel)
                // and that instance had its properties updated.
            }
            else
            {
                // Fallback navigation if GoBack is not possible or if the main window manages navigation
                // Ensure 'MainWindow' and 'MainContentFrame' are correct for your application structure
                if (Application.Current.MainWindow is MainWindow mainWindow && mainWindow.MainContentFrame is not null)
                {
                    // Re-create the profile page instance; it will load fresh data for the employee.
                    // This assumes _loadedEmployeeId was correctly set in the original Profile page,
                    // and originalEmployeeDataViewModel.EmployeeId holds that ID.
                    Profile profilePageInstance = new(originalEmployeeDataViewModel.EmployeeId); // 'new' expression simplified
                    mainWindow.MainContentFrame.Navigate(profilePageInstance);
                }
                else
                {
                    Debug.WriteLine("NavigateBackToProfile: Could not determine how to navigate back. NavigationService is null or MainWindow/MainContentFrame not found.");
                }
            }
        }
    }
}
