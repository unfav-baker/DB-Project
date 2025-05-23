using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

namespace Adminn
{
    public partial class ViewReport : Page
    {
        public ObservableCollection<ReportMetric> ReportMetrics { get; set; }

        public ViewReport()
        {
            InitializeComponent();
            LoadSampleReportData();
            DataContext = this;
        }

        private void LoadSampleReportData()
        {
            // Initialize sample data for the report
            ReportMetrics = new ObservableCollection<ReportMetric>
            {
                new ReportMetric { Metric = "Productivity", Value = "85%", Target = "80%", Status = "Above Target" },
                new ReportMetric { Metric = "Efficiency", Value = "92%", Target = "90%", Status = "Above Target" },
                new ReportMetric { Metric = "Quality Score", Value = "4.3", Target = "4.0", Status = "Above Target" },
                new ReportMetric { Metric = "Response Time", Value = "2.1 hrs", Target = "2.5 hrs", Status = "Above Target" },
                new ReportMetric { Metric = "Completion Rate", Value = "96%", Target = "95%", Status = "Above Target" },
                new ReportMetric { Metric = "Customer Satisfaction", Value = "4.2", Target = "4.0", Status = "Above Target" }
            };

            ReportDataGrid.ItemsSource = ReportMetrics;

            // Set current date for demonstration
            GeneratedDateText.Text = DateTime.Now.ToString("dd/MM/yyyy");
            UpdatedDateText.Text = DateTime.Now.ToString("dd/MM/yyyy");
        }

        private void BackToReports_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to Reports page
            if (Application.Current.MainWindow is MainWindow mainWindow)
            {
                Reports reportsPage = new Reports();
                mainWindow.MainContentFrame.Navigate(reportsPage);
            }
        }

        private void PrintReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create print dialog
                PrintDialog printDialog = new PrintDialog();

                if (printDialog.ShowDialog() == true)
                {
                    // Print the current page
                    printDialog.PrintVisual(this, "Report Print");
                    MessageBox.Show("Report sent to printer successfully!", "Print",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing report: {ex.Message}", "Print Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportPDF_Click(object sender, RoutedEventArgs e)
        {
            // Placeholder for PDF export functionality
            MessageBox.Show("PDF export functionality will be implemented here.\n\nThis would typically:\n• Convert the report to PDF format\n• Save to user-selected location\n• Show save confirmation",
                "Export PDF", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void EmailReport_Click(object sender, RoutedEventArgs e)
        {
            // Placeholder for email functionality
            MessageBox.Show("Email report functionality will be implemented here.\n\nThis would typically:\n• Open email client\n• Attach report as PDF\n• Pre-fill subject and recipient",
                "Email Report", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    public class ReportMetric
    {
        public string Metric { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}