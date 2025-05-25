using System;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient; // Ensure you have this NuGet package (MySql.Data)

namespace Adminn
{
    public partial class AddSupplier : Page
    {
        // Connection string for your MySQL database
        private readonly string connectionString = "Server=127.0.0.1;Port=3306;Database=prime_tech;Uid=root;Pwd=Abubaker85@@;";

        public AddSupplier()
        {
            InitializeComponent();
            // Set default values if any, e.g., for ComboBoxes
            if (cmbStatus.Items.Count > 0) // Ensure ComboBox is populated before setting index
            {
                cmbStatus.SelectedIndex = 0; // Default to "Active"
            }
        }

        private void SaveSupplier_Click(object sender, RoutedEventArgs e)
        {
            // Basic Validation
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                ShowValidationError("Supplier Name cannot be empty.", txtName);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPhoneNumber.Text))
            {
                ShowValidationError("Phone Number cannot be empty.", txtPhoneNumber);
                return;
            }
            // Basic phone number format check (can be enhanced)
            if (txtPhoneNumber.Text.Trim().Length < 7 || !System.Text.RegularExpressions.Regex.IsMatch(txtPhoneNumber.Text, @"^[0-9+-\s()]*$"))
            {
                ShowValidationError("Please enter a valid Phone Number.", txtPhoneNumber);
                return;
            }
            if (cmbRole.SelectedItem == null)
            {
                ShowValidationError("Please select a Role for the supplier.", cmbRole);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtPlantName.Text)) // Assuming Plant Name is required
            {
                ShowValidationError("Plant Name cannot be empty.", txtPlantName);
                return;
            }
            if (cmbStatus.SelectedItem == null)
            {
                ShowValidationError("Please select a Status for the supplier.", cmbStatus);
                return;
            }

            try
            {
                int newSupplierId = SaveSupplierToDatabase(); // Get the new ID

                if (newSupplierId > 0)
                {
                    MessageBox.Show($"Supplier added successfully! Supplier ID: {newSupplierId}", "Success",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    NavigateToSupplierPage(); // Navigate back after successful save
                }
                else
                {
                    MessageBox.Show("Failed to add supplier. No ID was returned.", "Error",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving supplier: {ex.Message}", "Database Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private int SaveSupplierToDatabase() // Changed to return int (the new ID)
        {
            using var connection = new MySqlConnection(connectionString);
            connection.Open();

            // Assuming your table is named 'supplier' and columns match these names
            // Ensure your database table `supplier` has an auto-increment primary key for LAST_INSERT_ID() to work.
            string query = @"INSERT INTO supplier (S_Name, Phone_Number, S_Role, Plant_Name, S_Status, Created_At, Updated_At) 
                             VALUES (@Name, @PhoneNumber, @Role, @PlantName, @Status, @CreatedAt, @UpdatedAt);
                             SELECT LAST_INSERT_ID();"; // Get the ID of the inserted row

            using var command = new MySqlCommand(query, connection);

            command.Parameters.AddWithValue("@Name", txtName.Text.Trim());
            command.Parameters.AddWithValue("@PhoneNumber", txtPhoneNumber.Text.Trim());
            command.Parameters.AddWithValue("@Role", ((ComboBoxItem)cmbRole.SelectedItem).Content.ToString());
            command.Parameters.AddWithValue("@PlantName", txtPlantName.Text.Trim()); // Assuming PlantName is text
            command.Parameters.AddWithValue("@Status", ((ComboBoxItem)cmbStatus.SelectedItem).Content.ToString());
            command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
            command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);

            // Use ExecuteScalar to get the result of SELECT LAST_INSERT_ID();
            var result = command.ExecuteScalar();
            if (result != null && result != DBNull.Value)
            {
                return Convert.ToInt32(result);
            }
            return 0; // Indicate failure if no ID was returned
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
            NavigateToSupplierPage();
        }

        private void ClearForm()
        {
            txtName.Clear();
            txtPhoneNumber.Clear();
            txtPlantName.Clear();
            cmbRole.SelectedIndex = -1;    // No selection
            if (cmbStatus.Items.Count > 0) // Check if items exist before setting index
            {
                cmbStatus.SelectedIndex = 0; // Default to "Active" (first item)
            }
            txtName.Focus(); // Set focus to the first field
        }

        // Renamed for clarity
        private void NavigateToSupplierPage()
        {
            try
            {
                if (Application.Current.MainWindow is MainWindow mainWindow && mainWindow.MainContentFrame != null)
                {
                    // Navigate to a new instance of the Supplier page to ensure fresh data
                    Supplier supplierPage = new Supplier();
                    mainWindow.MainContentFrame.Navigate(supplierPage);
                }
                // Fallback if direct frame access isn't working or if this page is hosted in a sub-frame
                else if (this.NavigationService != null && this.NavigationService.CanGoBack)
                {
                    // This would go to the *previous* page, which might or might not be the Supplier list.
                    // Navigating to a new instance is generally safer for "closing" an add form.
                    // For now, let's prioritize navigating to a new Supplier page instance.
                    // this.NavigationService.GoBack();
                    // Consider what happens if MainContentFrame is null but NavigationService is also null or can't go back.
                    // The above block handles the primary navigation case.
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to Supplier page: {ex.Message}", "Navigation Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private static void ShowValidationError(string message, Control controlToFocus) // Marked as static
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            if (controlToFocus != null)
            {
                controlToFocus.Focus();
            }
        }

        // Optional: Add TextChanged event handlers for real-time validation if needed
        // For example, for txtPhoneNumber or txtPlantName, similar to AddEmployee.xaml.cs
        // Remember to connect them in AddSupplier.xaml if you add them here.
        // private void TxtPhoneNumber_TextChanged(object sender, TextChangedEventArgs e) { /* ... validation ... */ }
        // private void TxtPlantName_TextChanged(object sender, TextChangedEventArgs e) { /* ... validation ... */ }
    }
}