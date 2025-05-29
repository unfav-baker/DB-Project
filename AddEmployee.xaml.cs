using System;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace Adminn
{
    public partial class AddEmployee : Page
    {
        private readonly string? connectionString;
        private const string DbConnectionStringEnvVar = "PRIMETECH_DB_CONN_STRING";

        public AddEmployee()
        {
            InitializeComponent();

            connectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVar);

            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show($"Database connection string environment variable '{DbConnectionStringEnvVar}' was not found or is empty. " +
                                $"Please ensure your .env file is correctly set up and loaded at application startup (App.xaml.cs).\n\n" +
                                "Cannot save new employees.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                btnSave.IsEnabled = false;
            }

            if (cmbStatus.Items.Count > 0)
            {
                cmbStatus.SelectedIndex = 0;
            }
            if (cmbGender.Items.Count > 0) cmbGender.SelectedIndex = 0;
            txtName.Focus();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Database connection not configured. Cannot save employee.", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // --- Validation ---
            if (string.IsNullOrWhiteSpace(txtName.Text)) { ShowValidationError("Please enter employee's full name.", txtName); return; }
            if (cmbRole.SelectedItem == null || string.IsNullOrWhiteSpace((cmbRole.SelectedItem as ComboBoxItem)?.Content?.ToString())) { ShowValidationError("Please select a role.", cmbRole); return; }
            if (string.IsNullOrWhiteSpace(txtUsername.Text)) { ShowValidationError("Please enter a username.", txtUsername); return; }
            if (string.IsNullOrWhiteSpace(txtPassword.Password)) { ShowValidationError("Please enter a password.", txtPassword); return; }
            if (string.IsNullOrWhiteSpace(txtEmail.Text)) { ShowValidationError("Please enter an email address.", txtEmail); return; }
            if (!IsValidEmail(txtEmail.Text.Trim())) { ShowValidationError("Please enter a valid email address.", txtEmail); return; }
            if (string.IsNullOrWhiteSpace(txtPhoneNumber.Text)) { ShowValidationError("Please enter phone number.", txtPhoneNumber); return; }
            if (txtPhoneNumber.Text.Trim().Length < 7) { ShowValidationError("Please enter a valid phone number (at least 7 digits).", txtPhoneNumber); return; }
            if (string.IsNullOrWhiteSpace(txtSalary.Text)) { ShowValidationError("Please enter salary.", txtSalary); return; }
            if (!decimal.TryParse(txtSalary.Text, out decimal salary) || salary < 0) { ShowValidationError("Please enter a valid non-negative salary amount.", txtSalary); return; }
            if (cmbStatus.SelectedItem == null || string.IsNullOrWhiteSpace((cmbStatus.SelectedItem as ComboBoxItem)?.Content?.ToString())) { ShowValidationError("Please select a status.", cmbStatus); return; }
            if (cmbGender.SelectedItem == null || string.IsNullOrWhiteSpace((cmbGender.SelectedItem as ComboBoxItem)?.Content?.ToString())) { ShowValidationError("Please select a gender.", cmbGender); return; }
            // --- End of Validation ---

            try
            {
                int newEmployeeId = SaveEmployeeToDatabase(salary);
                if (newEmployeeId > 0)
                {
                    MessageBox.Show($"Employee '{txtName.Text.Trim()}' added successfully! Employee ID: {newEmployeeId}", "Success",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    NavigateToEmployeePage();
                }
                else
                {
                    MessageBox.Show("Failed to add employee or retrieve new ID. The employee might have been saved without returning an ID, or the save failed.", "Operation Status",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (MySqlException myEx)
            {
                Debug.WriteLine($"MySQL Error saving employee: {myEx.ToString()}");
                if (myEx.Number == 1062)
                {
                    MessageBox.Show("An employee with this Username or Email already exists. Please use unique values.", "Duplicate Entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (myEx.Number == 1054) // Unknown column
                {
                    MessageBox.Show($"Database Error (MySQL): {myEx.Message}. Please check if all column names in the code match the database table schema for 'employee'.", "Database Column Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"Database Error (MySQL): {myEx.Message} (Code: {myEx.Number})", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Generic error saving employee: {ex.ToString()}");
                MessageBox.Show($"Error saving employee: {ex.Message}", "Application Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int SaveEmployeeToDatabase(decimal salaryAmount)
        {
            using MySqlConnection connection = new(connectionString);
            connection.Open();

            // CORRECTED: Column names 'Created_At' and 'Updated_At' to match database schema
            // Also ensure table name is 'employee' (singular)
            string query = @"INSERT INTO employee 
                                (Name, Role, Username, Password, Email, Phone_Number, Salary, Status, Gender, Date_Of_Birth, Address, Hire_Date, Created_At, Updated_At) 
                             VALUES 
                                (@Name, @Role, @Username, @Password, @Email, @PhoneNumber, @Salary, @Status, @Gender, @DateOfBirth, @Address, @HireDate, @CreatedAtParam, @UpdatedAtParam);
                             SELECT LAST_INSERT_ID();";
            // Assuming primary key for 'employee' table is AUTO_INCREMENT (e.g., Employee_ID or AdminId)

            using MySqlCommand command = new(query, connection);

            command.Parameters.AddWithValue("@Name", txtName.Text.Trim());
            command.Parameters.AddWithValue("@Role", ((ComboBoxItem)cmbRole.SelectedItem).Content.ToString());
            command.Parameters.AddWithValue("@Username", txtUsername.Text.Trim());
            // IMPORTANT: HASH THE PASSWORD before saving. This is plain text for example only.
            command.Parameters.AddWithValue("@Password", txtPassword.Password);
            command.Parameters.AddWithValue("@Email", txtEmail.Text.Trim());
            command.Parameters.AddWithValue("@PhoneNumber", txtPhoneNumber.Text.Trim());
            command.Parameters.AddWithValue("@Salary", salaryAmount);
            command.Parameters.AddWithValue("@Status", ((ComboBoxItem)cmbStatus.SelectedItem).Content.ToString());
            command.Parameters.AddWithValue("@Gender", ((ComboBoxItem)cmbGender.SelectedItem).Content.ToString());
            command.Parameters.AddWithValue("@DateOfBirth", dpDateOfBirth.SelectedDate as object ?? DBNull.Value);
            command.Parameters.AddWithValue("@Address", string.IsNullOrWhiteSpace(txtAddress.Text) ? DBNull.Value : (object)txtAddress.Text.Trim());
            command.Parameters.AddWithValue("@HireDate", DateTime.Now);
            // CORRECTED: Parameter names to avoid conflict with column names if SQL is picky
            command.Parameters.AddWithValue("@CreatedAtParam", DateTime.Now);
            command.Parameters.AddWithValue("@UpdatedAtParam", DateTime.Now);

            object? result = command.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
            return 0;
        }

        private void NavigateToEmployeePage()
        {
            try
            {
                if (Application.Current.MainWindow is MainWindow mainWindow && mainWindow.MainContentFrame != null)
                {
                    Employee employeePage = new();
                    mainWindow.MainContentFrame.Navigate(employeePage);
                }
            }
            catch (Exception ex) { MessageBox.Show($"Navigation Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to clear the form? All entered data will be lost.",
                                         "Confirm Clear", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) ClearForm();
        }

        private void ClosePage_Click(object sender, RoutedEventArgs e)
        {
            if (this.NavigationService is { CanGoBack: true } nav) nav.GoBack();
            else NavigateToEmployeePage();
        }

        private void ClearForm()
        {
            txtName.Text = string.Empty;
            if (cmbRole.Items.Count > 0) cmbRole.SelectedIndex = -1; // Clear selection
            txtUsername.Text = string.Empty;
            txtPassword.Password = string.Empty;
            txtEmail.Text = string.Empty;
            txtPhoneNumber.Text = string.Empty;
            txtSalary.Text = string.Empty;
            if (cmbStatus.Items.Count > 0) cmbStatus.SelectedIndex = 0;
            if (cmbGender.Items.Count > 0) cmbGender.SelectedIndex = -1; // Clear selection
            dpDateOfBirth.SelectedDate = null;
            txtAddress.Text = string.Empty;
            txtName.Focus();
        }

        private static void ShowValidationError(string message, Control controlToFocus)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            controlToFocus.Focus();
        }

        private bool IsValidEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email)) return false;
            try
            {
                return Regex.IsMatch(email,
                    @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException) { return false; }
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
                    if (cleaned.Length > 0) firstChar = false;
                }
                if (text != cleaned)
                {
                    int caretPosition = textBox.CaretIndex;
                    textBox.Text = cleaned;
                    textBox.CaretIndex = Math.Min(caretPosition, cleaned.Length);
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
                int decimalPlaces = 0;
                foreach (char c in text)
                {
                    if (char.IsDigit(c))
                    {
                        if (hasDecimal) decimalPlaces++;
                        if (!hasDecimal || decimalPlaces <= 2)
                        {
                            cleaned += c;
                        }
                    }
                    else if (c == '.' && !hasDecimal && cleaned.Length > 0)
                    {
                        cleaned += c;
                        hasDecimal = true;
                    }
                }
                if (text != cleaned)
                {
                    int caretPosition = textBox.CaretIndex;
                    textBox.Text = cleaned;
                    textBox.CaretIndex = Math.Min(caretPosition, cleaned.Length);
                }
            }
        }
    }
}
