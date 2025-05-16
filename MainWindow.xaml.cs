
using System.Reflection.Metadata;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Adminn;
// The error CS0246 indicates that the namespace 'ProfileApplication' cannot be found.  
// To fix this, ensure that the assembly containing 'ProfileApplication' is referenced in your project.  
// If 'ProfileApplication' is part of your solution, ensure the project is added as a reference.  

// Step 1: Check if the 'ProfileApplication' project exists in your solution.  
// Step 2: Add a reference to the 'ProfileApplication' project or its compiled DLL.  
// Step 3: Ensure the namespace is correctly spelled and matches the target assembly.  

// Example of adding a project reference in Visual Studio:  
// 1. Right-click on your project in the Solution Explorer.  
// 2. Select "Add" -> "Project Reference..."  
// 3. Check the box for the 'ProfileApplication' project and click OK.  

// If the namespace is still not found, verify that the 'ProfileApplication' assembly contains the required types.

namespace Adminn
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            MainContentFrame.Navigate(new Home());
        }
        


        private void PlaceHolderTextBox_TextChanged(object sender, System.Windows.Controls.TextChangedEventArgs e)
        {
            // You can handle the logic here for when the text changes
            // For example, you could print the new text to the console
            var textBox = sender as TextBox;
            if (textBox != null)
            {
                System.Diagnostics.Debug.WriteLine("Text changed: " + textBox.Text);
            }
        }


        //private void Profile_Click(object sender, RoutedEventArgs e)
        //{
        //    ProfileMenu.Visibility = ProfileMenu.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        //}

        private void NavigationButton_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            if (button == null) return;

            var pageTypeTag = button.Tag?.ToString();
            var currentContent = MainContentFrame.Content?.GetType().Name;

            if (currentContent == pageTypeTag)
            {
                // Toggle off if same button clicked
                MainContentFrame.Content = null;
            }
            else
            {
                // Load new page
                switch (pageTypeTag)
                {
                    case "HomePage":
                        MainContentFrame.Content = new Home();
                        break;
                    case "ProfilePage":
                        MainContentFrame.Content = new Profile();
                        break;
                    case "ReportsPage":
                        MainContentFrame.Content = new Reports();
                        break;
                    case "EmployeePage":
                        MainContentFrame.Content = new Employee();
                        break;
                    case "SupplierPage":
                        MainContentFrame.Content = new Supplier();
                        break;
                    case "ProductsPage":
                        MainContentFrame.Content = new Products();
                        break;
                    case "CustomerPage":
                        MainContentFrame.Content = new Customer();
                        break;
                    case "ExportOrdersPage":
                        MainContentFrame.Content = new Export_Orders();
                        break;
                        // Add more cases as needed
                }
            }
        }

      
        //private void Settings_Click(object sender, RoutedEventArgs e)
        //{
        //    SettingsMenu.Visibility = SettingsMenu.Visibility == Visibility.Visible ? Visibility.Collapsed : Visibility.Visible;
        //}

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void MainContentFrame_Navigated(object sender, NavigationEventArgs e)
        {

        }

        private void Logout_Click(object sender, RoutedEventArgs e)
        {
            // Simplified object initialization for newWindow  
            //var newWindow = new Dashboard.MainWindow 

            //{
            //    WindowState = this.WindowState
            //};

            //if (this.WindowState == WindowState.Normal)
            //{
            //    newWindow.Left = this.Left;
            //    newWindow.Top = this.Top;
            //    newWindow.Width = this.Width;
            //    newWindow.Height = this.Height;
            //}

            //newWindow.Show();
            //this.Close();
        }
    }
}













