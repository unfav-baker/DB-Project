using System; // Add this if Environment.GetEnvironmentVariable is not found
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Windows;
using DotNetEnv;

namespace Adminn
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        // Keep only ONE OnStartup method
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: App.xaml.cs - OnStartup() called. Attempting to load .env file.");

            try
            {
                // Env.Load() will search for the .env file starting from the application's
                // current working directory and traverse upwards to find it in the project root.
                // Ensure your .env file is in the root of your project (alongside .csproj)
                // and its "Copy to Output Directory" property is set to "Copy if newer" or "Copy always"
                // if you encounter issues with it not being found during runtime.
                Env.Load();
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: App.xaml.cs - Env.Load() executed successfully.");

                // Optional: Verify if a specific critical variable was loaded (for debugging)
#if DEBUG // This code only runs in Debug builds
                string? testConnectionString = Environment.GetEnvironmentVariable("PRIMETECH_DB_CONN_STRING");
                if (string.IsNullOrEmpty(testConnectionString))
                {
                    Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] WARNING: App.xaml.cs - PRIMETECH_DB_CONN_STRING is NULL or EMPTY after Env.Load(). " +
                                    "This might indicate the variable is missing in the .env file or the file wasn't found/read correctly.");
                    // You could show a MessageBox here for debug builds if this variable is absolutely critical
                    // MessageBox.Show("DEBUG: PRIMETECH_DB_CONN_STRING was not loaded from .env file!", "Env Load Check", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] INFO: App.xaml.cs - PRIMETECH_DB_CONN_STRING was successfully loaded from .env file.");
                }
#endif
            }
            catch (System.IO.FileNotFoundException fnfEx)
            {
                // Specific exception if .env file is not found by DotNetEnv
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] CRITICAL ERROR: App.xaml.cs - .env file not found by DotNetEnv.Env.Load(). Exception: {fnfEx.ToString()}");
                MessageBox.Show($"CRITICAL CONFIGURATION ERROR:\nThe .env file was not found. This file is required to load application settings (like database connection strings).\n\n" +
                                $"Please ensure a '.env' file exists in the project root directory.\n\n" +
                                $"Details: {fnfEx.Message}",
                                "Configuration File Missing", MessageBoxButton.OK, MessageBoxImage.Error);
                // Consider shutting down the application if .env is critical for operation
                // Current.Shutdown(-1); // Example of shutting down
            }
            catch (Exception ex) // Catch any other exceptions during .env loading
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] CRITICAL ERROR: App.xaml.cs - Failed to load .env file. Exception: {ex.ToString()}");
                MessageBox.Show($"CRITICAL CONFIGURATION ERROR:\nAn error occurred while trying to load settings from the .env file.\n\n" +
                                $"Details: {ex.Message}\n\n" +
                                "The application might not function correctly without these settings.",
                                "Configuration Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // Consider shutting down
                // Current.Shutdown(-1);
            }
        }

        // Make sure there are NO OTHER OnStartup methods below this line in this class.
    }
}