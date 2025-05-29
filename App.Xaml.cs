using System;
using System.Diagnostics;
using System.Windows;
using DotNetEnv; // Ensure DotNetEnv NuGet package is installed in THIS Dashboard project

// Make sure this namespace matches your Dashboard project's root namespace
// and the 'x:Class' attribute in App.xaml (e.g., "Dashboard.App")
namespace Dashboard
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application // 'App' should match the class name in x:Class
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Dashboard/App.xaml.cs - OnStartup() called. Attempting to load .env file.");

            try
            {
                // Env.Load() will search for the .env file starting from this application's
                // current working directory (usually bin\Debug\netX.X for the startup project)
                // and traverse upwards to find it in the project root.
                Env.Load(); // Ensure .env file is in Dashboard project root & "Copy to Output" is set.
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Dashboard/App.xaml.cs - Env.Load() executed successfully.");

#if DEBUG
                string? testConnectionString = Environment.GetEnvironmentVariable("PRIMETECH_DB_CONN_STRING");
                if (string.IsNullOrEmpty(testConnectionString))
                {
                    Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] WARNING: Dashboard/App.xaml.cs - PRIMETECH_DB_CONN_STRING is NULL or EMPTY after Env.Load(). " +
                                    "Check .env file in Dashboard project root: content, name, 'Copy to Output Directory' property.");
                    MessageBox.Show("DEBUG MODE (Dashboard Project):\n\nPRIMETECH_DB_CONN_STRING was NOT loaded from the .env file located in the Dashboard project's root.\n\n" +
                                    "Please verify:\n" +
                                    "1. '.env' file exists in Dashboard project root.\n" +
                                    "2. It contains the PRIMETECH_DB_CONN_STRING variable.\n" +
                                    "3. Its 'Copy to Output Directory' property is 'Copy if newer'.",
                                    "Env Load Debug Check (Dashboard)", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] INFO: Dashboard/App.xaml.cs - PRIMETECH_DB_CONN_STRING was successfully loaded by Dashboard project.");
                }
#endif
            }
            catch (System.IO.FileNotFoundException fnfEx)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] CRITICAL ERROR: Dashboard/App.xaml.cs - .env file not found. Exception: {fnfEx.ToString()}");
                MessageBox.Show($"CRITICAL CONFIGURATION ERROR (Dashboard Project):\nThe '.env' file was not found. This file is essential for application settings (like database connection strings).\n\n" +
                                $"Please ensure a file named '.env' exists in the 'Dashboard' project's root directory and its 'Copy to Output Directory' property is correctly set in Visual Studio.\n\n" +
                                $"Details: {fnfEx.Message}",
                                "Configuration File Missing (Dashboard)", MessageBoxButton.OK, MessageBoxImage.Error);
                // Optional: Consider shutting down if .env is absolutely critical
                // if (Current != null) Current.Shutdown(-1); 
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] CRITICAL ERROR: Dashboard/App.xaml.cs - Failed to load .env file. Exception: {ex.ToString()}");
                MessageBox.Show($"CRITICAL CONFIGURATION ERROR (Dashboard Project):\nAn error occurred while loading settings from the .env file.\n\n" +
                                $"Details: {ex.Message}\n\n" +
                                "The application might not function correctly.",
                                "Configuration Load Error (Dashboard)", MessageBoxButton.OK, MessageBoxImage.Error);
                // if (Current != null) Current.Shutdown(-1);
            }

            // If you are NOT using StartupUri in App.xaml, you would create and show your first window here.
            // For example:
            // MainWindow mainWindow = new MainWindow(); // Or your LoginWindow
            // mainWindow.Show();
            // If you ARE using StartupUri in App.xaml, this part (creating and showing window) is not needed here.
        }
    }
}
