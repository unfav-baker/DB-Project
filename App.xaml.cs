using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using QuestPDF.Infrastructure; // <-- ADD THIS LINE
using DotNetEnv;
using System.Diagnostics;


namespace Adminn // Your application's namespace
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        public App()
        {
            // Set the QuestPDF license type here
            QuestPDF.Settings.License = LicenseType.Community;
            // If you have a commercial license, use:
            // QuestPDF.Settings.License = LicenseType.Pro; 
            // QuestPDF.Settings.License = LicenseType.Enterprise; 

            // InitializeComponent(); // This might already be here if you have an App_Startup event in App.xaml
            // or it might be implicitly called. If your app worked before,
            // don't worry about adding InitializeComponent() here unless needed.
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            try
            {
                Env.Load();
                // You can keep or remove this Debug.WriteLine, it doesn't show a pop-up.
                Debug.WriteLine("INFO: Env.Load() attempt finished in App.OnStartup.");

                // --- Start of section to clean up ---
                // We've confirmed it works, so we can remove the immediate check & pop-up.
                // You can choose to keep a silent Debug.WriteLine check if you want.
#if DEBUG // This preprocessor directive means the code inside only runs in Debug builds
                string? testConnStringAfterLoad = Environment.GetEnvironmentVariable("PRIMETECH_DB_CONN_STRING");
                if (string.IsNullOrEmpty(testConnStringAfterLoad))
                {
                    Debug.WriteLine("WARNING (App.xaml.cs): PRIMETECH_DB_CONN_STRING is NULL or EMPTY after Env.Load(). Check .env file.");
                }
                else
                {
                    Debug.WriteLine("INFO (App.xaml.cs): PRIMETECH_DB_CONN_STRING was loaded after Env.Load().");
                }
#endif
                // --- End of section to clean up ---
            }
            catch (System.Exception ex) // It's good to keep this catch for critical .env load failures
            {
                Debug.WriteLine($"CRITICAL (App.xaml.cs): Exception during Env.Load(). Error: {ex.ToString()}");
                MessageBox.Show($"CRITICAL: Could not load the .env configuration file due to an EXCEPTION.\n\n" +
                                $"Error details: {ex.Message}\n\n" +
                                $"Please check the .env file existence, permissions, and format.",
                                "Configuration Load Exception", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}