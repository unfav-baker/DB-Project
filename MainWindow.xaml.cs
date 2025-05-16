using System.Windows;

namespace Dashboard
{
    /// <summary>  
    /// Interaction logic for MainWindow.xaml  
    /// </summary>  
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void GetStarted_Click(object sender, RoutedEventArgs e)
        {
            // Simplified object initialization for newWindow  
            var newWindow = new Window2
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