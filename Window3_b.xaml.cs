using System.Windows;
using System.Windows.Input;


namespace Dashboard
{
    public partial class Window3_b : Window
    {
        public Window3_b()
        {
            InitializeComponent();
        }

        //private void Minimize_Click(object sender, RoutedEventArgs e)
        //{
        //    WindowState = WindowState.Minimized;
        //}

        //private void Maximize_Click(object sender, RoutedEventArgs e)
        //{
        //    WindowState = WindowState == WindowState.Maximized
        //        ? WindowState.Normal
        //        : WindowState.Maximized;
        //}

        //private void Close_Click(object sender, RoutedEventArgs e)
        //{
        //    Close();
        //}

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            Window6 newWindow = new Window6();
            newWindow.Show();
            this.Close();// Optional: close current window
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            Window4_b newWindow = new Window4_b();
            newWindow.Show();
            this.Close();// Optional: close current window
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);
            DragMove();
        }

        private void Facebook_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://www.facebook.com", // Replace with your profile link
                UseShellExecute = true
            });
        }

        private void X_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://x.com",
                UseShellExecute = true
            });
        }

        private void Instagram_Click(object sender, RoutedEventArgs e)
        {
            System.Diagnostics.Process.Start(new System.Diagnostics.ProcessStartInfo
            {
                FileName = "https://instagram.com",
                UseShellExecute = true
            });
        }


        private void ReturnToPreviousPage(object sender, RoutedEventArgs e)
        {
            Window2 mainWindow = new Window2();
            mainWindow.WindowState = this.WindowState; // Inherit current state (Maximized, Normal, Minimized)
            mainWindow.Show();
            this.Close();
        }
    }
}