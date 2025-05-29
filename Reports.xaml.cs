using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Data;

namespace Employee // Your namespace
{
    public partial class Reports : Page
    {
        public ObservableCollection<ReportDisplayData> ReportDataList { get; set; }

        private readonly string? connectionString;
        private const string DbConnectionStringEnvVar = "PRIMETECH_DB_CONN_STRING";

        public Reports()
        {
            InitializeComponent();
            ReportDataList = new ObservableCollection<ReportDisplayData>();
            this.DataContext = this; // For binding ReportDataList to the DataGrid

            connectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVar);

            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show($"Database connection string environment variable '{DbConnectionStringEnvVar}' was not found or is empty. " +
                                $"Please ensure your .env file is correctly set up and loaded at application startup.\n\n" +
                                "Report data cannot be loaded.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                LoadReportDataFromDatabase();
            }
        }

        private void LoadReportDataFromDatabase()
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                Debug.WriteLine("Reports.xaml.cs: Connection string is null. Cannot load report data.");
                return;
            }

            ReportDataList.Clear();
            try
            {
                using MySqlConnection connection = new(connectionString);
                connection.Open();

                // Query to get data primarily from 'orders' table and 'Name' from 'customer' table
                string query = @"
                    SELECT 
                        o.Order_ID,
                        o.Order_Date,
                        c.Name AS PartyName,
                        o.Export_Through,
                        o.Plant,
                        o.Importer,
                        o.Phyto_Number,
                        o.Carton,
                        o.Weight,
                        o.Rate,
                        o.Total_Amount,
                        o.Amount_Received
                    FROM 
                        orders o
                    JOIN 
                        customer c ON o.FK_Customer_ID = c.Customer_ID
                    ORDER BY 
                        o.Order_Date DESC, o.Order_ID DESC;";
                // Example ordering

                using MySqlCommand command = new(query, connection);
                using MySqlDataReader reader = command.ExecuteReader();

                while (reader.Read())
                {
                    decimal totalAmount = reader.IsDBNull("Total_Amount") ? 0m : reader.GetDecimal("Total_Amount");
                    decimal amountReceived = reader.IsDBNull("Amount_Received") ? 0m : reader.GetDecimal("Amount_Received");

                    var reportEntry = new ReportDisplayData
                    {
                        OrderId = reader.GetInt32("Order_ID"),
                        OrderDate = reader.IsDBNull("Order_Date") ? DateTime.MinValue : reader.GetDateTime("Order_Date"),
                        PartyName = reader.IsDBNull("PartyName") ? "N/A" : reader.GetString("PartyName"),
                        ExportThrough = reader.IsDBNull("Export_Through") ? string.Empty : reader.GetString("Export_Through"),
                        Plant = reader.IsDBNull("Plant") ? string.Empty : reader.GetString("Plant"),
                        Importer = reader.IsDBNull("Importer") ? string.Empty : reader.GetString("Importer"),
                        PhytoNumber = reader.IsDBNull("Phyto_Number") ? string.Empty : reader.GetString("Phyto_Number"),
                        Carton = reader.IsDBNull("Carton") ? (int?)null : reader.GetInt32("Carton"),
                        Weight = reader.IsDBNull("Weight") ? (decimal?)null : reader.GetDecimal("Weight"),
                        Rate = reader.IsDBNull("Rate") ? (decimal?)null : reader.GetDecimal("Rate"),
                        TotalAmount = totalAmount,
                        AmountReceived = amountReceived,
                        Balance = totalAmount - amountReceived // Calculate balance
                    };
                    ReportDataList.Add(reportEntry);
                }
            }
            catch (MySqlException myEx)
            {
                Debug.WriteLine($"MySQL Error loading report data: {myEx.ToString()}");
                MessageBox.Show($"Database Error (MySQL): {myEx.Message} (Code: {myEx.Number})", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Generic error loading report data: {ex.ToString()}");
                MessageBox.Show($"An error occurred while loading report data: {ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Renamed from ViewReport_Click as the button seems generic now
        private void RefreshReports_Click(object sender, RoutedEventArgs e)
        {
            // The button text in your XAML is "View Reports", which sounds like a refresh.
            // If it's meant to navigate to a specific report creation/view page, the logic would differ.
            LoadReportDataFromDatabase();
            MessageBox.Show("Report data has been refreshed.", "Data Refreshed", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        // Add other event handlers if needed (e.g., for viewing details of a selected row, export, etc.)
        // For example, if you add a "View Details" button in your XAML for each row or a context menu:
        /*
        private void ViewReportRowDetails_Click(object sender, RoutedEventArgs e)
        {
            if (ReportsDataGrid.SelectedItem is ReportDisplayData selectedReport)
            {
                // Show more details or navigate
                MessageBox.Show($"Details for Order ID: {selectedReport.OrderId}\nParty: {selectedReport.PartyName}", "Report Item Details");
            }
        }
        */
    }
}
