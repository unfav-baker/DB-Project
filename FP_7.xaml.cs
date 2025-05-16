using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Dashboard
{
    /// <summary>
    /// Interaction logic for FP_5.xaml
    /// </summary>
    public partial class FP_7 : Window
    {
        public FP_7()
        {
            InitializeComponent();


        }

        private void ReturnToPreviousPage(object sender, RoutedEventArgs e)
        {
            Window7 mainWindow = new();
            //mainWindow.WindowState = this.WindowState; // Inherit current state (Maximized, Normal, Minimized)
            mainWindow.Show();
            this.Close();
        }


        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            // Simplified object initialization for newWindow  
            var newWindow = new Window7
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
