using System;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Text.RegularExpressions; // For email validation

namespace Adminn
{
    public partial class AddCustomer : Page
    {
        private readonly string? connectionString;
        private const string DbConnectionStringEnvVar = "PRIMETECH_DB_CONN_STRING";

        public AddCustomer()
        {
            InitializeComponent();
            connectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVar);

            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show($"Database connection string not configured. Cannot save customer.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // Assuming your save button in XAML is named 'btnSave'
                if (this.FindName("btnSave") is Button saveButton) saveButton.IsEnabled = false;
            }
            // Set default values for ComboBoxes or other controls if needed
            // Example: if you add cmbCustomerType or cmbStatus here
            // if(cmbStatus.Items.Count > 0) cmbStatus.SelectedIndex = 0; // Default to "Active"
            PartyNameTextBox.Focus(); // Set focus to the first input field
        }

        private void SaveCustomer_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Database connection not configured.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validation for customer fields
            if (string.IsNullOrWhiteSpace(PartyNameTextBox.Text)) { ShowValidationError("Party Name is required.", PartyNameTextBox); return; }
            if (string.IsNullOrWhiteSpace(txtPhoneNumber.Text)) { ShowValidationError("Phone Number is required.", txtPhoneNumber); return; } // Assuming x:Name="txtPhoneNumber"
            if (string.IsNullOrWhiteSpace(txtEmail.Text) || !IsValidEmail(txtEmail.Text.Trim())) { ShowValidationError("Valid Email is required.", txtEmail); return; } // Assuming x:Name="txtEmail"
            // Add other validations for Address, CustomerType, BusinessName, Status as needed

            try
            {
                using MySqlConnection connection = new(connectionString);
                connection.Open();

                // Query to insert into 'customer' table
                string query = @"INSERT INTO customer 
                                 (Name, Phone_Number, Email, Address, Customer_Type, Business_Name, Status, FK_Admin_ID, Created_At, Updated_At) 
                               VALUES 
                                 (@Name, @PhoneNumber, @Email, @Address, @CustomerType, @BusinessName, @Status, @AdminId, @CreatedAt, @UpdatedAt);
                               SELECT LAST_INSERT_ID();";

                using MySqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@Name", PartyNameTextBox.Text.Trim());
                command.Parameters.AddWithValue("@PhoneNumber", txtPhoneNumber.Text.Trim()); // Use x:Name from XAML
                command.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());             // Use x:Name from XAML
                command.Parameters.AddWithValue("@Address", string.IsNullOrWhiteSpace(txtAddress.Text) ? DBNull.Value : (object)txtAddress.Text.Trim()); // Use x:Name
                command.Parameters.AddWithValue("@CustomerType", (cmbCustomerType.SelectedItem as ComboBoxItem)?.Content?.ToString()); // Use x:Name
                command.Parameters.AddWithValue("@BusinessName", string.IsNullOrWhiteSpace(txtBusinessName.Text) ? DBNull.Value : (object)txtBusinessName.Text.Trim()); // Use x:Name
                command.Parameters.AddWithValue("@Status", (cmbStatus.SelectedItem as ComboBoxItem)?.Content?.ToString() ?? "Active"); // Use x:Name
                command.Parameters.AddWithValue("@AdminId", 1); // Placeholder for logged-in Admin ID or null if not applicable
                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                // Removed parameters for fields not in 'customer' table like:
                // DatePicker, PhytoNumberTextBox, ExportThroughTextBox, PlantTextBox, ImporterTextBox,
                // CartonTextBox, WeightTextBox, RateTextBox, AmountTextBox, ReceivedTextBox

                object? result = command.ExecuteScalar();
                int newCustomerId = 0;
                if (result is not (null or DBNull))
                {
                    newCustomerId = Convert.ToInt32(result);
                }

                if (newCustomerId > 0)
                {
                    MessageBox.Show($"Customer '{PartyNameTextBox.Text.Trim()}' added successfully! Customer ID: {newCustomerId}", "Success",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    NavigateToCustomerPage();
                }
                else
                {
                    MessageBox.Show("Failed to add customer or retrieve ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (MySqlException myEx)
            {
                Debug.WriteLine($"MySQL Error saving customer: {myEx.ToString()}");
                if (myEx.Number == 1062) // Duplicate entry for unique key (e.g., Email)
                {
                    MessageBox.Show("A customer with this Email or other unique information already exists.", "Duplicate Entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    MessageBox.Show($"Database Error (MySQL): {myEx.Message} (Code: {myEx.Number})", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Generic error saving customer: {ex.ToString()}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            PartyNameTextBox.Text = string.Empty;
            txtPhoneNumber.Text = string.Empty; // Use x:Name from XAML
            txtEmail.Text = string.Empty;       // Use x:Name from XAML
            txtAddress.Text = string.Empty;     // Use x:Name from XAML
            if (cmbCustomerType.Items.Count > 0) cmbCustomerType.SelectedIndex = -1; // Use x:Name
            txtBusinessName.Text = string.Empty;  // Use x:Name
            if (cmbStatus.Items.Count > 0) cmbStatus.SelectedIndex = 0; // Default to first item e.g. "Active"

            // Removed fields that were for 'orders' table
            // DatePicker.SelectedDate = DateTime.Today;
            // PhytoNumberTextBox.Text = string.Empty;
            // ExportThroughTextBox.Text = string.Empty;
            // PlantTextBox.Text = string.Empty;
            // ImporterTextBox.Text = string.Empty;
            // CartonTextBox.Text = string.Empty;
            // WeightTextBox.Text = string.Empty;
            // RateTextBox.Text = string.Empty;
            // AmountTextBox.Text = string.Empty;
            // ReceivedTextBox.Text = string.Empty;

            PartyNameTextBox.Focus();
        }

        private void NavigateToCustomerPage()
        {
            try
            {
                if (Application.Current.MainWindow is MainWindow mainWindow && mainWindow.MainContentFrame is not null)
                {
                    Customer customerPage = new();
                    mainWindow.MainContentFrame.Navigate(customerPage);
                }
            }
            catch (Exception ex) { MessageBox.Show($"Navigation Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e) // This is your "Clear Form" button
        {
            var result = MessageBox.Show("Are you sure you want to clear the form? All entered data will be lost.",
                                         "Confirm Clear", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ClearForm();
            }
        }

        private void ClosePage_Click(object sender, RoutedEventArgs e) // For the 'X' button in header
        {
            NavigateToCustomerPage(); // Or this.NavigationService.GoBack();
        }

        private static void ShowValidationError(string message, Control controlToFocus)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            if (controlToFocus is not null) controlToFocus.Focus();
        }
        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try { return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$", RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250)); }
            catch (RegexMatchTimeoutException) { return false; }
        }
    }
}
