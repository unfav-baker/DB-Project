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

namespace Supplier
{
    /// <summary>
    /// Interaction logic for LGS.xaml
    /// </summary>
    public partial class LGS : Window
    {
        public LGS()
        {
            InitializeComponent();
        
        }

        private void CloseApplication_Click(object sender, RoutedEventArgs e)
        {
            this.Close();// Optional: close current window
        }

    }
}
