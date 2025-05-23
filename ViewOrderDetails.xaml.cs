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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Adminn
{
    /// <summary>
    /// Interaction logic for ViewOrderDetails.xaml
    /// </summary>
    public partial class ViewOrderDetails : Page
    {
        public ViewOrderDetails()
        {
            InitializeComponent();
        }

        public ViewOrderDetails(Export_Orders.OrderData selectedOrder)
        {
            SelectedOrder = selectedOrder;
        }

        public Export_Orders.OrderData SelectedOrder { get; }
    }
}
