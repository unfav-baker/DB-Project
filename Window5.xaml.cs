using MySql.Data.MySqlClient;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System; // For DateTime, if using Debug.WriteLine with it
using System.Windows.Controls; // For Button

namespace Dashboard
{
    public partial class Window5 : Window
    {
        // For Environment Variable
        private readonly string? _connectionString;
        private const string DbConnectionStringEnvVar = "PRIMETECH_DB_CONN_STRING"; // Use a consistent name

        public Window5()
        {
            InitializeComponent();
            LoadSavedCredentials(); // Your existing method

            _connectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVar);

            // Assuming your Sign In button in Window5.xaml has x:Name="btnSignIn"
            // If it has a different name, please change "btnSignIn" below accordingly.
            Button? signInButton = this.FindName("btnSignIn") as Button;

            if (string.IsNullOrEmpty(_connectionString))
            {
                // Optional: Use Debug.WriteLine for logging if running in Debug mode
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] WARNING: Window5.xaml.cs - Constructor - Database connection string environment variable '{DbConnectionStringEnvVar}' not found or empty. Sign-in will fail.");
                MessageBox.Show($"Database connection string ('{DbConnectionStringEnvVar}') is not configured. Please set the environment variable.\n\nSign-in functionality will be affected.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (signInButton != null)
                {
                    signInButton.IsEnabled = false;
                }
            }
            else
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] INFO: Window5.xaml.cs - Constructor - Database connection string loaded successfully from '{DbConnectionStringEnvVar}'.");
                if (signInButton != null)
                {
                    signInButton.IsEnabled = true;
                }
            }
        }

        private bool isLoaded = false;
        private bool userCheckedRemember = false; // Kept as per your provided code

        private void LoadSavedCredentials()
        {
            string filePath = @"C:\Users\SilentWishMAFA\Documents\Database Project\rememberme.txt"; // Kept as per your provided code
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                if (lines.Length >= 2)
                {
                    // Ensure these XAML controls (EmailInput, PasswordInput, RememberMeCheckbox) exist
                    if (EmailInput != null) EmailInput.Text = lines[0];
                    if (PasswordInput != null) PasswordInput.Text = lines[1]; // Assuming PasswordInput is a TextBox. If PasswordBox, use .Password
                    if (RememberMeCheckbox != null) RememberMeCheckbox.IsChecked = true;
                }
            }
            isLoaded = true;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            if (isLoaded)
            {
                userCheckedRemember = true;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            string filePath = @"C:\Users\SilentWishMAFA\Documents\Database Project\rememberme.txt"; // Kept as per your provided code
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        private void ReturnToPreviousPage(object sender, RoutedEventArgs e)
        {
            Window3 mainWindow = new();
            mainWindow.Show();
            this.Close();
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new Window4 { WindowState = this.WindowState };
            if (this.WindowState == WindowState.Normal)
            {
                newWindow.Left = this.Left;
                newWindow.Top = this.Top;
                newWindow.Width = this.Width;
                newWindow.Height = this.Height;
            }
            newWindow.Show();
            this.Close();
        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            // Check if connection string was loaded successfully
            if (string.IsNullOrEmpty(_connectionString))
            {
                MessageBox.Show("Database connection is not configured. Cannot sign in.", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string email = EmailInput.Text.Trim().ToLower();
            // If PasswordInput is a PasswordBox, use PasswordInput.Password
            // If it's a TextBox as suggested by your LoadSavedCredentials, .Text is fine (but less secure for passwords)
            string password = PasswordInput.Text.Trim();
            string filePath = @"C:\Users\SilentWishMAFA\Documents\Database Project\rememberme.txt";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
            {
                MessageBox.Show("Please enter both Email and Password.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                EmailInput.Clear();
                PasswordInput.Clear(); // Or PasswordInput.Password = ""; if PasswordBox
                return;
            }

            // string connectionString = "server=127.0.0.1;Port=3306;Database=prime_tech;Uid=root;Pwd=Abubaker85@@;"; // REMOVED

            try
            {
                // Use the _connectionString field loaded from environment variable
                using (MySqlConnection connection = new MySqlConnection(_connectionString))
                {
                    connection.Open();
                    string query = "SELECT * FROM login WHERE LOWER(email) = @Email AND password = @Password";
                    using (MySqlCommand cmd = new MySqlCommand(query, connection))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password); // WARNING: Plain text password comparison

                        using (MySqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                string dbEmail = reader["email"].ToString().Trim().ToLower();
                                string dbUserName = reader["user_name"].ToString();

                                if (RememberMeCheckbox.IsChecked == true)
                                {
                                    File.WriteAllText(filePath, $"{email}\n{password}"); // WARNING: Plain text credentials
                                }
                                else if (File.Exists(filePath))
                                {
                                    File.Delete(filePath);
                                }

                                if (dbEmail == "manakhanoffical@gmail.com")
                                {
                                    MessageBox.Show($"Welcome, {dbUserName}!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information); // Kept as per your original code
                                    Adminn.MainWindow adminWindow = new();
                                    adminWindow.Show();
                                    this.Close();
                                }
                                else
                                {
                                    MessageBox.Show("Access denied. You are not allowed to log in here.", "Access Denied", MessageBoxButton.OK, MessageBoxImage.Warning);
                                    EmailInput.Clear();
                                    PasswordInput.Clear(); // Or PasswordInput.Password = "";
                                }
                            }
                            else
                            {
                                MessageBox.Show("Incorrect Email or Password.", "Login Failed", MessageBoxButton.OK, MessageBoxImage.Error);
                                EmailInput.Clear();
                                PasswordInput.Clear(); // Or PasswordInput.Password = "";
                            }
                        }
                    }
                }
            }
            catch (MySqlException myEx) // More specific exception first
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] MySQL Error during sign in (Window5): {myEx.ToString()}");
                MessageBox.Show($"Error connecting to database (MySQL):\n{myEx.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Generic error during sign in (Window5): {ex.ToString()}");
                MessageBox.Show($"An unexpected error occurred:\n{ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new FP_5 { WindowState = this.WindowState };
            if (this.WindowState == WindowState.Normal)
            {
                newWindow.Left = this.Left;
                newWindow.Top = this.Top;
                newWindow.Width = this.Width;
                newWindow.Height = this.Height;
            }
            newWindow.Show();
            this.Close();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            string filePath = @"C:\Users\SilentWishMAFA\Documents\Database Project\rememberme.txt";
            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                if (lines.Length >= 2)
                {
                    if (EmailInput != null) EmailInput.Text = lines[0].Trim();
                    if (PasswordInput != null) PasswordInput.Text = lines[1].Trim(); // Assuming TextBox
                    if (RememberMeCheckbox != null) RememberMeCheckbox.IsChecked = true;
                }
            }
        }
    }
}