using System;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient; // Ensure you have this NuGet package (MySql.Data)

namespace Adminn
{
    public partial class AddEmployee : Page
    {
        private readonly string connectionString = "Server=127.0.0.1;Port=3306;Database=prime_tech;Uid=root;Pwd=Abubaker85@@;";

        public AddEmployee()
        {
            InitializeComponent();
            cmbStatus.SelectedIndex = 0; // Default to "Active"
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
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
            if (txtPhoneNumber.Text.Trim().Length < 10) // Basic phone length validation
            {
                ShowValidationError("Please enter a valid phone number (at least 10 digits).", txtPhoneNumber);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtSalary.Text))
            {
                ShowValidationError("Please enter salary.", txtSalary);
                return;
            }
            if (!decimal.TryParse(txtSalary.Text, out decimal salary) || salary <= 0)
            {
                ShowValidationError("Please enter a valid salary amount.", txtSalary);
                return;
            }

            try
            {
                int newEmployeeId = SaveEmployeeToDatabase();
                if (newEmployeeId > 0)
                {
                    MessageBox.Show($"Employee added successfully! Employee ID: {newEmployeeId}", "Success",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    NavigateToEmployeePage(); // Navigate to employee list after successful save
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

            string query = @"INSERT INTO employees (Name, Role, PhoneNumber, Salary, Status, CreatedAt, UpdatedAt) 
                             VALUES (@Name, @Role, @PhoneNumber, @Salary, @Status, @CreatedAt, @UpdatedAt);
                             SELECT LAST_INSERT_ID();";

            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@Name", txtName.Text.Trim());
            command.Parameters.AddWithValue("@Role", ((ComboBoxItem)cmbRole.SelectedItem).Content.ToString());
            command.Parameters.AddWithValue("@PhoneNumber", txtPhoneNumber.Text.Trim());
            command.Parameters.AddWithValue("@Salary", decimal.Parse(txtSalary.Text));
            command.Parameters.AddWithValue("@Status", ((ComboBoxItem)cmbStatus.SelectedItem).Content.ToString());
            command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

            var result = command.ExecuteScalar();
            return Convert.ToInt32(result);
        }

        // Renamed from NavigateBackAndRefresh for clarity
        private void NavigateToEmployeePage()
        {
            try
            {
                if (Application.Current.MainWindow is MainWindow mainWindow && mainWindow.MainContentFrame != null)
                {
                    var employeePage = new Employee(); // Create new instance to show fresh list
                    mainWindow.MainContentFrame.Navigate(employeePage);
                }
                // Removed the NavigationService.GoBack() part as we are always navigating to a new Employee page
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Navigation Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // MODIFIED: btnCancel_Click now only clears the form
        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to clear the form? All entered data will be lost.",
                                         "Confirm Clear", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ClearForm();
            }
        }

        // NEW: Event handler for the Close button in the header
        private void ClosePage_Click(object sender, RoutedEventArgs e)
        {
            NavigateToEmployeePage();
        }

        private void ClearForm()
        {
            txtName.Text = string.Empty;
            cmbRole.SelectedIndex = -1; // No role selected
            txtPhoneNumber.Text = string.Empty;
            txtSalary.Text = string.Empty;
            cmbStatus.SelectedIndex = 0; // Default to "Active"
            txtName.Focus();
        }

        private static void ShowValidationError(string message, Control controlToFocus) // Made static
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            controlToFocus.Focus();
        }

        private void txtPhoneNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                string text = textBox.Text;
                string cleaned = "";
                bool firstChar = true;
                foreach (char c in text)
                {
                    if (char.IsDigit(c) || (c == '+' && firstChar && cleaned.Length == 0))
                    {
                        cleaned += c;
                    }
                    if (cleaned.Length > 0) firstChar = false; // Allow '+' only at the beginning
                }
                if (text != cleaned)
                {
                    textBox.Text = cleaned;
                    textBox.CaretIndex = cleaned.Length;
                }
            }
        }

        private void txtSalary_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                string text = textBox.Text;
                string cleaned = "";
                bool hasDecimal = false;
                foreach (char c in text)
                {
                    if (char.IsDigit(c))
                    {
                        cleaned += c;
                    }
                    else if (c == '.' && !hasDecimal && cleaned.Length > 0) // Allow decimal if not present and after at least one digit
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
    }
}