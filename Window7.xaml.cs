// Add this code to the code-behind file for Window5 (Window5.xaml.cs)
using System.Diagnostics;
using System.IO;
using System.Windows;

namespace Dashboard
{
    public partial class Window7 : Window
    {
        public Window7()
        {
            InitializeComponent();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Add your logic for when the checkbox is checked
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Add your logic for when the checkbox is unchecked
        }

        private void ReturnToPreviousPage(object sender, RoutedEventArgs e)
        {
            Window3_c mainWindow = new()
            {
                //WindowState = this.WindowState // Inherit current state (Maximized, Normal, Minimized)
            };
            mainWindow.Show();
            this.Close();
        }

        private WindowState GetWindowState()
        {
            return WindowState;
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            // Simplified object initialization for newWindow  
            var newWindow = new Window4_c
            {
                WindowState = this.WindowState
            };
          
            if (this.WindowState == WindowState.Normal)
            {
                newWindow.Left = this.Left;
                newWindow.Top = this.Top;
                newWindow.Width = this.Width;
                newWindow.Height = this.Height;
            }

            newWindow.Show();
            this.Close();
        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Create and show Admin window first
                Supplier.MainWindow mainWindow = new();
                var Window = mainWindow;

                Window.Show();

                // Close current window after showing new one
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error opening Admin window: {ex.Message}\n\n{ex.StackTrace}",
                               "Critical Error",
                               MessageBoxButton.OK,
                               MessageBoxImage.Error);
            }
        }
        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            // Simplified object initialization for newWindow  
            var newWindow = new FP_7
            {
                WindowState = this.WindowState
            };

            if (this.WindowState == WindowState.Normal)
            {
                newWindow.Left = this.Left;
                newWindow.Top = this.Top;
                newWindow.Width = this.Width;
                newWindow.Height = this.Height;
            }

            newWindow.Show();
            this.Close();
        }


    }
}