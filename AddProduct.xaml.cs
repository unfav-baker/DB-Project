using System;
using System.Collections.Generic; // For List<T>
using System.Globalization;       // For CultureInfo in TryParse if needed
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;   // Ensure you have this NuGet package (MySql.Data)

namespace Adminn
{
    public partial class AddProduct : Page
    {
        private readonly string connectionString = "Server=127.0.0.1;Port=3306;Database=prime_tech;Uid=root;Pwd=Abubaker85@@;";

        public AddProduct()
        {
            InitializeComponent();
            LoadSuppliers();
            if (cmbStatus.Items.Count > 0) // Ensure ComboBox is populated
            {
                cmbStatus.SelectedIndex = 0; // Default to "In Stock"
            }
            // Set focus to the first actionable field if cmbSupplierId is populated
            // Otherwise, you might focus on another field like txtQuantity after suppliers load.
            // For now, let cmbSupplierId get focus if it has items.
        }

        private void LoadSuppliers()
        {
            var suppliers = new List<SupplierItem>();
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                // Assuming your supplier table has Supplier_ID and S_Name
                string query = "SELECT Supplier_ID, S_Name FROM supplier WHERE S_Status = 'Active' ORDER BY S_Name";

                using var command = new MySqlCommand(query, connection);
                using var reader = command.ExecuteReader();

                while (reader.Read())
                {
                    suppliers.Add(new SupplierItem
                    {
                        SupplierId = reader.GetInt32("Supplier_ID"),
                        // DisplayText used for ComboBox DisplayMemberPath
                        DisplayText = $"{reader.GetInt32("Supplier_ID")} - {reader.GetString("S_Name")}"
                    });
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading suppliers: {ex.Message}", "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally // Ensure ItemsSource is set even if list is empty after an error during population
            {
                cmbSupplierId.ItemsSource = suppliers;
                if (suppliers.Count > 0)
                {
                    cmbSupplierId.SelectedIndex = 0; // Optionally select first supplier
                }
            }
        }

        private void SaveProduct_Click(object sender, RoutedEventArgs e)
        {
            // Validate input fields
            if (cmbSupplierId.SelectedItem == null)
            {
                ShowValidationError("Please select a supplier.", cmbSupplierId);
                return;
            }
            if (cmbCategory.SelectedItem == null)
            {
                ShowValidationError("Please select a category.", cmbCategory);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtQuantity.Text) || !int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
            {
                ShowValidationError("Please enter a valid quantity (non-negative integer).", txtQuantity);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPrice.Text) || !decimal.TryParse(txtPrice.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out decimal price) || price < 0)
            {
                // Try parsing without currency symbol if first attempt fails (more robust)
                if (!decimal.TryParse(txtPrice.Text, out price) || price < 0)
                {
                    ShowValidationError("Please enter a valid price (non-negative number).", txtPrice);
                    return;
                }
            }
            if (string.IsNullOrWhiteSpace(txtBatchNo.Text)) // Assuming Batch No is required
            {
                ShowValidationError("Please enter the Batch Number.", txtBatchNo);
                return;
            }
            if (cmbStatus.SelectedItem == null)
            {
                ShowValidationError("Please select a status.", cmbStatus);
                return;
            }

            try
            {
                int newProductId = SaveProductToDatabase(quantity, price); // Pass parsed values

                if (newProductId > 0)
                {
                    MessageBox.Show($"Product added successfully! Product ID: {newProductId}", "Success",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    NavigateToProductsPage(); // Navigate back after successful save
                }
                else
                {
                    MessageBox.Show("Failed to add product or retrieve new ID. Please try again.", "Error",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving product: {ex.Message}", "Database Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int SaveProductToDatabase(int quantity, decimal price) // Accept parsed values
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            // Assuming your products table has an auto-increment primary key (e.g., Product_ID)
            // and the column names match.
            string query = @"INSERT INTO products (Supplier_ID, Category, Quantity, Price, Batch_No, Status, Created_At) 
                             VALUES (@SupplierId, @Category, @Quantity, @Price, @BatchNo, @Status, @CreatedAt);
                             SELECT LAST_INSERT_ID();";

            using var command = new MySqlCommand(query, connection);

            var selectedSupplier = (SupplierItem)cmbSupplierId.SelectedItem;
            command.Parameters.AddWithValue("@SupplierId", selectedSupplier.SupplierId);
            command.Parameters.AddWithValue("@Category", ((ComboBoxItem)cmbCategory.SelectedItem).Content.ToString());
            command.Parameters.AddWithValue("@Quantity", quantity); // Use parsed int
            command.Parameters.AddWithValue("@Price", price);       // Use parsed decimal
            command.Parameters.AddWithValue("@BatchNo", txtBatchNo.Text.Trim());
            command.Parameters.AddWithValue("@Status", ((ComboBoxItem)cmbStatus.SelectedItem).Content.ToString());
            command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
            // Note: Your original INSERT query for Products did not include Updated_At, which is fine for new records.

            var result = command.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
            return 0; // Indicate failure
        }

        // MODIFIED: Cancel_Click now only clears the form
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to clear the form? All entered data will be lost.",
                                         "Confirm Clear", MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (result == MessageBoxResult.Yes)
            {
                ClearForm();
            }
        }

        // NEW: Click handler for the Close button in the header
        private void ClosePage_Click(object sender, RoutedEventArgs e)
        {
            NavigateToProductsPage();
        }

        private void ClearForm()
        {
            cmbSupplierId.SelectedIndex = (cmbSupplierId.Items.Count > 0) ? 0 : -1; // Default to first or no selection
            cmbCategory.SelectedIndex = -1;   // No selection
            txtQuantity.Clear();
            txtPrice.Clear();
            txtBatchNo.Clear();
            if (cmbStatus.Items.Count > 0)
            {
                cmbStatus.SelectedIndex = 0; // Default to "In Stock" (first item)
            }
            cmbSupplierId.Focus(); // Set focus to the first input field
        }

        // Renamed for clarity and consistency
        private void NavigateToProductsPage()
        {
            try
            {
                if (Application.Current.MainWindow is MainWindow mainWindow && mainWindow.MainContentFrame != null)
                {
                    // Navigate to a new instance of the Products page to ensure fresh data
                    Products productsPage = new Products(); // Assuming your list page is named Products.xaml
                    mainWindow.MainContentFrame.Navigate(productsPage);
                }
                else if (this.NavigationService != null && this.NavigationService.CanGoBack)
                {
                    this.NavigationService.GoBack(); // Fallback if appropriate
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to Products page: {ex.Message}", "Navigation Error",
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

        // Optional: Add TextChanged event handlers for real-time validation for numeric fields if desired
        // private void TxtQuantity_TextChanged(object sender, TextChangedEventArgs e) { /* Allow only numbers */ }
        // private void TxtPrice_TextChanged(object sender, TextChangedEventArgs e) { /* Allow only numbers and one decimal */ }
    }

    // This class was part of your provided code for populating cmbSupplierId.
    // Ideally, it would be in its own file or a Models folder if used elsewhere.
    public class SupplierItem
    {
        public int SupplierId { get; set; }
        public string? DisplayText { get; set; } // Made DisplayText nullable for safety
    }
}