using System;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace Adminn
{
    public partial class AddSupplier : Page
    {
        // Replace this with your actual MySQL connection string
        private readonly string connectionString = "Server=127.0.0.1;Port=3306;Database=prime_tech;Uid=root;Pwd=Abubaker85@@;";

        public AddSupplier()
        {
            InitializeComponent();
        }

        private void SaveSupplier_Click(object sender, RoutedEventArgs e)
        {
            // Validate input fields
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Please enter supplier name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtName.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
            {
                MessageBox.Show("Please enter phone number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPhoneNumber.Focus();
                return;
            }

            if (cmbRole.SelectedItem == null)
            {
                MessageBox.Show("Please select a role.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbRole.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtPlantName.Text))
            {
                MessageBox.Show("Please enter plant name.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                txtPlantName.Focus();
                return;
            }

            if (cmbStatus.SelectedItem == null)
            {
                MessageBox.Show("Please select a status.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                cmbStatus.Focus();
                return;
            }

            // Save supplier to database
            SaveSupplierToDatabase();
        }

        private void SaveSupplierToDatabase()
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = @"INSERT INTO supplier (S_Name, Phone_Number, S_Role, Plant_Name, S_Status, Created_At, Updated_At) 
                               VALUES (@Name, @PhoneNumber, @Role, @PlantName, @Status, @CreatedAt, @UpdatedAt)";

                using var command = new MySqlCommand(query, connection);

                // Add parameters
                command.Parameters.AddWithValue("@Name", txtName.Text.Trim());
                command.Parameters.AddWithValue("@PhoneNumber", txtPhoneNumber.Text.Trim());
                command.Parameters.AddWithValue("@Role", ((ComboBoxItem)cmbRole.SelectedItem).Content.ToString());
                command.Parameters.AddWithValue("@PlantName", txtPlantName.Text.Trim());
                command.Parameters.AddWithValue("@Status", ((ComboBoxItem)cmbStatus.SelectedItem).Content.ToString());
                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Supplier added successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Clear form fields
                    ClearForm();

                    // Navigate back to supplier page and refresh data
                    NavigateBackToSupplierPage();
                }
                else
                {
                    MessageBox.Show("Failed to add supplier. Please try again.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062) // Duplicate entry error
            {
                MessageBox.Show("A supplier with this information already exists.", "Duplicate Entry",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding supplier: {ex.Message}", "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            var result = MessageBox.Show("Are you sure you want to cancel? All unsaved changes will be lost.",
                "Confirm Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                NavigateBackToSupplierPage();
            }
        }

        private void ClearForm()
        {
            txtName.Clear();
            txtPhoneNumber.Clear();
            txtPlantName.Clear();
            cmbRole.SelectedIndex = -1;
            cmbStatus.SelectedIndex = 0; // Set to Active by default
        }

        private void NavigateBackToSupplierPage()
        {
            try
            {
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    // Create a new instance of the Supplier page
                    Supplier supplierPage = new Supplier();
                    mainWindow.MainContentFrame.Navigate(supplierPage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating back to supplier page: {ex.Message}", "Navigation Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}