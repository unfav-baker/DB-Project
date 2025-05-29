using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Text; // For StringBuilder in ViewReportDetails_Click

namespace Adminn
{
    public partial class Reports : Page
    {
        public ObservableCollection<OrderSummaryData> ReportsList { get; set; } // Changed from ReportData

        private readonly string? connectionString;
        private const string DbConnectionStringEnvVar = "PRIMETECH_DB_CONN_STRING";

        public Reports()
        {
            InitializeComponent();
            ReportsList = new ObservableCollection<OrderSummaryData>();
            this.DataContext = this;

            connectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVar);

            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show($"Database connection string environment variable '{DbConnectionStringEnvVar}' was not found or is empty. " +
                                $"Please ensure your .env file is correctly set up and loaded at application startup (App.xaml.cs).\n\n" +
                                "Report data cannot be loaded.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                LoadOrderSummaryDataFromDatabase(); // Renamed method for clarity
            }
        }

        private void LoadOrderSummaryDataFromDatabase()
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                Debug.WriteLine("Adminn.Reports.xaml.cs: Connection string is null. Cannot load data.");
                return;
            }

            ReportsList.Clear();
            try
            {
                using MySqlConnection connection = new(connectionString);
                connection.Open();

                string query = @"
                    SELECT 
                        o.Order_ID, o.Order_Date, c.Name AS PartyName, o.Export_Through,
                        o.Plant, o.Importer, o.Phyto_Number, o.Carton, o.Weight,
                        o.Rate, o.Total_Amount, o.Amount_Received
                    FROM orders o
                    INNER JOIN customer c ON o.FK_Customer_ID = c.Customer_ID
                    ORDER BY o.Order_Date DESC, o.Order_ID DESC;";

                using MySqlCommand command = new(query, connection);
                using MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    decimal totalAmount = reader.IsDBNull(reader.GetOrdinal("Total_Amount")) ? 0m : reader.GetDecimal(reader.GetOrdinal("Total_Amount"));
                    decimal amountReceived = reader.IsDBNull(reader.GetOrdinal("Amount_Received")) ? 0m : reader.GetDecimal(reader.GetOrdinal("Amount_Received"));

                    ReportsList.Add(new OrderSummaryData
                    {
                        OrderId = reader.GetInt32(reader.GetOrdinal("Order_ID")),
                        OrderDate = reader.IsDBNull(reader.GetOrdinal("Order_Date")) ? DateTime.MinValue : reader.GetDateTime(reader.GetOrdinal("Order_Date")),
                        PartyName = reader.IsDBNull(reader.GetOrdinal("PartyName")) ? "N/A" : reader.GetString(reader.GetOrdinal("PartyName")),
                        ExportThrough = reader.IsDBNull(reader.GetOrdinal("Export_Through")) ? string.Empty : reader.GetString(reader.GetOrdinal("Export_Through")),
                        Plant = reader.IsDBNull(reader.GetOrdinal("Plant")) ? string.Empty : reader.GetString(reader.GetOrdinal("Plant")),
                        Importer = reader.IsDBNull(reader.GetOrdinal("Importer")) ? string.Empty : reader.GetString(reader.GetOrdinal("Importer")),
                        PhytoNumber = reader.IsDBNull(reader.GetOrdinal("Phyto_Number")) ? string.Empty : reader.GetString(reader.GetOrdinal("Phyto_Number")),
                        Carton = reader.IsDBNull(reader.GetOrdinal("Carton")) ? (int?)null : reader.GetInt32(reader.GetOrdinal("Carton")),
                        Weight = reader.IsDBNull(reader.GetOrdinal("Weight")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Weight")),
                        Rate = reader.IsDBNull(reader.GetOrdinal("Rate")) ? (decimal?)null : reader.GetDecimal(reader.GetOrdinal("Rate")),
                        TotalAmount = totalAmount,
                        AmountReceived = amountReceived,
                        Balance = totalAmount - amountReceived
                    });
                }
            }
            catch (MySqlException myEx)
            {
                Debug.WriteLine($"MySQL Error loading order summary data: {myEx.ToString()}");
                MessageBox.Show($"Database Error (MySQL): {myEx.Message} (Code: {myEx.Number})", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Generic error loading order summary data: {ex.ToString()}");
                MessageBox.Show($"An error occurred: {ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ViewReport_Click(object sender, RoutedEventArgs e) // This is your "View Reports" button
        {
            // This button currently navigates to a generic "ViewReport" page.
            // You need to define what this page should do.
            // Does it show a summary of all orders? Or a form to generate a *new type* of report?
            // For now, it navigates to a placeholder page.
            // If you want this to act as a "Refresh" button for the current list, change its logic:
            // LoadOrderSummaryDataFromDatabase();
            // MessageBox.Show("Data refreshed!");

            Debug.WriteLine("ViewReport_Click: Navigating to ViewReport page (placeholder).");
            ViewReport viewReportPage = new ViewReport(); // Assuming ViewReport.xaml exists
            if (Application.Current.MainWindow is MainWindow mainWindow && mainWindow.MainContentFrame != null)
            {
                mainWindow.MainContentFrame.Navigate(viewReportPage);
            }
            else
            {
                MessageBox.Show("Could not find main frame for navigation.", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void ViewReportDetails_Click(object sender, RoutedEventArgs e) // This is your "👁" button
        {
            if (ReportsDataGrid.SelectedItem is OrderSummaryData selectedOrder)
            {
                StringBuilder detailsBuilder = new StringBuilder();
                detailsBuilder.AppendLine("Order Summary Details:");
                detailsBuilder.AppendLine("------------------------------");
                detailsBuilder.AppendLine($"Order ID: {selectedOrder.OrderId}");
                detailsBuilder.AppendLine($"Order Date: {selectedOrder.OrderDate:dd/MM/yyyy}");
                detailsBuilder.AppendLine($"Party Name: {selectedOrder.PartyName}");
                detailsBuilder.AppendLine($"Export Through: {selectedOrder.ExportThrough}");
                detailsBuilder.AppendLine($"Plant: {selectedOrder.Plant}");
                detailsBuilder.AppendLine($"Importer: {selectedOrder.Importer}");
                detailsBuilder.AppendLine($"Phyto Number: {selectedOrder.PhytoNumber}");
                detailsBuilder.AppendLine($"Carton(s): {selectedOrder.Carton?.ToString() ?? "N/A"}");
                detailsBuilder.AppendLine($"Weight: {selectedOrder.Weight?.ToString("N2") ?? "N/A"}");
                detailsBuilder.AppendLine($"Rate: {selectedOrder.Rate?.ToString("C") ?? "N/A"}");
                detailsBuilder.AppendLine($"Total Amount: {selectedOrder.TotalAmount:C}");
                detailsBuilder.AppendLine($"Amount Received: {selectedOrder.AmountReceived?.ToString("C") ?? "N/A"}");
                detailsBuilder.AppendLine($"Balance: {selectedOrder.Balance:C}");

                MessageBox.Show(detailsBuilder.ToString(), "Order Details", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select an order item to view its details.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteReport_Click(object sender, RoutedEventArgs e) // This is your "🗑" button
        {
            if (ReportsDataGrid.SelectedItem is OrderSummaryData selectedOrder)
            {
                var result = MessageBox.Show($"Are you sure you want to delete Order ID: {selectedOrder.OrderId} for '{selectedOrder.PartyName}'?",
                                             "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteOrderFromDatabase(selectedOrder.OrderId);
                }
            }
            else
            {
                MessageBox.Show("Please select an order item to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteOrderFromDatabase(int orderId)
        {
            if (string.IsNullOrEmpty(this.connectionString))
            {
                MessageBox.Show("Database connection is not configured. Cannot delete.", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            try
            {
                using MySqlConnection connection = new(this.connectionString);
                connection.Open();
                string query = "DELETE FROM orders WHERE Order_ID = @OrderId";
                using MySqlCommand command = new(query, connection);
                command.Parameters.AddWithValue("@OrderId", orderId);
                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Order entry deleted successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadOrderSummaryDataFromDatabase(); // Refresh the list
                }
                else
                {
                    MessageBox.Show("Order entry not found or could not be deleted.", "Deletion Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error deleting order entry: {ex.ToString()}");
                MessageBox.Show($"Error deleting order entry: {ex.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
