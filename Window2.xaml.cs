using System.Windows;
using System.Windows.Input;

namespace Dashboard
{
    public partial class Window2 : Window // Partial class
    {
        public Window2()
        {
            InitializeComponent(); // Correct casing
            CenterWindow();
        }

        //private void InitializeComponent()
        //{
        //    throw new NotImplementedException();
        //}

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CenterWindow()
        {
            Left = (SystemParameters.PrimaryScreenWidth - Width) / 2;
            Top = (SystemParameters.PrimaryScreenHeight - Height) / 2;
        }

        private void ReturnToPreviousPage(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.WindowState = this.WindowState; // Inherit current state (Maximized, Normal, Minimized)
            mainWindow.Show();
            this.Close();
        }
        private void Admin_Click(object sender, RoutedEventArgs e)
        {
            // Simplified object initialization for newWindow  
            var newWindow = new Window3
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

        private void Supplier_Click(object sender, RoutedEventArgs e)
        {
            // Simplified object initialization for newWindow  
            var newWindow = new Window3_c
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

        private void Employee_Click(object sender, RoutedEventArgs e)
        {
            // Simplified object initialization for newWindow  
            var newWindow = new Window3_b
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