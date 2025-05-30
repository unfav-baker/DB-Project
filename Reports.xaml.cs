using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.Data;

namespace Supplier // Or your appropriate namespace
{
    public partial class Reports : Page
    {
        public ObservableCollection<ReportDisplayData> ReportDataList { get; set; }

        private readonly string? connectionString;
        private const string DbConnectionStringEnvVar = "PRIMETECH_DB_CONN_STRING";

        public Reports()
        {
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Reports.xaml.cs - Constructor - Before InitializeComponent().");
            InitializeComponent();
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Reports.xaml.cs - Constructor - After InitializeComponent().");

            ReportDataList = new();
            this.DataContext = this;

            connectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVar);

            if (string.IsNullOrEmpty(connectionString))
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Reports.xaml.cs - Constructor - Connection string IS NULL or EMPTY. Env Var: '{DbConnectionStringEnvVar}'");
                MessageBox.Show($"Database connection string environment variable '{DbConnectionStringEnvVar}' was not found or is empty. " +
                                $"Please ensure your .env file is correctly set up and loaded at application startup.\n\n" +
                                "Report data cannot be loaded.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                string connStringSnippet = connectionString.Length > 30 ? connectionString.Substring(0, 30) + "..." : connectionString;
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Reports.xaml.cs - Constructor - Connection string FOUND (snippet): {connStringSnippet}");
                LoadReportDataFromDatabase();
            }
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Reports.xaml.cs - Constructor finished.");
        }

        private void LoadReportDataFromDatabase()
        {
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Reports.xaml.cs - LoadReportDataFromDatabase() called.");
            if (string.IsNullOrEmpty(connectionString))
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Reports.xaml.cs - LoadReportDataFromDatabase: Connection string is missing. Data not loaded.");
                return;
            }

            ReportDataList.Clear();
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Reports.xaml.cs - ReportDataList cleared.");
            try
            {
                using MySqlConnection connection = new(connectionString);
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Reports.xaml.cs - MySqlConnection object created.");
                connection.Open();
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Reports.xaml.cs - Database connection opened successfully.");

                string query = @"
                    SELECT 
                        o.Order_ID, o.Order_Date, c.Name AS PartyName, o.Export_Through,
                        o.Plant, o.Importer, o.Phyto_Number, o.Carton, o.Weight,
                        o.Rate, o.Total_Amount, o.Amount_Received
                    FROM orders o
                    INNER JOIN customer c ON o.FK_Customer_ID = c.Customer_ID
                    ORDER BY o.Order_Date DESC, o.Order_ID DESC;";
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Reports.xaml.cs - Executing query: {query.Trim()}");

                using MySqlCommand command = new(query, connection);
                using MySqlDataReader reader = command.ExecuteReader();
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Reports.xaml.cs - Query executed, reader obtained.");

                int itemsLoaded = 0;
                while (reader.Read())
                {
                    itemsLoaded++;
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
                        Carton = reader.IsDBNull("Carton") ? null : reader.GetInt32("Carton"),
                        Weight = reader.IsDBNull("Weight") ? null : reader.GetDecimal("Weight"),
                        Rate = reader.IsDBNull("Rate") ? null : reader.GetDecimal("Rate"),
                        TotalAmount = totalAmount,
                        AmountReceived = amountReceived,
                        Balance = totalAmount - amountReceived
                    };
                    ReportDataList.Add(reportEntry);
                }
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Reports.xaml.cs - Finished reading data. Total entries processed: {itemsLoaded}. ReportDataList.Count now: {ReportDataList.Count}");
                if (itemsLoaded == 0 && ReportDataList.Count == 0)
                {
                    Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Reports.xaml.cs - No order data found in the database OR no data was added to the list after reading.");
                }
            }
            catch (MySqlException myEx)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] MySQL Error loading report data: {myEx.ToString()}");
                MessageBox.Show($"Database Error (MySQL): {myEx.Message} (Code: {myEx.Number})", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Generic error loading report data: {ex.ToString()}");
                MessageBox.Show($"An error occurred while loading report data: {ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Reports.xaml.cs - LoadReportDataFromDatabase() finished.");
        }

        private void RefreshReports_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Reports.xaml.cs - RefreshReports_Click called.");
            LoadReportDataFromDatabase();
            MessageBox.Show("Report data has been refreshed.", "Data Refreshed", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
