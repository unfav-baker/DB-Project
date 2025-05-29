using System;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Text.RegularExpressions; // For Regex
using System.Windows.Input;      // For TextCompositionEventArgs

namespace Adminn
{
    public partial class AddSupplier : Page
    {
        private readonly string? connectionString;
        private const string DbConnectionStringEnvVar = "PRIMETECH_DB_CONN_STRING";

        public AddSupplier()
        {
            InitializeComponent();

            connectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVar);

            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show($"Database connection string environment variable '{DbConnectionStringEnvVar}' was not found or is empty. " +
                                $"Please ensure your .env file is correctly set up and loaded at application startup (App.xaml.cs).\n\n" +
                                "Cannot save new suppliers.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // Assuming your save button in XAML is named 'btnSave'
                if (this.FindName("btnSave") is Button saveButton) saveButton.IsEnabled = false;
            }

            if (cmbStatus.Items.Count > 0)
            {
                cmbStatus.SelectedIndex = 0;
            }
            txtName.Focus();
        }

        private void SaveSupplier_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Database connection not configured. Cannot save supplier.", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            // Validation
            if (string.IsNullOrWhiteSpace(txtName.Text)) { ShowValidationError("Supplier Name cannot be empty.", txtName); return; }
            if (string.IsNullOrWhiteSpace(txtPhoneNumber.Text)) { ShowValidationError("Phone Number cannot be empty.", txtPhoneNumber); return; }
            if (cmbRole.SelectedItem == null) { ShowValidationError("Please select a Role for the supplier.", cmbRole); return; }
            if (cmbStatus.SelectedItem == null) { ShowValidationError("Please select a Status for the supplier.", cmbStatus); return; }

            // Validate Supplier Rating if the TextBox exists and is not empty
            decimal? supplierRating = null;
            if (this.FindName("txtSupplierRating") is TextBox ratingBox && !string.IsNullOrWhiteSpace(ratingBox.Text))
            {
                if (decimal.TryParse(ratingBox.Text, out decimal ratingValue) && ratingValue >= 0 && ratingValue <= 5) // Assuming rating is 0-5
                {
                    supplierRating = ratingValue;
                }
                else
                {
                    ShowValidationError("Please enter a valid rating (e.g., 0.0 to 5.0).", ratingBox);
                    return;
                }
            }


            try
            {
                using MySqlConnection connection = new(connectionString);
                connection.Open();

                string query = @"INSERT INTO supplier 
                                (Name, Phone_Number, Role, Plant_Name, Status, Email, Address, BRN_TAX_ID, Supplier_Rating, FK_Admin_ID, Username, Password, Created_At, Updated_At) 
                             VALUES 
                                (@Name, @PhoneNumber, @Role, @PlantName, @Status, @Email, @Address, @BrnTaxId, @SupplierRating, @FkAdminId, @Username, @Password, @CreatedAt, @UpdatedAt);
                             SELECT LAST_INSERT_ID();";

                using MySqlCommand command = new(query, connection);

                command.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                command.Parameters.AddWithValue("@PhoneNumber", txtPhoneNumber.Text.Trim());
                command.Parameters.AddWithValue("@Role", ((ComboBoxItem)cmbRole.SelectedItem).Content.ToString());
                command.Parameters.AddWithValue("@PlantName", string.IsNullOrWhiteSpace(txtPlantName.Text) ? DBNull.Value : (object)txtPlantName.Text.Trim());
                command.Parameters.AddWithValue("@Status", ((ComboBoxItem)cmbStatus.SelectedItem).Content.ToString());

                // Handle optional fields safely
                command.Parameters.AddWithValue("@Email", string.IsNullOrWhiteSpace(txtEmail?.Text) ? DBNull.Value : (object)txtEmail.Text.Trim());
                command.Parameters.AddWithValue("@Address", string.IsNullOrWhiteSpace(txtAddress?.Text) ? DBNull.Value : (object)txtAddress.Text.Trim());
                command.Parameters.AddWithValue("@BrnTaxId", string.IsNullOrWhiteSpace(txtBrnTaxId?.Text) ? DBNull.Value : (object)txtBrnTaxId.Text.Trim());
                command.Parameters.AddWithValue("@SupplierRating", supplierRating as object ?? DBNull.Value);
                command.Parameters.AddWithValue("@FkAdminId", DBNull.Value); // Placeholder - set actual Admin ID if available
                command.Parameters.AddWithValue("@Username", string.IsNullOrWhiteSpace(txtUsername?.Text) ? DBNull.Value : (object)txtUsername.Text.Trim());
                command.Parameters.AddWithValue("@Password", string.IsNullOrWhiteSpace(txtPassword?.Password) ? DBNull.Value : (object)txtPassword.Password); // HASH THIS!

                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                object? result = command.ExecuteScalar();
                int newSupplierId = 0;
                if (result != null && result != DBNull.Value)
                {
                    newSupplierId = Convert.ToInt32(result);
                }

                if (newSupplierId > 0)
                {
                    MessageBox.Show($"Supplier '{txtName.Text.Trim()}' added successfully! Supplier ID: {newSupplierId}", "Success",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    NavigateToSupplierPage();
                }
                else
                {
                    MessageBox.Show("Failed to add supplier or retrieve new ID.", "Operation Status",
                                    MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (MySqlException myEx)
            {
                Debug.WriteLine($"MySQL Error saving supplier: {myEx.ToString()}");
                if (myEx.Number == 1062)
                {
                    MessageBox.Show("A supplier with this Email, Username, or BRN/TAX ID already exists.", "Duplicate Entry", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else if (myEx.Number == 1054)
                {
                    MessageBox.Show($"Database Error (MySQL): {myEx.Message}. Check column names.", "Database Column Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
                else
                {
                    MessageBox.Show($"Database Error (MySQL): {myEx.Message} (Code: {myEx.Number})", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Generic error saving supplier: {ex.ToString()}");
                MessageBox.Show($"Error saving supplier: {ex.Message}", "Application Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to clear the form? All entered data will be lost.",
                                         "Confirm Clear", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ClearForm();
            }
        }

        private void ClosePage_Click(object sender, RoutedEventArgs e)
        {
            NavigateToSupplierPage();
        }

        private void ClearForm()
        {
            txtName.Clear();
            txtPhoneNumber.Clear();
            txtPlantName.Clear();
            if (cmbRole.Items.Count > 0) cmbRole.SelectedIndex = -1;
            if (cmbStatus.Items.Count > 0) cmbStatus.SelectedIndex = 0;

            if (this.FindName("txtEmail") is TextBox emailBox) emailBox.Clear();
            if (this.FindName("txtAddress") is TextBox addressBox) addressBox.Clear();
            if (this.FindName("txtBrnTaxId") is TextBox brnTaxIdBox) brnTaxIdBox.Clear();
            if (this.FindName("txtSupplierRating") is TextBox ratingBox) ratingBox.Clear();
            if (this.FindName("txtUsername") is TextBox usernameBox) usernameBox.Clear();
            if (this.FindName("txtPassword") is PasswordBox passwordBox) passwordBox.Clear();

            txtName.Focus();
        }

        private void NavigateToSupplierPage()
        {
            try
            {
                if (Application.Current.MainWindow is MainWindow mainWindow && mainWindow.MainContentFrame != null)
                {
                    Supplier supplierPage = new Supplier();
                    mainWindow.MainContentFrame.Navigate(supplierPage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to Supplier page: {ex.Message}", "Navigation Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void ShowValidationError(string message, Control controlToFocus)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            if (controlToFocus != null)
            {
                controlToFocus.Focus();
            }
        }

        // Method to allow only decimal input (numbers and one decimal point, up to 2 decimal places)
        private void DecimalOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                string currentText = textBox.Text;
                int caretIndex = textBox.CaretIndex;
                string newText = currentText.Insert(caretIndex, e.Text);

                // Allows numbers, one optional decimal point, and up to 2 decimal places.
                // Also allows an empty string or just "." to start typing a decimal.
                if (!Regex.IsMatch(newText, @"^[0-9]*(\.[0-9]{0,2})?$") && newText != "." && !string.IsNullOrEmpty(newText))
                {
                    e.Handled = true; // Block the input if it doesn't match the pattern
                }
            }
            else
            {
                e.Handled = true; // If sender is not a TextBox, block input
            }
        }
    }
}
