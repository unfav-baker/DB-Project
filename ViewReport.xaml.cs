using System;
using System.Windows;
using System.Windows.Controls;
using System.Diagnostics; // For Debug.WriteLine

namespace Employee // Your namespace
{
    public partial class ViewReport : Page
    {
        public ReportItemData? CurrentReport { get; set; } // Make it nullable

        // Default constructor for XAML designer support (optional but good practice)
        public ViewReport()
        {
            InitializeComponent();
            // You could set a design-time DataContext here if needed
            // CurrentReport = new ReportItemData { ReportType = "Sample Report (Design Time)" };
            // this.DataContext = CurrentReport;
        }

        public ViewReport(ReportItemData reportToView)
        {
            InitializeComponent();
            CurrentReport = reportToView;
            this.DataContext = CurrentReport; // Set DataContext to the passed object for binding

            // You can add more logic here to load further details for the report if needed,
            // using CurrentReport.ReportId or other properties.
            // For now, it will display the properties already in ReportItemData.
            if (CurrentReport == null)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ERROR: ViewReport.xaml.cs - reportToView parameter was null.");
                MessageBox.Show("No report data was provided to display.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // Optionally navigate back if no data
                if (this.NavigationService != null && this.NavigationService.CanGoBack)
                {
                    this.NavigationService.GoBack();
                }
            }
            else
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: ViewReport.xaml.cs - Displaying details for Report ID: {CurrentReport.ReportId}");
                // Example of adding a placeholder if ReportItemData doesn't have a full "details" field
                if (CurrentReport != null)
                {
                    // This is just a placeholder. In a real app, you might fetch more details from DB.
                    // CurrentReport.ReportDetailsPlaceholder = $"This is the placeholder detail area for report type '{CurrentReport.ReportType}' generated on {CurrentReport.DateGenerated:D}.";
                }
            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.NavigationService != null && this.NavigationService.CanGoBack)
                {
                    this.NavigationService.GoBack();
                }
                else
                {
                    // Fallback if GoBack is not available (e.g., direct navigation or frame issue)
                    // Try navigating to the main reports list page explicitly
                    Reports reportsPage = new Reports(); // Create a new instance
                    if (Application.Current.MainWindow is MainWindow mainWindow && mainWindow.MainContentFrame != null)
                    {
                        mainWindow.MainContentFrame.Navigate(reportsPage);
                    }
                    else
                    {
                        MessageBox.Show("Cannot navigate back. Main frame not found.", "Navigation Error");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ERROR during BackButton_Click: {ex}");
                MessageBox.Show($"Error navigating back: {ex.Message}", "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
