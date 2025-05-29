using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation; // Required for NavigationService
using System.Diagnostics; // For Debug.WriteLine

namespace Employee // Your namespace
{
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
        }

        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Home.xaml.cs - ReportsButton_Click called.");
            try
            {
                // Assuming your Reports list page is named 'Reports.xaml'
                // and is in the same 'Employee' namespace.
                Reports reportsPage = new Reports();

                // Navigate using the Page's NavigationService if available
                if (this.NavigationService != null)
                {
                    this.NavigationService.Navigate(reportsPage);
                }
                // Fallback to MainWindow's frame if this page is not directly in a Frame
                // that provides NavigationService, or if you want to ensure navigation
                // happens in a specific main frame.
                // Ensure your MainWindow class is named 'MainWindow' and has a Frame named 'MainContentFrame'
                else if (Application.Current.MainWindow is Employee.MainWindow mainWindow && mainWindow.MainContentFrame != null)
                {
                    // Note: If 'MainWindow' is in a different namespace (e.g., Adminn),
                    // you'll need a using statement for that namespace or qualify it fully.
                    // Example: using Adminn;
                    mainWindow.MainContentFrame.Navigate(reportsPage);
                }
                else
                {
                    MessageBox.Show("Could not find a frame to navigate to the Reports page.",
                                    "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ERROR navigating to Reports page: {ex.ToString()}");
                MessageBox.Show($"Error navigating to Reports: {ex.Message}",
                                "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void GenerateReportsButton_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Home.xaml.cs - GenerateReportsButton_Click called.");
            try
            {
                // Assuming your "Create New Order Entry" page is named 'CreateReportPage.xaml'
                // and is in the same 'Employee' namespace.
                CreateReportPage createReportPage = new CreateReportPage();

                if (this.NavigationService != null)
                {
                    this.NavigationService.Navigate(createReportPage);
                }
                else if (Application.Current.MainWindow is Employee.MainWindow mainWindow && mainWindow.MainContentFrame != null)
                {
                    // Ensure Adminn.MainWindow and its MainContentFrame are accessible
                    mainWindow.MainContentFrame.Navigate(createReportPage);
                }
                else
                {
                    MessageBox.Show("Could not find a frame to navigate to the Create New Order Entry page.",
                                    "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] ERROR navigating to CreateReportPage: {ex.ToString()}");
                MessageBox.Show($"Error navigating to Create New Order Entry: {ex.Message}",
                                "Navigation Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
