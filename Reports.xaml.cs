using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices; // For CallerMemberName
using System.Text; // For StringBuilder
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;
using System.Diagnostics; // For Debug.WriteLine

namespace Supplier // Project's root namespace, as provided
{
    public partial class Reports : Page // Class name matches x:Class
    {
        public ObservableCollection<ReportItemData> ReportItemsList { get; set; }

        // Connection string will be loaded from environment variable
        private readonly string? connectionString; // Made nullable
        private const string DbConnectionStringEnvVar = "PRIMETECH_DB_CONN_STRING"; // Consistent name

        public Reports()
        {
            InitializeComponent();

            // Load connection string from environment variable
            // Assumes Env.Load() has been called in App.xaml.cs of your main application
            connectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVar);

            ReportItemsList = new ObservableCollection<ReportItemData>(); // Initialize before use

            if (string.IsNullOrEmpty(connectionString))
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Reports.xaml.cs - Connection string IS NULL or EMPTY. Env Var: '{DbConnectionStringEnvVar}'");
                MessageBox.Show($"Database connection string environment variable '{DbConnectionStringEnvVar}' was not found or is empty. " +
                                $"Please ensure your .env file is correctly set up and loaded at application startup.\n\n" +
                                "Report data cannot be loaded.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            else
            {
                string connStringSnippet = connectionString.Length > 30 ? connectionString.Substring(0, 30) + "..." : connectionString;
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Reports.xaml.cs - Connection string FOUND (snippet): {connStringSnippet}");
                LoadReportDataFromDatabase();
            }

            DataContext = this; // Set the DataContext for binding
        }

        private void LoadReportDataFromDatabase()
        {
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Reports.xaml.cs - LoadReportDataFromDatabase() called.");
            if (string.IsNullOrEmpty(this.connectionString))
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Reports.xaml.cs - LoadReportDataFromDatabase: Connection string is missing. Data not loaded.");
                return;
            }

            ReportItemsList.Clear();
            try
            {
                using MySqlConnection connection = new(this.connectionString); // Simplified new
                connection.Open();
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Reports.xaml.cs - Database connection opened successfully.");

                string query = @"
                    SELECT 
                        r.Report_ID, 
                        r.Report_Type, 
                        r.Date_Generated, 
                        r.Statuss, -- Using 'Statuss' as per your table definition
                        r.Employee_Id,
                        e.Name AS EmployeeName, 
                        r.Created_At,
                        r.Updated_At
                    FROM 
                        reports r
                    LEFT JOIN 
                        employees e ON r.Employee_Id = e.AdminId 
                    ORDER BY 
                        r.Report_ID DESC";
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Reports.xaml.cs - Executing query: {query.Trim()}");

                using MySqlCommand command = new(query, connection); // Simplified new
                using MySqlDataReader reader = command.ExecuteReader();

                int itemsLoaded = 0;
                while (reader.Read())
                {
                    var reportItem = new ReportItemData
                    {
                        IsSelected = false,
                        ReportId = reader.GetInt32("Report_ID"),
                        ReportType = reader.IsDBNull(reader.GetOrdinal("Report_Type")) ? string.Empty : reader.GetString("Report_Type"),
                        DateGenerated = reader.IsDBNull(reader.GetOrdinal("Date_Generated")) ? DateTime.MinValue : reader.GetDateTime("Date_Generated"),
                        Status = reader.IsDBNull(reader.GetOrdinal("Statuss")) ? string.Empty : reader.GetString("Statuss"), // Reading from 'Statuss'
                        EmployeeId = reader.IsDBNull(reader.GetOrdinal("Employee_Id")) ? (int?)null : reader.GetInt32("Employee_Id"),
                        EmployeeName = reader.IsDBNull(reader.GetOrdinal("EmployeeName")) ? "N/A" : reader.GetString("EmployeeName"),
                        CreatedAt = reader.IsDBNull(reader.GetOrdinal("Created_At")) ? DateTime.MinValue : reader.GetDateTime("Created_At"),
                        UpdatedAt = reader.IsDBNull(reader.GetOrdinal("Updated_At")) ? DateTime.MinValue : reader.GetDateTime("Updated_At")
                    };
                    ReportItemsList.Add(reportItem);
                    itemsLoaded++;
                }
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Reports.xaml.cs - Finished reading data. Total reports attempted: {itemsLoaded}. ReportItemsList.Count now: {ReportItemsList.Count}");
                if (itemsLoaded == 0 && ReportItemsList.Count == 0)
                {
                    Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Reports.xaml.cs - No reports were found in the database OR no reports were added to the list after reading.");
                }
            }
            catch (MySqlException myEx)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] MySQL Error loading report data: {myEx.ToString()}");
                MessageBox.Show($"MySQL Database Error: {myEx.Message}\n(Number: {myEx.Number})\n\nPlease check the database connection and query.", "Database Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Generic Error loading report data: {ex.ToString()}");
                MessageBox.Show($"Error loading report data: {ex.Message}", "Application Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void RefreshButton_Click(object sender, RoutedEventArgs e) // Assuming this button might exist or be added
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Database connection is not configured. Cannot refresh report data.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            LoadReportDataFromDatabase();
            MessageBox.Show("Report data refreshed!", "Refresh Complete", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ViewDetails_Click(object sender, RoutedEventArgs e) // Assuming this button exists for selected item details
        {
            if (ReportsDataGrid.SelectedItem is ReportItemData selectedReportItem) // Assuming DataGrid named ReportsDataGrid
            {
                // Using C# 9 raw string literals for better readability
                string details = $"""
                                      Report Details - ID: {selectedReportItem.ReportId}     
                                  ----------------------------------------------------
                                  Report Type:      {selectedReportItem.ReportType}
                                  Date Generated:   {selectedReportItem.DateGenerated:dd/MM/yyyy}
                                  Status:           {selectedReportItem.Status}
                                  Employee:         {selectedReportItem.EmployeeName} (ID: {(selectedReportItem.EmployeeId.HasValue ? selectedReportItem.EmployeeId.Value.ToString() : "N/A")})
                                  Created At:       {selectedReportItem.CreatedAt:dd/MM/yyyy HH:mm}
                                  Updated At:       {selectedReportItem.UpdatedAt:dd/MM/yyyy HH:mm}
                                  """;
                MessageBox.Show(details,
                                $"Details for Report ID: {selectedReportItem.ReportId}",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select a report to view its details.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void Delete_Click(object sender, RoutedEventArgs e) // Assuming this is the delete button event
        {
            if (ReportsDataGrid.SelectedItem is ReportItemData selectedReportItem) // Assuming DataGrid named ReportsDataGrid
            {
                var result = MessageBox.Show($"Are you sure you want to delete Report ID: {selectedReportItem.ReportId} ('{selectedReportItem.ReportType}')?",
                                             "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteReportFromDatabase(selectedReportItem.ReportId);
                }
            }
            else
            {
                MessageBox.Show("Please select a report to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteReportFromDatabase(int reportIdToDelete)
        {
            if (string.IsNullOrEmpty(this.connectionString))
            {
                MessageBox.Show("Database connection is not configured. Cannot delete report.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            try
            {
                using MySqlConnection connection = new(this.connectionString); // Simplified new
                connection.Open();

                string query = "DELETE FROM reports WHERE Report_ID = @ReportID_ToDelete";

                using MySqlCommand command = new(query, connection); // Simplified new
                command.Parameters.AddWithValue("@ReportID_ToDelete", reportIdToDelete);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Report deleted successfully!", "Success",
                                    MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadReportDataFromDatabase(); // Refresh the DataGrid
                }
                else
                {
                    MessageBox.Show("Report not found or could not be deleted.", "Delete Error",
                                    MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Error deleting report: {ex.ToString()}");
                MessageBox.Show($"Error deleting report: {ex.Message}", "Database Error",
                                MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void RefreshData() // Public method if called from elsewhere
        {
            if (string.IsNullOrEmpty(connectionString))
            {
                MessageBox.Show("Database connection is not configured. Cannot refresh data.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            LoadReportDataFromDatabase();
        }
    }

    public class ReportItemData : INotifyPropertyChanged // Your existing ReportItemData class
    {
        private bool _isSelected;
        private int _reportId;
        private string _reportType = string.Empty;
        private DateTime _dateGenerated;
        private string _status = string.Empty;
        private int? _employeeId;
        private string _employeeName = string.Empty;
        private DateTime _createdAt;
        private DateTime _updatedAt;

        public bool IsSelected
        {
            get => _isSelected;
            set { if (_isSelected != value) { _isSelected = value; OnPropertyChanged(); } }
        }

        public int ReportId
        {
            get => _reportId;
            set { if (_reportId != value) { _reportId = value; OnPropertyChanged(); } }
        }

        public string ReportType
        {
            get => _reportType;
            set { if (_reportType != value) { _reportType = value ?? string.Empty; OnPropertyChanged(); } }
        }

        public DateTime DateGenerated
        {
            get => _dateGenerated;
            set { if (_dateGenerated != value) { _dateGenerated = value; OnPropertyChanged(); } }
        }

        public string Status // Maps to 'Statuss' from DB
        {
            get => _status;
            set { if (_status != value) { _status = value ?? string.Empty; OnPropertyChanged(); } }
        }

        public int? EmployeeId
        {
            get => _employeeId;
            set { if (_employeeId != value) { _employeeId = value; OnPropertyChanged(); } }
        }

        public string EmployeeName
        {
            get => _employeeName;
            set { if (_employeeName != value) { _employeeName = value ?? string.Empty; OnPropertyChanged(); } }
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set { if (_createdAt != value) { _createdAt = value; OnPropertyChanged(); } }
        }

        public DateTime UpdatedAt
        {
            get => _updatedAt;
            set { if (_updatedAt != value) { _updatedAt = value; OnPropertyChanged(); } }
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}