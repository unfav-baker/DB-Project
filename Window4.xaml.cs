using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MySql.Data.MySqlClient;
using System.Diagnostics; // For Debug.WriteLine

namespace Dashboard
{
    /// <summary>
    /// Interaction logic for Window4.xaml
    /// </summary>
    public partial class Window4 : Window
    {
        // Use an environment variable for the connection string
        private readonly string? _connectionString;
        // Using the consistent environment variable name as requested
        private const string DbConnectionStringEnvVar = "PRIMETECH_DB_CONN_STRING";

        public Window4()
        {
            InitializeComponent();

            _connectionString = Environment.GetEnvironmentVariable(DbConnectionStringEnvVar);

            Button? signUpButton = this.FindName("btnSignUp") as Button; // Assuming x:Name="btnSignUp"

            if (string.IsNullOrEmpty(_connectionString))
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] WARNING: Window4.xaml.cs - Constructor - Database connection string environment variable '{DbConnectionStringEnvVar}' not found or empty. Sign-up will fail.");
                MessageBox.Show($"Database connection string ('{DbConnectionStringEnvVar}') is not configured. Please set the environment variable.\n\nSign-up functionality will be affected.",
                                "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                if (signUpButton != null)
                {
                    signUpButton.IsEnabled = false;
                }
            }
            else
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] INFO: Window4.xaml.cs - Constructor - Database connection string loaded successfully from '{DbConnectionStringEnvVar}'.");
                if (signUpButton != null)
                {
                    signUpButton.IsEnabled = true;
                }
            }
        }

        private void ReturnToPreviousPage(object sender, RoutedEventArgs e)
        {
            Window3 mainWindow = new Window3();
            mainWindow.Show();
            this.Close();
        }

        private void SignUp_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_connectionString))
            {
                MessageBox.Show("Database connection is not configured. Cannot complete registration.", "Configuration Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string userName = NameTextBox.Text.Trim();
            string email = EmailTextBox.Text.Trim();
            string password = PasswordTextBox.Text.Trim();
            string confirmPassword = ConfirmPasswordTextBox.Text.Trim();

            if (string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) || string.IsNullOrEmpty(confirmPassword))
            {
                MessageBox.Show("Please fill all fields.");
                return;
            }

            if (!Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$"))
            {
                MessageBox.Show("Invalid email format.");
                return;
            }

            if (password.Length < 6)
            {
                MessageBox.Show("Password must be at least 6 characters.");
                return;
            }

            if (password != confirmPassword)
            {
                MessageBox.Show("Passwords do not match.");
                return;
            }

            try
            {
                using (MySqlConnection conn = new MySqlConnection(_connectionString))
                {
                    conn.Open();

                    string checkQuery = "SELECT COUNT(*) FROM login WHERE email = @Email";
                    MySqlCommand checkCmd = new MySqlCommand(checkQuery, conn);
                    checkCmd.Parameters.AddWithValue("@Email", email);

                    int userExists = Convert.ToInt32(checkCmd.ExecuteScalar());
                    if (userExists > 0)
                    {
                        MessageBox.Show("An account with this email already exists.");
                        return;
                    }

                    string query = "INSERT INTO login (user_name, email, password) VALUES (@UserName, @Email, @Password)";
                    MySqlCommand cmd = new MySqlCommand(query, conn);
                    cmd.Parameters.AddWithValue("@UserName", userName);
                    cmd.Parameters.AddWithValue("@Email", email);
                    cmd.Parameters.AddWithValue("@Password", password); // Remember to hash passwords in real apps

                    int result = cmd.ExecuteNonQuery();
                    if (result > 0)
                    {
                        MessageBox.Show("Registration successful!");
                        Window2 window2 = new Window2();
                        window2.Show();
                        this.Hide();
                    }
                    else
                    {
                        MessageBox.Show("Registration failed. Try again.");
                    }
                }
            }
            catch (MySqlException myEx)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] MySQL Error during sign up (Window4): {myEx.ToString()}");
                MessageBox.Show("Database Error: " + myEx.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[{DateTime.Now:HH:mm:ss.fff}] Generic error during sign up (Window4): {ex.ToString()}");
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private void SignIn_Click(object sender, RoutedEventArgs e)
        {
            var newWindow = new Window5 { WindowState = this.WindowState };
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

        private void ClearFields()
        {
            NameTextBox.Text = "";
            EmailTextBox.Text = "";
            PasswordTextBox.Text = "";
            ConfirmPasswordTextBox.Text = "";
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            NameTextBox.Focus();
        }
    }
}