using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Input;

namespace Adminn
{
    public partial class AddProduct : Page
    {
        private readonly string? connectionString;
        private const string DbConnectionStringEnvVar = "PRIMETECH_DB_CONN_STRING";

        public class SupplierComboBoxItem
        {
            public int SupplierId { get; set; }
            public string DisplayText { get; set; } = string.Empty;
        }

        public AddProduct()
        {
            InitializeComponent();
            connectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVar);

            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show($"Database connection string not configured. Cannot save product.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (this.FindName("btnSave") is Button saveButton) saveButton.IsEnabled = false;
            }
            else
            {
                LoadSuppliersIntoComboBox();
            }

            if (cmbStatus.Items.Count > 0) cmbStatus.SelectedIndex = 0;
        }

        private void LoadSuppliersIntoComboBox()
        {
            if (string.IsNullOrEmpty(connectionString)) return;

            var suppliers = new List<SupplierComboBoxItem>();
            try
            {
                using MySqlConnection connection = new(connectionString);
                connection.Open();
                // Assuming your supplier table is named 'supplier' and has 'Name' and 'Status' columns
                string query = "SELECT Supplier_ID, Name FROM supplier WHERE Status = 'Active' ORDER BY Name";
                using MySqlCommand command = new(query, connection);
                using MySqlDataReader reader = command.ExecuteReader();
                while (reader.Read())
                {
                    suppliers.Add(new SupplierComboBoxItem
                    {
                        SupplierId = reader.GetInt32(reader.GetOrdinal("Supplier_ID")),
                        DisplayText = $"{reader.GetInt32(reader.GetOrdinal("Supplier_ID"))} - {(reader.IsDBNull(reader.GetOrdinal("Name")) ? "N/A" : reader.GetString(reader.GetOrdinal("Name")))}"
                    });
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error loading suppliers for AddProduct: {ex.Message}");
                MessageBox.Show($"Error loading suppliers: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                cmbSupplierId.ItemsSource = suppliers;
                if (suppliers.Count > 0) cmbSupplierId.SelectedIndex = 0;
                else Debug.WriteLine("No active suppliers found to populate ComboBox.");
            }
        }

        private void SaveProduct_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Database connection not configured.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (cmbSupplierId.SelectedItem == null) { ShowValidationError("Please select a Supplier.", cmbSupplierId); return; }
            if (cmbCategory.SelectedItem == null) { ShowValidationError("Please select a Category.", cmbCategory); return; }
            if (string.IsNullOrWhiteSpace(txtQuantity.Text) || !int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
            { ShowValidationError("Please enter a valid non-negative Quantity.", txtQuantity); return; }
            if (string.IsNullOrWhiteSpace(txtPrice.Text) || !decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
            { ShowValidationError("Please enter a valid non-negative Price.", txtPrice); return; }

            decimal? weight = null;
            if (this.FindName("txtWeight") is TextBox weightTextBox && !string.IsNullOrWhiteSpace(weightTextBox.Text))
            {
                if (decimal.TryParse(weightTextBox.Text, out decimal w)) weight = w;
                else { ShowValidationError("Please enter a valid Weight or leave empty.", weightTextBox); return; }
            }
            if (cmbStatus.SelectedItem == null) { ShowValidationError("Please select a Status.", cmbStatus); return; }

            try
            {
                using MySqlConnection connection = new(connectionString);
                connection.Open();

                // CORRECTED: Table name changed to 'Product' (singular)
                // Also included Updated_At based on your Product table schema.
                string query = @"INSERT INTO Product 
                                 (FK_Supplier_ID, Category, Description, Quantity, Price, Batch_No, Weight, Status, Created_At, Updated_At) 
                               VALUES 
                                 (@SupplierId, @Category, @Description, @Quantity, @Price, @BatchNo, @Weight, @Status, @CreatedAt, @UpdatedAt);
                               SELECT LAST_INSERT_ID();";

                using MySqlCommand command = new(query, connection);

                command.Parameters.AddWithValue("@SupplierId", ((SupplierComboBoxItem)cmbSupplierId.SelectedItem).SupplierId);
                command.Parameters.AddWithValue("@Category", ((ComboBoxItem)cmbCategory.SelectedItem).Content.ToString());
                command.Parameters.AddWithValue("@Description", string.IsNullOrWhiteSpace(txtDescription?.Text) ? DBNull.Value : (object)txtDescription.Text.Trim());
                command.Parameters.AddWithValue("@Quantity", quantity);
                command.Parameters.AddWithValue("@Price", price);
                command.Parameters.AddWithValue("@BatchNo", string.IsNullOrWhiteSpace(txtBatchNo.Text) ? DBNull.Value : (object)txtBatchNo.Text.Trim());
                command.Parameters.AddWithValue("@Weight", weight as object ?? DBNull.Value);
                command.Parameters.AddWithValue("@Status", ((ComboBoxItem)cmbStatus.SelectedItem).Content.ToString());
                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now); // Added Updated_At

                object? result = command.ExecuteScalar();
                int newProductId = 0;
                if (result is not (null or DBNull))
                {
                    newProductId = Convert.ToInt32(result);
                }

                if (newProductId > 0)
                {
                    MessageBox.Show($"Product '{((ComboBoxItem)cmbCategory.SelectedItem).Content}' added successfully! Product ID: {newProductId}", "Success",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    NavigateToProductsPage();
                }
                else
                {
                    MessageBox.Show("Failed to add product or retrieve ID.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (MySqlException myEx)
            {
                Debug.WriteLine($"MySQL Error saving product: {myEx.ToString()}");
                MessageBox.Show($"Database Error (MySQL): {myEx.Message} (Code: {myEx.Number})", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Generic error saving product: {ex.ToString()}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearForm()
        {
            if (cmbSupplierId.Items.Count > 0) cmbSupplierId.SelectedIndex = 0; else cmbSupplierId.SelectedIndex = -1;
            if (cmbCategory.Items.Count > 0) cmbCategory.SelectedIndex = -1;
            txtQuantity.Clear();
            txtPrice.Clear();
            txtBatchNo.Clear();
            if (this.FindName("txtDescription") is TextBox descBox) descBox.Clear();
            if (this.FindName("txtWeight") is TextBox weightBox) weightBox.Clear();
            if (cmbStatus.Items.Count > 0) cmbStatus.SelectedIndex = 0;
            if (cmbSupplierId.Items.Count > 0) cmbSupplierId.Focus(); else txtQuantity.Focus();
        }

        private void NavigateToProductsPage()
        {
            try
            {
                if (Application.Current.MainWindow is MainWindow mainWindow && mainWindow.MainContentFrame != null)
                {
                    Products productsPage = new();
                    mainWindow.MainContentFrame.Navigate(productsPage);
                }
            }
            catch (Exception ex) { MessageBox.Show($"Navigation Error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error); }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to clear the form? All entered data will be lost.",
                                         "Confirm Clear", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes) ClearForm();
        }

        private void ClosePage_Click(object sender, RoutedEventArgs e)
        {
            NavigateToProductsPage();
        }

        private static void ShowValidationError(string message, Control controlToFocus)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            if (controlToFocus is not null) controlToFocus.Focus();
        }

        private void NumericOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void DecimalOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            if (sender is TextBox textBox)
            {
                string currentText = textBox.Text;
                int caretIndex = textBox.CaretIndex;
                string newText = currentText.Insert(caretIndex, e.Text);

                if (!Regex.IsMatch(newText, @"^[0-9]*(\.[0-9]{0,2})?$") && newText != "." && !string.IsNullOrEmpty(newText))
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = true;
            }
        }
    }
}
