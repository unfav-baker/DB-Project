using System;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace Adminn
{
    public partial class AddCustomer : Page
    {
        // Replace this with your actual MySQL connection string
        private readonly string connectionString = "Server=127.0.0.1;Port=3306;Database=prime_tech;Uid=root;Pwd=Abubaker85@@;";

        public AddCustomer()
        {
            InitializeComponent();

            // Set default date to today
            DatePicker.SelectedDate = DateTime.Today;
        }

        private void SaveCustomer_Click(object sender, RoutedEventArgs e)
        {
            // Validate required fields
            if (string.IsNullOrWhiteSpace(PartyNameTextBox.Text))
            {
                MessageBox.Show("Party Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                PartyNameTextBox.Focus();
                return;
            }

            if (!DatePicker.SelectedDate.HasValue)
            {
                MessageBox.Show("Date is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                DatePicker.Focus();
                return;
            }

            // Validate numeric fields
            if (!int.TryParse(CartonTextBox.Text, out int carton))
            {
                carton = 0; // Default value if parsing fails
            }

            if (!decimal.TryParse(WeightTextBox.Text, out decimal weight))
            {
                weight = 0; // Default value if parsing fails
            }

            if (!decimal.TryParse(RateTextBox.Text, out decimal rate))
            {
                rate = 0; // Default value if parsing fails
            }

            if (!decimal.TryParse(AmountTextBox.Text, out decimal amount))
            {
                amount = 0; // Default value if parsing fails
            }

            // For Received field - treating as string since your database shows Yes/No values
            string received = ReceivedTextBox.Text.Trim();

            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                // Updated column names to match your database structure
                string query = @"INSERT INTO customer (Date, Phyto_Number, Party_Name, Export_Through, Plant, 
                               Importer, Carton, Weight, Rate, Amount, Received) 
                               VALUES (@Date, @PhytoNumber, @PartyName, @ExportThrough, @Plant, 
                               @Importer, @Carton, @Weight, @Rate, @Amount, @Received)";

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
                command.Parameters.AddWithValue("@Received", received);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Customer added successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Clear the form for next entry
                    ClearForm();

                    // Navigate back to Customer page and refresh data
                    NavigateBackToCustomer();
                }
                else
                {
                    MessageBox.Show("Failed to add customer.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error saving customer: {ex.Message}", "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            // Ask for confirmation before canceling
            var result = MessageBox.Show("Are you sure you want to cancel? Any unsaved data will be lost.",
                "Confirm Cancel", MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                NavigateBackToCustomer();
            }
        }

        private void NavigateBackToCustomer()
        {
            try
            {
                // Navigate back to Customer page within the parent frame
                Customer customerPage = new Customer();
                if (Application.Current.MainWindow is MainWindow mainWindow)
                {
                    mainWindow.MainContentFrame.Navigate(customerPage);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error navigating back to Customer page: {ex.Message}", "Navigation Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
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
        }
    }
}