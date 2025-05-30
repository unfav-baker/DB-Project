using System.Windows.Controls;

using System.Windows;
using MySql.Data.MySqlClient;
using System.Windows.Media;
using LiveCharts;
using LiveCharts.Wpf;

namespace Adminn
{
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
            LoadRevenueChart();
            LoadSummaryChart();
            LoadDailySalesChart();
          
        }

        private readonly string? connectionString = Environment.GetEnvironmentVariable("PRIMETECH_DB_CONN_STRING");

        private void LoadRevenueChart()
        {
            var series = new SeriesCollection();

            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = @"
        SELECT c.customer_type, COUNT(e.order_id) AS order_count, SUM(e.total_amount) AS total_amount
        FROM EXPORT_ORDER e
        JOIN CUSTOMER c ON e.customer_id = c.customer_id
        WHERE e.order_date >= DATE_SUB(CURDATE(), INTERVAL 12 MONTH)
        GROUP BY c.customer_type";

            using var cmd = new MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                string type = reader.GetString("customer_type");
                double amount = reader.GetDouble("total_amount");
                int count = reader.GetInt32("order_count");

                var pieSeries = new PieSeries
                {
                    Title = $"{type} ({count} orders)",
                    Values = new ChartValues<double> { amount },
                    DataLabels = true,
                    Fill = type switch
                    {
                        "Export" => Brushes.Green,
                        "Local" => Brushes.Orange,
                        "Retail" => Brushes.SkyBlue,
                        _ => Brushes.Gray
                    }
                };

                series.Add(pieSeries);
            }

            RevenuePieChart.Series = series;
        }


        private void LoadSummaryChart()
        {
            var values = new ChartValues<double>();
            var labels = new List<string>();

            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = @"
        SELECT DATE_FORMAT(p.created_at, '%Y-%m') AS month, SUM(p.price * p.quantity) AS total_amount
        FROM PRODUCT p
        WHERE p.created_at >= DATE_SUB(CURDATE(), INTERVAL 12 MONTH)
        GROUP BY month
        ORDER BY month";

            using var cmd = new MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                labels.Add(reader.GetString("month"));
                values.Add(reader.GetDouble("total_amount"));
            }

            SummaryChart.Series = new SeriesCollection
    {
        new LineSeries
        {
            Title = "Monthly Revenue",
            Values = values,
            PointGeometry = DefaultGeometries.Circle,
            PointGeometrySize = 8,
            Stroke = Brushes.DarkBlue,
            Fill = Brushes.LightBlue
        }
    };

            SummaryChart.AxisX.Clear();
            SummaryChart.AxisX.Add(new Axis
            {
                Title = "Month",
                Labels = labels
            });

            SummaryChart.AxisY.Clear();
            SummaryChart.AxisY.Add(new Axis
            {
                Title = "Amount",
                LabelFormatter = value => $"${value:N2}"
            });
        }

        private void LoadDailySalesChart()
        {
            var values = new ChartValues<double>();

            // Create labels for last 14 days (including today)
            var labels = Enumerable.Range(0, 14)
                .Select(i => DateTime.Today.AddDays(-13 + i).ToString("MM/dd"))
                .ToList();

            // Prepare a dictionary with all days initialized to zero
            var salesDict = labels.ToDictionary(label => label, label => 0.0);

            using var conn = new MySqlConnection(connectionString);
            conn.Open();

            string query = @"
        SELECT DATE(p.created_at) AS day, SUM(p.price * p.quantity) AS total_amount
        FROM PRODUCT p
        WHERE p.created_at >= DATE_SUB(CURDATE(), INTERVAL 14 DAY)
        GROUP BY day
        ORDER BY day";

            using var cmd = new MySqlCommand(query, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                var day = reader.GetDateTime("day").ToString("MM/dd");
                var amount = reader.GetDouble("total_amount");

                if (salesDict.ContainsKey(day))
                    salesDict[day] = amount;  // Set actual sales amount
            }

            // Add values in order of labels
            foreach (var label in labels)
            {
                values.Add(salesDict[label]);
            }

            DailySalesChart.Series = new SeriesCollection
    {
        new ColumnSeries
        {
            Title = "Daily Sales",
            Values = values,
            Fill = Brushes.Teal
        }
    };

            DailySalesChart.AxisX.Clear();
            DailySalesChart.AxisX.Add(new Axis
            {
                Title = "Day",
                Labels = labels,
                Separator = new LiveCharts.Wpf.Separator { Step = 1, IsEnabled = false }
            });

            DailySalesChart.AxisY.Clear();
            DailySalesChart.AxisY.Add(new Axis
            {
                Title = "Amount",
                LabelFormatter = value => $"${value:N2}"
            });
        }

        private void AddEmployee_Click(object sender, RoutedEventArgs e)
        {
            // Create the Reports page instance
            ViewReport reportsPage = new ViewReport();

            // Navigate to Reports page using NavigationService
            if (this.NavigationService != null)
            {
                this.NavigationService.Navigate(reportsPage);
            }
            else
            {
                MessageBox.Show("Navigation service is not available.");
            }
        }

    }
}
