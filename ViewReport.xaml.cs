using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation; // For BackToReports_Click if using NavigationService.GoBack()
using System.IO;                // For Path, FileAccess
using System.Windows.Xps.Packaging; // For XpsDocument (if you had a separate XPS export)
using System.Windows.Xps;           // For XpsDocumentWriter (if you had a separate XPS export)
using Microsoft.Win32;          // For SaveFileDialog
using System.Windows.Media;         // For Visual (needed for PrintVisual)
using QuestPDF.Fluent;          // For QuestPDF
using QuestPDF.Helpers;         // For QuestPDF Colors, PageSize, Unit
using QuestPDF.Infrastructure;    // For QuestPDF Unit, LicenseType
using System.Linq;              // For .Any()

namespace Adminn
{
    public partial class ViewReport : Page
    {
        public ObservableCollection<ReportMetric> ReportMetrics { get; set; }

        // Example property for report type, you'll likely pass this in or load it
        public string CurrentReportType { get; set; } = "Sample Performance Report";
        public string CurrentReportStatus { get; set; } = "Completed";

        public ViewReport() // Consider passing actual report data/ID here  
        {
            InitializeComponent();
            // Initialize the ReportMetrics property to avoid nullability issues  
            ReportMetrics = [];
            LoadSampleReportData();
            DataContext = this;
        }

        // Overload constructor to accept report data (example)
        public ViewReport(object reportToView) : this() // Calls the default constructor first
        {
            // TODO: Use 'reportToView' to load specific data into:
            // ReportTypeText.Text
            // GeneratedDateText.Text
            // StatusText.Text
            // UpdatedDateText.Text
            // ReportMetrics (for the DataGrid)

            // Example: if (reportToView is YourActualReportObjectClass actualReport)
            // {
            //     ReportTypeText.Text = actualReport.Type;
            //     GeneratedDateText.Text = actualReport.GeneratedDate.ToString("dd/MM/yyyy");
            //     StatusText.Text = actualReport.Status;
            //     UpdatedDateText.Text = actualReport.LastUpdate.ToString("dd/MM/yyyy");
            //     // Load actualReport.Metrics into ReportMetrics collection
            // }
        }

        private void LoadSampleReportData()
        {
            // Initialize sample data for the report summary
            if (ReportTypeText != null) ReportTypeText.Text = CurrentReportType;
            if (StatusText != null) StatusText.Text = CurrentReportStatus;
            if (GeneratedDateText != null) GeneratedDateText.Text = DateTime.Now.ToString("dd/MM/yyyy");
            if (UpdatedDateText != null) UpdatedDateText.Text = DateTime.Now.AddDays(-1).ToString("dd/MM/yyyy");


            // Initialize sample data for the metrics DataGrid
            ReportMetrics = new ObservableCollection<ReportMetric>
            {
                new ReportMetric { Metric = "Productivity Index", Value = "85%", Target = "80%", Status = "Exceeds Target" },
                new ReportMetric { Metric = "Task Efficiency", Value = "92%", Target = "90%", Status = "Exceeds Target" },
                new ReportMetric { Metric = "Quality Assurance Score", Value = "4.3/5", Target = "4.0/5", Status = "Meets Target" },
                new ReportMetric { Metric = "Average Response Time", Value = "2.1 hrs", Target = "2.5 hrs", Status = "Below Target (Good)" },
                new ReportMetric { Metric = "Project Completion Rate", Value = "96%", Target = "95%", Status = "Exceeds Target" }
            };

            if (ReportDataGrid != null)
            {
                ReportDataGrid.ItemsSource = ReportMetrics;
            }
        }

        private void BackToReports_Click(object sender, RoutedEventArgs e)
        {
            // Navigate back to Reports page
            if (Application.Current.MainWindow is MainWindow mainWindow && mainWindow.MainContentFrame.CanGoBack)
            {
                mainWindow.MainContentFrame.GoBack();
            }
            // Fallback or alternative if GoBack isn't suitable:
            // else if (Application.Current.MainWindow is MainWindow mw)
            // {
            //     Reports reportsPage = new Reports();
            //     mw.MainContentFrame.Navigate(reportsPage);
            // }
        }

        private void PrintReport_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                PrintDialog printDialog = new PrintDialog();

                if (printDialog.ShowDialog() == true)
                {
                    if (ReportContentPanel != null)
                    {
                        printDialog.PrintVisual(ReportContentPanel, "Report Print");
                        MessageBox.Show("Report sent to printer successfully!", "Print",
                            MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                    else
                    {
                        MessageBox.Show("Report content not found to print.", "Print Error",
                           MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error printing report: {ex.Message}", "Print Error",
                    MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ExportReport_Click(object sender, RoutedEventArgs e)
        {
            if (ReportContentPanel == null)
            {
                MessageBox.Show("Report content is not available for export.", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog
            {
                Filter = "PDF Document (*.pdf)|*.pdf",
                Title = "Export Report as PDF",
                FileName = $"Report_{(ReportTypeText?.Text ?? "Untitled").Replace(" ", "")}_{DateTime.Now:yyyyMMdd}.pdf"
            };

            if (saveFileDialog.ShowDialog() == true)
            {
                try
                {
                    // For QuestPDF, ensure license is set if required by your usage.
                    // Example for eligible community/open-source projects (place in App.xaml.cs or similar startup location):
                    // QuestPDF.Settings.License = LicenseType.Community; 

                    Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Margin(2, Unit.Centimetre);

                            page.Header()
                                .AlignCenter()
                                .Text("Report: " + (ReportTypeText?.Text ?? "N/A"))
                                .SemiBold().FontSize(18).FontColor(QuestPDF.Helpers.Colors.Black); // Fully Qualified

                            page.Content()
                                .PaddingTop(1, Unit.Centimetre)
                                .Column(col =>
                                {
                                    col.Item().Text("Report Summary").Bold().FontSize(14);
                                    col.Item().Text($"Generated On: {GeneratedDateText?.Text ?? "N/A"}");
                                    col.Item().Text($"Status: {StatusText?.Text ?? "N/A"}");
                                    col.Item().Text($"Last Updated: {UpdatedDateText?.Text ?? "N/A"}");
                                    col.Spacing(20);

                                    if (ReportMetrics != null && ReportMetrics.Any())
                                    {
                                        col.Item().Text("Performance Metrics").Bold().FontSize(14);
                                        col.Spacing(5);
                                        col.Item().Table(table =>
                                        {
                                            table.ColumnsDefinition(columns =>
                                            {
                                                columns.RelativeColumn(3);
                                                columns.RelativeColumn(2);
                                                columns.RelativeColumn(2);
                                                columns.RelativeColumn(2);
                                            });

                                            table.Header(header =>
                                            {
                                                header.Cell().BorderBottom(1).Padding(5).Text("Metric").SemiBold();
                                                header.Cell().BorderBottom(1).Padding(5).Text("Value").SemiBold();
                                                header.Cell().BorderBottom(1).Padding(5).Text("Target").SemiBold();
                                                header.Cell().BorderBottom(1).Padding(5).Text("Status").SemiBold();
                                            });

                                            foreach (var metric in ReportMetrics)
                                            {
                                                table.Cell().BorderBottom(0.5f).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(5).Text(metric.Metric); // Fully Qualified
                                                table.Cell().BorderBottom(0.5f).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(5).Text(metric.Value);  // Fully Qualified
                                                table.Cell().BorderBottom(0.5f).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(5).Text(metric.Target); // Fully Qualified
                                                table.Cell().BorderBottom(0.5f).BorderColor(QuestPDF.Helpers.Colors.Grey.Lighten2).Padding(5).Text(metric.Status); // Fully Qualified
                                            }
                                        });
                                    }
                                });

                            page.Footer()
                                .AlignCenter()
                                .Text(x =>
                                {
                                    x.Span("Page ");
                                    x.CurrentPageNumber();
                                    x.Span(" of ");
                                    x.TotalPages();
                                });
                        });
                    })
                    .GeneratePdf(saveFileDialog.FileName);

                    MessageBox.Show($"Report exported successfully as PDF to:\n{saveFileDialog.FileName}", "Export Successful",
                        MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error exporting report to PDF: {ex.Message}\n\nMake sure you have installed the QuestPDF NuGet package and set any necessary license information (e.g., QuestPDF.Settings.License = LicenseType.Community; in App.xaml.cs for eligible projects).", "Export Error",
                        MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }

    public class ReportMetric // This class definition should ideally be in its own file
    {
        public string Metric { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public string Target { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
    }
}