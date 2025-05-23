using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace Adminn
{
    public partial class Reports : Page
    {
        public ObservableCollection<ReportData> ReportsList { get; set; }

        // Replace this with your actual MySQL connection string
        private readonly string connectionString = "Server=127.0.0.1;Port=3306;Database=prime_tech;Uid=root;Pwd=Abubaker85@@;";

        public Reports()
        {
            InitializeComponent();
            ReportsList = new ObservableCollection<ReportData>();

            // Load report data from database
            LoadReportDataFromDatabase();

            // Set the DataContext for binding
            DataContext = this;
        }

        private void ViewReport_Click(object sender, RoutedEventArgs e)
        {
            // Navigate to ViewReport page within the parent frame
            ViewReport viewReportPage = new ViewReport();
            // Find the main window's frame and navigate
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                mainWindow.MainContentFrame.Navigate(viewReportPage);
            }
        }

        private void ViewReportDetails_Click(object sender, RoutedEventArgs e)
        {
            if (ReportsDataGrid.SelectedItem is ReportData selectedReport)
            {
                MessageBox.Show($"Report Details:\nID: {selectedReport.ReportId}\nType: {selectedReport.ReportType}\nDate Generated: {selectedReport.DateGenerated:dd/MM/yyyy}\nStatus: {selectedReport.Status}\nCreated At: {selectedReport.CreatedAt:dd/MM/yyyy}\nUpdated At: {selectedReport.UpdatedAt:dd/MM/yyyy}",
                    "Report Details", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select a report to view.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void EditReport_Click(object sender, RoutedEventArgs e)
        {
            if (ReportsDataGrid.SelectedItem is ReportData selectedReport)
            {
                // Here you can navigate to an edit page or open an edit dialog
                MessageBox.Show($"Edit functionality for Report ID: {selectedReport.ReportId}", "Edit Report", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select a report to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteReport_Click(object sender, RoutedEventArgs e)
        {
            if (ReportsDataGrid.SelectedItem is ReportData selectedReport)
            {
                var result = MessageBox.Show($"Are you sure you want to delete report '{selectedReport.ReportType}' (ID: {selectedReport.ReportId})?",
                    "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteReportFromDatabase(selectedReport.ReportId);
                }
            }
            else
            {
                MessageBox.Show("Please select a report to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void LoadReportDataFromDatabase()
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = @"SELECT Report_ID, Report_Type, Date_Generated, Statuss, Created_At, Updated_At 
                               FROM reports ORDER BY Report_ID";

                using var command = new MySqlCommand(query, connection);
                using var reader = command.ExecuteReader();

                ReportsList.Clear();

                while (reader.Read())
                {
                    var report = new ReportData
                    {
                        ReportId = reader.GetInt32("Report_ID"),
                        ReportType = reader.IsDBNull("Report_Type") ? string.Empty : reader.GetString("Report_Type"),
                        DateGenerated = reader.IsDBNull("Date_Generated") ? DateTime.Now : reader.GetDateTime("Date_Generated"),
                        Status = reader.IsDBNull("Statuss") ? string.Empty : reader.GetString("Statuss"),
                        CreatedAt = reader.IsDBNull("Created_At") ? DateTime.Now : reader.GetDateTime("Created_At"),
                        UpdatedAt = reader.IsDBNull("Updated_At") ? DateTime.Now : reader.GetDateTime("Updated_At")
                    };

                    ReportsList.Add(report);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading report data: {ex.Message}", "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteReportFromDatabase(int reportId)
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = "DELETE FROM reports WHERE Report_ID = @Report_ID";

                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Report_ID", reportId);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Report deleted successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);

                    // Refresh the data
                    LoadReportDataFromDatabase();
                }
                else
                {
                    MessageBox.Show("Report not found or could not be deleted.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting report: {ex.Message}", "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public void RefreshData()
        {
            LoadReportDataFromDatabase();
        }
    }

    public class ReportData : INotifyPropertyChanged
    {
        private bool _isSelected;
        private int _reportId;
        private string _reportType = string.Empty;
        private DateTime _dateGenerated;
        private string _status = string.Empty;
        private DateTime _createdAt;
        private DateTime _updatedAt;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                _isSelected = value;
                OnPropertyChanged();
            }
        }

        public int ReportId
        {
            get => _reportId;
            set
            {
                _reportId = value;
                OnPropertyChanged();
            }
        }

        public string ReportType
        {
            get => _reportType;
            set
            {
                _reportType = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public DateTime DateGenerated
        {
            get => _dateGenerated;
            set
            {
                _dateGenerated = value;
                OnPropertyChanged();
            }
        }

        public string Status
        {
            get => _status;
            set
            {
                _status = value ?? string.Empty;
                OnPropertyChanged();
            }
        }

        public DateTime CreatedAt
        {
            get => _createdAt;
            set
            {
                _createdAt = value;
                OnPropertyChanged();
            }
        }

        public DateTime UpdatedAt
        {
            get => _updatedAt;
            set
            {
                _updatedAt = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}