using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Runtime.CompilerServices;
using System.Text; // For StringBuilder
using System.Windows;
using System.Windows.Controls;
using MySql.Data.MySqlClient;

namespace Adminn
{
    public partial class Export_Orders : Page
    {
        public ObservableCollection<OrderData> Orders { get; set; } // Public property is PascalCase

        private readonly string connectionString = "Server=127.0.0.1;Port=3306;Database=prime_tech;Uid=root;Pwd=Abubaker85@@;";

        public Export_Orders()
        {
            InitializeComponent();
            Orders = new ObservableCollection<OrderData>(); // Initialize PascalCase property
            LoadOrderDataFromDatabase();
            DataContext = this;
        }

        private void ViewOrdersButton_Click(object sender, RoutedEventArgs e)
        {
            LoadOrderDataFromDatabase();
            MessageBox.Show("Order data refreshed!", "Refresh Complete", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void ViewSelectedOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is OrderData selectedOrder)
            {
                StringBuilder orderDetailsBuilder = new StringBuilder();

                orderDetailsBuilder.AppendLine($"        Order Details - ID: {selectedOrder.OrderId}       \n ");
                orderDetailsBuilder.AppendLine("-------------------------------------------------\n\n");
                orderDetailsBuilder.AppendLine($"1:   Customer Name:    {selectedOrder.CustomerName}\n");
                orderDetailsBuilder.AppendLine($"2:   Product ID:       {selectedOrder.ProductId}\n");
                orderDetailsBuilder.AppendLine($"3:   Quantity Ordered: {selectedOrder.QuantityOrdered} \n");
                orderDetailsBuilder.AppendLine($"4:   Order Date:       {selectedOrder.OrderDate:dd/MM/yyyy} \n");
                orderDetailsBuilder.AppendLine($"5:   Delivery Date:    {selectedOrder.DeliveryDate:dd/MM/yyyy} \n");
                orderDetailsBuilder.AppendLine($"6:   Order Status:     {selectedOrder.OrderStatus}  \n");
                orderDetailsBuilder.AppendLine($"7:   Total Price:      {selectedOrder.TotalPrice:C}  \n");
                orderDetailsBuilder.AppendLine($"8:   Payment Status:   {selectedOrder.PaymentStatus}  \n");
                orderDetailsBuilder.AppendLine($"9:   Supplier ID:      {(selectedOrder.SupplierId.HasValue ? selectedOrder.SupplierId.Value.ToString() : "N/A")}\n");

                MessageBox.Show(orderDetailsBuilder.ToString(),
                                $"Details for Order ID: {selectedOrder.OrderId}",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select an order to view its details.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void EditOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is OrderData selectedOrder)
            {
                MessageBox.Show($"Edit functionality for Order ID: {selectedOrder.OrderId}\n(This would typically navigate to an Edit Order page)", "Edit Order Action", MessageBoxButton.OK, MessageBoxImage.Information);
            }
            else
            {
                MessageBox.Show("Please select an order to edit.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void DeleteOrder_Click(object sender, RoutedEventArgs e)
        {
            if (OrdersDataGrid.SelectedItem is OrderData selectedOrder)
            {
                var result = MessageBox.Show($"Are you sure you want to delete Order ID: {selectedOrder.OrderId} for '{selectedOrder.CustomerName}'?",
                                              "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);

                if (result == MessageBoxResult.Yes)
                {
                    DeleteOrderFromDatabase(selectedOrder.OrderId);
                }
            }
            else
            {
                MessageBox.Show("Please select an order to delete.", "No Selection", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }

        private void CreateOrderReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = @"SELECT
                                COUNT(*) as TotalOrders,
                                SUM(Quantity_Ordered) as TotalQuantityOrdered,
                                AVG(Total_Price) as AverageTotalPrice,
                                COUNT(CASE WHEN Order_Status = 'Delivered' THEN 1 END) as DeliveredCount,
                                COUNT(CASE WHEN Order_Status = 'Processing' THEN 1 END) as ProcessingCount,
                                COUNT(CASE WHEN Order_Status = 'Shipped' THEN 1 END) as ShippedCount
                                FROM orders";

                using var command = new MySqlCommand(query, connection);
                using var reader = command.ExecuteReader();

                if (reader.Read())
                {
                    StringBuilder reportBuilder = new();
                    reportBuilder.AppendLine("📊 ORDERS REPORT");
                    reportBuilder.AppendLine("----------------------------------------------------------");
                    reportBuilder.AppendLine($"Generated on: {DateTime.Now:dd/MM/yyyy HH:mm}");
                    reportBuilder.AppendLine();
                    reportBuilder.AppendLine($"Total Orders:             {reader.GetInt32("TotalOrders")}");
                    reportBuilder.AppendLine($"Total Quantity Ordered:   {reader.GetInt32("TotalQuantityOrdered")}");
                    reportBuilder.AppendLine($"Average Total Price:      {reader.GetDecimal("AverageTotalPrice"):C}");
                    reportBuilder.AppendLine();
                    reportBuilder.AppendLine("ORDER STATUS BREAKDOWN:");
                    reportBuilder.AppendLine($"  ✅ Delivered:    {reader.GetInt32("DeliveredCount")}");
                    reportBuilder.AppendLine($"  🔄 Processing:   {reader.GetInt32("ProcessingCount")}");
                    reportBuilder.AppendLine($"  🚚 Shipped:      {reader.GetInt32("ShippedCount")}");

                    MessageBox.Show(reportBuilder.ToString(), "Orders Report", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error generating report: {ex.Message}", "Report Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadOrderDataFromDatabase()
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = @"SELECT Order_ID, Customer_Name, Product_ID, Quantity_Ordered,
                                 Order_Date, Delivery_Date, Order_Status, Total_Price, Payment_Status, Supplier_ID
                                 FROM orders ORDER BY Order_ID DESC";

                using var command = new MySqlCommand(query, connection);
                using var reader = command.ExecuteReader();

                // It's crucial that 'Orders' is the same instance the DataGrid is bound to.
                // If 'Orders' was already initialized in the constructor, Clear and Add is correct.
                Orders.Clear();
                while (reader.Read())
                {
                    var order = new OrderData
                    {
                        IsSelected = false,
                        OrderId = reader.GetInt32("Order_ID"),
                        CustomerName = reader.IsDBNull("Customer_Name") ? string.Empty : reader.GetString("Customer_Name"),
                        ProductId = reader.IsDBNull("Product_ID") ? 0 : reader.GetInt32("Product_ID"),
                        QuantityOrdered = reader.IsDBNull("Quantity_Ordered") ? 0 : reader.GetInt32("Quantity_Ordered"),
                        OrderDate = reader.IsDBNull("Order_Date") ? DateTime.MinValue : reader.GetDateTime("Order_Date"),
                        DeliveryDate = reader.IsDBNull("Delivery_Date") ? DateTime.MinValue : reader.GetDateTime("Delivery_Date"),
                        OrderStatus = reader.IsDBNull("Order_Status") ? string.Empty : reader.GetString("Order_Status"),
                        TotalPrice = reader.IsDBNull("Total_Price") ? 0m : reader.GetDecimal("Total_Price"),
                        PaymentStatus = reader.IsDBNull("Payment_Status") ? string.Empty : reader.GetString("Payment_Status"),
                        SupplierId = reader.IsDBNull("Supplier_ID") ? (int?)null : reader.GetInt32("Supplier_ID")
                    };
                    Orders.Add(order);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading order data: {ex.Message}", "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void DeleteOrderFromDatabase(int orderId)
        {
            try
            {
                using var connection = new MySqlConnection(connectionString);
                connection.Open();

                string query = "DELETE FROM orders WHERE Order_ID = @OrderId";

                using var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@OrderId", orderId);

                int rowsAffected = command.ExecuteNonQuery();

                if (rowsAffected > 0)
                {
                    MessageBox.Show("Order deleted successfully!", "Success",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                    LoadOrderDataFromDatabase();
                }
                else
                {
                    MessageBox.Show("Order not found or could not be deleted.", "Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting order: {ex.Message}", "Database Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    public class OrderData : INotifyPropertyChanged
    {
        private bool _isSelected;
        private int _orderId;
        private string _customerName = string.Empty;
        private int _productId;
        private int _quantityOrdered;
        private DateTime _orderDate;
        private DateTime _deliveryDate;
        private string _orderStatus = string.Empty;
        private decimal _totalPrice;
        private string _paymentStatus = string.Empty;
        private int? _supplierId;

        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public int OrderId { get => _orderId; set { _orderId = value; OnPropertyChanged(); } }
        public string CustomerName { get => _customerName; set { _customerName = value ?? string.Empty; OnPropertyChanged(); } }
        public int ProductId { get => _productId; set { _productId = value; OnPropertyChanged(); } }
        public int QuantityOrdered { get => _quantityOrdered; set { _quantityOrdered = value; OnPropertyChanged(); } }
        public DateTime OrderDate { get => _orderDate; set { _orderDate = value; OnPropertyChanged(); } }
        public DateTime DeliveryDate { get => _deliveryDate; set { _deliveryDate = value; OnPropertyChanged(); } }
        public string OrderStatus { get => _orderStatus; set { _orderStatus = value ?? string.Empty; OnPropertyChanged(); } }
        public decimal TotalPrice { get => _totalPrice; set { _totalPrice = value; OnPropertyChanged(); } }
        public string PaymentStatus { get => _paymentStatus; set { _paymentStatus = value ?? string.Empty; OnPropertyChanged(); } }
        public int? SupplierId { get => _supplierId; set { _supplierId = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}