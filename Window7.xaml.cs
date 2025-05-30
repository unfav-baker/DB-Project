using System.Diagnostics;
using System.IO;
using System.Windows;
using MySql.Data.MySqlClient;
// using System.Configuration; // This was in your 'using' list but not used for environment variables. Keeping it commented.
using System; // For DateTime
using System.Windows.Controls; // For Button

namespace Dashboard
{
    public partial class Window7 : Window
    {
        // For Environment Variable
        private readonly string? _connectionString;
        private const string DbConnectionStringEnvVar = "PRIMETECH_DB_CONN_STRING"; // Consistent environment variable name

        public Window7()
        {
            InitializeComponent();
            LoadSavedCredentials(); // Your existing method

            _connectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVar);

            // Assuming your Sign In button in Window7.xaml has x:Name="btnSignIn"
            // If it has a different name, please change "btnSignIn" below.
            Button? signInButton = this.FindName("btnSignIn") as Button;

            if (string.IsNullOrEmpty(_connectionString))
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] WARNING: Window7.xaml.cs - Constructor - Database connection string environment variable '{DbConnectionStringEnvVar}' not found or empty. Sign-in will fail.");
                MessageBox.Show($"Database connection string ('{DbConnectionStringEnvVar}') is not configured. Please set the environment variable.\n\nSign-in functionality will be affected.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (signInButton != null)
                {
                    signInButton.IsEnabled = false;
                }
            }
            else
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] INFO: Window7.xaml.cs - Constructor - Database connection string loaded successfully from '{DbConnectionStringEnvVar}'.");
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
            // Path and logic kept as per your provided code (uses rememberme2.txt)
            string filePath = @"C:\Users\SilentWishMAFA\Documents\Database Project\rememberme2.txt";

            if (File.Exists(filePath))
            {
                string[] lines = File.ReadAllLines(filePath);
                if (lines.Length >= 2)
                {
                    // Ensure these XAML controls exist and are correctly named in Window7.xaml
                    if (EmailInput != null) EmailInput.Text = lines[0];
                    // Assuming PasswordInput is a TextBox. If PasswordBox, use .Password
                    if (PasswordInput != null) PasswordInput.Text = lines[1];
                    if (RememberMeCheckbox != null) RememberMeCheckbox.IsChecked = true;
                }
            }
            isLoaded = true;
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // Logic kept as per your provided code
            if (isLoaded)
            {
                userCheckedRemember = true;
            }
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // Path and logic kept as per your provided code (uses rememberme2.txt)
            string filePath = @"C:\Users\SilentWishMAFA\Documents\Database Project\rememberme2.txt";
            if (File.Exists(filePath))
                File.Delete(filePath);
        }

        private void ReturnToPreviousPage(object sender, RoutedEventArgs e)
        {
            // Navigation logic kept as per your provided code
            Window3_c mainWindow = new(); // Navigates to Window3_c
            mainWindow.Show();
            this.Close();
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            // Navigation logic kept as per your provided code
            var newWindow = new Window4_c { WindowState = this.WindowState }; // Navigates to Window4_c
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
            // Assuming PasswordInput is a TextBox. If PasswordBox, use PasswordInput.Password
            string password = PasswordInput.Text.Trim();
            string filePath = @"C:\Users\SilentWishMAFA\Documents\Database Project\rememberme2.txt"; // Kept as per your provided code

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

                                // Remember Me logic kept as per your provided code (uses rememberme2.txt)
                                if (RememberMeCheckbox.IsChecked == true)
                                {
                                    File.WriteAllText(filePath, $"{email}\n{password}"); // WARNING: Plain text credentials
                                }
                                else if (File.Exists(filePath))
                                {
                                    File.Delete(filePath);
                                }

                                // Role-based access logic kept as per your provided code
                                if (dbEmail == "abubakerbarkat45@gmail.com")
                                {
                                    MessageBox.Show($"Welcome, {dbUserName}!", "Login Successful", MessageBoxButton.OK, MessageBoxImage.Information);
                                    Supplier.MainWindow supplierWindow = new(); // Navigates to Supplier.MainWindow
                                    supplierWindow.Show();
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
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] MySQL Error during sign in (Window7): {myEx.ToString()}");
                MessageBox.Show($"Error connecting to database (MySQL):\n{myEx.Message}", "Database Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Generic error during sign in (Window7): {ex.ToString()}");
                MessageBox.Show($"An unexpected error occurred:\n{ex.Message}", "Application Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ForgotPassword_Click(object sender, RoutedEventArgs e)
        {
            // Navigation logic kept as per your provided code
            var newWindow = new FP_7 { WindowState = this.WindowState }; // Navigates to FP_7
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
            // Logic kept as per your provided code (uses rememberme1.txt - inconsistent with other parts of this file)
            string filePath = @"C:\Users\SilentWishMAFA\Documents\Database Project\rememberme1.txt";
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