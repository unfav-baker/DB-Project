using System; // Required for Environment, DateTime
using System.Configuration; // Often included by default, may not be strictly needed for this logic
using System.Data; // Often included by default, may not be strictly needed for this logic
using System.Diagnostics; // Required for Debug.WriteLine
using System.Windows;
using DotNetEnv; // Required for loading .env file

// Ensure this namespace matches your project's root namespace (Employee)
namespace Employee
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e); // Call the base class method first

            Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Employee/App.xaml.cs - OnStartup() called. Attempting to load .env file.");

            try
            {
                // Env.Load() will search for the .env file starting from the application's
                // current working directory and traverse upwards to find it in the project root.
                // Key things for .env file:
                // 1. Name: EXACTLY '.env' (no .txt extension).
                // 2. Location: In the root of your C# project (next to your .csproj file for this Employee project if run standalone,
                //    OR in the root of your main 'Dashboard' startup project if this Employee project is a class library).
                // 3. Properties in Visual Studio: Select .env file in Solution Explorer,
                //    set "Copy to Output Directory" to "Copy if newer" or "Copy always".
                Env.Load();
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] DEBUG: Employee/App.xaml.cs - Env.Load() executed successfully.");

                // Optional: Verify if a specific critical variable was loaded (for debugging)
                // This block will only compile and run when you are in a DEBUG build configuration.
#if DEBUG
                string? testConnectionString = Environment.GetEnvironmentVariable("PRIMETECH_DB_CONN_STRING");
                if (string.IsNullOrEmpty(testConnectionString))
                {
                    Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] WARNING: Employee/App.xaml.cs - PRIMETECH_DB_CONN_STRING is NULL or EMPTY after Env.Load(). " +
                                    "This might indicate the variable is missing in the .env file, is misspelled, " +
                                    "the .env file content is incorrect, or the file wasn't found/read correctly by Env.Load().");
                    // You could show a MessageBox here specifically for debug builds if this variable is absolutely critical
                    // MessageBox.Show("DEBUG MODE (Employee Project): PRIMETECH_DB_CONN_STRING was not loaded from .env file! " +
                    //                 "Check .env file content, name, location, and 'Copy to Output Directory' property.", 
                    //                 "Env Load Debug Check", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
                else
                {
                    Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] INFO: Employee/App.xaml.cs - PRIMETECH_DB_CONN_STRING was successfully loaded from .env file (verified in Employee/App.xaml.cs).");
                }
#endif
            }
            catch (System.IO.FileNotFoundException fnfEx)
            {
                // This specific exception is caught if DotNetEnv explicitly cannot find any .env file in its search paths.
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] CRITICAL ERROR: Employee/App.xaml.cs - .env file not found by DotNetEnv.Env.Load(). Exception: {fnfEx.ToString()}");
                MessageBox.Show($"CRITICAL CONFIGURATION ERROR (Employee Project):\nThe '.env' file was not found. This file is required to load application settings (like database connection strings).\n\n" +
                                $"Please ensure a file named exactly '.env' exists in the appropriate project root directory (likely your main 'Dashboard' startup project, or this 'Employee' project if run standalone) " +
                                $"and its 'Copy to Output Directory' property is set correctly in Visual Studio.\n\n" +
                                $"Details: {fnfEx.Message}",
                                "Configuration File Missing", MessageBoxButton.OK, MessageBoxImage.Error);
                // Consider shutting down the application if .env is critical for operation
                // if (Current != null) Current.Shutdown(-1); 
            }
            catch (Exception ex) // Catch any other exceptions during .env loading
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] CRITICAL ERROR: Employee/App.xaml.cs - Failed to load .env file during OnStartup. Exception: {ex.ToString()}");
                MessageBox.Show($"CRITICAL CONFIGURATION ERROR (Employee Project):\nAn error occurred while trying to load settings from the .env file.\n\n" +
                                $"Details: {ex.Message}\n\n" +
                                "The application might not function correctly without these settings.",
                                "Configuration Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
                // Consider shutting down
                // if (Current != null) Current.Shutdown(-1);
            }

            // If you have other application startup logic for the Employee module specifically, it can go here.
        }
    }
}
