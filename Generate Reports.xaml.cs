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
    public partial class Generate_Reports : Page
    {
        // Collection to hold the data for the DataGrid
        public ObservableCollection<ReportDisplayData> ReportDataList { get; set; }

        private readonly string? connectionString;
        private const string DbConnectionStringEnvVar = "PRIMETECH_DB_CONN_STRING"; // Ensure this matches your .env file

        public Generate_Reports()
        {
            InitializeComponent();
            ReportDataList = new ObservableCollection<ReportDisplayData>();
            this.DataContext = this; // Set DataContext for binding ReportDataList

            connectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVar);

            if (string.IsNullOrEmpty(connectionString))
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Generate_Reports.xaml.cs - Connection string IS NULL or EMPTY. Env Var: '{DbConnectionStringEnvVar}'");
                MessageBox.Show($"Database connection string environment variable '{DbConnectionStringEnvVar}' was not found or is empty. " +
                                $"Please ensure your .env file is correctly set up and loaded at application startup.\n\n" +
                                "Report data cannot be loaded.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                string connStringSnippet = connectionString.Length > 30 ? connectionString.Substring(0, 30) + "..." : connectionString;
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Generate_Reports.xaml.cs - Connection string FOUND (snippet): {connStringSnippet}");
                LoadReportDataFromDatabase();
            }
        }

        private void LoadReportDataFromDatabase()
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                Debug.WriteLine("Generate_Reports.xaml.cs: Connection string is null. Cannot load report data.");
                return;
            }

            ReportDataList.Clear();
            try
            {
                using MySqlConnection connection = new(connectionString);
                connection.Open();
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Generate_Reports.xaml.cs - Database connection opened successfully.");

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
                    INNER JOIN 
                        customer c ON o.FK_Customer_ID = c.Customer_ID
                    ORDER BY 
                        o.Order_Date DESC, o.Order_ID DESC;";
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Generate_Reports.xaml.cs - Executing query: {query.Trim()}");

                using MySqlCommand command = new(query, connection);
                using MySqlDataReader reader = command.ExecuteReader();

                int itemsLoaded = 0;
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
                        Balance = totalAmount - amountReceived
                    };
                    ReportDataList.Add(reportEntry);
                    itemsLoaded++;
                }
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Generate_Reports.xaml.cs - Finished reading data. Total entries loaded: {itemsLoaded}. ReportDataList.Count now: {ReportDataList.Count}");
                if (itemsLoaded == 0 && ReportDataList.Count == 0)
                {
                    Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Generate_Reports.xaml.cs - No order data found in the database or no data was added to the list after reading.");
                }
            }
            catch (MySqlException myEx)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] MySQL Error loading report data from Generate_Reports: {myEx.ToString()}");
                MessageBox.Show($"Database Error (MySQL): {myEx.Message} (Code: {myEx.Number})", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Generic error loading report data from Generate_Reports: {ex.ToString()}");
                MessageBox.Show($"An error occurred while loading report data: {ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        private void CreateNewReportButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Generate_Reports.xaml.cs - CreateNewReportButton_Click called.");

            CreateReportPage createReportPage = new CreateReportPage();

            // Attempt to navigate using the Page's NavigationService first
            if (this.NavigationService != null)
            {
                // Subscribe to the event if you want to refresh after navigation returns
                // createReportPage.ReportSavedSuccessfully += (s, ev) => LoadReportDataFromDatabase(); 
                this.NavigationService.Navigate(createReportPage);
            }
            // Fallback to MainWindow's frame if page's NavigationService is null (e.g. if not hosted in a Frame directly)
            else if (Application.Current.MainWindow is MainWindow mainWindow && mainWindow.MainContentFrame != null)
            {
                mainWindow.MainContentFrame.Navigate(createReportPage);
            }
            else
            {
                MessageBox.Show("Could not find a frame to navigate to the Create Report page.", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }


        // Event handler for the "Create New Report" button if needed for specific actions
        // For now, this page just displays the data like the Reports page.
        // If "Create New Report" means something else (like generating a PDF, or a new summary),
        // this method's logic would change.
      
    }
}
