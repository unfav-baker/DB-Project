using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace Adminn
{
    public partial class AddProduct : Page
    {
        // Replace this with your actual MySQL connection string
        private readonly string connectionString = "Server=127.0.0.1;Port=3306;Database=prime_tech;Uid=root;Pwd=Abubaker85@@;";

        public AddProduct()
        {
            InitializeComponent();
            LoadSuppliers();
        }

        private void LoadSuppliers()
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = "SELECT Supplier_ID, S_Name FROM supplier WHERE S_Status = 'Active' ORDER BY S_Name";

                using var command = new MySqlCommand(query, connection);
                using var reader = command.ExecuteReader();

                var suppliers = new List<SupplierItem>();

                while (reader.Read())
                {
                    suppliers.Add(new SupplierItem
                    {
                        SupplierId = reader.GetInt32("Supplier_ID"),
                        DisplayText = $"{reader.GetInt32("Supplier_ID")} - {reader.GetString("S_Name")}"
                    });
                }

                cmbSupplierId.ItemsSource = suppliers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading suppliers: {ex.Message}", "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void SaveProduct_Click(object sender, RoutedEventArgs e)
        {
            // Validate input fields
            if (cmbSupplierId.SelectedItem == null)
            {
                MessageBox.Show("Please select a supplier.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbSupplierId.Focus();
                return;
            }

            if (cmbCategory.SelectedItem == null)
            {
                MessageBox.Show("Please select a category.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbCategory.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtQuantity.Text) || !int.TryParse(txtQuantity.Text, out int quantity) || quantity < 0)
            {
                MessageBox.Show("Please enter a valid quantity (non-negative number).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtQuantity.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPrice.Text) || !decimal.TryParse(txtPrice.Text, out decimal price) || price < 0)
            {
                MessageBox.Show("Please enter a valid price (non-negative number).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPrice.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtBatchNo.Text))
            {
                MessageBox.Show("Please enter batch number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtBatchNo.Focus();
                return;
            }

            if (cmbStatus.SelectedItem == null)
            {
                MessageBox.Show("Please select a status.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbStatus.Focus();
                return;
            }

            // Save product to database
            SaveProductToDatabase();
        }

        private void SaveProductToDatabase()
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = @"INSERT INTO products (Supplier_ID, Category, Quantity, Price, Batch_No, Status, Created_At) 
                               VALUES (@SupplierId, @Category, @Quantity, @Price, @BatchNo, @Status, @CreatedAt)";

                using var command = new MySqlCommand(query, connection);

                // Add parameters
                var selectedSupplier = (SupplierItem)cmbSupplierId.SelectedItem;
                command.Parameters.AddWithValue("@SupplierId", selectedSupplier.SupplierId);
                command.Parameters.AddWithValue("@Category", ((ComboBoxItem)cmbCategory.SelectedItem).Content.ToString());
                command.Parameters.AddWithValue("@Quantity", int.Parse(txtQuantity.Text));
                command.Parameters.AddWithValue("@Price", decimal.Parse(txtPrice.Text));
                command.Parameters.AddWithValue("@BatchNo", txtBatchNo.Text.Trim());
                command.Parameters.AddWithValue("@Status", ((ComboBoxItem)cmbStatus.SelectedItem).Content.ToString());
                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Product added successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Clear form fields
                    ClearForm();

                    // Navigate back to products page and refresh data
                    NavigateBackToProductsPage();
                }
                else
                {
                    MessageBox.Show("Failed to add product. Please try again.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062) // Duplicate entry error
            {
                MessageBox.Show("A product with this batch number already exists.", "Duplicate Entry",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding product: {ex.Message}", "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to cancel? All unsaved changes will be lost.",
                "Confirm Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                NavigateBackToProductsPage();
            }
        }

        private void ClearForm()
        {
            cmbSupplierId.SelectedIndex = -1;
            cmbCategory.SelectedIndex = -1;
            txtQuantity.Clear();
            txtPrice.Clear();
            txtBatchNo.Clear();
            cmbStatus.SelectedIndex = 0; // Set to "In Stock" by default
        }

        private static void NavigateBackToProductsPage()
        {
            try
            {
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    // Create a new instance of the Products page
                    var productsPage = new Products();
                    mainWindow.MainContentFrame.Navigate(productsPage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating back to products page: {ex.Message}", "Navigation Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class SupplierItem
    {
        public int SupplierId { get; set; }
        public string DisplayText { get; set; }
    }
}