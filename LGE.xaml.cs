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

namespace Employee
{
    /// <summary>
    /// Interaction logic for LGE.xaml
    /// </summary>
    public partial class LGE : Window
    {
        public LGE()
        {
            InitializeComponent();
        }
        private void CloseApplication_Click(object sender, RoutedEventArgs e)
        {
            this.Close();// Optional: close current window
        }


    }

}
