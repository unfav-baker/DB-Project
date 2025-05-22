using System;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace Adminn
{
    public partial class AddEmployee : Page
    {
        // Use the same connection string as your Employee page
        private readonly string connectionString = "Server=127.0.0.1;Port=3306;Database=prime_tech;Uid=root;Pwd=Abubaker85@@;";

        public AddEmployee()
        {
            InitializeComponent();

            // Set default values
            cmbStatus.SelectedIndex = 0; // Active
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                ShowValidationError("Please enter employee name.", txtName);
                return;
            }

            if (cmbRole.SelectedItem == null)
            {
                ShowValidationError("Please select a role.", cmbRole);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
            {
                ShowValidationError("Please enter phone number.", txtPhoneNumber);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSalary.Text))
            {
                ShowValidationError("Please enter salary.", txtSalary);
                return;
            }

            // Validate salary is numeric
            if (!decimal.TryParse(txtSalary.Text, out decimal salary) || salary <= 0)
            {
                ShowValidationError("Please enter a valid salary amount.", txtSalary);
                return;
            }

            // Validate phone number format (basic validation)
            if (txtPhoneNumber.Text.Trim().Length < 10)
            {
                ShowValidationError("Please enter a valid phone number (at least 10 digits).", txtPhoneNumber);
                return;
            }

            try
            {
                // Save employee to database
                int newEmployeeId = SaveEmployeeToDatabase();

                if (newEmployeeId > 0)
                {
                    MessageBox.Show($"Employee added successfully! Employee ID: {newEmployeeId}", "Success",
                                   MessageBoxButton.OK, MessageBoxImage.Information);

                    // Clear form after successful save
                    ClearForm();

                    // Navigate back to Employee list page and refresh data
                    NavigateBackAndRefresh();
                }
                else
                {
                    MessageBox.Show("Failed to add employee. Please try again.", "Error",
                                   MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving employee: {ex.Message}", "Database Error",
                               MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int SaveEmployeeToDatabase()
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            // Insert new employee into database
            string query = @"INSERT INTO employees (Name, Role, PhoneNumber, Salary, Status, CreatedAt, UpdatedAt) 
                           VALUES (@Name, @Role, @PhoneNumber, @Salary, @Status, @CreatedAt, @UpdatedAt);
                           SELECT LAST_INSERT_ID();";

            using var command = new MySqlCommand(query, connection);

            // Add parameters
            command.Parameters.AddWithValue("@Name", txtName.Text.Trim());
            command.Parameters.AddWithValue("@Role", ((ComboBoxItem)cmbRole.SelectedItem).Content.ToString());
            command.Parameters.AddWithValue("@PhoneNumber", txtPhoneNumber.Text.Trim());
            command.Parameters.AddWithValue("@Salary", decimal.Parse(txtSalary.Text));
            command.Parameters.AddWithValue("@Status", ((ComboBoxItem)cmbStatus.SelectedItem).Content.ToString());
            command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

            // Execute and get the new employee ID
            var result = command.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        private void ShowValidationError(string message, Control controlToFocus)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            controlToFocus.Focus();
        }

        private void NavigateBackAndRefresh()
        {
            try
            {
                // Find the main window and navigate back to employee page
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    // Create a new Employee page instance to refresh data
                    var employeePage = new Employee();
                    mainWindow.MainContentFrame.Navigate(employeePage);
                }
                else if (NavigationService.CanGoBack)
                {
                    NavigationService.GoBack();

                    // Try to refresh the employee page if possible
                    if (NavigationService.Content is Employee employeePage)
                    {
                        employeePage.RefreshData();
                    }
                }
            }
            catch (Exception ex)
            {
                // If navigation fails, just show a message
                MessageBox.Show("Employee added successfully! Please refresh the employee list manually.",
                    "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            // Ask for confirmation before canceling
            var result = MessageBox.Show("Are you sure you want to cancel? All entered data will be lost.",
                                        "Confirm Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                // Navigate back to Employee list page
                try
                {
                    if (Application.Current.MainWindow is MainWindow mainWindow)
                    {
                        var employeePage = new Employee();
                        mainWindow.MainContentFrame.Navigate(employeePage);
                    }
                    else if (NavigationService.CanGoBack)
                    {
                        NavigationService.GoBack();
                    }
                    else
                    {
                        // If can't go back, clear the form
                        ClearForm();
                    }
                }
                catch
                {
                    ClearForm();
                }
            }
        }

        private void ClearForm()
        {
            txtName.Text = string.Empty;
            cmbRole.SelectedIndex = -1;
            txtPhoneNumber.Text = string.Empty;
            txtSalary.Text = string.Empty;
            cmbStatus.SelectedIndex = 0; // Active

            // Focus on the first field
            txtName.Focus();
        }

        // Real-time validation for phone number
        private void txtPhoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // Remove any non-digit characters except +
                string text = textBox.Text;
                string cleaned = "";
                foreach (char c in text)
                {
                    if (char.IsDigit(c) || c == '+')
                    {
                        cleaned += c;
                    }
                }

                if (text != cleaned)
                {
                    textBox.Text = cleaned;
                    textBox.CaretIndex = cleaned.Length;
                }
            }
        }

        // Real-time validation for salary input
        private void txtSalary_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                // Only allow digits and decimal point
                string text = textBox.Text;
                string cleaned = "";
                bool hasDecimal = false;

                foreach (char c in text)
                {
                    if (char.IsDigit(c))
                    {
                        cleaned += c;
                    }
                    else if (c == '.' && !hasDecimal)
                    {
                        cleaned += c;
                        hasDecimal = true;
                    }
                }

                if (text != cleaned)
                {
                    textBox.Text = cleaned;
                    textBox.CaretIndex = cleaned.Length;
                }
            }
        }

        // Optional: Add these event handlers to the XAML if you want real-time validation
        // TextChanged="txtPhoneNumber_TextChanged" for phone number TextBox
        // TextChanged="txtSalary_TextChanged" for salary TextBox
    }
}