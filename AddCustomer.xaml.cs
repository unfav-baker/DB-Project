using System;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient; // Ensure you have this NuGet package (MySql.Data)

namespace Adminn
{
    public partial class AddCustomer : Page
    {
        // Connection string for your MySQL database
        private readonly string connectionString = "Server=127.0.0.1;Port=3306;Database=prime_tech;Uid=root;Pwd=Abubaker85@@;";

        public AddCustomer()
        {
            InitializeComponent();
            DatePicker.SelectedDate = DateTime.Today; // Set default date
            // Set focus to the first input field
            PartyNameTextBox.Focus();
        }

        private void SaveCustomer_Click(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            if (!DatePicker.SelectedDate.HasValue)
            {
                ShowValidationError("Date is required.", DatePicker);
                return;
            }
            if (string.IsNullOrWhiteSpace(PhytoNumberTextBox.Text)) // Assuming Phyto Number is required
            {
                ShowValidationError("Phyto Number is required.", PhytoNumberTextBox);
                return;
            }
            if (string.IsNullOrWhiteSpace(PartyNameTextBox.Text))
            {
                ShowValidationError("Party Name is required.", PartyNameTextBox);
                return;
            }
            // Export Through and Plant might be optional, add validation if required
            if (string.IsNullOrWhiteSpace(ImporterTextBox.Text)) // Assuming Importer is required
            {
                ShowValidationError("Importer is required.", ImporterTextBox);
                return;
            }
            if (string.IsNullOrWhiteSpace(CartonTextBox.Text) || !int.TryParse(CartonTextBox.Text, out _))
            {
                ShowValidationError("Please enter a valid number for Carton.", CartonTextBox);
                return;
            }
            if (string.IsNullOrWhiteSpace(WeightTextBox.Text) || !decimal.TryParse(WeightTextBox.Text, out _))
            {
                ShowValidationError("Please enter a valid number for Weight.", WeightTextBox);
                return;
            }
            if (string.IsNullOrWhiteSpace(RateTextBox.Text) || !decimal.TryParse(RateTextBox.Text, out _))
            {
                ShowValidationError("Please enter a valid number for Rate.", RateTextBox);
                return;
            }
            if (string.IsNullOrWhiteSpace(AmountTextBox.Text) || !decimal.TryParse(AmountTextBox.Text, out _))
            {
                ShowValidationError("Please enter a valid number for Amount.", AmountTextBox);
                return;
            }
            if (string.IsNullOrWhiteSpace(ReceivedTextBox.Text)) // Assuming Received is required
            {
                ShowValidationError("Received amount/status is required.", ReceivedTextBox);
                return;
            }


            // Parse numeric fields (already partially done in validation, ensure they are used)
            int.TryParse(CartonTextBox.Text, out int carton);
            decimal.TryParse(WeightTextBox.Text, out decimal weight);
            decimal.TryParse(RateTextBox.Text, out decimal rate);
            decimal.TryParse(AmountTextBox.Text, out decimal amount);
            // Received is treated as string in your original code, kept as is.

            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                // Ensure your table is 'customer' and columns match your DB schema.
                // Added `SELECT LAST_INSERT_ID();` to get the new customer ID.
                string query = @"INSERT INTO customer (Date, Phyto_Number, Party_Name, Export_Through, Plant, 
                                     Importer, Carton, Weight, Rate, Amount, Received) 
                                     VALUES (@Date, @PhytoNumber, @PartyName, @ExportThrough, @Plant, 
                                     @Importer, @Carton, @Weight, @Rate, @Amount, @Received);
                                     SELECT LAST_INSERT_ID();";

                using var command = new MySqlCommand(query, connection);

                command.Parameters.AddWithValue("@Date", DatePicker.SelectedDate.Value);
                command.Parameters.AddWithValue("@PhytoNumber", PhytoNumberTextBox.Text.Trim());
                command.Parameters.AddWithValue("@PartyName", PartyNameTextBox.Text.Trim());
                command.Parameters.AddWithValue("@ExportThrough", ExportThroughTextBox.Text.Trim());
                command.Parameters.AddWithValue("@Plant", PlantTextBox.Text.Trim());
                command.Parameters.AddWithValue("@Importer", ImporterTextBox.Text.Trim());
                command.Parameters.AddWithValue("@Carton", carton);
                command.Parameters.AddWithValue("@Weight", weight);
                command.Parameters.AddWithValue("@Rate", rate);
                command.Parameters.AddWithValue("@Amount", amount);
                command.Parameters.AddWithValue("@Received", ReceivedTextBox.Text.Trim());

                var result = command.ExecuteScalar(); // Use ExecuteScalar to get the LAST_INSERT_ID
                int newCustomerId = 0;
                if (result != null && result != DBNull.Value)
                {
                    newCustomerId = Convert.ToInt32(result);
                }


                if (newCustomerId > 0)
                {
                    MessageBox.Show($"Customer added successfully! Customer ID: {newCustomerId}", "Success",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    ClearForm();
                    NavigateToCustomerPage(); // Navigate back after successful save
                }
                else
                {
                    MessageBox.Show("Failed to add customer or retrieve new ID.", "Error",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (MySqlException ex) when (ex.Number == 1062) // Handle potential duplicate entry
            {
                MessageBox.Show("A customer with this Phyto Number or other unique information already exists.", "Duplicate Entry",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving customer: {ex.Message}", "Database Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
            NavigateToCustomerPage();
        }

        private void ClearForm()
        {
            DatePicker.SelectedDate = DateTime.Today;
            PhytoNumberTextBox.Text = string.Empty;
            PartyNameTextBox.Text = string.Empty;
            ExportThroughTextBox.Text = string.Empty;
            PlantTextBox.Text = string.Empty;
            ImporterTextBox.Text = string.Empty;
            CartonTextBox.Text = string.Empty;
            WeightTextBox.Text = string.Empty;
            RateTextBox.Text = string.Empty;
            AmountTextBox.Text = string.Empty;
            ReceivedTextBox.Text = string.Empty;

            PartyNameTextBox.Focus(); // Set focus to a primary input field
        }

        // Renamed for clarity and consistency
        private void NavigateToCustomerPage()
        {
            try
            {
                if (Application.Current.MainWindow is MainWindow mainWindow && mainWindow.MainContentFrame != null)
                {
                    // Navigate to a new instance of the Customer page to ensure fresh data
                    Customer customerPage = new Customer();
                    mainWindow.MainContentFrame.Navigate(customerPage);
                }
                else if (this.NavigationService != null && this.NavigationService.CanGoBack)
                {
                    // Fallback: This might not always lead to the Customer list if navigation stack is complex
                    this.NavigationService.GoBack();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating to Customer page: {ex.Message}", "Navigation Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Helper for validation messages
        private static void ShowValidationError(string message, Control controlToFocus)
        {
            MessageBox.Show(message, "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            if (controlToFocus != null)
            {
                controlToFocus.Focus();
            }
        }

        // Optional: Add TextChanged event handlers for real-time validation if needed
        // e.g., for PhytoNumberTextBox, CartonTextBox, WeightTextBox, RateTextBox, AmountTextBox
        // Remember to connect them in AddCustomer.xaml if you add them here.
        // private void NumericOnly_TextChanged(object sender, TextChangedEventArgs e) { /* ... logic ... */ }
    }
}