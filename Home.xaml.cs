using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation; // Required for navigation

namespace Supplier // Make sure this namespace matches your project's namespace
{
    /// <summary>
    /// Interaction logic for Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
        }

        private void ViewProductsButton_Click(object sender, RoutedEventArgs e)
        {
            // Assuming your Products page is named "Products.xaml"
            // and is in the same namespace or a referenced one.
            // Make sure the class name for your Products page is "Products".
            Products productsPage = new Products(); // Create an instance of your Products page
            this.NavigationService.Navigate(productsPage);
        }

        // Placeholder for Reports button click (if you added the handler in XAML)
        private void ReportsButton_Click(object sender, RoutedEventArgs e)
        {
            // Example: Navigate to a ReportsPage
             Reports reportsPage = new Reports();
             this.NavigationService.Navigate(reportsPage);
           
        }

        // Placeholder for Orders button click (if you added the handler in XAML)
        private void OrdersButton_Click(object sender, RoutedEventArgs e)
        {
            // Example: Navigate to an OrdersPage
             Export_Orders ordersPage = new Export_Orders();
             this.NavigationService.Navigate(ordersPage);
           
        }
    }
}